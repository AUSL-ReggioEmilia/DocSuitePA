namespace VecompSoftware.DocSuiteWeb.API
{
    public class CategoryDTO : ICategoryDTO
    {
        #region [ Constructors ]

        public CategoryDTO()
        {
        }

        public CategoryDTO(int? id)
        {
            this.Id = id;
        }

        #endregion

        #region [ Properties ]

        public int? Id { get; set; }

        public string Name { get; set; }

        public string FullCode { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return this.Id.HasValue && !this.Id.Equals(0);
        }

        #endregion
    }
}