using System;

namespace Axiom.Component.OpenAsset
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AssetPlugInAttribute : Attribute
    {
        public AssetPlugInAttribute(string description)
        {
            m_description = description;
        }

        private string m_description;

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }
    }
}
