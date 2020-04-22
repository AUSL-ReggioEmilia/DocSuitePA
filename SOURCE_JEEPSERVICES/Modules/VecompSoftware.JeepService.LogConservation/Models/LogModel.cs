using System;
using System.Xml.Serialization;

namespace VecompSoftware.JeepService.LogConservation.Models
{
    [Serializable]
    [XmlRoot("Log")]
    public class LogModel
    {
        [XmlElement("DataRegistrazione")]
        public string RegistrationDateToXml
        {
            get
            {
                return RegistrationDate.HasValue ? RegistrationDate.Value.ToString("o") : string.Empty;
            }
            set
            {
                RegistrationDate = DateTimeOffset.Parse(value);
            }
        }

        [XmlIgnore]
        public DateTimeOffset? RegistrationDate { get; set; }

        [XmlElement("UtenteRegistrazione")]
        public string RegistrationUser { get; set; }        

        [XmlElement("TipologiaLog")]
        public string LogType { get; set; }

        [XmlElement("Riferimento")]
        public ReferenceModel ReferenceModel { get; set; }

        [XmlElement("TipoRiferimento")]
        public string EntityName { get; set; }

        [XmlElement("Descrizione")]
        public string Description { get; set; }

        [XmlElement("Hash")]
        public string Hash { get; set; }
    }
}
