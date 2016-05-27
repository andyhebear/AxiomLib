
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>

[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false, ElementName = "int_array")]
public partial class IntArray : ColladaBase {
    
    private string idField;
    
    private string nameField;
    
    private ulong countField;
    
    private string minInclusiveField;
    
    private string maxInclusiveField;
    
    private List<long> textField;
    
    public IntArray() {
        this.minInclusiveField = "-2147483648";
        this.maxInclusiveField = "2147483647";
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
    [XmlAttributeAttribute(DataType="integer")]
    [DefaultValueAttribute("-2147483648")]
    public string minInclusive {
        get {
            return this.minInclusiveField;
        }
        set {
            this.minInclusiveField = value;
            this.RaisePropertyChanged("minInclusive");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="integer")]
    [DefaultValueAttribute("2147483647")]
    public string maxInclusive {
        get {
            return this.maxInclusiveField;
        }
        set {
            this.maxInclusiveField = value;
            this.RaisePropertyChanged("maxInclusive");
        }
    }
    
    /// <remarks/>
    [XmlTextAttribute()]
    public List<long> Text {
        get {
            return this.textField;
        }
        set {
            this.textField = value;
            this.RaisePropertyChanged("Text");
        }
    }
}
