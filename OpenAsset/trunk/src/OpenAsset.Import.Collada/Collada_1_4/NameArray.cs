
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false)]
public partial class NameArray : ColladaBase {
    
    private string idField;
    
    private string nameField;
    
    private ulong countField;
    
    private List<string> textField;
    
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
    
    /// <remarks/>
    [XmlAttributeAttribute(DataType="NCName")]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
            this.RaisePropertyChanged("name");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute()]
    public ulong count {
        get {
            return this.countField;
        }
        set {
            this.countField = value;
            this.RaisePropertyChanged("count");
        }
    }
    
    /// <remarks/>
    [XmlTextAttribute(DataType="Name")]
    public List<string> Text {
        get {
            return this.textField;
        }
        set {
            this.textField = value;
            this.RaisePropertyChanged("Text");
        }
    }
}
