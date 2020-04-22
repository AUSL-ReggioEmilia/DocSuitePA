using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;

namespace VecompSoftware.DocSuiteWeb.Model.ExternalViewer
{
    public class ExternalViewerModel : IContentBase
    {
        #region [Constructor]

        public ExternalViewerModel()
        {

        }

        #endregion

        #region [Properties]

        /// <summary>
        /// Unique identifier della UD
        /// </summary>
        public Guid UniqueId { get; set; }

        /// <summary>
        /// Anno della UD
        /// </summary>
        public short Year { get; set; }

        /// <summary>
        /// Numero della UD
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Oggetto del messaggio
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Corpo del messaggio
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Link (External Viewer) della UD
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Indicazione del mittente
        /// </summary>
        public ExternalViewerContactModel Sender { get; set; }

        /// <summary>
        /// Indicazione dei destinatari
        /// </summary>
        public ICollection<ExternalViewerContactModel> Recipients { get; set; }

        /// <summary>
        /// Indicazione dell'utente di registrazione
        /// </summary>
        public string RegistrationUser { get; set; }

        /// <summary>
        /// Indicazione della data di registrazione
        /// </summary>
        public DateTimeOffset? RegistrationDate { get; set; }

        #endregion
    }
}
