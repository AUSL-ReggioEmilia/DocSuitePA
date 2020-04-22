using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Messages;
using VecompSoftware.DocSuiteWeb.DTO.Messages;
using VecompSoftware.Helpers;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Messages
{
    public class SMSNotificationFacade : BaseProtocolFacade<SMSNotification, Guid, SMSNotificationDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public SMSNotificationFacade(string userName)
            : base()
        {
            _userName = userName;
        }
        #endregion        

        #region [ Methods ]
        public ICollection<SMSNotification> GetAllActivePECNotifications()
        {
            return _dao.GetByTipology(notificationType: SMSNotificationType.PEC, logicalState: LogicalStateType.Active);
        }

        private Func<UserLog, String> _getMobilePhoneLambda = (UserLog userLog) => userLog.MobilePhone;
        public ICollection<JobSMSNotification> GetAllActivePecNotificationJob()
        {

            ICollection<SMSNotification> result = GetAllActivePECNotifications();
            ICollection<JobSMSNotification> structures = result
                .ToLookup(f => f.AccountName, f => f)
                .Select(f => new JobSMSNotification() 
                {
                    MobileNumber = LambdaExpressionHelper.GetDefaultValue<UserLog, String>(FacadeFactory.Instance.UserLogFacade.GetByUser(f.Key, String.Empty), _getMobilePhoneLambda), 
                    Reports = f.Select(c => new SMSReport()
                        {
                            SMSNotificationId = c.Id,
                            MailBoxName = c.PECMail.MailBox.MailBoxName,
                            PECMailId = c.PECMail.Id
                        }).ToList()
                }).ToList();
            return structures;
        }

        public Boolean ExistPecNotificationForAccountName(int idPECMail, String accountName)
        {
            return _dao.ExistNotification(idPECMail, accountName, SMSNotificationType.PEC);
        }

        #endregion
    }
}
