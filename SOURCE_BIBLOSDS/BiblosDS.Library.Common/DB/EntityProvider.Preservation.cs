using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Model;
using BiblosDS.Library.Common.Objects;
using Model = BiblosDS.Library.Common.Model;
using System.ComponentModel;
using System.Configuration;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Enums;
using System.Data.Objects.DataClasses;
using DataObjects = System.Data.Objects;
using System.Data;
using System.Transactions;
using System.Linq.Expressions;
using BiblosDS.Library.Common.Objects.Response;
using System.Data.Objects;
using System.Security.Principal;
using System.IO;
using Document = BiblosDS.Library.Common.Objects.Document;
using Preservation = BiblosDS.Library.Common.Objects.Preservation;
using PreservationAlert = BiblosDS.Library.Common.Objects.PreservationAlert;
using PreservationAlertType = BiblosDS.Library.Common.Objects.PreservationAlertType;
using PreservationInStorageDevice = BiblosDS.Library.Common.Objects.PreservationInStorageDevice;
using PreservationJournaling = BiblosDS.Library.Common.Objects.PreservationJournaling;
using PreservationJournalingActivity = BiblosDS.Library.Common.Objects.PreservationJournalingActivity;
using PreservationRole = BiblosDS.Library.Common.Objects.PreservationRole;
using PreservationSchedule = BiblosDS.Library.Common.Objects.PreservationSchedule;
using PreservationStorageDevice = BiblosDS.Library.Common.Objects.PreservationStorageDevice;
using PreservationStorageDeviceStatus = BiblosDS.Library.Common.Objects.PreservationStorageDeviceStatus;
using PreservationTask = BiblosDS.Library.Common.Objects.PreservationTask;
using PreservationTaskGroup = BiblosDS.Library.Common.Objects.PreservationTaskGroup;
using PreservationTaskGroupType = BiblosDS.Library.Common.Objects.PreservationTaskGroupType;
using PreservationTaskType = BiblosDS.Library.Common.Objects.PreservationTaskType;
using PreservationUser = BiblosDS.Library.Common.Objects.PreservationUser;
using PreservationUserRole = BiblosDS.Library.Common.Objects.PreservationUserRole;
using System.Data.SqlClient;
using BiblosDS.Library.Common.Utility;
using System.Data.Odbc;

namespace BiblosDS.Library.Common.DB
{
    public partial class EntityProvider
    {
        private const int DEFAULT_LEVEL = 0,
                DEFAULT_DEEP_LEVEL = 2;

        #region Getters

        /// <summary>
        /// Metodo di utilita': dato il nome dell'archivio, restituisce l'ID ad esso associato.
        /// </summary>
        /// <param name="archiveName">Nome dell'archivio di cui si desidera recuperare l'id.</param>
        /// <returns></returns>
        public Guid GetIdPreservationArchiveFromName(string archiveName)
        {
            var resultSet = this.db.Archive
                .Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals(archiveName, StringComparison.InvariantCultureIgnoreCase));

            if (resultSet.Count() > 1)
                new PreservationError("Archive Name is not unique.", PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();

            return resultSet.FirstOrDefault().IdArchive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public Guid GetIdPreservationRoleFromKeyCode(int keyCode)
        {
            Guid retval = Guid.Empty;

            var ret = this.db.PreservationRole
                .Where(x => x.KeyCode == keyCode)
                .SingleOrDefault();

            if (ret != null)
                retval = ret.IdPreservationRole;

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionName"></param>
        /// <returns></returns>
        public Guid GetIdPreservationExceptionFromDescription(string exceptionName)
        {
            Guid retval = Guid.Empty;

            if (!string.IsNullOrEmpty(exceptionName))
            {
                var ex = this.db.PreservationException
                    .Where(x => x.KeyName.Equals(exceptionName, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                if (ex != null)
                    retval = ex.IdPreservationException;
                else
                    new PreservationError("Prevista la configurazione dell'eccezione: " + exceptionName, PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();
            }

            return retval;
        }
        /// <summary>
        /// GetParametro
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPreservationParameter(Guid idArchive)
        {
            return this.GetPreservationParameter(idArchive, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idArchive"></param>
        /// <param name="admitNullReturnValue"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPreservationParameter(Guid idArchive, bool admitNullReturnValue)
        {
            Dictionary<string, string> retval = new Dictionary<string, string>();
            var parameter = this.db.PreservationParameters.Where(x => x.IdArchive == idArchive);
            foreach (var item in parameter)
            {
                retval.Add(item.Label, item.Value);
            }
            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="name"></param>
        /// <param name="admitNullReturnValue"></param>
        /// <returns></returns>
        public string GetPreservationParameter(Guid idArchive, string name, bool admitNullReturnValue)
        {
            return this.db.PreservationParameters.Where(x => (x.IdArchive == idArchive) && x.Label == name).Select(x => x.Value).FirstOrDefault();
        }
        /// <summary>
        /// Ritorna il valore (value) del primo parametro trovato avente etichetta (label) = "paramKey".
        /// </summary>
        /// <param name="paramKey"></param>
        /// <returns></returns>
        public string GetFirstPreservationParameter(string paramKey)
        {
            var ret = db.PreservationParameters.Where(x => (x.Label ?? string.Empty).Equals(paramKey, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
            return (ret == null) ? string.Empty : ret.Value ?? string.Empty;
        }

        /// <summary>
        /// GetUserRoleInArchive
        /// </summary>
        /// <param name="domainUserName"></param>
        /// <param name="idArchive">In realta' non usato -> si sfrutta il parametro "TipoArchivio" per ottenere l'archivio associato.</param>
        /// <returns></returns>
        public BindingList<PreservationUserRole> GetUserRoleInArchive(string domainUserName, Guid idArchive)
        {
            var retval = new BindingList<PreservationUserRole>();

            if (!string.IsNullOrEmpty(domainUserName))
            {
                var results = from users in this.db.PreservationUser

                              join userRoles in this.db.PreservationUserRole
                                on users.IdPreservationUser equals userRoles.IdPreservationUser

                              join roles in this.db.PreservationRole
                                on userRoles.IdPreservationRole equals roles.IdPreservationRole

                              where (users.DomainUser ?? string.Empty).Equals(domainUserName, StringComparison.InvariantCultureIgnoreCase)

                              select new
                              {
                                  RoleId = roles.IdPreservationRole,
                                  UserId = users.IdPreservationUser,
                                  RoleName = roles.Name,
                              };

                string archName = this.GetPreservationParameter(idArchive, "TipoArchivio", false);

                PreservationUserRole toAdd;

                foreach (var item in results)
                {
                    toAdd = new PreservationUserRole
                    {
                        Archive = this.db.Archive
                        .Include(x => x.Preservation)
                        .Where(x => x.IdArchive == idArchive)
                        .FirstOrDefault()
                        .Convert(),
                        PreservationRole = this.db.PreservationRole.Where(x => x.IdPreservationRole == item.RoleId).FirstOrDefault().Convert(),
                        PreservationUser = this.db.PreservationUser.Where(x => x.IdPreservationUser == item.UserId).SingleOrDefault().Convert(),
                    };

                    if (toAdd.Archive != null)
                        toAdd.Archive.Name = archName ?? string.Empty;

                    retval.Add(toAdd);
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BindingList<Preservation> GetPreservationsToSign(Guid idArchive, int take, int skip, out int totalPreservations)
        {
            var retval = new BindingList<Preservation>();

            var set = this.db.Preservation
                .Include(x => x.Archive)
                .Include(x => x.PreservationTaskGroup)
                .Include(x => x.PreservationTask)
                .Include(x => x.PreservationUser)
                .Where(x => x.IdArchive == idArchive && x.PreservationTask != null && !x.PreservationTask.ExecutedDate.HasValue).OrderByDescending(x => x.PreservationDate);
            //.Include(x => x.PreservationJournaling)
            //.Include(x => x.PreservationJournaling.First().PreservationJournalingActivity);

            totalPreservations = set.Count();

            if (skip < 0)
                skip = 0;

            if (take < 1)
                take = 1;

            Preservation pres;
            IQueryable<Model.Preservation> preservations = set.OrderBy(x => x.CloseDate).Skip(skip).Take(take);

            foreach (var item in preservations)
            {
                pres = item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, typeof(PreservationJournaling), typeof(PreservationTask), typeof(Document));

                //if (pres != null)
                //{
                //    pres.IsPreservationInStorageDevice = db.PreservationInStorageDevice.Any(x => x.IdPreservation == pres.IdPreservation);
                //}

                retval.Add(pres);
            }

            return retval;
        }

        public void RemovePreservation(Guid idArchive, Guid idPreservation)
        {
            try
            {
                var parameters = new[]
{
new SqlParameter("@1", idPreservation),
new SqlParameter("@2", idArchive)
};

                db.ExecuteStoreCommand("sp_RemovePreservation @1, @2", parameters);
                db.SaveChanges();
                //var documents = db.Document.Where(x => x.IdPreservation == idPreservation);
                //foreach (var item in documents)
                //{
                //    item.IdPreservation = null;
                //    item.IsConservated = 0;
                //}


                //var journaling = db.PreservationJournaling.Where(x => x.IdPreservation == idPreservation);
                //foreach (var item in journaling)
                //{
                //    db.DeleteObject(item);   
                //}                

                //var tasks = db.PreservationTask.Where(x => x.IdArchive == idArchive && !x.IdPreservation.HasValue);
                //foreach (var item in tasks)
                //{
                //    db.DeleteObject(item);
                //}
                //tasks = db.PreservationTask.Where(x => x.IdArchive == idArchive && x.IdPreservation == idPreservation);
                //foreach (var item in tasks)
                //{
                //    db.DeleteObject(item);
                //}


                //var storaeDevices = db.PreservationInStorageDevice.Where(x => x.IdPreservation == idPreservation);
                //List<Guid> idSd = new List<Guid>();
                //foreach (var item in storaeDevices)
                //{
                //    idSd.Add(item.IdPreservationStorageDevice);
                //    db.DeleteObject(item);
                //}

                //foreach (var item in idSd)
                //{
                //    var storage = db.PreservationStorageDevice.Where(x => x.IdPreservationStorageDevice == item).FirstOrDefault();
                //    if (storage != null)
                //        db.DeleteObject(storage);
                //}

                //var preservation = db.Preservation.Where(x => x.IdPreservation == idPreservation).FirstOrDefault();
                //if (preservation != null)
                //    db.DeleteObject(preservation);

                //db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BindingList<Preservation> GetPreservations(Guid idArchive, int take, int skip, out int totalPreservations)
        {
            return GetPreservations(idArchive, take, skip, true, true, out totalPreservations);
        }

        public BindingList<Preservation> GetPreservations(Guid idArchive, int take, int skip, bool includeReleatedEntities, bool includeCloseContent, out int totalPreservations)
        {
            BindingList<Preservation> retval = new BindingList<Preservation>();
            IQueryable<Model.Preservation> query = this.db.Preservation.Where(x => x.IdArchive == idArchive);
            totalPreservations = query.Count();

            if (skip < 0) skip = 0;
            if (take < 1) take = 1;

            IQueryable<Model.PreservationTableValuedResult> preservations = this.db.Preservations_FX_SearchPreservations(includeCloseContent, includeReleatedEntities, idArchive, skip, take);
            preservations.ToList().ForEach(f => retval.Add(f.Convert()));
            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<Preservation> GetPreservationsForSuperMarkFromArchive(Guid idArchive)
        {
            var retval = new BindingList<Preservation>();
            var preservations = this.db.Preservation.Where(x => x.IdArchive == idArchive).ToList();

            preservations.ForEach(x => retval.Add(x.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, typeof(Document), typeof(PreservationTaskGroup), typeof(DocumentArchive), typeof(PreservationTask), typeof(PreservationUser), typeof(PreservationJournaling))));

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <returns></returns>
        public Preservation GetPreservation(Guid idPreservation, bool fillAll = true)
        {
            if (idPreservation == Guid.Empty)
                return null;

            var query = this.db.Preservation
                .Include(x => x.Archive)
                .Include(x => x.PreservationTaskGroup)
                .Include(x => x.PreservationTask)
                .Include(x => x.PreservationUser);
            if (fillAll)
            {
                query = query.Include(x => x.PreservationDocuments.First().Document.AttributesValue.First().Attributes);
            }
            var ret = query.Where(x => x.IdPreservation == idPreservation)
                .SingleOrDefault();

            List<Type> itemToFill = new List<Type>();
            if (!fillAll)
                itemToFill.Add(typeof(Document));
            return ret != null ? ret.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, itemToFill.ToArray()) : null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<Preservation> GetPreservationsFromArchive(Guid idArchive, int take, int skip, out long totaiItems)
        {
            totaiItems = 0;
            if (idArchive == Guid.Empty)
                return null;

            var retval = new BindingList<Preservation>();

            var set = this.db.Preservation
                .Include(x => x.Archive)
                .Include(x => x.PreservationTaskGroup)
                .Include(x => x.PreservationTask)
                .Include(x => x.PreservationUser)
                //.Include(x => x.PreservationJournaling)
                //.Include(x => x.PreservationJournaling.First().PreservationJournalingActivity)
                .Where(x => x.IdArchive == idArchive).OrderByDescending(x => x.StartDate);

            totaiItems = set.Count();

            foreach (var item in set.Skip(skip).Take(take))
            {
                retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, typeof(PreservationJournaling), typeof(Document)));
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <returns></returns>
        public Document GetLastDocumentPreservation(Guid idPreservation)
        {
            if (idPreservation == Guid.Empty)
            {
                return null;
            }

            IQueryable<Model.Document> query = this.db.Document
                .Include(x => x.AttributesValue)
                .Include(x => x.AttributesValue.Single().Attributes)
                .Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation) && x.IsVisible == 1)
                .OrderByDescending(x => x.PrimaryKeyValue);

            Model.Document doc = query.FirstOrDefault();
            if (doc == null)
            {
                return null;
            }

            return new Document()
            {
                AttributeValues = doc.AttributesValue.Convert(),
            };
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="idArchive"></param>
        ///// <returns></returns>
        //public BindingList<PreservationTaskGroup> GetTaskGroupsFromArchive(Guid idArchive)
        //{
        //    BindingList<PreservationTaskGroup> retval = new BindingList<PreservationTaskGroup>();

        //    if (idArchive != Guid.Empty)
        //    {
        //        var pres = this.GetPreservationsFromArchive(idArchive);
        //        IQueryable<Model.PreservationTaskGroup> set;

        //        foreach (var p in pres)
        //        {
        //            set = this.db.PreservationTaskGroup.Where(x => x.IdPreservationTaskGroup == p.IdPreservationTaskGroup);
        //            foreach (var item in set)
        //            {
        //                retval.Add(item.Convert());
        //            }
        //        }
        //    }

        //    return retval;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public bool GetIfExistDocumentNotPreservedFromArchive(Guid idArchive)
        {
            return this.db.Document
                .Where(x => !x.PreservationDocuments.Any() && x.IsVisible == 1 && x.IdArchive == idArchive && x.IdParentBiblos.HasValue).Count() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<Document> GetDocumentNotPreservedFromArchive(Guid idArchive)
        {
            BindingList<Document> retval = new BindingList<Document>();

            //string sCommand = @"SELECT Count(*) AS DOCUMENTI"
            //                      + " FROM Oggetto O LEFT JOIN Oggetto_Conservazione OC ON O.IdOggetto = OC.IdOggetto"
            //                      + " WHERE OC.IdConservazione IS NULL AND O.IdOggettoPadre <> 0"; 

            var set = this.db.Document
                .Include(x => x.PreservationDocuments)
                .Where(x => !x.PreservationDocuments.Any() && x.IsVisible == 1 && x.IdArchive == idArchive && x.IdParentBiblos.HasValue);

            foreach (var item in set)
                retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, null));

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <returns></returns>
        public BindingList<Document> GetDocumentsFromPreservation(Guid idPreservation)
        {
            var retval = new BindingList<Document>();

            var set = this.db.Document
                .Include(x => x.PreservationDocuments)
                .Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation) && x.IsVisible == 1);

            foreach (var item in set)
                retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, null));

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="domainUser"></param>
        /// <returns></returns>
        public PreservationUser GetPreservationUserForArchive(Guid idArchive, string domainUser)
        {
            PreservationUser retval = null;

            if (idArchive != Guid.Empty && !string.IsNullOrEmpty(domainUser))
            {
                var usr = this.db.Preservation
                    .Include(x => x.PreservationUser)
                    .Where(x => x.IdArchive == idArchive && x.PreservationUser != null
                        && (x.PreservationUser.DomainUser ?? string.Empty).Equals(domainUser, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.PreservationUser)
                    .FirstOrDefault();

                if (usr != null)
                    retval = usr.Convert();
            }

            return retval;
        }

        /// <summary>
        /// Ritorna l'elenco di TUTTI gli attributi presenti in banca dati.
        /// </summary>
        /// <returns></returns>
        public BindingList<DocumentAttribute> GetAttributes(Guid idArchive)
        {
            var retval = new BindingList<DocumentAttribute>();

            //    string sCmd = "SELECT Nome, Obbligatorio, Disabilitato, TipoCampo, ProgressivoIntero, DataPrincipale,"
            //+ "       PosizioneInChiaveUnivoca, PorzioneInChiaveUnivoca, FormatoInChiaveUnivoca,"
            //+ "       PosizioneInFileChiusura, Validazione, Formato"
            //+ "  FROM AttributoOggetto"
            //+ " ORDER BY PosizioneInChiaveUnivoca";

            var set = this.db.Attributes
                .Where(x => x.IdArchive == idArchive)
                .OrderBy(x => x.KeyOrder);

            foreach (var item in set)
            {
                retval.Add(item.Convert());
            }

            return retval;
        }
        /// <summary>
        /// Ritorna la lista dei documenti che hanno una conservazione associata.
        /// </summary>
        /// <returns></returns>
        public BindingList<Document> GetPreservedDocuments(Guid idPreservation)
        {
            //string sCmd = "Select COUNT(*) FROM Oggetto_Conservazione oc, Oggetto o"
            //    + " WHERE oc.IdConservazione=" + nConservazione.ToString()
            //    + " AND oc.IdOggetto=o.idOggetto";

            var retval = new BindingList<Document>();

            this.db.Document
                .Include(x => x.PreservationDocuments.Single().Preservation)
                .Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation))
                .ToList()
                .ForEach(x => retval.Add(x.Convert(DEFAULT_LEVEL, 1, null)));

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationHoliday"></param>
        /// <returns></returns>
        public BindingList<PreservationHoliday> GetPreservationHolidays(Nullable<Guid> idPreservationHoliday)
        {
            BindingList<PreservationHoliday> retval = new BindingList<PreservationHoliday>();
            IQueryable<Model.PreservationHolidays> query = null;

            if (idPreservationHoliday.HasValue && idPreservationHoliday.Value != Guid.Empty)
            {
                query = this.db.PreservationHolidays
                    .Where(x => x.IdPreservationHolidays == idPreservationHoliday.Value);
            }
            else
            {
                query = this.db.PreservationHolidays;
            }

            if (query != null)
            {
                foreach (var item in query)
                {
                    retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <returns></returns>
        public BindingList<DocumentAttribute> GetAttributeByPreservationPosition(Guid idArchive)
        {
            BindingList<DocumentAttribute> retval = new BindingList<DocumentAttribute>();


            IOrderedQueryable<Model.Attributes> set;


            set = this.db.Attributes
                    .Where(x => x.IdArchive == idArchive
                        && x.ConservationPosition.HasValue && x.ConservationPosition.Value > 0)
                    .OrderBy(x => x.ConservationPosition);

            foreach (var item in set)
            {
                retval.Add(item.Convert(0, 1));
            }
            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <returns></returns>
        public BindingList<Document> GetDocumentsInPreservation(Guid idPreservation, int take, int skip, out int totalDocuments)
        {
            BindingList<Document> retval = new BindingList<Document>();

            var set = this.db.Document
                .Include(x => x.AttributesValue)
                .Include(x => x.AttributesValue.First().Attributes)
                .Include(x => x.PreservationDocuments.Single().Preservation)
                .Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation));

            totalDocuments = set.Count();

            foreach (var item in set.OrderBy(x => x.DateCreated).Skip(skip).Take(take))
            {
                retval.Add(item.Convert(0, 3, null));
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <returns></returns>
        public BindingList<DocumentAttributeValue> GetAttributeValuesFromPreservation(Guid idPreservation)
        {
            BindingList<DocumentAttributeValue> retval = new BindingList<DocumentAttributeValue>();

            var set = this.db.AttributesValue
                .Include(x => x.Attributes)
                .Include(x => x.Document)
                .Where(x => x.Document.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation));

            foreach (var item in set)
            {
                retval.Add(item.Convert());
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationTaskGroup"></param>
        /// <returns></returns>
        public BindingList<PreservationTaskGroup> GetTaskGroupNotClosed(Guid idArchive, Guid? idUser)
        {
            BindingList<PreservationTaskGroup> retval = new BindingList<PreservationTaskGroup>();
            IQueryable<Model.PreservationTaskGroup> set = null;

            set = this.db.PreservationTaskGroup
                    .Include(x => x.Preservation)
                    .Include(x => x.PreservationUser)
                    .Include(x => x.PreservationSchedule)
                    .Include(x => x.PreservationTaskGroupType)
                    .Where(x => !x.Closed.HasValue
                        && x.IdArchive == idArchive
                        && x.Preservation.Count() < 1);

            if (idUser.HasValue && idUser.Value != Guid.Empty)
            {
                set = set
                    .Where(x => x.IdPreservationUser == idUser.Value);
            }

            foreach (var item in set)
            {
                retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationTaskGroup"></param>
        /// <returns></returns>
        public BindingList<PreservationTaskGroup> GetTaskGroup(Nullable<Guid> idPreservationTaskGroup = null)
        {
            BindingList<PreservationTaskGroup> retval = new BindingList<PreservationTaskGroup>();
            IQueryable<Model.PreservationTaskGroup> set = null;

            if (idPreservationTaskGroup.HasValue)
            {
                if (idPreservationTaskGroup.Value != Guid.Empty)
                {
                    set = this.db.PreservationTaskGroup
                        .Include(x => x.PreservationUser)
                        .Include(x => x.PreservationSchedule)
                        .Include(x => x.PreservationTaskGroupType)
                        .Where(x => x.IdPreservationTaskGroup == idPreservationTaskGroup);
                }
            }
            else
            {
                set = this.db.PreservationTaskGroup
                        .Include(x => x.PreservationUser)
                        .Include(x => x.PreservationSchedule)
                        .Include(x => x.PreservationTaskGroupType);
            }

            if (set != null)
            {
                foreach (var item in set)
                {
                    retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAlert"></param>
        /// <param name="idTaskType"></param>
        /// <param name="idAlertType"></param>
        /// <param name="idTask"></param>
        /// <param name="orderByOffset"></param>
        /// <returns></returns>
        public BindingList<PreservationAlert> GetPreservationAlert(Nullable<Guid> idAlert, Nullable<Guid> idTaskType, Nullable<Guid> idAlertType, Nullable<Guid> idTask, bool orderByOffset = false)
        {
            BindingList<PreservationAlert> retval = new BindingList<PreservationAlert>();
            IQueryable<Model.PreservationAlert> set = null;

            set = this.db.PreservationAlert
                .Include(x => x.PreservationAlertType)
                .Include(x => x.PreservationAlertType.PreservationRole)
                .Include(x => x.PreservationAlertType.PreservationAlertTask)
                .Include(x => x.PreservationAlertType.PreservationAlertTask.First().PreservationTaskType)
                .Include(x => x.PreservationTask)
                .Where(x => x.IdPreservationAlert == ((idAlert.HasValue && idAlert.Value != Guid.Empty) ? idAlert.Value : x.IdPreservationAlert)
                    && ((idTaskType.HasValue && idTaskType.Value != Guid.Empty) ? x.PreservationTask.IdPreservationTaskType == idTaskType.Value : true)
                    && ((idTask.HasValue && idTask.Value != Guid.Empty) ? x.PreservationTask.IdPreservationTask == idTask.Value : true)
                    && ((idAlertType.HasValue && idAlertType.Value != Guid.Empty) ? x.PreservationAlertType.IdPreservationAlertType == idAlertType.Value : true));

            if (orderByOffset && set.Any(x => x.PreservationAlertType != null))
            {
                set = set.OrderBy(x => x.PreservationAlertType.Offset).AsQueryable();
            }

            if (set != null)
            {
                foreach (var item in set)
                {
                    retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }
            /*
            #region CASO 1: "idTaskType" e "idAlertType".


            if (idTaskType.HasValue && idAlertType.HasValue && idTask.HasValue && idTaskType.Value != Guid.Empty && idAlertType.Value != Guid.Empty && idTask.Value != Guid.Empty)
            {
                set = this.db.PreservationAlert
                    .Include(x => x.PreservationAlertType)
                    .Include(x => x.PreservationAlertType.PreservationRole)
                    .Include(x => x.PreservationTask)
                    .Include(x => x.PreservationTask.PreservationTaskType)
                    .Where(x => x.IdPreservationAlertType == idAlertType.Value
                        && x.PreservationTaskType
                            .Any(y => y.IdPreservationTaskType == idTaskType.Value)
                        && x.PreservationAlert != null && x.PreservationAlert
                                .Any(y => y.IdPreservationTask == idTask));
            }

            #endregion
            #region CASO 2: solo "idAlertType".

            else if (!idTaskType.HasValue && idAlertType.HasValue && idAlertType.Value != Guid.Empty)
            {
                set = this.db.PreservationAlertType
                    .Include(x => x.PreservationTaskType)
                    .Include(x => x.PreservationRole)
                    .Where(x => x.IdPreservationAlertType == idAlertType.Value);
            }

            #endregion
            #region CASO 3: solo "idTaskType".

            else if (!idAlertType.HasValue && idTaskType.HasValue && idTaskType.Value != Guid.Empty)
            {
                set = this.db.PreservationAlertType
                    .Include(x => x.PreservationTaskType)
                    .Include(x => x.PreservationRole)
                    .Where(x => x.PreservationTaskType.Any(y => y.IdPreservationTaskType == idTaskType.Value));
            }

            #endregion
            #region CASO 4: niente di niente.

            else if (!idTaskType.HasValue && !idAlertType.HasValue)
            {
                set = this.db.PreservationAlertType
                    .Include(x => x.PreservationTaskType)
                    .Include(x => x.PreservationRole);
            }

            #endregion
            #region ORDINAMENTO

            if (orderByOffset)
            {
                set = set.OrderBy(x => x.Offset).AsQueryable();
            }

            #endregion

            PreservationAlertType toAdd;

            foreach (var item in set)
            {
                toAdd = item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

                if (idTaskType.HasValue && idTaskType.Value != Guid.Empty)
                {
                    for (int i = 0; i < toAdd.TaskTypes.Count; )
                    {
                        if (toAdd.TaskTypes.ElementAt(i).IdPreservationTaskType != idTaskType.Value)
                            toAdd.TaskTypes.RemoveAt(i);
                        else
                            i++;
                    }
                }

                if (idTask.HasValue && idTask.Value != Guid.Empty)
                {
                    for (int i = 0; i < toAdd.TaskTypes.Count; )
                    {
                        if (toAdd.TaskTypes.ElementAt(i).IdPreservationTaskType != idTaskType.Value)
                            toAdd.TaskTypes.RemoveAt(i);
                        else
                            i++;
                    }
                }

                retval.Add(toAdd);
            }
            */

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaskGroupType"></param>
        /// <returns></returns>
        public BindingList<PreservationTaskGroupType> GetTaskGroupTypes(Nullable<Guid> idTaskGroupType)
        {
            var retval = new BindingList<PreservationTaskGroupType>();
            IQueryable<Model.PreservationTaskGroupType> set = null;

            if (idTaskGroupType.HasValue)
            {
                if (idTaskGroupType.Value != Guid.Empty)
                {
                    set = this.db.PreservationTaskGroupType
                        .Include(x => x.PreservationTaskGroup)
                        .Where(x => x.IdPreservationTaskGroupType == idTaskGroupType.Value);
                }
            }
            else
            {
                set = this.db.PreservationTaskGroupType
                        .Include(x => x.PreservationTaskGroup);
            }

            if (set != null)
            {
                foreach (var item in set)
                {
                    retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public BindingList<PreservationUser> GetPreservationUser(Nullable<Guid> idUser, Guid idArchive)
        {
            BindingList<PreservationUser> retval = new BindingList<PreservationUser>();
            IQueryable<Model.PreservationUser> set = null;

            if (!idUser.HasValue)
            {
                set = this.db.PreservationUser;
            }
            else if (idUser.Value != Guid.Empty)
            {
                set = this.db.PreservationUser
                    .Include(x => x.Preservation)
                    //.Include(x => x.PreservationTask)
                    //.Include(x => x.PreservationTaskGroup)
                    .Include(x => x.PreservationUserRole)
                    .Include(x => x.PreservationUserRole.First().Archive)
                    .Include(x => x.PreservationUserRole.First().PreservationRole)
                    .Where(x => x.IdPreservationUser == idUser.Value);
            }

            //string sql = ((ObjectQuery)set).ToTraceString(); 

            if (set != null)
            {
                foreach (var item in set)
                {
                    var user = item.Convert(0, 1);
                    user.UserRoles = item.PreservationUserRole.Where(x => x.IdArchive == idArchive).Convert();
                    retval.Add(user);
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationTaskType"></param>
        /// <returns></returns>
        public BindingList<PreservationTaskType> GetPreservationTaskTypes(Nullable<Guid> idPreservationTaskType)
        {
            IQueryable<Model.PreservationTaskType> query = null;
            var retval = new BindingList<PreservationTaskType>();

            if (idPreservationTaskType.HasValue)
            {
                if (idPreservationTaskType.Value != Guid.Empty)
                {
                    query = this.db.PreservationTaskType
                        .Include(x => x.PreservationTaskRole)
                        .Include(x => x.PreservationTaskRole.First().PreservationRole)
                        .Where(x => x.IdPreservationTaskType == idPreservationTaskType);
                }
            }
            else
            {
                query = this.db.PreservationTaskType
                        .Include(x => x.PreservationTaskRole)
                        .Include(x => x.PreservationTaskRole.First().PreservationRole)
                    .AsQueryable();
            }

            if (query != null)
            {
                foreach (var taskType in query)
                {
                    retval.Add(taskType.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationSchedule"></param>
        /// <returns></returns>
        public BindingList<PreservationTaskType> GetPreservationTaskTypesAndPreservationScheduleTaskTypes(Nullable<Guid> idPreservationSchedule, Guid idArchive)
        {
            var retval = new BindingList<PreservationTaskType>();

            /*
                 ex query sql 
                 declare @IdScadenziario as int
set @IdScadenziario = 3

SELECT TipoTask.IdTipoTask, 
(CASE WHEN (SELECT Offset FROM Scadenziario_TipoTask WHERE IdScadenziario = @IdScadenziario AND IdTipoTask = TipoTask.IdTipoTask) IS NULL THEN 0 ELSE 1 END) as Associato, 
Descrizione, 
(ISNULL((SELECT Offset FROM Scadenziario_TipoTask WHERE IdScadenziario = @IdScadenziario AND IdTipoTask = TipoTask.IdTipoTask), 0)) as Offset, 
IdRuoloEsecutore 
FROM TipoTask 
ORDER BY Descrizione

                 * */
            //var query = from p in db.PreservationTaskType                  
            //        join s in db.PreservationSchedule_TaskType on p.IdPreservationTaskType equals s.IdPreservationTaskType into SchedukeTaskType
            //        from sched in SchedukeTaskType.Where(x => !idPreservationSchedule.HasValue || x.IdPreservationSchedule == idPreservationSchedule.Value).DefaultIfEmpty()                    
            //        select p;
            var query = this.db.PreservationTaskType
                    .Include(x => x.PreservationTaskRole)
                    .Include(x => x.PreservationTaskRole.First().PreservationRole)
                    .Include(x => x.PreservationSchedule_TaskType)
                    .Include(x => x.PreservationSchedule_TaskType.First().PreservationSchedule)
                    .Include(x => x.PreservationSchedule_TaskType.First().PreservationTaskType)
                    .Include(x => x.PreservationSchedule_TaskType.First().PreservationTaskType.PreservationTask);
            //if (idPreservationSchedule.HasValue && idPreservationSchedule.Value != Guid.Empty)
            //{                
            //    query = this.db.PreservationTaskType
            //        .Include(x => x.PreservationTaskRole)
            //        .Include(x => x.PreservationTaskRole.First().PreservationRole)
            //        .Include(x => x.PreservationSchedule_TaskType)
            //        .Include(x => x.PreservationSchedule_TaskType.First().PreservationSchedule)
            //        .Include(x => x.PreservationSchedule_TaskType.First().PreservationTaskType)
            //        .Include(x => x.PreservationSchedule_TaskType.First().PreservationTaskType.PreservationTask)
            //        .Where(x => x.PreservationSchedule_TaskType.Count() <= 0 || (x.PreservationSchedule_TaskType
            //                        .All(y => y.IdPreservationSchedule == idPreservationSchedule.Value && y.PreservationTaskType.PreservationTask
            //                            .All(z => z.IdArchive == idArchive))));
            //}
            //else
            //{
            //    query = this.db.PreservationTaskType
            //            .Include(x => x.PreservationTaskRole)
            //            .Include(x => x.PreservationTaskRole.First().PreservationRole)
            //            .Include(x => x.PreservationSchedule_TaskType)
            //            .Include(x => x.PreservationSchedule_TaskType.First().PreservationSchedule)
            //            .Include(x => x.PreservationSchedule_TaskType.First().PreservationTaskType)
            //            .Include(x => x.PreservationSchedule_TaskType.First().PreservationTaskType.PreservationTask)
            //            .Where(x => x.PreservationSchedule_TaskType
            //                .All(y => y.PreservationTaskType.PreservationTask
            //                    .All(z => z.IdArchive == idArchive)));
            //}

            if (query != null)
            {
                foreach (var item in query)
                {
                    retval.Add(item.Convert(0, 2));
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <returns></returns>
        public BindingList<PreservationTask> GetTasksFromTaskGroup(Guid idTaskGroup)
        {
            BindingList<PreservationTask> retval = new BindingList<PreservationTask>();

            var set = this.db.PreservationTask
                .Where(x => x.IdPreservationTaskGroup == idTaskGroup);

            foreach (var item in set)
            {
                retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return retval;
        }

        /// <summary>
        /// Ritorna la lista degli archivi che hanno almeno una conservazione sostitutiva associata.
        /// </summary>
        /// <returns>Lista archivi con conservazione.</returns>
        public BindingList<DocumentArchive> GetArchivesWithPreservations()
        {
            BindingList<DocumentArchive> ret = new BindingList<DocumentArchive>();

            var query = this.db.Archive
                .Include(x => x.Preservation)
                .Where(x => x.Preservation.Count > 0)
                .OrderBy(x => x.Name);

            foreach (var arch in query)
            {
                ret.Add(arch.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<PreservationUser> GetPreservationUsersInArchiveByRole(Guid idRole, Guid idArchive)
        {
            //string sCommand = "SELECT S.* FROM Soggetto S "
            //            + " INNER JOIN Ruolo_Soggetto RS ON S.IdSoggetto = RS.IdSoggetto"
            //            + " INNER JOIN Ruolo R ON R.IdRuolo = RS.IdRuolo"
            //            + " WHERE R.IdRuolo = " + m_Id.ToString();

            BindingList<PreservationUser> retval = new BindingList<PreservationUser>();

            var query = this.db.PreservationUser
                .Where(x => x.PreservationUserRole
                    .Any(y => y.IdArchive == idArchive && y.IdPreservationRole == idRole));

            foreach (var usr in query)
            {
                retval.Add(usr.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        public BindingList<PreservationRole> GetRoles(Guid? idRole)
        {
            IQueryable<Model.PreservationRole> query = null;
            BindingList<PreservationRole> retval = new BindingList<PreservationRole>();

            if (idRole.HasValue)
            {
                if (idRole.Value != Guid.Empty)
                {
                    query = this.db.PreservationRole
                        .Where(x => x.IdPreservationRole == idRole.Value);
                }
            }
            else
            {
                query = this.db.PreservationRole.AsQueryable();
            }

            if (query != null)
            {
                foreach (var role in query)
                {
                    retval.Add(role.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationUserRole"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<PreservationTaskType> GetTaskTypesByUserRole(Nullable<Guid> idPreservationUserRole, Guid idArchive)
        {
            IQueryable<Model.PreservationTaskType> query = null;
            BindingList<PreservationTaskType> retval = new BindingList<PreservationTaskType>();

            if (idPreservationUserRole.HasValue && idPreservationUserRole.Value != Guid.Empty)
            {
                query = this.db.PreservationTaskType
                    .Include(x => x.PreservationTaskRole)
                    .Include(x => x.PreservationTaskRole.First().PreservationRole)
                    .Include(x => x.PreservationTaskRole.First().PreservationRole.PreservationUserRole)
                    .Include(x => x.PreservationTaskRole.First().PreservationRole.PreservationUserRole.First().PreservationUser)
                    .Where(x => x.PreservationTaskRole
                        .All(y => y.PreservationRole.PreservationUserRole
                            .All(w => w.IdPreservationUserRole == idPreservationUserRole.Value && w.IdArchive == idArchive)));
            }
            else
            {
                query = this.db.PreservationTaskType
                    .Include(x => x.PreservationTaskRole)
                    .Include(x => x.PreservationTaskRole.First().PreservationRole)
                    .Include(x => x.PreservationTaskRole.First().PreservationRole.PreservationUserRole)
                    .Include(x => x.PreservationTaskRole.First().PreservationRole.PreservationUserRole.First().PreservationUser)
                    .Where(x => x.PreservationTaskRole
                        .All(y => y.PreservationRole.PreservationUserRole
                            .All(z => z.IdArchive == idArchive)));
            }

            if (query != null)
            {
                foreach (var item in query)
                {
                    retval.Add(item.Convert(0, 1));
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <param name="idArchive"></param>
        /// <param name="maxReturnedValues"></param>
        /// <returns></returns>
        public BindingList<PreservationTaskGroup> GetDetailedPreservationTaskGroup(Nullable<Guid> idTaskGroup, Nullable<Guid> idArchive, int maxReturnedValues)
        {
            BindingList<PreservationTaskGroup> retval = new BindingList<PreservationTaskGroup>();
            IQueryable<Model.PreservationTaskGroup> query = null;

            //            string qry = @"	SELECT 
            //                CONVERT(VARCHAR(10),IdTask) AS [Id],
            //								Nome AS [Nome Gruppo Task], 
            //								CONVERT(VARCHAR(10), Scadenza, 105) AS [Scadenza Gruppo Task], 
            //								TTDesc AS [Tipo Task], 
            //								CONVERT(VARCHAR(10), DataScadenza, 105) AS [Scadenza Task], 
            //								TestoAvviso AS [Testo Avviso], 
            //								CONVERT(VARCHAR(10), DataAvviso, 105) AS [Scadenza Avviso]
            //							FROM 
            //							(
            //								SELECT TOP 100
            //                  Task.IdTask,
            //									GruppoTask.Nome, 
            //									GruppoTask.Scadenza, 
            //									TipoTask.Descrizione AS TTDesc, 
            //									Task.DataScadenza, 
            //									TipoAvviso.TestoAvviso, 
            //									Avviso.DataAvviso
            //								FROM GruppoTask 
            //								INNER JOIN TipoGruppoTask ON GruppoTask.IdTipoGruppoTask = TipoGruppoTask.IdTipoGruppoTask 
            //								INNER JOIN Task ON GruppoTask.IdGruppoTask = Task.IdGruppoTask 
            //								INNER JOIN TipoTask ON Task.IdTipoTask = TipoTask.IdTipoTask 
            //								INNER JOIN Avviso ON Task.IdTask = Avviso.IdTask 
            //								INNER JOIN TipoAvviso ON Avviso.IdTipoAvviso = TipoAvviso.IdTipoAvviso
            //								ORDER BY GruppoTask.Scadenza DESC, Task.DataScadenza DESC, Avviso.DataAvviso DESC
            //							) AS InnerTable";

            if (idTaskGroup.HasValue && idTaskGroup.Value != Guid.Empty) //Filtra per ID gruppo task.
            {
                if (idArchive.HasValue && idArchive.Value != Guid.Empty) //Filtra per ID archivio.
                {
                    query = this.db.PreservationTaskGroup
                            .Include(x => x.PreservationTask)
                            .Include(x => x.PreservationTask.First().PreservationTaskType)
                            .Include(x => x.PreservationTask.First().PreservationAlert)
                            .Include(x => x.PreservationTask.First().PreservationAlert.First().PreservationAlertType)
                            .Include(x => x.PreservationTaskGroupType)
                            .Where(x => x.IdPreservationTaskGroup == idTaskGroup.Value && x.PreservationTask
                                .All(y => y.IdArchive == idArchive.Value));
                }
                else //Lista completa senza tener conto dell'archivio. 
                {
                    query = this.db.PreservationTaskGroup
                              .Include(x => x.PreservationTask)
                              .Include(x => x.PreservationTask.First().PreservationTaskType)
                              .Include(x => x.PreservationTask.First().PreservationAlert)
                              .Include(x => x.PreservationTask.First().PreservationAlert.First().PreservationAlertType)
                              .Include(x => x.PreservationTaskGroupType)
                              .Where(x => x.IdPreservationTaskGroup == idTaskGroup.Value);
                }
            }
            else
            {
                if (maxReturnedValues < 1) //Lista completa senza limite di righe.
                {
                    if (idArchive.HasValue && idArchive.Value != Guid.Empty) //Filtra per ID archivio.
                    {
                        query = this.db.PreservationTaskGroup
                            .Include(x => x.PreservationTask)
                            .Include(x => x.PreservationTask.First().PreservationTaskType)
                            .Include(x => x.PreservationTask.First().PreservationAlert)
                            .Include(x => x.PreservationTask.First().PreservationAlert.First().PreservationAlertType)
                            .Include(x => x.PreservationTaskGroupType)
                            .Where(x => x.PreservationTask
                                .All(y => y.IdArchive == idArchive.Value))
                                .OrderByDescending(x => x.Expiry);
                    }
                    else //Lista completa senza tener conto dell'archivio. 
                    {
                        query = this.db.PreservationTaskGroup
                             .Include(x => x.PreservationTask)
                             .Include(x => x.PreservationTask.First().PreservationTaskType)
                             .Include(x => x.PreservationTask.First().PreservationAlert)
                             .Include(x => x.PreservationTask.First().PreservationAlert.First().PreservationAlertType)
                             .Include(x => x.PreservationTaskGroupType)
                             .OrderByDescending(x => x.Expiry);
                    }
                }
                else //Limita il numero di righe ritornate a "maxReturnedValues".
                {
                    if (idArchive.HasValue && idArchive.Value != Guid.Empty) //Filtra per ID archivio.
                    {
                        query = this.db.PreservationTaskGroup
                            .Include(x => x.PreservationTask)
                            .Include(x => x.PreservationTask.First().PreservationTaskType)
                            .Include(x => x.PreservationTask.First().PreservationAlert)
                            .Include(x => x.PreservationTask.First().PreservationAlert.First().PreservationAlertType)
                            .Include(x => x.PreservationTaskGroupType)
                            .Where(x => x.PreservationTask
                            .All(y => y.IdArchive == idArchive.Value))
                            .OrderByDescending(x => x.Expiry)
                            .Take(maxReturnedValues)
                            .AsQueryable();
                    }
                    else //Lista completa senza tener conto dell'archivio. 
                    {
                        query = this.db.PreservationTaskGroup
                           .Include(x => x.PreservationTask)
                           .Include(x => x.PreservationTask.First().PreservationTaskType)
                           .Include(x => x.PreservationTask.First().PreservationAlert)
                           .Include(x => x.PreservationTask.First().PreservationAlert.First().PreservationAlertType)
                           .Include(x => x.PreservationTaskGroupType)
                           .OrderByDescending(x => x.Expiry)
                           .Take(maxReturnedValues)
                           .AsQueryable();
                    }
                }
            }

            if (query != null)
            {
                foreach (var item in query)
                {
                    retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }

            if (maxReturnedValues > 0 && (!idTaskGroup.HasValue || idTaskGroup.Value == Guid.Empty))
                foreach (var item in retval)
                {
                    if (item.Tasks != null)
                        foreach (var task in item.Tasks)
                        {
                            if (task.User != null)
                                task.User.UserRoles = null;
                            if (task.TaskGroup != null)
                                task.TaskGroup = null;
                            foreach (var alert in task.Alerts)
                            {
                                alert.Task = null;
                            }
                        }
                }
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationUser"></param>
        /// <param name="idPreservationTask"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<Preservation> GetPreservationsByUserAndTask(Guid idPreservationUser, Nullable<Guid> idPreservationTask, Guid idArchive)
        {
            var retval = new BindingList<Preservation>();
            IQueryable<Model.Preservation> query = null;

            if (idPreservationUser != Guid.Empty && idArchive != Guid.Empty)
            {
                if (!idPreservationTask.HasValue || idPreservationTask.Value == Guid.Empty)
                {
                    query = this.db.Preservation
                        .Include(x => x.PreservationTask)
                        .Include(x => x.PreservationTaskGroup)
                        .Include(x => x.PreservationUser)
                        .Where(x => x.IdArchive == idArchive && x.IdPreservationUser == idPreservationUser);
                }
                else
                {
                    query = this.db.Preservation
                        .Include(x => x.PreservationTask)
                        .Include(x => x.PreservationTaskGroup)
                        .Include(x => x.PreservationUser)
                        .Where(x => x.IdArchive == idArchive && x.IdPreservationUser == idPreservationUser && x.IdPreservationTask == idPreservationTask.Value);
                }

                foreach (var pres in query)
                {
                    retval.Add(pres.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationAlertType"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<PreservationAlertType> GetPreservationAlertTypes(Nullable<Guid> idPreservationAlertType, Guid idArchive)
        {
            var retval = new BindingList<PreservationAlertType>();
            var query = this.db.PreservationAlertType
                    .Include(x => x.PreservationRole)
                    .Include(x => x.PreservationAlertTask)
                    .Include(x => x.PreservationAlertTask.First().PreservationTaskType)
                    .Where(x => x.PreservationAlertTask.All(y => y.IdArchive == idArchive));

            if (idPreservationAlertType.HasValue && idPreservationAlertType.Value != Guid.Empty)
            {
                query = query
                    .Where(x => x.IdPreservationAlertType == idPreservationAlertType.Value);
            }

            foreach (var alertType in query)
            {
                retval.Add(alertType.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationAlertType"></param>
        /// <param name="idPreservationTaskType"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<PreservationAlertType> GetPreservationAlertTypesByTaskType(Nullable<Guid> idPreservationAlertType, Guid idPreservationTaskType, Guid idArchive)
        {
            var retval = new BindingList<PreservationAlertType>();
            IQueryable<Model.PreservationAlertType> query = null;

            if (idPreservationTaskType != Guid.Empty && idArchive != Guid.Empty)
            {
                query = this.db.PreservationAlertType
                        .Include(x => x.PreservationRole)
                        .Include(x => x.PreservationAlertTask)
                        .Include(x => x.PreservationAlertTask.First().PreservationTaskType)
                        .Include(x => x.PreservationAlertTask.First().PreservationTaskType.PreservationTask)
                        .Include(x => x.PreservationAlertTask.First().PreservationTaskType.PreservationTask.First().Archive)
                        .Include(x => x.PreservationAlert)
                        .Where(x => x.PreservationAlertTask
                            .Any(y => y.IdPreservationTaskType == idPreservationTaskType
                                && (y.IdArchive == idArchive || y.PreservationTaskType.PreservationTask.Any(p => p.IdArchive == idArchive))));

                if (idPreservationAlertType.HasValue && idPreservationAlertType.Value != Guid.Empty)
                {
                    query = query
                        .Where(x => x.IdPreservationAlertType == idPreservationAlertType.Value);
                }

                foreach (var alertType in query)
                {
                    retval.Add(alertType.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, typeof(PreservationAlert)));
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSchedule"></param>
        /// <param name="idTaskGroupType"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public PreservationExpireResponse GetPreservationExpire(Guid idSchedule, Guid idTaskGroupType, Guid idArchive)
        {
            var retval = new PreservationExpireResponse();

            if (idSchedule != Guid.Empty && idTaskGroupType != Guid.Empty)
            {
                var groups = this.db.PreservationTaskGroup
                    .Include(x => x.PreservationSchedule)
                    .Include(x => x.PreservationTaskGroupType)
                    .Where(x => x.IdPreservationSchedule == idSchedule && x.IdArchive == idArchive
                        && x.IdPreservationTaskGroupType == idTaskGroupType);

                if (groups.Count() < 1)
                {
                    retval.Expiry = null;
                    retval.EstimatedExpiry = null;
                }
                else
                {
                    retval.Expiry = groups.Select(x => x.Expiry).Max();
                    retval.EstimatedExpiry = groups.Select(x => x.EstimatedExpiry.HasValue ? x.EstimatedExpiry.Value : new DateTime(1899, 12, 31)).Max();
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSchedule"></param>
        /// <param name="idTaskGroupType"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<PreservationTask> GetPreservationExpireTask()
        {
            BindingList<PreservationTask> result = new BindingList<PreservationTask>();
            var query = this.db.PreservationTask.Include(x => x.Archive).Include(x => x.PreservationTaskType).Where(x => x.Enabled && (x.Executed == false || (x.Executed && x.HasError)) && x.EstimatedDate < DateTime.Now).OrderBy(x => x.StartDocumentDate);

            foreach (var item in query)
            {
                result.Add(item.Convert());
            }
            return result;
        }

        public BindingList<PreservationJournalingActivity> GetPreservationJournalingActivityByCode(string keyCode)
        {
            IQueryable<Model.PreservationJournalingActivity> query = null;
            BindingList<PreservationJournalingActivity> retval = new BindingList<PreservationJournalingActivity>();

            query = this.db.PreservationJournalingActivity
                    .Include(x => x.PreservationJournaling)
                    .Where(x => x.KeyCode == keyCode);

            foreach (var activity in query)
            {
                retval.Add(activity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return retval;
        }

        public BindingList<PreservationJournalingActivity> GetPreservationJournalingActivities(Guid? idJournalingActivity, bool includeJournal = true)
        {
            BindingList<PreservationJournalingActivity> retval = new BindingList<PreservationJournalingActivity>();
            ObjectQuery<Model.PreservationJournalingActivity> query = this.db.PreservationJournalingActivity;
            if (includeJournal)
            {
                query = query.Include(x => x.PreservationJournaling);
            }

            IQueryable<Model.PreservationJournalingActivity> queryExt = query.AsQueryable();
            if (idJournalingActivity.HasValue && idJournalingActivity.Value != Guid.Empty)
            {
                queryExt = queryExt.Where(x => x.IdPreservationJournalingActivity == idJournalingActivity.Value);
            }
            queryExt.OrderBy(o => o.Description).ToList().ForEach(f => retval.Add(f.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL)));
            return retval;
        }

        public Guid? GetPreservationStorageForPreservation(Preservation pres, string label, Guid idCompany)
        {
            try
            {
                var storageDevices = db.PreservationStorageDevice.Where(x => x.IdCompany == idCompany && x.MinDate <= pres.StartDate && x.MaxDate >= pres.EndDate && x.Label == label);
                if (storageDevices.Any())
                {
                    return storageDevices.First().IdPreservationStorageDevice;
                }
                return null;
            }
            finally
            {
                Dispose();
            }
        }

        public PreservationStorageDevice GetPreservationStorageDevice(Guid idPreservationStorageDevice)
        {
            var query = this.db.PreservationStorageDevice
                    .Include(x => x.PreservationInStorageDevice)
                    .Include(x => x.PreservationStorageDeviceStatus)
                    .Where(x => x.IdPreservationStorageDevice == idPreservationStorageDevice)
                    .OrderByDescending(x => x.DateCreated);

            return query.Single().Convert(0, 4);
        }

        /// <summary>
        /// Ritorna la lista dei supporti per conservazioni.
        /// </summary>
        /// <param name="idPreservationStorageDevice"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<PreservationStorageDevice> GetPreservationStorageDevices(Guid? idPreservationStorageDevice)
        {
            var retval = new BindingList<PreservationStorageDevice>();

            IQueryable<Model.PreservationStorageDevice> query = null;

            if (idPreservationStorageDevice.HasValue && idPreservationStorageDevice.Value != Guid.Empty)
            {
                query = this.db.PreservationStorageDevice
                    .Include(x => x.PreservationInStorageDevice)
                    .Include(x => x.PreservationInStorageDevice.First().Preservation)
                    .Include(x => x.PreservationInStorageDevice.First().Preservation.Archive)
                    .Include(x => x.PreservationStorageDeviceStatus)
                    .Where(x => x.IdPreservationStorageDevice == idPreservationStorageDevice.Value)
                    .OrderByDescending(x => x.DateCreated);
            }
            else
            {
                query = this.db.PreservationStorageDevice
                    .Include(x => x.PreservationInStorageDevice)
                    .Include(x => x.PreservationInStorageDevice.First().Preservation)
                    .Include(x => x.PreservationInStorageDevice.First().Preservation.Archive)
                    .Include(x => x.PreservationStorageDeviceStatus)
                    .OrderByDescending(x => x.DateCreated);
            }

            foreach (var storage in query)
            {
                retval.Add(storage.Convert(0, 4));
            }

            return retval;
        }
        /// <summary>
        /// Ritorna il conteggio assoluto dei supporti archivio presenti in banca dati.
        /// </summary>
        /// <returns></returns>
        public int GetPreservationStorageDevicesCount()
        {
            return db.PreservationStorageDevice.Count();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationStorageDevice"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <returns></returns>
        public BindingList<PreservationStorageDevice> GetPreservationStorageDevicesFromDates(Guid? idPreservationStorageDevice, DateTime? minDate, DateTime? maxDate, string username, int skip, int take, out long totalItems)
        {
            var retval = new BindingList<PreservationStorageDevice>();

            var query = this.db.PreservationStorageDevice
                .Include(x => x.PreservationInStorageDevice)
                .Include(x => x.PreservationStorageDeviceStatus)
                .AsQueryable();

            //.Include(x => x.PreservationInStorageDevice.First().Preservation)

            if (idPreservationStorageDevice.HasValue)
                query = query.Where(x => x.IdPreservationStorageDevice == idPreservationStorageDevice.Value);
            if (minDate.HasValue)
                query = query.Where(x => x.MinDate.HasValue && x.MinDate.Value >= minDate.Value);
            if (maxDate.HasValue)
                query = query.Where(x => x.MaxDate.HasValue && x.MaxDate.Value <= maxDate.Value);

            //Athesia 23112012 Utente di windows non più supportato
            //if (!string.IsNullOrWhiteSpace(username))
            //{
            //    var idArch = this.db.PreservationUserRole
            //        .Include(x => x.PreservationUser)
            //        .Where(x => x.PreservationUser.DomainUser.Equals(username, StringComparison.InvariantCultureIgnoreCase)
            //            && x.PreservationRole.KeyCode == 2
            //            && x.IdArchive.HasValue)
            //        .Select(x => x.IdArchive)
            //        .ToArray();

            //    //query = query
            //    //    .Where(x => x.PreservationInStorageDevice.Count == 0 
            //    //        || x.PreservationInStorageDevice.Any(arch => id.Contains(arch.Preservation.IdArchive)));

            //    var arcComp = GetArchiveCompanyByUser(null, null, username)
            //        .Select(x => x.CompanyName)
            //        .Distinct()
            //        .ToArray();

            //    query = query.Where(x => arcComp.Contains(x.Company));
            //}

            totalItems = query.Count();

            if (skip < 0)
                skip = 0;

            if (take < 1)
                take = 1;

            query = query
                .OrderBy(x => x.IdPreservationStorageDevice)
                .Skip(skip)
                .Take(take);

            PreservationStorageDevice toAdd;
            foreach (var storage in query)
            {
                storage.PreservationInStorageDevice = null;
                toAdd = storage.Convert(0, 2, typeof(PreservationStorageDevice));
                retval.Add(toAdd);
            }

            return retval;
        }

        public ICollection<Guid> GetIdPreservationsInStorageDevice(Guid idStorageDevice)
        {
            return this.db.PreservationInStorageDevice.Where(x => x.IdPreservationStorageDevice == idStorageDevice)
                .Select(s => s.IdPreservation).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="idPreservationStorageDevice"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="totalItems"></param>
        /// <returns></returns>
        public BindingList<PreservationInStorageDevice> GetPreservationsInStorageDevices(Nullable<Guid> idPreservation, Nullable<Guid> idPreservationStorageDevice, int skip, int take, out int totalItems)
        {
            var retval = new BindingList<PreservationInStorageDevice>();

            var query = this.db.PreservationInStorageDevice
                .Include(x => x.PreservationStorageDevice)
                .Include(x => x.Preservation)
                .Where(x => x.IdPreservation == ((idPreservation.HasValue) ? idPreservation.Value : x.IdPreservation)
                    && x.IdPreservationStorageDevice == ((idPreservationStorageDevice.HasValue) ? idPreservationStorageDevice.Value : x.IdPreservationStorageDevice));

            totalItems = query.Count();

            if (skip > -1)
                query = query.OrderBy(x => x.IdPreservationStorageDevice).Skip(skip);
            if (take > 0)
                query = query.Take(take);

            foreach (var item in query)
            {
                var toAdd = item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);
                var pres = toAdd.Preservation;

                if (pres != null)
                {
                    pres.CloseContent = null; //Alleggerisce la mole di dati che viaggia sulla rete.                    
                }

                retval.Add(toAdd);
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Nullable<DateTime> GetPreservationJournalingLastPrintManualActivityDate()
        {
            return this.db.PreservationJournaling
                .Include(x => x.PreservationJournalingActivity)
                .Where(x => x.PreservationJournalingActivity.KeyCode.Equals("StampaManuale", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.DateActivity)
                .Max();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idStatus"></param>
        /// <returns></returns>
        public BindingList<PreservationStorageDeviceStatus> GetPreservationStorageDeviceStatus(Nullable<Guid> idStatus)
        {
            var retval = new BindingList<PreservationStorageDeviceStatus>();
            IQueryable<Model.PreservationStorageDeviceStatus> query = null;

            if (idStatus.HasValue && idStatus.Value != Guid.Empty)
            {
                query = this.db.PreservationStorageDeviceStatus
                    .Where(x => x.IdPreservationStorageDeviceStatus == idStatus.Value);
            }
            else
            {
                query = this.db.PreservationStorageDeviceStatus;
            }

            foreach (var item in query)
            {
                retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return retval;
        }

        public BindingList<PreservationStorageDevice> GetPreservationStorageDeviceFromLabel(string label)
        {
            if (string.IsNullOrEmpty(label))
                new PreservationError("Label is not valid.", PreservationErrorCode.E_INVALID_PARAMS)
                    .ThrowsAsFaultException();

            IQueryable<Model.PreservationStorageDevice> query = null;
            BindingList<PreservationStorageDevice> retval = new BindingList<PreservationStorageDevice>();

            query = this.db.PreservationStorageDevice
                .Include(x => x.PreservationInStorageDevice)
                .Include(x => x.PreservationInStorageDevice.First().Preservation)
                .Include(x => x.PreservationStorageDeviceStatus)
                .Where(x => x.Label.Equals(label, StringComparison.InvariantCultureIgnoreCase));

            foreach (var device in query)
            {
                retval.Add(device.Convert(0, 3));
            }

            return retval;
        }

        public void UpdatePreservationStorageDeviceLastVerifyDate(Guid idStorageDevice, Nullable<DateTime> verifyDate)
        {
            var entity = this.db.PreservationStorageDevice
                .Where(x => x.IdPreservationStorageDevice == idStorageDevice)
                .SingleOrDefault();

            if (entity != null)
            {
                entity.LastVerifyDate = verifyDate;
                this.db.SaveChanges();
            }
        }

        public PreservationStorageDeviceStatus PreservationStorageDeviceChangeStatus(Guid idPreservationStorageDevice, PreservationStatus preservationStatus)
        {
            string keyCode;

            switch (preservationStatus)
            {
                case PreservationStatus.Copiato:
                    keyCode = "Copiato";
                    break;
                case PreservationStatus.Chiuso:
                    keyCode = "Chiuso";
                    break;
                default:
                    return null;
            }

            var entityStatus = db.PreservationStorageDeviceStatus.Where(x => x.KeyCode == keyCode).SingleOrDefault();
            var entity = db.PreservationStorageDevice.Where(x => x.IdPreservationStorageDevice == idPreservationStorageDevice).SingleOrDefault();

            if ((entity == null) || (entityStatus == null)) return null;

            entity.IdPreservationStorageDeviceStatus = entityStatus.IdPreservationStorageDeviceStatus;
            db.SaveChanges();

            return entityStatus.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);
        }

        public Guid PreservationStorageDeviceGetStatusNullo()
        {
            var entityStatus = db.PreservationStorageDeviceStatus.Where(x => x.KeyCode == "StatoNullo").SingleOrDefault();
            if (entityStatus == null) throw new Exception("Nessuno stato definito con Codice \"StatoNullo\". Inserire le configurazioni nella tabella \"PreservationStorageDeviceStatus\".");
            return entityStatus.IdPreservationStorageDeviceStatus;
        }

        public BindingList<PreservationJournaling> GetPreservationJournalings(Guid? idArchive, Guid? idPreservation, DateTime? startDate, DateTime? endDate, Guid? idActivityType, Guid? idCompany,
            int skip, int take, out int journalingsInArchive, bool includePreservation = true, bool sortingDescending = true)
        {
            var retval = new BindingList<PreservationJournaling>();

            if (idArchive.HasValue && idArchive == Guid.Empty && idPreservation.HasValue && idPreservation == Guid.Empty)
                new PreservationError("E' necessario specificare almeno l'archivio e/o la conservazione.", PreservationErrorCode.E_INVALID_PARAMS)
                .ThrowsAsFaultException();

            try
            {
                IQueryable<Model.PreservationJournaling> queryExt = this.db.PreservationJournaling.Include(x=>x.Preservation)
                    .Include(x=>x.Preservation.Archive)
                    .Include(x=>x.Preservation.Archive.ArchiveCompany)
                    .Where(x => ((idArchive.HasValue && idArchive.Value != Guid.Empty) ? x.Preservation.IdArchive == idArchive.Value : true)
                            && ((idPreservation.HasValue && idPreservation.Value != Guid.Empty) ? x.IdPreservation == idPreservation.Value : true)
                            && ((startDate.HasValue && x.DateActivity.HasValue) ? x.DateActivity.Value >= startDate.Value : true)
                            && ((endDate.HasValue && x.DateActivity.HasValue) ? x.DateActivity.Value <= endDate.Value : true)
                            && ((idActivityType.HasValue && idActivityType.Value != Guid.Empty) ? x.IdPreservationJournalingActivity == idActivityType.Value : true)
                            && ((idCompany.HasValue && idCompany.Value != Guid.Empty) ? x.Preservation.Archive.ArchiveCompany.Any(ac=>ac.IdCompany == idCompany) : true));

                journalingsInArchive = queryExt.Count();
                string sortingType = sortingDescending ? "DESC" : "ASC";
                IQueryable <PreservationJournalingTableValuedResult> query = this.db.PreservationJournalings_FX_SearchAudits(idArchive, idPreservation, idActivityType,idCompany, startDate, endDate, skip, take, sortingType);
                query.ToList().ForEach(f => retval.Add(f.Convert()));
                return retval;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Preservation> GetPreservationsFromJournaling(Guid idJournaling, int skip, int take, out int preservationsCount)
        {
            var retval = new BindingList<Preservation>();

            preservationsCount = -1;

            try
            {
                IQueryable<Guid> query = null;

                if (idJournaling != Guid.Empty)
                {
                    query = this.db.PreservationJournaling
                        .Where(x => x.IdPreservationJournaling == idJournaling && x.IdPreservation.HasValue)
                        .OrderBy(x => x.DateActivity)
                        .Skip(skip)
                        .Take(take)
                        .Select(x => x.IdPreservation.Value)
                        .Distinct();
                }
                else
                {
                    query = this.db.PreservationJournaling
                        .Where(x => x.IdPreservation.HasValue)
                        .OrderBy(x => x.DateActivity)
                        .Skip(skip)
                        .Take(take)
                        .Select(x => x.IdPreservation.Value)
                        .Distinct();
                }

                preservationsCount = query.Count();

                foreach (var idPreservation in query)
                {
                    retval.Add(this.GetPreservation(idPreservation));
                }
            }
            catch
            {
                retval = new BindingList<Preservation>();
            }
            finally
            {
                Dispose();
            }

            return retval;
        }

        public BindingList<Preservation> GetPreservationsFromArchiveAndDates(Guid idArchive, DateTime minDate, DateTime maxDate, int take, int skip, out long totalItems)
        {
            var ret = new BindingList<Preservation>();

            var query = this.db.Preservation
                .Include(x => x.Archive)
                .Where(x => x.IdArchive == idArchive
                    && x.StartDate.HasValue
                    && x.EndDate.HasValue
                    && x.StartDate.Value >= minDate && x.EndDate.Value <= maxDate);

            totalItems = query.Count();

            var lista = query.OrderBy(x => x.IdPreservation)
                .Skip(skip)
                .Take(take)
                .ToList();

            lista.ForEach(x => ret.Add(x.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, typeof(Document))));

            return ret;
        }

        public PreservationSchedule GetDefaultPreservationSchedule()
        {
            var scad = db.PreservationSchedule
                .Include(x => x.PreservationSchedule_TaskType)
                .Include(x => x.PreservationSchedule_TaskType.First().PreservationTaskType)
                .Include(x => x.PreservationSchedule_TaskType.First().PreservationSchedule)
                .Where(x => x.IsDefault == (short)1)
                .SingleOrDefault();

            if (scad != null)
            {
                return scad.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);
            }
            else
            {
                return null;
            }
        }

        public BindingList<BiblosDS.Library.Common.Objects.ArchiveCompany> GetArchiveCompany(Guid? idArchive, string companyName)
        {
            return GetArchiveCompanyByUser(idArchive, companyName, null);
        }

        public BindingList<BiblosDS.Library.Common.Objects.ArchiveCompany> GetArchiveCompanyByUser(Guid? idArchive, string companyName, string username)
        {
            var query = db.ArchiveCompany
                .Include(x => x.Archive)
                .Include(x => x.Company)
                .Where(x => x.Company != null);

            var retval = new BindingList<BiblosDS.Library.Common.Objects.ArchiveCompany>();

            if (idArchive.HasValue)
                query = query.Where(x => x.IdArchive == idArchive.Value);

            if (!string.IsNullOrWhiteSpace(companyName))
                query = query.Where(x => x.Company.CompanyName == companyName);

            if (!string.IsNullOrWhiteSpace(username))
            {
                var id = this.db.PreservationUserRole
                    .Include(x => x.PreservationUser)
                    .Where(x => x.PreservationUser.DomainUser.Equals(username, StringComparison.InvariantCultureIgnoreCase)
                        && x.PreservationRole.KeyCode == 2
                        && x.IdArchive.HasValue)
                    .Select(x => x.IdArchive)
                    .ToArray();

                query = query
                    .Where(x => id.Contains(x.IdArchive));
            }

            foreach (var item in query)
            {
                retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
            }

            return retval;
        }

        public BiblosDS.Library.Common.Objects.Company GetCompany(Guid idCompany)
        {
            BiblosDS.Library.Common.Objects.Company retval = null;

            var comp = db.Company
                .SingleOrDefault(x => x.IdCompany == idCompany);

            if (comp != null)
            {
                retval = comp.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);
            }

            return retval;
        }

        //Serve nella pagina ArchiveCompany
        public BindingList<BiblosDS.Library.Common.Objects.Company> GetCompanies()
        {
            BindingList<BiblosDS.Library.Common.Objects.Company> retval = new BindingList<BiblosDS.Library.Common.Objects.Company>();

            this.db.Company
                .ToList()
                .ForEach(x => retval.Add(x.Convert()));

            return retval;
        }

        public BiblosDS.Library.Common.Objects.Company GetCompanyFromArchive(Guid idArchive)
        {
            BiblosDS.Library.Common.Objects.Company retval = null;

            var comp = db.ArchiveCompany.Include(x => x.Company)
                .SingleOrDefault(x => x.IdArchive == idArchive);

            if (comp != null && comp.Company != null)
            {
                retval = comp.Company.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);
                //if (retval != null)
                //    retval.TemplateXSLTFile = comp.TemplateXSLTFile;
            }

            return retval;
        }



        #endregion

        #region Inserters

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAdd"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public PreservationTaskGroup AddPreservationTaskGroup(PreservationTaskGroup toAdd, Guid idArchive)
        {
            if (toAdd == null)
                new PreservationError("PreservationTaskGroup is invalid.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            PreservationTaskGroup ret = null;

            try
            {
                var entity = new Model.PreservationTaskGroup
                {
                    IdPreservationTaskGroup = Guid.NewGuid(),
                    IdPreservationTaskGroupType = toAdd.GroupType.IdPreservationTaskGroupType,
                    Name = toAdd.Name,
                    IdPreservationUser = toAdd.User.IdPreservationUser,
                    IdPreservationSchedule = toAdd.Schedule.IdPreservationSchedule,
                    Expiry = toAdd.Expiry,
                    EstimatedExpiry = toAdd.EstimatedExpiry,
                    IdArchive = idArchive,
                    PreservationTask = new EntityCollection<Model.PreservationTask>(),
                };

                #region TASK + AVVISI

                if (toAdd.Tasks != null && toAdd.Tasks.Count > 0)
                {
                    Model.PreservationTask dbTask;
                    Model.PreservationTaskType dbTaskType;
                    Model.PreservationAlert dbAlert;
                    Model.PreservationAlertType dbAlertType;

                    foreach (var task in toAdd.Tasks)
                    {
                        dbTaskType = null;
                        dbTask = null;
                        //Verifica se il task da associare al gruppo task è già presente in db.
                        if (task.IdPreservationTask != Guid.Empty)
                        {
                            dbTask = this.db.PreservationTask
                                .Where(x => x.IdPreservationTask == task.IdPreservationTask)
                                .SingleOrDefault();
                        }
                        //Se non esiste alcun task in banca dati, prova a crearne uno nuovo.
                        if (dbTask == null)
                        {
                            logger.InfoFormat("AddPreservationTaskGroup - {0}", "Creazione nuovo TASK.");

                            dbTask = new Model.PreservationTask
                            {
                                IdPreservationTask = Guid.NewGuid(),
                                IdArchive = idArchive,
                                EstimatedDate = DateTime.MaxValue,
                                IdPreservationUser = toAdd.User.IdPreservationUser,
                                PreservationAlert = new EntityCollection<Model.PreservationAlert>(),
                            };

                            #region TIPO TASK

                            //Controlla se il tipo task è stato passato o meno.
                            if (task.TaskType != null && task.TaskType.IdPreservationTaskType != Guid.Empty)
                            {
                                dbTaskType = this.db.PreservationTaskType
                                    .Where(x => x.IdPreservationTaskType == task.TaskType.IdPreservationTaskType)
                                    .SingleOrDefault();
                            }
                            //non è stato trovato il tipo task in banca dati?
                            if (dbTaskType == null)
                            {
                                logger.InfoFormat("AddPreservationTaskGroup - {0}", "Nessun tipo task corrispondente. Verra' usato quello predefinito.");
                                //Piglia quello di default.
                                dbTaskType = this.db.PreservationTaskType
                                    .Where(x => x.KeyCode == "1")
                                    .SingleOrDefault();
                                //Se non c'è neanche quello di default...
                                throw new Exception("Impossibile creare un gruppo task: il tipo task predefinito e' assente in banca dati.");
                            }
                            else
                            {
                                logger.InfoFormat("AddPreservationTaskGroup - {0}", "Il tipo task con ID", dbTaskType.IdPreservationTaskType, "e' gia' presente in db.");
                            }
                            //Abbina il tipo task al task.
                            dbTask.IdPreservationTaskType = dbTaskType.IdPreservationTaskType;

                            #endregion TIPO TASK

                            //Associazione task - tipo task.
                            dbTask.PreservationTaskType = dbTaskType;

                            #region AVVISI

                            if (task.Alerts != null && task.Alerts.Count > 0)
                            {
                                foreach (var alert in task.Alerts)
                                {
                                    dbAlert = null;
                                    dbAlertType = null;
                                    //Controlla se l'avviso e' gia' presente in db.
                                    if (alert.IdPreservationAlert != Guid.Empty)
                                    {
                                        dbAlert = this.db.PreservationAlert
                                            .Where(x => x.IdPreservationAlert == alert.IdPreservationAlert)
                                            .SingleOrDefault();
                                    }
                                    //Non è stato trovato?
                                    if (dbAlert == null)
                                    {
                                        logger.InfoFormat("AddPreservationTaskGroup - {0}", "Creazione nuovo AVVISO.");

                                        dbAlert = new Model.PreservationAlert
                                        {
                                            IdPreservationAlert = Guid.NewGuid(),
                                            AlertDate = alert.AlertDate,
                                        };

                                        #region TIPO AVVISO

                                        if (alert.AlertType != null && alert.AlertType.IdPreservationAlertType != Guid.Empty)
                                        {
                                            dbAlertType = this.db.PreservationAlertType
                                                .Where(x => x.IdPreservationAlertType == alert.AlertType.IdPreservationAlertType)
                                                .SingleOrDefault();
                                        }

                                        if (dbAlertType == null)
                                            throw new Exception("Impossibile creare il gruppo task: il tipo avviso da associare all'avviso non e' configurato correttamente.");
                                        //Associazione avviso -> tipo avviso
                                        dbAlert.IdPreservationAlertType = dbAlertType.IdPreservationAlertType;

                                        #endregion TIPO AVVISO
                                    }
                                    else
                                    {
                                        logger.InfoFormat("AddPreservationTaskGroup - {0} {1} {2}", "L'avviso con ID", alert.IdPreservationAlert, "e' gia' presente in banca dati.");
                                    }

                                    //Associazione avviso - task.
                                    dbTask.PreservationAlert.Add(dbAlert);
                                }
                            }

                            #endregion

                            //Aggiunta del task al gruppo task.
                            entity.PreservationTask.Add(dbTask);
                        }
                        else
                        {
                            logger.InfoFormat("AddPreservationTaskGroup - {0}", "Il task e' gia' presente in banca dati e non deve essere creato.");
                        }
                    }
                }

                #endregion TASK + AVVISI

                this.db.PreservationTaskGroup.AddObject(entity);

                if (requireSave)
                    this.db.SaveChanges();

                ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);
            }
            catch (Exception exx)
            {
                try { this.db.Dispose(); }
                catch { }
                logger.Error("AddPreservationTaskGroup", exx);
                throw;
            }

            Dispose();

            return ret;
        }

        public PreservationTask AddPreservationTask(PreservationTask toAdd, Guid idArchive)
        {
            if (toAdd == null)
                new PreservationError("PreservationTask is invalid.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            if (idArchive == Guid.Empty)
                new PreservationError("idArchive is invalid.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            Model.PreservationTaskType taskType = this.db.PreservationTaskType
                .Where(x => x.IdPreservationTaskType == toAdd.TaskType.IdPreservationTaskType)
                .SingleOrDefault();

            if (taskType == null)
            {
                taskType = new Model.PreservationTaskType
                {
                    IdPreservationTaskType = Guid.NewGuid(),
                    Description = ((toAdd.TaskType != null) ? toAdd.TaskType.Description : null) ?? "NEW_TaskType",
                    Period = (toAdd.TaskType != null) ? toAdd.TaskType.Period : (short)0,
                };

                this.db.PreservationTaskType.AddObject(taskType);
            }

            var entity = new Model.PreservationTask
            {
                IdPreservationTask = Guid.NewGuid(),
                IdArchive = idArchive,
                IdPreservationTaskGroup = toAdd.TaskGroup.IdPreservationTaskGroup,
                IdPreservationTaskType = taskType.IdPreservationTaskType,
                IdPreservationUser = toAdd.User.IdPreservationUser,
                EstimatedDate = toAdd.EstimatedDate,
            };

            Model.PreservationTaskStatus status = null;

            if (!string.IsNullOrEmpty(toAdd.TaskGroup.Name))
            {
                status = this.db.PreservationTaskStatus
                    .Where(x => x.Status.Equals(toAdd.TaskGroup.Name, StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();
            }

            if (status == null)
            {
                status = new Model.PreservationTaskStatus { IdPreservationTaskStatus = Guid.NewGuid(), Status = toAdd.TaskGroup.Name ?? "NEW_TaskStatus", };
                this.db.PreservationTaskStatus.AddObject(status);
            }

            entity.IdPreservationTaskStatus = status.IdPreservationTaskStatus;

            this.db.PreservationTask.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationAlert AddPreservationAlert(PreservationAlert toAdd)
        {
            if (toAdd == null)
                new PreservationError("PreservationAlert is invalid.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            var entity = new Model.PreservationAlert
            {
                IdPreservationAlert = Guid.NewGuid(),
                IdPreservationTask = toAdd.Task.IdPreservationTask,
                IdPreservationAlertType = toAdd.AlertType.IdPreservationAlertType,
                AlertDate = toAdd.AlertDate,
            };

            this.db.PreservationAlert.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();


            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationUser AddPreservationUser(PreservationUser user, Guid idArchive)
        {
            if (user == null)
                new PreservationError("User is invalid.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            var entity = new Model.PreservationUser
            {
                IdPreservationUser = Guid.NewGuid(),
                Address = user.Address,
                DomainUser = user.DomainUser,
                Email = user.EMail,
                Enable = user.Enabled,
                FiscalId = user.FiscalId,
                Name = user.Name,
                Surname = user.Surname,
            };

            try
            {
                this.db.PreservationUser.AddObject(entity);

                if (user.UserRoles != null)
                {
                    for (int i = 0; i < user.UserRoles.Count; i++)
                    {
                        if (user.UserRoles[i].PreservationUser == null)
                            user.UserRoles[i].PreservationUser = new PreservationUser();

                        user.UserRoles[i].PreservationUser.IdPreservationUser = entity.IdPreservationUser;

                        user.UserRoles[i].IdPreservationUserRole = this.AddPreservationUserRole(user.UserRoles[i], idArchive).IdPreservationUserRole;
                    }
                }

                if (requireSave)
                    this.SaveChanges();

                Dispose();
            }
            catch (Exception exx)
            {
                Dispose();
                throw exx;
            }

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationUserRole AddPreservationUserRole(PreservationUserRole userRole, Guid idArchive)
        {
            if (userRole == null)
                new PreservationError("PreservationUserRole is invalid.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            if (idArchive == Guid.Empty)
                new PreservationError("IdArchive not valid.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            if (userRole.PreservationUser == null || userRole.PreservationUser.IdPreservationUser == Guid.Empty)
                new PreservationError("It's impossible to create an user's role without the associated user.", PreservationErrorCode.E_INVALID_CALL).ThrowsAsFaultException();
            if (userRole.PreservationRole == null || userRole.PreservationRole.IdPreservationRole == Guid.Empty)
                new PreservationError("It's impossible to create an user's role without the associated role.", PreservationErrorCode.E_INVALID_CALL).ThrowsAsFaultException();

            var entity = new Model.PreservationUserRole
            {
                IdPreservationUserRole = Guid.NewGuid(),
                IdPreservationRole = userRole.PreservationRole.IdPreservationRole,
                IdPreservationUser = userRole.PreservationUser.IdPreservationUser,
            };

            entity.IdArchive = idArchive;

            this.db.PreservationUserRole.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationSchedule AddPreservationSchedule(PreservationSchedule sched, Guid idArchive)
        {
            if (sched == null)
                new PreservationError("PreservationSchedule non valido", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            var entity = new Model.PreservationSchedule
            {
                IdPreservationSchedule = Guid.NewGuid(),
                Active = (byte)((sched.Active) ? 1 : 0),
                FrequencyType = sched.FrequencyType,
                Name = sched.Name,
                Period = sched.Period,
                ValidWeekDays = sched.ValidWeekDays,
                IsDefault = (byte)(sched.Default ? 0x01 : 0x00),
            };

            Model.PreservationSchedule defaultSchedule = null;

            try
            {
                //Controlla se esiste già uno scadenziario predefinito.
                if (sched.Default)
                {
                    defaultSchedule = db.PreservationSchedule
                        .Where(x => x.IsDefault == 1)
                        .SingleOrDefault();

                    if (defaultSchedule != null)
                    {
                        defaultSchedule.IsDefault = 0; //Non è più lo scadenziario di default.
                        db.SaveChanges();
                    }
                }

                this.db.PreservationSchedule.AddObject(entity);

                if (sched.PreservationScheduleTaskTypes != null)
                {
                    foreach (var item in sched.PreservationScheduleTaskTypes)
                    {
                        item.Schedule = new PreservationSchedule { IdPreservationSchedule = entity.IdPreservationSchedule };
                        this.AddPreservationScheduleTaskType(item, idArchive);
                    }
                }

                if (requireSave)
                    this.db.SaveChanges();

                Dispose();
            }
            catch (Exception exx)
            {
                if (defaultSchedule != null)
                {
                    defaultSchedule.IsDefault = 0x01;
                    db.SaveChanges();
                }

                Dispose();

                new PreservationError(exx, PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();
            }

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public List<PreservationSchedule> GetPreservationSchedule()
        {
            try
            {
                List<PreservationSchedule> items = new List<PreservationSchedule>();
                var entities = this.db.PreservationSchedule;

                foreach (var item in entities)
                {
                    items.Add(item.Convert());
                }

                return items;
            }
            finally
            {
                Dispose();
            }
        }

        public PreservationScheduleTaskType AddPreservationScheduleTaskType(PreservationScheduleTaskType schedTaskType, Guid idArchive)
        {
            try
            {
                if (schedTaskType == null)
                    new PreservationError("PreservationScheduleTaskType non valido", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

                PreservationSchedule_TaskType entity;
                if ((entity = db.PreservationSchedule_TaskType.Where(x => x.IdPreservationSchedule == schedTaskType.IdPreservationSchedule && x.IdPreservationTaskType == schedTaskType.IdPreservationTaskType).FirstOrDefault()) == null)
                {
                    entity = new PreservationSchedule_TaskType
                    {
                        IdPreservationSchedule = schedTaskType.Schedule.IdPreservationSchedule,
                        IdPreservationTaskType = schedTaskType.TaskType.IdPreservationTaskType,
                        Offset = schedTaskType.Offset,
                    };
                }
                else
                    entity.Offset = schedTaskType.Offset;

                this.db.AddToPreservationSchedule_TaskType(entity);

                if (requireSave)
                    this.db.SaveChanges();

                var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

                return ret;
            }
            finally
            {
                Dispose();
            }
        }

        public PreservationTaskType AddPreservationTaskType(PreservationTaskType taskType, Guid idArchive)
        {
            if (taskType == null)
                new PreservationError("PreservationTaskType non valido.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            taskType.IdPreservationTaskType = Guid.NewGuid();

            var entity = taskType.Convert(this.db, null, DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            this.db.PreservationTaskType.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationHoliday AddPreservationHoliday(PreservationHoliday holiday, Guid idArchive)
        {
            if (holiday == null)
                new PreservationError("PreservationHoliday non valido.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            var entity = new Model.PreservationHolidays
            {
                IdPreservationHolidays = Guid.NewGuid(),
                Description = holiday.Description,
                HolidayDate = holiday.HolidayDate,
            };

            this.db.PreservationHolidays.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationAlertType AddPreservationAlertType(PreservationAlertType alertType, Guid idArchive)
        {
            if (alertType == null)
                new PreservationError("PreservationAlertType non valido.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            if (idArchive == Guid.Empty)
                new PreservationError("IdArchive non valido.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            if (alertType.TaskTypes == null || alertType.TaskTypes.Count < 1 || alertType.TaskTypes.Any(x => x.IdPreservationTaskType == Guid.Empty))
                new PreservationError("Alcuni tipi di task da associare al nuovo tipo avviso non sono stati configurati correttamente.", PreservationErrorCode.E_INVALID_CALL).ThrowsAsFaultException();

            var entity = new Model.PreservationAlertType
            {
                IdPreservationAlertType = Guid.NewGuid(),
                IdPreservationRole = alertType.Role.IdPreservationRole,
                Offset = alertType.Offset,
                AlertText = alertType.AlertText,
            };

            //Assegnazione tipo gruppo task.
            Model.PreservationTaskType t;
            foreach (var tt in alertType.TaskTypes)
            {
                t = this.db.PreservationTaskType
                    .Where(x => x.IdPreservationTaskType == tt.IdPreservationTaskType)
                    .SingleOrDefault();

                if (t == null)
                    new PreservationError("Non esiste alcun tipo task con ID = " + tt.IdPreservationTaskType, PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();

                entity.PreservationAlertTask.Add(new Model.PreservationAlertTask { IdPreservationAlertType = entity.IdPreservationAlertType, IdPreservationTaskType = tt.IdPreservationTaskType, IdArchive = idArchive });
            }

            this.db.PreservationAlertType.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationRole AddPreservationRole(PreservationRole role, Guid idArchive)
        {
            if (role == null)
                new PreservationError("PreservationRole non valido.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            if (idArchive == Guid.Empty)
                new PreservationError("IdArchive non valido.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            var entity = new Model.PreservationRole
            {
                IdPreservationRole = Guid.NewGuid(),
                AlertEnable = role.Enabled,
                Enable = role.Enabled,
                Name = role.Name,
            };

            if (role.UserRoles != null)
            {
                Model.PreservationUserRole existentUserRole;
                foreach (var usrRole in role.UserRoles)
                {
                    if (usrRole.PreservationRole != null)
                        usrRole.IdPreservationRole = usrRole.PreservationRole.IdPreservationRole;

                    if (usrRole.PreservationUser != null)
                        usrRole.IdPreservationUser = usrRole.PreservationUser.IdPreservationUser;

                    existentUserRole = this.db.PreservationUserRole
                        .Where(x => x.IdPreservationUserRole == usrRole.IdPreservationUserRole
                            || (x.IdPreservationUser == usrRole.IdPreservationUser && x.IdPreservationRole == usrRole.IdPreservationRole))
                            .SingleOrDefault();

                    entity.PreservationUserRole.Add(usrRole.Convert(this.db, existentUserRole, DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }

            this.db.PreservationRole.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public void AddPreservationParameter(string label, string value, Guid idArchive)
        {
            var entity = new Model.PreservationParameters
            {
                Label = label,
                Value = value,
                IdArchive = idArchive,
            };

            this.db.PreservationParameters.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            Dispose();
        }

        public PreservationTaskGroupType AddPreservationTaskGroupType(PreservationTaskGroupType groupType, Guid idArchive)
        {
            if (groupType == null)
                new PreservationError("PreservationTaskGroupType non valido.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            if (idArchive == Guid.Empty)
                new PreservationError("IdArchive non valido.", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            var entity = new Model.PreservationTaskGroupType
            {
                IdPreservationTaskGroupType = Guid.NewGuid(),
                Description = groupType.Description,
            };

            this.db.PreservationTaskGroupType.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationJournaling AddPreservationJournaling(PreservationJournaling toAdd)
        {
            if (toAdd == null)
                new PreservationError("PreservationJournaling non valido.", PreservationErrorCode.E_INVALID_PARAMS)
                    .ThrowsAsFaultException();

            if (toAdd.PreservationJournalingActivity == null && toAdd.IdPreservationJournalingActivity == Guid.Empty)
                new PreservationError("Utente di dominio non valido.", PreservationErrorCode.E_INVALID_PARAMS)
                    .ThrowsAsFaultException();

            //if (toAdd.Preservation == null)
            //    new PreservationError("Conservazione non valida.", PreservationErrorCode.E_INVALID_PARAMS)
            //        .ThrowsAsFaultException();

            if (string.IsNullOrEmpty(toAdd.DomainUser))
                new PreservationError("Utente di dominio non valido.", PreservationErrorCode.E_INVALID_PARAMS)
                    .ThrowsAsFaultException();

            Guid idUtente = Guid.Empty;

            if (toAdd.User == null || toAdd.User.IdPreservationUser == Guid.Empty)
            {
                idUtente = this.db.PreservationUser
                    .Where(x => toAdd.DomainUser
                        .Equals(x.DomainUser, StringComparison.InvariantCultureIgnoreCase) && x.Enable == true)
                    .Select(x => x.IdPreservationUser)
                    .SingleOrDefault();
                if (idUtente == null || idUtente == Guid.Empty)
                    idUtente = this.db.PreservationUser
                    .Where(x => x.DefaultUser != null && x.DefaultUser.Value)
                    .Select(x => x.IdPreservationUser)
                    .SingleOrDefault();
            }
            else
            {
                idUtente = toAdd.User.IdPreservationUser;
            }

            if (idUtente == Guid.Empty)
            {
                new PreservationError(string.Format("Nessun utente corrispondente al domain user {0}.", toAdd.DomainUser), PreservationErrorCode.E_INVALID_PARAMS)
                    .ThrowsAsFaultException();
            }

            var entity = new Model.PreservationJournaling
            {
                IdPreservationJournaling = Guid.NewGuid(),
                IdPreservationUser = idUtente,
                IdPreservationJournalingActivity = toAdd.PreservationJournalingActivity.IdPreservationJournalingActivity,
                //IdPreservation = toAdd.Preservation.IdPreservation,
                DateActivity = toAdd.DateActivity,
                DateCreated = toAdd.DateCreated,
                DomainUser = toAdd.DomainUser,
                Notes = toAdd.Notes,
            };

            if (toAdd.Preservation != null)
                entity.IdPreservation = toAdd.Preservation.IdPreservation;
            else
                entity.IdPreservation = toAdd.IdPreservation;

            this.db.PreservationJournaling.AddObject(entity);

            var retval = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            retval.Preservation = toAdd.Preservation;
            retval.PreservationJournalingActivity = toAdd.PreservationJournalingActivity;

            if (requireSave)
                this.db.SaveChanges();

            Dispose();

            return retval;
        }

        public PreservationInStorageDevice AddPreservationInStorageDevice(PreservationInStorageDevice toAdd)
        {
            if (toAdd == null)
                new PreservationError("PreservationInStorageDevice non valido.", PreservationErrorCode.E_INVALID_PARAMS)
                    .ThrowsAsFaultException();

            if (toAdd.Device == null)
                new PreservationError("Supporto non valido.", PreservationErrorCode.E_INVALID_PARAMS)
                    .ThrowsAsFaultException();

            if (toAdd.Preservation == null)
                new PreservationError("Conservazione non valida.", PreservationErrorCode.E_INVALID_PARAMS)
                    .ThrowsAsFaultException();

            var entity = new Model.PreservationInStorageDevice
            {
                IdPreservationStorageDevice = toAdd.Device.IdPreservationStorageDevice,
                IdPreservation = toAdd.Preservation.IdPreservation,
                Path = toAdd.Path,
            };

            this.db.PreservationInStorageDevice.AddObject(entity);

            if (requireSave)
                this.db.SaveChanges();

            var ret = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

            Dispose();

            return ret;
        }

        public PreservationStorageDevice AddPreservationStorageDevice(PreservationStorageDevice toAdd)
        {
            try
            {
                if (toAdd == null)
                    new PreservationError("PreservationStorageDevice non valido.", PreservationErrorCode.E_INVALID_PARAMS)
                        .ThrowsAsFaultException();

                if (toAdd.User == null)
                    new PreservationError("Utente non valido.", PreservationErrorCode.E_INVALID_PARAMS)
                        .ThrowsAsFaultException();

                var entity = new Model.PreservationStorageDevice
                {
                    IdPreservationStorageDevice = Guid.NewGuid(),
                    DateCreated = toAdd.DateCreated,
                    DateStorageDevice = toAdd.DateStorageDevice,
                    DomainUser = toAdd.User.DomainUser,
                    Label = toAdd.Label,
                    LastVerifyDate = toAdd.LastVerifyDate,
                    Location = toAdd.Location,
                    MinDate = toAdd.MinDate,
                    MaxDate = toAdd.MaxDate,
                    Company = toAdd.Company,
                    IdCompany = toAdd.IdCompany
                };

                //Controlla se l'etichetta esiste già.
                var count = db.PreservationStorageDevice.Where(x => x.Label.ToUpper().Trim().Contains(toAdd.Label.ToUpper().Trim())).Count();
                if (count > 0)
                {
                    entity.Label = string.Format("{0}_{1}", entity.Label, count);
                }

                entity.IdPreservationStorageDeviceStatus = toAdd.Status != null ? toAdd.Status.IdPreservationStorageDeviceStatus : PreservationStorageDeviceGetStatusNullo();

                if (toAdd.OriginalPreservationStorageDevice != null)
                    entity.IdPreservationStorageDeviceOriginal = toAdd.OriginalPreservationStorageDevice.IdPreservationStorageDevice;

                if (toAdd.PreservationsInDevice != null && toAdd.PreservationsInDevice.Count > 0)
                {
                    foreach (var pres in toAdd.PreservationsInDevice)
                    {
                        entity.PreservationInStorageDevice.Add(new Model.PreservationInStorageDevice
                        {
                            IdPreservation = pres.Preservation.IdPreservation,
                            Path = pres.Path,
                        });
                    }
                }

                this.db.PreservationStorageDevice.AddObject(entity);

                if (requireSave)
                    this.db.SaveChanges();

                var ret = entity.Convert(0, 3);

                return ret;
            }
            finally
            {
                Dispose();
            }
        }

        public BiblosDS.Library.Common.Objects.Company UpdateCompany(BiblosDS.Library.Common.Objects.Company company)
        {
            if (company == null)
                return null;

            try
            {
                var dbCompany = db.Company.SingleOrDefault(x => x.IdCompany == company.IdCompany);
                if (dbCompany == null)
                    return null;
                dbCompany.Address = company.Address;
                dbCompany.CompanyName = company.CompanyName;
                dbCompany.FiscalCode = company.FiscalCode;
                dbCompany.PECEmail = company.PECEmail;
                dbCompany.TemplateADEFile = company.TemplateADEFile;
                dbCompany.TemplateCloseFile = company.TemplateCloseFile;
                dbCompany.TemplateIndexFile = company.TemplateIndexFile;

                SaveChanges();
            }
            finally
            {
                Dispose();
            }

            return company;
        }

       public List<Objects.Company> GetCustomerCompanies(string idCustomer)
        {
            try
            {
                List<Model.Company> currentUserCompanyList = db.Company
                    .Where(x => x.CustomerCompany.Select(y => y.IdCustomer).Contains(idCustomer)).ToList();

                List<Objects.Company> listCurrentUserCompany = new List<Objects.Company>();

                foreach (Model.Company item in currentUserCompanyList)
                {
                    listCurrentUserCompany.Add(new Objects.Company()
                    {
                        IdCompany = item.IdCompany,
                        CompanyName = item.CompanyName
                    });
                }
                                
                return listCurrentUserCompany;
            }
            finally
            {
                Dispose();
            }
        }

        public BiblosDS.Library.Common.Objects.Company AddCompany(BiblosDS.Library.Common.Objects.Company company)
        {
            if (company == null)
                return null;

            try
            {
                var toAdd = new Model.Company
                {
                    IdCompany = company.IdCompany,
                    Address = company.Address,
                    CompanyName = company.CompanyName,
                    FiscalCode = company.FiscalCode,
                    PECEmail = company.PECEmail,
                    TemplateADEFile = company.TemplateADEFile,
                    TemplateCloseFile = company.TemplateCloseFile,
                    TemplateIndexFile = company.TemplateIndexFile,
                };

                db.Company.AddObject(toAdd);

                if (requireSave)
                {
                    db.SaveChanges();
                }
            }
            finally
            {
                Dispose();
            }

            return company;
        }

        public Objects.ArchiveCompany UpdateArchiveCompany(Objects.ArchiveCompany archiveCompany)
        {
            if (archiveCompany == null)
                return null;

            try
            {
                Model.ArchiveCompany dbArchiveCompany = db.ArchiveCompany.SingleOrDefault(x => x.IdArchive == archiveCompany.IdArchive && x.IdCompany == archiveCompany.IdCompany);
                if (dbArchiveCompany == null)
                    return null;

                dbArchiveCompany.WorkingDir = archiveCompany.WorkingDir;
                dbArchiveCompany.XmlFileTemplatePath = archiveCompany.XmlFileTemplatePath;
                dbArchiveCompany.TemplateXSLTFile = archiveCompany.TemplateXSLTFile;
                dbArchiveCompany.AwardBatchXSLTFile = archiveCompany.AwardBatchXSLTFile;

                if (requireSave)
                {
                    db.SaveChanges();
                }
            }
            finally
            {
                Dispose();
            }

            return archiveCompany;
        }

        public BiblosDS.Library.Common.Objects.ArchiveCompany AddArchiveCompany(BiblosDS.Library.Common.Objects.ArchiveCompany archiveCompany)
        {
            if (archiveCompany == null)
                return null;

            try
            {
                var toAdd = new Model.ArchiveCompany
                {
                    IdArchive = archiveCompany.IdArchive,
                    IdCompany = archiveCompany.IdCompany,
                    WorkingDir = archiveCompany.WorkingDir,
                    XmlFileTemplatePath = archiveCompany.XmlFileTemplatePath,
                    TemplateXSLTFile = archiveCompany.TemplateXSLTFile
                };

                db.ArchiveCompany.AddObject(toAdd);

                if (requireSave)
                {
                    db.SaveChanges();
                }
            }
            finally
            {
                Dispose();
            }

            return archiveCompany;
        }

        #endregion

        #region Updaters

        public void UpdatePreservationModifyField(Preservation preservation)
        {
            try
            {
                var entity = this.db.Preservation
                    .Single(x => x.IdPreservation == preservation.IdPreservation);

                if (preservation.IsOnChangeList(() => preservation.Path))
                    entity.Path = preservation.Path;

                if (preservation.IsOnChangeList(() => preservation.PathHash))
                    entity.PathHash = preservation.PathHash;

                if (preservation.IsOnChangeList(() => preservation.Label))
                    entity.Label = preservation.Label;

                if (preservation.IsOnChangeList(() => preservation.StartDate))
                    entity.StartDate = preservation.StartDate;

                if (preservation.IsOnChangeList(() => preservation.EndDate))
                    entity.EndDate = preservation.EndDate;

                if (preservation.IsOnChangeList(() => preservation.CloseDate))
                    entity.CloseDate = preservation.CloseDate;

                if (preservation.IsOnChangeList(() => preservation.CloseContent))
                    entity.CloseContent = preservation.CloseContent;

                if (preservation.IsOnChangeList(() => preservation.IndexHash))
                    entity.IndexHash = preservation.IndexHash;

                if (preservation.IsOnChangeList(() => preservation.IdDocumentCloseFile))
                    entity.IdDocumentClose = preservation.IdDocumentCloseFile;

                if (preservation.IsOnChangeList(() => preservation.IdDocumentIndexFile))
                    entity.IdDocumentIndex = preservation.IdDocumentIndexFile;

                if (preservation.IsOnChangeList(() => preservation.IdDocumentIndexFileXML))
                    entity.IdDocumentIndexXml = preservation.IdDocumentIndexFileXML;

                if (preservation.IsOnChangeList(() => preservation.IdDocumentIndexFileXSLT))
                    entity.IdDocumentIndexXSLT = preservation.IdDocumentIndexFileXSLT;

                if (preservation.IsOnChangeList(() => preservation.IdDocumentSignedIndexFile))
                    entity.IdDocumentIndedSigned = preservation.IdDocumentSignedIndexFile;

                if (preservation.IsOnChangeList(() => preservation.IdDocumentSignedCloseFile))
                    entity.IdDocumentCloseSigned = preservation.IdDocumentSignedCloseFile;

                if (preservation.IsOnChangeList(() => preservation.PreservationSize))
                    entity.PreservationSize = preservation.PreservationSize;

                if (preservation.IsOnChangeList(() => preservation.IdArchiveBiblosStore))
                    entity.IdArchiveBiblosStore = preservation.IdArchiveBiblosStore;

                if (preservation.IsOnChangeList(() => preservation.LockOnDocumentInsert))
                    entity.LockOnDocumentInsert = preservation.LockOnDocumentInsert;
                if (requireSave)
                {
                    this.db.SaveChanges();
                    preservation.ClearModifiedField();
                }
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdatePreservation(Preservation preservation, bool withDocument)
        {
            try
            {
                if (preservation == null || preservation.IdPreservation == Guid.Empty)
                {
                    return;
                }

                Model.Preservation entity = db.Preservation
                    .Where(x => x.IdPreservation == preservation.IdPreservation).SingleOrDefault();

                if (entity == null)
                {
                    throw new Exception($"La conservazione con ID {preservation.IdPreservation} non è stata trovata");
                }

                preservation.Convert(db, entity);
                if (preservation.User != null)
                {
                    entity.IdPreservationUser = preservation.User.IdPreservationUser;
                }

                if (withDocument && preservation.Documents?.Count > 0)
                {
                    ICollection<PreservationDocuments> preservationDocuments = db.PreservationDocuments.Where(x => x.IdPreservation == preservation.IdPreservation).ToList();
                    ICollection<Document> toInsertDocuments = preservation.Documents.Where(x => !preservationDocuments.Any(pd => pd.IdDocument == x.IdDocument)).ToList();
                    Model.PreservationDocuments preservationDocument;
                    foreach (Document document in toInsertDocuments)
                    {
                        preservationDocument = new PreservationDocuments
                        {
                            IdPreservationDocument = Guid.NewGuid(),
                            IdDocument = document.IdDocument,
                            IdPreservation = preservation.IdPreservation,
                            RegistrationUser = "BiblosDS",
                            RegistrationDate = DateTimeOffset.UtcNow
                        };
                        db.PreservationDocuments.AddObject(preservationDocument);
                    }
                }
                logger.InfoFormat("UpdatePreservation: requireSave = {0}", requireSave);
                if (requireSave)
                     this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdatePreservationUser(PreservationUser user, Guid idArchive)
        {
            if (user != null && user.IdPreservationUser != Guid.Empty)
            {

                var entity = this.db.PreservationUser
                    .Include(x => x.PreservationUserRole)
                    .Include(x => x.PreservationUserRole.First().Archive)
                    .Where(x => x.IdPreservationUser == user.IdPreservationUser)
                    .SingleOrDefault();

                try
                {
                    entity.DomainUser = user.DomainUser;
                    entity.Name = user.Name;
                    entity.Surname = user.Surname;
                    entity.FiscalId = user.FiscalId;
                    entity.Address = user.Address;
                    entity.Email = user.EMail;
                    entity.Enable = user.Enabled;
                    //Cancella i ruoli associati all'utente.
                    Model.PreservationUserRole rol;
                    var ruoli = entity.PreservationUserRole
                        .Where(x => x.IdArchive == idArchive);

                    while ((rol = ruoli.FirstOrDefault()) != null)
                    {
                        this.db.PreservationUserRole.DeleteObject(rol);
                    }
                    //Persiste le modifiche su db.
                    //                    this.db.SaveChanges();
                    //Esegue la mappatura utente - ruolo.
                    if (user.UserRoles != null)
                    {
                        foreach (var usrRole in user.UserRoles)
                        {
                            if (usrRole.IdPreservationUser == Guid.Empty)
                                usrRole.IdPreservationUser = usrRole.PreservationUser.IdPreservationUser;

                            if (usrRole.IdPreservationRole == Guid.Empty)
                                usrRole.IdPreservationRole = usrRole.PreservationRole.IdPreservationRole;

                            rol = this.db.PreservationUserRole
                                .Where(x => x.IdPreservationUserRole == usrRole.IdPreservationUserRole
                                    || (x.IdPreservationUser == usrRole.IdPreservationUser && x.IdPreservationRole == usrRole.IdPreservationRole && x.IdArchive == idArchive))
                                .SingleOrDefault();

                            if (rol != null && rol.EntityState != EntityState.Deleted)
                                entity.PreservationUserRole.Add(new Model.PreservationUserRole { IdPreservationUser = usrRole.IdPreservationUserRole, IdPreservationUserRole = usrRole.IdPreservationUser, IdArchive = idArchive });
                            else
                                this.AddPreservationUserRole(usrRole, idArchive);
                        }
                    }

                    if (requireSave)
                        this.db.SaveChanges();

                    Dispose();
                }
                catch (Exception exx)
                {
                    Dispose();
                    throw exx;
                }
            }
        }

        public void UpdatePreservationSchedule(PreservationSchedule sched, Guid idArchive)
        {
            if (sched != null && sched.IdPreservationSchedule != Guid.Empty)
            {
                var entity = this.db.PreservationSchedule
                    .Where(x => x.IdPreservationSchedule == sched.IdPreservationSchedule)
                    .SingleOrDefault();

                if (entity == null)
                    new PreservationError("Nessuno scadenziario trovato con ID = " + sched.IdPreservationSchedule, PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();

                Model.PreservationSchedule defaultSchedule = null;

                try
                {
                    //Check sullo scadenziario di default.
                    if (sched.Default)
                    {
                        defaultSchedule = db.PreservationSchedule
                            .Where(x => x.IsDefault == 1)
                            .SingleOrDefault();

                        if (defaultSchedule != null)
                        {
                            defaultSchedule.IsDefault = 0; //Aggiorno il flag: lo scadenziario di default sarà dunque quello che si vuole aggiornare.
                            db.SaveChanges();
                        }
                    }

                    entity.Active = (short)((sched.Active) ? 1 : 0);
                    entity.FrequencyType = sched.FrequencyType;
                    entity.Name = sched.Name;
                    entity.Period = sched.Period;
                    entity.ValidWeekDays = sched.ValidWeekDays;
                    entity.IsDefault = (short)(sched.Default ? 0x01 : 0x00);

                    var dbPreservationScheduleTaskType = db.PreservationSchedule_TaskType.Where(x => x.IdPreservationSchedule == sched.IdPreservationSchedule);
                    foreach (var item in dbPreservationScheduleTaskType)
                    {
                        if (!sched.PreservationScheduleTaskTypes.Any(x => x.IdPreservationTaskType == item.IdPreservationTaskType && x.IdPreservationSchedule == item.IdPreservationSchedule))
                            db.DeleteObject(item);
                    }

                    if (sched.PreservationScheduleTaskTypes != null)
                    {
                        foreach (var item in sched.PreservationScheduleTaskTypes)
                        {
                            this.AddPreservationScheduleTaskType(item, idArchive);
                        }
                    }

                    if (requireSave)
                        db.SaveChanges();
                }
                catch (Exception exx)
                {
                    if (defaultSchedule != null)
                    {
                        defaultSchedule.IsDefault = 0x01;
                        db.SaveChanges();
                    }
                    throw exx;
                }
                finally
                {
                    Dispose();
                }
            }
        }

        public void UpdatePreservationTaskType(PreservationTaskType taskType, Guid idArchive)
        {
            if (taskType != null && taskType.Roles != null && taskType.Roles.Count > 0 && taskType.IdPreservationTaskType != Guid.Empty)
            {
                var entity = this.db.PreservationTaskType
                    .Where(x => x.IdPreservationTaskType == taskType.IdPreservationTaskType)
                    .SingleOrDefault();

                if (entity != null)
                {
                    entity.Description = taskType.Description;
                    //entity.Period = taskType.Period;

                    var ruolo = entity.PreservationTaskRole.SingleOrDefault();

                    if (ruolo == null)
                        new PreservationError("Nessun ruolo associato al tipo task con ID = " + taskType.IdPreservationTaskType, PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();

                    ruolo.IdPreservationRole = taskType.Roles.First().IdPreservationRole;

                    if (requireSave)
                        this.db.SaveChanges();

                    Dispose();
                }
            }
        }

        public void UpdatePreservationTaskGroupExpiryByTask(Guid idPreservationTask, DateTime newEstimatedExpiry, Guid idArchive)
        {
            if (idPreservationTask != Guid.Empty && idArchive != Guid.Empty)
            {
                //SqlCommand oInsCommandGT = new SqlCommand("UPDATE Task SET DataScadenza=@1 WHERE IdTask=@2", m_Connection);

                var tasks = this.db.PreservationTask
                    .Include(x => x.PreservationTaskGroup)
                    .Where(x => x.IdPreservationTask == idPreservationTask
                        && x.IdArchive == idArchive);

                if (tasks.Count() > 0)
                {
                    foreach (var t in tasks)
                    {
                        t.EstimatedDate = newEstimatedExpiry;

                        //oInsCommandGT = new SqlCommand("UPDATE GruppoTask" +
                        //                               "   SET GruppoTask.Scadenza=@1 " +
                        //                               "  FROM GruppoTask" +
                        //                               " INNER JOIN Task ON GruppoTask.IdGruppoTask=Task.IdGruppoTask " +
                        //                               "                AND Task.IdTask=@2" +
                        //                               " WHERE GruppoTask.Scadenza<@3", m_Connection);

                        if (t.PreservationTaskGroup != null && t.PreservationTaskGroup.Expiry < newEstimatedExpiry)
                            t.PreservationTaskGroup.Expiry = newEstimatedExpiry;
                    }

                    if (requireSave)
                        this.db.SaveChanges();

                    Dispose();
                }
            }
        }

        public void UpdatePreservationHoliday(PreservationHoliday holiday, Guid idArchive)
        {
            if (holiday != null && idArchive != Guid.Empty)
            {
                var entity = this.db.PreservationHolidays
                    .Where(x => x.IdPreservationHolidays == holiday.IdPreservationHolidays)
                    .SingleOrDefault();

                if (entity != null)
                {
                    entity.Description = holiday.Description;
                    entity.HolidayDate = holiday.HolidayDate;

                    if (requireSave)
                        this.db.SaveChanges();

                    Dispose();
                }
            }
        }

        public void UpdatePreservationAlertType(PreservationAlertType alertType, Guid idArchive)
        {
            if (alertType != null && alertType.IdPreservationAlertType != Guid.Empty && idArchive != Guid.Empty)
            {
                try
                {
                    var entity = this.db.PreservationAlertType
                        .Include(x => x.PreservationAlertTask)
                        .Where(x => x.IdPreservationAlertType == alertType.IdPreservationAlertType)
                        .SingleOrDefault();

                    entity.AlertText = alertType.AlertText;
                    entity.Offset = alertType.Offset;

                    if (alertType.Role != null && alertType.Role.IdPreservationRole != Guid.Empty)
                        entity.IdPreservationRole = alertType.Role.IdPreservationRole;

                    if (alertType.TaskTypes != null && alertType.TaskTypes.Count > 0)
                    {
                        Model.PreservationTaskType t;

                        Model.PreservationAlertTask pat;
                        while ((pat = entity.PreservationAlertTask.FirstOrDefault()) != null)
                        {
                            this.db.PreservationAlertTask.DeleteObject(pat);
                        }

                        foreach (var taskType in alertType.TaskTypes)
                        {
                            t = this.db.PreservationTaskType
                                .Where(x => x.IdPreservationTaskType == taskType.IdPreservationTaskType)
                                .SingleOrDefault();

                            if (t != null)
                            {
                                entity.PreservationAlertTask.Add(new Model.PreservationAlertTask { IdPreservationAlertType = entity.IdPreservationAlertType, IdPreservationTaskType = t.IdPreservationTaskType, IdArchive = idArchive });
                            }
                        }
                    }

                    if (requireSave)
                        this.db.SaveChanges();

                    Dispose();
                }
                catch (Exception exx)
                {
                    Dispose();
                    throw exx;
                }
            }
        }

        public void UpdatePreservationRole(PreservationRole role, Guid idArchive)
        {
            if (role != null && role.IdPreservationRole != Guid.Empty && idArchive != Guid.Empty)
            {
                try
                {
                    var entity = this.db.PreservationRole
                        .Include(x => x.PreservationUserRole)
                        .Include(x => x.PreservationUserRole.First().Archive)
                        .Where(x => x.IdPreservationRole == role.IdPreservationRole)
                        .SingleOrDefault();

                    if (entity != null)
                    {
                        int i;
                        //Imposta le informazioni base.
                        entity.Name = role.Name;
                        entity.AlertEnable = role.AlertEnabled;
                        entity.Enable = role.Enabled;
                        //Cancella le associazioni pre-esistenti fra utenti e ruolo.
                        Model.PreservationUserRole existentUserRole = null;
                        for (i = 0; i < entity.PreservationUserRole.Count; i++)
                        {
                            existentUserRole = entity.PreservationUserRole.ElementAt(i);
                            if (existentUserRole.IdArchive == idArchive)
                            {
                                this.db.PreservationUserRole.DeleteObject(existentUserRole);
                                i--;
                            }
                        }
                        this.db.SaveChanges();
                        //Effettua la mappatura utente - ruolo.
                        PreservationUserRole usrRole;
                        if (role.UserRoles != null)
                        {
                            for (i = 0; i < role.UserRoles.Count; i++)
                            {
                                existentUserRole = null;

                                usrRole = role.UserRoles[i];

                                if (usrRole.PreservationRole == null)
                                    new PreservationError("Ruolo utente non configurato correttamente (ruolo non corretto).", PreservationErrorCode.E_INVALID_CALL).ThrowsAsFaultException();

                                if (usrRole.PreservationUser == null)
                                    new PreservationError("Ruolo utente non configurato correttamente (utente non corretto).", PreservationErrorCode.E_INVALID_CALL).ThrowsAsFaultException();

                                usrRole.IdPreservationRole = usrRole.PreservationRole.IdPreservationRole;

                                usrRole.IdPreservationUser = usrRole.PreservationUser.IdPreservationUser;

                                if (usrRole.IdPreservationUserRole != Guid.Empty)
                                {
                                    //Non controlla l'archivio poichè se si entra con IdPreservationUserRole si suppone di avere già le informazioni corrette.
                                    existentUserRole = this.db.PreservationUserRole
                                        .Where(x => x.IdPreservationUserRole == usrRole.IdPreservationUserRole)
                                        .SingleOrDefault();
                                }

                                if (existentUserRole == null)
                                {
                                    existentUserRole = this.db.PreservationUserRole
                                        .Include(x => x.Archive)
                                        .Where(x => x.IdPreservationUser == usrRole.IdPreservationUser
                                            && x.IdPreservationRole == usrRole.IdPreservationRole
                                            && x.IdArchive == idArchive)
                                        .SingleOrDefault();
                                }

                                if (existentUserRole == null)
                                {
                                    role.UserRoles[i] = this.AddPreservationUserRole(usrRole, idArchive);
                                    //usrRole.IdPreservationUserRole = Guid.NewGuid();
                                    //this.db.PreservationUserRole.AddObject(usrRole.Convert(this.db, null, DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                                }
                                else
                                {
                                    entity.PreservationUserRole.Add(existentUserRole);
                                }
                            }
                        }
                    }

                    if (requireSave)
                        this.db.SaveChanges();

                    Dispose();
                }
                catch (Exception exx)
                {
                    Dispose();
                    throw exx;
                }
            }
        }

        public void UpdatePreservationParameter(string label, string value, string filterName, Nullable<Guid> idArchive)
        {
            if (!string.IsNullOrEmpty(label))
            {
                IQueryable<Model.PreservationParameters> query = null;

                if (idArchive.HasValue)
                {
                    query = this.db.PreservationParameters
                        .Include(x => x.Archive)
                        .Where(x => x.IdArchive == idArchive.Value
                                && (string.IsNullOrEmpty(filterName) ? true : x.Label.Equals(filterName, StringComparison.InvariantCultureIgnoreCase))
                                && x.Label
                                    .Equals(label, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    query = this.db.PreservationParameters
                        .Where(x => (string.IsNullOrEmpty(filterName) ? true : x.Label.Equals(filterName, StringComparison.InvariantCultureIgnoreCase))
                                && x.Label
                                    .Equals(label, StringComparison.InvariantCultureIgnoreCase));
                }

                if (query.Count() > 1)
                    new PreservationError("Esistono piu' parametri con lo stesso nome su diversi archivi. Specificare l'archivio di destinazione.", PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();

                var entity = query.SingleOrDefault();

                if (entity != null)
                {
                    entity.Label = label;
                    entity.Value = value;

                    if (requireSave)
                        this.db.SaveChanges();

                }
            }
            Dispose();
        }

        public void UpdatePreservationAsSigned(Guid idPreservation, Guid idArchive)
        {
            //            string qry = @"
            //							UPDATE ConservazioneSostitutiva
            //							SET DataFirma = GETDATE()
            //							WHERE (IdConservazione = " + this.m_Id.ToString() + @")
            //
            //							UPDATE Task
            //							SET DataCompletamento = GETDATE()
            //							WHERE IdTask = (SELECT IdTaskConservazione FROM ConservazioneSostitutiva WHERE IdConservazione = 
            //							" + this.m_Id.ToString() + ")";
            if (idPreservation != Guid.Empty && idArchive != Guid.Empty)
            {
                var entity = this.db.Preservation
                    .Include(x => x.PreservationTask)
                    .Include(x => x.Archive)
                    .Where(x => x.IdPreservation == idPreservation && x.PreservationTask != null && x.IdArchive == idArchive)
                    .SingleOrDefault();

                if (entity != null)
                {
                    entity.LastVerifiedDate = DateTime.Now;
                    entity.PreservationTask.ExecutedDate = DateTime.Now;

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }
            Dispose();
        }

        public void UpdatePreservationPath(Guid idPreservation, string path, Guid idArchive)
        {
            //            string qry = @"
            //							UPDATE ConservazioneSostitutiva
            //							SET PathFileTemporanei = '{0}'
            //							WHERE (IdConservazione = {1})
            //							";
            if (idPreservation != Guid.Empty && idArchive != Guid.Empty)
            {
                var entity = this.db.Preservation
                                .Include(x => x.Archive)
                                .Where(x => x.IdPreservation == idPreservation && x.IdArchive == idArchive)
                                .SingleOrDefault();

                if (entity != null)
                {
                    entity.Path = path;

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }

            Dispose();
        }

        public void UpdatePreservationTaskGroupTypeDescription(Guid idTaskGroupType, string description)
        {
            if (idTaskGroupType != Guid.Empty) //&& !string.IsNullOrEmpty(description)
            {
                var entity = this.db.PreservationTaskGroupType
                    .Where(x => x.IdPreservationTaskGroupType == idTaskGroupType)
                    .SingleOrDefault();

                if (entity != null)
                {
                    entity.Description = description;

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }

            Dispose();
        }

        public List<string> UpdateDocumentsPreservation(Guid? idPreservation, DocumentArchive archive, BindingList<Document> documents, bool verifyPreservationDate)
        {
            var exceptions = new List<string>();

            if (documents != null && documents.Count > 0)
            {
                var arr = documents.Select(x => x.IdDocument)
                    .ToArray();

                try
                {
                    var docs = this.db.Document
                        .Include(x => x.AttributesValue)
                        .Include(x => x.AttributesValue.First().Attributes)
                        .Include(x => x.PreservationDocuments)
                        .Where(x => arr.Contains(x.IdDocument)).ToList();

                    long count = 0;
                    /*
                     * 
                     */
                    //if (docs.Select(x => x.Archive).Take(1).Single().VerifyPreservationDateEnabled.GetValueOrDefault(false))
                    IOrderedEnumerable<Model.Document> documenti = null;

                    if (verifyPreservationDate)
                        documenti = docs.OrderBy(x => GetOrderAttributeValueString(x.AttributesValue.Where(a => a.Attributes.IsSectional.HasValue && a.Attributes.IsSectional.Value))).ThenBy(x => x.DateMain).ThenBy(x => GetOrderAttributeValue(x.AttributesValue.Where(a => a.Attributes.IsAutoInc.HasValue && a.Attributes.IsAutoInc.Value == 1)));
                    else
                        documenti = docs.OrderBy(x => GetOrderAttributeValueString(x.AttributesValue.Where(a => a.Attributes.IsSectional.HasValue && a.Attributes.IsSectional.Value))).ThenBy(x => GetOrderAttributeValue(x.AttributesValue.Where(a => a.Attributes.IsAutoInc.HasValue && a.Attributes.IsAutoInc.Value == 1)));

                    PreservationDocuments preservationDocument;
                    foreach (var doc in documenti)
                    {

                        count += 1;

                        logger.DebugFormat("Set Index {0} - Key {1} - Doc: {2} {4} - Date: {3}",
                            count,
                            GetOrderAttributeValue(doc.AttributesValue.Where(a => a.Attributes.IsAutoInc.HasValue && a.Attributes.IsAutoInc.Value == 1)),
                            doc.IdBiblos, doc.DateMain, doc.PrimaryKeyValue);

                        preservationDocument = new PreservationDocuments
                        {
                            IdPreservationDocument = Guid.NewGuid(),
                            IdPreservation = idPreservation.Value,
                            PreservationIndex = count,
                            RegistrationUser = "BiblosDS",
                            RegistrationDate = DateTimeOffset.UtcNow
                        };
                        doc.PreservationDocuments.Add(preservationDocument);

                        doc.IsConservated = 1;

                        var d = documents.Where(x => x.IdDocument == doc.IdDocument)
                            .SingleOrDefault();

                        if (d != null)
                        {
                            d.PreservationIndex = count;
                            d.IdPreservation = idPreservation;
                            d.IsConservated = true;
                            d.PrimaryKeyValue = doc.PrimaryKeyValue;
                            d.DateMain = doc.DateMain;
                        }
                    }

                    if (requireSave && exceptions.Count() <= 0)
                        this.db.SaveChanges();

                    Dispose();
                }
                catch (Exception exx)
                {
                    Dispose();
                    throw exx;
                }
            }
            return exceptions;
        }

        private string GetOrderAttributeValueString(IEnumerable<AttributesValue> attrs)
        {
            List<string> res = new List<string>();
            foreach (var val in attrs.OrderBy(x => x.Attributes.KeyOrder.GetValueOrDefault()))
            {
                if (val == null)
                    continue;
                switch (val.Attributes.AttributeType)
                {
                    case "System.Int64":
                        res.Add(val.ValueInt.GetValueOrDefault().ToString());
                        break;
                    case "System.Double":
                        res.Add(val.ValueFloat.GetValueOrDefault().ToString());
                        break;
                    case "System.DateTime":
                        res.Add(val.ValueDateTime.GetValueOrDefault().ToString());
                        break;
                    default:
                        res.Add(val.ValueString.ToUpper());
                        break;
                }
            }
            return string.Join("|", res);
        }

        private long GetOrderAttributeValue(IEnumerable<AttributesValue> attrs)
        {
            if (attrs.Count() > 1)
            {
                logger.WarnFormat("Attributo non univoco: {0}", string.Join("|", attrs.Select(x => x.Attributes.Name)));
            }
            var val = attrs.FirstOrDefault();
            if (val == null)
                return 0;
            if (val.Attributes.AttributeType == "System.Int64")
                return val.ValueInt.GetValueOrDefault();
            else if (val.Attributes.AttributeType == "System.Double")
                return (Int64)val.ValueFloat.GetValueOrDefault();
            else if (!string.IsNullOrEmpty(val.ValueString.ToStringExt()))
            {
                long value = 0;
                long.TryParse(val.ValueString, out value);
                return value;
            }
            return 0;
        }

        public void UpdatePreservationJournaling(PreservationJournaling toUpdate)
        {
            if (toUpdate == null || toUpdate.IdPreservationJournaling == Guid.Empty)
                return;

            var entity = this.db.PreservationJournaling
                .Where(x => x.IdPreservationJournaling == toUpdate.IdPreservationJournaling)
                .SingleOrDefault();

            if (entity != null)
            {
                entity = toUpdate.Convert(this.db, entity, DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);

                if (requireSave)
                    this.db.SaveChanges();
            }

            Dispose();
        }

        public void UpdateEntratelFileName(Guid idPreservationStorageDevice, string entratelFileName)
        {
            var dev = db.PreservationStorageDevice.Where(x => x.IdPreservationStorageDevice == idPreservationStorageDevice)
                .Single();

            dev.EntratelCompleteFileName = entratelFileName;
            dev.EntratelUploadDate = DateTime.Now;

            db.SaveChanges();
        }

        public void UpdatePreservationVerifyResetWarningsAndTaskErrors(Guid idPreservation)
        {
            logger.InfoFormat("UpdatePreservationVerifyResetWarningsAndTaskErrors - id {0}", idPreservation);
            try
            {
                var presVerify = db.PreservationVerify
                    .Where(x => x.IdPreservation == idPreservation && x.Warning != null);

                foreach (var ver in presVerify)
                {
                    ver.Warning = null;
                }

                var tasks = db.PreservationTask
                    .Where(x => x.IdPreservation == idPreservation && x.HasError);

                foreach (var t in tasks)
                {
                    t.HasError = false;
                    t.ErrorMessages = null;
                }

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
            finally
            {
                logger.Info("END");
                Dispose();
            }
        }

        #endregion

        #region Deleters

        public void DeletePreservation(Guid idPreservation)
        {
            try
            {
                if (idPreservation != Guid.Empty)
                {
                    Model.Preservation preservationToDelete = db.Preservation.SingleOrDefault(x => x.IdPreservation == idPreservation);
                    IQueryable<Model.PreservationDocuments> relationsToDelete = db.PreservationDocuments.Where(x => x.IdPreservation == idPreservation);
                    IQueryable<Model.Document> documentsToDelete = db.Document.Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation));

                    if (preservationToDelete != null)
                    {
                        foreach (Model.PreservationDocuments relation in relationsToDelete)
                        {
                            db.PreservationDocuments.DeleteObject(relation);
                        }

                        foreach (Model.Document document in documentsToDelete)
                        {
                            document.IsConservated = 0;
                        }

                        db.Preservation.DeleteObject(preservationToDelete);
                    }

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }
            catch
            {
                Dispose();
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Elimina tutti i ruoli associati ad un utente autorizzato alla conservazione.
        /// </summary>
        /// <param name="idPreservationUser"></param>
        public void DeletePreservationUserRolesByPreservationUser(Guid idPreservationUser)
        {
            //private string m_CmdDeleteRuoli = "DELETE FROM Ruolo_Soggetto WHERE IdSoggetto = @IdSoggetto";

            if (idPreservationUser != Guid.Empty)
            {
                var query = this.db.PreservationUserRole
                 .Where(x => x.IdPreservationUser == idPreservationUser);

                Model.PreservationUserRole userRole;
                while ((userRole = query.FirstOrDefault()) != null)
                {
                    if (userRole.EntityState != EntityState.Deleted && userRole.EntityState != EntityState.Detached)
                        this.db.PreservationUserRole.DeleteObject(userRole);
                }

                if (requireSave)
                    this.db.SaveChanges();
            }

            Dispose();
        }

        public void DeletePreservationUser(Guid idPreservationUser)
        {
            if (idPreservationUser != Guid.Empty)
            {
                var entity = this.db.PreservationUser
                    .Include(x => x.Preservation)
                    .Include(x => x.PreservationTask)
                    .Include(x => x.PreservationTask.First().PreservationTaskStatus)
                    .Include(x => x.PreservationTask.First().PreservationAlert)
                    .Include(x => x.PreservationTaskGroup)
                    .Include(x => x.PreservationTaskGroup.First().PreservationSchedule)
                    .Include(x => x.PreservationTaskGroup.First().PreservationSchedule.PreservationSchedule_TaskType)
                    .Include(x => x.PreservationUserRole)
                    .Where(x => x.IdPreservationUser == idPreservationUser)
                    .SingleOrDefault();

                if (entity != null)
                {
                    //Cancella i ruoli associati all'utente.
                    Model.PreservationUserRole rol;
                    while ((rol = entity.PreservationUserRole.FirstOrDefault()) != null)
                    {
                        this.db.PreservationUserRole.DeleteObject(rol);
                    }

                    //Elimina il gruppo task.
                    Model.PreservationTaskGroup group;
                    while ((group = entity.PreservationTaskGroup.FirstOrDefault()) != null)
                    {
                        //Via lo scadenziario.
                        var sched = group.PreservationSchedule;
                        if (sched != null)
                        {
                            //Elimina la mappatura schedule - task type
                            foreach (var tt in sched.PreservationSchedule_TaskType)
                            {
                                this.db.PreservationSchedule_TaskType.DeleteObject(tt);
                            }

                            this.db.PreservationSchedule.DeleteObject(sched);
                        }
                        //Fa fuori i task
                        Model.PreservationTask task;
                        while ((task = group.PreservationTask.FirstOrDefault()) != null)
                        {
                            //Via gli avvisi!
                            foreach (var alert in task.PreservationAlert)
                            {
                                this.db.PreservationAlert.DeleteObject(alert);
                            }

                            this.db.PreservationTask.DeleteObject(task);
                        }
                    }

                    //Fa fuori le conservazioni associate all'utente.
                    Model.Preservation pres;
                    while ((pres = entity.Preservation.FirstOrDefault()) != null)
                    {
                        this.db.Preservation.DeleteObject(pres);
                    }

                    this.db.PreservationUser.DeleteObject(entity);

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }

            Dispose();
        }

        public void DeletePreservationSchedule(Guid idPreservationSchedule)
        {
            if (idPreservationSchedule != Guid.Empty)
            {
                var entity = this.db.PreservationSchedule
                    .Where(x => x.IdPreservationSchedule == idPreservationSchedule)
                    .SingleOrDefault();

                if (entity != null)
                {
                    this.DeletePreservationSchedule_TaskTypeBySchedule(entity.IdPreservationSchedule);
                    this.db.PreservationSchedule.DeleteObject(entity);

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }

            Dispose();
        }

        public void DeletePreservationSchedule_TaskTypeBySchedule(Guid idPreservationSchedule)
        {
            if (idPreservationSchedule != Guid.Empty)
            {
                var entities = this.db.PreservationSchedule_TaskType
                    .Where(x => x.IdPreservationSchedule == idPreservationSchedule);

                foreach (var sched in entities)
                {
                    this.db.PreservationSchedule_TaskType.DeleteObject(sched);
                }

                if (entities.Count() > 0)
                    this.db.SaveChanges();
            }

            Dispose();
        }

        public void DeletePreservationHoliday(Guid idPreservationHoliday, Guid idArchive)
        {
            if (idPreservationHoliday != Guid.Empty && idArchive != Guid.Empty)
            {
                var entity = this.db.PreservationHolidays
                    .Where(x => x.IdPreservationHolidays == idPreservationHoliday)
                    .SingleOrDefault();

                if (entity != null)
                {
                    this.db.PreservationHolidays.DeleteObject(entity);

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }

            Dispose();
        }

        public void DeletePreservationAlertType(Guid idPreservationAlertType, Guid idArchive)
        {
            try
            {
                var entity = this.db.PreservationAlertType
                    .Include(x => x.PreservationAlertTask)
                    .Where(x => x.IdPreservationAlertType == idPreservationAlertType)
                    .SingleOrDefault();

                if (entity != null)
                {
                    Model.PreservationAlertTask pat;
                    while ((pat = entity.PreservationAlertTask.FirstOrDefault()) != null)
                    {
                        this.db.PreservationAlertTask.DeleteObject(pat);
                    }

                    this.db.PreservationAlertType.DeleteObject(entity);

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }
            catch (UpdateException exx)
            {
                logger.Error(exx);
                new PreservationError("Impossibile eliminare il tipo di avviso con id = " + idPreservationAlertType, PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();
            }
            finally
            {
                Dispose();
            }
        }

        public void DeletePreservationParameter(string label, Nullable<Guid> idArchive)
        {
            if (!string.IsNullOrEmpty(label))
            {
                IQueryable<Model.PreservationParameters> query;

                if (idArchive.HasValue)
                {
                    query = this.db.PreservationParameters
                        .Include(x => x.Archive)
                        .Where(x => x.IdArchive == idArchive.Value
                            && x.Label
                                .Equals(label, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    query = this.db.PreservationParameters
                        .Where(x => x.Label
                            .Equals(label, StringComparison.InvariantCultureIgnoreCase));
                }

                if (query.Count() > 1)
                    new PreservationError("Esistono piu' parametri con lo stesso nome su diversi archivi. Specificare l'archivio di destinazione.", PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();

                var entity = query.SingleOrDefault();

                if (entity != null)
                {
                    this.db.PreservationParameters.DeleteObject(entity);

                    if (requireSave)
                        this.db.SaveChanges();
                }
            }

            Dispose();
        }
        public void DeletePreservationTask(Guid idPreservationTask)
        {
            try
            {
                if (idPreservationTask != Guid.Empty)
                {
                    var query = db.PreservationTask
                       .Include(x => x.PreservationTask1)
                       .Include(x => x.PreservationTask2)
                       .Include(x => x.PreservationTask2.PreservationTask1)
                       .SingleOrDefault<Model.PreservationTask>(x => x.IdPreservationTask == idPreservationTask);

                    if (query != null)
                    {
                        var toDelete = query.PreservationTask1.FirstOrDefault();
                        if (toDelete != null)
                        {
                            db.PreservationTask.DeleteObject(toDelete);
                        }

                        db.PreservationTask.DeleteObject(query);

                        if (requireSave)
                        {
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }
        }
        public void DeletePreservationTaskGroup(Guid idTaskGroup, Guid idArchive)
        {
            if (idTaskGroup != Guid.Empty && idArchive != Guid.Empty)
            {
                try
                {
                    var taskGroup = this.db.PreservationTaskGroup
                        .Include(x => x.PreservationTaskGroupType)
                        .Include(x => x.Preservation)
                        .Include(x => x.PreservationTask)
                        .Include(x => x.PreservationTask.First().Archive)
                        .Include(x => x.PreservationTask.First().PreservationAlert)
                        .Include(x => x.PreservationTask.First().PreservationAlert.First().PreservationAlertType)
                        .Include(x => x.PreservationTask.First().PreservationAlert.First().PreservationAlertType.PreservationAlertTask)
                        .Include(x => x.PreservationTask.First().PreservationTaskStatus)
                        .Where(x => x.IdPreservationTaskGroup == idTaskGroup)
                        .SingleOrDefault();

                    if (taskGroup != null)
                    {
                        Model.PreservationTask task;
                        Model.PreservationAlert alert;
                        Model.Preservation pres;
                        Model.PreservationAlertTask alertTask;
                        //Elimina i task associati al gruppo.
                        while ((task = taskGroup.PreservationTask.FirstOrDefault()) != null)
                        {
                            //Elimina gli avvisi associati al task.
                            while ((alert = task.PreservationAlert.FirstOrDefault()) != null)
                            {
                                if (alert.PreservationAlertType != null)
                                {
                                    //Elimina l'associazione TIPO TASK - TIPO AVVISO
                                    while ((alertTask = alert.PreservationAlertType.PreservationAlertTask.FirstOrDefault()) != null)
                                    {
                                        this.db.PreservationAlertTask.DeleteObject(alertTask);
                                    }
                                }

                                this.db.PreservationAlert.DeleteObject(alert);
                            }
                            //Elimina le conservarzioni associate al task.
                            while ((pres = task.Preservation.FirstOrDefault()) != null)
                            {
                                this.db.Preservation.DeleteObject(pres);
                            }
                            //Elimina l'eventuale stato del task.
                            if (task.PreservationTaskStatus != null)
                                this.db.PreservationTaskStatus.DeleteObject(task.PreservationTaskStatus);

                            this.db.PreservationTask.DeleteObject(task);
                        }
                        //Elimina le conservazioni associate al gruppo task.
                        while ((pres = taskGroup.Preservation.FirstOrDefault()) != null)
                        {
                            this.db.Preservation.DeleteObject(pres);
                        }

                        this.db.PreservationTaskGroup.DeleteObject(taskGroup);


                    }

                    if (requireSave)
                        this.db.SaveChanges();

                    Dispose();
                }
                catch (Exception exx)
                {
                    Dispose();
                    throw exx;
                }
            }
        }

        public void DeletePreservationJournaling(Guid idJournaling, Guid idArchive)
        {
            using (var transaction = new TransactionScope())
            {
                var query = this.db.PreservationJournaling
                    .Include(x => x.Preservation)
                    .Where(x => x.IdPreservationJournaling == idJournaling && x.Preservation.IdArchive == idArchive);

                foreach (var journal in query)
                {
                    this.db.PreservationJournaling.DeleteObject(journal);
                }

                if (requireSave)
                    this.db.SaveChanges();

                transaction.Complete();
            }

            Dispose();
        }

        public void DeletePreservationStorageDevice(PreservationStorageDevice preservationStorageDevice)
        {
            if (preservationStorageDevice != null)
            {
                var entity = db.PreservationStorageDevice
                    .Include(x => x.PreservationInStorageDevice)
                    .Where(x => x.IdPreservationStorageDevice == preservationStorageDevice.IdPreservationStorageDevice).SingleOrDefault();

                if (entity != null)
                {
                    if (entity.PreservationInStorageDevice != null)
                    {
                        while (entity.PreservationInStorageDevice.Count > 0)
                        {
                            db.PreservationInStorageDevice.DeleteObject(entity.PreservationInStorageDevice.First());
                        }
                    }
                    db.PreservationStorageDevice.DeleteObject(entity);
                    if (requireSave)
                        db.SaveChanges();
                }
            }
            Dispose();
        }

        public void DeletePreservationInStorageDevice(PreservationInStorageDevice preservationInStorageDevice)
        {
            if (preservationInStorageDevice != null)
            {
                var entity = db.PreservationInStorageDevice
                    .Where(x => x.IdPreservation == preservationInStorageDevice.Preservation.IdPreservation &&
                                x.IdPreservationStorageDevice == preservationInStorageDevice.Device.IdPreservationStorageDevice).SingleOrDefault();

                if (entity != null)
                {
                    db.PreservationInStorageDevice.DeleteObject(entity);
                    if (requireSave)
                        db.SaveChanges();
                }
            }
            Dispose();
        }

        public void DeletePreservationVerifyResetTaskErrors(Guid idPreservation)
        {
            logger.InfoFormat("DeletePreservationVerifyResetTaskErrors - id {0}", idPreservation);
            try
            {
                var presVerify = db.PreservationVerify
                    .Where(x => x.IdPreservation == idPreservation);

                foreach (var entity in presVerify)
                {
                    db.PreservationVerify.DeleteObject(entity);
                }

                var tasks = db.PreservationTask
                    .Where(x => x.IdPreservation == idPreservation && x.HasError);

                foreach (var t in tasks)
                {
                    t.HasError = true;
                    t.ErrorMessages = null;
                }

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
            finally
            {
                logger.Info("END");
                Dispose();
            }
        }

        #endregion

        /// <summary>
        /// IsUserInRole
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="domainUserName"></param>
        /// <param name="idRole"></param>
        /// <returns></returns>
        public bool IsUserInRole(Guid idArchive, string domainUserName, Guid idRole)
        {
            bool retval = false;

            if (!string.IsNullOrEmpty(domainUserName) && idRole != null && idRole != Guid.Empty)
            {
                retval = this.db.PreservationUserRole
                    .Join(this.db.PreservationUser,
                        role => role.IdPreservationUser,
                        user => user.IdPreservationUser,
                        (role, user) => new { User = user, Role = role })
                    .Where(x => (x.User.DomainUser ?? string.Empty)
                                    .Equals(domainUserName, StringComparison.InvariantCultureIgnoreCase)
                                && x.Role.IdPreservationRole == idRole
                                && x.Role.IdArchive == idArchive)
                    //.Count() > 0;
                    .Count() == 1;
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        public bool IsUserInRole(string domainUserName, Guid idRole)
        {
            return db.PreservationUserRole.Any(x => x.IdPreservationRole == idRole && x.PreservationUser.DomainUser.Equals(domainUserName, StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// GetGruppoTaskList
        /// </summary>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<PreservationTaskGroup> GetTaskGroupList(Guid idArchive)
        {
            var retval = new BindingList<PreservationTaskGroup>();

            //var result = this.db.PreservationTaskGroup
            //    .Where(x => (x.Preservation == null || x.Preservation.All(y => !y.CloseDate.HasValue && y.IdArchive == idArchive))
            //        && x.Expiry == this.db.PreservationTaskGroup
            //            .Where(y => y == x
            //                && (y.Preservation == null || y.Preservation.All(z => !z.CloseDate.HasValue && z.IdArchive == idArchive))
            //                && y.PreservationSchedule == x.PreservationSchedule
            //                && !y.Closed.HasValue)
            //            .Select(y => y.Expiry)
            //            .Min()
            //        && !x.Closed.HasValue);

            //result
            //    .ToList()
            //    .ForEach(x => retval.Add(x.Convert()));

            //                                string qry = @" SELECT GruppoTask.IdGruppoTask, Nome, Scadenza
            //                							FROM GruppoTask 
            //                							LEFT JOIN ConservazioneSostitutiva 
            //                							ON GruppoTask.IdGruppoTask = ConservazioneSostitutiva.IdGruppoTask
            //                							INNER JOIN 
            //                							(
            //                								SELECT MIN(GruppoTask.Scadenza) AS MinScad, GruppoTask.IdScadenziario
            //                								FROM GruppoTask 
            //                								LEFT OUTER JOIN ConservazioneSostitutiva ON GruppoTask.IdGruppoTask = ConservazioneSostitutiva.IdGruppoTask
            //                								WHERE (ConservazioneSostitutiva.DataChiusura IS NULL) AND Chiuso IS NULL
            //                								GROUP BY GruppoTask.IdScadenziario
            //                							) AS TblMin ON TblMin.IdScadenziario = GruppoTask.IdScadenziario
            //                							WHERE ConservazioneSostitutiva.DataChiusura IS NULL 
            //                							    AND Scadenza = TblMin.MinScad
            //                                                AND Chiuso IS NULL
            //                							ORDER BY Scadenza
            //                							";

            var query = this.db.PreservationTaskGroup
                .Include(x => x.Preservation)
                .Include(x => x.PreservationSchedule)
                .Include(x => x.PreservationTask)
                .Where(x => (x.Preservation.Count < 1 || x.Preservation.All(pres => !pres.CloseDate.HasValue))
                            && !x.Closed.HasValue
                            && x.IdArchive == idArchive
                            && x.Expiry == this.db.PreservationTaskGroup
                                            .Where(inner => (inner.Preservation.Count < 1 || inner.Preservation.All(innerpres => !innerpres.CloseDate.HasValue))
                                                            && !inner.Closed.HasValue
                                                            && inner.IdArchive == idArchive)
                                            .Select(scad => scad.Expiry)
                                            .Min());


            //var query = from task in db.PreservationTaskGroup
            //            join p in db.Preservation on task.IdPreservationTaskGroup equals p.IdPreservationTaskGroup into preservationOnTaskGroup
            //            from prs in preservationOnTaskGroup.DefaultIfEmpty()
            //            where !task.Closed.HasValue
            //            && task.Expiry == (from t in db.PreservationTaskGroup
            //                               join s in db.Preservation on t.IdPreservationTaskGroup equals s.IdPreservationTaskGroup into prsOnPrs
            //                               from scad in prsOnPrs.DefaultIfEmpty()
            //                               where !scad.CloseDate.HasValue && t.IdPreservationSchedule == task.IdPreservationSchedule
            //                               select t.Expiry).Min()
            //            select task;

            foreach (var item in query)
            {
                retval.Add(item.Convert());
            }

            return retval;
        }
        /// <summary>
        /// SetGruppoTaskChiuso
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <param name="idArchive"></param>
        public void SetTaskGroupClosed(Guid idTaskGroup, Guid idArchive)
        {
            if (idTaskGroup != Guid.Empty)
            {
                /*
                 * string qry = " UPDATE GruppoTask SET Chiuso=GETDATE() WHERE IdGruppoTask="+IdGruppotask+ 
                     " DELETE ConservazioneSostitutiva WHERE IdGruppoTask="+IdGruppotask;
                 */
                var set = this.db.PreservationTaskGroup.Where(x => x.IdPreservationTaskGroup == idTaskGroup);
                Model.Preservation pres;

                foreach (var item in set)
                {
                    item.Closed = DateTime.Now;
                    while ((pres = item.Preservation.FirstOrDefault()) != null)
                    {
                        this.db.Preservation.DeleteObject(pres);
                    }
                }

                if (requireSave)
                    this.db.SaveChanges();

                Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <returns></returns>
        public Guid GetScheduleFromTaskGroup(Guid idTaskGroup)
        {
            Guid retval = Guid.Empty;

            if (idTaskGroup != Guid.Empty)
            {
                var set = this.db.PreservationSchedule
                    .Where(x => x.PreservationTaskGroup
                        .Any(y => y.IdPreservationTaskGroup == idTaskGroup))
                    .FirstOrDefault();

                if (set != null)
                    retval = set.IdPreservationSchedule;
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSchedule"></param>
        /// <returns></returns>
        public BindingList<PreservationSchedule> GetSchedule(Nullable<Guid> idSchedule = null)
        {
            BindingList<PreservationSchedule> retval = new BindingList<PreservationSchedule>();

            if (idSchedule.HasValue)
            {
                if (idSchedule.Value != Guid.Empty)
                {
                    this.db.PreservationSchedule
                        .Where(x => x.IdPreservationSchedule == idSchedule.Value)
                        .ToList()
                        .ForEach(x => retval.Add(x.Convert()));
                }
            }
            else
            {
                this.db.PreservationSchedule
                    .ToList()
                    .ForEach(x => retval.Add(x.Convert()));
            }

            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="idTaskGroup"></param>
        /// <returns></returns>
        [Obsolete("Metodo sostituido da GetAvailableDocumentDateForPreservation", false)]
        public PreservationInfoResponse GetPreservationDateByTask(Guid idArchive, Guid? idTaskGroup)
        {
            var retval = new PreservationInfoResponse();
            var MaxDate = DateTime.MinValue;
            var MinDate = DateTime.MaxValue;

            if (idTaskGroup.HasValue && idTaskGroup.Value != Guid.Empty)
            {
                var res = this.db.PreservationTaskGroup
                    .Where(x => x.IdPreservationTaskGroup == idTaskGroup.Value)
                    .FirstOrDefault();

                if (res != null)
                {
                    retval.DateExpire = res.Expiry;
                }
            }

            var set = this.db.Document
                .Include(x => x.PreservationDocuments.Single().Preservation)
                .Include(x => x.Archive)
                .Where(x => x.IdArchive == idArchive
                    && x.IsVisible == 1
                     && (!x.IsDetached.HasValue || !x.IsDetached.Value)
                    && x.IdParentBiblos.HasValue
                    && !x.PreservationDocuments.Any() || x.PreservationDocuments.Any(pd => !pd.Preservation.CloseDate.HasValue));

            logger.Info(LogQuery(set as ObjectQuery));

            BeginNoSaveNoTransaction();
            var archive = GetArchive(idArchive);
            //TODO verificare se il closedate può creare problemi
            //TODO gestire il fatto che un archivio non legale passa a legale e DateMain non è popolato.
            if (set.Count() > 0)
            {
                logger.Debug("Verifica progressivi valorizzati");
                if (set.Any(x => !x.DateMain.HasValue || string.IsNullOrEmpty(x.PrimaryKeyValue)))
                {
                    logger.Debug("Verifica progressivi valorizzati - Eseguo");
                    foreach (var doc in set)
                    {
                        if (string.IsNullOrEmpty(doc.PrimaryKeyValue) || !doc.DateMain.HasValue)
                        {
                            var docAttributes = GetAttributesValuesFromDocument(doc.IdDocument);
                            logger.DebugFormat("Doc id {0} - {1} recalc primary key and date", doc.IdBiblos, doc.IdDocument);
                            DateTime? mainDate = null;

                            var primaryKeyValue = AttributeService.ParseAttributeValues(archive, docAttributes, out mainDate);

                            doc.PrimaryKeyValue = primaryKeyValue;

                            doc.DateMain = mainDate;
                            UpdatePrimaryKey(doc.IdDocument, primaryKeyValue, mainDate);
                        }
                    }
                    db.SaveChanges();
                    set = this.db.Document
                .Include(x => x.PreservationDocuments.Single().Preservation)
                .Include(x => x.Archive)
                .Where(x => x.IdArchive == idArchive
                    && x.IsVisible == 1
                     && (!x.IsDetached.HasValue || !x.IsDetached.Value)
                    && x.IdParentBiblos.HasValue
                    && !x.PreservationDocuments.Any() || x.PreservationDocuments.Any(pd => !pd.Preservation.CloseDate.HasValue));
                }

                retval.HasPendingDocument = true;
                retval.EndDocumentDate = set.Max(x => x.DateMain);
                retval.StartDocumentDate = set.Min(x => x.DateMain);

                MaxDate = retval.EndDocumentDate.GetValueOrDefault();
                MinDate = retval.StartDocumentDate.GetValueOrDefault();
                logger.InfoFormat("GetPreservationDateByTask {0} {1}", MaxDate, MinDate);
            }
            else
            {
                MaxDate = retval.DateExpire.GetValueOrDefault();
                retval.EndDocumentDate = MaxDate;
            }

            DateTime? dt = this.db.Document
                   .Include(x => x.PreservationDocuments.Single().Preservation)
                   .Include(x => x.Archive)
                   .Where(x => x.IdArchive == idArchive && x.PreservationDocuments.Any(pd => pd.Preservation.CloseDate.HasValue) && x.IsVisible == 1 && (!x.IsDetached.HasValue || !x.IsDetached.Value))
                   .Select(x => x.DateMain)
                   .Max();

            if (dt.HasValue)
            {
                MinDate = dt.Value.AddDays(1);
                retval.StartDocumentDate = MinDate;
            }

            logger.InfoFormat("GetPreservationDateByTask {0} {1}", MaxDate, MinDate);

            if (MaxDate == DateTime.MinValue)
            {
                retval.Error = new PreservationError("Non esistono documenti da sottoporre a conservazione per l'archivio.", PreservationErrorCode.E_USER_DEFINED_EXCEPTION);
                return retval;
            }

            if (MaxDate < MinDate)
            {
                retval.Error = new PreservationError("Esistono documenti con data inferiore alla data di ultima conservazione.", PreservationErrorCode.E_USER_DEFINED_EXCEPTION);
                return retval;
            }
            return retval;
        }

        /// <summary>
        /// Date dei documenti effettivi         
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="idTask"></param>
        /// <returns>
        /// Verifica la data del primo documento successivo l'ultima conservazione e dell'ultimo documento presente
        /// </returns>
        public PreservationInfoResponse GetAvailableDocumentDateForPreservation(DocumentArchive archive, PreservationTask task, Guid preservationId, DateTime? underLimit)
        {
            PreservationInfoResponse infoResponse = new PreservationInfoResponse();
            DateTime endDateLimit = task.EndDocumentDate.GetValueOrDefault(DateTime.Now).Date.AddDays(1).AddSeconds(-1);

            IQueryable<Model.Document> queryDocument = GetPreservationDocumentQuery((ObjectQuery<Model.Document>)this.db.Document.Where(x => x.DateMain <= endDateLimit), archive, preservationId);
            if (underLimit != null)
            {
                queryDocument = queryDocument.Where(x => x.DateMain >= (underLimit));
            }

            int enumeratedDocuments = queryDocument.Count();
            if (enumeratedDocuments == 0)
            {
                logger.Info($"Nessun documento trovato per la conservazione {preservationId}");
                infoResponse.HasPendingDocument = false;
                infoResponse.Error = new ResponseError { ErrorCode = (int)PreservationErrorCode.E_NO_DOCUMENT_EX, Message = "Nessun documento presente per la conservazione." };
                return infoResponse;
            }

            logger.Info($"Trovati {enumeratedDocuments} documenti per la conservazione {preservationId}");
            logger.Info("Verifica dei progressivi valorizzati");
            ICollection<Model.Document> documents = queryDocument.ToList();

            if (documents.Any(x => string.IsNullOrEmpty(x.PrimaryKeyValue)))
            {
                logger.Info("Sono stati trovati alcuni documenti con PrimaryKeyValue non valorizzata. Si procede a calcolarne il valore.");
                BeginNoSaveNoTransaction();
                foreach (Model.Document document in documents.Where(x => string.IsNullOrEmpty(x.PrimaryKeyValue)))
                {
                    BindingList<DocumentAttributeValue> docAttributes = GetFullDocumentAttributeValues(document.IdDocument);
                    logger.Info($"Ricalcolo PrimaryKeyValue e DateMain per il documento {document.IdDocument}({document.IdBiblos})");
                    var primaryKeyValue = AttributeService.ParseAttributeValues(archive, docAttributes, out DateTime? mainDate);
                    logger.Debug($"Document {document.IdDocument}: Valore calcolato PrimaryKeyValue -> {primaryKeyValue}");
                    logger.Debug($"Document {document.IdDocument}: Valore calcolato DateMain -> {mainDate:dd/MM/yyyy}");
                    document.PrimaryKeyValue = primaryKeyValue;
                    document.DateMain = mainDate;
                    UpdatePrimaryKey(document.IdDocument, primaryKeyValue, mainDate);
                }
                db.SaveChanges();
            }

            infoResponse.HasPendingDocument = true;
            infoResponse.EndDocumentDate = documents.Max(x => x.DateMain);
            infoResponse.StartDocumentDate = documents.Min(x => x.DateMain);
            logger.Debug($"PreservationInfoResponse -> StartDate: {infoResponse.StartDocumentDate:dd/MM/yyyy} - EndDate: {infoResponse.EndDocumentDate:dd/MM/yyyy}");

            return infoResponse;
        }

        public BindingList<Document> PrepareDocumentsForPreservation(DocumentArchive archive, PreservationTask task, Guid idPreservation, bool orderByDateMain)
        {
            IQueryable<Model.Document> queryDocuments = GetPreservationDocumentQuery(
                (ObjectQuery<Model.Document>)this.db.Document.Include(x => x.PreservationDocuments).Include(x => x.AttributesValue.First().Attributes).Where(x => x.DateMain >= task.StartDocumentDate && x.DateMain <= task.EndDocumentDate), 
                archive, idPreservation);
            ICollection<Model.Document> documentsToOrder = queryDocuments.ToList();

            IOrderedEnumerable<Model.Document> orderedDocuments = null;
            if (orderByDateMain)
            {
                orderedDocuments = documentsToOrder.OrderBy(x => GetOrderAttributeValueString(x.AttributesValue.Where(a => a.Attributes.IsSectional.HasValue && a.Attributes.IsSectional.Value)))
                    .ThenBy(x => x.DateMain)
                    .ThenBy(x => GetOrderAttributeValue(x.AttributesValue.Where(a => a.Attributes.IsAutoInc.HasValue && a.Attributes.IsAutoInc.Value == 1)));
            }
            else
            {
                orderedDocuments = documentsToOrder.OrderBy(x => GetOrderAttributeValueString(x.AttributesValue.Where(a => a.Attributes.IsSectional.HasValue && a.Attributes.IsSectional.Value)))
                    .ThenBy(x => GetOrderAttributeValue(x.AttributesValue.Where(a => a.Attributes.IsAutoInc.HasValue && a.Attributes.IsAutoInc.Value == 1)));
            }

            BindingList<Document> result = new BindingList<Document>();
            long preservationIndex = 0;
            PreservationDocuments preservationDocument;
            ICollection<Model.Document> orderedDocumentCollection = orderedDocuments.ToList();
            foreach (var document in orderedDocumentCollection)
            {
                preservationIndex += 1;
                logger.Debug($"Set Index {preservationIndex} - Doc: {document.IdDocument}({document.IdBiblos}) {document.PrimaryKeyValue} - Date: {document.DateMain}");

                if (!orderedDocumentCollection.Any(x => x.PreservationDocuments.Any(xx => xx.IdDocument == document.IdDocument)))
                {
                    preservationDocument = new PreservationDocuments
                    {
                        IdPreservationDocument = Guid.NewGuid(),
                        IdPreservation = idPreservation,
                        PreservationIndex = preservationIndex,
                        RegistrationUser = "BiblosDS",
                        RegistrationDate = DateTimeOffset.UtcNow
                    };
                    document.PreservationDocuments.Add(preservationDocument);
                }                

                document.IsConservated = 1;

                db.SaveChanges();

                result.Add(document.Convert());
            }

            return result;
        }

        public bool ExistPreservationDocumentsNoDateMain(DocumentArchive archive)
        {
            int enumerateElements = GetPreservationDocumentQuery((ObjectQuery<Model.Document>)db.Document.Where(x => !x.DateMain.HasValue), archive, Guid.Empty).Count();
            return enumerateElements > 0;
        }

        public ICollection<Document> GetPreservationDocumentsNoDateMain(DocumentArchive archive)
        {
            List<Model.Document> preservationDocuments = GetPreservationDocumentQuery((ObjectQuery<Model.Document>)db.Document.Where(x => !x.DateMain.HasValue), archive, Guid.Empty).ToList();
            ICollection<Document> results = new List<Document>();
            preservationDocuments.ForEach(f => results.Add(f.Convert()));
            return results;
        }

        private IQueryable<Model.Document> GetPreservationDocumentQuery(ObjectQuery<Model.Document> documentTable, DocumentArchive archive, Guid idPreservation)
        {
            var set = documentTable
                .Where(x =>
                    x.IdArchive == archive.IdArchive
                    && x.IsVisible == 1
                    && (!x.IsDetached.HasValue || !x.IsDetached.Value)
                    && x.IdParentBiblos.HasValue
                    && x.IsLatestVersion
                    && x.IsConfirmed == 1
                    && (x.IdDocumentStatus == (short)Enums.DocumentStatus.InCache || x.IdDocumentStatus == (short)Enums.DocumentStatus.InStorage || x.IdDocumentStatus == (short)Enums.DocumentStatus.InTransito)
                    && (!x.PreservationDocuments.Any() || x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation)));
            return set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="domainUser"></param>
        /// <param name="idTaskGroup"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public Guid GetNextPreservationId(Guid idArchive, string domainUser, Guid idTaskGroup, DateTime startDate, DateTime endDate)
        {
            Guid? idPreservation = null;
            Model.Preservation nPreservation = new Model.Preservation();
            bool somethingDone = false;

            try
            {
                if (string.IsNullOrEmpty(domainUser) || idArchive == Guid.Empty || idTaskGroup == Guid.Empty)
                    new PreservationError("Parametri non validi per \"GetNextPreservationId\"", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

                var set = this.db.Preservation
                       .Where(x => x.IdArchive == idArchive && !x.CloseDate.HasValue);

                //Primo ID - se presente - senza la data di chiusura.
                idPreservation = set
                    .Select(x => x.IdPreservation)
                    .FirstOrDefault();

                var utenti = db.PreservationUser.Where(x => x.DomainUser.Equals(domainUser, StringComparison.InvariantCultureIgnoreCase));

                if (utenti.Count() > 1)
                {
                    new PreservationError(string.Format("Esistono piu' utenti con l'utenza di dominio \"{0}\".", domainUser), PreservationErrorCode.E_SYSTEM_EXCEPTION)
                        .ThrowsAsFaultException();
                }

                Guid userId = utenti.Select(x => x.IdPreservationUser).SingleOrDefault();
                if (userId == Guid.Empty)
                    new PreservationError("Utente non valido o non censito in anagrafica.", PreservationErrorCode.E_INVALID_CALL).ThrowsAsFaultException();

                if (idPreservation.GetValueOrDefault() == Guid.Empty) //Nessun elemento trovato: inserire nuovo!
                {
                    idPreservation = Guid.NewGuid();
                    nPreservation.IdPreservation = idPreservation.Value;
                    nPreservation.IdPreservationTaskGroup = idTaskGroup;
                    nPreservation.IdArchive = idArchive;
                    nPreservation.PreservationTask = this.db.PreservationTask
                        .Where(x => x.PreservationTaskType != null
                            && x.PreservationTaskType.KeyCode.Equals("1", StringComparison.InvariantCultureIgnoreCase)
                         && x.IdPreservationTaskGroup == idTaskGroup)
                        .FirstOrDefault();

                    nPreservation.PreservationDate = DateTime.Now;
                    nPreservation.StartDate = startDate;
                    nPreservation.EndDate = endDate;
                    nPreservation.IdPreservationUser = userId;
                    this.db.AddToPreservation(nPreservation);
                    db.SaveChanges();
                    somethingDone = true;
                }
                else //Almeno un elemento trovato.
                {
                    var taskDaAssociare = this.db.PreservationTask
                            .Where(x => x.PreservationTaskType != null
                                && x.PreservationTaskType.KeyCode.Equals("1", StringComparison.InvariantCultureIgnoreCase)
                                && x.IdPreservationTaskGroup == idTaskGroup)
                            .FirstOrDefault();

                    foreach (var item in set)
                    {
                        //item.PreservationTaskGroup = this.db.PreservationTaskGroup
                        //    .Where(x => x.IdPreservationTaskGroup == idTaskGroup)
                        //    .FirstOrDefault();
                        item.IdPreservationTaskGroup = idTaskGroup;

                        item.PreservationTask = taskDaAssociare;

                        item.StartDate = startDate;

                        item.EndDate = endDate;

                        somethingDone = true;
                    }
                }

                if (!somethingDone)
                    new PreservationError("Non è stato possibile creare una nuova conservazione. Verificare che l'utente sia Responsabile della Conservazione per l'archivio.", PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();

                somethingDone = false;

                //Aggiornamento task.
                var tasks = this.db.PreservationTask.Where(x => x.IdPreservationTaskGroup == idTaskGroup);

                foreach (var t in tasks)
                {
                    somethingDone = true;

                    t.IdPreservationUser = userId;
                }

                if (!somethingDone)
                    new PreservationError("Non è stato possibile aggiornare alcuna conservazione. Verificare che l'utente sia Responsabile della Conservazione per l'archivio.", PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();

                //Persiste su db.
                if (requireSave)
                    this.db.SaveChanges();

                Dispose();
            }
            catch (EvaluateException exx)
            {
                if (idPreservation.HasValue)
                    this.AbortPreservation(idPreservation.GetValueOrDefault());

                try { Dispose(); }
                catch { }

                throw exx;
            }

            return idPreservation.HasValue ? idPreservation.Value : Guid.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        public void ResetPreparedPreservation(Guid idPreservation, bool resetId = false)
        {
            if (idPreservation == Guid.Empty)
                new PreservationError("Parametro \"idPreservation\" non valido per \"ResetPreparedPreservation\".", PreservationErrorCode.E_INVALID_PARAMS).ThrowsAsFaultException();

            bool existPreservation = db.Preservation.Any(x => x.IdPreservation == idPreservation);
            if (existPreservation)
            {
                ICollection<Model.Document> preservedDocuments = this.db.Document
                    .Include(c => c.PreservationDocuments)
                    .Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation))
                    .ToList();

                foreach (Model.Document document in preservedDocuments)
                {
                    document.PreservationDocuments?.ToList().ForEach(db.PreservationDocuments.DeleteObject);
                    document.IsConservated = 0;
                }

                if (requireSave)
                    this.db.SaveChanges();
            }

            Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="keyToVerify"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        public BindingList<Document> FindPreparedPreservationObjects(DateTime dateFrom, DateTime dateTo, Guid idArchive)
        {
            try
            {
                var retval = new BindingList<Document>();

                BeginNoSaveNoTransaction();
                var archive = GetArchive(idArchive);

                if (archive != null)
                {
                    var documents = db.Document
                        .Include(x => x.Archive.Attributes)
                        .Include(x => x.AttributesValue)
                        .Include(x => x.PreservationDocuments.Single().Preservation)
                        .Where(x =>
                               x.IdParentBiblos.HasValue
                               && x.IdArchive == idArchive
                               && x.IsVisible == 1
                               && x.IsConfirmed == 1
                               && (!x.IsDetached.HasValue || !x.IsDetached.Value)
                               && (!x.PreservationDocuments.Any() || x.PreservationDocuments.Any(pd => !pd.Preservation.CloseDate.HasValue)));


                    documents = documents.Where(x => x.DateMain.Value >= dateFrom && x.DateMain.Value <= dateTo);

                    var set = documents.OrderBy(x => x.IdBiblos)
                        .ThenBy(x => x.PrimaryKeyValue);

                    foreach (var item in set)
                    {
                        retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL, null));
                    }
                }

                return retval;
            }
            finally
            {
                ForceDispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="idPreservationException"></param>
        /// <param name="idDocument"></param>
        public void UpdatePreservationException(Guid idPreservationException, Guid idDocument)
        {
            //string sSQL = "UPDATE Oggetto_Conservazione"
            //     + " SET IdEccezione = " + IdEccezione.ToString()
            //     + " WHERE IdEccezione NOT IN (SELECT IdEccezione FROM Eccezione WHERE Bloccante = 1)"
            //     + "AND IdOggetto = " + IdOggetto.ToString();

            Model.PreservationDocuments preservationDocument = db.PreservationDocuments.SingleOrDefault(x => x.IdDocument == idDocument);
            if (preservationDocument != null)
            {
                preservationDocument.IdPreservationException = idPreservationException;
            }            

            if (requireSave)
                this.db.SaveChanges();

            Dispose();
        }

        public void UpdatePreservationExceptionForAllDocuments(Guid idPreservationException, Guid idPreservation)
        {
            ICollection<Model.PreservationDocuments> preservationDocuments = db.PreservationDocuments.Where(x => x.IdPreservation == idPreservation).ToList();
            foreach (Model.PreservationDocuments document in preservationDocuments)
            {
                document.IdPreservationException = idPreservationException;
            }

            if (requireSave)
                db.SaveChanges();

            Dispose();
        }

        public void AbortPreservation(Guid idPreservation)
        {
            try
            {
                var tasks = db.PreservationTask.Where(x => x.PreservationTaskGroup.Preservation.Any(p => p.IdPreservation == idPreservation));
                tasks.ToList().ForEach(x => { x.IdPreservationUser = null; });

                if (requireSave)
                    db.SaveChanges();

                var documents = db.Document.Include(x => x.PreservationDocuments).Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation));
                documents.ToList().ForEach(x =>
                {
                    x.PreservationDocuments?.ToList().ForEach(db.PreservationDocuments.DeleteObject);
                    x.IsConservated = 0;
                });

                if (requireSave)
                    db.SaveChanges();

                foreach (var journaling in db.PreservationJournaling.Where(x => x.IdPreservation == idPreservation))
                {
                    db.PreservationJournaling.DeleteObject(journaling);
                }

                if (requireSave)
                    db.SaveChanges();

                var preservation = db.Preservation.Where(x => x.IdPreservation == idPreservation).Single();
                db.DeleteObject(preservation);

                if (requireSave)
                    db.SaveChanges();

                Dispose();
            }
            catch (Exception)
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Il funzionamento è simile alla ResetPreservation.
        /// </summary>
        public PreservationInfoResponse ResetPreservation(Guid idPreservation, string domainUser)
        {
            var retval = new PreservationInfoResponse();

            try
            {
                var preservation = this.db.Preservation
                    .Include(x => x.PreservationTask)
                    .Include(x => x.PreservationJournaling)
                    .Where(x => x.IdPreservation == idPreservation)
                    .SingleOrDefault();

                if (preservation == null)
                {
                    retval.Error = new PreservationError("Conservazione con id: " + idPreservation + " non trovata.", PreservationErrorCode.E_USER_DEFINED_EXCEPTION);
                    return retval;
                }

                var journalActivityId = this.db.PreservationJournalingActivity
                    .Where(x => x.KeyCode.Equals("RiaperturaConservazione", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.IdPreservationJournalingActivity)
                    .SingleOrDefault();

                var documents = this.db.Document
                    .Include(x => x.PreservationDocuments)
                    .Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation));

                foreach (var doc in documents)
                {
                    doc.PreservationDocuments?.ToList().ForEach(db.PreservationDocuments.DeleteObject);
                    doc.IsConservated = 0;
                }

                preservation.CloseDate = null;

                try { preservation.PreservationTask.ExecutedDate = null; }
                catch { }

                try { preservation.PreservationTaskGroup.Closed = null; }
                catch { }

                var userId = this.db.PreservationUser
                    .Where(x => (x.DomainUser ?? string.Empty).Equals(domainUser, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.IdPreservationUser)
                    .FirstOrDefault();

                var journaling = new Model.PreservationJournaling
                {
                    IdPreservationJournaling = Guid.NewGuid(),
                    IdPreservationUser = userId,
                    IdPreservationJournalingActivity = journalActivityId,
                    DateActivity = DateTime.Now,
                    DateCreated = DateTime.Now,
                    DomainUser = domainUser,
                };

                preservation.PreservationJournaling.Add(journaling);

                this.db.PreservationJournaling.AddObject(journaling);

                if (requireSave)
                    this.db.SaveChanges();

                retval.IdPreservation = preservation.IdPreservation; //Se può servire per debug.

                Dispose();
            }
            catch (Exception exx)
            {
                Dispose();
                new PreservationError(exx, PreservationErrorCode.E_SYSTEM_EXCEPTION)
                    .ThrowsAsFaultException();
            }

            return retval;
        }

        #region NOT USED / OBSOLETE

#if WANTS_OBSOLETE

        public void WritePreservationDocument(Guid idDocument, Nullable<Guid> idPreservation)
        {
            var set = this.db.Document
                .Where(x => x.IdDocument == idDocument);

            var item = set.Single();

            item.IdPreservation = idPreservation;

            this.db.SaveChanges();
        }

#endif

        #endregion

        public List<PreservationTask> GetPreservationVerify(Guid[] idArchives, bool? inError = null)
        {
            int total;
            return GetPreservationVerify(idArchives, 0, 0, out total, inError);
        }

        public List<PreservationTask> GetPreservationVerify(Guid[] idArchives, int skip, int take, out int total, bool? inError = null)
        {
            try
            {
                List<PreservationTask> result = new List<PreservationTask>();
                var query = this.db.PreservationVerify.Include(x => x.Archive).Where(x => idArchives.Any(a => a == x.IdArchive));
                if (inError.HasValue)
                {
                    if (inError.Value)
                    {
                        query = query.Where(x => x.Warning != null);
                        total = db.PreservationVerify.Count(x => x.Warning != null && idArchives.Any(a => a == x.IdArchive));
                    }
                    else
                    {
                        query = query.Where(x => x.Warning == null);
                        total = db.PreservationVerify.Count(x => x.Warning == null && idArchives.Any(a => a == x.IdArchive));
                    }
                }
                else
                {
                    total = db.PreservationVerify.Count(x => idArchives.Any(a => a == x.IdArchive));
                }

                if (skip > 0 || take > 0)
                {
                    query = query.OrderBy(x => x.IdPreservationVerify);

                    if (skip > 0)
                        query = query.Skip<Model.PreservationVerify>(skip);

                    if (take > 0)
                        query = query.Take<Model.PreservationVerify>(take);
                }

                foreach (var item in query)
                {
                    result.Add(new PreservationTask
                    {
                        ErrorMessages = item.Warning,
                        IdPreservation = item.IdPreservation,
                        Archive = item.Archive.Convert(),
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        VerifyPath = item.VerifyPath
                    });
                }

                return result;
            }
            finally
            {
                Dispose();
            }
        }

        public bool CheckPreservationVerify(Guid idPreservation)
        {
            try
            {
                return this.db.PreservationVerify.Any(x => x.IdPreservation == idPreservation);
            }
            finally
            {
                Dispose();
            }
        }

        public void SavePreservationVerify(Guid idPreservation, Exception ex)
        {
            try
            {
                var preservation = GetPreservation(idPreservation, false);
                if (!string.IsNullOrEmpty(preservation.Archive.ODBCConnection))
                {
                    //Write to odbc connection
                    using (OdbcConnection cnn = new OdbcConnection(preservation.Archive.ODBCConnection))
                    {
                        cnn.Open();
                        bool exists = false;
                        using (OdbcCommand cmdExists = new OdbcCommand(string.Format("select * from PreservationVerify where IdPreservation = '{0}'", idPreservation), cnn))
                        {
                            using (OdbcDataReader dr = cmdExists.ExecuteReader())
                                exists = dr.Read();
                        }
                        if (exists)
                        {
                            string queryUpdate = string.Format(@"UPDATE PreservationVerify
   SET             
       StartDate = '{0:yyyy-MM-dd}'
      ,EndDate = '{1:yyyy-MM-dd}'
      ,VerifyPath = '{2}'
      ,Warning = '{3}'
      ,`Lock` = {4}
 WHERE IdPreservation = '{5}'", preservation.StartDate.GetValueOrDefault(),
                            preservation.EndDate.GetValueOrDefault(),
                            preservation.Path,
                            ex == null ? "" : ex.Message.Replace("'", "''"),
                            preservation.LockOnDocumentInsert.GetValueOrDefault() ? 1 : 0,
                            preservation.IdPreservation);
                            using (OdbcCommand cmdUpdate = new OdbcCommand(queryUpdate, cnn))
                            {
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string query = string.Format(@"INSERT INTO PreservationVerify
           (IdPreservationVerify
           ,IdArchive
           ,IdPreservation
           ,StartDate
           ,EndDate
           ,VerifyPath
           ,Warning
           ,`Lock`)
     VALUES
           ('{0}'
           ,'{1}'
           ,'{2}'
           ,'{3:yyyy-MM-dd}'
           ,'{4:yyyy-MM-dd}'
           ,'{5}'
           ,'{6}'
           , {7})", Guid.NewGuid(), preservation.IdArchive, preservation.IdPreservation,
                  preservation.StartDate.GetValueOrDefault(), preservation.EndDate.GetValueOrDefault(), preservation.Path, ex == null ? "" : ex.Message.Replace("'", "''"), preservation.LockOnDocumentInsert.GetValueOrDefault() ? 1 : 0);
                            using (OdbcCommand cmdUpdate = new OdbcCommand(query,
                   cnn))
                            {
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                    }
                }
                Model.PreservationVerify preservationVerify = null;
                bool isNew = false;
                if ((preservationVerify = this.db.PreservationVerify.Where(x => x.IdPreservation == idPreservation).FirstOrDefault()) == null)
                {
                    isNew = true;
                    preservationVerify = new Model.PreservationVerify();
                    preservationVerify.IdPreservation = preservation.IdPreservation;
                    preservationVerify.IdPreservationVerify = Guid.NewGuid();
                }
                preservationVerify.Lock = ex == null;
                preservationVerify.IdArchive = preservation.IdArchive;
                preservationVerify.VerifyPath = preservation.Path;
                preservationVerify.StartDate = preservation.StartDate;
                preservationVerify.EndDate = preservation.EndDate;
                if (ex != null)
                    preservationVerify.Warning = ex.Message;
                if (isNew)
                    this.db.AddToPreservationVerify(preservationVerify);
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public string GetPreservationPathByTask(Guid idPreservationTask)
        {
            try
            {
                var res = this.db.Preservation.Where(x => x.PreservationTask1.Any(t => t.IdPreservationTask == idPreservationTask)).FirstOrDefault();
                if (res != null)
                    return res.Path;
                return null;
            }
            finally
            {
                Dispose();
            }
        }

        public Guid? GetPreservationIdByTask(Guid idPreservationTask)
        {
            try
            {
                var res = this.db.Preservation.Where(x => x.PreservationTask1.Any(t => t.IdPreservationTask == idPreservationTask)).FirstOrDefault();
                if (res != null)
                    return res.IdPreservation;
                return null;
            }
            finally
            {
                Dispose();
            }
        }

        public Guid CreatePreservation(PreservationTask task)
        {
            Guid idPreservation = Guid.NewGuid();
            try
            {
                Model.PreservationUser user = null;
                if ((user = db.PreservationUserRole.Include(x => x.PreservationUser).Where(x => x.IdArchive == task.Archive.IdArchive && (x.DefaultUser.HasValue && x.DefaultUser.Value)).Select(x => x.PreservationUser).FirstOrDefault()) == null)
                    user = db.PreservationUser.FirstOrDefault(x => x.DefaultUser.HasValue && x.DefaultUser.Value);
                if (user == null)
                    throw new Exceptions.Generic_Exception("Nessun utente conservazione definito.");
                Model.Preservation nPreservation = new Model.Preservation();
                nPreservation.IdPreservation = idPreservation;
                nPreservation.IdArchive = task.Archive.IdArchive;
                nPreservation.IdPreservationUser = user.IdPreservationUser;
                nPreservation.PreservationDate = DateTime.Now;
                nPreservation.StartDate = task.StartDate;
                nPreservation.EndDate = task.EndDate;
                this.db.AddToPreservation(nPreservation);
                SaveChanges();
                return idPreservation;
            }
            finally
            {
                Dispose();
            }
        }

        public void SavePreservationLastSectionalValue(Guid idPreservation, string LastSectionalValue)
        {
            try
            {
                var preservation = db.Preservation.Where(x => x.IdPreservation == idPreservation).FirstOrDefault();
                if (preservation == null)
                    throw new Exceptions.Generic_Exception("Nessuna conservazione definita con l'id: " + idPreservation);
                preservation.LastSectionalValue = LastSectionalValue;
                if (requireSave)
                    SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }


        public void SavePreservationTaskStatus(PreservationTask task, Objects.Enums.PreservationTaskStatus taskStatus, bool hasError, string errorMessage)
        {
            try
            {
                Model.PreservationTask preservationTask = db.PreservationTask.Where(x => x.IdPreservationTask == task.IdPreservationTask).FirstOrDefault();
                if (preservationTask == null)
                {
                    throw new Exceptions.Generic_Exception($"Nessuna conservazione definita con l'id task: {task.IdPreservationTask}");
                }

                string taskStatusString = taskStatus.ToString();
                Model.PreservationTaskStatus preservationTaskStatus = db.PreservationTaskStatus.FirstOrDefault(x => x.Status == taskStatusString);
                if (preservationTaskStatus == null)
                {
                    throw new Exceptions.Generic_Exception($"Nessuno status trovato con descrizione {taskStatus.ToString()}");
                }
                preservationTask.HasError = hasError;
                preservationTask.ErrorMessages = errorMessage == null ? null : errorMessage.Substring(0, Math.Min(errorMessage.Length, 3999));
                preservationTask.ExecutedDate = DateTime.Now;
                preservationTask.Executed = true;
                preservationTask.PreservationTaskStatus = preservationTaskStatus;

                if (requireSave)
                    SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Ritorna l'elenco dei task precedenti da eseguire 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public BindingList<PreservationTask> PreviousTaskToExecute(PreservationTask task, Guid idArchive)
        {
            BindingList<PreservationTask> result = new BindingList<PreservationTask>();
            try
            {
                var preservationTasks = db.PreservationTask.Where(x => x.IdArchive == idArchive && !x.Executed && !x.HasError && x.StartDocumentDate < task.StartDocumentDate);
                foreach (var item in preservationTasks)
                {
                    result.Add(item.Convert());
                }
            }
            finally
            {
                Dispose();
            }
            return result;
        }

        public long GetPreservationTotalSizeOnDisk(Guid idArchive, string path)
        {
            try
            {
                var totalSize = db.Preservation.Where(x => x.IdArchive == idArchive && x.Path == path).Sum(x => x.PreservationSize);
                return totalSize.GetValueOrDefault();
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdateArchivePathPreservation(DocumentArchive arcive)
        {
            try
            {
                var dbArchive = db.Archive.Single(x => x.IdArchive == arcive.IdArchive);
                dbArchive.PathPreservation = arcive.PathPreservation;
                if (requireSave)
                    SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Preservation> ArchivePreservationClosedInDate(Guid idArchive, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                BindingList<Preservation> items = new BindingList<Preservation>();
                IQueryable<Model.Preservation> preservations = this.db.Preservation
                    .Where(x => x.IdArchive == idArchive && x.CloseDate.HasValue
                        && x.CloseDate >= dateFrom && x.CloseDate <= dateTo)
                        .OrderByDescending(x => x.StartDate);

                foreach (var item in preservations)
                {
                    items.Add(item.Convert());
                }
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public BindingList<Objects.ArchiveCompany> GetArchiveCompanies(Guid idArchive)
        {
            try
            {
                BindingList<Objects.ArchiveCompany> items = new BindingList<Objects.ArchiveCompany>();
                IQueryable<Model.ArchiveCompany> archiveCompanies = db.ArchiveCompany
                    .Include(c => c.Archive)
                    .Include(c => c.Company)
                    .Where(x => x.IdArchive == idArchive);

                archiveCompanies.ToList().ForEach(f => items.Add(f.Convert()));
                return items;
            }
            finally
            {
                Dispose();
            }
        }

        public PreservationJournalingActivity GetCreatePreservationJournalingActivity()
        {
            try
            {                
                IQueryable<Model.PreservationJournalingActivity> query = this.db.PreservationJournalingActivity
                        .Where(x => x.KeyCode == "CreazioneConservazione");

                return query.SingleOrDefault()?.Convert();
            }
            finally
            {
                Dispose();
            }            
        }

        public BindingList<Objects.Document> GetPreservationDocumentsToPurge(Guid idPreservation)
        {
            try
            {
                BindingList<Document> results = new BindingList<Document>();
                IQueryable<Model.Document> query = db.Document.Include(i => i.PreservationDocuments)
                    .Include(i => i.Storage)
                    .Include(i => i.StorageArea)
                    .Include(i => i.DocumentParent)
                    .Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation) 
                    && x.IdDocumentStatus == (short)Enums.DocumentStatus.InStorage
                    && (!x.IsDetached.HasValue || x.IsDetached == false)
                    && x.IsConservated == 1)
                    .OrderBy(o => o.IdDocument);

                query.ToList().ForEach(f => results.Add(f.Convert()));
                return results;
            }
            finally
            {
                Dispose();
            }
        }

        public int CountPreservationDocumentsToPurge(Guid idPreservation)
        {
            try
            {
                return db.Document
                    .Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation) 
                    && x.IdDocumentStatus == (short)Enums.DocumentStatus.InStorage
                    && (!x.IsDetached.HasValue || x.IsDetached == false)
                    && x.IsConservated == 1)
                    .Count();
            }
            finally
            {
                Dispose();
            }
        }

        public string GetPreservationDocumentPath(Document document)
        {
            try
            {
                return db.PreservationDocuments.Where(x => x.IdDocument == document.IdDocument).Select(s => s.Path).SingleOrDefault();
            }
            finally
            {
                Dispose();
            }
        }
        
        public bool ExistPreservationsByArchive(Guid idArchive)
        {
            try
            {
                return db.Preservation.Any(x => x.Archive.IdArchive == idArchive);
            }
            finally
            {
                Dispose();
            }
        }
    }
}
