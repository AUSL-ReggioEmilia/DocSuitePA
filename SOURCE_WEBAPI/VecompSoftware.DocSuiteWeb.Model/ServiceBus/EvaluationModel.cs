using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.ServiceBus
{
    public class EvaluationModel
    {
        #region [ Constructor ]

        public EvaluationModel()
        {
            Steps = new List<StepModel>();
        }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public Guid? CorrelationId { get; set; }
        public string ReferenceModel { get; set; }
        public string ModuleName { get; set; }
        public string CommandName { get; set; }
        public List<StepModel> Steps { get; set; }
        #endregion
    }
}
