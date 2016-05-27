
using System.Xml.Serialization;


/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public enum MorphMethodType
{

    /// <remarks/>
    NORMALIZED,

    /// <remarks/>
    RELATIVE,
}