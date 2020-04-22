using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.DocumentSeries
{
    public class BaseRepository
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        private static DocumentSeriesItemFacade _currentDocumentSeriesItemFacade;
        public static DocumentSeriesItemFacade CurrentDocumentSeriesItemFacade()
        {
            if (_currentDocumentSeriesItemFacade == null)
            {
                _currentDocumentSeriesItemFacade = new DocumentSeriesItemFacade();
            }
            return _currentDocumentSeriesItemFacade;
        }
        public IList<SeriesToCsvFields> CsvFields { get; private set; }
        public IList<ArchiveAttribute> ArchiveAttribute { get; set; }

        #endregion

        #region [ Constructor ]

        public BaseRepository()
        {
            CsvFields = new List<SeriesToCsvFields>();
            ArchiveAttribute = new List<ArchiveAttribute>();
        }

        #endregion

        public void InitializeExport(IList<DocumentSeriesItem> items, IList<ArchiveAttribute> serieAttributes)
        {
            CsvFields.Clear();
            ArchiveAttribute.Clear();

            ArchiveAttribute = serieAttributes;

            if (!items.Any())
                return;

            foreach (DocumentSeriesItem item in items)
            {
                Dictionary<string, string> attributes = CurrentDocumentSeriesItemFacade().GetAttributes(item);

                var seriesToCsv = new SeriesToCsvFields();
                seriesToCsv.Object = item.Subject;
                seriesToCsv.PublicationDate = item.PublishingDate;
                seriesToCsv.IdSeries = item.DocumentSeries.Id;
                seriesToCsv.Year = item.Year == null ? string.Empty : item.Year.ToString();
                seriesToCsv.Number = item.Number == null ? string.Empty : item.Number.ToString();
                seriesToCsv.RegistrationDate = DateTime.Parse(item.RegistrationDate.ToString());
                seriesToCsv.Categorry = item.Category.GetFullName();

                foreach (var data in ArchiveAttribute)
                {
                    var value = string.Empty;
                    var keyExist = attributes.Any(x => x.Key.Equals(data.Name));
                    if (keyExist)
                        value =
                            attributes.Where(x => x.Key.Equals(data.Name)).Select(s => s.Value).SingleOrDefault();
                    if (ExporterHelper.EnumTipoProcedimento.Equals(data.Name))
                    {
                        value = ExporterHelper.GetTipoProcedimento(value);
                    }
                    var dict = new KeyValuePair<string, object>(data.Name, value);

                    seriesToCsv.DynamicData.Add(dict);
                }

                CsvFields.Add(seriesToCsv);                
            }
        }
    }
}
