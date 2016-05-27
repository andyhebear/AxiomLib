/*
This file is part of Caelum for NeoAxis Engine.
Caelum for NeoAxisEngine is a Caelum's modified version.
See http://www.ogre3d.org/wiki/index.php/Caelum for the original version.

Copyright (c) 2008-2009 Association Hat. See Contributors.txt for details.

Caelum for NeoAxis Engine is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Caelum for NeoAxis Engine is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Caelum for NeoAxis Engine. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections.Generic;
using Engine;
using Engine.Utils;
using Engine.MathEx;
using Engine.EntitySystem;


namespace Caelum
{
    /// <summary>
    /// Draws a CheckedListBox which allows to tick some values
    /// from a enum (flags).
    /// </summary>
    public class EnumListBoxEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                CheckedListBox lb = new CheckedListBox();

                Type t = context.PropertyDescriptor.PropertyType;
                // Adds all value from the enum
                // and ticks which are selectionned
                foreach (object v in Enum.GetValues(t))
                {
                    lb.Items.Add(v, EnumContainsValue(t, v, value));
                }
                lb.CheckOnClick = true;
                // Draws the list in a DropDown
                edSvc.DropDownControl(lb);

                string ret = string.Empty;
                for (int i = 0; i < lb.CheckedItems.Count; i++)
                {
                    if (i > 0)
                        ret += ", ";
                    ret += lb.CheckedItems[i].ToString();
                }
                if (ret != string.Empty)
                {
                    return Enum.Parse(t, ret);
                }
                return Enum.Parse(t, "0");
            }
            return value;
        }

        /// <summary>
        /// Sets if a value is contained in an other enum value
        /// whose the type is specified type.</summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="enumValue">Value with type enumType</param>
        /// <param name="value">Value wanted</param>
        /// <returns>True if the value is contained in enumValue, else returns false</returns>
        private bool EnumContainsValue(Type enumType, object enumValue, object value)
        {
            Type systemType = Enum.GetUnderlyingType(enumType);

            if (systemType == typeof(int))
            {
                return ((int)enumValue & (int)value) == (int)enumValue;
            }
            else if (systemType == typeof(short))
            {
                return ((short)enumValue & (short)value) == (short)enumValue;
            }
            else if (systemType == typeof(long))
            {
                return ((long)enumValue & (long)value) == (long)enumValue;
            }
            else if (systemType == typeof(byte))
            {
                return ((byte)enumValue & (byte)value) == (byte)enumValue;
            }
            else if (systemType == typeof(uint))
            {
                return ((uint)enumValue & (uint)value) == (uint)enumValue;
            }
            else if (systemType == typeof(ushort))
            {
                return ((ushort)enumValue & (ushort)value) == (ushort)enumValue;
            }
            else if (systemType == typeof(ulong))
            {
                return ((ulong)enumValue & (ulong)value) == (ulong)enumValue;
            }
            else if (systemType == typeof(sbyte))
            {
                return ((sbyte)enumValue & (sbyte)value) == (sbyte)enumValue;
            }

            throw new Exception("Type incompatible");
        }
    }

    ///////////////////////////////////////////

    /// <summary>
    /// Contains all Caelum's types. A flag is
    /// set to allow <c>EnumListBoxEditor</c> creation.</summary>
    [Flags]
    public enum CaelumComponent
    {
        Skydome = 1 << 1,
        Sun = 1 << 3,
        Starfield = 1 << 5,
        Clouds = 1 << 6,
        Moon = 1 << 8,
        Fog = 1 << (16 + 0)
    };

    ///////////////////////////////////////////

    /// <summary>
    /// Describes an element of caelum which can be created
    /// in the world.</summary>
    public class CaelumItem
    {
        [EntityType.FieldSerialize]
        protected string mMeshName;

        [EntityType.FieldSerialize]
        protected string mMatName;

        [EntityType.FieldSerialize]
        protected CaelumComponent mType;

        [EntityType.FieldSerialize]
        protected Vec3 mScale;

        [EntityType.FieldSerialize]
        protected Vec3 mRotation;

        [EntityType.FieldSerialize]
        protected Vec3 mTranslation;

        [Editor(typeof(EditorMeshUITypeEditor), typeof(UITypeEditor))]
        public string Mesh
        {
            get { return mMeshName; }
            set { mMeshName = value; }
        }

        public string Material
        {
            get { return mMatName; }
            set { mMatName = value; }
        }

        [Editor(typeof(EnumListBoxEditor), typeof(UITypeEditor))]
        public CaelumComponent Type
        {
            get { return mType; }
            set { mType = value; }
        }

        public Vec3 Scale
        {
            get { return mScale; }
            set { mScale = value; }
        }

        public Vec3 Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }

        public Vec3 Translation
        {
            get { return mTranslation; }
            set { mTranslation = value; }
        }

        /// <summary>
        /// Checks if this element describes a type of caelum.</summary>
        /// <remarks>An element can describe more than one type.</remarks>
        /// <param name="type">The type to search</param>
        /// <returns>Returns true if this element contains 
        /// <paramref name="type"/>, else returns false</returns>
        public bool IsContainType(CaelumComponent type)
        {
            return IsContainType(this.Type, type);
        }

        /// <summary>
        /// Checks if an item describes a type of caelum.</summary>
        /// <remarks>An item can describe more than one type.</remarks>
        /// <param name="item">The item which may contain <paramref name="type"/></param>
        /// <param name="type">The type to search</param>
        /// <returns>Returns true if this item contains</returns>
        static public bool IsContainType(CaelumComponent item, CaelumComponent type)
        {
            if (item == (item | type))
                return true;

            return false;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    ///////////////////////////////////////////

    /// <summary>
    /// Defines some math tools. It also make all necessary 
    /// conversion for NAE (I prefer use Yaxis as UpAxis).</summary>
    public class CaelumUtils
    {
        // Static attributes -----------------------------------------------------------------

        /** Wraps OgreAxis to NAE axis
         *  So YAxis is the up axis and ZAxis is the deep axis
         */
        static public Vec3 XAxis = new Vec3(1, 0, 0);
        static public Vec3 ZAxis = new Vec3(0, 1, 0);
        static public Vec3 YAxis = new Vec3(0, 0, 1);

        static public Vec3 unit_Scale = new Vec3(1, 1, 1);

        /** Wraps some Colors to NAE
         *  @note: ColorBlack is equal to ColorValue.Zero
         */
        static public ColorValue ColorWhite = new ColorValue(1.0f, 1.0f, 1.0f);
        static public ColorValue ColorBlack = new ColorValue(0.0f, 0.0f, 0.0f);
        static public ColorValue ColorRed = new ColorValue(1.0f, 0.0f, 0.0f);
        static public ColorValue ColorGreen = new ColorValue(0.0f, 1.0f, 0.0f);
        static public ColorValue ColorBlue = new ColorValue(0.0f, 0.0f, 1.0f);

        // Static Methods --------------------------------------------------------------------

        /// <summary>
        /// Generates a quaternion from an axis and an Euler's angle in degrees.</summary>
        /// <param name="axis">The axis of the rotation</param>
        /// <param name="angle">An Euler's angle in degrees</param>
        static public Quat GenerateQuat(Vec3 axis, Degree angle)
        {
            return GenerateQuat(axis, angle.InRadians());
        }

        /// <summary>
        /// Generates a quaternion from an axis and an Euler's angle in radians.</summary>
        /// <param name="axis">The axis of the rotation</param>
        /// <param name="angle">An Euler's angle in Radian</param>
        static public Quat GenerateQuat(Vec3 axis, Radian angle)
        {
            float sin_a = MathFunctions.Sin(angle / 2);
            float cos_a = MathFunctions.Cos(angle / 2);

            Quat q = new Quat(axis * sin_a, cos_a);
            q.Normalize();

            return q;
        }
    }
}
