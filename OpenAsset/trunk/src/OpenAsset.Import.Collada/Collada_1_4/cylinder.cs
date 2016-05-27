
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>

[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false, ElementName = "cylinder")]
public partial class Cylinder : ColladaBase {
    
    private double heightField;
    
    private string radiusField;
    
    private List<Extra> extraField;
    
    /// <remarks/>
    public double height {
        get {
            return this.heightField;
        }
        set {
            this.heightField = value;
            this.RaisePropertyChanged("height");
        }
    }
    
    /// <remarks/>
    public string radius {
        get {
            return this.radiusField;
        }
        set {
            this.radiusField = value;
            this.RaisePropertyChanged("radius");
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
