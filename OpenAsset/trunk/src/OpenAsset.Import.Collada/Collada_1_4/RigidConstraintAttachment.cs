using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class RigidConstraintAttachment : ColladaBase
{

    private List<object> itemsField;

    private string rigid_bodyField;

    /// <remarks/>
    [XmlElement("extra", typeof(Extra))]
    [XmlElement("rotate", typeof(Rotate))]
    [XmlElement("translate", typeof(TargetableFloat3))]
    public List<object> Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
            this.RaisePropertyChanged("Items");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "anyURI")]
    public string rigid_body
    {
        get
        {
            return this.rigid_bodyField;
        }
        set
        {
            this.rigid_bodyField = value;
            this.RaisePropertyChanged("rigid_body");
        }
    }
}
