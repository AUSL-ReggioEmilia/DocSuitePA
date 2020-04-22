using System;
using VecompSoftware.StampaConforme.Interfaces.Common.ServiceRules;
using VecompSoftware.StampaConforme.Interfaces.Common.Services;
using VecompSoftware.StampaConforme.Models.Office.Excel;

namespace VecompSoftware.StampaConforme.Interfaces.Office
{
    public interface IExcelToPdfService : IDisposable
    {
        bool SaveTo(SaveWorkbookToPdfRequest model);
    }
}
