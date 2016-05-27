
using System.Xml.Serialization;

    /// <remarks/>    
    [System.SerializableAttribute()]
    [XmlTypeAttribute(Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
    public partial class InputLocal : ColladaBase
    {

        private string semanticField;

        private string sourceField;

        /// <remarks/>
        [XmlAttributeAttribute(DataType = "NMTOKEN")]
        public string semantic
        {
            get
            {
                return this.semanticField;
            }
            set
            {
                this.semanticField = value;
                this.RaisePropertyChanged("semantic");
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
                this.RaisePropertyChanged("source");
            }
        }

    }
