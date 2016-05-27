
using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false, ElementName="box")]
public partial class Box : ColladaBase
{

    private string half_extentsField;

    private List<Extra> extraField;

    /// <remarks/>
    public string half_extents
    {
        get
        {
            return this.half_extentsField;
        }
        set
        {
            this.half_extentsField = value;
            this.RaisePropertyChanged("half_extents");
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

}
