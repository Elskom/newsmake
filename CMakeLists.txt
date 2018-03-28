cmake_minimum_required(VERSION 3.6)
project(newsmake)

if( CMAKE_BINARY_DIR STREQUAL CMAKE_SOURCE_DIR )
  message( FATAL_ERROR "In-source builds aren't allowed.\nDo `rm -f CMakeCache.txt && cd build && cmake ..` instead." )
endif()

set(SOURCE_FILES
        main.cpp
)

add_executable(newsmake ${SOURCE_FILES})
