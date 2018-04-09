# newsmake cmake project file.

# TODO: make sure tests are built
# c++ 17 and ensure everything is
# linking with the filesystem stuff.

# Do we need to link to
# stdc++fs on linux/mac on gcc
# or to the clang library for
# the std::experimental
# stuff too manually?
cmake_minimum_required(VERSION 3.6)
project(newsmake)

set(SOURCE_FILES
        main.cpp
)

# compile checks.
set(NEWSMAKE_FILESYSTEM
        "#incluse <iostream>
         #include <filesystem>
         namespace fs = std::filesystem;
         int main()
         {
           // print current directory.
           std::cout << fs::current_path(); << std::endl;
           return 0;
         }"
)
set(NEWSMAKE_EXP_FILESYSTEM
        "#incluse <iostream>
         #include <experimental/filesystem>
         namespace fs = std::experimental::filesystem;
         int main()
         {
           // print current directory.
           std::cout << fs::current_path(); << std::endl;
           return 0;
         }"
)
check_cxx_source_compiles(
        ${NEWSMAKE_FILESYSTEM}
        HAVE_STD_FS
)
check_cxx_source_compiles(
        ${NEWSMAKE_EXP_FILESYSTEM}
        HAVE_STD_EXP_FS
)

# configure fs.hpp for main.cpp to compile properly.
configure_file(${CMAKE_SOURCE_DIR}/fs.hpp.in
${CMAKE_SOURCE_DIR}/fs.hpp)

target_compile_features(newsmake
        PUBLIC
        cxx_std_17
)

add_executable(newsmake ${SOURCE_FILES})
