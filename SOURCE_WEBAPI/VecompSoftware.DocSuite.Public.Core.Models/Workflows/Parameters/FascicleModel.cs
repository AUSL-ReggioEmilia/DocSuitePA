using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello del Fascicolo
    /// </summary>
    public sealed class FascicleModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        private FascicleModel()
        {
            Sectors = new List<DocSuiteSectorModel>();
            DocumentUnits = new List<DocumentUnitModel>();
        }

        public FascicleModel(string subject = "", CategoryModel category = null, ContactModel manager = null, Guid? fascicleId = null)
            :this()
        {
            this.FascicleId = fascicleId;
            this.Subject = subject;
            this.Category = category;
            this.Manager = manager;            
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Identificativo del fascicolo
        /// </summary>
        public Guid? FascicleId { get; private set; }
        /// <summary>
        /// Oggetto del fascicolo
        /// </summary>
        public string Subject { get; private set; }
        /// <summary>
        /// Note del fascicolo
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Classificatore del fascicolo
        /// </summary>
        public CategoryModel Category { get; set; }
        /// <summary>
        /// Responsabile di procedimento del fascicolo
        /// </summary>
        public ContactModel Manager { get; set; }
        /// <summary>
        /// Settori autorizzati del fascicolo
        /// </summary>
        public ICollection<DocSuiteSectorModel> Sectors { get; set; }
        /// <summary>
        /// Unità documentali associate al fascicolo
        /// </summary>
        public ICollection<DocumentUnitModel> DocumentUnits { get; set; }
        #endregion
    }
}
