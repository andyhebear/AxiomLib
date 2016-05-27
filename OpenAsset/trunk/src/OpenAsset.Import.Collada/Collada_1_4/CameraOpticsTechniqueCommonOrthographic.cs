
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class CameraOpticsTechniqueCommonOrthographic : ColladaBase
{

    private List<TargetableFloat> itemsField;

    private List<ItemsChoiceType> itemsElementNameField;

    private TargetableFloat znearField;

    private TargetableFloat zfarField;

    /// <remarks/>
    [XmlElement("aspect_ratio", typeof(TargetableFloat))]
    [XmlElement("xmag", typeof(TargetableFloat))]
    [XmlElement("ymag", typeof(TargetableFloat))]
    [XmlChoiceIdentifierAttribute("ItemsElementName")]
    public List<TargetableFloat> Items
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
    [XmlElement("ItemsElementName")]
    [XmlIgnoreAttribute()]
    public List<ItemsChoiceType> ItemsElementName
    {
        get
        {
            return this.itemsElementNameField;
        }
        set
        {
            this.itemsElementNameField = value;
            this.RaisePropertyChanged("ItemsElementName");
        }
    }

    /// <remarks/>
    public TargetableFloat znear
    {
        get
        {
            return this.znearField;
        }
        set
        {
            this.znearField = value;
            this.RaisePropertyChanged("znear");
        }
    }

    /// <remarks/>
    public TargetableFloat zfar
    {
        get
        {
            return this.zfarField;
        }
        set
        {
            this.zfarField = value;
            this.RaisePropertyChanged("zfar");
        }
    }
}
