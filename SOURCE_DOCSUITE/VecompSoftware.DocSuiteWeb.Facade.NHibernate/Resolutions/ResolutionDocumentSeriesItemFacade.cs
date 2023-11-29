using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Resolutions;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
{
    public class ResolutionDocumentSeriesItemFacade: BaseResolutionFacade<ResolutionDocumentSeriesItem, Guid, ResolutionDocumentSeriesItemDao>
    {
        #region [ Fields ]

        private ResolutionKindDocumentSeriesFacade _currentResolutionKindDocumentSeriesFacade;

        #endregion [ Fields ]

        #region [ Properties ]

        public ResolutionKindDocumentSeriesFacade CurrentResolutionKindDocumentSeriesFacade
        {
            get
            {
                if (_currentResolutionKindDocumentSeriesFacade == null)
                {
                    _currentResolutionKindDocumentSeriesFacade = new ResolutionKindDocumentSeriesFacade(DocSuiteContext.Current.User.FullUserName);
                }
                return _currentResolutionKindDocumentSeriesFacade;
            }
        }

        #endregion [ Properties ]

        #region [ Constructor ]

        public ResolutionDocumentSeriesItemFacade()
            : base()
        {
        }

        public ResolutionDocumentSeriesItemFacade(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }


        #endregion [ Constructor ]

        #region [ Methods ]

        /// <summary>
        /// Confermo la serie documentale AVCP e bandi di gara
        /// </summary>
        private void ConfirmDraftAvcpSeriesItem(Resolution resolution)
        {
            //Se sto confermando Bandi di Gara ed ho una serie AVCP associata
            if (HasAvcpSeriesToAutomaticConfirm(resolution))
            {
                ICollection<DocumentSeriesItem> seriesToComplete = FacadeFactory.Instance.ResolutionFacade.GetSeriesToComplete(resolution);
                DocumentSeriesItem avcpSeriesItem = seriesToComplete.SingleOrDefault(s => s.DocumentSeries.Id == DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId);
                //Imposto la data di pubblicazione quando l'atto sarà ritirato
                avcpSeriesItem.PublishingDate = resolution.PublishingDate.Value.AddDays(15);
                if (DocSuiteContext.Current.ResolutionEnv.CompleteTransparencyExecutiveStepEnabled && resolution.EffectivenessDate.HasValue)
                {
                    avcpSeriesItem.PublishingDate = resolution.EffectivenessDate;
                }
                BiblosChainInfo avcpChain = FacadeFactory.Instance.DocumentSeriesItemFacade.GetMainChainInfo(avcpSeriesItem);
                FacadeFactory.Instance.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(avcpSeriesItem, avcpChain, null, null, $"Pubblicata serie AVCP {avcpSeriesItem.Year:0000}/{avcpSeriesItem.Number:0000000} in data {avcpSeriesItem.PublishingDate:dd/MM/yyyy}");
                bool hasInserted = avcpSeriesItem.Status == DocumentSeriesItemStatus.Active;
                FacadeFactory.Instance.DocumentSeriesItemFacade.AssignNumber(avcpSeriesItem);
                if (hasInserted)
                {
                    FacadeFactory.Instance.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(avcpSeriesItem);
                }
                else
                {
                    FacadeFactory.Instance.DocumentSeriesItemFacade.SendInsertDocumentSeriesItemCommand(avcpSeriesItem, new List<IWorkflowAction>());
                }
            }           
        }

        /// <summary>
        /// Controllo se devo automaticamente confermare la serie documentale AVCP e Bandi di gara.
        /// </summary>
        /// <returns></returns>
        private bool HasAvcpSeriesToAutomaticConfirm(Resolution resolution)
        {
            ICollection<DocumentSeriesItem> seriesToComplete = FacadeFactory.Instance.ResolutionFacade.GetSeriesToComplete(resolution);

            if(seriesToComplete != null)
            {
                if (!DocSuiteContext.Current.ResolutionEnv.AutomaticConfirmAvcpSeries || !(seriesToComplete.Count > 0))
                    return false;

                ICollection<DocumentSeriesItem> checkSeries = seriesToComplete.Where(x => x.DocumentSeries.Id == DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId
                                                                || x.DocumentSeries.Id == DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId).ToList();
                return checkSeries.Count > 1;
            }
            return false;
        }


        /// <summary>
        /// Confermara e pubblica tutte le serie documentali, senza documento richiesto, associate all'atto attuale.
        /// </summary>
        public void ConfirmAndPublishSeries(Resolution resolution)
        {
            ICollection<DocumentSeriesItem> seriesItems = FacadeFactory.Instance.ResolutionFacade.GetSeriesByResolution(resolution);
            if(seriesItems != null && seriesItems.Count > 0)
            {
                foreach (DocumentSeriesItem seriesItem in seriesItems)
                {
                    bool isDocumentRequired = CurrentResolutionKindDocumentSeriesFacade.IsDocumentRequired(resolution.ResolutionKind.Id, seriesItem.DocumentSeries.Id);
                    BiblosChainInfo chain = FacadeFactory.Instance.DocumentSeriesItemFacade.GetMainChainInfo(seriesItem);
                    IEnumerable<BiblosDocumentInfo> docInfo = Factory.DocumentSeriesItemFacade.GetMainDocuments(seriesItem);
                    // Confermo e pubblico SOLO se il documento non è richiesto 
                    if (!isDocumentRequired || (isDocumentRequired && docInfo != null && docInfo.Count() > 0))
                    {
                        if (DocSuiteContext.Current.ResolutionEnv.CompleteTransparencyExecutiveStepEnabled && resolution.EffectivenessDate.HasValue)
                        {
                            seriesItem.PublishingDate = resolution.EffectivenessDate;
                        }
                        if (resolution.PublishingDate.HasValue && !seriesItem.PublishingDate.HasValue)
                        {
                            seriesItem.PublishingDate = resolution.PublishingDate.Value.AddDays(15);
                        }
                        
                        if (seriesItem.Status == DocumentSeriesItemStatus.Draft)
                        {
                            if(seriesItem.DocumentSeries.Id == DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId)
                            {
                                ConfirmDraftAvcpSeriesItem(resolution);
                            }
                            else
                            {
                                bool hasInserted = seriesItem.Status == DocumentSeriesItemStatus.Active;
                                FacadeFactory.Instance.DocumentSeriesItemFacade.AssignNumber(seriesItem);
                                if (hasInserted)
                                {
                                    FacadeFactory.Instance.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(seriesItem);
                                }
                                else
                                {
                                    FacadeFactory.Instance.DocumentSeriesItemFacade.SendInsertDocumentSeriesItemCommand(seriesItem, new List<IWorkflowAction>());
                                }
                            }
                        }
                        FacadeFactory.Instance.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(seriesItem, chain, null, null, $"Pubblicata registrazione {seriesItem.Year:0000}/{seriesItem.Number:0000000} in data {seriesItem.PublishingDate:dd/MM/yyyy}");
                    }
                }
            }
        }            



        #endregion [ Methods ]
    }
}
