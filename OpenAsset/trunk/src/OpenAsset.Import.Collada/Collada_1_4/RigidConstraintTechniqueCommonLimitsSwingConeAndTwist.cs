using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class RigidConstraintTechniqueCommonLimitsSwingConeAndTwist : ColladaBase
{

    private TargetableFloat3 minField;

    private TargetableFloat3 maxField;

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type TargetableFloat3 is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='0.0 0.0 0.0' attribute.
    public TargetableFloat3 min
    {
        get
        {
            return this.minField;
        }
        set
        {
            this.minField = value;
            this.RaisePropertyChanged("min");
        }
    }

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type TargetableFloat3 is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='0.0 0.0 0.0' attribute.
    public TargetableFloat3 max
    {
        get
        {
            return this.maxField;
        }
        set
        {
            this.maxField = value;
            this.RaisePropertyChanged("max");
        }
    }
}
