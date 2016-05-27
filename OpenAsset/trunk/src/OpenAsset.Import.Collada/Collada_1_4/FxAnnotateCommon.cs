using System.Xml.Serialization;
using System.ComponentModel;


/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class  FxAnnotateCommon : ColladaBase
{
    private string sidField;

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

    }
