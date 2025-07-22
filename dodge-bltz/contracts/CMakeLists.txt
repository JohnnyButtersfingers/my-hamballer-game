cmake_minimum_required(VERSION 3.5)
project(dodge_bltz_contracts)

find_package(eosio.cdt)

# DBP Token Contract
add_contract(dbp_token dbp_token
   ${CMAKE_CURRENT_SOURCE_DIR}/dbp_token.cpp
)

target_include_directories(dbp_token PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})

# Gameplay Contract
add_contract(gameplay gameplay
   ${CMAKE_CURRENT_SOURCE_DIR}/gameplay.cpp
)

target_include_directories(gameplay PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})