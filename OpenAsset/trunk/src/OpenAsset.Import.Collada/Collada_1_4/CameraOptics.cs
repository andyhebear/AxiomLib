using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class CameraOptics : ColladaBase 
{

    private CameraOpticsTechniqueCommon technique_commonField;

    private List<Technique> techniqueField;

    private List<Extra> extraField;

    /// <remarks/>
    public CameraOpticsTechniqueCommon technique_common
    {
        get
        {
            return this.technique_commonField;
        }
        set
        {
            this.technique_commonField = value;
            this.RaisePropertyChanged("technique_common");
        }
    }

    /// <remarks/>
    [XmlElementAttribute("technique")]
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
    [XmlElementAttribute("extra")]
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
