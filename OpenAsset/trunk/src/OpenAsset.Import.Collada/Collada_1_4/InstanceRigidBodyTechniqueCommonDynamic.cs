
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class InstanceRigidBodyTechniqueCommonDynamic : ColladaBase
{

    private string sidField;

    private bool valueField;

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NCName")]
    public string sid
    {
        get
        {
            return this.sidField;
        }
        set
        {
            this.sidField = value;
            this.RaisePropertyChanged("sid");
        }
    }

    /// <remarks/>
    [XmlTextAttribute()]
    public bool Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
            this.RaisePropertyChanged("Value");
        }
    }
}
