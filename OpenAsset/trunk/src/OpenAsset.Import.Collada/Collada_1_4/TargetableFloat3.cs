using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class TargetableFloat3 : ColladaBase
{

    private string sidField;

    private List<double> textField;

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
    public List<double> Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
            this.RaisePropertyChanged("Text");
        }
    }
}
