using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Archive", Namespace = "http://BiblosDS/2009/10/Archive")]
    public partial class DocumentArchive : BiblosDSObject
    {
        /// <summary>
        /// Id dell'archivio in cui salvare i documenti
        /// </summary>
        [DataMember]
        public Guid IdArchive { get; set; }

        /// <summary>
        /// Nome dell'archivio
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Storage
        /// </summary>
        [DataMember]
        public DocumentStorage Storage { get; set; }

        [DataMember]
        public bool IsLegal { get; set; }

        [DataMember]
        public Nullable<long> MaxCache { get; set; }

        [DataMember]
        public Nullable<long> UpperCache { get; set; }

        [DataMember]
        public Nullable<long> LowerCache { get; set; }

        [DataMember]
        public int LastIdBiblos { get; set; }

        [DataMember]
        public bool EnableSecurity { get; set; }

        [DataMember]
        public bool AutoVersion { get; set; }

        [DataMember]
        public string AuthorizationAssembly { get; set; }

        [DataMember]
        public string AuthorizationClassName { get; set; }

        [DataMember]
        public string PathCache { get; set; }

        [DataMember]
        public string PathTransito { get; set; }

        [DataMember]
        public bool TransitoEnabled { get; set; }

        [DataMember]
        public string PathPreservation { get; set; }
       
        [DataMember]
        public bool ThumbnailEmabled { get; set; }

        [DataMember]
        public bool PdfConversionEmabled { get; set; }

        #region Constructor

        public DocumentArchive(): base()
        {
            this.IdArchive = Guid.NewGuid();            
        }

        public DocumentArchive(Guid IdArchive): this()
        {
            this.IdArchive = IdArchive;            
        }

        #endregion

        [DataMember]
        public bool FullSignEnabled { get; set; }

        [DataMember]
        public string FiscalDocumentType { get; set; }

        [DataMember]
        public BindingList<ArchiveServerConfig> ServerConfigs { get; set; }

        public string ODBCConnection { get; set; }

        public string PreservationConfiguration { get; set; }
    }
}
