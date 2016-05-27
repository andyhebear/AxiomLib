
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class InstancePhysicsModel : ColladaBase
{

    private List<InstanceWithExtra> instance_force_fieldField;

    private List<InstanceRigidBody> instance_rigid_bodyField;

    private List<instance_rigid_constraint> instance_rigid_constraintField;

    private List<Extra> extraField;

    private string urlField;

    private string sidField;

    private string nameField;

    private string parentField;

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
    [XmlElement("instance_rigid_body")]
    public List<InstanceRigidBody> instance_rigid_body
    {
        get
        {
            return this.instance_rigid_bodyField;
        }
        set
        {
            this.instance_rigid_bodyField = value;
            this.RaisePropertyChanged("instance_rigid_body");
        }
    }

    /// <remarks/>
    [XmlElement("instance_rigid_constraint")]
    public List<instance_rigid_constraint> instance_rigid_constraint
    {
        get
        {
            return this.instance_rigid_constraintField;
        }
        set
        {
            this.instance_rigid_constraintField = value;
            this.RaisePropertyChanged("instance_rigid_constraint");
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
    [XmlAttributeAttribute(DataType = "anyURI")]
    public string url
    {
        get
        {
            return this.urlField;
        }
        set
        {
            this.urlField = value;
            this.RaisePropertyChanged("url");
        }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "NCName")]
    public string sid
    {
        get
        {
            return this.sidField;
        }
        set
        {
            this.sidField = value;
            this.RaisePropertyChanged("sid");
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

    /// <remarks/>
    [XmlAttributeAttribute(DataType = "anyURI")]
    public string parent
    {
        get
        {
            return this.parentField;
        }
        set
        {
            this.parentField = value;
            this.RaisePropertyChanged("parent");
        }
    }
}
