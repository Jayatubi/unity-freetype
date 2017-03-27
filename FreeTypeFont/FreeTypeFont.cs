using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FreeTypeFont : MonoBehaviour
{
    public TextAsset m_fontAsset;
    public Material m_material;

    private string m_fontName = null;
    private FreeTypeFontAtlas m_atlas;
    private bool m_updateScheduled = false;
    private bool m_textureEnlarged = false;
    private bool m_textureUpdated = false;

    public static event System.Action<FreeTypeFont> textureRebuilt;

    public Material material
    {
        get { return m_material; }
    }

    public string[] fontNames
    {
        get { return new string[] { m_fontName }; }
    }

    void Init()
    {
        if (m_atlas == null && m_fontAsset != null)
        {
            m_fontName = m_fontAsset.name;

            var fontFile = Path.Combine(Application.persistentDataPath, Path.Combine("Font", m_fontAsset.name));
            var fileInfo = new FileInfo(fontFile);
            if (!fileInfo.Exists || fileInfo.Length != m_fontAsset.bytes.Length)
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
                File.WriteAllBytes(fontFile, m_fontAsset.bytes);
            }
            Resources.UnloadAsset(m_fontAsset);
            m_atlas = new FreeTypeFontAtlas(fontFile);
#if !UNITY_EDITOR
            m_fontAsset = null;
#endif

            m_atlas.textureEnglarged += OnTextureEnlarged;
            m_atlas.textureUpdated += OnTextureUpdated;

            m_atlas.textureEnglarged += OnTextureEnlarged;
            m_atlas.textureUpdated += OnTextureUpdated;

            if (m_material == null)
            {
                m_material = new Material(Shader.Find("Unlit/FreeType Text"));
                m_material.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            }
            m_material.mainTexture = m_atlas.Texture;
            m_updateScheduled = false;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (m_atlas != null)
        {
            m_atlas.Dispose();
            m_atlas = null;
        }

        Init();
    }
#endif

    public void RequestCharactersInTexture(string characters, int size = 0, FontStyle style = FontStyle.Normal)
    {
        Init();

        if (m_atlas != null)
        {
            int outlineSize = (size >> 16) & 0xFFFF;
            size = size & 0xFFFF;
            FreeTypeFontAtlas.Glyph glyph;
            for (int i = 0; i < characters.Length; i++)
            {
                m_atlas.AqurieGlyph(characters[i], out glyph, size, outlineSize, style == FontStyle.Bold);
            }
        }
    }

    public bool GetCharacterInfo(char ch, out CharacterInfo info, int size = 0, FontStyle style = FontStyle.Normal)
    {
        info = new CharacterInfo();

        Init();

        if (m_atlas != null)
        {
            int outlineSize = (size >> 16) & 0xFFFF;
            size = size & 0xFFFF;
            FreeTypeFontAtlas.Glyph glyph;
            if (m_atlas.AqurieGlyph(ch, out glyph, size, outlineSize, style == FontStyle.Bold))
            {
                var rect = glyph.uv;
                info = new CharacterInfo
                {
                    index = glyph.ftGlyph.code,
                    advance = glyph.ftGlyph.advance,
                    bearing = glyph.ftGlyph.bearing,
                    minX = glyph.ftGlyph.minX,
                    maxX = glyph.ftGlyph.maxX,
                    minY = glyph.ftGlyph.minY,
                    maxY = glyph.ftGlyph.maxY,
                    size = size,
                    style = style,
                    glyphWidth = glyph.ftGlyph.width - outlineSize,
                    glyphHeight = glyph.ftGlyph.height - outlineSize,
                    uvBottomLeft = new Vector2(rect.xMin, rect.yMax),
                    uvBottomRight = new Vector2(rect.xMax, rect.yMax),
                    uvTopLeft = new Vector2(rect.xMin, rect.yMin),
                    uvTopRight = new Vector2(rect.xMax, rect.yMin),
                };
            }
        }
        return info.index != 0;
    }

    private void OnTextureEnlarged()
    {
        m_textureEnlarged = true;
        ScheduleUpdate();
    }

    private void OnTextureUpdated()
    {
        m_textureUpdated = true;
        ScheduleUpdate();
    }

    public void ScheduleUpdate()
    {
        if (!m_updateScheduled)
        {
            FreeTypeFontUpdater.ScheduleUpdate(() =>
            {
                if (m_textureEnlarged)
                {
                    textureRebuilt(this);
                    m_textureEnlarged = false;
                }

                if (m_textureUpdated)
                {
                    m_atlas.ApplyPacker();
                    m_textureUpdated = false;
                }
                m_updateScheduled = false;
            });

            m_updateScheduled = true;
        }
    }
}