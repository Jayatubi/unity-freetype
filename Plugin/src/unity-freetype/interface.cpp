#include "interface.h"

FontContext* UFT_CreateFontContext(unsigned char* pData, unsigned int length)
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

unsigned char* UFT_GetGlyph(FontContext* pContext, int code, FreeTypeGlyph* glyph, int fontSize, int outlineSize, bool bold)
{
    unsigned char* pResult = nullptr;

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
