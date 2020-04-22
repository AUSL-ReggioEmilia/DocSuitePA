using System;
using System.Collections.Generic;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Data.Entity.PECMails;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Messages
{
    public class SMSNotificationDao : BaseNHibernateDao<SMSNotification>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public SMSNotificationDao() : base()
        {
        }

        public SMSNotificationDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }
        #endregion        

        #region [ Methods ]

        public ICollection<SMSNotification> GetByTipology(Int16 IdPECMailBox = Int16.MinValue, SMSNotificationType notificationType = SMSNotificationType.PEC, LogicalStateType logicalState = LogicalStateType.Active)
        {
            PECMail pec  = null;
            PECMailBox mailBox = null;
            IQueryOver<SMSNotification, SMSNotification> expression = NHibernateSession.QueryOver<SMSNotification>();

            if (IdPECMailBox != Int16.MinValue)
            {
                expression = expression
                    .JoinAlias(x => x.PECMail, () => pec)
                    .JoinAlias(x => x.PECMail.MailBox, () => mailBox)
                    .Where(() => mailBox.Id == IdPECMailBox);
            }
             return expression
                 .Where(f => f.NotificationType == notificationType)
                 .And((f) => f.LogicalState == logicalState)
                 .List<SMSNotification>();
        }

        public Boolean ExistNotification(int idPECMail, String accountName, SMSNotificationType notificationType = SMSNotificationType.PEC)
        {
            PECMail pec = null;
            return NHibernateSession.QueryOver<SMSNotification>()
                .JoinAlias(j => j.PECMail, () => pec)
                .Where(f => f.AccountName == accountName)
                .And((f) => pec.Id == idPECMail)
                .SingleOrDefault() != null;
        }

        #endregion
    }
}
