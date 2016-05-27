using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class RigidBodyTechniqueCommon : ColladaBase
{

    private RigidBodyTechniqueCommonDynamic dynamicField;

    private TargetableFloat massField;

    private List<object> mass_frameField;

    private TargetableFloat3 inertiaField;

    private object itemField;

    private List<RigidBodyTechniqueCommonShape> shapeField;

    /// <remarks/>
    public RigidBodyTechniqueCommonDynamic dynamic
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
    public List<RigidBodyTechniqueCommonShape> shape
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

