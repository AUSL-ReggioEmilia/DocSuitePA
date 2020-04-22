namespace VecompSoftware.DocSuiteWeb.API
{
    public class ContainerDTO : IContainerDTO
    {
        #region [ Constructors ]

        public ContainerDTO()
        {
        }

        public ContainerDTO(int? id)
        {
            this.Id = id;
        }

        #endregion

        #region [ Properties ]

        public int? Id { get; set; }

        public string Name { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return this.Id.HasValue && !this.Id.Equals(0);
        }

        #endregion
    }
}