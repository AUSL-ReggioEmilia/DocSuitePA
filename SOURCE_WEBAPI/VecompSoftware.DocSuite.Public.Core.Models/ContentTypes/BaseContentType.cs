
namespace VecompSoftware.DocSuite.Public.Core.Models.ContentTypes
{
    /// <summary>
    /// Classe astratta base che definisce un contentuto generico
    /// </summary>
    /// <typeparam name="TModel">Tipo del modello di trasaporto</typeparam>
    public abstract class BaseContentType<TModel> : IContentType<TModel>
        where TModel : IModel
    {
        #region [ Fields ]
        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Classe astratta base che definisce un contentuto generico
        /// </summary>
        /// <param name="content">Modello trasportato</param>
        public BaseContentType(TModel content)
        {
            Content = content;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Modello trasportato
        /// </summary>
        public TModel Content { get; private set; }
        #endregion
    }
}
