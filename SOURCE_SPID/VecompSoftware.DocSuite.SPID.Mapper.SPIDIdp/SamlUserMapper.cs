using System.Linq;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.Mapper.SPIDIdp
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
                            case "name":
                                user.Name = attribute.AttributeValue[0].ToString();
                                break;
                            case "familyName":
                                user.Surname = attribute.AttributeValue[0].ToString();
                                break;
                            case "gender":
                                user.Gender = attribute.AttributeValue[0].ToString();
                                break;
                            case "ivaCode":
                                user.IvaCode = attribute.AttributeValue[0].ToString().Replace("VATIT-", string.Empty);
                                break;
                            case "companyName":
                                user.CompanyName = attribute.AttributeValue[0].ToString();
                                break;
                            case "mobilePhone":
                                user.MobilePhone = attribute.AttributeValue[0].ToString();
                                break;
                            case "address":
                                user.Address = attribute.AttributeValue[0].ToString();
                                break;
                            case "fiscalNumber":
                                user.FiscalNumber = attribute.AttributeValue[0].ToString().Replace("TINIT-", string.Empty);
                                break;
                            case "dateOfBirth":
                                user.DateOfBirth = attribute.AttributeValue[0].ToString();
                                break;
                            case "countyOfBirth":
                                user.CountyOfBirth = attribute.AttributeValue[0].ToString();
                                break;
                            case "placeOfBirth":
                                user.PlaceOfBirth = attribute.AttributeValue[0].ToString();
                                break;
                            case "idCard":
                                user.IdCard = attribute.AttributeValue[0].ToString();
                                break;
                            case "registeredOffice":
                                user.RegisteredOffice = attribute.AttributeValue[0].ToString();
                                break;
                            case "email":
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
