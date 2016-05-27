using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class BindMaterial : ColladaBase
{

    private List<Param> paramField;

    private List<InstanceMaterial> technique_commonField;

    private List<Technique> techniqueField;

    private List<Extra> extraField;

    /// <remarks/>
    [XmlElement("param")]
    public List<Param> param
    {
        get
        {
            return this.paramField;
        }
        set
        {
            this.paramField = value;
            this.RaisePropertyChanged("param");
        }
    }

    /// <remarks/>
    [XmlArrayItemAttribute("instance_material", IsNullable = false)]
    public List<InstanceMaterial> technique_common
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

