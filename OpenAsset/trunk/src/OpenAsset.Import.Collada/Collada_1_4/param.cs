
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Param : ColladaBase {
    
    private string nameField;
    
    private string sidField;
    
    private string semanticField;
    
    private string typeField;
    
    private string valueField;
    
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
    [XmlAttributeAttribute(DataType="NMTOKEN")]
    public string semantic {
        get {
            return this.semanticField;
        }
        set {
            this.semanticField = value;
            this.RaisePropertyChanged("semantic");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="NMTOKEN")]
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
            this.RaisePropertyChanged("type");
        }
    }
    
    /// <remarks/>
    [XmlTextAttribute()]
    public string Value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
            this.RaisePropertyChanged("Value");
        }
    }
}
