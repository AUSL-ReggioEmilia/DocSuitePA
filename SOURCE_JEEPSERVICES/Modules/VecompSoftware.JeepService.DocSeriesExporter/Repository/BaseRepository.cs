using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.DocSeriesExporter.CsvHelper;

namespace VecompSoftware.JeepService.DocSeriesExporter.Repository
{
    public abstract class BaseRepository
    {
        #region [ Fields ]
        private readonly DocSeriesExporterParameters _parameters;
        private readonly Lazy<WsSeriesConnector> _wsSeriesConnector;
        #endregion

        #region [ Properties ]

        public IList<SeriesToCsvFields> CsvFields { get; private set; }
        public IList<ArchiveAttributeWSO> ArchiveAttribute { get; set; }

        private WsSeriesConnector InitConnector()
        {
            return new WsSeriesConnector(this._parameters.WsSeriesUrl);
        }

        protected WsSeriesConnector WsSeriesConnector
        {
            get { return _wsSeriesConnector.Value; }
        }

        #endregion

        #region [ Constructor ]

        public BaseRepository(DocSeriesExporterParameters parameters)
        {
            CsvFields = new List<SeriesToCsvFields>();
            ArchiveAttribute = new List<ArchiveAttributeWSO>();
            this._parameters = parameters;
            this._wsSeriesConnector = new Lazy<WsSeriesConnector>(InitConnector);
        }

        #endregion

        public void InitializeExport(IList<DocumentSeriesItemWSO> itemWsos, IList<ArchiveAttributeWSO> archiveAttributeWsos)
        {
            CsvFields.Clear();
            ArchiveAttribute.Clear();

            ArchiveAttribute = archiveAttributeWsos;
            if (this._parameters.ExportDocuments)
            {
                ArchiveAttribute.Add(new ArchiveAttributeWSO() { Description = "DocumentName" });
                ArchiveAttribute.Add(new ArchiveAttributeWSO() { Description = "DocumentType" });
            }

            if (!itemWsos.Any())
                return;

            foreach (var itemWso in itemWsos)
            {
                var seriesToCsv = new SeriesToCsvFields();
                seriesToCsv.Object = itemWso.Subject;
                seriesToCsv.PublicationDate = itemWso.PublishingDate;
                seriesToCsv.IdSeries = itemWso.IdDocumentSeries;
                seriesToCsv.IdSubsection = itemWso.IdDocumentSeriesSubsection;

                foreach (var data in ArchiveAttribute)
                {
                    var value = string.Empty;
                    var keyExist = itemWso.DynamicData.Any(x => x.Key.Eq(data.Name));
                    if (keyExist)
                        value =
                            itemWso.DynamicData.Where(x => x.Key.Eq(data.Name)).Select(s => s.Value).SingleOrDefault();

                    if (ExporterHelper.EnumTipoProcedimento.Eq(data.Name))
                    {
                        value = ExporterHelper.GetTipoProcedimento(value);
                    }

                    var dict = new KeyValuePair<string, object>(data.Description, value);

                    seriesToCsv.DynamicData.Add(dict);
                }

                if (this._parameters.ExportDocuments)
                {
                    DocumentSeriesItemWSO fullItem = this.WsSeriesConnector.GetFullItemWso(itemWso.Id);                    

                    fullItem.MainDocs.ForEach((x) =>
                    {
                        SeriesToCsvFields clone = seriesToCsv.DeepClone();
                        clone.DynamicData["DocumentName"] = x.Name;
                        clone.DynamicData["DocumentType"] = "Documento Principale";
                        CsvFields.Add(clone);
                    });

                    fullItem.AnnexedDocs.ForEach((x) =>
                    {
                        SeriesToCsvFields clone = seriesToCsv.DeepClone();
                        clone.DynamicData["DocumentName"] = x.Name;
                        clone.DynamicData["DocumentType"] = "Annesso";
                        CsvFields.Add(clone);
                    });

                    if (fullItem.MainDocs.Count == 0 && fullItem.AnnexedDocs.Count == 0)
                    {
                        CsvFields.Add(seriesToCsv);
                    }
                }
                else
                {
                    CsvFields.Add(seriesToCsv);
                }                
            }
        }
    }
}
