#ifndef FONT_H
#define FONT_H

#include <ft2build.h>
#include <freetype/freetype.h>
#include <freetype/ftstroke.h>
#include <freetype/ftbitmap.h>
#include <string>
#include <unordered_map>
#include <vector>

struct Rect
{
    int minX;
    int maxX;
    int minY;
    int maxY;
    
    void Include(int x, int y)
    {
#define min(a, b) (a < b ? a : b)
#define max(a, b) (a > b ? a : b)
        minX = min(minX, x);
        maxX = max(maxX, x);
        minY = min(minY, y);
        maxY = max(maxY, y);
#undef min
#undef max
    }
    
    int width() { return maxX - minX + 1; }
    int height() { return maxY - minY + 1; }
};

struct Span
{
    int x;
    int y;
    int width;
    int coverage;
};
typedef std::vector<Span> Spans;

struct FreeTypeGlyph
{
    int code;
    int bearing;
    int advance;
    int minX;
    int maxX;
    int minY;
    int maxY;

    int width;
    int height;
    int bpp;
};

struct FreeTypeContext
{
    FreeTypeContext()
    : m_face(nullptr)
    , m_stroker(nullptr)
    , lastSize(0)
    , lastStrokeSize(0)
    {}
    
    FreeTypeContext(const char* pPath);
    FreeTypeContext(const FT_Byte* pData, unsigned int length);
    ~FreeTypeContext();   

    void SetSize(int _size);
    void SetStroker(int _size);
    
public:
    FT_Face m_face;
    FT_Stroker m_stroker;
    FT_Library   m_FTlibrary;
    
private:
    int lastSize;
    int lastStrokeSize;
    FT_Byte* m_pData;
};

class FontContext
{
public:
    FontContext(const char* pPath);
    FontContext(const FT_Byte* pData, unsigned int length);
    
    const FT_Byte* GetGlyph(int code, FreeTypeGlyph& glyph, int fontSize, int outlineSize = 0, bool bold = false);
private:
    void RenderSpans(FT_Outline* pOutline, Spans* spans);
    
private:
    FreeTypeContext    m_context;
};

#endif
