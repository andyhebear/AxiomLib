#region LGPL License
/*
Axiom Graphics Engine Library
Copyright (C) 2003-2010 Axiom Project Team

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


#region Namespace Declarations
using System;
using System.Globalization;
using System.Xml;
using Axiom.Core;
using Axiom.Math;
#endregion Namespace Declarations

namespace Axiom.Components.DotScene
{

    /// <summary>
    /// Contains utility functions for extracting certain standardized types of data from DotScene xml
    /// </summary>
    static class DotSceneXmlUtility
    {

        #region Static Members

        private static int _uniqueIdNumber = 0;

        #endregion


        #region Internal Methods


        /// <summary>
        /// Retrieves value of a given xml node attribute or throws an exception if attribute not found.
        /// </summary>
        /// <param name="xNode">XmlNode to parse, must not be null.</param>
        /// <param name="attributeName">Attribute to search for.</param>
        /// <returns>A string containing the attribute's value.</returns>
        internal static float RetrieveXmlAttributeValue(XmlNode xNode, string attributeName)
        {

            XmlAttribute xmlAttrib = xNode.Attributes[attributeName];
            if (xmlAttrib != null)
            {
                return Convert.ToSingle(xmlAttrib.Value, CultureInfo.InvariantCulture);
            }

            throw new Exception(string.Format("Attribute not found. Xml node: '{0}' Attribute expected: '{1}'.", xNode.Name, attributeName));
        }


        /// <summary>
        /// Retrieves value of a given xml node attribute or a default value.
        /// </summary>
        /// <param name="xNode">XmlNode to parse, must not be null.</param>
        /// <param name="attributeName">Attribute to search for.</param>
        /// <param name="defaultValue">Value to return if the attribute doesn't exist.</param>
        /// <returns>A string containing either the attribute's value or user defined defaultValue.</returns>
        internal static string RetrieveXmlAttributeValue(XmlNode xNode, string attributeName, string defaultValue)
        {

            XmlAttribute xmlAttrib = xNode.Attributes[attributeName];
            if (xmlAttrib != null)
            {
                return xmlAttrib.Value;
            }

            return defaultValue;
        }


        internal static float RetrieveXmlAttributeValue(XmlNode xNode, string attributeName, float defaultValue)
        {

            XmlAttribute xmlAttrib = xNode.Attributes[attributeName];
            if (xmlAttrib != null)
            {
                return Convert.ToSingle(xmlAttrib.Value, CultureInfo.InvariantCulture);
            }
            return defaultValue;
        }


        internal static bool RetrieveXmlAttributeValue(XmlNode xNode, string attributeName, bool defaultValue)
        {

            XmlAttribute xmlAttrib = xNode.Attributes[attributeName];
            if (xmlAttrib != null)
            {
                return Convert.ToBoolean(xmlAttrib.Value, CultureInfo.InvariantCulture);
            }

            return defaultValue;
        }



        /// <summary>
        /// Retrieves value of the "name" xml node attribute or generates an unique value when the "name" attribute doesn't exist.
        /// </summary>
        /// <param name="nodeToParse">XmlNode to parse.</param>
        /// <returns>A string containing the name.</returns>
        internal static string RetrieveOrGenerateNodeName(XmlNode nodeToParse)
        {
            try
            {            
                XmlAttribute nodeNameAttribute = nodeToParse.Attributes["name"];
                if (nodeNameAttribute != null)
                {
                    return nodeNameAttribute.Value;
                }

            }
            catch
            {
            }

            return "AutomaticallyNamedNode" + Convert.ToString(_uniqueIdNumber++, CultureInfo.InvariantCulture);
        }



        /// <summary>
        /// Retrieves a Vector3 object from the given node
        /// </summary>
        /// <param name="vectorXmlNode">XmlNode defining a Vector3</param>
        /// <returns>A populated Vector3 object</returns>
        internal static Vector3 RetrieveVector3(XmlNode vectorXmlNode)
        {

            Vector3 vector = new Vector3();
            vector.x = RetrieveXmlAttributeValue(vectorXmlNode, "x");
            vector.y = RetrieveXmlAttributeValue(vectorXmlNode, "y");
            vector.z = RetrieveXmlAttributeValue(vectorXmlNode, "z");
            return vector;

        }

        /// <summary>
        /// Retrieves a Quaternion object from the given node.
        /// </summary>
        /// <param name="quaternionXmlNode">XmlNode defining a Quaternion. Must not be null.</param>
        /// <returns>A populated Quaternion object</returns>
        internal static Quaternion RetrieveQuaternion(XmlNode quaternionXmlNode)
        {

            Quaternion quaternion = new Quaternion();
            quaternion.x = RetrieveXmlAttributeValue(quaternionXmlNode, "x");
            quaternion.y = RetrieveXmlAttributeValue(quaternionXmlNode, "y");
            quaternion.z = RetrieveXmlAttributeValue(quaternionXmlNode, "z");
            quaternion.w = RetrieveXmlAttributeValue(quaternionXmlNode, "w");

            return quaternion;
        }

        internal static Quaternion RetrieveRotation(XmlNode quaternionXmlNode)
        {

            Quaternion quaternion = new Quaternion();
            quaternion.x = RetrieveXmlAttributeValue(quaternionXmlNode, "qx");
            quaternion.y = RetrieveXmlAttributeValue(quaternionXmlNode, "qy");
            quaternion.z = RetrieveXmlAttributeValue(quaternionXmlNode, "qz");
            quaternion.w = RetrieveXmlAttributeValue(quaternionXmlNode, "qw");

            return quaternion;
        }

        /// <summary>
        /// Retrieves a quaternion object from the given node.
        /// </summary>
        /// <param name="quaternionXmlNode">XmlNode defining a Quaternion or null.</param>
        /// <param name="defaultValue">Default value to be returned when node is null.</param>
        /// <returns></returns>
        internal static Quaternion RetrieveQuaternion(XmlNode quaternionXmlNode, Quaternion defaultValue)
        {
            if (quaternionXmlNode != null)
            {
                return RetrieveQuaternion(quaternionXmlNode);
            }
            else
            {
                return defaultValue;
            }
        }

        internal static Quaternion RetrieveRotation(XmlNode quaternionXmlNode, Quaternion defaultValue)
        {
            if (quaternionXmlNode != null)
            {
                return RetrieveRotation(quaternionXmlNode);
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Retrieves a color object from the given node.
        /// </summary>
        /// <param name="colorXmlNode">XmlNode to parse, must not be null.</param>
        /// <returns></returns>
        internal static ColorEx RetrieveColor(XmlNode colorXmlNode)
        {

            ColorEx newColor = new ColorEx();
            newColor.r = RetrieveXmlAttributeValue(colorXmlNode, "r");
            newColor.g = RetrieveXmlAttributeValue(colorXmlNode, "g");
            newColor.b = RetrieveXmlAttributeValue(colorXmlNode, "b");
            newColor.a = RetrieveXmlAttributeValue(colorXmlNode, "a", 1.0f);

            return newColor;

        }


        /// <summary>
        /// Retrieves a color object from the given node.
        /// </summary>
        /// <param name="colorXmlNode">Node to parse or null.</param>
        /// <param name="defaultColor">Value to return if node is null.</param>
        /// <returns></returns>
        internal static ColorEx RetrieveColor(XmlNode colorXmlNode, ColorEx defaultColor)
        {

            if (colorXmlNode != null)
            {
                return RetrieveColor(colorXmlNode);
            }
            else
            {
                return defaultColor;
            }

        }

        #endregion

    }

}