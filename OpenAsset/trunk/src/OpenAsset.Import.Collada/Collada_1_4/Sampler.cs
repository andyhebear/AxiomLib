
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class Sampler : ColladaBase {
    
    private List<InputLocal> inputField;
    
    private string idField;
    
    /// <remarks/>
    [XmlElement("input")]
    public List<InputLocal> input {
        get {
            return this.inputField;
        }
        set {
            this.inputField = value;
            this.RaisePropertyChanged("input");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="ID")]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
            this.RaisePropertyChanged("id");
        }
    }
}
