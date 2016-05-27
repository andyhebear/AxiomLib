
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false, ElementName = "instance_controller")]
public partial class InstanceController : ColladaBase {
    
    private List<string> skeletonField;
    
    private BindMaterial bind_materialField;
    
    private List<Extra> extraField;
    
    private string urlField;
    
    private string sidField;
    
    private string nameField;
    
    /// <remarks/>
    [XmlElement("skeleton", DataType="anyURI")]
    public List<string> skeleton {
        get {
            return this.skeletonField;
        }
        set {
            this.skeletonField = value;
            this.RaisePropertyChanged("skeleton");
        }
    }
    
    /// <remarks/>
    public BindMaterial bind_material {
        get {
            return this.bind_materialField;
        }
        set {
            this.bind_materialField = value;
            this.RaisePropertyChanged("bind_material");
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
    [XmlAttributeAttribute(DataType="anyURI")]
    public string url {
        get {
            return this.urlField;
        }
        set {
            this.urlField = value;
            this.RaisePropertyChanged("url");
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
}
