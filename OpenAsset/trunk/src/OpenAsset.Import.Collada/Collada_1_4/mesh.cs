
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Mesh : ColladaBase {
    
    private List<Source> sourceField;
    
    private Vertices verticesField;
    
    private List<object> itemsField;
    
    private List<Extra> extraField;
    
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
    public Vertices vertices {
        get {
            return this.verticesField;
        }
        set {
            this.verticesField = value;
            this.RaisePropertyChanged("vertices");
        }
    }
    
    /// <remarks/>
    [XmlElement("lines", typeof(Lines))]
    [XmlElement("linestrips", typeof(Linestrips))]
    [XmlElement("polygons", typeof(Polygons))]
    [XmlElement("polylist", typeof(PolyList))]
    [XmlElement("triangles", typeof(Triangles))]
    [XmlElement("trifans", typeof(Trifans))]
    [XmlElement("tristrips", typeof(Tristrips))]
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
}
