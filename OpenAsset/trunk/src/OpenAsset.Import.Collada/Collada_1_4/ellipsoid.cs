
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="ellipsoid")]
public partial class Ellipsoid : ColladaBase {
    
    private string sizeField;
    
    /// <remarks/>
    public string size {
        get {
            return this.sizeField;
        }
        set {
            this.sizeField = value;
            this.RaisePropertyChanged("size");
        }
    }
    
}
