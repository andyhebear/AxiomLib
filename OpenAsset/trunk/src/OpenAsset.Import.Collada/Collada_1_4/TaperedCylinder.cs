

using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class TaperedCylinder : ColladaBase {
    
    private double heightField;
    
    private string radius1Field;
    
    private string radius2Field;
    
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
    public string radius1 {
        get {
            return this.radius1Field;
        }
        set {
            this.radius1Field = value;
            this.RaisePropertyChanged("radius1");
        }
    }
    
    /// <remarks/>
    public string radius2 {
        get {
            return this.radius2Field;
        }
        set {
            this.radius2Field = value;
            this.RaisePropertyChanged("radius2");
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
