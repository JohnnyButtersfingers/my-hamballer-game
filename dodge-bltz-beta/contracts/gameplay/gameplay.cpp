#include <eosio/eosio.hpp>
#include <eosio/asset.hpp>
#include <eosio/system.hpp>
#include <eosio/crypto.hpp>
#include <eosio/transaction.hpp>

using namespace eosio;
using std::string;

class [[eosio::contract("gameplay")]] gameplay : public contract {
public:
    using contract::contract;

    // Constants
    static constexpr uint32_t WIN_CHANCE = 35; // 35% chance to win
    static constexpr symbol DBP_SYMBOL = symbol("DBP", 4);
    static constexpr int64_t REWARD_AMOUNT = 10000; // 1.0000 DBP

    /**
     * Play action - Main game entry point
     * @param player - Player account
     * @param nonce - Unique nonce for replay protection
     */
    [[eosio::action]]
    void play(const name& player, const string& nonce) {
        require_auth(player);
        
        // Validate nonce
        check(nonce.length() > 0 && nonce.length() <= 64, "invalid nonce length");
        
        // Check player exists or create new entry
        players_table players(get_self(), get_self().value);
        auto player_itr = players.find(player.value);
        
        if (player_itr == players.end()) {
            players.emplace(player, [&](auto& p) {
                p.player = player;
                p.total_plays = 0;
                p.total_wins = 0;
                p.last_nonce = "";
            });
            player_itr = players.find(player.value);
        }
        
        // Check nonce for replay protection
        check(player_itr->last_nonce != nonce, "nonce already used");
        
        // Update player stats
        players.modify(player_itr, player, [&](auto& p) {
            p.total_plays++;
            p.last_nonce = nonce;
        });
        
        // Get config
        config_table config(get_self(), get_self().value);
        auto cfg = config.get_or_default();
        check(cfg.token_contract != name(), "token contract not set");
        check(cfg.rng_contract != name(), "rng contract not set");
        
        // Generate unique signing value for RNG
        uint64_t signing_value = current_time_point().time_since_epoch().count();
        signing_value ^= player.value;
        signing_value ^= std::hash<string>{}(nonce);
        
        // Store pending RNG request
        pending_table pending(get_self(), get_self().value);
        uint64_t request_id = pending.available_primary_key();
        
        pending.emplace(get_self(), [&](auto& p) {
            p.id = request_id;
            p.player = player;
            p.signing_value = signing_value;
            p.timestamp = current_time_point();
        });
        
        // Request RNG from oracle
        action(
            permission_level{get_self(), "active"_n},
            cfg.rng_contract,
            "requestrand"_n,
            std::make_tuple(request_id, signing_value, get_self())
        ).send();
    }

    /**
     * Receive random value callback from RNG oracle
     * @param request_id - Original request ID
     * @param random_value - Random value from oracle
     */
    [[eosio::action]]
    void receiverand(uint64_t request_id, const checksum256& random_value) {
        // Only RNG oracle can call this
        config_table config(get_self(), get_self().value);
        auto cfg = config.get_or_default();
        require_auth(cfg.rng_contract);
        
        // Find pending request
        pending_table pending(get_self(), get_self().value);
        auto pending_itr = pending.find(request_id);
        check(pending_itr != pending.end(), "request not found");
        
        // Extract player from pending request
        name player = pending_itr->player;
        
        // Remove pending request
        pending.erase(pending_itr);
        
        // Calculate win/loss from random value
        auto byte_array = random_value.extract_as_byte_array();
        uint32_t random_num = (byte_array[0] << 24) | (byte_array[1] << 16) | 
                             (byte_array[2] << 8) | byte_array[3];
        uint32_t result = random_num % 100;
        
        bool won = result < WIN_CHANCE;
        
        // Update player stats
        players_table players(get_self(), get_self().value);
        auto player_itr = players.find(player.value);
        check(player_itr != players.end(), "player not found");
        
        if (won) {
            players.modify(player_itr, same_payer, [&](auto& p) {
                p.total_wins++;
            });
            
            // Issue reward token
            asset reward(REWARD_AMOUNT, DBP_SYMBOL);
            
            action(
                permission_level{cfg.token_contract, "active"_n},
                cfg.token_contract,
                "issue"_n,
                std::make_tuple(player, reward, string("BLTZ win reward"))
            ).send();
        }
        
        // Log result
        action(
            permission_level{get_self(), "active"_n},
            get_self(),
            "logresult"_n,
            std::make_tuple(player, won, result)
        ).send();
    }

    /**
     * Set token contract address
     * @param token_contract - DBP token contract account
     */
    [[eosio::action]]
    void settoken(const name& token_contract) {
        require_auth(get_self());
        check(is_account(token_contract), "token contract does not exist");
        
        config_table config(get_self(), get_self().value);
        auto cfg = config.get_or_default();
        cfg.token_contract = token_contract;
        config.set(cfg, get_self());
    }

    /**
     * Set RNG oracle contract
     * @param rng_contract - WAX RNG oracle account
     */
    [[eosio::action]]
    void setrng(const name& rng_contract) {
        require_auth(get_self());
        check(is_account(rng_contract), "rng contract does not exist");
        
        config_table config(get_self(), get_self().value);
        auto cfg = config.get_or_default();
        cfg.rng_contract = rng_contract;
        config.set(cfg, get_self());
    }

    /**
     * Log game result (inline action for tracking)
     * @param player - Player account
     * @param won - Whether player won
     * @param roll - Random number rolled (0-99)
     */
    [[eosio::action]]
    void logresult(const name& player, bool won, uint32_t roll) {
        require_auth(get_self());
        // This is just for logging/tracking purposes
    }

    /**
     * Clear expired pending requests (maintenance action)
     * @param max_rows - Maximum rows to clear in one transaction
     */
    [[eosio::action]]
    void clearexpired(uint32_t max_rows) {
        require_auth(get_self());
        
        pending_table pending(get_self(), get_self().value);
        auto idx = pending.get_index<"bytimestamp"_n>();
        
        // Consider requests older than 5 minutes as expired
        auto expiry_time = current_time_point() - seconds(300);
        auto itr = idx.begin();
        
        uint32_t count = 0;
        while (itr != idx.end() && count < max_rows) {
            if (itr->timestamp < expiry_time) {
                itr = idx.erase(itr);
                count++;
            } else {
                break; // Remaining entries are not expired
            }
        }
    }

private:
    // Tables
    struct [[eosio::table]] player_stats {
        name     player;
        uint64_t total_plays;
        uint64_t total_wins;
        string   last_nonce;
        
        uint64_t primary_key() const { return player.value; }
    };

    struct [[eosio::table]] pending_rng {
        uint64_t      id;
        name          player;
        uint64_t      signing_value;
        time_point    timestamp;
        
        uint64_t primary_key() const { return id; }
        uint64_t by_timestamp() const { return timestamp.time_since_epoch().count(); }
    };

    struct [[eosio::table]] game_config {
        name token_contract;
        name rng_contract;
    };

    typedef eosio::multi_index<
        "players"_n, 
        player_stats
    > players_table;

    typedef eosio::multi_index<
        "pending"_n, 
        pending_rng,
        indexed_by<"bytimestamp"_n, const_mem_fun<pending_rng, uint64_t, &pending_rng::by_timestamp>>
    > pending_table;

    typedef eosio::singleton<"config"_n, game_config> config_table;
};