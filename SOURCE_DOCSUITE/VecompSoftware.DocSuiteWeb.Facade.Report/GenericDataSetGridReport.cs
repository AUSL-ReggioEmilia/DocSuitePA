using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Facade.Report.Helpers;
using VecompSoftware.DocSuiteWeb.Report;

namespace VecompSoftware.DocSuiteWeb.Facade.Report
{
    public class GenericDataSetGridReport<T> : ReportBase<T>
    {
        private FacadeFactory _factory;

        public GenericDataSetGridReport() : base(ReportType.Memory)
        {
        }

        protected FacadeFactory Factory
        {
            get { return _factory ?? (_factory = new FacadeFactory("ProtDB")); }
        }

        protected override IList<T> Items
        {
            get { return base.Items.ToList(); }
        }

        protected override void DataBind()
        {
            DataSource = DataSetConvertor.Convert(RawDataSet);
        }
    }
}