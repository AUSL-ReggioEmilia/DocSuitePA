using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks
{
    [HasSelfValidation]
    public class DeskMessageValidator : ObjectValidator<DeskMessage, DeskMessageValidator>, IDeskMessageValidator
    {
        #region [ Constructor ]
        public DeskMessageValidator(ILogger logger, IDeskMessageValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]  
        public Guid UniqueId { get; set; }
        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Riferimento al "Tavolo"
        /// </summary>
        public Desk Desk { get; set; }
        /// <summary>
        /// Riferimento al "Message"
        /// </summary>
        public MessageEmail Message { get; set; }
        #endregion
    }
}
