using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Document.TelerikReport.Verbs;

namespace VecompSoftware.DocSuite.Document.TelerikReport.Clients
{
    public interface ITelerikReportClient
    {
        Task<ICollection<T>> GetAsync<T>(GetAction action, string parameter);
        Task<byte[]> PostAsync<T>(PostAction action, T data);
    }
}
