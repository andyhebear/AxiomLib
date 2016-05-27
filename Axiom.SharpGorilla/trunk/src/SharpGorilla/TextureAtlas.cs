#region License
/*
    Gorilla
    -------
    
    Copyright (c) 2010 Robin Southern
 
    This is a c# (Axiom) port of Gorrilla, developed by Robin Southern, ported by me (bostich)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
                                                                                  
    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.
                                                                                  
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE. 
 */
#endregion

#region SVN Version Information
// <file>
//     <id value="$Id: 2118 2010-09-26 23:56:56Z bostich $"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations
using System;
using System.Collections.Generic;
using System.IO;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Axiom.Configuration;
using SettingsMultimap = System.Collections.Generic.Dictionary<string, string>;
using ConfigMultiMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// The TextureAtlas file represents a .gorilla file which contains all the needed information that
    /// describes the portions of a single texture. Such as Glyph and Sprite information, text kerning,
    /// line heights and so on. It isn't typically used by the end-user.
    /// </summary>
    public class TextureAtlas
    {
        protected Texture _texture;
        protected Material _material;
        protected Material _2DMaterial;
        protected Material _3DMaterial;
        protected TextureUnitState _textureUnit;
        protected Pass _pass;
        protected Pass _2dPass;
        protected Pass _3dPass;
        protected Dictionary<int, GlyphData> _glyphData;
        protected Dictionary<string, Sprite> _sprites;
        protected Vector2 _whitePixel;
        protected Vector2 _inverseTextureSize;
        protected ColorEx[] _markupColor = new ColorEx[10];
        /// <summary>
        /// Gets the texture assigned to this TextureAtlas
        /// </summary>
        public Texture Texture
        {
            get { return _texture; }
        }
        /// <summary>
        ///  Gets the material assigned to this TextureAtlas
        /// </summary>
        public Material Material2D
        {
            get { return _2DMaterial; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Material Material3D
        {
            get { return _3DMaterial; }
        }
        /// <summary>
        /// Gets the UV information for a designated white pixel in the texture.
        /// </summary>
        /// <remarks>
        /// Units are in relative coordinates (0..1)
        /// </remarks>
        public Vector2 WhitePixel
        {
            get { return _whitePixel; }
        }
        /// <summary>
        /// Gets the X coordinate for a designated white pixel in the texture.
        /// </summary>
        /// <remarks>
        /// Units are in relative coordinates (0..1)
        /// </remarks>
        public Real WhitePixelX
        {
            get { return _whitePixel.x; }
        }
        /// <summary>
        /// Gets the Y coordinate for a designated white pixel in the texture.
        /// </summary>
        /// <remarks>
        /// Units are in relative coordinates (0..1)
        /// </remarks>
        public Real WhitePixelY
        {
            get { return _whitePixel.y; }
        }
        /// <summary>
        /// Gets the name of the material assigned to this TextureAtlas
        /// </summary>
        public string MaterialName2D
        {
            get { return _2DMaterial == null ? "NULL" : _2DMaterial.Name; }
        }
        /// <summary>
        /// Gets the name of the material assigned to this TextureAtlas
        /// </summary>
        public string MaterialName3D
        {
            get { return _3DMaterial == null ? "NULL" : _3DMaterial.Name; }
        }
        /// <summary>
        /// Gets the size of the texture.
        /// </summary>
        public Vector2 TextureSize
        {
            get { return new Vector2(_texture.Width, _texture.Height); }
        }
        /// <summary>
        /// Get the reciprocal of the width of the texture.
        /// </summary>
        public Real InvTextureCoordsX
        {
            get { return 1.0f / _texture.Width; }
        }
        /// <summary>
        /// Get the reciprocal of the height of the texture.
        /// </summary>
        public Real InvTextureCoordsY
        {
            get { return 1.0f / _texture.Height; }
        }
        /// <summary>
        /// Gets the first pass of the material used by this TextureAtlas
        /// </summary>
        public Pass Pass
        {
            get { return _pass; }
        }
        public Pass Pass2D
        {
            get { return _2dPass; }
        }
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="gorillaFile">file to load this atlas texture from</param>
        /// <param name="group">group where to find the file</param>
        public TextureAtlas(string gorillaFile, string group)
        {
            _glyphData = new Dictionary<int, GlyphData>();
            _sprites = new Dictionary<string, Sprite>();
            _inverseTextureSize = new Vector2();
            _whitePixel = new Vector2();
            Reset();
            Load(gorillaFile, group);
            CalculateCoordinates();
            Create2DMaterial();
            Create3DMaterial();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GlyphData GetGlyphData(int index)
        {
            GlyphData ret = null;
            _glyphData.TryGetValue(index, out ret);
            return ret;
        }
        /// <summary>
        /// Get a sprite (portion of a texture) from a name.
        /// </summary>
        /// <param name="name">name of the sprite</param>
        /// <returns>
        ///  If the sprite doesn't exist then a null pointer is returned.
        ///  Do not delete the Sprite pointer.
        /// </returns>
        public Sprite GetSprite(string name)
        {
            Sprite ret = null;
            _sprites.TryGetValue(name, out ret);
            return ret;
            
        }
        /// <summary>
        /// Reset the ten markup colours used in the MarkupText, by default these are:
        /// <para>0 = 255, 255, 255</para>
        /// <para>1 = 0, 0, 0</para>
        /// <para> 2 = 204, 204, 204</para>
        /// <para>3 = 254, 220, 129</para>
        /// <para>4 = 254, 138, 129</para>
        /// <para>5 = 123, 236, 110</para>
        /// <para>6 = 44,  192, 171</para>
        /// <para>7 = 199, 93,  142</para>
        /// <para>8 = 254, 254, 254</para>
        /// <para>9 = 13,  13,  13</para>
        /// </summary>
        public void RefreshMarkupColors()
        {
            _markupColor[0] = Converter.ToRGB(255, 255, 255);
            _markupColor[1] = Converter.ToRGB(0, 0, 0);
            _markupColor[2] = Converter.ToRGB(204, 204, 204);
            _markupColor[3] = Converter.ToRGB(254, 220, 129);
            _markupColor[4] = Converter.ToRGB(254, 138, 129);
            _markupColor[5] = Converter.ToRGB(123, 236, 110);
            _markupColor[6] = Converter.ToRGB(44, 192, 171);
            _markupColor[7] = Converter.ToRGB(199, 93, 142);
            _markupColor[8] = Converter.ToRGB(254, 254, 254);
            _markupColor[9] = Converter.ToRGB(13, 13, 13);
        }
        /// <summary>
        /// Change one of the ten markup colors.
        /// </summary>
        /// <param name="colorPaletteIndex"> must be between or equal to 0 and 9</param>
        /// <param name="value"></param>
        public void SetMarkupColor(int colorPaletteIndex, ColorEx value)
        {
            if (colorPaletteIndex > 9)
                return;

            _markupColor[colorPaletteIndex] = value;
        }
        /// <summary>
        /// Get one of the ten markup colors.
        /// </summary>
        /// <param name="colorPaletteIndex"> must be between or equal to 0 and 9.</param>
        /// <returns></returns>
        public ColorEx GetMarkupColor(int colorPaletteIndex)
        {
            if (colorPaletteIndex > 9)
                return ColorEx.White;

            return _markupColor[colorPaletteIndex];
        }
        /// <summary>
        /// 
        /// </summary>
        protected void Reset()
        {
            RefreshMarkupColors();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gorillaFile"></param>
        /// <param name="groupName"></param>
        protected void Load(string gorillaFile, string groupName)
        {
            StreamReader sr = new StreamReader(ResourceGroupManager.Instance.OpenResource(gorillaFile, groupName));
            string sLine = string.Empty;
            bool newSection = false;
            bool inSection = false;
            ConfigMultiMap _sections = new ConfigMultiMap();
            string currentSection = string.Empty;
            while ((sLine = sr.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(sLine))
                    continue;

                if (sLine.StartsWith("["))
                {
                    newSection = true;
                    inSection = false;
                    currentSection = sLine.Replace("[", "").Replace("]", "");
                    _sections[currentSection] = new SettingsMultimap();
                }
                if (inSection)
                {
                    string[] sectionSplit = sLine.Split(' ');
                    string sectionName = sectionSplit[0];
                    string sectionParams = string.Empty;
                    for (int i = 1; i < sectionSplit.Length; i++)
                    {
                        if (sectionSplit[i] == "")
                            continue;
                        sectionParams += sectionSplit[i].Trim() + ' ';
                    }
                    if (_sections[currentSection].ContainsKey(sectionName))
                        Console.WriteLine(sectionName + " is doubled, skipping");
                    else
                        _sections[currentSection].Add(sectionName, sectionParams);
                }
                if (newSection)
                {
                    newSection = false;
                    inSection = true;
                }
            }

            foreach (string section in _sections.Keys)
            {
                if (section.ToLower().Equals("texture"))
                    LoadTexture(_sections[section]);
                else if (section.ToLower().StartsWith("font."))
                {
                    int index = int.Parse(section.Substring(section.LastIndexOf('.') + 1));
                    GlyphData glyphData = new GlyphData();
                    _glyphData[index] = glyphData;
                    LoadGlyphs(_sections[section], ref glyphData);
                    LoadKerning(_sections[section], ref glyphData);
                }
                else if (section.ToLower().Equals("sprites"))
                    LoadSprites(_sections[section]);
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingsMultimap"></param>
        protected void LoadTexture(SettingsMultimap multimap)
        {
            string name = string.Empty;
            string data = string.Empty;

            foreach(KeyValuePair<string, string> i in multimap)
            {
                name = i.Key;
                data = i.Value;

                if (name.ToLower().Equals("file"))
                {
                    string textureName = data;
                    string groupName = ResourceGroupManager.DefaultResourceGroupName;
                    if (data.Contains("~"))
                    {
                        string[] split = data.Split(' ');
                        textureName = split[0].Trim().TrimEnd();
                        groupName = split[1].Replace("~", "").Trim();
                    }

                    if (ResourceGroupManager.Instance.ResourceExists(groupName, textureName) || true)
                    {
                        _texture = TextureManager.Instance.Load(textureName.Trim(), groupName);
                        _inverseTextureSize.x = 1.0f / _texture.Width;
                        _inverseTextureSize.y = 1.0f / _texture.Height;
                        continue;

                    }
                    else
                    {
                        throw new AxiomException(string.Format("Texture named {0} in resource group {1} not found!", textureName, groupName));
                    }

                }
                else if (name.ToLower().Equals("whitepixel"))
                {
                    string[] split = data.TrimEnd().Split(' ');
                    _whitePixel = new Vector2(int.Parse(split[0]), int.Parse(split[1]));
                    _whitePixel.x *= _inverseTextureSize.x;
                    _whitePixel.y *= _inverseTextureSize.y;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="multimap"></param>
        /// <param name="data"></param>
        protected void LoadGlyphs(SettingsMultimap settings,ref GlyphData glyphData)
        {
            string name = string.Empty;
            string data = string.Empty;
            Vector2 offset = new Vector2();
            foreach (KeyValuePair<string, string> i in settings)
            {
                name = i.Key.ToLower();
                data = i.Value.TrimEnd();

                if (name.Equals("offset"))
                {
                    string[] split = data.Split(' ');
                    offset = new Vector2(int.Parse(split[0]), int.Parse(split[1]));
                    continue;
                }
                else if (name.Equals("lineheight"))
                {
                    glyphData.LineHeight = float.Parse(data, System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }
                else if (name.Equals("spacelength"))
                {
                    glyphData.SpaceLength = float.Parse(data, System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }
                else if (name.Equals("baseline"))
                {
                    glyphData.Baseline = float.Parse(data, System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }
                else if (name.Equals("monowidth"))
                {
                    glyphData.MonoWidth = float.Parse(data, System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }
                else if (name.Equals("range"))
                {
                    string[] split = data.Split(' ');
                    Vector2 t = new Vector2(int.Parse(split[0]), int.Parse(split[1]));
                    glyphData.RangeBegin = t.x;
                    glyphData.RangeEnd = t.y;
                    continue;
                }
                else if (name.Equals("letterspacing"))
                {
                    glyphData.LetterSpacing = float.Parse(data, System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }
            }//end foreach

            for (int index = (int)glyphData.RangeBegin; index < (int)glyphData.RangeEnd; index++)
            {
                Glyph glyph = new Glyph();
                
                glyphData.Glyphs.Add(glyph);

                string s = "glyph_" + index;
                string datastr = string.Empty;
                if (!settings.TryGetValue(s, out datastr))
                    continue;
                datastr = datastr.TrimEnd();
                string[] split = datastr.Split(' ');
                if (split.Length < 4)
                {
                    LogManager.Instance.Write("[Gorilla] Glyph #" + s + " does not have enough properties. Must be minimum 4.");
                    continue;
                }

                glyph.UVLeft = offset.x + float.Parse(split[0], System.Globalization.CultureInfo.InvariantCulture);
                glyph.UVTop = offset.y + float.Parse(split[1], System.Globalization.CultureInfo.InvariantCulture);
                glyph.UVWidth = float.Parse(split[2], System.Globalization.CultureInfo.InvariantCulture);
                glyph.UVHeight = float.Parse(split[3], System.Globalization.CultureInfo.InvariantCulture);
                glyph.UVRight = glyph.UVLeft + glyph.UVWidth;
                glyph.UVBottom = glyph.UVTop + glyph.UVHeight;

                if (split.Length == 5)
                {
                    glyph.GlyphAdvance = int.Parse(split[4]);
                }
                else
                {
                    glyph.GlyphAdvance = glyph.GlyphWidth;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="multimap"></param>
        /// <param name="data"></param>
        protected void LoadKerning(SettingsMultimap settings, ref GlyphData glyphData)
        {
            string leftName = string.Empty;
            string data = string.Empty;
            int leftGlyphId;
            int rightGlyphId;
            int kerning;

            foreach (KeyValuePair<string, string> i in settings)
            {
                leftName = i.Key.ToLower();
                data = i.Value.TrimEnd();

                if (!leftName.Contains("kerning_"))
                    continue;

                leftName = leftName.Replace("kerning_", "");
                leftGlyphId = int.Parse(leftName);

                string[] split = data.Split(' ');

                if (split.Length != 2)
                {
                    LogManager.Instance.Write("[Gorilla] Kerning Glyph #" + leftName + " does not have enough properties. Must be 2.");
                    continue;
                }

                rightGlyphId = int.Parse(split[0]);
                kerning = int.Parse(split[1]);
                glyphData.Glyphs[rightGlyphId - (int)glyphData.RangeBegin].Kerning.Add(new Kerning(leftGlyphId, kerning));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="multimap"></param>
        protected void LoadSprites(SettingsMultimap settings)
        {
            string spriteName = string.Empty;
            string data = string.Empty;
            foreach (KeyValuePair<string, string> i in settings)
            {
                spriteName = i.Key;
                data = i.Value.TrimEnd();

                string[] split = data.Split(' ');
                if (split.Length != 4)
                {
                    LogManager.Instance.Write("[Gorilla] Sprite #" + spriteName + " does not have enough properties. Must be 4.");
                    continue;
                }

                Sprite sprite = new Sprite();

                sprite.UVLeft = int.Parse(split[0]);
                sprite.UVTop = int.Parse(split[1]);
                sprite.SpriteWidth = int.Parse(split[2]);
                sprite.SpriteHeight = int.Parse(split[3]);

                _sprites[spriteName] = sprite;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gorillaFile"></param>
        protected void Save(string gorillaFile)
        {
            //not yet implemented
        }
        protected Material CreateOrGet2DMasterMaterial()
        {
            Material d2Material = (Material)MaterialManager.Instance.GetByName("Gorrilla2D");
            if (d2Material != null) //allready existing, no need to create, just return it
                return d2Material;

            d2Material = (Material)MaterialManager.Instance.Create("Gorrilla2D", ResourceGroupManager.DefaultResourceGroupName);
            Pass pass = d2Material.GetTechnique(0).GetPass(0);
            pass.CullingMode = CullingMode.None;
            pass.DepthCheck = false;
            pass.DepthWrite = false;
            pass.LightingEnabled = false;
            pass.SetSceneBlending(SceneBlendType.TransparentAlpha);

            TextureUnitState texUnit = pass.CreateTextureUnitState();
            texUnit.SetTextureAddressingMode(TextureAddressing.Clamp);
            texUnit.SetTextureFiltering(FilterOptions.None, FilterOptions.None, FilterOptions.None);
            
            return d2Material;
        }

        protected Material CreateOrGet3DMasterMaterial()
        {
            Material d3Material = (Material)MaterialManager.Instance.GetByName("Gorrilla3D");
            if (d3Material != null) //allready existing, no need to create, just return it
                return d3Material;

            d3Material = (Material)MaterialManager.Instance.Create("Gorrilla3D", ResourceGroupManager.DefaultResourceGroupName);
            Pass pass = d3Material.GetTechnique(0).GetPass(0);
            pass.CullingMode = CullingMode.None;
            pass.DepthCheck = false;
            pass.DepthWrite = false;
            pass.LightingEnabled = false;
            pass.SetSceneBlending(SceneBlendType.TransparentAlpha);
            pass.AlphaRejectFunction = CompareFunction.Greater;
            pass.AlphaRejectValue = 125;
            TextureUnitState texUnit = pass.CreateTextureUnitState();
            texUnit.SetTextureAddressingMode(TextureAddressing.Clamp);
            texUnit.SetTextureFiltering(FilterOptions.Anisotropic, FilterOptions.Anisotropic, FilterOptions.Anisotropic);
            
            return d3Material;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Create2DMaterial()
        {
            string matName = "Gorilla2D." + _texture.Name;

            _2DMaterial = (Material)MaterialManager.Instance.GetByName(matName);
            if (_2DMaterial != null)
                return;

            _2DMaterial = CreateOrGet2DMasterMaterial().Clone(matName);
            _2dPass = _2DMaterial.GetTechnique(0).GetPass(0);
            _2dPass.GetTextureUnitState(0).SetTextureName(_texture.Name);
            _2DMaterial.Load();
        }
        /// <summary>
        /// 
        /// </summary>
        protected void Create3DMaterial()
        {
            string matName = "Gorilla3D." + _texture.Name;

            _3DMaterial = (Material)MaterialManager.Instance.GetByName(matName);
            if (_3DMaterial != null)
                return;

            _3DMaterial = CreateOrGet3DMasterMaterial().Clone(matName);
            _3dPass = _3DMaterial.GetTechnique(0).GetPass(0);
            _3dPass.GetTextureUnitState(0).SetTextureName(_texture.Name);
            _3DMaterial.Load();
        }
        /// <summary>
        /// 
        /// </summary>
        protected void CreateMaterial()
        {
            string matName = "GorrillaMaterialFor" + _texture.Name;
            _material = (Material)MaterialManager.Instance.GetByName(matName);
            
            if (!(_material == null))//allready existing, no need to create
                return;

            _material = (Material)MaterialManager.Instance.Create(matName, ResourceGroupManager.DefaultResourceGroupName);
            _pass = _material.GetTechnique(0).GetPass(0);

            _pass.CullingMode = CullingMode.None;
            _pass.DepthCheck = false;
            _pass.DepthWrite = false;
            _pass.LightingEnabled = false;
            _pass.SetSceneBlending(SceneBlendType.TransparentAlpha);

            _textureUnit = _pass.CreateTextureUnitState(_texture.Name);
            _textureUnit.SetTextureAddressingMode(TextureAddressing.Clamp);
            _textureUnit.SetTextureFiltering(FilterOptions.None, FilterOptions.None, FilterOptions.None);
            _material.Load();
        }
        /// <summary>
        /// 
        /// </summary>
        protected void CalculateCoordinates()
        {
            RenderSystem rs = Root.Instance.RenderSystem;
            Real texelX = rs.HorizontalTexelOffset;
            Real texelY = rs.VerticalTexelOffset;

            foreach (KeyValuePair<int, GlyphData> gdIT in _glyphData)
            {
                foreach (Glyph it in gdIT.Value.Glyphs)
                {
                    it.UVLeft -= texelX;
                    it.UVTop -= texelY;
                    it.UVRight += texelX;
                    it.UVBottom += texelY;

                    it.UVLeft *= _inverseTextureSize.x;
                    it.UVTop *= _inverseTextureSize.y;
                    it.UVRight *= _inverseTextureSize.x;
                    it.UVBottom *= _inverseTextureSize.y;

                    it.TexCoords[(int)QuadCorner.TopLeft].x = it.UVLeft;
                    it.TexCoords[(int)QuadCorner.TopLeft].y = it.UVTop;
                    it.TexCoords[(int)QuadCorner.TopRight].x = it.UVRight;
                    it.TexCoords[(int)QuadCorner.TopRight].y = it.UVTop;
                    it.TexCoords[(int)QuadCorner.BottomRight].x = it.UVRight;
                    it.TexCoords[(int)QuadCorner.BottomRight].y = it.UVBottom;
                    it.TexCoords[(int)QuadCorner.BottomLeft].x = it.UVLeft;
                    it.TexCoords[(int)QuadCorner.BottomLeft].y = it.UVBottom;

                    it.GlyphWidth = it.UVWidth;
                    it.GlyphHeight = it.UVHeight;
                }
            }

            foreach (KeyValuePair<string, Sprite> it in _sprites)
            {
                it.Value.UVRight = it.Value.UVLeft + it.Value.SpriteWidth;
                it.Value.UVBottom = it.Value.UVTop + it.Value.SpriteHeight;

                it.Value.UVLeft *= _inverseTextureSize.x;
                it.Value.UVTop *= _inverseTextureSize.y;
                it.Value.UVRight *= _inverseTextureSize.x;
                it.Value.UVBottom *= _inverseTextureSize.y;

                it.Value.TexCoords[(int)QuadCorner.TopLeft].x = it.Value.UVLeft;
                it.Value.TexCoords[(int)QuadCorner.TopLeft].y = it.Value.UVTop;
                it.Value.TexCoords[(int)QuadCorner.TopRight].x = it.Value.UVRight;
                it.Value.TexCoords[(int)QuadCorner.TopRight].y = it.Value.UVTop;
                it.Value.TexCoords[(int)QuadCorner.BottomRight].x = it.Value.UVRight;
                it.Value.TexCoords[(int)QuadCorner.BottomRight].y = it.Value.UVBottom;
                it.Value.TexCoords[(int)QuadCorner.BottomLeft].x = it.Value.UVLeft;
                it.Value.TexCoords[(int)QuadCorner.BottomLeft].y = it.Value.UVBottom;
            }
        }
    }
}
