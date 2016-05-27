
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class LightTechniqueCommon : ColladaBase
{

    private object itemField;

    /// <remarks/>
    [XmlElement("ambient", typeof(LightTechniqueCommonAmbient))]
    [XmlElement("directional", typeof(LightTechniqueCommonDirectional))]
    [XmlElement("point", typeof(LightTechniqueCommonPoint))]
    [XmlElement("spot", typeof(LightTechniqueCommonSpot))]
    public object Item
    {
        get
        {
            return this.itemField;
        }
        set
        {
            this.itemField = value;
            this.RaisePropertyChanged("Item");
        }
    }
}

