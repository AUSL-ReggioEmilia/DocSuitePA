using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions
{
    public class ResolutionValidator : ObjectValidator<Resolution, ResolutionValidator>, IResolutionValidator
    {
        #region [ Constructor ]
        public ResolutionValidator(ILogger logger, IResolutionValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
            ICollection<ResolutionContact> ResolutionContacts = new Collection<ResolutionContact>();
            ICollection<ResolutionLog> ResolutionLogs = new Collection<ResolutionLog>();
        }

        #endregion

        #region [ Properties ]

        public int EntityId { get; set; }

        public DateTime? AdoptionDate { get; set; }

        public string AlternativeAssignee { get; set; }

        public string AlternativeManager { get; set; }

        public string AlternativeProposer { get; set; }

        public string AlternativeRecipient { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public DateTime? EffectivenessDate { get; set; }

        public DateTime? LeaveDate { get; set; }

        public int? Number { get; set; }

        public DateTime? ProposeDate { get; set; }

        public DateTime? PublishingDate { get; set; }

        public DateTime? ResponseDate { get; set; }

        public string ServiceNumber { get; set; }

        public string Object { get; set; }

        public DateTime? WaitDate { get; set; }

        public DateTime? WarningDate { get; set; }

        public string WorkflowType { get; set; }

        public short? Year { get; set; }

        public string ProposeUser { get; set; }

        public string LeaveUser { get; set; }

        public string EffectivenessUser { get; set; }

        public string ResponseUser { get; set; }

        public string WaitUser { get; set; }

        public string ConfirmUser { get; set; }

        public string WarningUser { get; set; }

        public string PublishingUser { get; set; }

        public string AdoptionUser { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public ResolutionStatus Status { get; set; }

        public string InclusiveNumber { get; set; }

        public DateTime? WebPublicationDate { get; set; }

        public float? Amount { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Category Category { get; set; }

        public Container Container { get; set; }

        public FileResolution FileResolution { get; set; }



        public ICollection<ResolutionContact> ResolutionContacts { get; set; }

        //public ICollection<ResolutionRole> ResolutionRoles { get; set; }
        public ICollection<ResolutionLog> ResolutionLogs { get; set; }


        #endregion
    }
}
