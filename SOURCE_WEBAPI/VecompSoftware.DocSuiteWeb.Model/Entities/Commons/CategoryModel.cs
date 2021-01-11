
using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class CategoryModel
    {
        #region [ Constructors ]

        public CategoryModel()
        {
        }

        public CategoryModel(int? id)
        {
            IdCategory = id;
        }

        #endregion

        #region [ Properties ]

        public int? IdCategory { get; set; }

        public string Name { get; set; }

        public string FullCode { get; set; }

        public short? Code { get; set; }

        public Guid? UniqueId { get; set; }

        public string FullIncrementalPath { get; set; }

        public bool? HasChildren { get; set; }

        public int? IdParent { get; set; }

        public bool? HasFascicleDefinition { get; set; }

        public CategoryModelType CategoryType { get; set; }

        #endregion

        #region [ Navigation properties ]

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return IdCategory.HasValue && !IdCategory.Equals(0);
        }

        #endregion
    }
}
