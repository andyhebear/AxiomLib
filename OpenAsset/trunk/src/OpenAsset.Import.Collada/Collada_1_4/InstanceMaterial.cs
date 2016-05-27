
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="instance_material")]
public partial class InstanceMaterial : ColladaBase {
    
    private List<InstanceMaterialBind> bindField;
    
    private List<InstanceMaterialBindVertexInput> bind_vertex_inputField;
    
    private List<Extra> extraField;
    
    private string symbolField;
    
    private string targetField;
    
    private string sidField;
    
    private string nameField;
    
    /// <remarks/>
    [XmlElement("bind")]
    public List<InstanceMaterialBind> bind {
        get {
            return this.bindField;
        }
        set {
            this.bindField = value;
            this.RaisePropertyChanged("bind");
        }
    }
    
    /// <remarks/>
    [XmlElement("bind_vertex_input")]
    public List<InstanceMaterialBindVertexInput> bind_vertex_input {
        get {
            return this.bind_vertex_inputField;
        }
        set {
            this.bind_vertex_inputField = value;
            this.RaisePropertyChanged("bind_vertex_input");
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
    public string symbol {
        get {
            return this.symbolField;
        }
        set {
            this.symbolField = value;
            this.RaisePropertyChanged("symbol");
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
