
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class LightTechniqueCommonAmbient : ColladaBase
{

    private TargetableFloat3 colorField;

    /// <remarks/>
    public TargetableFloat3 color
    {
        get
        {
            return this.colorField;
        }
        set
        {
            this.colorField = value;
            this.RaisePropertyChanged("color");
        }
    }
}
