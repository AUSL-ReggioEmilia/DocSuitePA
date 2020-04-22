namespace VecompSoftware.DocSuiteWeb.API
{
    public class MailboxDTO : IMailboxDTO
    {
        #region [ Constructors ]

        public MailboxDTO()
        {
        }

        public MailboxDTO(string address)
        {
            this.Address = address;
            this.Name = this.Address;
        }

        #endregion

        #region [ Properties ]

        public string TypeName { get; set; }

        public int? Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return this.Id.HasValue && !this.Id.Equals(0);
        }

        #endregion
    }
}