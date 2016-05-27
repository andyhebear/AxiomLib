
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Node : ColladaBase {
    
    private Asset assetField;
    
    private List<object> itemsField;
    
    private List<ItemsChoiceType> itemsElementNameField;
    
    private List<InstanceWithExtra> instance_cameraField;
    
    private List<InstanceController> instance_controllerField;
    
    private List<InstanceGeometry> instance_geometryField;
    
    private List<InstanceWithExtra> instance_lightField;
    
    private List<InstanceWithExtra> instance_nodeField;
    
    private List<Node> node1Field;
    
    private List<Extra> extraField;
    
    private string idField;
    
    private string nameField;
    
    private string sidField;
    
    private NodeType typeField;
    
    private List<string> layerField;
    
    public Node() {
        this.typeField = NodeType.NODE;
    }
    
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
    [XmlElement("lookat", typeof(LookAt))]
    [XmlElement("matrix", typeof(Matrix))]
    [XmlElement("rotate", typeof(Rotate))]
    [XmlElement("scale", typeof(TargetableFloat3))]
    [XmlElement("skew", typeof(Skew))]
    [XmlElement("translate", typeof(TargetableFloat3))]
    [XmlChoiceIdentifierAttribute("ItemsElementName")]
    public List<object> Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
            this.RaisePropertyChanged("Items");
        }
    }
    
    /// <remarks/>
    [XmlElement("ItemsElementName")]
    [XmlIgnoreAttribute()]
    public List<ItemsChoiceType> ItemsElementName {
        get {
            return this.itemsElementNameField;
        }
        set {
            this.itemsElementNameField = value;
            this.RaisePropertyChanged("ItemsElementName");
        }
    }
    
    /// <remarks/>
    [XmlElement("instance_camera")]
    public List<InstanceWithExtra> instance_camera {
        get {
            return this.instance_cameraField;
        }
        set {
            this.instance_cameraField = value;
            this.RaisePropertyChanged("instance_camera");
        }
    }
    
    /// <remarks/>
    [XmlElement("instance_controller")]
    public List<InstanceController> instance_controller {
        get {
            return this.instance_controllerField;
        }
        set {
            this.instance_controllerField = value;
            this.RaisePropertyChanged("instance_controller");
        }
    }
    
    /// <remarks/>
    [XmlElement("instance_geometry")]
    public List<InstanceGeometry> instance_geometry {
        get {
            return this.instance_geometryField;
        }
        set {
            this.instance_geometryField = value;
            this.RaisePropertyChanged("instance_geometry");
        }
    }
    
    /// <remarks/>
    [XmlElement("instance_light")]
    public List<InstanceWithExtra> instance_light {
        get {
            return this.instance_lightField;
        }
        set {
            this.instance_lightField = value;
            this.RaisePropertyChanged("instance_light");
        }
    }
    
    /// <remarks/>
    [XmlElement("instance_node")]
    public List<InstanceWithExtra> instance_node {
        get {
            return this.instance_nodeField;
        }
        set {
            this.instance_nodeField = value;
            this.RaisePropertyChanged("instance_node");
        }
    }
    
    /// <remarks/>
    [XmlElement("node")]
    public List<Node> node1 {
        get {
            return this.node1Field;
        }
        set {
            this.node1Field = value;
            this.RaisePropertyChanged("node1");
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
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(NodeType.NODE)]
    public NodeType type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
            this.RaisePropertyChanged("type");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="Name")]
    public List<string> layer {
        get {
            return this.layerField;
        }
        set {
            this.layerField = value;
            this.RaisePropertyChanged("layer");
        }
    }
}
