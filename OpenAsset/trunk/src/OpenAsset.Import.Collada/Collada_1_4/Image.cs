
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="image")]
public partial class Image : ColladaBase {
    
    private Asset assetField;
    
    private object itemField;
    
    private List<Extra> extraField;
    
    private string idField;
    
    private string nameField;
    
    private string formatField;
    
    private ulong heightField;
    
    private bool heightFieldSpecified;
    
    private ulong widthField;
    
    private bool widthFieldSpecified;
    
    private ulong depthField;
    
    public Image() {
        this.depthField = ((ulong)(1m));
    }
    
    /// <remarks/>
    public Asset asset {
        get {
            return this.assetField;
        }
        set {
            this.assetField = value;
            this.RaisePropertyChanged("asset");
        }
    }
    
    /// <remarks/>
    [XmlElement("data", typeof(byte[]), DataType="hexBinary")]
    [XmlElement("init_from", typeof(string), DataType="anyURI")]
    public object Item {
        get {
            return this.itemField;
        }
        set {
            this.itemField = value;
            this.RaisePropertyChanged("Item");
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
    [XmlAttributeAttribute(DataType="token")]
    public string format {
        get {
            return this.formatField;
        }
        set {
            this.formatField = value;
            this.RaisePropertyChanged("format");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute()]
    public ulong height {
        get {
            return this.heightField;
        }
        set {
            this.heightField = value;
            this.RaisePropertyChanged("height");
        }
    }
    
    /// <remarks/>
    [XmlIgnoreAttribute()]
    public bool heightSpecified {
        get {
            return this.heightFieldSpecified;
        }
        set {
            this.heightFieldSpecified = value;
            this.RaisePropertyChanged("heightSpecified");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute()]
    public ulong width {
        get {
            return this.widthField;
        }
        set {
            this.widthField = value;
            this.RaisePropertyChanged("width");
        }
    }
    
    /// <remarks/>
    [XmlIgnoreAttribute()]
    public bool widthSpecified {
        get {
            return this.widthFieldSpecified;
        }
        set {
            this.widthFieldSpecified = value;
            this.RaisePropertyChanged("widthSpecified");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(typeof(ulong), "1")]
    public ulong depth {
        get {
            return this.depthField;
        }
        set {
            this.depthField = value;
            this.RaisePropertyChanged("depth");
        }
    }
}
