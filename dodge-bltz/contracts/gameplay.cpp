#include "gameplay.hpp"

ACTION gameplay::init( const name& token_contract )
{
    require_auth( get_self() );
    
    config_table config_tbl( get_self(), get_self().value );
    auto existing = config_tbl.find( 0 );
    
    if( existing == config_tbl.end() ) {
        config_tbl.emplace( get_self(), [&]( auto& c ) {
            c.token_contract = token_contract;
            c.success_rate_percent = 35; // 35% success rate
            c.reward_amount = asset(10000, symbol("DBP", 4)); // 1.0000 DBP tokens
        });
    } else {
        config_tbl.modify( existing, get_self(), [&]( auto& c ) {
            c.token_contract = token_contract;
            c.success_rate_percent = 35;
            c.reward_amount = asset(10000, symbol("DBP", 4));
        });
    }
}

ACTION gameplay::play( const name& player, const uint64_t& nonce )
{
    require_auth( player );
    
    // Verify nonce hasn't been used
    used_nonces_table used_nonces( get_self(), get_self().value );
    auto nonce_itr = used_nonces.find( nonce );
    check( nonce_itr == used_nonces.end(), "nonce already used" );
    
    // Add nonce to used nonces table
    used_nonces.emplace( player, [&]( auto& n ) {
        n.nonce = nonce;
        n.player = player;
        n.timestamp = current_time_point().sec_since_epoch();
    });
    
    // Request random number from WAX RNG Oracle
    request_random( player, nonce );
    
    // Cleanup old nonces periodically
    cleanup_old_nonces();
}

ACTION gameplay::receiverand( const uint64_t& caller_signing_value, 
                             const checksum256& caller_signing_value_hash,
                             const uint64_t& random_value )
{
    // Only the RNG Oracle can call this
    require_auth( RNG_ORACLE );
    
    // Find the pending play request
    pending_plays_table pending_plays( get_self(), get_self().value );
    auto pending_itr = pending_plays.find( caller_signing_value );
    
    check( pending_itr != pending_plays.end(), "no pending play found for this signing value" );
    
    // Process the result
    process_result( pending_itr->player, pending_itr->nonce, random_value );
    
    // Remove from pending plays
    pending_plays.erase( pending_itr );
}

void gameplay::request_random( const name& player, const uint64_t& nonce )
{
    // Generate a unique signing value
    uint64_t signing_value = current_time_point().time_since_epoch().count() + nonce;
    
    // Store pending play
    pending_plays_table pending_plays( get_self(), get_self().value );
    pending_plays.emplace( player, [&]( auto& p ) {
        p.signing_value = signing_value;
        p.player = player;
        p.nonce = nonce;
        p.timestamp = current_time_point().sec_since_epoch();
    });
    
    // Send RNG request to WAX Oracle
    action(
        permission_level{ get_self(), "active"_n },
        RNG_ORACLE,
        "requestrand"_n,
        std::make_tuple( signing_value, signing_value, get_self() )
    ).send();
}

void gameplay::process_result( const name& player, const uint64_t& nonce, const uint64_t& random_value )
{
    // Determine if play was successful (35% chance)
    bool success = is_successful_play( random_value );
    
    if( success ) {
        // Issue reward to player
        issue_reward( player );
    }
    
    // Note: On failure, nothing happens (no penalty, no reward)
}

bool gameplay::is_successful_play( const uint64_t& random_value )
{
    // Use modulo to get percentage (0-99)
    uint32_t result_percent = random_value % 100;
    
    // Success if result is less than 35 (35% chance)
    return result_percent < 35;
}

void gameplay::issue_reward( const name& player )
{
    // Get contract configuration
    config_table config_tbl( get_self(), get_self().value );
    auto config_itr = config_tbl.begin();
    check( config_itr != config_tbl.end(), "contract not initialized" );
    
    // Issue DBP tokens to player
    action(
        permission_level{ get_self(), "active"_n },
        config_itr->token_contract,
        "issue"_n,
        std::make_tuple( player, config_itr->reward_amount, std::string("BLTZ reward") )
    ).send();
}

void gameplay::cleanup_old_nonces()
{
    // Clean up nonces older than 24 hours to prevent table from growing too large
    uint32_t cutoff_time = current_time_point().sec_since_epoch() - (24 * 60 * 60);
    
    used_nonces_table used_nonces( get_self(), get_self().value );
    auto itr = used_nonces.begin();
    
    int cleanup_count = 0;
    while( itr != used_nonces.end() && cleanup_count < 10 ) { // Limit cleanup per call
        if( itr->timestamp < cutoff_time ) {
            itr = used_nonces.erase( itr );
            cleanup_count++;
        } else {
            ++itr;
        }
    }
}