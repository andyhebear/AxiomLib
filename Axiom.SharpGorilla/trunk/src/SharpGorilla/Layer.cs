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

using Axiom.Core;
using Axiom.Math;
using Rectangles = System.Collections.Generic.List<SharpGorilla.Rectangle>;
using Polygons = System.Collections.Generic.List<SharpGorilla.Polygon>;
using LineLists = System.Collections.Generic.List<SharpGorilla.LineList>;
using QuadLists = System.Collections.Generic.List<SharpGorilla.QuadList>;
using Captions = System.Collections.Generic.List<SharpGorilla.Caption>;
using MarkupTexts = System.Collections.Generic.List<SharpGorilla.MarkupText>;

#endregion

namespace SharpGorilla
{
    /// <summary>
    /// Text
    /// </summary>
    public class Layer : IDisposable
    {
        protected int _index;
        protected Rectangles _rectangles = new Rectangles();
        protected Polygons _polygons = new Polygons();
        protected LineLists _lineLists = new LineLists();
        protected QuadLists _quadLists = new QuadLists();
        protected Captions _captions = new Captions();
        protected MarkupTexts _markupTexts = new MarkupTexts();
        protected ILayerContainer _parent;
        protected bool _isVisible;
		private int _zOrder = 0;
		public int ZMax
		{
			get { return _zOrder; }
		}
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
        /// <summary>
        /// Gets or sets this layer is visible or not.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible)
                    return;
                _isVisible = value;
                MarkDirty();
            }
        }
        /// <summary>
        /// Show the layer
        /// </summary>
        public void Show()
        {
            if (IsVisible)
                return;

            IsVisible = true;
        }
        /// <summary>
        /// hide the layer
        /// </summary>
        public void Hide()
        {
            if (!IsVisible)
                return;

            IsVisible = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public Captions Captions
        {
            get { return _captions; }
        }
        /// <summary>
        /// 
        /// </summary>
        public QuadLists QuadLists
        {
            get { return _quadLists; }
        }
        /// <summary>
        /// 
        /// </summary>
        public MarkupTexts MarkupTexts
        {
            get { return _markupTexts; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Rectangles Rectangles
        {
            get { return _rectangles; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Polygons Polygons
        {
            get { return _polygons; }
        }
        /// <summary>
        /// Gets a white pixel in the TextureAtlas.
        /// </summary>
        public Vector2 SolidUV
        {
            get { return _parent.Atlas.WhitePixel; }
        }
        /// <summary>
        ///  Gets the used texture size.
        /// </summary>
        public Vector2 TextureSize
        {
            get { return _parent.Atlas.TextureSize; }
        }
        /// <summary>
        /// Gets the used TextureAtlas
        /// </summary>
        public TextureAtlas Atlas
        {
            get { return _parent.Atlas; }
        }
        /// <summary>
        /// Gets the offset X texel coordinate.
        /// </summary>
        public Real TexelX
        {
            get { return _parent.TexelOffsetX; }
        }
        /// <summary>
        /// Gets the offset Y texel coordinate.
        /// </summary>
        public Real TexelY
        {
            get { return _parent.TexelOffsetY; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="screen"></param>
        internal Layer(int index, ILayerContainer parent)
        {
            _index = index;
            _parent = parent;
            _isVisible = true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            RemoveAllRectangles();
            RemoveAllPolygons();
            RemoveAllLineLists();
            RemoveAllQuadLists();
            RemoveAllCaptions();
            RemoveAllMarkupTexts();
        }
        /// <summary>
        ///  Helper function to get the markup color value.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ColorEx GetMarkupColor(int index)
        {
            return _parent.Atlas.GetMarkupColor(index);
        }
        /// <summary>
        /// Helper function to get a Sprite from the assigned texture atlas.
        /// </summary>
        /// <param name="name">name of the sprite</param>
        /// <returns>Sprite from the assigned texture atlas given by name</returns>
        public Sprite GetSprite(string name)
        {
            return _parent.Atlas.GetSprite(name);
        }
        /// <summary>
        /// Helper function to get a Glyph from the assigned texture atlas.
        /// </summary>
        /// <param name="id">id of the glyphdata</param>
        /// <returns>glyphdata for given ID</returns>
        public GlyphData GetGlyphData(int id)
        {
            return _parent.Atlas.GetGlyphData(id);
        }

        /// <summary>
        /// Creates a rectangle.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public Rectangle CreateRectangle(float left, float top)
        {
            return CreateRectangle(left,top,100,100);
        }

        /// <summary>
        /// Creates a rectangle.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Rectangle CreateRectangle(float left, float top, float width)
        {
            return CreateRectangle(left,top,width,100);
        }
        /// <summary>
        /// Creates a rectangle.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Rectangle CreateRectangle(float left, float top, float width, float height)
        {
            Rectangle rectangle = new Rectangle(left, top, width, height, this);
            _rectangles.Add(rectangle);
            return rectangle;
        }

        /// <summary>
        /// Creates a rectangle.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Rectangle CreateRectangle(Vector2 position)
        {
            return CreateRectangle(position, new Vector2(100, 100));
        }
        /// <summary>
        /// Creates a rectangle.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Rectangle CreateRectangle(Vector2 position, Vector2 size)
        {
            return CreateRectangle(position.x, position.y, size.x, size.y);
        }
        /// <summary>
        /// Removes a rectangle from the layer and *deletes* it.
        /// </summary>
        /// <param name="rectangle"></param>
        public void RemoveRectangle(ref Rectangle rectangle)
        {
            if (rectangle == null)
                return;

            _rectangles.Remove(rectangle);
            rectangle = null;
            MarkDirty();
        }
        /// <summary>
        /// Removes all rectangles from the layer and *deletes* them.
        /// </summary>
        public void RemoveAllRectangles()
        {
            _rectangles.Clear();
        }

        /// <summary>
        /// Creates a regular polygon.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public Polygon CreatePolygon(float left, float top)
        {
            return CreatePolygon(left,top,100,6);
        }

        /// <summary>
        /// Creates a regular polygon.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public Polygon CreatePolygon(float left, float top, float radius)
        {
            return CreatePolygon(left,top,radius,6);
        }

        /// <summary>
        /// Creates a regular polygon.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="radius"></param>
        /// <param name="sides"></param>
        /// <returns></returns>
        public Polygon CreatePolygon(float left, float top, float radius, int sides )
        {
            if (sides < 3)
                sides = 3;
            Polygon polygon = new Polygon(left, top, radius, sides, this);
            _polygons.Add(polygon);
            return polygon;
        }
        /// <summary>
        /// Removes a polygon from the layer and *deletes* it.
        /// </summary>
        /// <param name="polygon">polygon to remove</param>
        public void RemovePolygon(ref Polygon polygon)
        {
            if (polygon == null)
                return;

            _polygons.Remove(polygon);
            polygon = null;
            MarkDirty();
        }
        /// <summary>
        /// Removes all polygons from the layer and *deletes* them.
        /// </summary>
        public void RemoveAllPolygons()
        {
            _polygons.Clear();
        }
        /// <summary>
        /// Removes a line list from the layer and *deletes* it.
        /// </summary>
        /// <returns>new linelist</returns>
        public LineList CreateLineList()
        {
            LineList lineList = new LineList(this);
            _lineLists.Add(lineList);
            return lineList;
        }
        /// <summary>
        /// Removes a line list from the layer and *deletes* it.
        /// </summary>
        /// <param name="lineList">linelist to remove</param>
        public void RemoveLineList(ref LineList lineList)
        {
            if (lineList == null)
                return;

            _lineLists.Remove(lineList);
            lineList = null;
            MarkDirty();
        }
        /// <summary>
        /// Removes all line lists from the layer and *deletes* them.
        /// </summary>
        public void RemoveAllLineLists()
        {
            _lineLists.Clear();
        }
        /// <summary>
        /// Creates a quad list.
        /// </summary>
        /// <returns>new quad list</returns>
        public QuadList CreateQuadList()
        {
            QuadList quadList = new QuadList(this);
            _quadLists.Add(quadList);
            return quadList;
        }
        /// <summary>
        /// Removes a quad list from the layer and *deletes* it.
        /// </summary>
        /// <param name="quadlist">quadlist to remove</param>
        public void RemoveQuadList(ref QuadList quadlist)
        {
            if (quadlist == null)
                return;

            _quadLists.Remove(quadlist);
            quadlist = null;
            MarkDirty();
        }
        /// <summary>
        /// Removes all quad lists from the layer and *deletes* them.
        /// </summary>
        public void RemoveAllQuadLists()
        {
            _quadLists.Clear();
        }
        /// <summary>
        /// Creates a caption
        /// </summary>
        /// <param name="glyphDataIndex">glyphdata to use</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="text">text of caption</param>
        /// <returns>new caption</returns>
        public Caption CreateCaption(int glyphDataIndex, Real x, Real y, string text)
        {
            Caption caption = new Caption(glyphDataIndex, x, y, text, this);
            _captions.Add(caption);
            return caption;
        }
        /// <summary>
        /// Removes a caption from the layer and *deletes* it.
        /// </summary>
        /// <param name="caption">caption to remove</param>
        public void RemoveCaption(ref Caption caption)
        {
            if (caption == null)
                return;

            _captions.Remove(caption);
            caption = null;
            MarkDirty();
        }
        /// <summary>
        /// Removes all caption from the layer and *deletes* them.
        /// </summary>
        public void RemoveAllCaptions()
        {
            _captions.Clear();
        }
        /// <summary>
        /// Creates a markup text
        /// </summary>
        /// <param name="defaultGlyphIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public MarkupText CreateMarkupText(int defaultGlyphIndex, Real x, Real y, string text)
        {
            MarkupText markupText = new MarkupText(defaultGlyphIndex, x, y, text, this);
            _markupTexts.Add(markupText);
            return markupText;
        }
        /// <summary>
        /// Removes a markup text from the layer and *deletes* it.
        /// </summary>
        /// <param name="markupText">markuptext to delete</param>
        public void RemoveMarkupText(ref MarkupText markupText)
        {
            if (markupText == null)
                return;

            _markupTexts.Remove(markupText);
            MarkDirty();
        }
        /// <summary>
        ///  Removes all markup text from the layer and *deletes* them.
        /// </summary>
        public void RemoveAllMarkupTexts()
        {
            _markupTexts.Clear();
        }
        /// <summary>
        ///  Make this layer redraw itself on the next time that
        /// Gorilla updates to the screen.
        /// <para></para>
        /// Note: This shouldn't be needed to be called by the user.
        ///</summary>
        public void MarkDirty()
        {
            _parent.RequestIndexRedraw(_index);
        }
		public ILayerContainer Parent
		{
			get { return _parent; }
		}

        /// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="recalulateIndex"></param>
		public void SetIndex( int index )
        {
            SetIndex( index, false );
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="recalulateIndex"></param>
		public void SetIndex( int index, bool recalulateIndex )
		{
			_index = index;
			MarkDirty();
			if ( recalulateIndex )
			{
				_parent.ReCalculateIndexes();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        internal void Render(ref DynamicBuffer<Vertex> vertices)
        {
            Render( ref vertices, false );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        internal void Render(ref DynamicBuffer<Vertex> vertices, bool force)
        {
            int begin = vertices.Size;
            int i = 0;
			//_rectangles.Sort( new ZSorter<Rectangle>() );
            // Render/redraw rectangles
            foreach (Rectangle it in _rectangles)
            {
                if (it._isDirty || force)
                    it.Redraw();

                for (i = 0; i < it._vertices.Size; i++)
                    vertices.Add(it._vertices[i]);
            }

            // Render/redraw polygons
            foreach (Polygon it in _polygons)
            {
                if (it._isDirty || force)
                    it.Redraw();

                for (i = 0; i < it._vertices.Size; i++)
                    vertices.Add(it._vertices[i]);
            }

            // Render/redraw line lists
            foreach (LineList it in _lineLists)
            {
                if (it._isDirty || force)
                    it.Redraw();

                for (i = 0; i < it._vertices.Size; i++)
                    vertices.Add(it._vertices[i]);
            }

            // Render/redraw quad lists
            foreach (QuadList it in _quadLists)
            {
                if (it._isDirty || force)
                    it.Redraw();

                for (i = 0; i < it._vertices.Size; i++)
                    vertices.Add(it._vertices[i]);
            }

            // Render/redraw caption
            foreach (Caption it in _captions)
            {
                if (it._isDirty || force)
                    it.Redraw();

                for (i = 0; i < it._vertices.Size; i++)
                    vertices.Add(it._vertices[i]);
            }

            // Render/redraw markups
            foreach (MarkupText it in _markupTexts)
            {
                if (it._isTextDirty || force)
                    it.CalculateCharacters();

                if (it._isDirty || force)
                    it.Redraw();

                for (i = 0; i < it._vertices.Size; i++)
                    vertices.Add(it._vertices[i]);
            }

            _parent.Transform(ref vertices, begin, vertices.Size);
        }
    }
}
