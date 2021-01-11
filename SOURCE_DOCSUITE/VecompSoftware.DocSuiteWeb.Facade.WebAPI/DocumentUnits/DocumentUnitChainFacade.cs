using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.DocumentUnits
{
    public class DocumentUnitChainFacade : FacadeWebAPIBase<DocumentUnitChain, DocumentUnitChainDao>
    {
        #region [ Fields ]
        public const string BIBLOS_ATTRIBUTE_UniqueId = "UniqueId";
        public const string BIBLOS_ATTRIBUTE_Miscellanea = "Miscellanea";
        public const string BIBLOS_ATTRIBUTE_Environment = "Environment";
        public const string BIBLOS_ATTRIBUTE_Year = "Year";
        public const string BIBLOS_ATTRIBUTE_Number = "Number";
        public const string BIBLOS_ATTRIBUTE_Disabled = "Disabled";
        public const string BIBLOS_ATTRIBUTE_UserVisibilityAuthorized = "UserVisibilityAuthorized";
        #endregion

        #region [ Construcor ]
        public DocumentUnitChainFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<DocumentUnitChain, DocumentUnitChainDao>(s)).ToList(), currentTenant)
        {
        }
        #endregion

        #region [ Methods ]

        public DocumentInfo GetDocumentUnitChainsDocuments(ICollection<DocumentUnitChain> documentUnitChains, IDictionary<Model.Entities.DocumentUnits.ChainType, string> seriesCaptionLabel, bool? forceAuthorization = null)
        {
            if (documentUnitChains != null && documentUnitChains.Count > 0)
            {
                DocumentUnit documentUnit = documentUnitChains.First().DocumentUnit;
                FolderInfo mainFolder = new FolderInfo
                {
                    Name = $"{documentUnit.DocumentUnitName} {documentUnit.Title}",
                    ID = documentUnit.UniqueId.ToString()
                };

                FolderInfo folderDoc;
                BiblosDocumentInfo[] docs;
                string folderName;
                foreach (DocumentUnitChain chain in documentUnitChains.Where(f => f.ChainType <= ChainType.UnpublishedAnnexedChain || f.ChainType == ChainType.DematerialisationChain).OrderBy(x => x.ChainType))
                {
                    docs = null;
                    folderDoc = new FolderInfo
                    {
                        Parent = mainFolder
                    };
                    folderName = string.Empty;
                    switch (chain.ChainType)
                    {
                        case ChainType.MainChain:
                            {
                                folderName = "Documento";
                                if (documentUnit.Environment == (int)Data.DSWEnvironment.DocumentSeries && seriesCaptionLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.MainChain))
                                {
                                    folderName = seriesCaptionLabel[Model.Entities.DocumentUnits.ChainType.MainChain];
                                }
                                folderDoc.Name = folderName;
                                break;
                            }
                        case ChainType.AttachmentsChain:
                            {
                                folderDoc.Name = "Allegati (parte integrante)";
                                break;
                            }
                        case ChainType.AnnexedChain:
                            {
                                folderName = "Annessi (non parte integrante)";
                                if (documentUnit.Environment == (int)Data.DSWEnvironment.DocumentSeries && seriesCaptionLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.AnnexedChain))
                                {
                                    folderName = seriesCaptionLabel[Model.Entities.DocumentUnits.ChainType.AnnexedChain];
                                }
                                folderDoc.Name = folderName;
                                break;
                            }
                        case ChainType.UnpublishedAnnexedChain:
                            {
                                folderName = "Annessi non pubblicati";
                                if (documentUnit.Environment == (int)Data.DSWEnvironment.DocumentSeries && seriesCaptionLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain))
                                {
                                    folderName = seriesCaptionLabel[Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain];
                                }
                                folderDoc.Name = folderName;
                                break;
                            }
                        case ChainType.DematerialisationChain:
                            {
                                folderDoc.Name = "Attestazione di conformità";
                                break;
                            }
                        default:
                            break;
                    }
                    docs = BiblosDocumentInfo.GetDocuments(chain.IdArchiveChain).ToArray();
                    if (docs != null)
                    {
                        //TODO: attributi da non salvare in Biblos
                        foreach (BiblosDocumentInfo doc in docs)
                        {
                            doc.AddAttribute(BIBLOS_ATTRIBUTE_Environment, documentUnit.Environment.ToString());
                            doc.AddAttribute(BIBLOS_ATTRIBUTE_Year, documentUnit.Year.ToString());
                            doc.AddAttribute(BIBLOS_ATTRIBUTE_Number, documentUnit.Number.ToString());
                            doc.AddAttribute(BIBLOS_ATTRIBUTE_UniqueId, documentUnit.UniqueId.ToString());
                            if (chain.DocumentUnit != null && chain.DocumentUnit.UDSRepository != null)
                            {
                                UDSModel model = UDSModel.LoadXml(chain.DocumentUnit.UDSRepository.ModuleXML);
                                if (!model.Model.StampaConformeEnabled)
                                {
                                    doc.AddAttribute(BIBLOS_ATTRIBUTE_Disabled, bool.TrueString);
                                }
                            }
                            if (forceAuthorization.HasValue && forceAuthorization.Value)
                            {
                                doc.AddAttribute(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, bool.TrueString);
                            }
                        }
                        folderDoc.AddChildren(docs);
                    }
                }

                return mainFolder;
            }
            return null;
        }
        #endregion
    }
}
