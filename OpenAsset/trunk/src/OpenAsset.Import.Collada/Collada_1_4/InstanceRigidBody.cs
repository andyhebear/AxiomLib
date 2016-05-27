
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class InstanceRigidBody : ColladaBase {
    
    private InstanceRigidBodyTechniqueCommon technique_commonField;
    
    private List<Technique> techniqueField;
    
    private List<Extra> extraField;
    
    private string bodyField;
    
    private string sidField;
    
    private string nameField;
    
    private string targetField;
    
    /// <remarks/>
    public InstanceRigidBodyTechniqueCommon technique_common {
        get {
            return this.technique_commonField;
        }
        set {
            this.technique_commonField = value;
            this.RaisePropertyChanged("technique_common");
        }
    }
    
    /// <remarks/>
    [XmlElement("technique")]
    public List<Technique> technique {
        get {
            return this.techniqueField;
        }
        set {
            this.techniqueField = value;
            this.RaisePropertyChanged("technique");
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
    [XmlAttributeAttribute(DataType="NCName")]
    public string body {
        get {
            return this.bodyField;
        }
        set {
            this.bodyField = value;
            this.RaisePropertyChanged("body");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="NCName")]
    public string sid {
        get {
            return this.sidField;
        }
        set {
            this.sidField = value;
            this.RaisePropertyChanged("sid");
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
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="anyURI")]
    public string target {
        get {
            return this.targetField;
        }
        set {
            this.targetField = value;
            this.RaisePropertyChanged("target");
        }
    }
}
