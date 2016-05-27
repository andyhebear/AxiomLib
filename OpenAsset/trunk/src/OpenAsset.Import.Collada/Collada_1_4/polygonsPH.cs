using System.Xml.Serialization;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class PolygonsPH : ColladaBase
{

    private string pField;

    private List<string> hField;

    /// <remarks/>
    public string p
    {
        get
        {
            return this.pField;
        }
        set
        {
            this.pField = value;
            this.RaisePropertyChanged("p");
        }
    }

    /// <remarks/>
    [XmlElement("h")]
    public List<string> h
    {
        get
        {
            return this.hField;
        }
        set
        {
            this.hField = value;
            this.RaisePropertyChanged("h");
        }
    }
}

