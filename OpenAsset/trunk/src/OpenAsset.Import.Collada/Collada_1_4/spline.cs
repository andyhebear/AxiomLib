
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class spline : ColladaBase {
    
    private List<Source> sourceField;
    
    private SplineControlVertices control_verticesField;
    
    private List<Extra> extraField;
    
    private bool closedField;
    
    public spline() {
        this.closedField = false;
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
    public SplineControlVertices control_vertices {
        get {
            return this.control_verticesField;
        }
        set {
            this.control_verticesField = value;
            this.RaisePropertyChanged("control_vertices");
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
    [DefaultValueAttribute(false)]
    public bool closed {
        get {
            return this.closedField;
        }
        set {
            this.closedField = value;
            this.RaisePropertyChanged("closed");
        }
    }
}
