@echo off

set proj=%cd%
rd build_windows /s /q

md build_windows\32 & pushd build_windows\32
cmake -DTARGET_PLATFORM=Windows -G "Visual Studio 14 2015" %proj%
popd

md build_windows\64 & pushd build_windows\64
cmake -DTARGET_PLATFORM=Windows -G "Visual Studio 14 2015 Win64" %proj%
popd

cmake --build build_windows\32 --config Release
cmake --build build_windows\64 --config Release