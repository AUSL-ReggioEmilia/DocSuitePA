using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.ENCO;
using VecompSoftware.Services.Logging;
using VecompSoftware.DocSuiteWeb.AVCP;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.JeepService.AVCP
{
    public class AVCPModule : ModuleBase
    {
        #region [ Fields ]
        private ServiceAVCP _avcpWs;
        
        #endregion

        #region [ Properties ]
        private ServiceAVCP AvcpWs
        {
            get
            {
                if (this._avcpWs != null)
                {
                    return this._avcpWs;
                }
                ServiceAVCP serviceAVCP = new ServiceAVCP()
                {
                    Url = base.Parameters.AVCPServiceUrl
                };
                this._avcpWs = serviceAVCP;
                return this._avcpWs;
            }
        }
        #endregion


        #region [ Override ]
        protected override void OnSingleWork()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (base.Parameters.SendAllCategories)
            {
                stringBuilder.Append(this.SendAllCategories());
                stringBuilder.AppendLine();
            }
            if (base.Parameters.SendNewDocuments)
            {
                stringBuilder.Append(this.SendNewDocuments());
                stringBuilder.AppendLine();
            }
            if (base.Parameters.UpdateDocuments)
            {
                stringBuilder.Append(base.UpdateDocumentsAvcp());
                stringBuilder.AppendLine();
            }
            this.SendMessage(stringBuilder.ToString());
        }

        protected override bool UpdateDocumentItem(DocumentSeriesItem item)
        {
            WSDBPPubblicazioneOutput datiAVCP;
            DateTime minValue;
            bool flag;
            string str = "";
            try
            {
                Dictionary<string, string> attributes = base.Facade.DocumentSeriesItemFacade.GetAttributes(item);
                WSDBPAVCPPubblicazioneInput wSDBPAVCPPubblicazioneInput = new WSDBPAVCPPubblicazioneInput()
                {
                    CodiceServizio = attributes[AttributeCodiceSettore],
                    NumeroAtto = Convert.ToInt32((!string.IsNullOrEmpty(attributes[AttributeNumero]) ? attributes[AttributeNumero] : "0")),
                    AnnoAtto = Convert.ToInt32((!string.IsNullOrEmpty(attributes[AttributeAnno]) ? attributes[AttributeAnno] : "0"))
                };
                WSDBPAVCPPubblicazioneInput wSDBPAVCPPubblicazioneInput1 = wSDBPAVCPPubblicazioneInput;
                string documentTitle = AVCPHelper.GetDocumentTitle(wSDBPAVCPPubblicazioneInput1.AnnoAtto, wSDBPAVCPPubblicazioneInput1.CodiceServizio, wSDBPAVCPPubblicazioneInput1.NumeroAtto);
                FileLogger.Info(base.Name, string.Format("UpdateDocumentItem: {0}", documentTitle));
                try
                {
                    datiAVCP = this.AvcpWs.GetDatiAVCP(wSDBPAVCPPubblicazioneInput1);
                }
                catch (Exception exception)
                {
                    base.OnError("Errore in consultazione servizio.", exception);
                    flag = false;
                    return flag;
                }
                if ((int)datiAVCP.DatiAvcp.data.Length != 0)
                {
                    string aVCPXml = ENCOHelper.GetAVCPXml(datiAVCP.DatiAvcp);
                    FileLogger.Debug("XML_AVCP", item.Id.ToString());
                    FileLogger.Debug("XML_AVCP", aVCPXml);
                    if (!attributes.ContainsKey(AttributeDataUltimoAggiornamento) || !DateTime.TryParse(attributes[AttributeDataUltimoAggiornamento], out minValue))
                    {
                        minValue = DateTime.MinValue;
                    }
                    if (minValue.Date < datiAVCP.DatiAvcp.metadata.dataUltimoAggiornamentoDataset.Date)
                    {
                        FileLogger.Info(base.Name, string.Format("UpdateAndCheckResponse: {0}", string.Format("CodiceServizio: {0}, AnnoAtto: {1}, NumeroAtto: {2}", wSDBPAVCPPubblicazioneInput1.CodiceServizio, wSDBPAVCPPubblicazioneInput1.AnnoAtto, wSDBPAVCPPubblicazioneInput1.NumeroAtto)));
                        UpdateData updateDatum = new UpdateData()
                        {
                            titolo = documentTitle,
                            @abstract = documentTitle,
                            dataPubblicazione = item.PublishingDate.GetValueOrDefault(DateTime.Today),
                            entePubblicatore = base.Parameters.AVCPEntePubblicatore,
                            annoDocumento = wSDBPAVCPPubblicazioneInput1.AnnoAtto,
                            urlFile = string.Format(base.Parameters.AVCPDatasetUrlMask, item.Id),
                            licenza = base.Parameters.AVCPLicenza
                        };
                        VecompSoftware.DocSuiteWeb.AVCP.pubblicazione _pubblicazione = this.UpdateAndCheckResponse(updateDatum, datiAVCP);
                        if (_pubblicazione != null)
                        {
                            SetDataSetResult setDataSetResult = _avcpFacade.SetDataSetPub(_pubblicazione, item, base.Parameters.Username);
                            FileLogger.Debug(AVCPFacade.LoggerName, string.Format("DataSet Aggiornato {0}", setDataSetResult.Updated));
                            FileLogger.Debug(AVCPFacade.LoggerName, string.Format("DataSet Flushed {0}", setDataSetResult.Flushed));
                            FileLogger.Debug(AVCPFacade.LoggerName, string.Format("DataSet LastUpdate {0}", setDataSetResult.LastUpdate));
                            FileLogger.Debug(AVCPFacade.LoggerName, string.Format("DataSet Saved {0}", setDataSetResult.Saved));
                            FileLogger.Debug(AVCPFacade.LoggerName, string.Format("DataSet Chain.ID {0}", setDataSetResult.Chain.ID));
                            TenderHeader tender = _avcpFacade.LinkToTender(_pubblicazione, null, item);
                            FileLogger.Info(AVCPFacade.LoggerName, string.Format("Collegato a Tender {0}", tender.Id));
                            if (item.PublishingDate.HasValue)
                            {
                                DateTime? publishingDate = item.PublishingDate;
                                FileLogger.Debug(AVCPFacade.LoggerName, string.Format("DocumentSeriesItem già pubblicata {0}", publishingDate.Value));
                            }
                            else
                            {
                                item.PublishingDate = new DateTime?(DateTime.Today);
                                FacadeFactory.Instance.DocumentSeriesItemFacade.Update(ref item);
                                DateTime? nullable = item.PublishingDate;
                                FileLogger.Info(AVCPFacade.LoggerName, string.Format("DocumentSeriesItem pubblicata {0}", nullable.Value));
                            }
                            FileLogger.Info(base.Name, string.Format("{0} - Aggiornato {1}", "UpdateDocumentItem", str));
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        FileLogger.Info(base.Name, string.Format("Nessun Aggiornamento da fare. Data Ultimo Aggiornamento: {0:dd/MM/yyyy}", datiAVCP.DatiAvcp.metadata.dataUltimoAggiornamentoDataset));
                        flag = true;
                    }
                }
                else
                {
                    FileLogger.Info(base.Name, "Nessun dato.");
                    if (base.Parameters.UnpublishEmptyData && item.PublishingDate.HasValue)
                    {
                        item.PublishingDate = null;
                        FacadeFactory.Instance.DocumentSeriesItemFacade.Update(ref item);
                        FileLogger.Info(base.Name, "Pubblicazione annullata.");
                    }
                    flag = true;
                }
            }
            catch (Exception exception1)
            {
                base.OnError(string.Format("{0} - Errore aggiornamento {1}", "UpdateDocumentItem", str), exception1);
                flag = false;
            }
            return flag;
        }
        #endregion

        #region [ Methods ]
        private string SendAllCategories()
        {
            string str = "SendAllCategories non completato.";
            try
            {
                IList<Role> all = base.Facade.RoleFacade.GetAll();
                int num = 0;
                int num1 = 0;
                foreach (Role role in all)
                {
                    if (role.IsActive != 1 || string.IsNullOrEmpty(role.ServiceCode))
                    {
                        continue;
                    }
                    WSDBPAVCPOrganiDelibernatiInput wSDBPAVCPOrganiDelibernatiInput = new WSDBPAVCPOrganiDelibernatiInput()
                    {
                        CodiceServizio = role.ServiceCode,
                        DescrizioneServizio = role.Name
                    };
                    WSDBPAVCPOutput wSDBPAVCPOutput = this.AvcpWs.PutOrganoDeliberante(wSDBPAVCPOrganiDelibernatiInput);
                    if (this.CheckResponse("SendDocumentCategories", role.ServiceCode, wSDBPAVCPOutput))
                    {
                        num++;
                        FileLogger.Info(base.Name, string.Format("Inviato OrganoDeliberante CodiceServizio:{0}, DescrizioneServizio: {1}", role.ServiceCode, role.Name));
                    }
                    else
                    {
                        num1++;
                    }
                }
                str = string.Format("SendDocumentCategories - Trasmessi {0} ruoli con successo. {1} errori.", num, num1);
                FileLogger.Info(base.Name, str);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.OnError(string.Format("Errore in SendDocumentCategories - FullstackTrace: {0}", this.FullStacktrace(exception)), exception);
            }
            return str;
        }

        private string SendNewDocuments()
        {
            string str = "SendNewDocuments non completato.";
            try
            {
                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (base.Parameters.ResolutionFromDate != DateTime.MinValue)
                {
                    fromDate = new DateTime?(base.Parameters.ResolutionFromDate);
                }
                if (base.Parameters.ResolutionToDate != DateTime.MinValue)
                {
                    toDate = new DateTime?(base.Parameters.ResolutionToDate);
                }
                List<Resolution> list = base.Facade.ResolutionFacade.GetAdoptedResolutionNotAvcp(fromDate, toDate).ToList<Resolution>();
                if (list.Count > 0)
                {
                    FileLogger.Info(base.Name, string.Format("Trovati {0} documenti da spedire.", list.Count));
                }
                int num = 0;
                foreach (Resolution resolution in list)
                {
                    if (!base.Cancel)
                    {
                        try
                        {
                            WSDBPAVCPDelibereInput wSDBPAVCPDelibereInput = new WSDBPAVCPDelibereInput()
                            {
                                CodiceServizio = resolution.CodiceServizio(),
                                AnnoAtto = resolution.Year.Value,
                                NumeroAtto = resolution.NumeroAtto(),
                                DataAtto = resolution.AdoptionDate.Value,
                                TipoAtto = resolution.Type.Description,
                                Oggetto = resolution.ResolutionObject,
                                LinkEsterno = string.Format(base.Parameters.DocumentUrlMask, resolution.Id)
                            };
                            string description = resolution.Type.Description;
                            string inclusiveNumber = resolution.InclusiveNumber;
                            DateTime value = resolution.AdoptionDate.Value;
                            wSDBPAVCPDelibereInput.Segnatura = string.Format("{0} {1} del {2}", description, inclusiveNumber, value.ToShortDateString());
                            wSDBPAVCPDelibereInput.CodiceEsternoAtto = resolution.InclusiveNumber;
                            WSDBPAVCPDelibereInput wSDBPAVCPDelibereInput1 = wSDBPAVCPDelibereInput;
                            WSDBPAVCPOutput wSDBPAVCPOutput = this.AvcpWs.PutDelibera(wSDBPAVCPDelibereInput1);
                            if (this.CheckResponse("SendNewDocuments", resolution.Id.ToString(), wSDBPAVCPOutput))
                            {
                                FileLogger.Info(base.Name, string.Format("Invio - PutDelibera CodiceServizio: {0}, AnnoAtto: {1}, NumeroAtto: {2}", wSDBPAVCPDelibereInput1.CodiceServizio, wSDBPAVCPDelibereInput1.AnnoAtto, wSDBPAVCPDelibereInput1.NumeroAtto));
                                base.SaveToBiblos(resolution);
                            }
                            else
                            {
                                num++;
                                FileLogger.Error(base.Name, string.Format("Errore - PutDelibera CodiceServizio: {0}, AnnoAtto: {1}, NumeroAtto: {2}", wSDBPAVCPDelibereInput1.CodiceServizio, wSDBPAVCPDelibereInput1.AnnoAtto, wSDBPAVCPDelibereInput1.NumeroAtto));
                            }
                        }
                        catch (Exception exception1)
                        {
                            Exception exception = exception1;
                            num++;
                            base.OnError(string.Concat("Errore in SendNewDocuments per Resolution ", resolution.Id), exception);
                        }
                    }
                    else
                    {
                        FileLogger.Info(base.Name, "Modulo interrotto da STOP Servizio");
                        break;
                    }
                }
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
                str = string.Format("SendNewDocuments - Trasmessi {0} documenti con successo. {1} errori.", list.Count - num, num);
                FileLogger.Info(base.Name, str);
            }
            catch (Exception exception2)
            {
                base.OnError("ERRORI in SendNewDocuments.", exception2);
            }
            return str;
        }
        private bool CheckResponse(string caller, string objectId, WSDBPAVCPOutput output)
        {
            if (output.result == null || (int)output.result.Length == 0)
            {
                return false;
            }
            if (output.result[0].ReturnCode == 0)
            {
                return true;
            }
            string str = XmlUtil.Serialize<WSDBPAVCPResult>(output.result[0], null, "");
            FileLogger.Error(base.Name, string.Format("Errore risposta in {0}: ObjectId:{1} - WSDBPAVCPResult:{2}", caller, objectId, str));
            return false;
        }

        private VecompSoftware.DocSuiteWeb.AVCP.pubblicazione UpdateAndCheckResponse(UpdateData data, WSDBPPubblicazioneOutput response)
        {
            if (response == null || response.Result == null)
            {
                return null;
            }
            if (response.Result.ReturnCode != 0)
            {
                string str = XmlUtil.Serialize<WSDBPAVCPResult>(response.Result, null, "");
                FileLogger.Error(base.Name, string.Format("Errore risposta in {0}", "UpdateAndCheckResponse"));
                FileLogger.Debug(base.Name, string.Format("WSDBPAVCPResult:{0}", str));
                return null;
            }
            VecompSoftware.DocSuiteWeb.AVCP.pubblicazione _pubblicazione = XmlUtil.Deserialize<VecompSoftware.DocSuiteWeb.AVCP.pubblicazione>(ENCOHelper.GetAVCPXml(response.DatiAvcp));
            _pubblicazione.metadata.titolo = data.titolo;
            _pubblicazione.metadata.@abstract = data.@abstract;
            _pubblicazione.metadata.dataPubbicazioneDataset = data.dataPubblicazione;
            _pubblicazione.metadata.entePubblicatore = data.entePubblicatore;
            _pubblicazione.metadata.annoRiferimento = data.annoDocumento;
            _pubblicazione.metadata.urlFile = data.urlFile;
            _pubblicazione.metadata.licenza = data.licenza;
            List<string> validationErrors = new List<string>();
            if (_avcpFacade.ValidateAVCP(_pubblicazione, out validationErrors))
            {
                return _pubblicazione;
            }
            FileLogger.Error(base.Name, string.Format("Errore di validazione AVCP in {0}", "UpdateAndCheckResponse"));
            FileLogger.Debug(base.Name, "Validator Errors:");
            foreach (string str1 in validationErrors)
            {
                FileLogger.Debug(base.Name, str1);
            }
            return null;
        }

        #endregion

    }

}


