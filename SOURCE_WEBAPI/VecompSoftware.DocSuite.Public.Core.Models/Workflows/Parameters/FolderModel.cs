using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello di una cartella
    /// </summary>
    public sealed class FolderModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public FolderModel(string name = "", Guid? uniqueId = null)
        {
            this.UniqueId = uniqueId;
            this.Name = name;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Identificativo della cartella
        /// </summary>
        public Guid? UniqueId { get; private set; }
        /// <summary>
        /// Nome della cartella
        /// </summary>
        public string Name { get; private set; }
        #endregion
    }
}
