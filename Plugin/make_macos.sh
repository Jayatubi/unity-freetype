#!/bin/bash

proj=$(pwd)
rm -rf build_macos

mkdir -p build_macos && cd build_macos
cmake -DTARGET_PLATFORM=macOS $proj
cd -

cmake --build build_macos --config Release -- -j