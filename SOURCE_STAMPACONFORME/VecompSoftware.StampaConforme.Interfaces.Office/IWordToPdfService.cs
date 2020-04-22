using System;
using VecompSoftware.StampaConforme.Interfaces.Common.ServiceRules;
using VecompSoftware.StampaConforme.Interfaces.Common.Services;
using VecompSoftware.StampaConforme.Models.Office.Word;

namespace VecompSoftware.StampaConforme.Interfaces.Office
{
    public interface IWordToPdfService : IDisposable
    {
        bool SaveTo(SaveDocumentToPdfRequest model);
    }
}
