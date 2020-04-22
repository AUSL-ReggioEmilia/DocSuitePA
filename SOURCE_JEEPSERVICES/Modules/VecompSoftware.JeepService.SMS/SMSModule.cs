using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Messages;
using VecompSoftware.DocSuiteWeb.DTO.Messages;
using VecompSoftware.DocSuiteWeb.Facade.NHibernate.Messages;
using VecompSoftware.Helpers.SMS;
using VecompSoftware.Helpers.SMS.Entity;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.SMS
{
    public class SMSModule : JeepModuleBase<SMSParameters>
    {
        private static SMSNotificationFacade _smsFacade;
        public static SMSNotificationFacade NotificationFacade
        {
            get { return _smsFacade ?? (_smsFacade = new SMSNotificationFacade(WindowsIdentity.GetCurrent().Name)); }
        }

        public override void SingleWork()
        {
            FileLogger.Info(Name, "Avvio notifiche SMS");

            ICollection<JobSMSNotification> results = NotificationFacade.GetAllActivePecNotificationJob();

            if (results != null)
            {
                FileLogger.Info(Name, String.Format("Trovate {0} notifice pec da inviare", results.Count));
                String partialText = String.Empty;
                SMSReceipt result = null;
                SMSNotification smsNotification;
                foreach (JobSMSNotification notification in results)
                {
                    try 
	                {	        
                        result = null;
                        partialText = String.Empty;
                        partialText = String.Format(Parameters.GlobalSMSText, 
                            String.Concat(DateTime.Now.ToShortDateString(), " ", DateTime.Now.ToShortTimeString()),
                            String.Join<String>(", ", notification.Reports
                                .ToLookup(f => f.MailBoxName, f => f.PECMailId)
                                .Select(f => String.Format(Parameters.JoinStringText, f.Key.Length > Parameters.TakeMailBoxName ? new string(f.Key.Take(Parameters.TakeMailBoxName).ToArray()) : f.Key))));

                        result = SMSSender.Send(new String[] { String.Concat(Parameters.InternationalPrefixNumber, notification.MobileNumber) }, partialText);

                        if (!result.Success)
                        {
                            throw new Exception(String.Concat(result.ID, " : ", result.ErrorMessage));
                        }
                        foreach (var report in notification.Reports)
                        {
                            smsNotification = _smsFacade.GetById(report.SMSNotificationId);
                            smsNotification.LogicalState = LogicalStateType.Successful;
                            _smsFacade.UpdateOnly(ref smsNotification);
                        }
                        FileLogger.Info(Name, String.Format("Invio SMS {0} effettuato correttamente a {1}.", 
                            partialText, notification.MobileNumber));
	                }
	                catch (Exception ex)
	                {
                        FileLogger.Error(Name, String.Concat(Parameters.GlobalSMSText, " ", Parameters.JoinStringText));
                        if (result == null || (result != null && !result.Success))
                        {
                            FileLogger.Error(Name, String.Format("Errore {0} in fase di invio SMS {1} a {2}", 
                                ex.Message, partialText, notification.MobileNumber), ex);
                        }
                        else
                        {
                            FileLogger.Error(Name, String.Format("Invio SMS {0} effettuato correttamente a {1}, ma con errore '{2}' in fase di salvataggio degli stati delle notifiche nel database", 
                                partialText, notification.MobileNumber, ex.Message), ex);
                        }
	                }
                }
            }
        }
    }
}
