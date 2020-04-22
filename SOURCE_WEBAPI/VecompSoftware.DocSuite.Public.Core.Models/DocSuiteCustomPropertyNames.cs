namespace VecompSoftware.DocSuite.Public.Core.Models
{
    /// <summary>
    /// Helper con le stringe valide per le proprietà da usare nell'eventualità che il messaggio venga gestito dal ServiceBus.
    /// Determina le chiavi sulle quali si appoggiano le regole di business per l'instradamento dei messaggi
    /// </summary>
    public partial class DocSuiteCustomPropertyNames
    {
        /// <summary>
        /// ID
        /// </summary>
        public const string ID = "Id";

        /// <summary>
        /// NAME
        /// </summary>
        public const string NAME = "Name";

        /// <summary>
        /// TenantName
        /// </summary>
        public const string TENANT_NAME = "TenantName";

        /// <summary>
        /// TenantId
        /// </summary>
        public const string TENANT_ID = "TenantId";

        /// <summary>
        /// Identity
        /// </summary>
        public const string IDENTITY = "Identity";

        /// <summary>
        /// AuthorizationType
        /// </summary>
        public const string AUTHORIZATION_TYPE = "AuthorizationType";

        /// <summary>
        /// MessageDate
        /// </summary>
        public const string MESSAGE_DATE = "MessageDate";

        /// <summary>
        /// UniqueId
        /// </summary>
        public const string UNIQUE_ID = "UniqueId";

        /// <summary>
        /// MessageName
        /// </summary>
        public const string MESSAGE_NAME = "MessageName";

        /// <summary>
        /// MessageType
        /// </summary>
        public const string MESSAGE_TYPE = "MessageType";

        /// <summary>
        /// ExecutorUser
        /// </summary>
        public const string EXECUTOR_USER = "ExecutorUser";

        /// <summary>
        /// ContentType
        /// </summary>
        public const string CONTENT_TYPE = "ContentType";

    }
}
