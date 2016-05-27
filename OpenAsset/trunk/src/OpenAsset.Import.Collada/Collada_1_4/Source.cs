
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Source : ColladaBase {
    
    private Asset assetField;
    
    private object itemField;
    
    private SourceTechniqueCommon technique_commonField;
    
    private List<Technique> techniqueField;
    
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
    [XmlElement("IDREF_array", typeof(IDREFArray))]
    [XmlElement("Name_array", typeof(NameArray))]
    [XmlElement("bool_array", typeof(BoolArray))]
    [XmlElement("float_array", typeof(FloatArray))]
    [XmlElement("int_array", typeof(IntArray))]
    public object Item {
        get {
            return this.itemField;
        }
        set {
            this.itemField = value;
            this.RaisePropertyChanged("Item");
        }
    }
    
    /// <remarks/>
    public SourceTechniqueCommon technique_common {
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
