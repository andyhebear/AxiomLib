
using System.Xml.Serialization;
using System.ComponentModel;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class AssetUnit : ColladaBase 
{

    private double meterField;

    private string nameField;

    public AssetUnit()
    {
        this.meterField = 1D;
        this.nameField = "meter";
    }

    /// <remarks/>
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(1D)]
    public double meter
    {
        get
        {
            return this.meterField;
        }
        set
        {
            this.meterField = value;
            this.RaisePropertyChanged("meter");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NMTOKEN")]
    [DefaultValueAttribute("meter")]
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
