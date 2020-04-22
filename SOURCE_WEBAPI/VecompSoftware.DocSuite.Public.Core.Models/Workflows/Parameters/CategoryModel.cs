using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello del Classificatore / Titolario
    /// </summary>
    public sealed class CategoryModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello del Classificatore / Titolario
        /// </summary>
        /// <param name="name">Voce del classificatore</param>
        /// <param name="uniqueId">Codice del classificatore della DocSuite</param>
        /// <param name="mappingTag">Codice di mapping concordato con Dgroove</param>
        public CategoryModel(string name = "", Guid? uniqueId = null, string mappingTag = "")
        {
            Name = name;
            UniqueId = uniqueId;
            MappingTag = mappingTag;
            if (string.IsNullOrEmpty(name) && !uniqueId.HasValue && string.IsNullOrEmpty(mappingTag))
            {
                throw new ArgumentException("E' necessario specifiare almeno un valore identificativo del Classificatore.");
            }
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Codice del classificatore della DocSuite
        /// </summary>
        public Guid? UniqueId { get; private set; }

        /// <summary>
        /// Voce del classificatore
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Codice di mapping concordato con Dgroove
        /// </summary>
        public string MappingTag { get; private set; }

        #endregion
    }

}
