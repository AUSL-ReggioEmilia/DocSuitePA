using System;
using VecompSoftware.DocSuite.Public.Core.Models.Securities;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello base del valore booleano
    /// </summary>
    public sealed class BooleanModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello base del valore booleano
        /// </summary>
        /// <param name="value">Specifica il valore true o false</param>
        public BooleanModel(bool value)
        {
            Value = value;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Valore booleano true o false
        /// </summary>
        public bool Value { get; private set; }

        #endregion
    }
}
