
using BiblosDS.Library.Common.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.DB
{
    public partial class EntityProvider
    {
        /// <summary>
        /// Ritorna AwardBatch
        /// </summary>
        /// <param name="IdAwardBatch">Identificativo</param>
        /// <returns></returns>
        public AwardBatch GetAwardBatch(Guid IdAwardBatch)
        {
            try
            {
                return db.AwardBatch.SingleOrDefault(p => p.IdAwardBatch == IdAwardBatch).Convert();
            }
            finally
            {
                Dispose();
            }
        }


        /// <summary>
        /// Ritorna l'elenco di lotti per archivio
        /// </summary>
        /// <returns></returns>
        public BindingList<AwardBatch> GetAwardBatches(Guid idArchive, int take, int skip, out int totalBatches)
        {
            var set = this.db.AwardBatch.Where(x => x.IdArchive == idArchive).OrderByDescending(x => x.DateFrom);
            totalBatches = set.Count();

            if (skip < 0)
                skip = 0;

            if (take < 1)
                take = 1;

            IQueryable<Model.AwardBatch> batches = set.Skip(skip).Take(take);

            var retval = new BindingList<AwardBatch>();
            foreach (var item in batches)
            {
                retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return retval;
        }



        /// <summary>
        /// Ritorna l'elenco di lotti per archivio
        /// </summary>
        /// <returns></returns>
        public BindingList<AwardBatch> GetAwardBatches(Guid idArchive, Guid? idPreservation, DateTime? fromDate, DateTime? toDate, int take, int skip, out int totalBatches)
        {
            BindingList<AwardBatch> results = new BindingList<AwardBatch>();
            IQueryable<Model.AwardBatch> query = this.db.AwardBatch.Where(x => x.IdArchive == idArchive);
            if (fromDate.HasValue)
            {
                query = query.Where(x => x.DateFrom >= fromDate);
            }

            if(toDate.HasValue)
            {
                query = query.Where(x => x.DateFrom <= toDate);
            }

            if(idPreservation.HasValue && idPreservation.Value != Guid.Empty)
            {
                query = query.Where(x => x.Document.Any(xs => xs.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation.Value)));
            }
            query = query.OrderByDescending(x => x.DateFrom);
            totalBatches = query.Count();

            if (skip < 0)
                skip = 0;

            if (take < 1)
                take = 1;

            query = query.Skip(skip).Take(take);
            query.ToList().ForEach(f => results.Add(f.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL)));
            return results;
        }




        /// <summary>
        /// Ritorna l'elenco dei documenti assegnati ad un lotto di versamento.
        /// </summary>
        /// <param name="IdAwardBatch">Lotto di versamento</param>
        /// <returns></returns>

        public BindingList<Document> GetAwardBatchDocuments(Guid idAwardBatch, int skip, int take, out int documentsCount)
        {
            try
            {
                BindingList<Document> list = new BindingList<Document>();
                var query = db.Document
                    .Include("PreservationDocuments")
                    .Where(x => x.DocumentParent != null && x.IsVisible == 1 && x.IdAwardBatch == idAwardBatch && !x.IdParentVersion.HasValue 
                        && x.IsLatestVersion == true && (!x.IsDetached.HasValue || x.IsDetached == false));

                documentsCount = query.Count();
                foreach (var item in query.OrderBy(x => x.DateCreated).Skip(skip).Take(take))
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Ritorna l'elenco dei documenti assegnati ad un lotto di versamento.
        /// </summary>
        /// <param name="IdAwardBatch">Lotto di versamento</param>
        /// <returns></returns>
        public BindingList<Document> GetAwardBatchDocuments(Guid idAwardBatch)
        {
            try
            {
                BindingList<Document> list = new BindingList<Document>();
                var query = db.Document
                    .Include("PreservationDocuments")
                    .Where(x => x.DocumentParent != null && x.IsVisible == 1 && x.IdAwardBatch == idAwardBatch && !x.IdParentVersion.HasValue 
                        && x.IsLatestVersion == true && (!x.IsDetached.HasValue || x.IsDetached == false));
                foreach (var item in query.OrderBy(x => x.DateCreated))
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public int GetAwardBatchDocumentsCounter(Guid idAwardBatch)
        {
            try
            {
                BindingList<Document> list = new BindingList<Document>();
                var query = db.Document
                    .Where(x => x.DocumentParent != null && x.IsVisible == 1 && x.IdAwardBatch == idAwardBatch && !x.IdParentVersion.HasValue);
                return query.Count();
            }
            finally
            {
                Dispose();
            }
        }

        public ICollection<AwardBatch> GetToSignAwardBatchRDV(Guid idArchive)
        {
            try
            {
                ICollection<AwardBatch> results = new List<AwardBatch>();
                IQueryable<Model.AwardBatch> query = db.AwardBatch.Where(x => x.IdArchive == idArchive && x.IsOpen == 0 
                    && (x.IsRDVSigned == false || !x.IsRDVSigned.HasValue) && x.IdRDVDocument.HasValue);
                query.ToList().ForEach(f => results.Add(f.Convert()));
                return results;
            }
            finally
            {
                Dispose();
            }
        }


        /// <summary>
        /// Ritorna il batch aperto per archivio. Se nessuno è aperto lo crea
        /// </summary>
        /// <param name="idArchive">Archivio</param>
        /// <param name="isAuto">Indica se il lotto deve essere automatico o meno</param>
        /// <returns></returns>
        public AwardBatch GetOpenAwardBatch(Guid idArchive, bool isAuto = true)
        {
            return GetOpenAwardBatchModel(idArchive, (short)(isAuto == true ? 1 : 0)).Convert();
        }


        /// <summary>
        /// Chiude il lotto di versamento
        /// </summary>
        /// <param name="IdAwardBatch">Lotto da chiudere</param>
        /// <returns>true se chiude con successo, false se vi sono problemi</returns>
        public bool CloseAwardBatch(Guid IdAwardBatch)
        {
            try
            {
                var batch = db.AwardBatch
                  .Where(x => x.IdAwardBatch == IdAwardBatch && x.IsOpen == 1)
                  .FirstOrDefault();

                if (batch == null)
                    throw new Exceptions.AwardBatch_Exception("AwardBatch non trovato in CloseAwardBatch");

                var archive = db.Archive.Where(x => x.IdArchive == batch.IdArchive).SingleOrDefault();
                if (archive == null)
                    throw new Exceptions.AwardBatch_Exception("Archive non trovato in CloseAwardBatch");

                batch.DateTo = DateTime.Now;
                batch.Name = GetAwardBatchName(archive.Name, batch.DateFrom, batch.DateTo.Value);
                batch.IsOpen = 0;

                if (requireSave)
                    db.SaveChanges();

                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                Dispose();
            }
        }


        /// <summary>
        /// Sposta i documenti conservati o meno (dipende da parametro isConservated).
        /// appartenenti al batch passato.
        /// Viene salvato in IdParentBatch l'IdAwardBatch di sorgente in modo sia possibile vedere la catena di spostamento.
        /// </summary>
        /// <param name="idAwardBatch">Lotto origine</param>
        /// <param name="isConservated">Se 1 sposta i documenti conservati se 0 sposta i documenti da conservare</param>
        /// <param name="closeIfOpen">Se 1 sposta i documenti conservati se 0 sposta i documenti da conservare</param>
        public void MoveDocumentsAwardBatch(AwardBatch batch, bool isConservated, bool closeIfOpen)
        {
            using (DbTransaction tran = BeginNoSave())
            {
                try
                {
                    var docs = db.Document.Where(x => x.DocumentParent != null &&
                             x.IsVisible == 1 &&
                             !x.IdParentVersion.HasValue &&
                             x.IdAwardBatch == batch.IdAwardBatch &&
                             (x.IsConservated ?? 0) == (short)(isConservated == true ? 1 : 0)).ToList();

                    if (docs.Count() > 0)
                    {
                        //Crea nuovo lotto aperto ed aggiorna il parent
                        var newBatch = GetOpenAwardBatchModel(batch.IdArchive, 1);
                        newBatch.IdParentBatch = batch.IdAwardBatch;

                        //aggiorna tutti i documenti
                        foreach (var doc in docs)
                        {
                            doc.AwardBatch = newBatch;
                        }

                        db.SaveChanges();
                    }

                    tran.Commit();
                }
                catch (Exception)
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch
                    {
                    }

                    throw;
                }
            }
        }


        private BiblosDS.Library.Common.Model.AwardBatch GetDefaultAwardBatchModel(Guid idArchive, DateTime fromDate, short isAuto = 1)
        {
            var archive = db.Archive.Where(x => x.IdArchive == idArchive).SingleOrDefault();
            if (archive == null)
                throw new Exceptions.AwardBatch_Exception("Archive non trovato in GetDefaultAwardBatchModel");

            return new BiblosDS.Library.Common.Model.AwardBatch
            {
                IdAwardBatch = Guid.NewGuid(),
                IdArchive = idArchive,
                Name = GetAwardBatchName(archive.Name, fromDate, null),
                DateFrom = fromDate,
                IsAuto = isAuto,
                IsOpen = 1
            };
        }


        /// <summary>
        /// Ritorna il batch aperto per archivio. Se nessuno è aperto lo crea
        /// </summary>
        /// <param name="idArchive">Archivio</param>
        /// <param name="IsAuto">Indica se deve trattarsi dei lotto di versamento automatico</param>
        /// <returns></returns>
        private BiblosDS.Library.Common.Model.AwardBatch GetOpenAwardBatchModel(Guid idArchive, short isAuto)
        {
            try
            {
                var batch = db.AwardBatch
                  .Where(x => x.IdArchive == idArchive && x.IsAuto == isAuto && x.IsOpen == 1)
                  .FirstOrDefault();

                if (batch != null)
                    return batch;

                //inserisce un nuovo batch
                var fromDate = DateTime.Now;

                batch = GetDefaultAwardBatchModel(idArchive, fromDate, isAuto);
                db.AwardBatch.AddObject(batch);

                if (requireSave)
                    db.SaveChanges();

                return batch;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Dispose();
            }
        }


        /// <summary>
        /// Determina la stringa da assegnare al lotto di versamento
        /// </summary>
        /// <param name="fromDate">Data apertura lotto</param>
        /// <param name="toDate">Data chiusura del lotto - Quando viene aperto questa data è null</param>
        /// <returns></returns>
        private string GetAwardBatchName(string archiveName, DateTime fromDate, DateTime? toDate)
        {
            string res = String.Format("Pacchetto di versamento. '{0}' dal {1}", archiveName, fromDate.ToString("dd-MM-yyyy HH:mm"));
            if (toDate.HasValue)
                res += " al " + toDate.Value.ToString("dd-MM-yyyy HH:mm");

            return res;
        }

        public void UpdateAwardBatch(AwardBatch awardBatch)
        {
            try
            {
                Model.AwardBatch persistedModel = db.AwardBatch.First(x => x.IdAwardBatch == awardBatch.IdAwardBatch);
                persistedModel.IdPDVDocument = awardBatch.IdPDVDocument;
                persistedModel.IdRDVDocument = awardBatch.IdRDVDocument;
                persistedModel.IsRDVSigned = awardBatch.IsRDVSigned;
                if (requireSave)
                {
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public BindingList<AwardBatch> GetPreservationAwardBatches(Guid idPreservation)
        {
            try
            {
                BindingList<AwardBatch> results = new BindingList<AwardBatch>();
                var query = db.AwardBatch
                    .Where(x => x.Document.Any(d => d.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation) && d.IsVisible == 1));
                query.ToList().ForEach(f => results.Add(f.Convert()));
                return results;
            }
            finally
            {
                Dispose();
            }
        }
    }
}
