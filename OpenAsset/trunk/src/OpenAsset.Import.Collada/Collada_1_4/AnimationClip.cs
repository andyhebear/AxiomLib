
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>

[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=false, ElementName="animation_clip")]
public partial class AnimationClip : ColladaBase {
    
    private Asset assetField;
    
    private List<InstanceWithExtra> instance_animationField;
    
    private List<Extra> extraField;
    
    private string idField;
    
    private string nameField;
    
    private double startField;
    
    private double endField;
    
    private bool endFieldSpecified;
    
    public AnimationClip() {
        this.startField = 0D;
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
    [XmlElement("instance_animation")]
    public List<InstanceWithExtra> instance_animation {
        get {
            return this.instance_animationField;
        }
        set {
            this.instance_animationField = value;
            this.RaisePropertyChanged("instance_animation");
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
    [XmlAttributeAttribute()]
    [DefaultValueAttribute(0D)]
    public double start {
        get {
            return this.startField;
        }
        set {
            this.startField = value;
            this.RaisePropertyChanged("start");
        }
    }
    
    /// <remarks/>
    [XmlAttributeAttribute()]
    public double end {
        get {
            return this.endField;
        }
        set {
            this.endField = value;
            this.RaisePropertyChanged("end");
        }
    }
    
    /// <remarks/>
    [XmlIgnoreAttribute()]
    public bool endSpecified {
        get {
            return this.endFieldSpecified;
        }
        set {
            this.endFieldSpecified = value;
            this.RaisePropertyChanged("endSpecified");
        }
    }
    
}
