#!/bin/bash

proj=$(pwd)
rm -rf build_android

mkdir -p build_android/arm && cd build_android/arm
cmake -DTARGET_PLATFORM=Android -DCMAKE_BUILD_TYPE=Release -DANDROID_ABI="armeabi-v7a" $proj
cd -

mkdir -p build_android/x86 && cd build_android/x86
cmake -DTARGET_PLATFORM=Android -DCMAKE_BUILD_TYPE=Release -DANDROID_ABI="x86" $proj
cd -

cmake --build build_android/arm --config Release -- -j
cmake --build build_android/x86 --config Release -- -j