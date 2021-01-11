using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Report;
using VecompSoftware.DocSuiteWeb.Report.Templates.Resolution;

namespace VecompSoftware.DocSuiteWeb.Facade.Report
{
    public class ResolutionLetterReport : ReportBase<Resolution>
    {
        private FacadeFactory _factory;

        public ResolutionLetterReport() : base(ReportType.Rdlc) { }

        protected FacadeFactory Factory
        {
            get { return _factory ?? (_factory = new FacadeFactory("ReslDB")); }
        }

        private ResolutionDataSet _resolutionDataSet;

        protected override IList<Resolution> Items
        {
            get
            {
                return base.Items.ToList();
            }
        }

        protected override void DataBind()
        {
            _resolutionDataSet = new ResolutionDataSet();
            foreach (var resolution in Items)
            {
                var row = _resolutionDataSet.ResolutionLetterDataTable.NewResolutionLetterDataTableRow();
                row.Resolution_ID = resolution.Id.ToString(CultureInfo.InvariantCulture);
                row.Resolution_Year = resolution.Year.ToString();
                row.Resolution_Number = resolution.Number.ToString();
                row.Resolution_FullNumber = ResolutionJournalPrinter.GetDetermina(resolution);
                row.Resolution_Controllo = ResolutionJournalPrinter.GetControllo(resolution);
                row.Resolution_AdoptionDate = resolution.AdoptionDate.HasValue ? resolution.AdoptionDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                row.Resolution_PublishingDate = resolution.PublishingDate.HasValue ? resolution.PublishingDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                row.Resolution_EffectivenessDate = resolution.EffectivenessDate.HasValue ? resolution.EffectivenessDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                row.Resolution_RetiredDate = resolution.LeaveDate.HasValue ? resolution.LeaveDate.Value.ToString("dd/MM/yyyy") : string.Empty;

                row.Resolution_Object = resolution.ResolutionObject;

                // CONTENITORE
                if (resolution.Container != null)
                {
                    row.Container_ID = resolution.Container.Id;
                    row.Container_Description = resolution.Container.Name;
                    row.Resolution_HeadingLetter = resolution.Container.HeadingLetter;
                }

                _resolutionDataSet.ResolutionLetterDataTable.Rows.Add(row);
            }
            PrimaryTableName = "ResolutionLetterDataTable";
            DataSource = _resolutionDataSet;
        }
    }
}
