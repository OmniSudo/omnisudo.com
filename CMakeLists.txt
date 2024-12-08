cmake_minimum_required(VERSION 3.20)

project(SkillQuest)

set(CMAKE_INSTALL_LIBDIR "${PROJECT_SOURCE_DIR}/lib/")

# SKILLQUEST ENGINE
include("${PROJECT_SOURCE_DIR}/skillquest/engine/shared/CMakeLists.txt")
include("${PROJECT_SOURCE_DIR}/skillquest/engine/client/CMakeLists.txt")
if (NOT EMSCRIPTEN)
    include("${PROJECT_SOURCE_DIR}/skillquest/engine/server/CMakeLists.txt")
endif ()

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
    include("${PROJECT_SOURCE_DIR}/skillquest/platform/client/desktop/CMakeLists.txt")
    include("${PROJECT_SOURCE_DIR}/skillquest/platform/server/terminal/CMakeLists.txt")
else ()
    include("${PROJECT_SOURCE_DIR}/skillquest/platform/client/web/CMakeLists.txt")
endif ()