using System.Xml.Serialization;
using System.ComponentModel;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class FxIncludeCommon : ColladaBase 
{
    private string sidField;

    private string urlField;

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NCName")]
    [DefaultValueAttribute("sid")]
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
    [XmlAttributeAttribute(DataType = "anyURI")]
    [DefaultValueAttribute("url")]
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
}
