using System.Collections.Generic;
using System.IO;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.DocumentSeries
{
    interface IExportRepository
    {
        void InitializeExport(IList<DocumentSeriesItem> item, IList<ArchiveAttribute> archiveAttribute);
        SeriesToCsvFields GetHeaderDynamicFields();
        byte[] StartExport(int idSeries);
    }
}
