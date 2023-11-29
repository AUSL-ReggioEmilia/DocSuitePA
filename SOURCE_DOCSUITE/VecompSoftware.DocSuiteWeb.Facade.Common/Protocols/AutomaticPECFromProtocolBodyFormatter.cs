using System;
using System.Configuration;
using System.Globalization;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Protocols
{
    public class AutomaticPECFromProtocolBodyFormatter
        : ICustomFormatter, IFormatProvider
    {
        #region [ Fields ]
        private const string PROTOCOL_SUBJECT = "subject";
        private const string PROTOCOL_NUMBER = "number";
        private const string PROTOCOL_YEAR = "year";
        private const string PROTOCOL_REGISTRATIONDATE = "registrationDate";
        private const string PEC_MAILDATE = "pecMailDate";
        private const string PEC_FROMDESCRIPTION = "fromPecDescription";

        private static readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        #endregion

        #region [ Methods ]
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(format))
            {
                return string.Empty;
            }

            if (!(arg is Tuple<Protocol, PECMail>))
            {
                return string.Empty;
            }

            string[] formatParameters = format.Split(new[] { ':' }, 2);
            Protocol protocol = (arg as Tuple<Protocol, PECMail>).Item1;
            PECMail pecMail = (arg as Tuple<Protocol, PECMail>).Item2;
            if (protocol != null)
            {
                switch (formatParameters[0])
                {
                    case PROTOCOL_SUBJECT:
                        {
                            return protocol.ProtocolObject;
                        }
                    case PROTOCOL_NUMBER:
                        {
                            if (formatParameters.Length > 1)
                            {
                                return protocol.Number.ToString(formatParameters[1]);
                            }
                            return protocol.Number.ToString();
                        }
                    case PROTOCOL_YEAR:
                        {
                            if (formatParameters.Length > 1)
                            {
                                return protocol.Year.ToString(formatParameters[1]);
                            }
                            return protocol.Year.ToString();
                        }
                    case PROTOCOL_REGISTRATIONDATE:
                        {
                            DateTimeOffset localRegistrationDate = TimeZoneInfo.ConvertTimeFromUtc(protocol.RegistrationDate.DateTime, _timeZone);
                            if (formatParameters.Length > 1)
                            {
                                return localRegistrationDate.ToString(formatParameters[1]);
                            }
                            return localRegistrationDate.ToString();
                        }
                    case PEC_FROMDESCRIPTION:
                        {
                            if (pecMail != null)
                            {
                                return formatParameters[1];
                            }
                            return string.Empty;
                        }
                    case PEC_MAILDATE:
                        {
                            if (pecMail != null)
                            {
                                if (formatParameters.Length > 1)
                                {
                                    return pecMail.MailDate?.ToString(formatParameters[1]);
                                }
                                return pecMail.MailDate?.ToString();
                            }
                            return string.Empty;
                        }
                }
            }

            return arg.ToString();
        }

        public object GetFormat(Type formatType)
        {
            return this;
        }
        #endregion
    }
}
