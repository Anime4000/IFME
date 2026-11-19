cmake_minimum_required(VERSION 3.21)

project(ifme
    VERSION 0.1.0
    DESCRIPTION "Internet Friendly Media Encoder"
    LANGUAGES C CXX)

option(IFME_BUILD_GUI "Build the Qt GUI (needs Qt 6 Widgets)" ON)

set(CMAKE_C_STANDARD 11)
set(CMAKE_C_STANDARD_REQUIRED ON)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

if(NOT CMAKE_BUILD_TYPE AND NOT CMAKE_CONFIGURATION_TYPES)
    set(CMAKE_BUILD_TYPE Release CACHE STRING "Build type" FORCE)
endif()

if(CMAKE_C_COMPILER_ID MATCHES "GNU|Clang")
    add_compile_options(-Wall -Wextra)
endif()

# ---------------------------------------------------------------------------
# libifmecore (C11, no Qt)
# ---------------------------------------------------------------------------
# cpu_x86.c / cpu_other.c are both listed; each compiles to nothing unless the
# compiler targets its architecture (CMAKE_SYSTEM_PROCESSOR is unreliable and
# can even be empty, e.g. when the environment lacks PROCESSOR_ARCHITECTURE).
add_library(ifmecore STATIC
    core/src/core.c
    core/src/platform/paths.c
    core/src/platform/cpu_x86.c
    core/src/platform/cpu_other.c)

target_include_directories(ifmecore PUBLIC core/include)
target_compile_definitions(ifmecore PRIVATE IFME_VERSION="${PROJECT_VERSION}")

if(WIN32)
    target_compile_definitions(ifmecore PUBLIC UNICODE _UNICODE)
    target_link_libraries(ifmecore PRIVATE shell32 ole32)   # SHGetKnownFolderPath, CoTaskMemFree
endif()

# Windows: embed the manifest (UTF-8 code page, long paths) in every executable.
if(WIN32)
    set(IFME_MANIFEST "${CMAKE_CURRENT_SOURCE_DIR}/res/ifme.manifest")
    configure_file(res/ifme.rc.in "${CMAKE_CURRENT_BINARY_DIR}/ifme.rc" @ONLY)
    set(IFME_WIN_RESOURCES "${CMAKE_CURRENT_BINARY_DIR}/ifme.rc")
endif()

# ---------------------------------------------------------------------------
# ifmecli (C)
# ---------------------------------------------------------------------------
add_executable(ifmecli cli/ifmecli.c ${IFME_WIN_RESOURCES})
target_link_libraries(ifmecli PRIVATE ifmecore)

# ---------------------------------------------------------------------------
# ifme GUI (Qt 6, C++)
# ---------------------------------------------------------------------------
if(IFME_BUILD_GUI)
    find_package(Qt6 REQUIRED COMPONENTS Widgets)
    set(CMAKE_AUTOMOC ON)

    add_executable(ifme WIN32 MACOSX_BUNDLE
        gui/main.cpp
        gui/mainwindow.cpp
        gui/mainwindow.h
        ${IFME_WIN_RESOURCES})

    target_link_libraries(ifme PRIVATE ifmecore Qt6::Widgets)

    set_target_properties(ifme PROPERTIES
        MACOSX_BUNDLE_BUNDLE_NAME "IFME"
        MACOSX_BUNDLE_GUI_IDENTIFIER "io.github.anime4000.ifme"
        MACOSX_BUNDLE_SHORT_VERSION_STRING "${PROJECT_VERSION}")
endif()
