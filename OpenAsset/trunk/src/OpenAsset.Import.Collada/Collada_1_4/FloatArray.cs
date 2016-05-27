
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="float_array")]
public partial class FloatArray : ColladaBase {
    
    private string idField;
    
    private string nameField;
    
    private ulong countField;
    
    private short digitsField;
    
    private short magnitudeField;
    
    private List<double> textField;
    
    public FloatArray() {
        this.digitsField = ((short)(6));
        this.magnitudeField = ((short)(38));
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
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(typeof(short), "6")]
    public short digits {
        get {
            return this.digitsField;
        }
        set {
            this.digitsField = value;
            this.RaisePropertyChanged("digits");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(typeof(short), "38")]
    public short magnitude {
        get {
            return this.magnitudeField;
        }
        set {
            this.magnitudeField = value;
            this.RaisePropertyChanged("magnitude");
        }
    }
    
    /// <remarks/>
    [XmlTextAttribute()]
    public List<double> Text {
        get {
            return this.textField;
        }
        set {
            this.textField = value;
            this.RaisePropertyChanged("Text");
        }
    }
}
