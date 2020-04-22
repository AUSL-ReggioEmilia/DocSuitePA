using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Processes
{
    public class ProcessModel
    {
        #region [ Constructors ]

        public ProcessModel()
        {
        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public FascicleType FascicleType { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }
        public ProcessType ProcessType { get; set; }
        public CategoryModel Category { get; set; }        
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
