using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Entity.Messages
{

    public class Message : DSWBaseEntity, IWorkflowContentBase
    {
        #region [ Constructor ]

        public Message() : this(Guid.NewGuid()) { }

        public Message(Guid uniqueId)
            : base(uniqueId)
        {
            MessageEmails = new HashSet<MessageEmail>();
            MessageAttachments = new HashSet<MessageAttachment>();
            MessageContacts = new HashSet<MessageContact>();
            Protocols = new HashSet<Protocol>();
            Resolutions = new HashSet<Resolution>();
            Dossiers = new HashSet<Dossier>();
            UDSMessages = new HashSet<UDSMessage>();
            DocumentSeriesItems = new HashSet<DocumentSeriesItem>();
            WorkflowActions = new List<IWorkflowAction>();
            DocumentSeriesItems = new HashSet<DocumentSeriesItem>();
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Indica lo Status del Messaggio
        /// </summary>
        public MessageStatus Status { get; set; }

        /// <summary>
        /// Tipo di messaggio
        /// </summary>
        public MessageType MessageType { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Oggetto Location collegato
        /// </summary>
        public virtual Location Location { get; set; }
        /// <summary>
        ///  Collezione di messaggi relativi alla mail corrente 
        /// </summary>
        /// <remarks>consente di accedere alle proprietà dell'invio effettivo</remarks>
        public virtual ICollection<MessageEmail> MessageEmails { get; set; }
        /// <summary>
        ///  Collezione di attachments relativi alla mail corrente 
        /// </summary>
        /// <remarks>consente di accedere alle proprietà dell'invio effettivo</remarks>
        public virtual ICollection<MessageAttachment> MessageAttachments { get; set; }
        /// <summary>
        ///  Collezione di contatti relativi alla mail corrente 
        /// </summary>
        /// <remarks>consente di accedere alle proprietà dell'invio effettivo</remarks>
        public virtual ICollection<MessageContact> MessageContacts { get; set; }
        public virtual ICollection<MessageLog> MessageLogs { get; set; }
        public virtual ICollection<DocumentSeriesItem> DocumentSeriesItems { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }
        public virtual ICollection<Dossier> Dossiers { get; set; }
        public virtual ICollection<UDSMessage> UDSMessages { get; set; }
        public virtual ICollection<Resolution> Resolutions { get; set; }
        #endregion

        #region [ Not Mapping Properties ]
        public bool WorkflowAutoComplete { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion
    }
}
