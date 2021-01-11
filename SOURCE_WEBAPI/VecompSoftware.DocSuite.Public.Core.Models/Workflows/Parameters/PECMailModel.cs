using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello della PEC
    /// </summary>
    public sealed class PECMailModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello della PEC
        /// </summary>
        /// <param name="subject">Oggetto della email</param>
        /// <param name="body">Corpo della email</param>
        /// <param name="sender">Mittente della email</param>
        /// <param name="recipients">Destinatari della email</param>
        /// <param name="recipientsCc">Destinatari copia conoscenza della email</param>
        public PECMailModel(string subject, string body, string sender, string recipients, string recipientsCc)
        {
            Subject = subject;
            Body = body;
            Recipients = recipients;
            RecipientsCc = recipientsCc;
            Sender = sender;
        }

        #endregion

        #region [ Properties ]
        /// <summary>
        /// Oggetto dell'unità documentaria
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Corpo della email
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Mittente della email
        /// </summary>
        public string Sender { get; private set; }

        /// <summary>
        /// Destinatari della email
        /// </summary>
        public string Recipients { get; private set; }

        /// <summary>
        /// Destinatari copia conoscenza della email
        /// </summary>
        public string RecipientsCc { get; private set; }
        #endregion
    }
}
