
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class PolyList : ColladaBase {
    
    private List<InputLocalOffset> inputField;
    
    private string vcountField;
    
    private string pField;
    
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
    public string vcount {
        get {
            return this.vcountField;
        }
        set {
            this.vcountField = value;
            this.RaisePropertyChanged("vcount");
        }
    }
    
    /// <remarks/>
    public string p {
        get {
            return this.pField;
        }
        set {
            this.pField = value;
            this.RaisePropertyChanged("p");
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
