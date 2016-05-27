
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

/// <remarks/>
[System.SerializableAttribute()]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.collada.org/2005/11/COLLADASchema")]
[XmlRootAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false, ElementName = "asset")]
public partial class Asset : ColladaBase {
    
    private List<AssetContributor> contributorField;
    
    private System.DateTime createdField;
    
    private string keywordsField;
    
    private System.DateTime modifiedField;
    
    private string revisionField;
    
    private string subjectField;
    
    private string titleField;
    
    private AssetUnit unitField;
    
    private UpAxisType up_axisField;
    
    public Asset() {
        this.up_axisField = UpAxisType.Y_UP;
    }
    
    /// <remarks/>
    [XmlElement("contributor")]
    public List<AssetContributor> contributor {
        get {
            return this.contributorField;
        }
        set {
            this.contributorField = value;
            this.RaisePropertyChanged("contributor");
        }
    }
    
    /// <remarks/>
    public System.DateTime created {
        get {
            return this.createdField;
        }
        set {
            this.createdField = value;
            this.RaisePropertyChanged("created");
        }
    }
    
    /// <remarks/>
    public string keywords {
        get {
            return this.keywordsField;
        }
        set {
            this.keywordsField = value;
            this.RaisePropertyChanged("keywords");
        }
    }
    
    /// <remarks/>
    public System.DateTime modified {
        get {
            return this.modifiedField;
        }
        set {
            this.modifiedField = value;
            this.RaisePropertyChanged("modified");
        }
    }
    
    /// <remarks/>
    public string revision {
        get {
            return this.revisionField;
        }
        set {
            this.revisionField = value;
            this.RaisePropertyChanged("revision");
        }
    }
    
    /// <remarks/>
    public string subject {
        get {
            return this.subjectField;
        }
        set {
            this.subjectField = value;
            this.RaisePropertyChanged("subject");
        }
    }
    
    /// <remarks/>
    public string title {
        get {
            return this.titleField;
        }
        set {
            this.titleField = value;
            this.RaisePropertyChanged("title");
        }
    }
    
    /// <remarks/>
    public AssetUnit unit {
        get {
            return this.unitField;
        }
        set {
            this.unitField = value;
            this.RaisePropertyChanged("unit");
        }
    }
    
    /// <remarks/>
    [DefaultValueAttribute(UpAxisType.Y_UP)]
    public UpAxisType up_axis {
        get {
            return this.up_axisField;
        }
        set {
            this.up_axisField = value;
            this.RaisePropertyChanged("up_axis");
        }
    }
}

