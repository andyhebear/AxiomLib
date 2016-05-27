
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false, ElementName = "accessor")]
public partial class Accessor :  ColladaBase {
    
    private List<Param> paramField;
    
    private ulong countField;
    
    private ulong offsetField;
    
    private string sourceField;
    
    private ulong strideField;
    
    public Accessor() {
        this.offsetField = ((ulong)(0m));
        this.strideField = ((ulong)(1m));
    }
    
    /// <remarks/>
    [XmlElement("param")]
    public List<Param> param {
        get {
            return this.paramField;
        }
        set {
            this.paramField = value;
            this.RaisePropertyChanged("param");
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
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(typeof(ulong), "0")]
    public ulong offset {
        get {
            return this.offsetField;
        }
        set {
            this.offsetField = value;
            this.RaisePropertyChanged("offset");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="anyURI")]
    public string source {
        get {
            return this.sourceField;
        }
        set {
            this.sourceField = value;
            this.RaisePropertyChanged("source");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(typeof(ulong), "1")]
    public ulong stride {
        get {
            return this.strideField;
        }
        set {
            this.strideField = value;
            this.RaisePropertyChanged("stride");
        }
    }    
}
