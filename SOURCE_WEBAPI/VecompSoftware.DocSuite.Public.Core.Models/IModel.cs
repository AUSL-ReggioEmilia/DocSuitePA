using System;

namespace VecompSoftware.DocSuite.Public.Core.Models
{
    /// <summary>
    /// Interfaccia generalista del modello
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Identificativo univivoco del modello
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Nome del modello
        /// </summary>
        string Name { get; set; }
    }
}