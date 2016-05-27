
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class LightTechniqueCommonPoint : ColladaBase
{

    private TargetableFloat3 colorField;

    private TargetableFloat constant_attenuationField;

    private TargetableFloat linear_attenuationField;

    private TargetableFloat quadratic_attenuationField;

    /// <remarks/>
    public TargetableFloat3 color
    {
        get
        {
            return this.colorField;
        }
        set
        {
            this.colorField = value;
            this.RaisePropertyChanged("color");
        }
    }

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type TargetableFloat is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='1.0' attribute.
    public TargetableFloat constant_attenuation
    {
        get
        {
            return this.constant_attenuationField;
        }
        set
        {
            this.constant_attenuationField = value;
            this.RaisePropertyChanged("constant_attenuation");
        }
    }

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type TargetableFloat is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='0.0' attribute.
    public TargetableFloat linear_attenuation
    {
        get
        {
            return this.linear_attenuationField;
        }
        set
        {
            this.linear_attenuationField = value;
            this.RaisePropertyChanged("linear_attenuation");
        }
    }

    /// <remarks/>
    // CODEGEN Warning: DefaultValue attribute on members of type TargetableFloat is not supported in this version of the .Net Framework.
    // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='0.0' attribute.
    public TargetableFloat quadratic_attenuation
    {
        get
        {
            return this.quadratic_attenuationField;
        }
        set
        {
            this.quadratic_attenuationField = value;
            this.RaisePropertyChanged("quadratic_attenuation");
        }
    }
}
