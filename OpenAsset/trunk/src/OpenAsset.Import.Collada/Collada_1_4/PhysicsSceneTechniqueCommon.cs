using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class PhysicsSceneTechniqueCommon : ColladaBase
{

    private TargetableFloat3 gravityField;

    private TargetableFloat time_stepField;

    /// <remarks/>
    public TargetableFloat3 gravity
    {
        get
        {
            return this.gravityField;
        }
        set
        {
            this.gravityField = value;
            this.RaisePropertyChanged("gravity");
        }
    }

    /// <remarks/>
    public TargetableFloat time_step
    {
        get
        {
            return this.time_stepField;
        }
        set
        {
            this.time_stepField = value;
            this.RaisePropertyChanged("time_step");
        }
    }
}

