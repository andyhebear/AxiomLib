
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class MorphTargets : ColladaBase
{

    private List<InputLocal> inputField;

    private List<Extra> extraField;

    /// <remarks/>
    [XmlElement("input")]
    public List<InputLocal> input
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
}
