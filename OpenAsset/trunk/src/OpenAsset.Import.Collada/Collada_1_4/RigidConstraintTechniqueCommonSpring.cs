using System.Xml.Serialization;


/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class RigidConstraintTechniqueCommonSpring : ColladaBase
{

    private RigidConstraintTechniqueCommonSpringAngular angularField;

    private RigidConstraintTechniqueCommonSpringLinear linearField;

    /// <remarks/>
    public RigidConstraintTechniqueCommonSpringAngular angular
    {
        get
        {
            return this.angularField;
        }
        set
        {
            this.angularField = value;
            this.RaisePropertyChanged("angular");
        }
    }

    /// <remarks/>
    public RigidConstraintTechniqueCommonSpringLinear linear
    {
        get
        {
            return this.linearField;
        }
        set
        {
            this.linearField = value;
            this.RaisePropertyChanged("linear");
        }
    }
}
