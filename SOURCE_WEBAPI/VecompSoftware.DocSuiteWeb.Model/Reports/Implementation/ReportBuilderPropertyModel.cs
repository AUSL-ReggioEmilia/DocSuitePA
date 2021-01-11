using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Reports
{
    public class ReportBuilderPropertyModel : IReportItem
    {
        #region [ Constructor ]
        public ReportBuilderPropertyModel()
        {
            Children = new List<ReportBuilderPropertyModel>();
        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ReportBuilderPropertyType PropertyType { get; set; }
        public ReportBuilderPropertyCondition Condition { get; set; }
        public DSWEnvironmentType? Environment { get; set; }
        public ICollection<ReportBuilderPropertyModel> Children { get; set; }
        public bool HasChildren => Children.Count > 0;
        #endregion        
    }
}
