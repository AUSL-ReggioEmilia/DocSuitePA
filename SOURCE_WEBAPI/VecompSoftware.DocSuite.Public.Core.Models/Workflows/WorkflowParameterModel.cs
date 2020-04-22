using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows
{
    /// <summary>
    ///  Classe dei parametri di avvio del workflow. 
    /// </summary>
    public sealed class WorkflowParameterModel : BaseModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Parametri di avvio del workflow. 
        /// </summary>
        /// <param name="parameterModel">contiente un modello che deriva dalla interfaccia generica IWorkflowModel. 
        /// Le classi esistenti sono le seguenti:
        ///     - ArchiveModel 
        ///     - CategoryModel 
        ///     - ContactModel 
        ///     - DocumentModel 
        ///     - DocumentUnitModel
        ///     - FascicleModel
        ///     - PaperworkModel
        ///     - ReferenceModel
        ///     - SignerModel
        ///     </param>
        /// <param name="parameterName">Determina il nome del parametro. Attenzione per popolare i valori corretti usare la classe <see cref="Parameters.WorkflowParameterNames"/> </param>
        public WorkflowParameterModel(IWorkflowModel parameterModel, string parameterName)
            : base(Guid.NewGuid())
        {
            Name = parameterModel?.GetType().Name;
            ParameterName = parameterName;
            ParameterModel = parameterModel;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determina il nome del parametro. Attenzione per popolare i valori corretti usare la classe <see cref="Parameters.WorkflowParameterNames"/> 
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// contiente un modello che deriva dalla interfaccia generica IWorkflowModel. 
        /// Le classi esistenti sono le seguenti:
        ///     - ArchiveModel 
        ///     - CategoryModel 
        ///     - ContactModel 
        ///     - DocumentModel 
        ///     - DocumentUnitModel
        ///     - FascicleModel
        ///     - PaperworkModel
        ///     - ReferenceModel
        ///     - SignerModel
        /// </summary>
        public IWorkflowModel ParameterModel { get; private set; }

        #endregion
    }
}
