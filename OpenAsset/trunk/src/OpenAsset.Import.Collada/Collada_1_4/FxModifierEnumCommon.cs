using System.Xml.Serialization;


/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public enum  FxModifierEnumCommon 
{
	CONST,
	UNIFORM,
	VARYING,
	STATIC,
	VOLATILE,
	EXTERN,
	SHARED 
}
