cmake_minimum_required(VERSION 3.6)
project(newsmake)

set(SOURCE_FILES
        main.cpp
)
target_compile_features(newsmake
    PUBLIC
    cxx_std_17
)

add_executable(newsmake ${SOURCE_FILES})
