using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.MailManager
{
  public interface IMailStoreFacade
  {
    List<PECMailBox> GetMailBoxes();
    PECMailBox GetMailBox(short idBox);
    PECMail GetMail(int idMail);
    bool HeaderHashExists(string hash, string boxRecipient);
    bool ArchiveMail(string dropFolder, string mailInfoFilename, bool debugMode, bool isDsw7Legacy, bool isDsw7Legacy729, bool saveSegnatura, out PECMail lastPecMail, out string errMsg);
  }
}
