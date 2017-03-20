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
    EXPORT FontContext* UFT_CreateFontContext(unsigned char* pData, unsigned int length);
    EXPORT void UFT_DeleteFontContext(FontContext* pDefine);
    EXPORT unsigned char* UFT_GetGlyph(FontContext* pDefine, int code, FreeTypeGlyph* glyph, int fontSize, int outlineSize, bool bold);
    EXPORT void UFT_MemoryCopy(unsigned int* pDst, unsigned int* pSrc, int size);
}

#endif
