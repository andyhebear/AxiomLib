﻿
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="library_physics_materials")]
public partial class LibraryPhysicsMaterials : ColladaBase {
    
    private Asset assetField;
    
    private List<PhysicsMaterial> physics_materialField;
    
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
    [XmlElement("physics_material")]
    public List<PhysicsMaterial> physics_material {
        get {
            return this.physics_materialField;
        }
        set {
            this.physics_materialField = value;
            this.RaisePropertyChanged("physics_material");
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