using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.PECMails
{
    public static class PECMailFinder
    {
        public static IQueryable<PECMail> GetByProtocol(this IRepository<PECMail> repository, Guid uniqueId, PECMailDirection direction)
        {
            return repository.Query(p => p.DocumentUnit.UniqueId == uniqueId && p.Direction == direction && p.DocumentUnit.Environment == (int)DSWEnvironmentType.Protocol)
                .Include(i => i.PECMailReceipts)
                .Include(i => i.PECMailChildrenReceipts)
                .SelectAsQueryable();
        }

        public static int CountOutgoing(this IRepository<PECMail> repository, Guid uniqueId)
        {
            return repository.Queryable(true).Count(p => p.DocumentUnit.UniqueId == uniqueId &&
                                                         p.Direction == PECMailDirection.Outgoing &&
                                                         p.DocumentUnit.Environment == (int)DSWEnvironmentType.Protocol);
        }

        public static int CountIncoming(this IRepository<PECMail> repository, Guid uniqueId)
        {
            return repository.Queryable(true).Count(p => p.DocumentUnit.UniqueId == uniqueId &&
                                                         p.Direction == PECMailDirection.Incoming &&
                                                         p.DocumentUnit.Environment == (int)DSWEnvironmentType.Protocol);
        }

        public static IQueryable<PECMail> GetByUniqueId(this IRepository<PECMail> repository, Guid uniqueId)
        {
            return repository.Query(x => x.UniqueId == uniqueId)
                .SelectAsQueryable();
        }

        public static IQueryable<PECMailAttachment> GetAttachments(this IRepository<PECMailAttachment> repository, int id)
        {
            return repository.Query(x => x.PECMail.EntityId == id).SelectAsQueryable();
        }
    }
}
