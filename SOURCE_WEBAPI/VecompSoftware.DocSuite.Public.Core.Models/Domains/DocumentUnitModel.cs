using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DocumentUnitModel : DomainModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="year"></param>
        /// <param name="number"></param>
        /// <param name="subject"></param>
        /// <param name="category"></param>
        /// <param name="container"></param>
        /// <param name="location"></param>
        public DocumentUnitModel(Guid id, short year, long number, string subject,
            CategoryModel category, ContainerModel container, string location) : base(id)
        {
            DocumentUnitAttribute documentUnitAttribute = Attribute.GetCustomAttribute(GetType(), typeof(DocumentUnitAttribute)) as DocumentUnitAttribute;
            if (documentUnitAttribute == null)
            {
                throw new ArgumentNullException("Unità documentaria definita senza gli attributi di sviluppo. Anomalia critica.");
            }
            if (category == null)
            {
                throw new ArgumentNullException("Unità documentaria non può essere inizializzata senza un classificatore.");
            }
            if (container == null)
            {
                throw new ArgumentNullException("Unità documentaria non può essere inizializzata senza un contenitore archivistico.");
            }
            Category = category;
            Container = container;
            DocumentUnitType = documentUnitAttribute.DocumentUnitType;
            Year = year;
            Number = number;
            DocumentUnitName = documentUnitAttribute.DocumentUnitName;
            Title = string.Concat(year.ToString("0000"), "/", number.ToString("0000000"));
            Subject = subject;
            Location = location;
            Name = string.Concat(DocumentUnitName, " ", Title);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Anno dell'unità documentaria
        /// </summary>
        public short Year { get; set; }

        /// <summary>
        /// Numero dell'unità documentaria
        /// </summary>
        public long Number { get; set; }

        /// <summary>
        /// Nome distingubile dell'unità documentaria
        /// </summary>
        public string DocumentUnitName { get; set; }

        /// <summary>
        /// Rappresentazione standard in DocSuite dell'unità documentaria. Può essere utile per pulsanti di visualizzazione
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Oggetto dell'unità documentaria
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Archivio fisico dei documenti
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Classificatore o Titolario dell'unità documentaria
        /// </summary>
        public CategoryModel Category { get; set; }

        /// <summary>
        /// Tipologia dell'unità documentaria
        /// </summary>
        public DocumentUnitType DocumentUnitType { get; set; }

        /// <summary>
        /// Contenitore dell'unità documentaria
        /// </summary>
        public ContainerModel Container { get; set; }

        /// <summary>
        /// Utente creatore dell'unità documentaria
        /// </summary>
        public DateTimeOffset RegistrationDate { get; set; }

        /// <summary>
        /// Data di creazione dell'unità documentaria
        /// </summary>
        public string RegistrationUser { get; set; }

        /// <summary>
        /// Ultimo utente che ha modificato l'unità documentaria
        /// </summary>
        public DateTimeOffset? LastChangedDate { get; set; }


        /// <summary>
        /// Ultima data di modifica dell'unità documentaria
        /// </summary>
        public string LastChangedUser { get; set; }


        #endregion
    }
}
