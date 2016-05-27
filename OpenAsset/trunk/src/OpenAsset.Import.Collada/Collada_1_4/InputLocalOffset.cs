
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class InputLocalOffset : ColladaBase
{

    private ulong offsetField;

    private string semanticField;

    private string sourceField;

    private ulong setField;

    private bool setFieldSpecified;

    /// <remarks/>
    [XmlAttributeAttribute()]
    public ulong offset
    {
        get
        {
            return this.offsetField;
        }
        set
        {
            this.offsetField = value;
            this.RaisePropertyChanged("offset");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NMTOKEN")]
    public string semantic
    {
        get
        {
            return this.semanticField;
        }
        set
        {
            this.semanticField = value;
            this.RaisePropertyChanged("semantic");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute()]
    public string source
    {
        get
        {
            return this.sourceField;
        }
        set
        {
            this.sourceField = value;
            this.RaisePropertyChanged("source");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute()]
    public ulong set
    {
        get
        {
            return this.setField;
        }
        set
        {
            this.setField = value;
            this.RaisePropertyChanged("set");
        }
    }

    /// <remarks/>
    [XmlIgnoreAttribute()]
    public bool setSpecified
    {
        get
        {
            return this.setFieldSpecified;
        }
        set
        {
            this.setFieldSpecified = value;
            this.RaisePropertyChanged("setSpecified");
        }
    }
}
