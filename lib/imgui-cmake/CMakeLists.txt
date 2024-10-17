cmake_minimum_required(VERSION 3.19)
include_guard()

set(TARGET imgui)
set(TARGET_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main)

project(${TARGET})

set(SRC_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/cpp)
set(INC_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/inc)


file(GLOB_RECURSE SRC ${SRC_DIR}/*.cpp ${SRC_DIR}/*.hpp ${SRC_DIR}/*.c ${SRC_DIR}/*.h)
file(GLOB_RECURSE INC ${INC_DIR}/*.hpp ${INC_DIR}/*.h)

add_library(${TARGET} ${SRC} ${INC})

target_include_directories(
        ${TARGET}
        PUBLIC ${INC_DIR}
        PRIVATE ${SRC_DIR}
)

set_target_properties(
        ${TARGET}
        PROPERTIES
        LINKER_LANGUAGE CXX
        CXX_STANDARD 23
        CXX_STANDARD_REQUIRED ON
)

if ( NOT EMSCRIPTEN )
find_package(glfw3 CONFIG REQUIRED)
find_package(OpenGL REQUIRED)

target_link_libraries(
        ${TARGET}
        glfw
        OpenGL::GL
)
endif()