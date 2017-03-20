# unity-freetype
Yer another Unity font provider with freetype

# Why?

There is a built-in FreeType based dynamic [font provider](https://docs.unity3d.com/ScriptReference/Font.html) has been involved since Unity 4.x.
However, this font provider is not extensible for advanced usage, such as you can't make a perfect stroke by FreeType APIs.

This project is a mock font provider implemention by get and render glyphs from the FreeType source which means you could change anything you want in the FreeType level.

# How to use?

## Build the plugin

To build the native plugins you should have [cmake](https://cmake.org/) installed. Then under the `Plugin` folder you will find such scripts:

    make_android.sh
    make_ios.sh
    make_macos.sh
    make_windows.bat
    
You may also need the NDK/Xcode/Visual Studio for each platform. After the plugins have been build place them into the right place in your Unity project.

## Integrate the code

1. Copy the folder `FreeTypeFont` into you Unity project.
2. Add `-unsafe` to the file `smcs.rsp` and `gmcs.rsp`. If you don't have such file in your Unity project just create a new one.
3. To use this font provide with NGUI:
    1. Seach all the `Font` in the NGUI source code and replace it with `FreeTypeFont`.
    2. Copy all the files under `Shaders` into the NGUI shader folder.
    3. Create a prefab and add two components: UIFont(From NGUI) and FreeTypeFont(From this project).
    4. Change the extension TTF font file you used to `.bytes` and drag it into the `Font Asset` field to the FreeTypeFont.
    5. Change the `Font Type` of UIFont to `Dynamic` and drag the FreeTypeFont component into the `TTF Font` field of the UIFont.
    
