cmake_minimum_required(VERSION 3.7)

if(${TARGET_PLATFORM} STREQUAL iOS)
    set(CMAKE_TOOLCHAIN_FILE cmake/iOS.cmake)
endif()

if(${TARGET_PLATFORM} STREQUAL Android)
    set(ANDROID_NDK $ENV{ANDROID_NDK_ROOT})
    set(CMAKE_TOOLCHAIN_FILE cmake/android.toolchain.cmake)
endif()

set(PROJ unity-freetype)
project(${PROJ})

include_directories (${CMAKE_CURRENT_SOURCE_DIR}/src/freetype/include)

add_definitions(-DFT2_BUILD_LIBRARY)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11 -fvisibility=hidden")
set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fvisibility=hidden -Wno-shorten-64-to-32")

if(${TARGET_PLATFORM} STREQUAL Android)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -s")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -s")
endif()

set(FREETYPE_SOURCE ${CMAKE_CURRENT_SOURCE_DIR}/src/freetype/src)
set(freetype_sources
    ${FREETYPE_SOURCE}/autofit/autofit.c
    ${FREETYPE_SOURCE}/base/ftbase.c
    ${FREETYPE_SOURCE}/base/ftbitmap.c    
    ${FREETYPE_SOURCE}/base/ftglyph.c
    ${FREETYPE_SOURCE}/base/ftinit.c
    ${FREETYPE_SOURCE}/base/ftstroke.c
    ${FREETYPE_SOURCE}/base/ftsystem.c
    ${FREETYPE_SOURCE}/sfnt/sfnt.c
    ${FREETYPE_SOURCE}/smooth/smooth.c
    ${FREETYPE_SOURCE}/truetype/truetype.c
    ${FREETYPE_SOURCE}/psnames/psnames.c
    ${FREETYPE_SOURCE}/raster/raster.c
    ${FREETYPE_SOURCE}/cff/cff.c
    ${FREETYPE_SOURCE}/pshinter/pshinter.c
)

file(GLOB_RECURSE unity-freetype_sources ${CMAKE_CURRENT_SOURCE_DIR}/src/unity-freetype/*.cpp)

set(sources ${freetype_sources} ${unity-freetype_sources})

set(OUTPUT_BASE ${CMAKE_CURRENT_SOURCE_DIR}/Plugins)

if(${TARGET_PLATFORM} STREQUAL macOS)
    set(CMAKE_OSX_ARCHITECTURES x86_64;i386)
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${OUTPUT_BASE}/${TARGET_PLATFORM})
    add_library(${PROJ} MODULE ${sources})
    set_target_properties(${PROJ} PROPERTIES BUNDLE TRUE)
endif()

if(${TARGET_PLATFORM} STREQUAL Windows)
    if (${CMAKE_SIZEOF_VOID_P} EQUAL 4)
        set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${OUTPUT_BASE}/${TARGET_PLATFORM}/x86)
    else()
        set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${OUTPUT_BASE}/${TARGET_PLATFORM}/x86_64)
    endif()
    add_library(${PROJ} SHARED ${sources})    
endif()

if(${TARGET_PLATFORM} STREQUAL iOS)
    set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${OUTPUT_BASE}/${TARGET_PLATFORM})
    add_library(${PROJ} STATIC ${sources})
endif()

if(${TARGET_PLATFORM} STREQUAL Android)
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${OUTPUT_BASE}/${TARGET_PLATFORM}/libs/${ANDROID_ABI})
    add_library(${PROJ} SHARED ${sources})    
endif()