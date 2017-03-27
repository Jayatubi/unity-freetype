#ifndef INTERFACE_H
#define INTERFACE_H

#include "font.h"

#ifdef _MSC_VER
#define EXPORT __declspec(dllexport)
#else
#define EXPORT __attribute__ ((visibility ("default")))
#endif

extern "C"
{
    EXPORT FontContext* UFT_CreateFontContextByFile(const char* pPath);
    EXPORT FontContext* UFT_CreateFontContextByData(const FT_Byte* pData, unsigned int length);
    EXPORT void UFT_DeleteFontContext(FontContext* pDefine);
    EXPORT const FT_Byte* UFT_GetGlyph(FontContext* pDefine, int code, FreeTypeGlyph* glyph, int fontSize, int outlineSize, bool bold);
    EXPORT void UFT_MemoryCopy(unsigned int* pDst, unsigned int* pSrc, int size);
}

#endif
