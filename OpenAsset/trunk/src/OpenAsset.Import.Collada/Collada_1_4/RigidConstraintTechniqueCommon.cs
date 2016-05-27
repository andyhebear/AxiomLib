using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class RigidConstraintTechniqueCommon : ColladaBase
{

    private RigidConstraintTechniqueCommonEnabled enabledField;

    private RigidConstraintTechniqueCommonInterpenetrate interpenetrateField;

    private RigidConstraintTechniqueCommonLimits limitsField;

    private RigidConstraintTechniqueCommonSpring springField;

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type rigid_constraintTechnique_commonEnabled is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='true' attribute.
    public RigidConstraintTechniqueCommonEnabled enabled
    {
        get
        {
            return this.enabledField;
        }
        set
        {
            this.enabledField = value;
            this.RaisePropertyChanged("enabled");
        }
    }

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type rigid_constraintTechnique_commonInterpenetrate is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='false' attribute.
    public RigidConstraintTechniqueCommonInterpenetrate interpenetrate
    {
        get
        {
            return this.interpenetrateField;
        }
        set
        {
            this.interpenetrateField = value;
            this.RaisePropertyChanged("interpenetrate");
        }
    }

    /// <remarks/>
    public RigidConstraintTechniqueCommonLimits limits
    {
        get
        {
            return this.limitsField;
        }
        set
        {
            this.limitsField = value;
            this.RaisePropertyChanged("limits");
        }
    }

    /// <remarks/>
    public RigidConstraintTechniqueCommonSpring spring
    {
        get
        {
            return this.springField;
        }
        set
        {
            this.springField = value;
            this.RaisePropertyChanged("spring");
        }
    }
}
