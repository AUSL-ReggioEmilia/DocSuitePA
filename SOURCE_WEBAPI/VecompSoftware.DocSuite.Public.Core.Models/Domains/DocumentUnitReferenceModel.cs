using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentUnitReferenceModel : DomainModel
    {
        /// <summary>
        /// DocumentUnitReferenceModel
        /// </summary>
        /// <param name="referenceId">identificativo univoco</param>
        /// <param name="name">nome</param>
        /// <param name="subject">oggetto</param>
        /// <param name="year">anno</param>
        /// <param name="number">numero in formato stringa per compatibilità con gli atti</param>
        public DocumentUnitReferenceModel(Guid referenceId, string name, string subject, short year, string number) : base(referenceId)
        {
            Name = name;
            Subject = subject;
            Year = year;
            Number = number;
        }
        /// <summary>
        /// Oggetto della unità documentale
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Anno della unità documentale
        /// </summary>
        public short Year { get; set; }

        /// <summary>
        /// Numero della unità documentale
        /// </summary>
        public string Number { get; set; }
    }
}
