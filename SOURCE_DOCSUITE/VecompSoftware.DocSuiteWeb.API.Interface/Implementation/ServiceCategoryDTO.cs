namespace VecompSoftware.DocSuiteWeb.API
{
    public class ServiceCategoryDTO : IServiceCategoryDTO
    {
        #region [ Constructor ]
        public ServiceCategoryDTO()
        {

        }

        public ServiceCategoryDTO(int id)
        {
            Id = id;
        }
        #endregion

        #region [ Properties ]
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        #endregion

        #region [ Methods ]
        public bool HasId()
        {
            return this.Id.HasValue;
        }
        #endregion
    }
}
