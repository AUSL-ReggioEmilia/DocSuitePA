namespace VecompSoftware.DocSuite.Public.Core.Models.ContentTypes
{
    /// <summary>
    /// Interfaccia che definisce un contentuto generico
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IContentType<TModel>
        where TModel : IModel
    {
        /// <summary>
        /// Modello trasportato
        /// </summary>
        TModel Content { get; }
    }
}