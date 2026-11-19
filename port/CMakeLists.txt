cmake_minimum_required(VERSION 3.21)

project(ifme
    VERSION 9.0.0
    DESCRIPTION "Internet Friendly Media Encoder"
    LANGUAGES C CXX)

option(IFME_BUILD_GUI "Build the Qt GUI (needs Qt 6 Widgets)" ON)
option(IFME_BUILD_TESTS "Build the tests (ctest)" ON)
set(IFME_TEST_ROOT "" CACHE PATH
    "IFME install with Plugins/ and Samples/ for integration tests (empty: skip them)")

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
# cJSON 1.7.18 (MIT), vendored so every platform builds the same code.
add_library(cjson STATIC core/third_party/cjson/cJSON.c)
target_include_directories(cjson PUBLIC core/third_party/cjson)
set_target_properties(cjson PROPERTIES POSITION_INDEPENDENT_CODE ON)
if(NOT WIN32)
    target_link_libraries(cjson PUBLIC m)
endif()

# Platform files are all listed; each one compiles to nothing on the wrong OS
# or CPU (CMAKE_SYSTEM_PROCESSOR is unreliable and can even be empty).
add_library(ifmecore STATIC
    core/src/core.c
    core/src/argv.c
    core/src/json_relax.c
    core/src/plugins.c
    core/src/probe.c
    core/src/progress.c
    core/src/task.c
    core/src/jsonutil.c
    core/src/encode.c
    core/src/extras.c
    core/src/subtime.c
    core/src/platform/paths.c
    core/src/platform/fs.c
    core/src/platform/proc_common.c
    core/src/platform/proc_win32.c
    core/src/platform/proc_posix.c
    core/src/platform/cpu_x86.c
    core/src/platform/cpu_other.c)

target_include_directories(ifmecore PUBLIC core/include)
# IFME 9 alpha (the .NET release is 8.x).
set(IFME_VERSION_STRING "${PROJECT_VERSION}-alpha")
# Shared with IFME 8's release (AssemblyProduct / AssemblyTrademark): shown on
# the splash screen, in About and in the Windows file details.
set(IFME_CODENAME "Yoake no Shirabe")
set(IFME_TRADEMARKS "Anime4000, Rigaya, MulticoreWare, VideoLAN, IETF, Xiph.Org Foundation, Josh Coalson, Erik de Castro Lopo, Jean Le Feuvre, People@GPAC, FFmpeg Team, Nero AG, Moritz Bunkus, AMD, Intel, Nvidia, Google, Alliance for Open Media, et al.")
string(TIMESTAMP IFME_COPYRIGHT_YEAR "%Y" UTC)
target_compile_definitions(ifmecore PRIVATE IFME_VERSION="${IFME_VERSION_STRING}")
target_link_libraries(ifmecore PRIVATE cjson)

if(WIN32)
    target_compile_definitions(ifmecore PUBLIC UNICODE _UNICODE)
    target_link_libraries(ifmecore PRIVATE shell32 ole32)   # SHGetKnownFolderPath, CoTaskMemFree
endif()

# IFME's artwork (icon, splash, banners, About) is used as is from the C#
# project next to this folder.
set(IFME_ART_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../IFME/Resources")

# Windows: embed the manifest (UTF-8 code page, long paths) in every executable;
# the two programs also get the IFME icon and file details (version, copyright).
if(WIN32)
    set(IFME_MANIFEST "${CMAKE_CURRENT_SOURCE_DIR}/res/ifme.manifest")
    configure_file(res/ifme.rc.in "${CMAKE_CURRENT_BINARY_DIR}/ifme.rc" @ONLY)
    set(IFME_WIN_RESOURCES "${CMAKE_CURRENT_BINARY_DIR}/ifme.rc")
    set(IFME_GUI_RESOURCES "${IFME_WIN_RESOURCES}")
    set(IFME_CLI_RESOURCES "${IFME_WIN_RESOURCES}")
    if(EXISTS "${IFME_ART_DIR}/ifme-zenui.ico")
        set(IFME_ICON "${IFME_ART_DIR}/ifme-zenui.ico")
        foreach(prog gui cli)
            if(prog STREQUAL "gui")
                set(IFME_EXE_NAME "ifme9.exe")
                set(IFME_EXE_DESCRIPTION "Internet Friendly Media Encoder")
            else()
                set(IFME_EXE_NAME "ifmecli.exe")
                set(IFME_EXE_DESCRIPTION "Internet Friendly Media Encoder (command line)")
            endif()
            configure_file(res/ifme-app.rc.in "${CMAKE_CURRENT_BINARY_DIR}/ifme-${prog}.rc" @ONLY)
        endforeach()
        set(IFME_GUI_RESOURCES "${CMAKE_CURRENT_BINARY_DIR}/ifme-gui.rc")
        set(IFME_CLI_RESOURCES "${CMAKE_CURRENT_BINARY_DIR}/ifme-cli.rc")
    endif()
endif()

# ---------------------------------------------------------------------------
# ifmecli (C)
# ---------------------------------------------------------------------------
add_executable(ifmecli cli/ifmecli.c cli/exec_ui.c cli/encode_cmd.c ${IFME_CLI_RESOURCES})
target_link_libraries(ifmecli PRIVATE ifmecore cjson)

# ---------------------------------------------------------------------------
# ifme GUI (Qt 6, C++)
# ---------------------------------------------------------------------------
if(IFME_BUILD_GUI)
    find_package(Qt6 REQUIRED COMPONENTS Widgets)
    find_package(ZLIB REQUIRED)   # deflated entries in the .wad files
    set(CMAKE_AUTOMOC ON)
    set(CMAKE_AUTORCC ON)
    # The icon is built in; the rest of the artwork is read from resources.wad.
    if(NOT EXISTS "${IFME_ART_DIR}/ifme-zenui.ico")
        message(FATAL_ERROR "IFME icon not found in ${IFME_ART_DIR} (the GUI uses IFME/Resources/ifme-zenui.ico)")
    endif()

    add_executable(ifme WIN32 MACOSX_BUNDLE
        gui/main.cpp
        gui/app.cpp gui/app.h
        gui/splash.cpp gui/splash.h
        gui/aboutdialog.cpp gui/aboutdialog.h
        gui/widgets.cpp gui/widgets.h
        gui/encodeworker.cpp gui/encodeworker.h
        gui/settingstabs.cpp gui/settingstabs.h
        gui/dialogs.cpp gui/dialogs.h
        gui/mainwindow.cpp gui/mainwindow.h
        gui/profiles.cpp gui/profiles.h
        gui/i18n.cpp gui/i18n.h
        gui/wad.cpp gui/wad.h
        gui/queueitem.h
        gui/resources.qrc
        ${IFME_GUI_RESOURCES})

    target_link_libraries(ifme PRIVATE ifmecore Qt6::Widgets ZLIB::ZLIB)
    target_compile_definitions(ifme PRIVATE IFME_CODENAME="${IFME_CODENAME}")

    # Interface translations (i18n/*.json, see tools/i18n_update.py) go to
    # i18n_qt/ next to the programs (IFME 8's are in i18n/), read at start-up.
    file(GLOB IFME_I18N_FILES CONFIGURE_DEPENDS "${CMAKE_CURRENT_SOURCE_DIR}/i18n/*.json")
    set(IFME_I18N_OUT "${CMAKE_CURRENT_BINARY_DIR}/i18n_qt")
    add_custom_command(OUTPUT "${IFME_I18N_OUT}/.stamp"
        COMMAND "${CMAKE_COMMAND}" -E make_directory "${IFME_I18N_OUT}"
        COMMAND "${CMAKE_COMMAND}" -E copy_if_different ${IFME_I18N_FILES} "${IFME_I18N_OUT}"
        COMMAND "${CMAKE_COMMAND}" -E touch "${IFME_I18N_OUT}/.stamp"
        DEPENDS ${IFME_I18N_FILES}
        COMMENT "Copying translations")
    add_custom_target(ifme_translations ALL DEPENDS "${IFME_I18N_OUT}/.stamp")
    add_dependencies(ifme ifme_translations)

    set_target_properties(ifme PROPERTIES
        MACOSX_BUNDLE_BUNDLE_NAME "IFME"
        MACOSX_BUNDLE_GUI_IDENTIFIER "io.github.anime4000.ifme"
        MACOSX_BUNDLE_SHORT_VERSION_STRING "${PROJECT_VERSION}"
        OUTPUT_NAME ifme9)   # ifme9.exe, beside the .NET IFME.exe in a combined release

    # Portable copy for a combined release: cmake --build build --target deploy
    # puts ifme9, ifmecli, the Qt libraries and plugins and the .wad files in
    # IFME/bin/Qt (the .NET Release folder's neighbour).
    set(IFME_DEPLOY_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../IFME/bin/Qt" CACHE PATH "Where the deploy target copies the portable build")
    set(IFME_DEPLOY_FROM "${CMAKE_CURRENT_SOURCE_DIR}/../IFME/bin/Debug" CACHE PATH "IFME folder the .wad files are copied from")
    if(UNIX AND NOT APPLE)
        # Linux: look in ./lib first. DT_RPATH (not RUNPATH) so Qt and its
        # plugins find their bundled libraries too.
        foreach(t ifme ifmecli)
            set_target_properties(${t} PROPERTIES BUILD_RPATH "$ORIGIN/lib")
            target_link_options(${t} PRIVATE "LINKER:--disable-new-dtags")
        endforeach()
        set(IFME_QT_PLUGIN_HINT "")
        if(TARGET Qt6::QXcbIntegrationPlugin)
            set(IFME_QT_PLUGIN_HINT "$<TARGET_FILE:Qt6::QXcbIntegrationPlugin>")
        endif()
        add_custom_target(deploy
            COMMAND bash "${CMAKE_CURRENT_SOURCE_DIR}/tools/deploy-linux.sh"
                    "$<TARGET_FILE:ifme>" "$<TARGET_FILE:ifmecli>" "${IFME_DEPLOY_DIR}" "${IFME_DEPLOY_FROM}" "${CMAKE_CURRENT_SOURCE_DIR}/i18n" "${IFME_QT_PLUGIN_HINT}"
            DEPENDS ifme ifmecli
            COMMENT "Copying the portable build to ${IFME_DEPLOY_DIR}"
            VERBATIM)
    elseif(WIN32)
        find_program(IFME_BASH bash REQUIRED)
        get_target_property(IFME_QMAKE Qt6::qmake IMPORTED_LOCATION)
        get_filename_component(IFME_QT_BIN "${IFME_QMAKE}" DIRECTORY)
        add_custom_target(deploy
            COMMAND "${IFME_BASH}" "${CMAKE_CURRENT_SOURCE_DIR}/tools/deploy.sh"
                    "$<TARGET_FILE:ifme>" "$<TARGET_FILE:ifmecli>" "${IFME_DEPLOY_DIR}" "${IFME_DEPLOY_FROM}" "${IFME_QT_BIN}" "${CMAKE_CURRENT_SOURCE_DIR}/i18n"
            DEPENDS ifme ifmecli
            COMMENT "Copying the portable build to ${IFME_DEPLOY_DIR}"
            VERBATIM)
    endif()
endif()

# ---------------------------------------------------------------------------
# Tests:  cmake -B build -DIFME_TEST_ROOT=/path/to/IFME  &&  ctest --test-dir build
# ---------------------------------------------------------------------------
if(IFME_BUILD_TESTS)
    enable_testing()
    find_package(Threads REQUIRED)

    # White-box tests of internal helpers (progress parser, argv, JSON).
    add_executable(test_units tests/test_units.c ${IFME_WIN_RESOURCES})
    target_include_directories(test_units PRIVATE core/src)
    target_link_libraries(test_units PRIVATE ifmecore cjson)
    if(NOT WIN32)
        target_link_libraries(test_units PRIVATE m)
    endif()
    add_test(NAME units COMMAND test_units)

    # Real ffmpeg | x265 pipeline: pause, resume, stop. Skipped without a root.
    add_executable(test_task tests/test_task.c ${IFME_WIN_RESOURCES})
    target_link_libraries(test_task PRIVATE ifmecore Threads::Threads)
    add_test(NAME task COMMAND test_task "${IFME_TEST_ROOT}")
    set_tests_properties(task PROPERTIES SKIP_RETURN_CODE 77 TIMEOUT 300)

    # Encoding jobs: defaults, a short real encode, stop + cleanup, bad jobs.
    add_executable(test_encode tests/test_encode.c ${IFME_WIN_RESOURCES})
    target_link_libraries(test_encode PRIVATE ifmecore cjson Threads::Threads)
    add_test(NAME encode COMMAND test_encode "${IFME_TEST_ROOT}" "${CMAKE_CURRENT_BINARY_DIR}")
    set_tests_properties(encode PROPERTIES SKIP_RETURN_CODE 77 TIMEOUT 600)

    # GUI track rules (Language boxes, batching, kept selections), off-screen.
    if(IFME_BUILD_GUI)
        add_executable(test_gui tests/test_gui.cpp
            gui/app.cpp gui/widgets.cpp gui/widgets.h gui/settingstabs.cpp gui/settingstabs.h
            gui/profiles.cpp gui/profiles.h gui/i18n.cpp gui/i18n.h gui/wad.cpp gui/wad.h gui/resources.qrc)
        add_dependencies(test_gui ifme_translations)
        target_include_directories(test_gui PRIVATE gui)
        target_link_libraries(test_gui PRIVATE ifmecore Qt6::Widgets ZLIB::ZLIB)
        target_compile_definitions(test_gui PRIVATE IFME_TEST_DATA="${CMAKE_CURRENT_SOURCE_DIR}/tests/data")
        add_test(NAME gui COMMAND test_gui "${IFME_TEST_ROOT}")
    endif()
endif()
