using Newtonsoft.Json;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models
{
    public class AccountModel
    {
        public AccountModel(string description)
        {
            Description = description;
        }

        [JsonProperty("email")]
        public string EmailAddress { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        public string FirstName
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                {
                    return string.Empty;
                }                    
                return Description.Split('|')[0];
            }
        }
        public string LastName
        {
            get
            {
                if (string.IsNullOrEmpty(Description) || Description.Split('|').Length == 1)
                {
                    return string.Empty;
                }
                return Description.Split('|')[1];
            }
        }        
        public string Telephone { get; set; }
        public string FiscalCode { get; set; }
    }
}
