
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class InstanceRigidBodyTechniqueCommonShape : ColladaBase
{

    private InstanceRigidBodyTechniqueCommonShapeHollow hollowField;

    private TargetableFloat massField;

    private TargetableFloat densityField;

    private object itemField;

    private object item1Field;

    private List<object> itemsField;

    private List<Extra> extraField;

    /// <remarks/>
    public InstanceRigidBodyTechniqueCommonShapeHollow hollow
    {
        get
        {
            return this.hollowField;
        }
        set
        {
            this.hollowField = value;
            this.RaisePropertyChanged("hollow");
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
    public TargetableFloat density
    {
        get
        {
            return this.densityField;
        }
        set
        {
            this.densityField = value;
            this.RaisePropertyChanged("density");
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
    [XmlElement("box", typeof(Box))]
    [XmlElement("capsule", typeof(Capsule))]
    [XmlElement("cylinder", typeof(Cylinder))]
    [XmlElement("instance_geometry", typeof(InstanceGeometry))]
    [XmlElement("plane", typeof(Plane))]
    [XmlElement("sphere", typeof(Sphere))]
    [XmlElement("tapered_capsule", typeof(TaperedCapsule))]
    [XmlElement("tapered_cylinder", typeof(TaperedCylinder))]
    public object Item1
    {
        get
        {
            return this.item1Field;
        }
        set
        {
            this.item1Field = value;
            this.RaisePropertyChanged("Item1");
        }
    }

    /// <remarks/>
    [XmlElement("rotate", typeof(Rotate))]
    [XmlElement("translate", typeof(TargetableFloat3))]
    public List<object> Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
            this.RaisePropertyChanged("Items");
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
