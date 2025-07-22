#include <boost/test/unit_test.hpp>
#include <eosio/testing/tester.hpp>

using namespace eosio::testing;
using namespace eosio;
using namespace eosio::chain;
using namespace fc;

// Test scaffold for DBP Token Contract
// These tests should be fleshed out during development

BOOST_AUTO_TEST_SUITE(dbp_token_tests)

class dbp_token_tester : public tester {
public:
   dbp_token_tester() {
      // Deploy the contract
      create_accounts({N(dbptoken), N(alice), N(bob)});
      set_code(N(dbptoken), contracts::dbp_token_wasm());
      set_abi(N(dbptoken), contracts::dbp_token_abi().data());
      
      produce_blocks();
   }
   
   action_result create_token(name issuer, asset maximum_supply) {
      return push_action(N(dbptoken), N(create), mvo()
         ("issuer", issuer)
         ("maximum_supply", maximum_supply)
      );
   }
   
   action_result issue_tokens(name to, asset quantity, string memo) {
      return push_action(N(dbptoken), N(issue), mvo()
         ("to", to)
         ("quantity", quantity)
         ("memo", memo)
      );
   }
   
   action_result transfer_tokens(name from, name to, asset quantity, string memo) {
      return push_action(N(dbptoken), N(transfer), mvo()
         ("from", from)
         ("to", to)
         ("quantity", quantity)
         ("memo", memo)
      );
   }
   
   asset get_balance(name account, symbol sym) {
      vector<char> data = get_row_by_account(N(dbptoken), account, N(accounts), sym.code().raw());
      return data.empty() ? asset(0, sym) : token_abi_ser.binary_to_variant("account", data, abi_serializer_max_time)["balance"].as<asset>();
   }
   
   asset get_supply(symbol sym) {
      vector<char> data = get_row_by_account(N(dbptoken), sym.code().raw(), N(stat), sym.code().raw());
      return token_abi_ser.binary_to_variant("currency_stats", data, abi_serializer_max_time)["supply"].as<asset>();
   }

private:
   abi_serializer token_abi_ser;
};

BOOST_FIXTURE_TEST_CASE(create_token_test, dbp_token_tester) try {
   // Test token creation
   auto result = create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
   BOOST_REQUIRE_EQUAL(success(), result);
   
   // Verify token stats
   auto supply = get_supply(symbol(4, "DBP"));
   BOOST_REQUIRE_EQUAL(asset::from_string("0.0000 DBP"), supply);
   
} FC_LOG_AND_RETHROW()

BOOST_FIXTURE_TEST_CASE(issue_tokens_test, dbp_token_tester) try {
   // Create token first
   create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
   
   // Test token issuance
   auto result = issue_tokens(N(alice), asset::from_string("100.0000 DBP"), "test issue");
   BOOST_REQUIRE_EQUAL(success(), result);
   
   // Verify balance
   auto balance = get_balance(N(alice), symbol(4, "DBP"));
   BOOST_REQUIRE_EQUAL(asset::from_string("100.0000 DBP"), balance);
   
   // Verify supply
   auto supply = get_supply(symbol(4, "DBP"));
   BOOST_REQUIRE_EQUAL(asset::from_string("100.0000 DBP"), supply);
   
} FC_LOG_AND_RETHROW()

BOOST_FIXTURE_TEST_CASE(transfer_tokens_test, dbp_token_tester) try {
   // Setup: create token and issue to alice
   create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
   issue_tokens(N(alice), asset::from_string("100.0000 DBP"), "initial");
   
   // Test transfer from alice to bob
   auto result = transfer_tokens(N(alice), N(bob), asset::from_string("50.0000 DBP"), "test transfer");
   BOOST_REQUIRE_EQUAL(success(), result);
   
   // Verify balances
   auto alice_balance = get_balance(N(alice), symbol(4, "DBP"));
   auto bob_balance = get_balance(N(bob), symbol(4, "DBP"));
   
   BOOST_REQUIRE_EQUAL(asset::from_string("50.0000 DBP"), alice_balance);
   BOOST_REQUIRE_EQUAL(asset::from_string("50.0000 DBP"), bob_balance);
   
} FC_LOG_AND_RETHROW()

BOOST_FIXTURE_TEST_CASE(restricted_minting_test, dbp_token_tester) try {
   // Create token
   create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
   
   // Test that unauthorized account cannot issue tokens
   BOOST_REQUIRE_EQUAL(wasm_assert_msg("missing required authority"),
                      issue_tokens(N(alice), asset::from_string("100.0000 DBP"), "unauthorized"));
   
} FC_LOG_AND_RETHROW()

BOOST_FIXTURE_TEST_CASE(max_supply_test, dbp_token_tester) try {
   // Create token with small max supply
   create_token(N(dbptoken), asset::from_string("100.0000 DBP"));
   
   // Issue tokens up to max supply
   issue_tokens(N(alice), asset::from_string("100.0000 DBP"), "max supply");
   
   // Try to issue beyond max supply
   BOOST_REQUIRE_EQUAL(wasm_assert_msg("quantity exceeds available supply"),
                      issue_tokens(N(alice), asset::from_string("1.0000 DBP"), "over limit"));
   
} FC_LOG_AND_RETHROW()

BOOST_AUTO_TEST_SUITE_END()