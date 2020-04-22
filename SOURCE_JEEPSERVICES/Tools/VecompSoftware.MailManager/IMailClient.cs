using Limilabs.Client.IMAP;
using System;
namespace VecompSoftware.MailManager
{
    public interface IMailClient
    {
        void DeleteMail(long uid);
        void DeleteMail(String uid);
        int GetMails(short idBox, bool IsProtocolBox, string boxRecipient, Func<string, string, bool> headerExistHandler, string defaultSubject);
        // Watcher
        int GetWatcherMails(short idBox, Boolean IsProtocolBox, string boxRecipient, Func<string, string, bool> headerExistHandler, string defaultSubject, DateTime? endDate);

    }
}
