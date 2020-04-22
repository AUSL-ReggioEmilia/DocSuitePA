using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    ///  Modello del Settore / Ruolo configurato nella DocSuite
    /// </summary>
    public sealed class DocSuiteSectorModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello del Settore / Ruolo configurato nella DocSuite
        /// </summary>
        /// <param name="name">Nome del settore specifico DocSuite</param>
        /// <param name="archiveSection">Se l'unità documentaria è un archivio, è necessario specificare la sezione del settore</param>
        /// <param name="mappingTag">Codice di mapping concordato con Dgroove</param>
        /// <param name="sectorRoleId">Codice univoco del settore specifico DocSuite</param>
        public DocSuiteSectorModel(string name, string archiveSection = "", string mappingTag = "", Guid? sectorRoleId = null)
        {
            Name = name;
            ArchiveSection = archiveSection;
            MappingTag = mappingTag;
            SectorRoleId = sectorRoleId;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Codice univoco del settore specifico DocSuite
        /// </summary>
        public Guid? SectorRoleId { get; private set; }

        /// <summary>
        /// Nome del settore specifico DocSuite
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Se l'unità documentaria è un archivio, è necessario specificare la sezione del settore
        /// </summary>
        public string ArchiveSection { get; private set; }

        /// <summary>
        /// Codice di mapping concordato con Dgroove
        /// </summary>
        public string MappingTag { get; private set; }
        #endregion
    }
}
