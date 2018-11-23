# newsmake cmake project file.

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
# allow make install or sudo make install.
install(TARGETS newsmake DESTINATION bin)
include(InstallRequiredSystemLibraries)
set(CPACK_RESOURCE_FILE_LICENSE
     "${CMAKE_CURRENT_SOURCE_DIR}/LICENSE.txt")
set(CPACK_PACKAGE_VENDOR "Els_kom Org.")
set(CPACK_PACKAGE_DESCRIPTION_SUMMARY "News / changelog making tool.")
set(CPACK_PACKAGE_VERSION_MAJOR "1")
set(CPACK_PACKAGE_VERSION_MINOR "0")
set(CPACK_PACKAGE_VERSION_PATCH "0")
set(CPACK_PACKAGE_VERSION_TWEAK "0")
set(CPACK_PACKAGE_CONTACT AraHaan)
set(CPACK_NSIS_MODIFY_PATH ON)
set(CPACK_WIX_UPGRADE_GUID D691CA39-A156-488C-946F-3B51207B3579)
# TODO: Force add the page to ask the user to add newsmake to %path% on all installers in all systems.
# set(CPACK_WIX_MODIFY_PATH ON)
include(CPack)
