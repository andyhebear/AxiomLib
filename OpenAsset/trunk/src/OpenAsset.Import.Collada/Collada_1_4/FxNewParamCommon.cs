using System.Xml.Serialization;
using System.ComponentModel;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class FxNewParamCommon : ColladaBase
{

    private FxAnnotateCommon annotateField;

    private string sidField;

    private string semanticField;

    private FxModifierEnumCommon modifierField;

    /// <remarks/>
    [XmlElement("annotate")]
    public FxAnnotateCommon annotate
    {
        get
        {
            return this.annotateField;
        }
        set
        {
            this.annotateField = value;
            this.RaisePropertyChanged("annotate");
        }

    }

    /// <remarks/>
    [XmlElement("semantic")]
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