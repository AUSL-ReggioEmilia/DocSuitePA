namespace VecompSoftware.DocSuite.Public.Core.Models.ContentTypes.Workflows
{
    /// <summary>
    /// Classe astratta di un ContentType per i comandi del Workflow
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class BaseWorkflowContentType<TModel> : BaseContentType<TModel>
        where TModel : IModel
    {
        #region [ Fields ]
        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Classe astratta di un ContentType per i comandi del Workflow
        /// </summary>
        /// <param name="content">Modello che viene trasportato dal comando</param>
        /// <param name="executorUser">Utente applicativo che sta eseguendo il comando.</param>
        public BaseWorkflowContentType(TModel content, string executorUser)
            : base(content)
        {
            ExecutorUser = executorUser;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Utente applicativo che sta eseguendo il comando.
        /// </summary>
        public string ExecutorUser { get; private set; }
        #endregion
    }
}