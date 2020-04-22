using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace VecompSoftware.JeepService.DocSeriesExporter
{
    public interface IExportRepository
    {
        void InitializeExport(IList<DocumentSeriesItemWSO> itemWso, IList<ArchiveAttributeWSO> archiveAttributeWsos);
        SeriesToCsvFields GetHeaderDynamicFields();
        void StartExport(int idSeries, bool isSubsection = false);
    }
}
