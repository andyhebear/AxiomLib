
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class CameraOpticsTechniqueCommon : ColladaBase
{

    private object itemField;

    /// <remarks/>
    [XmlElement("orthographic", typeof(CameraOpticsTechniqueCommonOrthographic))]
    [XmlElement("perspective", typeof(CameraOpticsTechniqueCommonPerspective))]
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
