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
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// Directions for background gradients
    /// </summary>
    public enum Gradient
    {
        NorthSouth,
        WestEast,
        Diagonal,
    }
    /// <summary>
    /// Border Directions
    /// <remarks>
    /// <para>+---------------------+</para>
    /// <para>|\       NORTH       /|</para>
    /// <para>| \                 / |</para>
    /// <para>|  +---------------+  |</para>
    /// <para>|  |               |  |</para>
    /// <para>| W|               |E |</para>
    /// <para>| E|               |A |</para>
    /// <para>| S|               |S |</para>
    /// <para>| T|               |T |</para>
    /// <para>|  |               |  |</para>
    /// <para>|  +---------------+  |</para>
    /// <para>| /      SOUTH      \ |</para>
    /// <para>|/                   \|</para>
    /// <para>+---------------------+</para>
    /// </remarks>
    /// </summary>
    public enum Border
    {
        North = 0,
        South = 1,
        East = 2,
        West = 3,
    }
    /// <summary>
    /// Names of each corner/vertex of a Quad
    /// </summary>
    public enum QuadCorner
    {
        TopLeft = 0,
        TopRight = 1,
        BottomRight = 2,
        BottomLeft = 3,
    }
    /// <summary>
    /// Horizontal text alignment for captions.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// Place the text to where left is (X = left)
        /// </summary>
        Left,
        /// <summary>
        /// Place the text to the right of left (X = left - text_width)
        /// </summary>
        Right,
        /// <summary>
        /// Place the text centered at left (X = left - (text_width / 2 ) )
        /// </summary>
        Center
    }
    /// <summary>
    /// Vertical text alignment for captions.
    /// </summary>
    public enum VerticalAlignment
    {
        Top,
        Middle,
        Bottom,
    }

}
