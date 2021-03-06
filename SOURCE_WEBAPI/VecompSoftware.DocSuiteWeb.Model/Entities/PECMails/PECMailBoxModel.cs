﻿using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.PECMails
{
    public class PECMailBoxModel
    {
        #region [ Constructor ]
        public PECMailBoxModel()
        {
        }
        #endregion

        #region [ Properties ]
        public Guid? UniqueId { get; set; }
        public int? PECMailBoxId { get; set; }
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
        public string RulesetDefinition { get; set; }
        public InvoiceType? InvoiceType { get; set; }

        public bool LoginError { get; set; }

        public LocationModel Location { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
