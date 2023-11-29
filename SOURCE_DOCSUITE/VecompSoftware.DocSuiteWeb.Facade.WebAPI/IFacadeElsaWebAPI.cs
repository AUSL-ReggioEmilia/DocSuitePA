using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI
{
    public interface IFacadeElsaWebAPI
    {
        string StartPreparePECMailDocumentsWorkflow(Guid pecMailId, List<DocumentInfoModel> docInfos);
    }
}
