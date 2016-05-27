
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class InstanceMaterialBind : ColladaBase
{

    private string semanticField;

    private string targetField;

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NCName")]
    public string semantic
    {
        get
        {
            return this.semanticField;
        }
        set
        {
            this.semanticField = value;
            this.RaisePropertyChanged("semantic");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "token")]
    public string target
    {
        get
        {
            return this.targetField;
        }
        set
        {
            this.targetField = value;
            this.RaisePropertyChanged("target");
        }
    }
}
