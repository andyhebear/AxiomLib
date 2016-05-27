using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class SkinVertexWeights : ColladaBase
{

    private List<InputLocalOffset> inputField;

    private string vcountField;

    private string vField;

    private List<Extra> extraField;

    private ulong countField;

    /// <remarks/>
    [XmlElement("input")]
    public List<InputLocalOffset> input
    {
        get
        {
            return this.inputField;
        }
        set
        {
            this.inputField = value;
            this.RaisePropertyChanged("input");
        }
    }

    /// <remarks/>
    public string vcount
    {
        get
        {
            return this.vcountField;
        }
        set
        {
            this.vcountField = value;
            this.RaisePropertyChanged("vcount");
        }
    }

    /// <remarks/>
    public string v
    {
        get
        {
            return this.vField;
        }
        set
        {
            this.vField = value;
            this.RaisePropertyChanged("v");
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
    [XmlAttributeAttribute()]
    public ulong count
    {
        get
        {
            return this.countField;
        }
        set
        {
            this.countField = value;
            this.RaisePropertyChanged("count");
        }
    }
}
