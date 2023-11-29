using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Biblos;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
{
    public class DeskFacade : BaseProtocolFacade<Desk, Guid, DeskDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskFacade(string userName) : base()
        {
            _userName = userName;
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public bool CheckRight(Desk item, string[] userGroups, DeskRightPositions rightsPosition)
        {
            if(item.Container.ContainerGroups.IsNullOrEmpty())
                return false;


            IEnumerable<ContainerGroup> containerGroupHasDeskRights = item.Container.ContainerGroups
                .Where(x => x.DeskRights != null && x.DeskRights.ToCharArray()
                        .ElementAt(Convert.ToInt32(rightsPosition))
                        .TryConvert<bool>())
                .Select(s => s);

            return containerGroupHasDeskRights.Any(x => userGroups.Any(xu => xu.Eq(x.Name)));
        }

        public bool CheckSecurityGroupsRight(Desk item, IList<SecurityGroups> securityGroups, DeskRightPositions rightsPosition)
        {
            if(item.Container.ContainerGroups.IsNullOrEmpty())
                return false;

            IEnumerable<ContainerGroup> containerGroupHasDeskRights = item.Container.ContainerGroups
                .Where(x => x.DeskRights.ToCharArray()
                .ElementAt(Convert.ToInt32(rightsPosition))
                .TryConvert<bool>())
                .Select(s => s);

            return containerGroupHasDeskRights.Any(x => securityGroups.Any(xu => xu.Id.Equals(x.SecurityGroup.Id)));
        }

        public Desk GetByIdCollaboration(int idCollaboration)
        {
            return _dao.GetByIdCollaboration(idCollaboration);
        }

        /// <summary>
        /// Inserisco un dizionario di documenti Biblos NON firmati sul tavolo.
        /// </summary>
        /// <param name="docCollection"></param>
        /// <param name="currentDesk"></param>
        /// <param name="currentDeskLocation"></param>
        /// <returns></returns>
        public Desk InsertDocumentNotSignedFromCollaboration(IDictionary<Guid, BiblosDocumentInfo> docCollection, Desk currentDesk, Location currentDeskLocation = null)
        {
            foreach (KeyValuePair<Guid, BiblosDocumentInfo> document in docCollection)
            {
                if (!document.Value.IsSigned)
                {
                    DeskDocument deskDocument = new DeskDocument(_userName);
                    Guid chain = Guid.Empty;
                    chain = DocumentInfoFactory.ArchiveDocumentsInBiblos(new List<DocumentInfo>() { document.Value }, currentDeskLocation.ProtBiblosDSDB, chain);

                    deskDocument.DocumentType = DeskDocumentType.MainDocument;
                    deskDocument.IdDocument = chain;
                    deskDocument.Desk = currentDesk;
                    deskDocument.IsActive = true;

                    DeskDocumentVersion version = new DeskDocumentVersion(_userName);
                    version.DeskDocument = deskDocument;
                    version.Version = 1;

                    deskDocument.DeskDocumentVersions.Add(version);

                    currentDesk.DeskDocuments.Add(deskDocument);
                }
            }
            return currentDesk;
        }

        /// <summary>
        /// Override per il salvataggio di un nuovo tavolo con la specifica dello status
        /// </summary>
        /// <param name="obj"></param>
        public override void Save(ref Desk obj)
        {
            obj.Status = DeskState.Open;
            base.Save(ref obj);
        }
        #endregion [ Methods ]
    }
}
