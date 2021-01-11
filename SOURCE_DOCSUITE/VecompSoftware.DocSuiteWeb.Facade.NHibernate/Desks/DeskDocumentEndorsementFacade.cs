using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
{
    public class DeskDocumentEndorsementFacade : BaseProtocolFacade<DeskDocumentEndorsement, Guid, DeskDocumentEndorsementDao>
    {
        #region [ Fields ]
        private DeskDocumentFacade _deskDocumentFacade;
        private DeskStoryBoardFacade _deskStoryBoardFacade;
        private DeskLogFacade _deskLogFacade;
        private DeskDocumentVersionFacade _deskDocumentVersionFacade;
        private const string APPROVE_COMMENT_FORMAT = "{0} approva il contenuto, {1:HH:mm} del {1:dd/MM/yyyy}";
        private const string REFUSE_COMMENT_FORMAT = "{0} rifiuta il contenuto, {1:HH:mm} del {1:dd/MM/yyyy}";
        private readonly string _userName;

        #endregion [ Fields ]

        #region [ Properties ]
        private DeskDocumentFacade CurrentDeskDocumentFacade
        {
            get { return _deskDocumentFacade ?? (_deskDocumentFacade = new DeskDocumentFacade(_userName)); }
        }

        private DeskStoryBoardFacade CurrentDeskStoryBoardFacade
        {
            get { return _deskStoryBoardFacade ?? (_deskStoryBoardFacade = new DeskStoryBoardFacade(_userName)); }
        }

        private DeskDocumentVersionFacade CurrentDeskDocumentVersionFacade
        {
            get { return _deskDocumentVersionFacade ?? (_deskDocumentVersionFacade = new DeskDocumentVersionFacade(_userName)); }
        }

        private DeskLogFacade CurrentDeskLogFacade
        {
            get { return _deskLogFacade ?? (_deskLogFacade = new DeskLogFacade(_userName)); }
        }
        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskDocumentEndorsementFacade(string userName) : base()
        {
            _userName = userName;
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        /// <summary>
        /// Inserisce una nuova approvazione per il documento specificato per l'ultimo versioning presente
        /// </summary>
        public void AddApprove(Guid idDeskDocument, DeskRoleUser user)
        {
            decimal lastVersion = CurrentDeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(idDeskDocument).Version.Value;
            AddApproveOrRefuse(idDeskDocument, lastVersion, user, true);
        }

        /// <summary>
        /// Inserisce una nuova approvazione o un nuovo rifiuto per il documento specificato
        /// </summary>
        /// <param name="idDeskDocument"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="approve">Se true approva il documento, se false lo rifiuta</param>
        public void AddApproveOrRefuse(Guid idDeskDocument, decimal version, DeskRoleUser user, bool approve)
        {
            DeskDocument doc = CurrentDeskDocumentFacade.GetById(idDeskDocument);
            DeskDocumentEndorsement endorsment = new DeskDocumentEndorsement();

            if (!doc.DeskDocumentVersions.Any(x => x.Version.Equals(version)))
                throw new Exception("Nessuna versione presente per quanto specificato");

            DeskDocumentVersion docVersion = doc.DeskDocumentVersions.Single(x => x.Version.Equals(version));
            endorsment.DeskDocumentVersion = docVersion;
            endorsment.DeskRoleUser = user;
            endorsment.Endorsement = approve;

            //Aggiungo il commento relativo all'approvazione
            string authorName = CommonUtil.GetInstance().UserDescription;
            DateTime approveDate = DateTime.Now;
            string comment = string.Empty;
            if(approve)
                comment = string.Format(APPROVE_COMMENT_FORMAT, authorName, approveDate);
            else
                comment = string.Format(REFUSE_COMMENT_FORMAT, authorName, approveDate);

            Save(ref endorsment);

            CurrentDeskStoryBoardFacade.AddCommentToStoryBoard(comment, authorName, approveDate, doc.Desk, user, docVersion);
            //Inserisco il log di approvazione
            CurrentDeskLogFacade.InsertLog(DeskLogType.Approve, comment, doc.Desk, SeverityLog.Info);
        }

        /// <summary>
        /// Inserisce un nuovo rifiuto per il documento specificato per l'ultimo versioning presente
        /// </summary>
        public void AddRefuse(Guid idDeskDocument, DeskRoleUser user)
        {
            decimal lastVersion = CurrentDeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(idDeskDocument).Version.Value;
            AddApproveOrRefuse(idDeskDocument, lastVersion, user, false);
        }
               
        public bool IsAlreadyApproved(Guid idDeskDocument, string accountName)
        {
            decimal lastVersion = CurrentDeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(idDeskDocument).Version.Value;
            return IsAlreadyApproved(idDeskDocument, lastVersion, accountName);
        }

        public bool IsAlreadyApproved(Guid idDeskDocument, decimal version, string accountName)
        {
            DeskDocumentVersion docVersion = CurrentDeskDocumentVersionFacade.GetByIdAndVersion(idDeskDocument, version);
            if(docVersion == null)
                return false;

            return !_dao.GetByIdVersionAndAccount(docVersion.Id, accountName).Any();
        }
        #endregion [ Methods ]
    }
}
