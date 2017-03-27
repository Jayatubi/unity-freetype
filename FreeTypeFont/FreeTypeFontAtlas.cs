using System;
using System.Collections.Generic;
using UnityEngine;

public class FreeTypeFontAtlas : IDisposable
{
    public struct Glyph
    {
        public FreeTypeFontApi.FreeTypeGlyph ftGlyph;
        public Rect uv;
    }

    private struct GlyphParam
    {
        public int charCode;
        public int fontSize;
        public int outlineSize;
        public bool bold;
    }

    private class AtlasPacker
    {
        private const int s_canvasMinSize = 0x200;
        private const int s_canvasMaxSize = 0x1000;

        private const int s_glyphPadding = 2;

        private int m_canvasSize = 0;
        private int m_x;
        private int m_y;
        private int m_lineHeight;
        private byte[] m_rawData;
        private System.Action m_textureEnlarged;
        private System.Action m_texureUpdated;

        public Texture2D m_texture;

        public AtlasPacker(System.Action textureEnlarged, System.Action texureUpdated)
        {
            EnlargeTexture();
            m_textureEnlarged = textureEnlarged;
            m_texureUpdated = texureUpdated;
        }

        public bool EnlargeTexture()
        {
            bool result = true;
            if (m_canvasSize < s_canvasMinSize)
            {
                m_canvasSize = s_canvasMinSize;
            }
            else if (m_canvasSize < s_canvasMaxSize)
            {
                m_canvasSize <<= 1;
            }
            else
            {
                result = false;
            }

            if (result)
            {
                m_rawData = new byte[m_canvasSize * m_canvasSize * 2];
                if (m_texture == null)
                {
                    m_texture = new Texture2D(m_canvasSize, m_canvasSize, TextureFormat.RGBA4444, false);
                    m_texture.wrapMode = TextureWrapMode.Clamp;
                    m_texture.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
                }
                else
                {
                    m_texture.Resize(m_canvasSize, m_canvasSize);
                    Apply();
                }

                m_x = 0;
                m_y = 0;
                m_lineHeight = 0;

                if (m_textureEnlarged != null)
                {
                    m_textureEnlarged();
                }
            }

            return result;
        }

        public bool Alloc(ref Glyph glyph)
        {
            bool result = true;

            m_lineHeight = Math.Max(m_lineHeight, glyph.ftGlyph.height);

            if (m_x + glyph.ftGlyph.width + s_glyphPadding > m_canvasSize)
            {
                m_x = 0;
                m_y += m_lineHeight + s_glyphPadding;
                m_lineHeight = 0;
            }

            if (m_y + glyph.ftGlyph.height + s_glyphPadding > m_canvasSize)
            {
                EnlargeTexture();
                result = false;
            }

            if (result)
            {
                glyph.uv = new Rect(
                    (float)m_x / m_canvasSize,
                    (float)m_y / m_canvasSize,
                    (float)glyph.ftGlyph.width / m_canvasSize,
                    (float)glyph.ftGlyph.height / m_canvasSize
                );
            }

            return result;
        }

        public void Advance(FreeTypeFontApi.FreeTypeGlyph glyph)
        {
            m_x += glyph.width + s_glyphPadding;
        }

        unsafe public void UpdateData(IntPtr data, FreeTypeFontApi.FreeTypeGlyph glyph)
        {
            for (int y = 0; y < glyph.height; y++)
            {
                byte* srcPtr = (byte*)data.ToPointer();
                fixed (byte* dstPtr = m_rawData)
                {
                    int srcIndex = y * glyph.width * glyph.bpp;
                    int dstIndex = ((m_y + y) * m_canvasSize + m_x) * glyph.bpp;
                    FreeTypeFontApi.UFT_MemoryCopy((IntPtr)(dstPtr + dstIndex), (IntPtr)(srcPtr + srcIndex), glyph.width * glyph.bpp);
                }
            }
            if (m_texureUpdated != null)
            {
                m_texureUpdated();
            }
        }

        public void Apply()
        {
            m_texture.LoadRawTextureData(m_rawData);
            m_texture.Apply();
        }
    }

    private IntPtr m_fontContext;
    private Dictionary<GlyphParam, Glyph> m_glyphs;
    private AtlasPacker m_packer;

    public event System.Action textureEnglarged;
    public event System.Action textureUpdated;

    public FreeTypeFontAtlas(string path)
    {
        m_fontContext = FreeTypeFontApi.CreateFontContext(path);
        Init();
    }

    public FreeTypeFontAtlas(byte[] bytes)
    {
        m_fontContext = FreeTypeFontApi.CreateFontContext(bytes);
        Init();
    }

    private void Init()
    {
        m_glyphs = new Dictionary<GlyphParam, Glyph>();
        m_packer = new AtlasPacker(OnTextureEnlarged, OnTextureUpdated);
    }

    public void Dispose()
    {
        FreeTypeFontApi.DeleteFontContext(m_fontContext);
        m_fontContext = IntPtr.Zero;
    }

    private void OnTextureEnlarged()
    {
        m_glyphs.Clear();

        if (textureEnglarged != null)
        {
            textureEnglarged();
        }
    }

    private void OnTextureUpdated()
    {
        if (textureUpdated != null)
        {
            textureUpdated();
        }
    }

    public bool AqurieGlyph(int charCode, out Glyph glyph, int fontSize, int outlineSize, bool bold)
    {
        var glyphContext = new GlyphParam { charCode = charCode, fontSize = fontSize, outlineSize = outlineSize, bold = bold };

        if (!m_glyphs.TryGetValue(glyphContext, out glyph))
        {
            if (m_fontContext != IntPtr.Zero)
            {
                var data = FreeTypeFontApi.GetGlyph(m_fontContext, charCode, out glyph.ftGlyph, fontSize, outlineSize, bold);

                if (data != IntPtr.Zero)
                {
                    if (m_packer.Alloc(ref glyph))
                    {
                        m_packer.UpdateData(data, glyph.ftGlyph);
                        m_packer.Advance(glyph.ftGlyph);
                    }
                    else
                    {
                        glyph.ftGlyph.code = 0;
                    }
                }

                if (glyph.ftGlyph.code != 0)
                {
                    m_glyphs[glyphContext] = glyph;
                }
            }
        }

        return glyph.ftGlyph.code != 0;
    }

    public void ApplyPacker()
    {
        m_packer.Apply();
    }

    public Texture2D Texture
    {
        get
        {
            return m_packer.m_texture;
        }
    }
}