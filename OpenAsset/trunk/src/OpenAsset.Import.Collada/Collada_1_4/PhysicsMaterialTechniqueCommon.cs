
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class PhysicsMaterialTechniqueCommon : ColladaBase
{

    private TargetableFloat dynamic_frictionField;

    private TargetableFloat restitutionField;

    private TargetableFloat static_frictionField;

    /// <remarks/>
    public TargetableFloat dynamic_friction
    {
        get
        {
            return this.dynamic_frictionField;
        }
        set
        {
            this.dynamic_frictionField = value;
            this.RaisePropertyChanged("dynamic_friction");
        }
    }

    /// <remarks/>
    public TargetableFloat restitution
    {
        get
        {
            return this.restitutionField;
        }
        set
        {
            this.restitutionField = value;
            this.RaisePropertyChanged("restitution");
        }
    }

    /// <remarks/>
    public TargetableFloat static_friction
    {
        get
        {
            return this.static_frictionField;
        }
        set
        {
            this.static_frictionField = value;
            this.RaisePropertyChanged("static_friction");
        }
    }
}

