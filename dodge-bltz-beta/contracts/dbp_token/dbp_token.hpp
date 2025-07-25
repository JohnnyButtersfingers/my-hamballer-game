#pragma once

#include <eosio/eosio.hpp>
#include <eosio/asset.hpp>
#include <eosio/system.hpp>

using namespace eosio;

CONTRACT dbp_token : public contract {
   public:
      using contract::contract;

      /**
       * Create action.
       *
       * @param issuer - the account that creates the token,
       * @param maximum_supply - the maximum supply set for the token
       */
      ACTION create( const name&   issuer,
                     const asset&  maximum_supply);

      /**
       * Issue action.
       *
       * @param to - the account to issue tokens to, it must be the same as the issuer,
       * @param quantity - the amount of tokens to be issued,
       * @param memo - the memo string to accompany the transaction.
       */
      ACTION issue( const name& to,
                    const asset& quantity,
                    const std::string& memo );

      /**
       * Transfer action.
       *
       * @param from - the account to transfer from,
       * @param to - the account to be transferred to,
       * @param quantity - the quantity of tokens to be transferred,
       * @param memo - the memo string to accompany the transaction.
       */
      ACTION transfer( const name&    from,
                       const name&    to,
                       const asset&   quantity,
                       const std::string& memo );

      /**
       * Get supply method. Gets the supply for token `sym_code`, created by `token_contract_account`.
       *
       * @param token_contract_account - the account to get the supply for
       * @param sym_code - the symbol to get the supply for
       */
      static asset get_supply( const name& token_contract_account, const symbol_code& sym_code )
      {
         stats statstable( token_contract_account, sym_code.raw() );
         const auto& st = statstable.get( sym_code.raw() );
         return st.supply;
      }

      /**
       * Get balance method. Gets the balance for account `owner` for token `sym_code`, created by `token_contract_account`.
       *
       * @param token_contract_account - the account to get the balance for
       * @param owner - the owner to get the balance for
       * @param sym_code - the symbol to get the balance for
       */
      static asset get_balance( const name& token_contract_account, const name& owner, const symbol_code& sym_code )
      {
         accounts accountstable( token_contract_account, owner.value );
         const auto& ac = accountstable.get( sym_code.raw() );
         return ac.balance;
      }

      using create_action = eosio::action_wrapper<"create"_n, &dbp_token::create>;
      using issue_action = eosio::action_wrapper<"issue"_n, &dbp_token::issue>;
      using transfer_action = eosio::action_wrapper<"transfer"_n, &dbp_token::transfer>;

   private:
      TABLE account {
         asset    balance;

         uint64_t primary_key()const { return balance.symbol.code().raw(); }
      };

      TABLE currency_stats {
         asset    supply;
         asset    max_supply;
         name     issuer;

         uint64_t primary_key()const { return supply.symbol.code().raw(); }
      };

      typedef eosio::multi_index< "accounts"_n, account > accounts;
      typedef eosio::multi_index< "stat"_n, currency_stats > stats;

      void sub_balance( const name& owner, const asset& value );
      void add_balance( const name& owner, const asset& value, const name& ram_payer );
};