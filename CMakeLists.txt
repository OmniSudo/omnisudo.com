cmake_minimum_required(VERSION 3.20)

project(OmniSudo)

set(CMAKE_INSTALL_LIBDIR "${PROJECT_SOURCE_DIR}/lib/")

# SKILLQUEST PUBLIC
IF (IS_DIRECTORY "${PROJECT_SOURCE_DIR}/skillquest/public/game")
    include("${PROJECT_SOURCE_DIR}/skillquest/public/game/shared/CMakeLists.txt")
    include("${PROJECT_SOURCE_DIR}/skillquest/public/game/client/CMakeLists.txt")
ENDIF ()

IF (IS_DIRECTORY "${PROJECT_SOURCE_DIR}/skillquest/public/platform")
    include("${PROJECT_SOURCE_DIR}/skillquest/public/platform/desktop/CMakeLists.txt")
ENDIF ()

# SKILLQUEST PRIVATE
if (IS_DIRECTORY "${PROJECT_SOURCE_DIR}/skillquest/private/game" AND NOT EMSCRIPTEN)
    include("${PROJECT_SOURCE_DIR}/skillquest/private/game/server/CMakeLists.txt")
endif ()
