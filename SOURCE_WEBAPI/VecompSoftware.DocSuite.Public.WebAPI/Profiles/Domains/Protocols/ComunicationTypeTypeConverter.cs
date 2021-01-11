using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Protocols
{
    public class ComunicationTypeTypeConverter : ITypeConverter<string, ComunicationType>
    {
        /*
            M = Sender,
            D = Recipient
        */

        public ComunicationType Convert(string source, ComunicationType destination, ResolutionContext context)
        {
            ComunicationType result = ComunicationType.Invalid;
            switch (source)
            {
                case "M":
                    {
                        result = ComunicationType.Sender;
                        break;
                    }
                case "D":
                    {
                        result = ComunicationType.Recipient;
                        break;
                    }
                default:
                    break;
            }
            return result;
        }
    }
}