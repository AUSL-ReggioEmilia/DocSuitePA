using System;
using System.Diagnostics;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Services.Sharepoint;

namespace VecompSoftware.DocSuiteWeb.Facade.Common
{
    public class SharePointFacade
    {
        public static void Publish(Resolution resl, DateTime pubblicationStartDate, DateTime pubblicationEndDate, string signature, string xmlDoc, string xmlOther)
        {
            Publish(resl, pubblicationStartDate, pubblicationEndDate, xmlDoc, xmlOther, 0, 0, false);
        }

        public static void Publish(Resolution resl, DateTime pubblicationStartDate, DateTime pubblicationEndDate, string xmlDoc, string xmlOther, int chainId, int chainEnum, bool isPrivacy)
        {
            var obj = resl.ResolutionObjectPrivacy != null ? resl.ResolutionObjectPrivacy.Replace("&", "&amp;") : resl.ResolutionObject.Replace("&", "&amp;");
            var resolutionType = resl.Type.Description;

            // Se non carica il valore, ricarico l'oggetto
            if (string.IsNullOrEmpty(resolutionType))
            {
                var reslTypeFacade = new ResolutionTypeFacade();
                resolutionType = reslTypeFacade.GetById(resl.Type.Id).Description;
            }
            var retVal = new ReturnValue();
            try
            {
                Debug.Assert(resl.AdoptionDate != null, "resl.AdoptionDate != null");
                retVal = ServiceSHP.InsertInPublicationArea(resl.Container.Name, resl.Year, resl.Number.ToString(), resl.AdoptionDate.Value, obj, DateTime.Now,
                                                            pubblicationStartDate, pubblicationEndDate, resolutionType, xmlOther, xmlDoc);
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Errore pubblicazione","Errore Pubblicazione internet: " + retVal.Guid, ex);
            }

            new ResolutionLogFacade().Log(resl, ResolutionLogType.WP, "File pubblicato correttamente");

            try
            {
                resl.WebSPGuid = retVal.Guid;
                resl.WebState = Resolution.WebStateEnum.Published;
                resl.WebPublicationDate = pubblicationStartDate;
                new ResolutionFacade().Update(ref resl);

                // Registro il numero di publicazione ritornato
                // Da verificare non appena il servizio sarà disponibile
                var numeroPubblicazione = string.Empty;
                for (var i = 0; numeroPubblicazione == string.Empty && i < retVal.ExtraMetaDatas.Length; i++)
                {
                    ExtraMetaData data = retVal.ExtraMetaDatas[i];
                    if (data.Name == "NumeroPubblicazione") numeroPubblicazione = data.Value;
                }
                if (numeroPubblicazione != string.Empty)
                {
                    new ResolutionFacade().DoGenericWebPublication(resl, numeroPubblicazione, chainId, chainEnum, isPrivacy);
                }
                else throw new DocSuiteException("Pubblicazione") { Descrizione = string.Format("Errore nella memorizzazione del numero di pubblicazione reslid = {0} Contattare l'amministratore del sistema.", resl.Id), User = DocSuiteContext.Current.User.FullUserName };

            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Pubblicazione", ex) { Descrizione = string.Format("Errore Pubblicazione internet: Impossibile aggiornare i dati sul database. IdResolution = {0}", resl.Id), User = DocSuiteContext.Current.User.FullUserName };
            }
        }

        public static void Retire(Resolution resl, DateTime revokeDate, string signature, string xmlDoc, string xmlOther)
        {
            var obj = resl.ResolutionObjectPrivacy != null ? resl.ResolutionObjectPrivacy.Replace("&", "&amp;") : resl.ResolutionObject.Replace("&", "&amp;");
            var retVal = new ReturnValue();
            try
            {
                Debug.Assert(resl.AdoptionDate != null, "resl.AdoptionDate != null");
                Debug.Assert(resl.WebPublicationDate != null, "resl.WebPublicationDate != null");
                retVal = ServiceSHP.InsertInRetireArea(resl.Container.Name, resl.Year, resl.Number.ToString(), resl.AdoptionDate.Value, obj,
                                                       DateTime.Now, resl.WebPublicationDate.Value, revokeDate, resl.Type.Description, xmlOther, xmlDoc, resl.WebSPGuid);
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Ritiro","Errore Ritiro internet: " + retVal.Guid , ex) { User = DocSuiteContext.Current.User.FullUserName };
            }

            new ResolutionLogFacade().Log(resl, ResolutionLogType.WP, "File ritirato correttamente");

            try
            {
                resl.WebSPGuid = retVal.Guid;
                resl.WebState = Resolution.WebStateEnum.Revoked;
                resl.WebRevokeDate = revokeDate;
                new ResolutionFacade().Update(ref resl);
            }
            catch (Exception ex)
            {
                throw new DocSuiteException("Ritiro", ex) { Descrizione = "Errore Ritiro internet: Impossibile aggiornare i dati sul database", User = DocSuiteContext.Current.User.FullUserName };
            }
        }
    }
}