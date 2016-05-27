
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class InstanceRigidBodyTechniqueCommon : ColladaBase
{

    private string angular_velocityField;

    private string velocityField;

    private InstanceRigidBodyTechniqueCommonDynamic dynamicField;

    private TargetableFloat massField;

    private List<object> mass_frameField;

    private TargetableFloat3 inertiaField;

    private object itemField;

    private List<InstanceRigidBodyTechniqueCommonShape> shapeField;

    public InstanceRigidBodyTechniqueCommon()
    {
        this.angular_velocityField = "0.0 0.0 0.0";
        this.velocityField = "0.0 0.0 0.0";
    }

    /// <remarks/>
    [DefaultValueAttribute("0.0 0.0 0.0")]
    public string angular_velocity
    {
        get
        {
            return this.angular_velocityField;
        }
        set
        {
            this.angular_velocityField = value;
            this.RaisePropertyChanged("angular_velocity");
        }
    }

    /// <remarks/>
    [DefaultValueAttribute("0.0 0.0 0.0")]
    public string velocity
    {
        get
        {
            return this.velocityField;
        }
        set
        {
            this.velocityField = value;
            this.RaisePropertyChanged("velocity");
        }
    }

    /// <remarks/>
    public InstanceRigidBodyTechniqueCommonDynamic dynamic
    {
        get
        {
            return this.dynamicField;
        }
        set
        {
            this.dynamicField = value;
            this.RaisePropertyChanged("dynamic");
        }
    }

    /// <remarks/>
    public TargetableFloat mass
    {
        get
        {
            return this.massField;
        }
        set
        {
            this.massField = value;
            this.RaisePropertyChanged("mass");
        }
    }

    /// <remarks/>
    [XmlArrayItemAttribute("rotate", typeof(Rotate), IsNullable = false)]
    [XmlArrayItemAttribute("translate", typeof(TargetableFloat3), IsNullable = false)]
    public List<object> mass_frame
    {
        get
        {
            return this.mass_frameField;
        }
        set
        {
            this.mass_frameField = value;
            this.RaisePropertyChanged("mass_frame");
        }
    }

    /// <remarks/>
    public TargetableFloat3 inertia
    {
        get
        {
            return this.inertiaField;
        }
        set
        {
            this.inertiaField = value;
            this.RaisePropertyChanged("inertia");
        }
    }

    /// <remarks/>
    [XmlElement("instance_physics_material", typeof(InstanceWithExtra))]
    [XmlElement("physics_material", typeof(PhysicsMaterial))]
    public object Item
    {
        get
        {
            return this.itemField;
        }
        set
        {
            this.itemField = value;
            this.RaisePropertyChanged("Item");
        }
    }

    /// <remarks/>
    [XmlElement("shape")]
    public List<InstanceRigidBodyTechniqueCommonShape> shape
    {
        get
        {
            return this.shapeField;
        }
        set
        {
            this.shapeField = value;
            this.RaisePropertyChanged("shape");
        }
    }
}
