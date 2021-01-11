using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Processes
{
    public class ProcessTableValuedModel : ICategoryTableValuedModel
    {
        #region [ Constructors ]

        public ProcessTableValuedModel()
        {
        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string Name { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }
        public ProcessType ProcessType { get; set; }

        #region [ Category ]

        public short? Category_IdCategory { get; set; }
        public string Category_Name { get; set; }
        public short? Category_Code { get; set; }

        #endregion

        #endregion
    }
}
