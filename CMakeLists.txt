# newsmake cmake project file.

# TODO: ensure everything are linking
# with the filesystem stuff.

# Do we need to link to
# stdc++fs on linux/mac on gcc
# or to the clang library for
# the std::experimental
# stuff too manually?
cmake_minimum_required(VERSION 3.7)
project(newsmake CXX)

set(SOURCE_FILES
        main.cpp
)

add_executable(newsmake ${SOURCE_FILES})
target_compile_features(newsmake
        PRIVATE
        cxx_std_17
)

if ("${CMAKE_CXX_COMPILER_ID}" STREQUAL "Clang")
  # since clang, link with -lc++experimental.
  target_link_libraries(${PROJECT_NAME} c++experimental)
elseif ("${CMAKE_CXX_COMPILER_ID}" STREQUAL "GNU")
  # since gcc, link with -lstdc++fs.
  target_link_libraries(${PROJECT_NAME} stdc++fs)
endif()
