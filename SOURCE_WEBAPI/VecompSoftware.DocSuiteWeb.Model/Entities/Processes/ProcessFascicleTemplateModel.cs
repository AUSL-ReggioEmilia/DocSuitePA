using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.Processes
{
    public class ProcessFascicleTemplateModel
    {
        #region [ Constructors ]

        public ProcessFascicleTemplateModel()
        {
        }

        #endregion

        #region[ Properties ]

        public Guid? UniqueId { get; set; }
        public string Name { get; set; }
        public string JsonModel { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string RegistrationUser { get; set; }

        #endregion

    }
}
