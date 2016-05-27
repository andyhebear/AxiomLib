
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Polygons : ColladaBase {
    
    private List<InputLocalOffset> inputField;
    
    private List<object> itemsField;
    
    private List<Extra> extraField;
    
    private string nameField;
    
    private ulong countField;
    
    private string materialField;
    
    /// <remarks/>
    [XmlElement("input")]
    public List<InputLocalOffset> input {
        get {
            return this.inputField;
        }
        set {
            this.inputField = value;
            this.RaisePropertyChanged("input");
        }
    }
    
    /// <remarks/>
    [XmlElement("p", typeof(string))]
    [XmlElement("ph", typeof(PolygonsPH))]
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
    [XmlAttributeAttribute()]
    public ulong count {
        get {
            return this.countField;
        }
        set {
            this.countField = value;
            this.RaisePropertyChanged("count");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="NCName")]
    public string material {
        get {
            return this.materialField;
        }
        set {
            this.materialField = value;
            this.RaisePropertyChanged("material");
        }
    }
}
