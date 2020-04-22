using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Reports
{
    public class ReportBuilderModel
    {
        #region [ Constructor ]
        public ReportBuilderModel()
        {
            Projections = new List<ReportBuilderProjectionModel>();
            Conditions = new List<ReportBuilderConditionModel>();
            SortItems = new List<ReportBuilderSortModel>();
        }
        #endregion

        #region [ Properties ]
        public DSWEnvironmentType Entity { get; set; }
        public Guid? MetadataRepositoryId { get; set; }
        public DSWEnvironmentType? DocumentUnit { get; set; }
        public ICollection<ReportBuilderProjectionModel> Projections { get; set; }
        public ICollection<ReportBuilderConditionModel> Conditions { get; set; }
        public ICollection<ReportBuilderSortModel> SortItems { get; set; }
        #endregion
    }
}
