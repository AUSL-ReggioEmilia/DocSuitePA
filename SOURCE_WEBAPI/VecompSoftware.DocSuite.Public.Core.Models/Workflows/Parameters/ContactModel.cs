using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello Standard del Contatto Anagrafico
    /// </summary>
    public sealed class ContactModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello Standard del Contatto Anagrafico
        /// </summary>
        /// <param name="contactType">Tipologia del contatto <see cref="ContactType"/></param>
        /// <param name="contactDirectionType">Tipologia mittente/destinatario del contatto <see cref="ContactDirectionType"/></param>
        /// <param name="archiveSection">Se l'unità documentaria è un archivio, è necessario specificare la sezione del contatto</param>
        /// <param name="contactId">Codice univoco del contatto anagrafico della DocSuite</param>
        /// <param name="mappingTag">Codice di mapping concordato con Dgroove</param>
        public ContactModel(ContactType contactType, ContactDirectionType? contactDirectionType = null,
            string archiveSection = "", Guid? contactId = null, string mappingTag = "")
        {
            ContactId = contactId;
            ArchiveSection = archiveSection;
            ContactType = contactType;
            ContactDirectionType = contactDirectionType;
            MappingTag = mappingTag;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Codice univoco del contatto anagrafico della DocSuite
        /// </summary>
        public Guid? ContactId { get; private set; }

        /// <summary>
        /// Se l'unità documentaria è un archivio, è necessario specificare la sezione del contatto
        /// </summary>
        public string ArchiveSection { get; private set; }

        /// <summary>
        /// Denominazione
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data di nascita
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Luogo di nascita
        /// </summary>
        public string BirthPlace { get; set; }

        /// <summary>
        /// Codice esterno del contatto
        /// </summary>
        public string ExternalCode { get; set; }

        /// <summary>
        /// Indirizzo email
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Indirizzo PEC
        /// </summary>
        public string PECAddress { get; set; }

        /// <summary>
        /// Numero di telefono mobile
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Via
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Città
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Sigla città
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// Numero civico
        /// </summary>
        public string CivicNumber { get; set; }

        /// <summary>
        /// CAP
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Codice Fiscale
        /// </summary>
        public string FiscalCode { get; set; }

        /// <summary>
        /// Partita Iva o Tax Code
        /// </summary>
        public string FiscalTaxCode { get; set; }

        /// <summary>
        /// Numero di telefono 
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Numero di FAX
        /// </summary>
        public string FaxNumber { get; set; }

        /// <summary>
        /// Nazionalità (descrizione)
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Lingua del contatto (codice es: IT, DE, UK, US ecc....)
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Tipologia del contatto <see cref="ContactType"/>
        /// </summary>
        public ContactType ContactType { get; set; }

        /// <summary>
        /// Tipologia mittente/destinatario del contatto <see cref="ContactDirectionType"/>
        /// </summary>
        public ContactDirectionType? ContactDirectionType { get; set; }

        /// <summary>
        /// Campo note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Codice di mapping concordato con Dgroove
        /// </summary>
        public string MappingTag { get; set; }

        #endregion
    }
}
