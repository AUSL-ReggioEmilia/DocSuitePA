using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello del contenitor
    /// </summary>
    public sealed class ContainerModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello del Contenitore
        /// </summary>
        /// <param name="name">Nome del contenitore</param>
        /// <param name="uniqueId">Codice del contenitore della DocSuite</param>
        /// <param name="mappingTag">Codice di mapping concordato con Dgroove</param>
        public ContainerModel(string name = "", Guid? uniqueId = null, string mappingTag = "")
        {
            Name = name;
            UniqueId = uniqueId;
            MappingTag = mappingTag;
            if (string.IsNullOrEmpty(name) && !uniqueId.HasValue && string.IsNullOrEmpty(mappingTag))
            {
                throw new ArgumentException("E' necessario specifiare almeno un valore identificativo del Contenitore.");
            }
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Codice del contenitore della DocSuite
        /// </summary>
        public Guid? UniqueId { get; private set; }

        /// <summary>
        /// Nome del contenitore
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Codice di mapping concordato con Dgroove
        /// </summary>
        public string MappingTag { get; private set; }
        #endregion
    }

}
