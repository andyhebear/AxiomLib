using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false, ElementName = "library_visual_scenes")]
public partial class LibraryMaterials : ColladaBase
{
    private Asset assetField;

    private List<Effect> effectField;

    private List<Extra> extraField;

    private string idField;

    private string nameField;

    /// <remarks/>
    [XmlElement("asset")]
    public Asset asset
    {
        get
        {
            return this.assetField;
        }
        set
        {
            this.assetField = value;
            this.RaisePropertyChanged("asset");
        }
    }

    /// <remarks/>
    [XmlElement("effect")]
    public List<Effect> effect
    {
        get
        {
            return this.effectField;
        }
        set
        {
            this.effectField = value;
            this.RaisePropertyChanged("effect");
        }
    }

    /// <remarks/>
    [XmlElement("extra")]
    public List<Extra> extra
    {
        get
        {
            return this.extraField;
        }
        set
        {
            this.extraField = value;
            this.RaisePropertyChanged("extra");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "ID")]
    public string id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
            this.RaisePropertyChanged("id");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NCName")]
    public string name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
            this.RaisePropertyChanged("name");
        }
    }
}