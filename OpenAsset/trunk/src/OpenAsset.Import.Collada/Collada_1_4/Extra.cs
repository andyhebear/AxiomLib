﻿
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="extra")]
public partial class Extra : ColladaBase {
    
    private Asset assetField;
    
    private List<Technique> techniqueField;
    
    private string idField;
    
    private string nameField;
    
    private string typeField;
    
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
    [XmlElement("technique")]
    public List<Technique> technique {
        get {
            return this.techniqueField;
        }
        set {
            this.techniqueField = value;
            this.RaisePropertyChanged("technique");
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
    [XmlAttributeAttribute(DataType="NMTOKEN")]
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
            this.RaisePropertyChanged("type");
        }
    }
}
