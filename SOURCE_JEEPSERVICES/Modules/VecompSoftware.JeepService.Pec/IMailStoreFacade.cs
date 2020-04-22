using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.JeepService.Pec.IterationTrackerFiles;
using VecompSoftware.MailManager;


namespace VecompSoftware.JeepService.Pec
{
  public interface IMailStoreFacade
  {
    PECMailBox GetMailBox(short idBox);
    PECMail GetMail(int idMail);
    bool HeaderHashExists(string hash, string boxRecipient);
    bool ArchiveMail(string dropFolder, string mailInfoFilename, bool debugMode, bool saveSegnatura, out PECMail lastPecMail, out string errMsg, IterationDescriptor iterationInfo);
  }
}
