using System;
using VecompSoftware.DocSuite.Public.Core.Models.ContentTypes;
using VecompSoftware.DocSuite.Public.Core.Models.Securities;

namespace VecompSoftware.DocSuite.Public.Core.Models.Commands
{
    /// <summary>
    /// Classe astratta base che definisce un comando generico
    /// </summary>
    /// <typeparam name="TContentType">Tipo del contenuto di traporto<see cref="IContentType{TModel}"/></typeparam>
    /// <typeparam name="TModel">Tipo del modello <see cref="IModel"/></typeparam>
    public abstract class BaseCommand<TContentType, TModel> : BaseMessage
        where TModel : IModel
        where TContentType : IContentType<TModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Classe astratta base che definisce un comando generico
        /// </summary>
        /// <param name="commandId">Valore guid che identifica univocamente il comando. 
        /// Questo valore sarà la chiave dell’istanza del workflow che funegrà da CorrelationId per gli eventi del Service Bus. 
        /// Diventerà ultile per determinare quali eventi sono associati a determinate istanze attive.</param>
        /// <param name="commandName">Nome interno del comando</param>
        /// <param name="tenantName">Nome del Tenant del Cliente. Contattare Vecomp Software per il valore</param>
        /// <param name="tenantId">Guid del Tenant del Cliente. Contattare Vecomp Software per il valore</param>
        /// <param name="identityContext">Classe che specifica l'Identità dell'utente che sta eseguento l'avvio del Workflow<see cref="IdentityContext"/></param>
        /// <param name="contentType">ContentType col modello di trasporto</param>
        public BaseCommand(Guid commandId, string commandName, string tenantName,
            Guid tenantId, IdentityContext identityContext, TContentType contentType)
            : base(commandId, commandName, tenantName, tenantId, identityContext)
        {
            ContentType = contentType;

            if (contentType != null && contentType.Content != null)
            {
                CustomProperties.Add(DocSuiteCustomPropertyNames.CONTENT_TYPE, contentType.Content.GetType().Name);
            }

        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// ContentType col modello di trasporto
        /// </summary>
        public TContentType ContentType { get; private set; }

        #endregion
    }
}