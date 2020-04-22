using System;
using VecompSoftware.StampaConforme.Interfaces.Common.ServiceRules;
using VecompSoftware.StampaConforme.Interfaces.Common.Services;
using VecompSoftware.StampaConforme.Models.Office.Outlook;

namespace VecompSoftware.StampaConforme.Interfaces.Office
{
    public interface IOutlookToPdfService : IDisposable
    {
        bool SaveTo(SaveMailToPdfRequest model);
    }
}
