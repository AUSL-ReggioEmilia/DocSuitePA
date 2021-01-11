using System;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.ServiceBus.Receiver.Base.Exceptions
{
    public class ServiceBusEvaluationException : Exception
    {
        #region [ Fields ]
        private readonly EvaluationModel _evaluationModel;
        #endregion
        public ServiceBusEvaluationException(EvaluationModel evaluationModel)
        {
            _evaluationModel = evaluationModel;
        }

        #region [ Properties ]
        public EvaluationModel EvaluationModel => _evaluationModel;
        #endregion
    }
}
