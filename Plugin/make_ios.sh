#!/bin/bash

proj=$(pwd)
rm -rf build_ios

mkdir -p build_ios && cd build_ios
cmake -DTARGET_PLATFORM=iOS -GXcode $proj
cd -

cmake --build build_ios --config Release

