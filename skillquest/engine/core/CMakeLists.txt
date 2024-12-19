include_guard()
project(GENERATED)
include("${PROJECT_SOURCE_DIR}/project.cmake")

PROJECT_TEMPLATE(
        MODULE skillquest.engine.core
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}/
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/inc
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        skillquest.engine.api
        skillquest.engine.audio.openal
        skillquest.engine.audio.opengl
)