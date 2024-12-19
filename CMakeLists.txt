cmake_minimum_required(VERSION 3.20)

project(SkillQuest)

set(CMAKE_INSTALL_LIBDIR "${PROJECT_SOURCE_DIR}/lib/")

# SKILLQUEST ENGINE
include("skillquest/engine/CMakeLists.txt")

file(GLOB addons "${CMAKE_CURRENT_SOURCE_DIR}/skillquest/addon/*")
foreach (addon_dir ${addons})
    if (NOT IS_DIRECTORY ${addon_dir})
        continue()
    endif ()

    if (NOT EXISTS "${addon_dir}/CMakeLists.txt")
        continue()
    endif ()

    get_filename_component( addon "${addon_dir}" NAME )

    include("${addon_dir}/CMakeLists.txt")
endforeach ()

# SKILLQUEST LAUNCHER
if (NOT EMSCRIPTEN)
    include("skillquest/platform/desktop/CMakeLists.txt")
else ()
    include("skillquest/platform/web/CMakeLists.txt")
endif ()