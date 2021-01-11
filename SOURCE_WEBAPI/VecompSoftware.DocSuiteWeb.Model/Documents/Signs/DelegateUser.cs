using System;

namespace VecompSoftware.DocSuiteWeb.Model.Documents.Signs
{
    public class DelegateUser
    {
        /// <summary>
        /// Account delegato (destinatario della delega di firma, ovvero che può usare le credenziali di firma)
        /// </summary>
        public string Account { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// Account dell'utente che ha delegato il profilo di firma
        /// </summary>
        public string DelegatedByUser { get; set; }
        public DateTimeOffset DelegateDate { get; set; }
        public DateTimeOffset? ExpiredDelegateDate { get; set; }

    }
}
