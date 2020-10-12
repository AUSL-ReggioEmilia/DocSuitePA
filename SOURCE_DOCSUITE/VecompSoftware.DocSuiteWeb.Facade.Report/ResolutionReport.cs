using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Report;
using VecompSoftware.DocSuiteWeb.Report.Templates.Resolution;

namespace VecompSoftware.DocSuiteWeb.Facade.Report
{
    public class ResolutionReport : ReportBase<Resolution>
    {
        private FacadeFactory _factory;

        public ResolutionReport() : base(ReportType.Rdlc) { }

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
                var cat = resolution.Category;

                var row = _resolutionDataSet.ResolutionDataTable.NewResolutionDataTableRow();
                row.Resolution_ID = resolution.Id.ToString(CultureInfo.InvariantCulture);
                row.Resolution_Year = resolution.Year.ToString();
                row.Resolution_Number = resolution.Number.ToString();
                row.Resolution_AdoptionDate = resolution.AdoptionDate.HasValue ? resolution.AdoptionDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                row.Resolution_FullNumber = ResolutionJournalPrinter.GetDetermina(resolution);
                row.Resolution_Controllo = ResolutionJournalPrinter.GetControllo(resolution);
                if (resolution.ImmediatelyExecutive.HasValue)
                {
                    row.Resolution_ImmediatelyExecutive = resolution.ImmediatelyExecutive.ToString();
                }

                row.Resolution_Object = resolution.ResolutionObject;
                if (resolution.ProposeDate.HasValue)
                {
                    row.Resolution_RegistrationDate = resolution.ProposeDate.Value;
                    row.Resolution_InclusiveNumber = resolution.InclusiveNumber;
                }
                row.Resolution_RegistrationUser = resolution.ProposeUser;

                // CLASSIFICATORE
                row.Category_ID = cat.Id;
                row.Category_Description = cat.Name;
                row.Category_Code = cat.Code.ToString(CultureInfo.InvariantCulture);
                row.Category_FullCode = cat.FullCodeDotted;


                // CONTENITORE
                if (resolution.Container != null)
                {
                    row.Container_ID = resolution.Container.Id;
                    row.Container_Description = resolution.Container.Name;
                }

                if (resolution.Status != null) row.Resolution_Status = resolution.Status.Description;

                _resolutionDataSet.ResolutionDataTable.Rows.Add(row);
            }
            DataSource = _resolutionDataSet;
        }
    }
}
