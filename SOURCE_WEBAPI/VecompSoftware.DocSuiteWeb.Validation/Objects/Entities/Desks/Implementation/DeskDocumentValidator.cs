using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks
{
    [HasSelfValidation]
    public class DeskDocumentValidator : ObjectValidator<DeskDocument, DeskDocumentValidator>, IDeskDocumentValidator
    {
        #region [ Constructor ]
        public DeskDocumentValidator(ILogger logger, IDeskDocumentValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Cancellazione logica documento
        /// </summary>
        public short IsActive { get; set; }
        /// <summary>
        /// Ultima data in cui è stato modificato il documento
        /// </summary>
        /// 
        /// <summary>
        /// Tipologia di documento memorizzato
        /// 1) Documento
        /// 2) Allegato
        /// 3) Annesso
        /// </summary>
        public DeskDocumentType DocumentType { get; set; }
        /// <summary>
        /// Identificativo del documento derivante da Biblos
        /// </summary>
        public Guid? IdDocument { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Relazione tra il documento e il "Tavolo"
        /// </summary>
        public Desk Desk { get; set; }
        /// <summary>
        /// Collezione di versioni del documento
        /// </summary>
        public ICollection<DeskDocumentVersion> DeskDocumentVersions { get; set; }
        #endregion
    }
}
