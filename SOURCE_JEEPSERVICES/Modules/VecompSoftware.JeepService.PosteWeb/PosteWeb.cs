using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Biblos.DocumentsService;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.PosteWeb
{
    public class PosteWeb : JeepModuleBase<PosteWebParameters>
    {
        #region [ Fields ]

        private readonly Lazy<FacadeFactory> _factory = new Lazy<FacadeFactory>();
        private DocumentsClient _biblosDSClient;
        #endregion

        #region [ Properties ]

        public FacadeFactory Factory
        {
            get { return _factory.Value; }
        }

        protected DocumentsClient BiblosDSClient
        {
            get { return _biblosDSClient ?? (_biblosDSClient = new DocumentsClient()); }
        }

        #endregion

        #region [ Methods ]

        public override void SingleWork()
        {
            FileLogger.Info(Name, "START PosteWeb.");

            IList<POLAccount> accounts = Factory.PosteOnLineAccountFacade.GetAll();
            FileLogger.Info(Name, string.Format("Account trovati [{0}].", accounts.Count));

            var processedCount = 0;
            foreach (var account in accounts)
            {
                try
                {
                    ProcessAccount(account);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    FileLogger.Warn(Name, string.Format("Errore in elaborazione Account [{0}].", account.Name), ex);
                }
            }

            FileLogger.Info(Name, string.Format("Account elaborati [{0}/{1}].", processedCount, accounts.Count));
            FileLogger.Info(Name, "STOP PosteWeb.");
        }


        private void ProcessAccount(POLAccount account)
        {
            FileLogger.Info(Name, string.Format("Elaborazione Account [{0}].", account.Name));

            // Istanzio il web service
            WebServicesWrapper webServices = new WebServicesWrapper(account, Name, Parameters);

            // Processo le raccomandate da spedire
            IList<ROLRequest> raccomandate = Factory.PosteOnLineRequestFacade.GetOngoingRaccomandate(account);
            FileLogger.Info(Name, string.Format("Raccomandate da spedire [{0}].", raccomandate.Count));
            ROLRequest raccomandata;
            POLRequest toSave;
            for (int index = 0; index < raccomandate.Count; index++)
            {
                raccomandata = raccomandate[index];
                try
                {
                    FileLogger.Info(Name, string.Format("Elaborazione raccomandata [{0}].", raccomandata.Id));

                    Factory.TaskHeaderFacade.ActivatePOLTaskProcess(raccomandata);

                    switch (raccomandata.Status)
                    {
                        case POLRequestStatusEnum.RequestQueued:
                            webServices.RaccomandataSendRequest(raccomandata);
                            break;
                        case POLRequestStatusEnum.RequestSent:
                            webServices.RaccomandataCheckRequest(raccomandata);
                            break;
                        case POLRequestStatusEnum.NeedConfirm:
                            webServices.RaccomandataConfirm(raccomandata);
                            break;
                    }

                    raccomandata.LastChangedUser = Tools.ServiceUser;
                    raccomandata.LastChangedDate = DateTime.Now;

                    toSave = raccomandata;
                    Factory.PosteOnLineRequestFacade.Save(ref toSave);

                    FileLogger.Info(Name, string.Format("Raccomandata elaborata correttamente [{0}].", raccomandata.Id));
                }
                catch (Exception ex)
                {
                    FileLogger.Warn(Name, string.Format("Errore in elaborazione raccomandata [{0}].", raccomandata.Id), ex);
                }
            }

            // Controllo lo stato delle raccomandate spedite
            try
            {
                raccomandate = Factory.PosteOnLineRequestFacade.GetConfirmedRaccomandate(account);
                FileLogger.Info(Name, string.Format("Raccomandate di cui aggiornare lo stato [{0}].", raccomandate.Count));
                if (raccomandate.Count > 0)
                {
                    webServices.RaccomandateGetStatus(raccomandate);

                    for (int index = 0; index < raccomandate.Count; index++)
                    {
                        raccomandata = raccomandate[index];

                        raccomandata.LastChangedUser = Tools.ServiceUser;
                        raccomandata.LastChangedDate = DateTime.Now;

                        toSave = raccomandata;
                        Factory.PosteOnLineRequestFacade.Save(ref toSave);

                        if (raccomandata.Status ==  POLRequestStatusEnum.Executed)
                        {
                            webServices.CompleteRequestTask(raccomandata);
                        }

                        FileLogger.Info(Name, string.Format("Raccomandata aggiornata correttamente [{0}].", raccomandata.Id));
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, "Errore in aggiornamento stato raccomandate spedite.", ex);
            }

            // Processo le lettere da spedire
            IList<LOLRequest> lettere = Factory.PosteOnLineRequestFacade.GetOngoingLettere(account);
            FileLogger.Info(Name, string.Format("Lettere da spedire [{0}].", lettere.Count));
            LOLRequest lettera;
            for (int index = 0; index < lettere.Count; index++)
            {
                lettera = lettere[index];
                try
                {
                    FileLogger.Info(Name, string.Format("Elaborazione lettera [{0}].", lettera.Id));

                    Factory.TaskHeaderFacade.ActivatePOLTaskProcess(lettera);

                    switch (lettera.Status)
                    {
                        case POLRequestStatusEnum.RequestQueued:
                            webServices.LetteraSendRequest(lettera);
                            break;
                        case POLRequestStatusEnum.RequestSent:
                            webServices.LetteraCheckRequest(lettera);
                            break;
                        case POLRequestStatusEnum.NeedConfirm:
                            webServices.LetteraConfirm(lettera);
                            break;
                    }

                    lettera.LastChangedUser = Tools.ServiceUser;
                    lettera.LastChangedDate = DateTime.Now;

                    toSave = lettera;
                    Factory.PosteOnLineRequestFacade.Save(ref toSave);

                    if (lettera.Status == POLRequestStatusEnum.Executed)
                    {
                        webServices.CompleteRequestTask(lettera);
                    }

                    FileLogger.Info(Name, string.Format("Lettera elaborata correttamente [{0}].", lettera.Id));
                }
                catch (Exception ex)
                {
                    FileLogger.Warn(Name, string.Format("Errore in elaborazione lettera [{0}].", lettera.Id), ex);
                }
            }

            // Processo i telegrammi da spedire
            IList<TOLRequest> telegrammi = Factory.PosteOnLineRequestFacade.GetOngoingTelegrammi(account);
            FileLogger.Info(Name, string.Format("Telegrammi da spedire [{0}].", lettere.Count));
            TOLRequest telegramma;
            for (int index = 0; index < telegrammi.Count; index++)
            {
                telegramma = telegrammi[index];
                try
                {
                    FileLogger.Info(Name, string.Format("Elaborazione telegramma [{0}].", telegramma.Id));

                    Factory.TaskHeaderFacade.ActivatePOLTaskProcess(telegramma);

                    switch (telegramma.Status)
                    {
                        case POLRequestStatusEnum.RequestQueued:
                            webServices.TelegrammaSend(telegramma, Parameters.MaxAddress);
                            break;
                        case POLRequestStatusEnum.RequestSent:
                            webServices.TelegrammaConfirm(telegramma);
                            break;
                    }

                    telegramma.LastChangedUser = Tools.ServiceUser;
                    telegramma.LastChangedDate = DateTime.Now;

                    toSave = telegramma;
                    Factory.PosteOnLineRequestFacade.Save(ref toSave);

                    if (telegramma.Status == POLRequestStatusEnum.Executed)
                    {
                        webServices.CompleteRequestTask(telegramma);
                    }

                    FileLogger.Info(Name, string.Format("Telegramma elaborato correttamente [{0}].", telegramma.Id));
                }
                catch (Exception ex)
                {
                    FileLogger.Warn(Name, string.Format("Errore in elaborazione telegramma [{0}].", telegramma.Id), ex);
                }
            }
        }
        #endregion

    }
}
