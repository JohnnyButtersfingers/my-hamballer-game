#include <eosio/eosio.hpp>
#include <eosio/asset.hpp>
#include <eosio/system.hpp>

using namespace eosio;
using std::string;

class [[eosio::contract("dbp_token")]] dbp_token : public contract {
public:
    using contract::contract;

    /**
     * Create token action
     * @param issuer - Token issuer account
     * @param maximum_supply - Maximum supply with symbol
     */
    [[eosio::action]]
    void create(const name& issuer, const asset& maximum_supply) {
        require_auth(get_self());
        
        auto sym = maximum_supply.symbol;
        check(sym.is_valid(), "invalid symbol name");
        check(maximum_supply.is_valid(), "invalid supply");
        check(maximum_supply.amount > 0, "max-supply must be positive");
        
        stats statstable(get_self(), sym.code().raw());
        auto existing = statstable.find(sym.code().raw());
        check(existing == statstable.end(), "token with symbol already exists");
        
        statstable.emplace(get_self(), [&](auto& s) {
            s.supply.symbol = maximum_supply.symbol;
            s.max_supply    = maximum_supply;
            s.issuer        = issuer;
        });
    }

    /**
     * Issue tokens action
     * @param to - Recipient account
     * @param quantity - Amount to issue
     * @param memo - Transfer memo
     */
    [[eosio::action]]
    void issue(const name& to, const asset& quantity, const string& memo) {
        auto sym = quantity.symbol;
        check(sym.is_valid(), "invalid symbol name");
        check(memo.size() <= 256, "memo has more than 256 bytes");
        
        stats statstable(get_self(), sym.code().raw());
        auto existing = statstable.find(sym.code().raw());
        check(existing != statstable.end(), "token with symbol does not exist");
        const auto& st = *existing;
        
        require_auth(st.issuer);
        check(quantity.is_valid(), "invalid quantity");
        check(quantity.amount > 0, "must issue positive quantity");
        check(quantity.symbol == st.supply.symbol, "symbol precision mismatch");
        check(quantity.amount <= st.max_supply.amount - st.supply.amount, "quantity exceeds available supply");
        
        statstable.modify(st, same_payer, [&](auto& s) {
            s.supply += quantity;
        });
        
        add_balance(st.issuer, quantity, st.issuer);
        
        if (to != st.issuer) {
            SEND_INLINE_ACTION(*this, transfer, {st.issuer, "active"_n},
                              {st.issuer, to, quantity, memo});
        }
    }

    /**
     * Transfer tokens action
     * @param from - Sender account
     * @param to - Recipient account
     * @param quantity - Amount to transfer
     * @param memo - Transfer memo
     */
    [[eosio::action]]
    void transfer(const name& from, const name& to, const asset& quantity, const string& memo) {
        check(from != to, "cannot transfer to self");
        require_auth(from);
        check(is_account(to), "to account does not exist");
        auto sym = quantity.symbol.code();
        stats statstable(get_self(), sym.raw());
        const auto& st = statstable.get(sym.raw());
        
        require_recipient(from);
        require_recipient(to);
        
        check(quantity.is_valid(), "invalid quantity");
        check(quantity.amount > 0, "must transfer positive quantity");
        check(quantity.symbol == st.supply.symbol, "symbol precision mismatch");
        check(memo.size() <= 256, "memo has more than 256 bytes");
        
        auto payer = has_auth(to) ? to : from;
        
        sub_balance(from, quantity);
        add_balance(to, quantity, payer);
    }

    /**
     * Burn tokens action
     * @param owner - Token owner
     * @param quantity - Amount to burn
     */
    [[eosio::action]]
    void burn(const name& owner, const asset& quantity) {
        require_auth(owner);
        
        auto sym = quantity.symbol;
        check(sym.is_valid(), "invalid symbol name");
        
        stats statstable(get_self(), sym.code().raw());
        auto existing = statstable.find(sym.code().raw());
        check(existing != statstable.end(), "token with symbol does not exist");
        const auto& st = *existing;
        
        check(quantity.is_valid(), "invalid quantity");
        check(quantity.amount > 0, "must burn positive quantity");
        check(quantity.symbol == st.supply.symbol, "symbol precision mismatch");
        
        sub_balance(owner, quantity);
        
        statstable.modify(st, same_payer, [&](auto& s) {
            s.supply -= quantity;
        });
    }

    /**
     * Get token balance
     * @param owner - Account to check
     * @param sym - Token symbol
     */
    static asset get_balance(const name& token_contract_account, const name& owner, const symbol_code& sym) {
        accounts accountstable(token_contract_account, owner.value);
        const auto& ac = accountstable.get(sym.raw());
        return ac.balance;
    }

private:
    struct [[eosio::table]] account {
        asset    balance;
        uint64_t primary_key() const { return balance.symbol.code().raw(); }
    };

    struct [[eosio::table]] currency_stats {
        asset    supply;
        asset    max_supply;
        name     issuer;
        uint64_t primary_key() const { return supply.symbol.code().raw(); }
    };

    typedef eosio::multi_index<"accounts"_n, account> accounts;
    typedef eosio::multi_index<"stat"_n, currency_stats> stats;

    void sub_balance(const name& owner, const asset& value) {
        accounts from_acnts(get_self(), owner.value);
        
        const auto& from = from_acnts.get(value.symbol.code().raw(), "no balance object found");
        check(from.balance.amount >= value.amount, "overdrawn balance");
        
        from_acnts.modify(from, owner, [&](auto& a) {
            a.balance -= value;
        });
    }

    void add_balance(const name& owner, const asset& value, const name& ram_payer) {
        accounts to_acnts(get_self(), owner.value);
        auto to = to_acnts.find(value.symbol.code().raw());
        if (to == to_acnts.end()) {
            to_acnts.emplace(ram_payer, [&](auto& a) {
                a.balance = value;
            });
        } else {
            to_acnts.modify(to, same_payer, [&](auto& a) {
                a.balance += value;
            });
        }
    }
};