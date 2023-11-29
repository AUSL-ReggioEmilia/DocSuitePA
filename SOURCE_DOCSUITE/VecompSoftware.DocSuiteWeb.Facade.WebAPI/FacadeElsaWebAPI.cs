using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI
{
    public class FacadeElsaWebAPI : IFacadeElsaWebAPI
    {
        #region [ Fields ]
        private readonly string _logName = LogName.FileLog;
        private readonly string _preparePECMailDocumentsWorkflow = "preparePECMailDocuments";
        private readonly string _elsaBaseUrl;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public FacadeElsaWebAPI(string elsaBaseUrl)
        {
            _elsaBaseUrl = elsaBaseUrl;
        }
        #endregion

        #region [ Methods ]
        public string StartPreparePECMailDocumentsWorkflow(Guid pecMailId, List<DocumentInfoModel> docInfos)
        {
            FileLogger.Info(_logName, $"Starting Elsa Workflow {_preparePECMailDocumentsWorkflow} with PECMailId: {pecMailId}");
            PreparePECMailDocumentsWorkflow payload = new PreparePECMailDocumentsWorkflow()
            {
                PECMailId = pecMailId,
                UniqueId = Guid.NewGuid(),
                DocumentInfos = docInfos
            };
            ElsaWebAPIHelper elsaWebAPIHelper = new ElsaWebAPIHelper($"{_elsaBaseUrl}{_preparePECMailDocumentsWorkflow}");
            string result = Task.Run(() => elsaWebAPIHelper.SendPostRequestAsync(_preparePECMailDocumentsWorkflow, JsonConvert.SerializeObject(payload))).Result;
            FileLogger.Info(_logName, $"Workflow started with result: {result}");
            return result;
        }
        #endregion
    }
}
