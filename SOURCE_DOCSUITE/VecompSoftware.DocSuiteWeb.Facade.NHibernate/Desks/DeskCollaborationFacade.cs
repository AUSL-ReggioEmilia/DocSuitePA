using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
{
    public class DeskCollaborationFacade : BaseProtocolFacade<DeskCollaboration, Guid, DeskCollaborationDao>
    {
        public DeskCollaborationFacade(string userName)
            : base()
        {

        }

        public void AddDeskCollaboration(Desk desk, Collaboration collaboration)
        {
            if (desk == null || collaboration == null)
                return;

            DeskCollaboration deskCollaboration = new DeskCollaboration() { Desk = desk, Collaboration = collaboration };            
            Save(ref deskCollaboration);
        }

        public Desk GetDeskByIdCollaboration(Collaboration collaboration)
        {
            return _dao.GetDeskByIdCollaboration(collaboration.Id);
        }
        public Desk GetDeskByIdCollaboration(int collaborationId)
        {
            return _dao.GetDeskByIdCollaboration(collaborationId);
        }

        public Collaboration GetActiveCollaborationByIdDesk(Desk desk)
        {
            return _dao.GetCollaborationByIdDesk(desk.Id);
        }

        public bool IsDocumentJustInCollaboration(Desk desk, string docName)
        {
            return _dao.IsDocumentJustInCollaboration(desk.Id, docName);
        }
        public ICollection<Collaboration> GetActiveCollaborationByIdDesk(Guid deskId)
        {
            return _dao.GetCollaborationsByIdDesk(deskId);
        }
    }
}
