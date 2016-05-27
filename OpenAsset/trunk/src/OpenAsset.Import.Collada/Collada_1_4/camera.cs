
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="camera")]
public partial class Camera : ColladaBase {
    
    private Asset assetField;
    
    private CameraOptics opticsField;
    
    private CameraImager imagerField;
    
    private List<Extra> extraField;
    
    private string idField;
    
    private string nameField;
    
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
    public CameraOptics optics {
        get {
            return this.opticsField;
        }
        set {
            this.opticsField = value;
            this.RaisePropertyChanged("optics");
        }
    }
    
    /// <remarks/>
    public CameraImager imager {
        get {
            return this.imagerField;
        }
        set {
            this.imagerField = value;
            this.RaisePropertyChanged("imager");
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
}

