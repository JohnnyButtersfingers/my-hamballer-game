#pragma once

#include <eosio/eosio.hpp>
#include <eosio/asset.hpp>
#include <eosio/system.hpp>
#include <eosio/crypto.hpp>

using namespace eosio;

CONTRACT gameplay : public contract {
   public:
      using contract::contract;

      /**
       * Play BLTZ action.
       * 
       * @param player - the player account making the play
       * @param nonce - unique nonce to prevent replay attacks
       */
      ACTION play( const name& player, const uint64_t& nonce );

      /**
       * Callback to receive random number from WAX RNG Oracle.
       * 
       * @param caller_signing_value - signing value from RNG request
       * @param caller_signing_value_hash - hash of signing value
       * @param random_value - random number from oracle
       */
      ACTION receiverand( const uint64_t& caller_signing_value, 
                         const checksum256& caller_signing_value_hash,
                         const uint64_t& random_value );

      /**
       * Initialize contract configuration.
       * 
       * @param token_contract - the DBP token contract account
       */
      ACTION init( const name& token_contract );

   private:
      // Table to store used nonces for replay protection
      TABLE used_nonce {
         uint64_t nonce;
         name player;
         uint32_t timestamp;

         uint64_t primary_key() const { return nonce; }
         uint64_t by_player() const { return player.value; }
      };

      // Table to store pending RNG requests
      TABLE pending_play {
         uint64_t signing_value;
         name player;
         uint64_t nonce;
         uint32_t timestamp;

         uint64_t primary_key() const { return signing_value; }
         uint64_t by_player() const { return player.value; }
      };

      // Contract configuration
      TABLE config {
         name token_contract;
         uint32_t success_rate_percent; // 35% success rate
         asset reward_amount;           // 1 DBP token reward
      };

      typedef eosio::multi_index<"usednonces"_n, used_nonce,
         indexed_by<"byplayer"_n, const_mem_fun<used_nonce, uint64_t, &used_nonce::by_player>>
      > used_nonces_table;

      typedef eosio::multi_index<"pendingplay"_n, pending_play,
         indexed_by<"byplayer"_n, const_mem_fun<pending_play, uint64_t, &pending_play::by_player>>
      > pending_plays_table;

      typedef eosio::multi_index<"config"_n, config> config_table;

      // WAX RNG Oracle contract
      static constexpr name RNG_ORACLE = "orng.wax"_n;

      void cleanup_old_nonces();
      void request_random( const name& player, const uint64_t& nonce );
      void process_result( const name& player, const uint64_t& nonce, const uint64_t& random_value );
      bool is_successful_play( const uint64_t& random_value );
      void issue_reward( const name& player );
};