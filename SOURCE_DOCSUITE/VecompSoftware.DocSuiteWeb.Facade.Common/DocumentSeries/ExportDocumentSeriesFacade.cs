using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.DocumentSeries
{
    public class ExportDocumentSeriesFacade
    {
        public static Byte[] ExportToCsv(IList<DocumentSeriesItem> items)
        {
            try
            {
                ArchiveInfo archive = DocumentSeriesFacade.GetArchiveInfo(items.First().DocumentSeries);
                List<ArchiveAttribute> archiveAtt = archive.Attributes;

                IExportRepository wrapper = new FileSystemRepository();
                wrapper.InitializeExport(items, archiveAtt);
                return wrapper.StartExport(items.First().DocumentSeries.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
