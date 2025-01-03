﻿using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails
{
    public class PECMailBoxValidator : ObjectValidator<PECMailBox, PECMailBoxValidator>, IPECMailBoxValidator
    {
        #region [ Constructor ]
        public PECMailBoxValidator(ILogger logger, IPECMailBoxValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public short EntityShortId { get; set; }
        public string MailBoxRecipient { get; set; }

        public string IncomingServer { get; set; }

        public string OutgoingServer { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string RedirectAnomaliesSMTP { get; set; }

        public string RedirectAnomaliesUsername { get; set; }

        public string RedirectAnomaliesPassword { get; set; }

        public string RedirectAnomaliesRecipient { get; set; }

        public string RedirectStorageSMTP { get; set; }

        public string RedirectStorageUsername { get; set; }

        public string RedirectStoragePassword { get; set; }

        public string RedirectStorageRecipient { get; set; }

        public bool IsForInterop { get; set; }

        public bool? IsDestinationEnabled { get; set; }

        public bool? DeleteMailFromServer { get; set; }

        public short? ReceiveDaysCap { get; set; }

        public bool? Managed { get; set; }

        public bool? Unmanaged { get; set; }

        public int? IdConfiguration { get; set; }

        public int? IncomingServerProtocol { get; set; }

        public int? IncomingServerPort { get; set; }

        public bool? IncomingServerUseSsl { get; set; }

        public int? OutgoingServerPort { get; set; }

        public bool? OutgoingServerUseSsl { get; set; }

        public bool? IsHandleEnabled { get; set; }

        public bool? IsProtocolBox { get; set; }

        public bool? IsProtocolBoxExplicit { get; set; }

        public Guid? IdJeepServiceIncomingHost { get; set; }

        public Guid? IdJeepServiceOutgoingHost { get; set; }

        public Guid UniqueId { get; set; }

        public string RulesetDefinition { get; set; }

        public InvoiceType? InvoiceType { get; set; }

        public bool LoginError { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Location Location { get; set; }

        public ICollection<PECMail> PECMails { get; set; }
        public ICollection<OChartItem> OChartItems { get; set; }


        #endregion
    }
}
