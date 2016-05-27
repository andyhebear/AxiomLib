using System.Xml.Serialization;

    /// <remarks/>    
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
    public partial class SourceTechniqueCommon : ColladaBase
    {

        private Accessor accessorField;

        /// <remarks/>
        public Accessor accessor
        {
            get
            {
                return this.accessorField;
            }
            set
            {
                this.accessorField = value;
                this.RaisePropertyChanged("accessor");
            }
        }
    }
