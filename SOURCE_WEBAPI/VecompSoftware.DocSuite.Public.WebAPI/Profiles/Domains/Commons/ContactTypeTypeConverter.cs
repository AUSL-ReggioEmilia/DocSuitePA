using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Commons
{
    public class ContactTypeTypeConverter : ITypeConverter<string, ContactType>
    {
        /*
        M	Amministrazione
        A	Area Organizzativa Omogenea (AOO)
        U	Unità Organizzativa (AO)
        R	Ruolo
        P	Persona
        G	Gruppo
        D	Persona AdAm
        I	Pubblica amministrazione da IPA
        S	Settore 
     */

        public ContactType Convert(string source, ContactType destination, ResolutionContext context)
        {
            ContactType result = ContactType.Invalid;
            switch (source)
            {
                case "M":
                    {
                        result = ContactType.Administration;
                        break;
                    }
                case "A":
                    {
                        result = ContactType.AOO;
                        break;
                    }
                case "MA":
                    {
                        result = ContactType.AOOManual;
                        break;
                    }
                case "U":
                    {
                        result = ContactType.AO;
                        break;
                    }
                case "R":
                    {
                        result = ContactType.Role;
                        break;
                    }
                case "P":
                    {
                        result = ContactType.Citizen;
                        break;
                    }
                case "MP":
                    {
                        result = ContactType.CitizenManual;
                        break;
                    }
                case "G":
                    {
                        result = ContactType.Group;
                        break;
                    }
                case "MD":
                case "D":
                    {
                        result = ContactType.Citizen;
                        break;
                    }
                case "MI":
                case "I":
                    {
                        result = ContactType.IPA;
                        break;
                    }
                case "S":
                    {
                        result = ContactType.Sector;
                        break;
                    }

                default:
                    break;
            }
            return result;
        }
    }
}