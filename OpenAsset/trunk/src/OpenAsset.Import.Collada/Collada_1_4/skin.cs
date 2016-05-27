
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Skin : ColladaBase {
    
    private string bind_shape_matrixField;
    
    private List<Source> sourceField;
    
    private SkinJoints jointsField;
    
    private SkinVertexWeights vertex_weightsField;
    
    private List<Extra> extraField;
    
    private string source1Field;
    
    /// <remarks/>
    public string bind_shape_matrix {
        get {
            return this.bind_shape_matrixField;
        }
        set {
            this.bind_shape_matrixField = value;
            this.RaisePropertyChanged("bind_shape_matrix");
        }
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
    public SkinJoints joints {
        get {
            return this.jointsField;
        }
        set {
            this.jointsField = value;
            this.RaisePropertyChanged("joints");
        }
    }
    
    /// <remarks/>
    public SkinVertexWeights vertex_weights {
        get {
            return this.vertex_weightsField;
        }
        set {
            this.vertex_weightsField = value;
            this.RaisePropertyChanged("vertex_weights");
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
