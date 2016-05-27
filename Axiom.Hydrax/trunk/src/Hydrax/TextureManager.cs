#region - License -

#region LGPL License
/*
 This file is part of the Axiom 3D Hydrax port.

 Originial project developer:
 Xavier Verguín González <xavierverguin@hotmail.com>
                         <xavyiy@gmail.com>
 (see http://www.ogre3d.org/addonforums/viewforum.php?f=20&sid=e58afbbf10919de484bf7c4f590f7d17 )
 
 Axiom Hydrax developer:
 Bostich

The overall design, and a majority of the core engine and rendering code 
contained within this library is a derivative of the open source Object Oriented 
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.  
Many thanks to the OGRE team for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion

#endregion

#region - using -
using System;
using System.Collections.Generic;
using System.Drawing;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Media;
using System.Runtime.InteropServices;
#endregion

#region - namespace -
namespace Axiom.Hydrax
{
    #region - class -
    /// <summary>
    /// Class to manage normal maps.
    /// </summary>
    public class TextureManager
    {
        #region - enum -
        /// <summary>
        /// Textures enumeration
        /// </summary>
        public enum TexturesID
        {
            /// <summary>
            /// Normal map
            /// </summary>
            TEX_NORMAL_ID = 0
        }
        #endregion

        #region - Fields -
        protected Texture[] mTextures = new Texture[1];
        protected string[] mTextureNames = new string[1];
        protected bool mIsCreated;
        protected Hydrax mHydrax;
        #endregion

        #region - Properties -

        #region - Textures -
        /// <summary>
        /// Get's a Texture.
        /// </summary>
        public Texture[] Textures
        {
            get { return mTextures; }
        }
        #endregion

        #region - TexureNames -
        /// <summary>
        /// Get's TextureNames.
        /// </summary>
        public string[] TextureNames
        {
            get { return mTextureNames; }
        }
        #endregion

        #region - IsCreated -
        /// <summary>
        /// Get's true if this was allready created. Otherwise false.
        /// </summary>
        public bool IsCreated
        {
            get { return mIsCreated; }
        }
        #endregion

        #endregion

        #region - Constructor, Destructor -
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="Hydrax">Hydrax object.</param>
        public TextureManager(Hydrax Hydrax)
        {
            mHydrax = Hydrax;
            for (int k = 0; k < 1; k++)
            {
                mTextures[k] = null;
            }
            mTextureNames[0] = "HydraxNormalMap";
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~TextureManager()
        {
            Remove();
        }
        #endregion

        #region - methods -
        #region - Create -
        /// <summary>
        /// Create height and normal map textures.
        /// </summary>
        ///<param name="Size">Textures's size.</param>
        public void Create(Size Size)
        {
            Remove();
            for (int k = 0; k < 1; k++)
            {
                CreateTexture(mTextures[k], mTextureNames[k], Size);
            }
            mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_WATER);

            mIsCreated = true;
        }
        #endregion

        #region - Remmove -
        /// <summary>
        /// Remove textures.
        /// </summary>
        public void Remove()
        {
            if (!mIsCreated)
                return;

            for (int k = 0; k < 1; k++)
            {
                Axiom.Core.TextureManager.Instance.Remove(mTextureNames[k]);
                mTextures[k] = null;
            }

            mIsCreated = false;
        }
        #endregion

        #region - Update -
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">Texture's ID</param>
        /// <param name="Image">Update image</param>
        /// <remarks>
        /// If you need to update the texture with another way of data,
        /// get the Texture and modify it directly.
        /// Normal image will be Image.Type.RGB [0,255] range.
        /// </remarks>
        public bool Update(TexturesID Id, Image Image)
        {
            if (Id == TexturesID.TEX_NORMAL_ID)
            {
                return UpdateNormalMap(Image);
            }
            return false;
        }
        #endregion

        #region - UpdateNormalMap -
        /// <summary>
        /// Update normal map
        /// </summary>
        /// <param name="Image">Update image</param>
        /// <returns>false if something fails</returns>
        /// <remarks>Image type will be Image.Type.RGB</remarks>
        private bool UpdateNormalMap(Image Image)
        {
            if (!mIsCreated)
            {
                Hydrax.HydraxLog("Error in TextureManager.UpdateNormalMap(), Create() was not called.");
                return false;
            }
            if (Image.Type != Image.ImageType.TYPE_RGB)
            {
                Hydrax.HydraxLog("Error in TextureManager.UpdateNormalMap, Image type isn't correct.");
                return false;
            }

            Texture Texture = mTextures[(int)TexturesID.TEX_NORMAL_ID];

            Size ImageSize = Image.Size;
            if (Texture.Width != ImageSize.Width ||
                Texture.Height != ImageSize.Height)
            {
                Hydrax.HydraxLog("Error in TextureManager.UpdateNormalMap, Update size doesn't correspond to " +
                    GetTextureName(TexturesID.TEX_NORMAL_ID) + " texture size.");

                return false;
            }

            HardwarePixelBuffer pixelBuffer = Texture.GetBuffer();
            pixelBuffer.Lock(BufferLocking.Normal);
            PixelBox pixelBox = pixelBuffer.CurrentLock;

            IntPtr pDest = pixelBox.Data;
            int x = 0, y = 0;

#warning correct pointer handling?
            List<float> mFloatList = new List<float>();
            for (x = 0; x < ImageSize.Width; x++)
            {
                for (y = 0; y < ImageSize.Height; y++)
                {
                    mFloatList.Add(Image.GetValue(x, y, 2)); //B
                    mFloatList.Add(Image.GetValue(x, y, 1)); //G
                    mFloatList.Add(Image.GetValue(x, y, 0)); //R
                    mFloatList.Add(255);                     //A
                }
            }
            unsafe
            {
                float* pDestFin = null;
                fixed (float* inc = mFloatList.ToArray())
                {
                    for(float* inc2 = inc;inc2 <inc + mFloatList.Count;inc2++)
                    {
                        pDestFin = inc2;
                    }
                }
                pDest = (IntPtr)pDestFin;
            }
            pixelBuffer.Unlock();


            return true;
        }
        #endregion

        #region - GetTexture -
        /// <summary>
        /// Get's an Texture by the ID.
        /// </summary>
        /// <param name="Name">Name of the Texture.</param>
        /// <returns>Texture.</returns>
        private Texture GetTexture(TexturesID Id)
        {
            return mTextures[(int)Id];
        }
        #endregion

        #region - GetTextureName -
        /// <summary>
        /// Get's a Texture's Name.
        /// </summary>
        /// <param name="Id">Id of the Texture.</param>
        /// <returns>Texture's Name.</returns>
        public string GetTextureName(TexturesID Id)
        {
            return mTextureNames[(int)Id];
        }
        #endregion

        #region - CreateTexture -
        /// <summary>
        /// Create's an Texture.
        /// </summary>
        /// <param name="Texture">Our Texture</param>
        /// <param name="Name">Texture's Name.</param>
        /// <param name="Size">Texture's Size.</param>
        /// <returns>false if there is a problem. </returns>
        private bool CreateTexture(Texture Texture, string Name, Size Size)
        {
            try
            {
                Axiom.Core.TextureManager.Instance.Remove(Name);

                Texture = Axiom.Core.TextureManager.Instance.CreateManual(
                    Name,
                    "HYDRAX_RESOURCE_GROUP",
                    TextureType.TwoD,
                    Size.Width,
                    Size.Height,
                    0,
                    PixelFormat.BYTE_BGRA,
                    TextureUsage.DynamicWriteOnlyDiscardable);

                Texture.Load();
            }
            catch(Exception Ex)
            {
                Hydrax.HydraxLog(Ex.Message);
                return false;
            }
            return true;
        }
        #endregion
        #endregion

    }//end class
    #endregion
}//end namespace
#endregion