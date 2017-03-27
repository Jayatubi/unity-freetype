#include "interface.h"

FontContext* UFT_CreateFontContextByFile(const char* pPath)
{
    return new FontContext(pPath);
}

FontContext* UFT_CreateFontContextByData(const FT_Byte* pData, unsigned int length)
{
    return new FontContext(pData, length);
}

void UFT_DeleteFontContext(FontContext* pContext)
{
    if (pContext != nullptr)
    {
        delete pContext;
    }
}

const FT_Byte* UFT_GetGlyph(FontContext* pContext, int code, FreeTypeGlyph* glyph, int fontSize, int outlineSize, bool bold)
{
    const FT_Byte* pResult = nullptr;

    if (pContext != nullptr && glyph != nullptr)
    {
        pResult = pContext->GetGlyph(code, *glyph, fontSize, outlineSize, bold);
    }

    return pResult;
}

void UFT_MemoryCopy(unsigned int* pDst, unsigned int* pSrc, int size)
{
    memcpy(pDst, pSrc, size);
}
