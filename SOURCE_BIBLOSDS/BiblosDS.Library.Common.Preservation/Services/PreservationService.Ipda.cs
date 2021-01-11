using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.IpdaDoc;
using BiblosDS.Library.Common.Utility;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;


namespace BiblosDS.Library.Common.Preservation.Services
{
    public partial class PreservationService
    {
        private string CreatePreservationIpdaFile(Objects.Preservation preservation, string workingDir, string preservationName, string exceptions, IDictionary<Guid, BindingList<DocumentAttributeValue>> fullDocumentAttributes)
        {
            logger.InfoFormat("CreatePreservationIpdaFile - work dir {0} eccezioni {1}", workingDir, exceptions);

            try
            {
                if (string.IsNullOrEmpty(workingDir))
                {
                    throw new Exception("Working directory non configurata correttamente.");
                }

                if (preservation.Documents == null)
                {
                    throw new Exception(string.Format("Nessun documento associato alla conservazione con id {0}", preservation.IdPreservation));
                }

                if (exceptions == null)
                {
                    exceptions = string.Empty;
                }

                var user = preservation.User;
                var sCognomeNomeCf = string.Format("{0} {1} C.F. {2}", user.Name, user.Surname, user.FiscalId);
                var nDoc = preservation.Documents.Count();

                var dtHlp = preservation.Documents.Select(x => x.DateMain).Min();
                var sDataMinima = dtHlp.HasValue ? dtHlp.Value.ToString("dd/MM/yyyy") : "N.D.";

                dtHlp = preservation.Documents.Select(x => x.DateMain).Max();
                var sDataMassima = dtHlp.HasValue ? dtHlp.Value.ToString("dd/MM/yyyy") : "N.D.";

                var sFileName = Path.Combine(workingDir, "CHIUSURA_" + preservationName + ".txt");
                var sFileNameIpda = Path.Combine(workingDir, "IPDA_" + preservationName + ".xml");

                var sFileIndice = Path.Combine(workingDir, "INDICE_" + preservationName + ".txt");
                var sFileIndiceXML = Path.Combine(workingDir, "INDICE_" + preservationName + ".xml");
                var sFileIndiceXsl = Path.Combine(workingDir, "INDICE_" + preservationName + ".xsl");

                var sFileLottoXML = Path.Combine(workingDir, "LOTTI_" + preservationName + ".xml");
                var sFileLottoXsl = Path.Combine(workingDir, "LOTTI_" + preservationName + ".xsl");

                var sFileIndiceNome = "INDICE_" + preservationName + ".txt";

                //generazione oggetto Ipda che sarà poi serializzato come file Xml               
                Ipda ipda = new Ipda();

                ipda.descGenerale = new DescGenerale
                {
                    id = preservationName,
                    applicazione = new DescApplicazione
                    {
                        nome = "Biblos Document Server",
                        produttore = "Dgroove Srl",
                        versione = "2015.0"
                    }
                };

                ipda.descGenerale.extraInfo = new DgExtraInfo();

                Dictionary<string, List<long>> sectional = EstraiGruppi_v1(preservation);
                List<DgmiBlocco> blocchiList = new List<DgmiBlocco>();

                foreach (var section in sectional)
                {
                    blocchiList.Add(new DgmiBlocco
                    {
                        id = section.Key,
                        documenti = section.Value.Count().ToString(),
                        dal = section.Value.Min().ToString(),
                        al = section.Value.Max().ToString()
                    });
                }

                ipda.descGenerale.extraInfo.metadatiIntegrati = new DgMetadatiIntegrati
                {
                    metadati = new DgmiMetadati
                    {
                        blocchi = new DgmiBlocchi
                        {
                            bloccoList = blocchiList.ToArray()
                        },
                        documenti = new DgmiDocumenti
                        {
                            tipologia = preservation.Archive.FiscalDocumentType,
                            numero = nDoc.ToString(),
                            dal = sDataMinima,
                            al = sDataMassima
                        }
                    }
                };

                List<DgMetadatiEsterni> metadatiEsterni = new List<DgMetadatiEsterni>();

                if (File.Exists(sFileName))
                {
                    metadatiEsterni.Add(new DgMetadatiEsterni
                    {
                        id = "CHIUSURA_TXT",
                        indirizzo = Path.GetFileName(sFileName),
                        impronta = UtilityService.GetHash(sFileName, true)
                    });
                }

                if (File.Exists(sFileIndice))
                {
                    metadatiEsterni.Add(new DgMetadatiEsterni
                    {
                        id = "INDICE_TXT",
                        indirizzo = Path.GetFileName(sFileIndice),
                        impronta = UtilityService.GetHash(sFileIndice, true)
                    });
                }

                if (File.Exists(sFileIndiceXML))
                {
                    metadatiEsterni.Add(new DgMetadatiEsterni
                    {
                        id = "INDICE_XML",
                        indirizzo = Path.GetFileName(sFileIndiceXML),
                        impronta = UtilityService.GetHash(sFileIndiceXML, true)
                    });
                }

                if (File.Exists(sFileIndiceXsl))
                {
                    metadatiEsterni.Add(new DgMetadatiEsterni
                    {
                        id = "INDICE_XSL",
                        indirizzo = Path.GetFileName(sFileIndiceXsl),
                        impronta = UtilityService.GetHash(sFileIndiceXsl, true)
                    });
                }

                if (File.Exists(sFileLottoXML))
                {
                    metadatiEsterni.Add(new DgMetadatiEsterni
                    {
                        id = "LOTTI_XML",
                        indirizzo = Path.GetFileName(sFileLottoXML),
                        impronta = UtilityService.GetHash(sFileLottoXML, true)
                    });
                }

                if (File.Exists(sFileLottoXsl))
                {
                    metadatiEsterni.Add(new DgMetadatiEsterni
                    {
                        id = "LOTTI_XSL",
                        indirizzo = Path.GetFileName(sFileLottoXsl),
                        impronta = UtilityService.GetHash(sFileLottoXsl, true)
                    });
                }

                ipda.descGenerale.extraInfo.metadatiEsterniList = metadatiEsterni.ToArray();

                ipda.pda = new Pda
                {
                    id = Path.GetFileNameWithoutExtension(sFileIndice),
                    extraInfo = new PdaExtraInfo
                    {
                        metadati = new PdaMetadati()
                    }
                };

                List<PdaAttributo> attrList = DbProvider.GetAttributeByPreservationPosition(preservation.IdArchive).Select(p => new PdaAttributo { nome = p.Description }).ToList();

                attrList.Insert(0, new PdaAttributo
                {
                    nome = "NomeFileInArchivio"
                });

                attrList.Add(new PdaAttributo
                {
                    nome = "ImprontaFileSHA256"
                });

                ipda.pda.extraInfo.metadati.attributoList = attrList.ToArray();

                //elenco dei file
                ipda.fileGruppo = new FileGruppo
                {
                    files = GetIpdaFiles(preservation, fullDocumentAttributes)
                };

                //processo
                ipda.processo = new Processo
                {
                    soggetto = new Soggetto
                    {
                        soggettoID = sCognomeNomeCf
                    },
                    tempo = new Tempo
                    {
                        rifTemporale = DateTime.Now.Date.ToString("d") + " " + DateTime.Now.ToString("HH:mm")
                    },
                    rifNormativo = ""
                };

                //sostituisce il file di chiusura
                logger.Info("Inizio serializzazione ipda");
                string xmlString = XmlFile<Ipda>.Serialize(ipda);
                logger.Info("Fine serializzazione ipda");
                preservation.CloseContent = Encoding.ASCII.GetBytes(xmlString);
                DbProvider.UpdatePreservation(preservation, true);

                //salva nella cartella di destinazione
                ipda.SaveAs(sFileNameIpda);
                logger.Info("CreatePreservationIpdaFile - ritorno al chiamante");

                return string.Empty;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return e.Message + " ===> " + e.Source;
            }
        }


        private FileGruppoFile[] GetIpdaFiles(Objects.Preservation preservation, IDictionary<Guid, BindingList<DocumentAttributeValue>> fullDocumentAttributes)
        {
            logger.Info("GetIpdaFiles - INIT");
            List<FileGruppoFile> files = new List<FileGruppoFile>();
            var attributes = DbProvider.GetAttributesFromArchive(preservation.IdArchive)
              .Where(x => !x.ConservationPosition.HasValue || x.ConservationPosition.Value > 0)
              .OrderBy(x => x.ConservationPosition);

            foreach (var doc in preservation.Documents.OrderBy(x => x.PreservationIndex))
            {
                BindingList<DocumentAttributeValue> docAttributes = fullDocumentAttributes[doc.IdDocument];                
                List<FgAttributo> attrList = new List<FgAttributo>();
                foreach (var attr in attributes)
                {
                    FgAttributo fgAttr = new FgAttributo();                    

                    DocumentAttributeValue currAttributeValue = docAttributes.Where(x => x.IdAttribute == attr.IdAttribute).FirstOrDefault();
                    if (currAttributeValue == null)
                    {
                        continue;
                    }

                    if (currAttributeValue.Attribute == null)
                    {
                        currAttributeValue.Attribute = attr;
                    }

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
                    attrList.Add(fgAttr);
                }

                string fileName = string.Format("{0}{1}", PurgeFileName(docAttributes, string.IsNullOrEmpty(doc.PrimaryKeyValue) ? doc.PreservationIndex.GetValueOrDefault().ToString() : doc.PrimaryKeyValue), Path.GetExtension(doc.Name));

                attrList.Insert(0, new FgAttributo
                {
                    nome = "NomeFileInArchivio",
                    valore = fileName
                });

                attrList.Add(new FgAttributo
                {
                    nome = "ImprontaFileSHA256",
                    valore = doc.DocumentHash
                });

                files.Add(new FileGruppoFile
                {
                    id = fileName,
                    impronta = doc.DocumentHash,
                    extraInfo = new FgExtraInfo
                    {
                        metadati = new FgMetadati
                        {
                            attributoList = attrList.ToArray()
                        }
                    }
                });

            }
            logger.Info("GetIpdaFiles - END");
            return files.ToArray();
        }

    }
}
