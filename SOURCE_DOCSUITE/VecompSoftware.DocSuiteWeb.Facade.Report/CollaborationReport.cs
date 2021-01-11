using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Report;
using VecompSoftware.DocSuiteWeb.Report.Templates.Protocol;

namespace VecompSoftware.DocSuiteWeb.Facade.Report
{
    public class CollaborationReport : ReportBase<Collaboration>
    {
        private FacadeFactory _factory;

        public CollaborationReport() : base(ReportType.Rdlc) { }

        protected FacadeFactory Factory
        {
            get { return _factory ?? (_factory = new FacadeFactory("ProtDB")); }
        }

        private ProtocolDataSet _protocolDataSet;

        protected override IList<Collaboration> Items
        {
            get
            {
                return base.Items.ToList();
            }
        }

        protected override void DataBind()
        {
            _protocolDataSet = new ProtocolDataSet();
            foreach (var collaboration in Items)
            {
                
                var row = _protocolDataSet.CollaborationDataTable.NewCollaborationDataTableRow();
                row.Collaboration_ID = collaboration.Id.ToString();
                row.Collaboration_Object = collaboration.CollaborationObject;
                row.Collaboration_RegistrationDate = collaboration.RegistrationDate.ToString("dd/MM/yyyy");
                _protocolDataSet.CollaborationDataTable.Rows.Add(row);
            }
            PrimaryTableName = "CollaborationDataTable";
            DataSource = _protocolDataSet;
        }
    }
}
