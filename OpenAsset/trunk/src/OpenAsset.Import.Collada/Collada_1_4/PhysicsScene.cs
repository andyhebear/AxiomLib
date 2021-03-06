﻿
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class PhysicsScene : ColladaBase
{

    private Asset assetField;

    private List<InstanceWithExtra> instance_force_fieldField;

    private List<InstancePhysicsModel> instance_physics_modelField;

    private PhysicsSceneTechniqueCommon technique_commonField;

    private List<Technique> techniqueField;

    private List<Extra> extraField;

    private string idField;

    private string nameField;

    /// <remarks/>
    public Asset asset
    {
        get
        {
            return this.assetField;
        }
        set
        {
            this.assetField = value;
            this.RaisePropertyChanged("asset");
        }
    }

    /// <remarks/>
    [XmlElement("instance_force_field")]
    public List<InstanceWithExtra> instance_force_field
    {
        get
        {
            return this.instance_force_fieldField;
        }
        set
        {
            this.instance_force_fieldField = value;
            this.RaisePropertyChanged("instance_force_field");
        }
    }

    /// <remarks/>
    [XmlElement("instance_physics_model")]
    public List<InstancePhysicsModel> instance_physics_model
    {
        get
        {
            return this.instance_physics_modelField;
        }
        set
        {
            this.instance_physics_modelField = value;
            this.RaisePropertyChanged("instance_physics_model");
        }
    }

    /// <remarks/>
    public PhysicsSceneTechniqueCommon technique_common
    {
        get
        {
            return this.technique_commonField;
        }
        set
        {
            this.technique_commonField = value;
            this.RaisePropertyChanged("technique_common");
        }
    }

    /// <remarks/>
    [XmlElement("technique")]
    public List<Technique> technique
    {
        get
        {
            return this.techniqueField;
        }
        set
        {
            this.techniqueField = value;
            this.RaisePropertyChanged("technique");
        }
    }

    /// <remarks/>
    [XmlElement("extra")]
    public List<Extra> extra
    {
        get
        {
            return this.extraField;
        }
        set
        {
            this.extraField = value;
            this.RaisePropertyChanged("extra");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "ID")]
    public string id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
            this.RaisePropertyChanged("id");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NCName")]
    public string name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
            this.RaisePropertyChanged("name");
        }
    }
}
