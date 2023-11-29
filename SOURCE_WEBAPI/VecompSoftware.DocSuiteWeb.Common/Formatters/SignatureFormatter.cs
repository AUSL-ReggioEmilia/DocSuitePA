using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Models;

namespace VecompSoftware.DocSuiteWeb.Common.Formatters
{
    public class SignatureFormatter : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string[] formatParameters = format.Split(':');

            SignatureModel formatterModel = arg as SignatureModel;
            if (formatterModel == null)
            {
                throw new ArgumentNullException("FormatterModel cannot be null");
            }
            switch (formatParameters[0])
            {
                case "Short":
                    {
                        return formatterModel.CorporateAcronym;
                    }
                case "Complete":
                    {
                        return formatterModel.CorporateName;
                    }
                case "None":
                    {
                        return string.Empty;
                    }
                case "Number":
                    {
                        return formatterModel.Number.ToString(formatParameters.Length > 1 ? formatParameters[1] : string.Empty);
                    }
                case "Year":
                    {
                        return formatterModel.Year.ToString();
                    }
                case "Date":
                    {
                        return formatterModel.RegistrationDate.ToLocalTime().DateTime.ToString(formatParameters.Length > 1 ? formatParameters[1] : string.Empty);
                    }
                case "Direction":
                    {
                        switch (formatParameters[1])
                        {
                            case "Id":
                                {
                                    return string.Format("{0}/{1:0000000}", formatterModel.Year, formatterModel.Number);
                                }
                            case "Short":
                                {
                                    switch (formatterModel.Typology)
                                    {
                                        case ProtocolTypology.Inbound:
                                            return "I";
                                        case ProtocolTypology.Internal:
                                            return "I/O";
                                        case ProtocolTypology.Outgoing:
                                            return "O";
                                    }
                                    return string.Empty;
                                }
                            case "Complete":
                                switch (formatterModel.Typology)
                                {
                                    case ProtocolTypology.Inbound:
                                        return "ingresso";
                                    case ProtocolTypology.Internal:
                                        return "tra uffici";
                                    case ProtocolTypology.Outgoing:
                                        return "uscita";
                                }
                                return string.Empty;
                        }
                        return string.Empty;
                    }
                case "Container":
                    {
                        switch (formatParameters[1])
                        {
                            case "Id":
                                {
                                    return formatterModel.ContainerId.ToString();
                                }
                            case "Name":
                                {
                                    return formatterModel.ContainerName;
                                }
                            case "Note":
                                {
                                    return formatterModel.ContainerNote;
                                }
                        }
                        return string.Empty;
                    }
                case "Roles":
                    {
                        if (formatterModel.RoleServiceCodes.Any())
                        {
                            return string.Concat("[{0}]", string.Join("-", formatterModel.RoleServiceCodes.Where(f => !string.IsNullOrEmpty(f)).Select(f => f.ToUpper())));
                        }
                        return string.Empty;
                    }
                case "DocumentType":
                    {
                        switch (formatParameters[1])
                        {
                            case "Short":
                                {
                                    switch (formatterModel.DocumentType)
                                    {
                                        case SignatureDocumentType.Main:
                                            return "P";
                                        case SignatureDocumentType.Attachment:
                                            return "A";
                                        case SignatureDocumentType.Annexed:
                                            return "X";
                                    }
                                    return string.Empty;
                                }

                            case "Long":
                                {
                                    switch (formatterModel.DocumentType)
                                    {
                                        case SignatureDocumentType.Main:
                                            return "Protocollo";
                                        case SignatureDocumentType.Attachment:
                                            return "Allegato";
                                        case SignatureDocumentType.Annexed:
                                            return "Annesso";
                                    }
                                    return string.Empty;
                                }
                        }
                        return string.Empty;
                    }
                case "AttachmentsCount":
                    {
                        if (formatterModel.AttachmentsCount.HasValue)
                        {
                            return formatterModel.AttachmentsCount.ToString();
                        }
                        return string.Empty;
                    }
                case "DocumentNumber":
                    {
                        if (formatterModel.DocumentNumber.HasValue)
                        {
                            return formatterModel.DocumentNumber.ToString();
                        }
                        return string.Empty;
                    }
            }
            return string.Empty;
        }

        public object GetFormat(Type formatType)
        {
            return this;
        }
    }
}
