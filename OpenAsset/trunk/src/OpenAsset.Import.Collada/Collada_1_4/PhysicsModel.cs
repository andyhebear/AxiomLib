
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class PhysicsModel : ColladaBase {
    
    private Asset assetField;
    
    private List<RigidBody> rigid_bodyField;
    
    private List<RigidConstraint>rigid_constraintField;
    
    private List<InstancePhysicsModel> instance_physics_modelField;
    
    private List<Extra> extraField;
    
    private string idField;
    
    private string nameField;
    
    /// <remarks/>
    public Asset asset {
        get {
            return this.assetField;
        }
        set {
            this.assetField = value;
            this.RaisePropertyChanged("asset");
        }
    }
    
    /// <remarks/>
    [XmlElement("rigid_body")]
    public List<RigidBody> rigid_body {
        get {
            return this.rigid_bodyField;
        }
        set {
            this.rigid_bodyField = value;
            this.RaisePropertyChanged("rigid_body");
        }
    }
    
    /// <remarks/>
    [XmlElement("rigid_constraint")]
    public List<RigidConstraint>rigid_constraint {
        get {
            return this.rigid_constraintField;
        }
        set {
            this.rigid_constraintField = value;
            this.RaisePropertyChanged("rigid_constraint");
        }
    }
    
    /// <remarks/>
    [XmlElement("instance_physics_model")]
    public List<InstancePhysicsModel> instance_physics_model {
        get {
            return this.instance_physics_modelField;
        }
        set {
            this.instance_physics_modelField = value;
            this.RaisePropertyChanged("instance_physics_model");
        }
    }
    
    /// <remarks/>
    [XmlElement("extra")]
    public List<Extra> extra {
        get {
            return this.extraField;
        }
        set {
            this.extraField = value;
            this.RaisePropertyChanged("extra");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="ID")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
            this.RaisePropertyChanged("id");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="NCName")]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
            this.RaisePropertyChanged("name");
        }
    }
}
