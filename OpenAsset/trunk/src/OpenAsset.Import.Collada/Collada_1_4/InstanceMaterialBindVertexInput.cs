
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class InstanceMaterialBindVertexInput : ColladaBase
{

    private string semanticField;

    private string input_semanticField;

    private ulong input_setField;

    private bool input_setFieldSpecified;

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NCName")]
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
    [XmlAttributeAttribute(DataType = "NCName")]
    public string input_semantic
    {
        get
        {
            return this.input_semanticField;
        }
        set
        {
            this.input_semanticField = value;
            this.RaisePropertyChanged("input_semantic");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute()]
    public ulong input_set
    {
        get
        {
            return this.input_setField;
        }
        set
        {
            this.input_setField = value;
            this.RaisePropertyChanged("input_set");
        }
    }

    /// <remarks/>
    [XmlIgnoreAttribute()]
    public bool input_setSpecified
    {
        get
        {
            return this.input_setFieldSpecified;
        }
        set
        {
            this.input_setFieldSpecified = value;
            this.RaisePropertyChanged("input_setSpecified");
        }
    }
}
