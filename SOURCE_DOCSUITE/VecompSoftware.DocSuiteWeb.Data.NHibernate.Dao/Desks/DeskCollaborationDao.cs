using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks
{
    public class DeskCollaborationDao : BaseNHibernateDao<DeskCollaboration>
    {
        #region [ Fields ]
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskCollaborationDao() : base()
        { }

        public DeskCollaborationDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        /// <summary>
        /// Restituisce l'unica collaborazione associata al tavolo.
        /// La collaborazione restituita sarà unica e non si troverà nello stato "Chiusa".
        /// </summary>
        /// <param name="idDesk"></param>
        /// <returns></returns>
        public Collaboration GetCollaborationByIdDesk(Guid idDesk)
        {
            Collaboration collaboration = null;
            Desk desk = null;

            Collaboration result = NHibernateSession.QueryOver<DeskCollaboration>()
                                                    .JoinAlias(x => x.Collaboration, () => collaboration)
                                                    .JoinAlias(x => x.Desk, () => desk)
                                                    .Where(() => desk.Id == idDesk)
                                                    .And(() => collaboration.IdStatus != CollaborationMainAction.AnnullataRitornoAlTavolo)
                                                    .Select(s => s.Collaboration)
                                                    .SingleOrDefault<Collaboration>();
            return result;
        }

        /// <summary>
        /// Restituisce tutte le collaborazioni associate a un tavolo
        /// </summary>
        /// <param name="idDesk"></param>
        /// <returns></returns>
        public ICollection<Collaboration> GetCollaborationsByIdDesk(Guid idDesk)
        {
            Collaboration collaboration = null;
            Desk desk = null;

            ICollection<Collaboration> result = NHibernateSession.QueryOver<DeskCollaboration>()
                                                    .JoinAlias(x => x.Collaboration, () => collaboration)
                                                    .JoinAlias(x => x.Desk, () => desk)
                                                    .Where(() => desk.Id == idDesk)
                                                    .Select(s => s.Collaboration)
                                                    .List<Collaboration>();
            return result;
        }

        public Desk GetDeskByIdCollaboration(int idCollaboration)
        {
            Collaboration collaboration = null;
            Desk desk = null;

            Desk result = NHibernateSession.QueryOver<DeskCollaboration>()
                                           .JoinAlias(x => x.Collaboration, () => collaboration)
                                           .JoinAlias(x => x.Desk, () => desk)
                                           .Where(() => collaboration.Id == idCollaboration)
                                           .Select(s => s.Desk)
                                           .SingleOrDefault<Desk>();
            return result;
        }

        public bool IsDocumentJustInCollaboration(Guid idDesk, string docName)
        {
   
            CollaborationVersioning collaborationVersioning = null;
            Collaboration collaboration = null;
            
            bool result = NHibernateSession.QueryOver<DeskCollaboration>()
                                                     .JoinAlias(x => x.Collaboration, () => collaboration)
                                                     .JoinAlias(x => collaboration.CollaborationVersioning, () => collaborationVersioning)
                                                     .Where((x) => x.Desk.Id == idDesk && collaborationVersioning.DocumentName == docName)
                                                     .RowCount() > 0;


            return result;
        }


        #endregion [ Methods ]
    }
}
