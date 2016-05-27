using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class TargetableFloat : ColladaBase
{

    private string sidField;

    private double valueField;

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
    public double Value
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
