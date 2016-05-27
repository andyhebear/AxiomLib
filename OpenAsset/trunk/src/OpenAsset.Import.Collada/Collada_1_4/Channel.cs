
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="channel")]
public partial class Channel : ColladaBase {
    
    private string sourceField;
    
    private string targetField;
    
    /// <remarks/>
    [XmlAttributeAttribute()]
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
    [XmlAttributeAttribute(DataType="token")]
    public string target {
        get {
            return this.targetField;
        }
        set {
            this.targetField = value;
            this.RaisePropertyChanged("target");
        }
    }
}
