using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class AccountingSectionalDTO : IAccountingSectionalDTO
    {
        #region [ Constructors ]

        public AccountingSectionalDTO()
        {
        }

        public AccountingSectionalDTO(int? id)
        {
            this.Id = id;
        }

        #endregion

        #region [ Properties ]

        public int? Id { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContainerDTO, IContainerDTO>))]
        public IContainerDTO Container { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return this.Id.HasValue && !this.Id.Equals(0);
        }

        #endregion
    }
}