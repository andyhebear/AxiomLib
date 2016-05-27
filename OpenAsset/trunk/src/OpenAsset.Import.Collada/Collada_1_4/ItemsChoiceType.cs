﻿
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IncludeInSchema = false)]
public enum ItemsChoiceType
{

    /// <remarks/>
    aspect_ratio,

    /// <remarks/>
    xmag,

    /// <remarks/>
    ymag,
}