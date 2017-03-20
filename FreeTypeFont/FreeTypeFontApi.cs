using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

public static class FreeTypeFontApi
{
    #region Dll import
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FreeTypeGlyph
    {
        public int code;
        public int bearing;
        public int advance;
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;

        public int width;
        public int height;
        public int bpp;
    }

#if !UNITY_EDITOR && UNITY_IOS
    private const string s_library = "__Internal";
#else
    private const string s_library = "unity-freetype";
#endif
    [DllImport(s_library)]
    public static extern IntPtr UFT_CreateFontContext(IntPtr data, int length);

    [DllImport(s_library)]
    public static extern void UFT_DeleteFontContext(IntPtr pContext);

    [DllImport(s_library)]
    public static extern IntPtr UFT_GetGlyph(IntPtr pContext, int code, ref FreeTypeGlyph glyph, int fontsize, int outlinesize, bool bold);

    [DllImport(s_library)]
    public static extern IntPtr UFT_MemoryCopy(IntPtr pDst, IntPtr pSrc, int size);
    #endregion

    unsafe public static IntPtr CreateFontContext(TextAsset asset)
    {
        IntPtr result = IntPtr.Zero;
        if (asset != null)
        {
            fixed (byte* pData = asset.bytes)
            {
                result = UFT_CreateFontContext((IntPtr)pData, asset.bytes.Length);
            }
        }
        return result;
    }

    public static void DeleteFontContext(IntPtr pContext)
    {
        if (pContext != IntPtr.Zero)
        {
            UFT_DeleteFontContext(pContext);
        }
    }

    public static IntPtr GetGlyph(IntPtr fontContext, int charCode, out FreeTypeGlyph glyph, int fontsize, int outlinesize, bool bold)
    {
        IntPtr result = IntPtr.Zero;
        glyph = new FreeTypeGlyph();

        if (fontContext != IntPtr.Zero)
        {
            result = UFT_GetGlyph(fontContext, charCode, ref glyph, fontsize, outlinesize, bold);
        }
        return result;
    }
}
