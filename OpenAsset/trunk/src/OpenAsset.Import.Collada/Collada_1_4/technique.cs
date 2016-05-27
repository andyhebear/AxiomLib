using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Technique : ColladaBase {
    
    private List<System.Xml.XmlElement> anyField;
    
    private string profileField;
    
    /// <remarks/>
    [XmlAnyElementAttribute()]
    public List<System.Xml.XmlElement> Any {
        get {
            return this.anyField;
        }
        set {
            this.anyField = value;
            this.RaisePropertyChanged("Any");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="NMTOKEN")]
    public string profile {
        get {
            return this.profileField;
        }
        set {
            this.profileField = value;
            this.RaisePropertyChanged("profile");
        }
    }
}
