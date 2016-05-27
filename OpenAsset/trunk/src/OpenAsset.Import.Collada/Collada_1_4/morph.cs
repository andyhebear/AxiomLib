
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Morph : ColladaBase {
    
    private List<Source> sourceField;
    
    private MorphTargets targetsField;
    
    private List<Extra> extraField;
    
    private MorphMethodType methodField;
    
    private string source1Field;
    
    public Morph() {
        this.methodField = MorphMethodType.NORMALIZED;
    }
    
    /// <remarks/>
    [XmlElement("source")]
    public List<Source> source {
        get {
            return this.sourceField;
        }
        set {
            this.sourceField = value;
            this.RaisePropertyChanged("source");
        }
    }
    
    /// <remarks/>
    public MorphTargets targets {
        get {
            return this.targetsField;
        }
        set {
            this.targetsField = value;
            this.RaisePropertyChanged("targets");
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
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(MorphMethodType.NORMALIZED)]
    public MorphMethodType method {
        get {
            return this.methodField;
        }
        set {
            this.methodField = value;
            this.RaisePropertyChanged("method");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute("source", DataType="anyURI")]
    public string source1 {
        get {
            return this.source1Field;
        }
        set {
            this.source1Field = value;
            this.RaisePropertyChanged("source1");
        }
    }
}
