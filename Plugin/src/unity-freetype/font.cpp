#include "font.h"

#define FT_SUCCEES(...) ((__VA_ARGS__) == 0)
#define FT_SHIFT6(Arg)  (int)((Arg) >> 6)

FreeTypeContext::FreeTypeContext(unsigned char* pData, unsigned int length)
: m_face(0)
, m_stroker(nullptr)
, m_pData(nullptr)
{
    if (FT_SUCCEES(FT_Init_FreeType(&m_FTlibrary)))
    {
        m_pData = new unsigned char[length];
        memcpy(m_pData, pData, length);
        FT_New_Memory_Face(m_FTlibrary, m_pData, length, 0, &m_face);
        FT_Select_Charmap(m_face, FT_ENCODING_UNICODE);
    }
}

FreeTypeContext::~FreeTypeContext()
{
    if (m_face != nullptr)
    {
        FT_Done_Face(m_face);
    }
    
    if (m_stroker != nullptr)
    {
        FT_Stroker_Done(m_stroker);
    }
    
    if (m_FTlibrary != nullptr)
    {
        FT_Done_FreeType(m_FTlibrary);
        m_FTlibrary = nullptr;
    }
    
    if (m_pData != nullptr)
    {
        delete[] m_pData;
    }
}

void FreeTypeContext::SetSize(int _size)
{
    if (lastSize != _size && m_face != nullptr)
    {
        lastSize = _size;
        int dpi = 72;
        int fontSizePoints = lastSize << 6;
        FT_Set_Char_Size(m_face, fontSizePoints, fontSizePoints, dpi, dpi);
    }
}

void FreeTypeContext::SetStroker(int _size)
{
    if (lastStrokeSize != _size && m_FTlibrary != nullptr)
    {
        lastStrokeSize = _size;
        if (m_stroker == nullptr)
        {
            FT_Stroker_New(m_FTlibrary, &m_stroker);
        }
        if (m_stroker != nullptr)
        {
            int outlineSizePoints = lastStrokeSize << 6;
            FT_Stroker_Set(m_stroker, outlineSizePoints, FT_STROKER_LINECAP_ROUND, FT_STROKER_LINEJOIN_ROUND, 0);
        }
    }
}

FontContext::FontContext(unsigned char* pData, unsigned int length)
: m_context(pData, length)
{
}

void FontContext::RenderSpans(FT_Outline* pOutline, Spans* spans)
{
    FT_Raster_Params params;
    memset(&params, 0, sizeof(params));
    params.flags = FT_RASTER_FLAG_AA | FT_RASTER_FLAG_DIRECT;
    params.gray_spans = [](const int y, const int count, const FT_Span * const spans, void * const user)
    {
        Spans* pSpans = (Spans*)user;
        for (int i = 0; i < count; ++i)
        {
            pSpans->push_back(Span{spans[i].x, y, spans[i].len, spans[i].coverage});
        }
    };
    params.user = spans;
    
    FT_Outline_Render(m_context.m_FTlibrary, pOutline, &params);
}


unsigned char* FontContext::GetGlyph(int code, FreeTypeGlyph& glyph, int fontSize, int outlineSize, bool bold)
{
    static unsigned char bitmapBuffer[0x2000] = {0};
    
    memset(&glyph, 0, sizeof(FreeTypeGlyph));
    
    if (m_context.m_FTlibrary != nullptr && m_context.m_face != nullptr)
    {
        FT_Face face = m_context.m_face;
        
        int index = FT_Get_Char_Index(face, code);
        
        if (index != 0)
        {
            m_context.SetSize(fontSize);
            m_context.SetStroker(outlineSize);
            
            // Load the glyph and render it into bitmapBuffer
            FT_Error error = FT_Load_Glyph(face, index, FT_LOAD_NO_BITMAP);
            if (FT_SUCCEES(error))
            {
                Spans faceSpans;
                Spans outlineSpans;
                
                RenderSpans(&face->glyph->outline, &faceSpans);
                
                if (outlineSize > 0)
                {
                    m_context.SetStroker(outlineSize);
                    
                    FT_Glyph outlineGlyph;
                    if (FT_SUCCEES(FT_Get_Glyph(face->glyph, &outlineGlyph)))
                    {
                        FT_Glyph_StrokeBorder(&outlineGlyph, m_context.m_stroker, 0, 1);
                        FT_Outline* strokeOutline = &reinterpret_cast<FT_OutlineGlyph>(outlineGlyph)->outline;
                        RenderSpans(strokeOutline, &outlineSpans);
                    }
                }
                
                int bitmapW = 0;
                int bitmapH = 0;
                
                if (!faceSpans.empty())
                {
                    Rect rect = Rect{ faceSpans.front().x, faceSpans.front().x, faceSpans.front().y, faceSpans.front().y };
                    
                    auto buildRect = [&](const Spans& spans)
                    {
                        for (auto iter = spans.begin(); iter != spans.end(); ++iter)
                        {
                            auto& span = *iter;
                            rect.Include(span.x, span.y);
                            rect.Include(span.x + span.width - 1, span.y);
                        }
                    };
                    
                    buildRect(faceSpans);
                    buildRect(outlineSpans);
                    
                    bitmapW = rect.width();
                    bitmapH = rect.height();
                    int bpp = 2;
                    
                    memset(bitmapBuffer, 0, bitmapW * bitmapH * bpp);
                    
                    auto fillBitmap = [&](const Spans& spans, int channel)
                    {
                        for (auto iter = spans.begin(); iter != spans.end(); ++iter)
                        {
                            auto& span = *iter;
                            int y = bitmapH - 1 - (span.y - rect.minY);
                            for (int i = 0; i < span.width; i++)
                            {
                                int x = span.x + i - rect.minX;
                                bitmapBuffer[(x + y * bitmapW) * bpp + channel] = span.coverage;
                            }
                        }
                    };
                    
                    fillBitmap(faceSpans, 0);
                    fillBitmap(outlineSpans, 1);
                }
                
                glyph.code = code;
                glyph.advance = FT_SHIFT6(face->glyph->metrics.horiAdvance);
                glyph.bearing = FT_SHIFT6(face->glyph->metrics.horiBearingX);
                glyph.minX = FT_SHIFT6(face->glyph->metrics.horiBearingX);
                glyph.maxX = glyph.minX + bitmapW;
                glyph.maxY = FT_SHIFT6(face->glyph->metrics.horiBearingY);
                glyph.minY = glyph.maxY - bitmapH;
                
                glyph.width = bitmapW;
                glyph.height = bitmapH;
                glyph.bpp = 2;
            }
        }
    }
    return bitmapBuffer;
}
