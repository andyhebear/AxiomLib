
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class CameraImager : ColladaBase
{

    private List<Technique> techniqueField;

    private List<Extra> extraField;

    /// <remarks/>
    [XmlElement("technique")]
    public List<Technique> technique
    {
        get
        {
            return this.techniqueField;
        }
        set
        {
            this.techniqueField = value;
            this.RaisePropertyChanged("technique");
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
