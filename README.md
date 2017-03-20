# unity-freetype
Yet another Unity font provider with freetype

# Why?

There is a built-in FreeType based dynamic [font provider](https://docs.unity3d.com/ScriptReference/Font.html) has been involved since Unity 4.x. However, this font provider is not extensible for advanced usage, such as it is hard for you to make a perfect stroke by FreeType APIs.

This project is a mock font provider implemention by getting and rendering glyphs from the FreeType source. And with this source you will be able to change anything you want in the FreeType level.

# How to use?

## Build the plugin

To build the native plugins you should have [cmake](https://cmake.org/) installed. Then under the `Plugin` folder you will find such scripts:

    make_android.sh
    make_ios.sh
    make_macos.sh
    make_windows.bat
    
> You may also need the NDK/Xcode/Visual Studio installed for each platform.

After the building finish put the output binaries into the right place in your Unity project.

## Integrate the code

1. Copy the folder *FreeTypeFont* into you Unity project.
2. Add *-unsafe* to the file *smcs.rsp* and *gmcs.rsp*. If you don't have them just create a new one.
    > This `-unsafe` option is used to enable direct memory operation for performance purpose. If you don't like it you could change those parts of code to `Marshal`s.
3. To use this font provide with NGUI:
    1. Seach all the *Font* in the NGUI source code and replace it with *FreeTypeFont*. Backup is strongly recommanded.
    2. Copy all the files under *Shaders* into the NGUI shader folder.
    3. Create a prefab with two components: *UIFont*(From NGUI) and *FreeTypeFont*(From this project).
    4. Change the extension of the TTF font file to *.bytes*, or any other TextAsset extensions, and drag it to the *Font Asset* field of the FreeTypeFont.
    5. Change the *Font Type* of UIFont to *Dynamic* and drag this FreeTypeFont component to its *TTF Font* field.
    
## Know issues

This project (might) isn't so good as the Unity built-in one because:

1. It uses a poor, but quick, algorithm to packing glyphs. A little more space wasting than the built-in one.
2. It uses a 16 bit texture in order to render the glyph stroke. It needs an extra 8 bit channels for the stroke alpha.
3. It need to two vertex color channels for the glyph face and stroke. However, the Unity Mesh component only has one Colors property so it compose these two color chanels into one. Which means the each channel will have only 16 bit colors. 

## Do I need it?

If you think the text apperance in your project is good enough don't look into this.
If you want to tweak the text in the FreeType level, clone this repo and make any changes you like.
