CMAKE_MINIMUM_REQUIRED(VERSION 3.20)

include_guard()

set(CMAKE_CXX_STANDARD 23)

function(PROJECT_TEMPLATE)
    set(ARGS_FLAGS EXECUTABLE SHARED GLOB_SOURCES GLOB_INCLUDES)
    set(ARGS_VARIABLES MODULE PROJECT_DIR SOURCE_DIR INCLUDE_DIR OUTPUT_DIR)
    set(ARGS_ARRAYS SOURCE_FILES SHARED_LINK WIN_LINK LINUX_LINK)

    cmake_parse_arguments(EXE_ARG "${ARGS_FLAGS}" "${ARGS_VARIABLES}" "${ARGS_ARRAYS}" ${ARGN})

    set(TARGET ${EXE_ARG_MODULE})

    set(TARGET_DIR ${EXE_ARG_PROJECT_DIR})
    if (NOT DEFINED TARGET_DIR)
        string(REPLACE "." "/" TARGET_DIR ${TARGET})
    endif ()

    project(${TARGET})

    set(SRC_DIR ${EXE_ARG_SOURCE_DIR})
    if (NOT DEFINED SRC_DIR)
        set(SRC_DIR ${PROJECT_SOURCE_DIR}/${TARGET_DIR}/src)
    endif ()

    set(INC_DIR ${EXE_ARG_INCLUDE_DIR})
    if (NOT DEFINED INC_DIR)
        set(INC_DIR ${PROJECT_SOURCE_DIR}/${TARGET_DIR}/inc)
    endif ()

    if (${EXE_ARG_GLOB_SOURCES})
        file(GLOB_RECURSE SRC ${SRC_DIR}/*.cpp ${SRC_DIR}/*.hpp ${SRC_DIR}/*.c ${SRC_DIR}/*.h)
    endif ()

    if (${EXE_ARG_GLOB_INCLUDES})
        file(GLOB_RECURSE INC ${INC_DIR}/*.hpp ${INC_DIR}/*.h)
    endif ()

    if (${EXE_ARG_EXECUTABLE})
        add_executable(${TARGET} ${EXE_ARG_SOURCE_FILES} ${SRC} ${INC})
    elseif (${EXE_ARG_SHARED})
        add_library(${TARGET} SHARED ${EXE_ARG_SOURCE_FILES} ${SRC} ${INC})
    else ()
        add_library(${TARGET} ${EXE_ARG_SOURCE_FILES} ${SRC} ${INC})
    endif ()

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

    target_link_libraries(
            ${TARGET}
            ${EXE_ARG_SHARED_LINK}
    )

    if (EMSCRIPTEN)
        target_compile_options( ${TARGET} PUBLIC -sUSE_PTHREADS=1)
        target_link_options(${TARGET} PUBLIC
                -sUSE_PTHREADS=1
        )
    elseif (WIN32)
        target_link_libraries(
                ${TARGET}
                ${EXE_ARG_WIN_LINK}
        )
    elseif ( UNIX )
        target_link_libraries(
                ${TARGET}
                dl
                pthread
                ${EXE_ARG_LINUX_LINK}
        )
    endif ()

    if (NOT DEFINED EXE_ARG_OUTPUT_DIR)
        set(EXE_ARG_OUTPUT_DIR ${TARGET_DIR}/bin)
    endif ()

    file(MAKE_DIRECTORY ${EXE_ARG_OUTPUT_DIR})

    add_custom_command(
            TARGET ${TARGET}
            POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${TARGET}> ${EXE_ARG_OUTPUT_DIR}
    )
endfunction()
