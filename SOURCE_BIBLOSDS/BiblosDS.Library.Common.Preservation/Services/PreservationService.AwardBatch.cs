using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.AwardBatch;
using BiblosDS.Library.Common.Preservation.IpdaDoc;
using BiblosDS.Library.Common.Preservation.ServiceReferenceStorage;
using BiblosDS.Library.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using BiblosStorageService = VecompSoftware.BiblosDS.Service.Storage;

namespace BiblosDS.Library.Common.Preservation.Services
{
    public partial class PreservationService
    {
        public static BindingList<Objects.AwardBatch> GetPreservationAwardBatches(Guid idPreservation)
        {
            return DbProvider.GetPreservationAwardBatches(idPreservation);
        }


        public Objects.AwardBatch GetAwardBatch(Guid idAwardBatch)
        {
            logger.Info("GetAwardBatch - entry point");

            try
            {
                Objects.AwardBatch ret = DbProvider.GetAwardBatch(idAwardBatch);
                logger.Info("GetAwardBatch - ritorno al chiamante");
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }


        public BindingList<Objects.AwardBatch> GetAwardBatches(Guid idArchive, int take, int skip, out int totalBatches)
        {
            logger.Info("GetAwardBatches - entry point");

            try
            {
                BindingList<Objects.AwardBatch> ret = DbProvider.GetAwardBatches(idArchive, take, skip, out totalBatches);

                logger.Info("GetAwardBatches - ritorno al chiamante");
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public BindingList<Objects.AwardBatch> GetAwardBatches(Guid idArchive, Guid? idPreservation, DateTime? fromDate, DateTime? toDate, int take, int skip, out int totalBatches)
        {
            logger.Info("GetAwardBatches - entry point");
            try
            {
                BindingList<Objects.AwardBatch> ret = DbProvider.GetAwardBatches(idArchive, idPreservation, fromDate, toDate, take, skip, out totalBatches);

                logger.Info("GetAwardBatches - ritorno al chiamante");
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public BindingList<Objects.AwardBatch> GetAwardBatches(Guid idArchive, DateTime fromDate, DateTime toDate, int take, int skip, out int totalBatches)
        {
            return GetAwardBatches(idArchive, null, fromDate, toDate, take, skip, out totalBatches);
        }

        public BindingList<Objects.Document> GetAwardBatchDocuments(Guid idAwardBatch, int skip, int take, out int totalDocs)
        {
            logger.Info("GetAwardBatchDocuments - entry point");

            try
            {
                BindingList<Objects.Document> ret = DbProvider.GetAwardBatchDocuments(idAwardBatch, skip, take, out totalDocs);

                logger.Info("GetAwardBatchDocuments - ritorno al chiamante");
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public int GetAwardBatchDocumentsCounter(Guid idAwardBatch)
        {
            try
            {
                return DbProvider.GetAwardBatchDocumentsCounter(idAwardBatch);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public ICollection<Objects.AwardBatch> GetToSignAwardBatchRDV(Guid idArchive)
        {
            try
            {
                return DbProvider.GetToSignAwardBatchRDV(idArchive);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        private string CreateAwardBatchRptFile(Objects.Preservation preservation, Company company, string workingDir, string preservationName, string exceptions, IDictionary<Guid, BindingList<DocumentAttributeValue>> fullDocumentAttributes)
        {
            logger.InfoFormat("CreateAwbReportFile - work dir {0} eccezioni {1}", workingDir, exceptions);

            try
            {
                if (string.IsNullOrEmpty(workingDir))
                    throw new Exception("Working directory non configurata correttamente.");

                if (preservation.Documents == null)
                    throw new Exception(string.Format("Nessun documento associato alla conservazione con id {0}", preservation.IdPreservation));

                if (exceptions == null)
                    exceptions = string.Empty;

                var nDoc = preservation.Documents.Count();

                var dtHlp = preservation.Documents.Select(x => x.DateMain).Min();
                var sDataMinima = dtHlp.HasValue ? dtHlp.Value.ToString("dd/MM/yyyy") : "N.D.";

                dtHlp = preservation.Documents.Select(x => x.DateMain).Max();
                var sDataMassima = dtHlp.HasValue ? dtHlp.Value.ToString("dd/MM/yyyy") : "N.D.";

                var sFileNameBatch = Path.Combine(workingDir, "LOTTI_" + preservationName + ".xml");
                var sFileBatchXsl = Path.Combine(workingDir, "LOTTI_" + preservationName + ".xsl");

                //Scrive il file XSLT di trasformazione del AwardBatch Xml
                ArchiveCompany archiveCompany = null;
                var list = GetArchiveCompany(preservation.Archive.IdArchive, company.CompanyName);
                if (list != null && list.Count > 0)
                    archiveCompany = list[0];

                File.WriteAllText(sFileBatchXsl, archiveCompany.AwardBatchXSLTFile);

                //generazione oggetto AwbReport che sarà poi serializzato come file Xml               
                AwbReport report = new AwbReport();
                BindingList<Objects.AwardBatch> batches = GetPreservationAwardBatches(preservation.IdPreservation);

                //elenco dei lotti
                List<AwbBatch> batchList = new List<AwbBatch>();
                foreach (Objects.AwardBatch awardBatch in batches)
                {
                    batchList.Add(new AwbBatch
                    {
                        Descrizione = awardBatch.Name,
                        Files = GetAwardBatchFiles(preservation, awardBatch.IdAwardBatch, fullDocumentAttributes)
                    });
                }

                //elenco dei file
                report.Batches = batchList.ToArray();

                //salva nella cartella di destinazione
                report.SaveAs(sFileNameBatch, sFileBatchXsl);
                logger.Info("CreateAwbReportFile - ritorno al chiamante");

                return string.Empty;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return e.Message + " ===> " + e.Source;
            }
        }


        private AwbBatchFile[] GetAwardBatchFiles(Objects.Preservation preservation, Guid idBatch, IDictionary<Guid, BindingList<DocumentAttributeValue>> fullDocumentAttributes)
        {
            List<AwbBatchFile> files = new List<AwbBatchFile>();
            var attributes = DbProvider.GetAttributesFromArchive(preservation.IdArchive)
              .Where(x => !x.ConservationPosition.HasValue || x.ConservationPosition.Value > 0)
              .OrderBy(x => x.ConservationPosition);

            foreach (var doc in preservation.Documents.OrderBy(x => x.PreservationIndex))
            {
                //salta i documenti che non sono di questo lotto
                if (doc.IdAwardBatch != idBatch)
                    continue;

                BindingList<DocumentAttributeValue> docAttributes = fullDocumentAttributes[doc.IdDocument];                
                List<AwbBatchFileAttribute> attrList = new List<AwbBatchFileAttribute>();
                foreach (var attr in attributes)
                {
                    AwbBatchFileAttribute awbAttr = new AwbBatchFileAttribute();                    

                    DocumentAttributeValue currAttributeValue = docAttributes.Where(x => x.IdAttribute == attr.IdAttribute).FirstOrDefault();
                    if (currAttributeValue == null)
                        continue;

                    if (currAttributeValue.Attribute == null)
                        currAttributeValue.Attribute = attr;

                    awbAttr.Nome = string.IsNullOrEmpty(attr.Description) ? attr.Name : attr.Description;

                    //L'attributo va formattato in funzione della proprieta' "Format" dell'Attributo, se valorizzata.
                    string valoreAttr;
                    if (currAttributeValue.Attribute != null && !string.IsNullOrEmpty(currAttributeValue.Attribute.Format))
                    {
                        try
                        {
                            valoreAttr = string.Format(currAttributeValue.Attribute.Format, currAttributeValue.Value);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            valoreAttr = string.Empty;
                        }
                    }
                    else
                    {
                        valoreAttr = (currAttributeValue.Value != null) ? currAttributeValue.Value.ToString() : string.Empty;
                    }

                    awbAttr.Valore = valoreAttr;
                    attrList.Add(awbAttr);
                }

                string fileName = string.Format("{0}{1}", PurgeFileName(docAttributes, string.IsNullOrEmpty(doc.PrimaryKeyValue) ? doc.PreservationIndex.GetValueOrDefault().ToString() : doc.PrimaryKeyValue), Path.GetExtension(doc.Name));

                attrList.Insert(0, new AwbBatchFileAttribute
                {
                    Nome = "NomeFileInArchivio",
                    Valore = fileName
                });

                attrList.Add(new AwbBatchFileAttribute
                {
                    Nome = "ImprontaFileSHA256",
                    Valore = doc.DocumentHash
                });

                files.Add(new AwbBatchFile
                {
                    Attributes = attrList.ToArray()
                });

            }

            return files.ToArray();
        }

        public string CreateAwardBatchPDVXml(Objects.AwardBatch awardBatch)
        {
            return CreateAwardBatchPDVXml(awardBatch, null);
        }

        public string CreateAwardBatchPDVXml(Objects.AwardBatch awardBatch, Objects.Preservation preservation)
        {
            try
            {

                Ipda ipda = new Ipda()
                {
                    descGenerale = new DescGenerale()
                    {
                        id = awardBatch.Name,
                        applicazione = new DescApplicazione()
                        {
                            nome = "Biblos Document Server",
                            produttore = "Dgroove Srl",
                            versione = "2015.0"
                        }
                    },
                    pda = new Pda()
                    {
                        extraInfo = new PdaExtraInfo()
                        {
                            metadati = new PdaMetadati()
                        }
                    }
                };

                List<PdaAttributo> attrList = DbProvider.GetAttributeByPreservationPosition(awardBatch.IdArchive).Select(p => new PdaAttributo { nome = p.Description }).ToList();
                attrList.Insert(0, new PdaAttributo() { nome = "NomeFile" });
                attrList.Insert(1, new PdaAttributo() { nome = "DataVersamento" });
                attrList.Add(new PdaAttributo() { nome = "ImprontaSHA" });
                ipda.pda.extraInfo.metadati.attributoList = attrList.ToArray();

                //TODO: Completamente da rivedere. Creare dei service specifici per creare e leggere i documenti da file system senza la necessità di chiamare ogni volta il WCFHost.
                List<FileGruppoFile> files = new List<FileGruppoFile>();
                BindingList<Document> documents = DbProvider.GetAwardBatchDocuments(awardBatch.IdAwardBatch);
                List<DocumentAttribute> attributes = DbProvider.GetAttributesFromArchive(awardBatch.IdArchive).Where(x => !x.ConservationPosition.HasValue || x.ConservationPosition.Value > 0).OrderBy(x => x.ConservationPosition).ToList();
                byte[] documentContent;
                string documentHash;
                BiblosStorageService.StorageService storageService = new BiblosStorageService.StorageService();
                foreach (Document document in documents)
                {
                    documentContent = null;
                    documentHash = null;
                    if (preservation != null && preservation.Documents != null)
                    {
                        Document currentDocument = preservation.Documents.SingleOrDefault(x => x.IdDocument == document.IdDocument);
                        if (currentDocument != null)
                        {
                            documentHash = currentDocument.DocumentHash;
                        }
                    }

                    if (string.IsNullOrEmpty(documentHash))
                    {                        
                        DocumentContent currentDocumentContent = storageService.GetDocumentContent(document);
                        documentContent = currentDocumentContent.Blob;
                        documentHash = UtilityService.GetHash(documentContent, true);
                    }

                    document.AttributeValues = DbProvider.GetFullDocumentAttributeValues(document.IdDocument);
                    List<FgAttributo> docAttrList = new List<FgAttributo>();
                    foreach (var attr in attributes)
                    {
                        FgAttributo fgAttr = new FgAttributo();

                        var currAttributeValue = document.AttributeValues.Where(x => x.IdAttribute == attr.IdAttribute).FirstOrDefault();
                        if (currAttributeValue == null)
                            continue;

                        if (currAttributeValue.Attribute == null)
                            currAttributeValue.Attribute = attr;

                        fgAttr.nome = string.IsNullOrEmpty(attr.Description) ? attr.Name : attr.Description;

                        //L'attributo va formattato in funzione della proprieta' "Format" dell'Attributo, se valorizzata.
                        string valoreAttr;
                        if (currAttributeValue.Attribute != null && !string.IsNullOrEmpty(currAttributeValue.Attribute.Format))
                        {
                            try
                            {
                                valoreAttr = string.Format(currAttributeValue.Attribute.Format, currAttributeValue.Value);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                valoreAttr = string.Empty;
                            }
                        }
                        else
                        {
                            valoreAttr = (currAttributeValue.Value != null) ? currAttributeValue.Value.ToString() : string.Empty;
                        }

                        fgAttr.valore = valoreAttr;
                        docAttrList.Add(fgAttr);
                    }

                    docAttrList.Insert(0, new FgAttributo() { nome = "NomeFile", valore = document.Name });
                    docAttrList.Insert(1, new FgAttributo() { nome = "DataVersamento", valore = document.DateMain.Value.ToString("dd/MM/yyyy") });
                    docAttrList.Add(new FgAttributo() { nome = "ImprontaSHA", valore = documentHash });

                    files.Add(new FileGruppoFile()
                    {
                        id = document.Name,
                        impronta = documentHash,
                        extraInfo = new FgExtraInfo()
                        {
                            metadati = new FgMetadati()
                            {
                                attributoList = docAttrList.ToArray()
                            }
                        }
                    });
                }

                ipda.fileGruppo = new FileGruppo()
                {
                    files = files.ToArray()
                };
                return XmlFile<Ipda>.Serialize(ipda);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
        }

        public void UpdateAwardBatch(Objects.AwardBatch awardBatch)
        {
            try
            {
                DbProvider.UpdateAwardBatch(awardBatch);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public void CloseAwardBatch(Objects.AwardBatch awardBatch)
        {
            try
            {
                if (!awardBatch.IsOpen)
                {
                    logger.WarnFormat("Il pacchetto di conservazione con Id {0} risulta già chiuso", awardBatch.IdAwardBatch);
                    return;
                }
                DbProvider.CloseAwardBatch(awardBatch.IdAwardBatch);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
