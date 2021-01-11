using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
{
    public class DeskDocumentVersionFacade : BaseProtocolFacade<DeskDocumentVersion, Guid, DeskDocumentVersionDao>
    {
        #region [ Fields ]
        private DeskDocumentFacade _deskDocumentFacade;
        private readonly String _userName;
        #endregion

        #region [ Properties ]
        private DeskDocumentFacade CurrentDeskDocumentFacade
        {
            get { return _deskDocumentFacade ?? (_deskDocumentFacade = new DeskDocumentFacade(_userName)); }
        }
        #endregion

        #region [ Constructor ]
        public DeskDocumentVersionFacade(string userName) : base()
        {
            _userName = userName;
        }
        #endregion

        #region [ Methods ]
        public DeskDocumentVersion GetLastVersionByIdDeskDocument(Guid idDeskDocument)
        {
            DeskDocumentVersion lastVersion = _dao.GetLastVersionByIdDeskDocument(idDeskDocument);
            if (lastVersion == null || !lastVersion.Version.HasValue)
            {
                throw new Exception("Nessun versioning presente per il documento specificato");
            }
            return lastVersion;
        }

        public DeskDocumentVersion GetByIdAndVersion(Guid idDocument, decimal version)
        {
            return _dao.GetByIdAndVersion(idDocument, version);
        }
        
        /// <summary>
        /// Aggiunge una nuova versione del documento.
        /// Viene richiamata successivamente al checkin del file.
        /// </summary>
        /// <param name="idDeskDocument"></param>
        /// <param name="idBiblosDocument"></param>
        /// <param name="version"></param>
        public DeskDocumentVersion InsertDocumentVersion(Guid idBiblosDocument, decimal version, DeskDocument deskDocument)
        {
            DeskDocumentVersion documentVersion = new DeskDocumentVersion(_userName)
            {
                DeskDocument = deskDocument,
                Version = version,
            };

            Save(ref documentVersion);
            return documentVersion;
        }
        #endregion
    }
}
