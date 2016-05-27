
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class InstanceWithExtra : ColladaBase
{

    private List<Extra> extraField;

    private string urlField;

    private string sidField;

    private string nameField;

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
    [XmlAttributeAttribute(DataType = "anyURI")]
    public string url
    {
        get
        {
            return this.urlField;
        }
        set
        {
            this.urlField = value;
            this.RaisePropertyChanged("url");
        }
    }

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
