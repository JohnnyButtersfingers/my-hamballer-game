#include <boost/test/unit_test.hpp>
#include <eosio/testing/tester.hpp>
#include <eosio/chain/abi_serializer.hpp>

#include <Runtime/Runtime.h>

#include <fc/variant_object.hpp>

using namespace eosio::testing;
using namespace eosio;
using namespace eosio::chain;
using namespace fc;

class dbp_token_tester : public tester {
public:
    dbp_token_tester() {
        create_accounts({N(dbptoken), N(alice), N(bob), N(carol)});
        produce_block();

        // Deploy contract
        set_code(N(dbptoken), contracts::dbp_token_wasm());
        set_abi(N(dbptoken), contracts::dbp_token_abi().data());
        produce_block();
    }

    // Helper function to create token
    action_result create_token(account_name issuer, asset max_supply) {
        return push_action(N(dbptoken), N(create), N(dbptoken), mvo()
            ("issuer", issuer)
            ("maximum_supply", max_supply)
        );
    }

    // Helper function to issue tokens
    action_result issue_tokens(account_name to, asset quantity, string memo) {
        return push_action(N(dbptoken), N(issue), N(dbptoken), mvo()
            ("to", to)
            ("quantity", quantity)
            ("memo", memo)
        );
    }

    // Helper function to transfer tokens
    action_result transfer_tokens(account_name from, account_name to, asset quantity, string memo) {
        return push_action(N(dbptoken), N(transfer), from, mvo()
            ("from", from)
            ("to", to)
            ("quantity", quantity)
            ("memo", memo)
        );
    }

    // Helper function to get balance
    asset get_balance(account_name account) {
        vector<char> data = get_row_by_account(N(dbptoken), account, N(accounts), name(symbol(4, "DBP").to_symbol_code()));
        return data.empty() ? asset(0, symbol(4, "DBP")) : abi_ser.binary_to_variant("account", data, abi_serializer_max_time)["balance"].as<asset>();
    }

    // Helper function to get token stats
    fc::variant get_stats() {
        vector<char> data = get_row_by_account(N(dbptoken), name(symbol(4, "DBP").to_symbol_code()), N(stat), name(symbol(4, "DBP").to_symbol_code()));
        return data.empty() ? fc::variant() : abi_ser.binary_to_variant("currency_stats", data, abi_serializer_max_time);
    }

private:
    abi_serializer abi_ser{json::from_string(contracts::dbp_token_abi().data()).as<abi_def>(), abi_serializer_max_time};
};

BOOST_AUTO_TEST_SUITE(dbp_token_tests)

BOOST_FIXTURE_TEST_CASE(create_token_test, dbp_token_tester) {
    // Test creating a token
    create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
    produce_block();

    // Verify token was created
    auto stats = get_stats();
    BOOST_REQUIRE_EQUAL(stats["issuer"].as_string(), "dbptoken");
    BOOST_REQUIRE_EQUAL(stats["max_supply"].as_string(), "1000000.0000 DBP");
    BOOST_REQUIRE_EQUAL(stats["supply"].as_string(), "0.0000 DBP");
}

BOOST_FIXTURE_TEST_CASE(issue_tokens_test, dbp_token_tester) {
    // Create token first
    create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
    produce_block();

    // Issue tokens
    issue_tokens(N(alice), asset::from_string("1000.0000 DBP"), "Initial allocation");
    produce_block();

    // Verify balance
    BOOST_REQUIRE_EQUAL(get_balance(N(alice)), asset::from_string("1000.0000 DBP"));

    // Verify supply increased
    auto stats = get_stats();
    BOOST_REQUIRE_EQUAL(stats["supply"].as_string(), "1000.0000 DBP");
}

BOOST_FIXTURE_TEST_CASE(transfer_tokens_test, dbp_token_tester) {
    // Setup
    create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
    issue_tokens(N(alice), asset::from_string("1000.0000 DBP"), "Initial allocation");
    produce_block();

    // Transfer tokens
    transfer_tokens(N(alice), N(bob), asset::from_string("250.0000 DBP"), "Test transfer");
    produce_block();

    // Verify balances
    BOOST_REQUIRE_EQUAL(get_balance(N(alice)), asset::from_string("750.0000 DBP"));
    BOOST_REQUIRE_EQUAL(get_balance(N(bob)), asset::from_string("250.0000 DBP"));
}

BOOST_FIXTURE_TEST_CASE(burn_tokens_test, dbp_token_tester) {
    // Setup
    create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
    issue_tokens(N(alice), asset::from_string("1000.0000 DBP"), "Initial allocation");
    produce_block();

    // Burn tokens
    push_action(N(dbptoken), N(burn), N(alice), mvo()
        ("owner", "alice")
        ("quantity", "100.0000 DBP")
    );
    produce_block();

    // Verify balance and supply decreased
    BOOST_REQUIRE_EQUAL(get_balance(N(alice)), asset::from_string("900.0000 DBP"));
    
    auto stats = get_stats();
    BOOST_REQUIRE_EQUAL(stats["supply"].as_string(), "900.0000 DBP");
}

BOOST_FIXTURE_TEST_CASE(invalid_operations_test, dbp_token_tester) {
    create_token(N(dbptoken), asset::from_string("1000000.0000 DBP"));
    produce_block();

    // Test: Create duplicate token
    BOOST_REQUIRE_EXCEPTION(
        create_token(N(dbptoken), asset::from_string("2000000.0000 DBP")),
        eosio_assert_message_exception,
        eosio_assert_message_is("token with symbol already exists")
    );

    // Test: Issue more than max supply
    BOOST_REQUIRE_EXCEPTION(
        issue_tokens(N(alice), asset::from_string("2000000.0000 DBP"), "Too much"),
        eosio_assert_message_exception,
        eosio_assert_message_is("quantity exceeds available supply")
    );

    // Test: Transfer more than balance
    issue_tokens(N(alice), asset::from_string("100.0000 DBP"), "Initial");
    produce_block();
    
    BOOST_REQUIRE_EXCEPTION(
        transfer_tokens(N(alice), N(bob), asset::from_string("200.0000 DBP"), "Too much"),
        eosio_assert_message_exception,
        eosio_assert_message_is("overdrawn balance")
    );
}

BOOST_AUTO_TEST_SUITE_END()