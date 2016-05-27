
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
public partial class AssetContributor : ColladaBase
{

    private string authorField;

    private string authoring_toolField;

    private string commentsField;

    private string copyrightField;

    private string source_dataField;

    /// <remarks/>
    public string author
    {
        get
        {
            return this.authorField;
        }
        set
        {
            this.authorField = value;
            this.RaisePropertyChanged("author");
        }
    }

    /// <remarks/>
    public string authoring_tool
    {
        get
        {
            return this.authoring_toolField;
        }
        set
        {
            this.authoring_toolField = value;
            this.RaisePropertyChanged("authoring_tool");
        }
    }

    /// <remarks/>
    public string comments
    {
        get
        {
            return this.commentsField;
        }
        set
        {
            this.commentsField = value;
            this.RaisePropertyChanged("comments");
        }
    }

    /// <remarks/>
    public string copyright
    {
        get
        {
            return this.copyrightField;
        }
        set
        {
            this.copyrightField = value;
            this.RaisePropertyChanged("copyright");
        }
    }

    /// <remarks/>
    [XmlElement(DataType = "anyURI")]
    public string source_data
    {
        get
        {
            return this.source_dataField;
        }
        set
        {
            this.source_dataField = value;
            this.RaisePropertyChanged("source_data");
        }
    }
}

