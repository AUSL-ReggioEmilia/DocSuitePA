namespace VecompSoftware.DocSuite.SPID.Model.SAML
{
    public class SamlUser
    {
        public string IdUser { get; set; }

        public string SpidCode { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string FamilyName { get; set; }

        public string PlaceOfBirth { get; set; }

        public string CountyOfBirth { get; set; }

        public string DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string CompanyName { get; set; }

        public string RegisteredOffice { get; set; }

        public string FiscalNumber { get; set; }

        public string IvaCode { get; set; }

        public string IdCard { get; set; }

        public string MobilePhone { get; set; }

        public string Email { get; set; }

        public string PEC { get; set; }

        public string Address { get; set; }

        public string ExpirationDate { get; set; }

        public string DigitalAddress { get; set; }

        public string IdpReferenceId
        {
            get
            {
                //No SPID auth
                if (string.IsNullOrEmpty(SpidCode))
                {
                    return FiscalNumber;
                }
                return SpidCode;
            }
        }

        public string Subject
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return CompanyName;
                }
                return string.Concat(Name, " ", Surname);
            }
        }

        public bool IsPerson
        {
            get
            {
                return string.IsNullOrEmpty(CompanyName);
            }
        }
    }
}
