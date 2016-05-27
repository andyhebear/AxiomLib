using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false, ElementName = "scene")]
public partial class Scene : ColladaBase
{
    private List<InstanceWithExtra> instancePhysicsSceneField;

    private List<InstanceWithExtra> instanceVisualSceneField;

    private List<Extra> extraField;

    public List<InstanceWithExtra> InstancePhysicsScene
    {
        get
        {
            return this.instancePhysicsSceneField;
        }
        set
        {
            this.instancePhysicsSceneField = value;
            this.RaisePropertyChanged("InstancePhysicsScene");
        }
    }

    public List<InstanceWithExtra> InstanceVisualScene
    {
        get
        {
            return this.instanceVisualSceneField;
        }
        set
        {
            this.instanceVisualSceneField = value;
            this.RaisePropertyChanged("InstanceVisualScene");
        }
    }

    public List<Extra> Extra
    {
        get
        {
            return this.extraField;
        }
        set
        {
            this.extraField = value;
            this.RaisePropertyChanged("Extra");
        }
    }

}
