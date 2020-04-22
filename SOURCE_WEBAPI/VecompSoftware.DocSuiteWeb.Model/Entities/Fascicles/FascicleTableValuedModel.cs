using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleTableValuedModel : ICategoryTableValuedModel, IContactTableValuedModel
    {
        #region [ Constructor ]
        public FascicleTableValuedModel()
        {
            UniqueId = Guid.NewGuid();
        }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public short Year { get; set; }

        public int Number { get; set; }

        public short? Conservation { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string FascicleObject { get; set; }

        public string Manager { get; set; }

        public string Rack { get; set; }

        public string Note { get; set; }

        public FascicleType FascicleType { get; set; }

        public VisibilityType VisibilityType { get; set; }


        #region [ Category ]

        public short? Category_IdCategory { get; set; }

        public string Category_Name { get; set; }

        #endregion

        #region [ Contact ]
        public int? Contact_Incremental { get; set; }

        public string Contact_Description { get; set; }
        #endregion

        #endregion
    }
}
