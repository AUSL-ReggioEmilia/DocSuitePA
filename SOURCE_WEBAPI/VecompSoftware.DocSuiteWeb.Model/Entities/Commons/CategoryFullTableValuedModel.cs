using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class CategoryFullTableValuedModel
    {
        #region [ Constructor ]
        public CategoryFullTableValuedModel()
        {

        }
        #endregion

        #region [ Properties ]
        public short IdCategory { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public short? Code { get; set; }

        public string FullIncrementalPath { get; set; }

        public string FullCode { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public Guid? UniqueId { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public bool? HasChildren { get; set; }

        public bool? HasFascicleDefinition { get; set; }
        #endregion

        #region [ Navigation properties ]

        #region [ Parent ]
        public short? CategoryParent_IdCategory { get; set; }
        #endregion

        #endregion

        #region [ Methods ]

        #endregion
    }
}
