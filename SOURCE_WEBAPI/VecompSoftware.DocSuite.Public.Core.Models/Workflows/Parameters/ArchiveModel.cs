using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello dell'Archivio / Unità Documentaria Specifica
    /// </summary>
    public sealed class ArchiveModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello dell'Archivio / Unità Documentaria Specifica
        /// </summary>
        /// <param name="archiveName">Nome dell'archivio documentale</param>
        /// <param name="archiveId">Identificativo dell'archivio documentale</param>
        public ArchiveModel(string archiveName, Guid? archiveId = null)
        {
            ArchiveName = archiveName;
            ArchiveId = archiveId;
            Metadatas = new HashSet<MetadataModel>();
        }

        #endregion

        #region [ Properties ]
        /// <summary>
        /// Nome dell'archivio documentale
        /// </summary>
        [Required]
        public string ArchiveName { get; private set; }

        /// <summary>
        /// Identificativo dell'archivio documentale
        /// </summary>
        public Guid? ArchiveId { get; private set; }

        /// <summary>
        /// Collezione coi metadati dell'archivio
        /// </summary>
        public ICollection<MetadataModel> Metadatas { get; private set; }
        #endregion
    }
}