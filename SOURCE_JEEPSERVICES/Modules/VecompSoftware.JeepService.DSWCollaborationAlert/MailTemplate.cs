using System;
using VecompSoftware.DocSuiteWeb.API;

namespace VecompSoftware.JeepService.DSWCollaborationAlert
{
    public class MailTemplateDetails
    {
        private DSWCollaborationAlertParameters _parameters;
        public MailTemplateDetails(CollaborationDTO dto, DSWCollaborationAlertParameters parameters)
        {
            this.IdCollaborazione = dto.Id;
            this.RegistrationDate = dto.RegistrationDate;
            this.MemorandumDate = dto.MemorandumDate;
            this.AlertDate = dto.AlertDate;
            this.Object = dto.CollaborationObject;
            this.Note = dto.Note;
            this.MailStatus = dto.IdStatus;
            this._parameters = parameters;
        }

        public int? IdCollaborazione { get; set; }
        public DateTime? MemorandumDate { get; set; }
        public DateTime? AlertDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Object { get; set; }
        public string Note { get; set; }
        public string MailStatus { get; set; }
        public string Link
        {
            get
            {
                return string.Format("<a href='{0}?Tipo=Coll&Azione=CS&Identificativo={1}&Stato=CS'>Collaborazione n. {1} del {2}</a>", 
                    _parameters.DswPath, 
                    IdCollaborazione, 
                    RegistrationDate != null ? RegistrationDate.Value.ToString("dd/MM/yyyy") : string.Empty);
            }
        }
    }

    public enum MailEnumFormat
    {
        IdCollaborazione,
        DataScadenza,
        DataAvviso,
        DataRegistrazione,
        Oggetto,
        Note,
        Link
    }

    public class MailTemplateFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return this;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var mailStructure = arg as MailTemplateDetails;
            if (mailStructure == null)
                return null;

            if (!Enum.IsDefined(typeof(MailEnumFormat), format))
                return null;

            var eValue = (MailEnumFormat)Enum.Parse(typeof(MailEnumFormat), format);

            switch (eValue)
            {
                case MailEnumFormat.IdCollaborazione:
                    return mailStructure.IdCollaborazione.ToString();
                case MailEnumFormat.DataScadenza:
                    if (mailStructure.MemorandumDate != null)
                        return mailStructure.MemorandumDate.Value.ToString("dd/MM/yyyy");
                    return string.Empty;
                case MailEnumFormat.DataAvviso:
                    if (mailStructure.AlertDate != null) 
                        return mailStructure.AlertDate.Value.ToString("dd/MM/yyyy");
                    return string.Empty;
                case MailEnumFormat.DataRegistrazione:
                    if (mailStructure.RegistrationDate != null)
                        return mailStructure.RegistrationDate.Value.ToString("dd/MM/yyyy");
                    return string.Empty;
                case MailEnumFormat.Oggetto:
                    return mailStructure.Object;
                case MailEnumFormat.Note:
                    return mailStructure.Note;
                case MailEnumFormat.Link:
                    return mailStructure.Link;
                default:
                    return string.Empty;
            }
        }
    }
}
