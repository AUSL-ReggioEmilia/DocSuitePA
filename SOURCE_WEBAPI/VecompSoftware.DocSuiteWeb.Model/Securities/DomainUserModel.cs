using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Securities
{
    public class DomainUserModel : DomainBaseModel
    {
        private string _account = string.Empty;
        private string _fullAccount = string.Empty;

        public DomainUserModel()
        {
            DomainGroups = new HashSet<DomainGroupModel>();
            Rights = new HashSet<SecurityRight>();
        }

        //
        // Summary:
        //     Gets or sets the e-mail address for this account.
        //
        // Returns:
        //     The e-mail address of the user principal.
        //
        public string EmailAddress { get; set; }
        //
        // Summary:
        //     Gets or sets the employee ID for this user principal.
        //
        // Returns:
        //     The employee ID of the user principal.
        //
        public string EmployeeId { get; set; }
        //
        // Summary:
        //     Gets or sets the given name for the user principal.
        //
        // Returns:
        //     The given name of the user principal.
        //
        public string GivenName { get; set; }
        //
        // Summary:
        //     Gets or sets the middle name for the user principal.
        //
        // Returns:
        //     The middle name of the user principal.
        //
        public string MiddleName { get; set; }
        //
        // Summary:
        //     Gets or sets the surname for the user principal.
        //
        // Returns:
        //     The surname of the user principal.
        //
        public string Surname { get; set; }
        //
        // Summary:
        //     Gets or sets the voice telephone number for the user principal.
        //
        // Returns:
        //     The voice telephone number of the user principal.
        //
        public string VoiceTelephoneNumber { get; set; }

        public string Domain { get; set; }

        public string ClientMachineName { get; set; }

        public ICollection<DomainGroupModel> DomainGroups { get; set; }

        //public IDictionary<DSWEnvironmentType, SecurityRight> Rights { get; set; }
        public ICollection<SecurityRight> Rights { get; set; }

        public string Account
        {
            get
            {
                if (string.IsNullOrEmpty(_account))
                {
                    _account = $"{Domain}\\{Name}";
                }
                return _account;
            }
            set { ; }
        }

        public string FullAccountInformation
        {
            get
            {
                if (string.IsNullOrEmpty(_fullAccount))
                {
                    _fullAccount = $"{Account} ({DisplayName} - {EmailAddress})";
                    if (string.IsNullOrEmpty(EmailAddress))
                    {
                        _fullAccount = $"{Account} ({DisplayName})";
                        
                    }
                }
                return _fullAccount;
            }

            set {; }
        }
    }
}
