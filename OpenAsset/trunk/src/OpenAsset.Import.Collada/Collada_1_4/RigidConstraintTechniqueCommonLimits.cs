using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class RigidConstraintTechniqueCommonLimits : ColladaBase
{

    private RigidConstraintTechniqueCommonLimitsSwingConeAndTwist swing_cone_and_twistField;

    private RigidConstraintTechniqueCommonLimitsLinear linearField;

    /// <remarks/>
    public RigidConstraintTechniqueCommonLimitsSwingConeAndTwist swing_cone_and_twist
    {
        get
        {
            return this.swing_cone_and_twistField;
        }
        set
        {
            this.swing_cone_and_twistField = value;
            this.RaisePropertyChanged("swing_cone_and_twist");
        }
    }

    /// <remarks/>
    public RigidConstraintTechniqueCommonLimitsLinear linear
    {
        get
        {
            return this.linearField;
        }
        set
        {
            this.linearField = value;
            this.RaisePropertyChanged("linear");
        }
    }
}
