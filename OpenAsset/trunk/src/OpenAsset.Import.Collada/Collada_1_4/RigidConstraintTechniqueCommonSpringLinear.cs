using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class RigidConstraintTechniqueCommonSpringLinear : ColladaBase
{

    private TargetableFloat stiffnessField;

    private TargetableFloat dampingField;

    private TargetableFloat target_valueField;

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type TargetableFloat is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='1.0' attribute.
    public TargetableFloat stiffness
    {
        get
        {
            return this.stiffnessField;
        }
        set
        {
            this.stiffnessField = value;
            this.RaisePropertyChanged("stiffness");
        }
    }

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type TargetableFloat is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='0.0' attribute.
    public TargetableFloat damping
    {
        get
        {
            return this.dampingField;
        }
        set
        {
            this.dampingField = value;
            this.RaisePropertyChanged("damping");
        }
    }

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type TargetableFloat is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='0.0' attribute.
    public TargetableFloat target_value
    {
        get
        {
            return this.target_valueField;
        }
        set
        {
            this.target_valueField = value;
            this.RaisePropertyChanged("target_value");
        }
    }
}
