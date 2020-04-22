using System.Linq;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.Mapper.FederaIdp
{
    public class SamlUserMapper : ISamlUserMapper
    {
        public SamlUser Map(ResponseType response)
        {
            SamlUser user = new SamlUser();
            AssertionType assertion = response.Items.OfType<AssertionType>().FirstOrDefault();
            if (assertion != null)
            {
                AttributeStatementType attributeStatement = assertion.Items.OfType<AttributeStatementType>().FirstOrDefault();
                if (attributeStatement != null)
                {
                    foreach (AttributeType attribute in attributeStatement.Items.OfType<AttributeType>())
                    {
                        switch (attribute.Name)
                        {
                            case "spidCode":
                                user.SpidCode = attribute.AttributeValue[0].ToString();
                                break;
                            case "IdUtente":
                                user.IdUser = attribute.AttributeValue[0].ToString();
                                break;
                            case "nome":
                                user.Name = attribute.AttributeValue[0].ToString();
                                break;
                            case "cognome":
                                user.Surname = attribute.AttributeValue[0].ToString();
                                break;
                            case "sesso":
                                user.Gender = attribute.AttributeValue[0].ToString();
                                break;
                            case "ivaCode":
                                user.IvaCode = attribute.AttributeValue[0].ToString();
                                break;
                            case "companyName":
                                user.CompanyName = attribute.AttributeValue[0].ToString();
                                break;
                            case "cellulare":
                                user.MobilePhone = attribute.AttributeValue[0].ToString();
                                break;
                            case "address":
                                user.Address = attribute.AttributeValue[0].ToString();
                                break;
                            case "CodiceFiscale":
                                user.FiscalNumber = attribute.AttributeValue[0].ToString();
                                break;
                            case "dataNascita":
                                user.DateOfBirth = attribute.AttributeValue[0].ToString();
                                break;
                            case "provinciaNascita":
                                user.CountyOfBirth = attribute.AttributeValue[0].ToString();
                                break;
                            case "luogoNascita":
                                user.PlaceOfBirth = attribute.AttributeValue[0].ToString();
                                break;
                            case "idCard":
                                user.IdCard = attribute.AttributeValue[0].ToString();
                                break;
                            case "registeredOffice":
                                user.RegisteredOffice = attribute.AttributeValue[0].ToString();
                                break;
                            case "emailAddress":
                                user.PEC = attribute.AttributeValue[0].ToString();
                                break;
                            case "emailAddressPersonale":
                                user.Email = attribute.AttributeValue[0].ToString();
                                break;
                            case "expirationDate":
                                user.ExpirationDate = attribute.AttributeValue[0].ToString();
                                break;
                            case "digitalAddress":
                                user.DigitalAddress = attribute.AttributeValue[0].ToString();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return user;
        }
    }
}
