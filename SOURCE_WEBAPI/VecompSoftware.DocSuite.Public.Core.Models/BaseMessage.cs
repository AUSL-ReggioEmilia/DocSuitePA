using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Public.Core.Models.Securities;

namespace VecompSoftware.DocSuite.Public.Core.Models
{
    /// <summary>
    /// Classe astratta base che generelizza comando e eventi
    /// </summary>
    public abstract class BaseMessage : BaseModel, IMessage
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Classe astratta base che generelizza comando e eventi
        /// </summary>
        /// <param name="messageDate">Data del messaggio</param>
        /// <param name="id">Identificativo univoco del messaggio</param>
        /// <param name="messageName">Nome interno del messaggio</param>
        /// <param name="tenantName">Nome del Tenant del Cliente. Contattare Dgroove per il valore</param>
        /// <param name="tenantId">Guid del Tenant del Cliente. Contattare Dgroove per il valore</param>
        /// <param name="tenantAOOId">Guid del TenantAOO del Cliente. Contattare Dgroove per il valore</param>
        /// <param name="identityContext">Classe che specifica l'Identità dell'utente che sta eseguento l'avvio del Workflow<see cref="IdentityContext"/></param>
        private BaseMessage(DateTimeOffset messageDate, Guid id, string messageName, string tenantName,
            Guid tenantId, Guid tenantAOOId, IdentityContext identityContext)
            : base(id)
        {
            MessageDate = messageDate;
            MessageName = messageName;
            TenantName = tenantName;
            TenantId = tenantId;
            TenantAOOId = tenantAOOId;
            IdentityContext = identityContext;
            CustomProperties = new Dictionary<string, object>
            {
                { DocSuiteCustomPropertyNames.ID, Id },
                { DocSuiteCustomPropertyNames.MESSAGE_NAME, messageName },
                { DocSuiteCustomPropertyNames.MESSAGE_TYPE, GetType().Name },
                { DocSuiteCustomPropertyNames.TENANT_ID, tenantId },
                { DocSuiteCustomPropertyNames.TENANT_NAME, tenantName },
                { DocSuiteCustomPropertyNames.MESSAGE_DATE, MessageDate }
            };

            if (identityContext != null)
            {
                CustomProperties.Add(DocSuiteCustomPropertyNames.IDENTITY, identityContext.Identity.Account);
                CustomProperties.Add(DocSuiteCustomPropertyNames.AUTHORIZATION_TYPE, identityContext.Identity.Authorization);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Identificativo univoco del messaggio</param>
        /// <param name="messageName">Nome interno del messaggio</param>
        /// <param name="tenantName">Nome del Tenant del Cliente. Contattare Dgroove per il valore</param>
        /// <param name="tenantId">Guid del Tenant del Cliente. Contattare Dgroove per il valore</param>
        /// <param name="tenantAOOId">Guid del TenantAOO del Cliente. Contattare Dgroove per il valore</param>
        /// <param name="identityContext">Classe che specifica l'Identità dell'utente che sta eseguento l'avvio del Workflow<see cref="IdentityContext"/></param>
        public BaseMessage(Guid id, string messageName, string tenantName, Guid tenantId, Guid tenantAOOId, IdentityContext identityContext)
             : this(DateTimeOffset.UtcNow, id, messageName, tenantName, tenantId, tenantAOOId, identityContext)
        {

        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Chiave di 'correlazione'. Può essere utile per legare più messaggi allo stesso processo di business.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// Data del messaggio
        /// </summary>
        public DateTimeOffset MessageDate { get; private set; }

        /// <summary>
        /// Classe che specifica l'Identità dell'utente che sta eseguento l'avvio del Workflow<see cref="IdentityContext"/>
        /// </summary>
        public IdentityContext IdentityContext { get; private set; }

        /// <summary>
        /// Guid del Tenant del Cliente. Contattare Dgroove per il valore
        /// </summary>
        public Guid TenantAOOId { get; private set; }

        /// <summary>
        /// Guid del Tenant del Cliente. Contattare Dgroove per il valore
        /// </summary>
        public Guid TenantId { get; private set; }

        /// <summary>
        /// Nome del Tenant del Cliente. Contattare Dgroove per il valore
        /// </summary>
        public string TenantName { get; private set; }

        /// <summary>
        /// Nome interno del messaggio
        /// </summary>
        public string MessageName { get; private set; }

        /// <summary>
        /// Dizionario chiave valore con le proprietà base del comando. Viene usato per determinare le regole di instradamenti del comando se il workflow prevede l'uso del Service Bus
        /// </summary>
        public IDictionary<string, object> CustomProperties { get; private set; }

        #endregion

    }
}
