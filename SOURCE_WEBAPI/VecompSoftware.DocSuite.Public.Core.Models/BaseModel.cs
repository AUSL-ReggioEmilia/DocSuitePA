using System;

namespace VecompSoftware.DocSuite.Public.Core.Models
{
    /// <summary>
    /// Modello generico
    /// </summary>
    public abstract class BaseModel : IModel
    {
        #region [ Contructors ]
        /// <summary>
        /// Modello generico
        /// </summary>
        /// <param name="id">Identificativo univivoco del modello</param>
        public BaseModel(Guid id)
        {
            Id = id;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Identificativo univivoco del modello
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Nome del modello
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}