include_guard()

set(ENABLE_ADDON FALSE)

if(ENABLE_ADDON)
    include( "${CMAKE_CURRENT_LIST_DIR}/shared/CMakeLists.txt" )
    include( "${CMAKE_CURRENT_LIST_DIR}/client/CMakeLists.txt" )

    if( NOT EMSCRIPTEN )
        include( "${CMAKE_CURRENT_LIST_DIR}/server/CMakeLists.txt" )
    endif()
endif()