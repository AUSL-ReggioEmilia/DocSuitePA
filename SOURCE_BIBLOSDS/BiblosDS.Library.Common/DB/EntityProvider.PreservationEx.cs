using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Obj = BiblosDS.Library.Common.Objects;
using Model = BiblosDS.Library.Common.Model;
using System.Data;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Objects.Enums;

namespace BiblosDS.Library.Common.DB
{
    public partial class EntityProvider
    {
        private static Model.PreservationTask CreateModelFromObj(Obj.PreservationTask obj)
        {
            if (obj == null)
                return null;

            var ret = new Model.PreservationTask
            {
                IdPreservationTask = obj.IdPreservationTask,
                IdArchive = (obj.Archive != null) ? obj.Archive.IdArchive : Guid.Empty,
                IdPreservationTaskType = (obj.TaskType != null) ? obj.TaskType.IdPreservationTaskType : Guid.Empty,
                EstimatedDate = obj.EstimatedDate,
                ExecutedDate = obj.ExecutedDate,
                StartDocumentDate = obj.StartDocumentDate,
                EndDocumentDate = obj.EndDocumentDate.Value.ToLocalTime().Date.AddDays(1).AddMilliseconds(-3),                
                Enabled = obj.Enabled,
            };

            if (obj.CorrelatedTasks != null)
            {
                foreach (var corr in obj.CorrelatedTasks)
                {
                    ret.PreservationTask1.Add(CreateModelFromObj(corr));
                }
            }

            return ret;
        }

        /// <summary>
        /// Verifica la coerenza delle date dei tasks.
        /// </summary>
        /// <returns></returns>
        private static bool CheckTasksDates(Model.BiblosDS2010Entities db, IEnumerable<Obj.PreservationTask> tasks)
        {
            var retval = false;

            if (db == null || tasks == null || tasks.Count() < 1)
                return true;

            try
            {
                if (tasks.Any(x => x.Archive == null || x.TaskType == null
                    || !x.StartDocumentDate.HasValue || !x.EndDocumentDate.HasValue
                    || x.StartDocumentDate.Value >= x.EndDocumentDate.Value))
                {
                    throw new Exception();
                }

                DateTime dataInizio, dataFine;
                Guid idArchivio;
                string keyCode;
                foreach (var t in tasks)
                {
                    idArchivio = t.Archive.IdArchive;
                    keyCode = ((int)t.TaskType.Type).ToString();
                    dataInizio = t.StartDocumentDate.Value.Date;
                    dataFine = t.EndDocumentDate.Value.Date;

                    retval = db.PreservationTask
                        .Include(x => x.PreservationTaskType)
                        .Where(x => x.IdArchive == idArchivio && x.StartDocumentDate.HasValue && x.EndDocumentDate.HasValue)
                        .Any(x => dataInizio <= x.EndDocumentDate.Value && dataFine >= x.StartDocumentDate.Value && x.PreservationTaskType.KeyCode == keyCode);                       
                    if (retval)
                        throw new Exception();

                    if (t.EstimatedDate < dataFine)
                        throw new Exception();
                }

                retval = true;
            }
            catch { retval = false; }

            return retval;
        }

        public BindingList<Obj.PreservationTask> CreatePreservationTask(BindingList<Obj.PreservationTask> tasks)
        {
            var retval = new BindingList<Obj.PreservationTask>();
            var added = new List<Model.PreservationTask>();
            Model.PreservationTask toAdd;

            try
            {
                //Le verifiche del caso.
                if (tasks == null || tasks.Count < 1)
                    throw new Exception("CreatePreservationTask - no tasks to add");

                if (tasks.Any(x => x.TaskType == null || x.TaskType.Type == Obj.Enums.PreservationTaskTypes.Unknown))
                    throw new Exception("CreatePreservationTask - some task types are invalid.");

                if (tasks.Any(x => x.CorrelatedTasks != null && x.CorrelatedTasks.Any(y => y.TaskType == null || y.TaskType.Type == Obj.Enums.PreservationTaskTypes.Unknown)))
                    throw new Exception("CreatePreservationTask - some correlated's task types are invalid.");

                if (tasks.Any(x => x.Archive == null || x.Archive.IdArchive == Guid.Empty))
                    throw new Exception("CreatePreservationTask - Archive is null");

                if (tasks.Any(x => !x.StartDocumentDate.HasValue || !x.EndDocumentDate.HasValue))
                    throw new Exception("CreatePreservationTask - some preservation dates aren't valid.");

                if (tasks.Any(x => x.CorrelatedTasks != null && x.CorrelatedTasks.Any(y => !y.StartDocumentDate.HasValue || !y.EndDocumentDate.HasValue)))
                    throw new Exception("CreatePreservationTask - some correlated preservation dates aren't valid.");

                //if (!checkTasksDates(db, tasks))
                //    throw new Exception("CreatePreservationTask - some tasks have the same preservation period in db.");

                if (tasks.Any(x => x.TaskType == null || x.TaskType.IdPreservationTaskType == null))
                    throw new Exception("CreatePreservationTask - TaskType is null");

                //
                var tipologieTasks = db.PreservationTaskType
                    .ToArray();

                foreach (var t in tasks)
                {
                    //if (tasks.Any(x => x.StartDocumentDate.Value >= t.StartDocumentDate.Value && x.EndDocumentDate.Value <= t.StartDocumentDate.Value && x.TaskType.Type == t.TaskType.Type))
                    //    throw new Exception("CreatePreservationTask - tasks dates are invalid.");

                    t.IdPreservationTask = Guid.NewGuid();
                    t.TaskType.IdPreservationTaskType = tipologieTasks
                        .Single(x => x.KeyCode == ((int)t.TaskType.Type).ToString())
                        .IdPreservationTaskType;

                    if (t.CorrelatedTasks != null)
                    {
                        foreach (var tmp in t.CorrelatedTasks)
                        {
                            tmp.Archive = new Obj.DocumentArchive(t.Archive.IdArchive);
                        }

                        if (!CheckTasksDates(db, t.CorrelatedTasks))
                            throw new Exception("CreatePreservationTask - some correlated tasks have the same preservation period in db.");

                        foreach (var corr in t.CorrelatedTasks)
                        {
                            //if (t.CorrelatedTasks.Any(x => x.StartDocumentDate.Value >= corr.StartDocumentDate.Value && x.EndDocumentDate.Value <= corr.StartDocumentDate.Value && x.TaskType.Type == corr.TaskType.Type))
                            //    throw new Exception("CreatePreservationTask - correlated tasks dates are invalid.");

                            //if (tasks.Any(x => x.StartDocumentDate.Value >= corr.StartDocumentDate.Value && x.EndDocumentDate.Value <= corr.StartDocumentDate.Value && x.TaskType.Type == corr.TaskType.Type))
                            //    throw new Exception("CreatePreservationTask - correlated tasks dates are invalid.");

                            //if (tasks.Any(x => x.CorrelatedTasks != null
                            //    && x.CorrelatedTasks.Any(y => y != t && y.StartDocumentDate.Value >= corr.StartDocumentDate.Value && y.EndDocumentDate.Value <= corr.StartDocumentDate.Value && y.TaskType.Type == corr.TaskType.Type)))
                            //    throw new Exception("CreatePreservationTask - correlated tasks dates are invalid.");

                            corr.IdPreservationTask = Guid.NewGuid();
                            corr.TaskType.IdPreservationTaskType = tipologieTasks
                                .Single(x => x.KeyCode == ((int)corr.TaskType.Type).ToString())
                                .IdPreservationTaskType;
                        }
                    }

                    toAdd = CreateModelFromObj(t);

                    db.PreservationTask.AddObject(toAdd);

                    added.Add(toAdd);
                }

                if (requireSave)
                {
                    SaveChanges();

                    foreach (var item in added)
                    {
                        retval.Add(db.PreservationTask
                            .Include(x => x.Archive)
                            .Include(x => x.PreservationTaskType)
                            .Include(x => x.PreservationTask1)
                            .Include(x => x.PreservationTask1.First().Archive)
                            .Include(x => x.PreservationTask1.First().PreservationTaskType)
                            .Single(x => x.IdPreservationTask == item.IdPreservationTask)
                            .Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                    }
                }
            }
            finally
            {
                Dispose();
            }

            return retval;
        }

        public void UpdatePreservationTaskPreservation(Obj.PreservationTask task, Guid idPreservation)
        {            
            try
            {
                var dbTsk = db.PreservationTask.Single(x => x.IdPreservationTask == task.IdPreservationTask);
                dbTsk.IdPreservation = idPreservation;
                
                if (requireSave)
                    SaveChanges();
            }
            finally
            {
                Dispose();
            }            
        }

        public BindingList<Obj.PreservationTask> UpdatePreservationTask(BindingList<Obj.PreservationTask> tasks, bool updateCorrelatedTasks)
        {
            var retval = new BindingList<Obj.PreservationTask>();

            try
            {
                //Array contenente gli ID dei tasks da aggiornare: servirà per recuperare i dati da db.
                var ids = tasks
                    .Where(x => x != null && x.IdPreservationTask != Guid.Empty)
                    .Select(x => x.IdPreservationTask)
                    .ToArray();
                //Recupera i tasks da db.
                var dbTasks = db.PreservationTask
                    .Where(x => ids.Contains(x.IdPreservationTask));
                //TODO: Implementare qualche controllo fatto bene!
                //TODO: Aggiungere le logiche per i task correlati sia su db che sull'oggetto in input!
                Obj.PreservationTask currentTask;
                foreach (var dbTsk in dbTasks)
                {
                    currentTask = tasks.Single(x => x.IdPreservationTask == dbTsk.IdPreservationTask);
                    dbTsk.ActivationPin = currentTask.ActivationPin;
                    dbTsk.Enabled = currentTask.Enabled;
                    dbTsk.HasError = currentTask.HasError;
                    dbTsk.ErrorMessages = currentTask.ErrorMessages;
                    dbTsk.Executed = currentTask.Executed;
                    dbTsk.StartDocumentDate = currentTask.StartDocumentDate;
                    dbTsk.EndDocumentDate = currentTask.EndDocumentDate;
                    dbTsk.EstimatedDate = currentTask.EstimatedDate;
                    dbTsk.ExecutedDate = currentTask.ExecutedDate;
                }

                if (requireSave)
                    SaveChanges();
            }
            finally
            {
                Dispose();
            }

            return retval;
        }


        public BindingList<Obj.PreservationTask> GetAllChildPreservationTasks(Guid idTask, out long count, int take = 0, int skip = 0)
        {            
            var tasks = new BindingList<Obj.PreservationTask>();

            try
            {                
                var ret = db.PreservationTask
                    .Include(x => x.Archive)
                    .Include(x => x.PreservationTaskType)
                    .Include(x => x.PreservationTaskStatus)
                    .Include(x => x.PreservationTask1)
                    .Include(x => x.PreservationTask1.First().Archive)
                    .Include(x => x.PreservationTask1.First().PreservationTaskType)
                    .Where(x => x.IdPreservationTask == idTask || x.IdCorrelatedPreservationTask == idTask);

                count = 0;

                if (skip > -1 && take > 0)
                {
                    count = ret.Count();
                    ret = ret.OrderByDescending(x => x.EstimatedDate).Skip(skip).Take(take);
                }
                else
                    ret = ret.OrderByDescending(x => x.EstimatedDate);

                foreach (var task in ret)
                {
                    tasks.Add(task.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }
            finally
            {
                Dispose();
            }

            return tasks;
        }

        public ICollection<Obj.PreservationTask> GetPreservationActiveTasks(ICollection<Guid> idArchives)
        {
            try
            {
                ICollection<Obj.PreservationTask> results = new List<Obj.PreservationTask>();
                IQueryable<Model.PreservationTask> query = db.PreservationTask
                    .Include(x => x.Archive)
                    .Include(x => x.PreservationTaskType)
                    .Where(x => idArchives.Any(xx => xx == x.IdArchive) &&
                        ((!x.Executed || (x.Executed && x.HasError)) 
                        && (!x.IdCorrelatedPreservationTask.HasValue || (x.PreservationTask2.Executed && !x.PreservationTask2.HasError))));

                query.AsEnumerable()
                    .GroupBy(g => g.Archive.IdArchive, (key, g) => g.OrderBy(o => o.EndDocumentDate).FirstOrDefault())
                    .OrderBy(o => o.PreservationTaskType.KeyCode)
                    .ThenBy(o => o.EndDocumentDate)
                    .ToList()
                    .ForEach(f => results.Add(f.Convert()));
                return results;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Obj.PreservationTask> GetPreservationTasks(IEnumerable<Obj.DocumentArchive> archives, out long count, int take = 0, int skip = 0)
        {
            var ids = archives.Select(x => x.IdArchive).ToArray<Guid>();
            var tasks = new BindingList<Obj.PreservationTask>();

            try
            {

                var ret = db.PreservationTask
                    .Include(x => x.Archive)
                    .Include(x => x.PreservationTaskType)
                    .Include(x => x.PreservationTaskStatus)
                    .Include(x => x.PreservationTask1)
                    .Include(x => x.PreservationTask1.First().Archive)
                    .Include(x => x.PreservationTask1.First().PreservationTaskType)
                    .Where(x => ids.Contains(x.IdArchive) && !x.IdCorrelatedPreservationTask.HasValue);

                count = 0;

                if (skip > -1 && take > 0)
                {
                    count = ret.Count();

                    ret = ret
                        .OrderByDescending(x => x.EstimatedDate)
                        .Skip(skip)
                        .Take(take);
                }
                else
                {
                    ret = ret
                        .OrderByDescending(x => x.EstimatedDate);
                }


                foreach (var task in ret)
                {
                    tasks.Add(task.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }
            finally
            {
                Dispose();
            }

            return tasks;
        }

        public Obj.PreservationTask GetPreservationTask(Guid idTask, bool fillRelations = true)
        {
            Obj.PreservationTask retval = null;

            try
            {
                System.Data.Objects.ObjectQuery<Model.PreservationTask> query = db.PreservationTask;
                if (fillRelations)
                {
                    query = query.Include(x => x.Archive)
                    .Include(x => x.PreservationTaskType)
                    .Include(x => x.PreservationTask1)
                    .Include(x => x.PreservationTask1.First().Archive)
                    .Include(x => x.PreservationTask1.First().PreservationTaskType);
                }                    
                Model.PreservationTask preservationTask = query.Where(x => x.IdPreservationTask == idTask).SingleOrDefault();

                if (preservationTask != null)
                {
                    retval = preservationTask.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);
                }
            }
            finally
            {
                Dispose();
            }

            return retval;
        }     

        public BindingList<PreservationTaskDatesResponse> GetNextPreservationTaskDatesForArchive(Guid idArchive, BindingList<PreservationTaskTypes> types)
        {
            var retval = new BindingList<PreservationTaskDatesResponse>();

            if (types == null || types.Count < 1)
                return retval;

            var type_ver = ((int)PreservationTaskTypes.Verify).ToString();
            var type_pre = ((int)PreservationTaskTypes.Preservation).ToString();

            try
            {
                if (types.Contains(PreservationTaskTypes.Verify))
                {
                    var queryVer = db.PreservationTask
                                    .Include(x => x.PreservationTaskType)
                                    .Where(x => x.IdArchive == idArchive && x.PreservationTaskType != null && !x.Executed
                                        && x.PreservationTaskType.KeyCode == type_ver)
                                    .OrderBy(x => x.EstimatedDate);

                    var ver = queryVer.FirstOrDefault();

                    if (ver != null)
                    {
                        retval.Add(new PreservationTaskDatesResponse
                        {
                            EstimatedExecution = ver.EstimatedDate,
                            ExecutionDate = ver.ExecutedDate.HasValue ? ver.ExecutedDate.Value : DateTime.MaxValue,
                            IsVerifyTask = true,
                            StartDate = ver.StartDocumentDate,
                            EndDate = ver.EndDocumentDate,
                            IdArchive = idArchive,
                        });
                    }
                }

                if (types.Contains(PreservationTaskTypes.Preservation))
                {
                    var queryPres = db.PreservationTask
                       .Include(x => x.PreservationTaskType)
                       .Where(x => x.IdArchive == idArchive && x.PreservationTaskType != null && !x.Executed
                           && x.PreservationTaskType.KeyCode == type_pre)
                       .OrderBy(x => x.EstimatedDate);

                    var con = queryPres.FirstOrDefault();

                    if (con != null)
                    {
                        retval.Add(new PreservationTaskDatesResponse
                        {
                            EstimatedExecution = con.EstimatedDate,
                            ExecutionDate = con.ExecutedDate.HasValue ? con.ExecutedDate.Value : DateTime.MaxValue,
                            IsVerifyTask = false,
                            StartDate = con.StartDocumentDate,
                            EndDate = con.EndDocumentDate,
                            IdArchive = idArchive,
                        });
                    }
                }
            }
            finally
            {
                Dispose();
            }

            return retval;
        }

        public bool EnablePreservationTask(Guid idTaskToEnable)
        {
            var ret = false;

            try
            {
                var dbTask = db.PreservationTask
                    .SingleOrDefault(x => x.IdPreservationTask == idTaskToEnable);

                dbTask.Enabled = true;

                if (requireSave)
                    db.SaveChanges();

                ret = true;
            }
            finally
            {
                Dispose();
            }

            return ret;
        }

        public bool EnablePreservationTaskByActivationPin(Guid idTaskToEnable, Guid activationPin, short mininumDaysOffset)
        {
            var ret = false;

            try
            {
                var dbTask = db.PreservationTask
                    .SingleOrDefault(x => x.IdPreservationTask == idTaskToEnable);

                if (dbTask == null)
                    throw new Exception(string.Format("Il task con ID {0} non esiste in banca dati.", idTaskToEnable));

                if(!dbTask.ActivationPin.HasValue)
                    throw new Exception(string.Format("Il task con ID {0} non ha alcun PIN assegnato in banca dati.", idTaskToEnable));

                if(dbTask.ActivationPin.Value != activationPin)
                    throw new Exception(string.Format("Il task con ID {0} ha un PIN differente rispetto a {1} .", idTaskToEnable, activationPin));

                dbTask.EstimatedDate = DateTime.Now.AddDays(mininumDaysOffset);
                dbTask.Enabled = true;

                if (requireSave)
                    db.SaveChanges();

                ret = true;
            }
            finally
            {
                Dispose();
            }

            return ret;
        }

        public Objects.PreservationSchedule GetPreservationScheduleWithinArchive(Guid idArchive)
        {
            Objects.PreservationSchedule retval = null;

            try
            {
                var query = db.PreservationScheduleArchive
                    .Include(x => x.PreservationSchedule)  
                    .Where(x => x.IdArchive == idArchive && x.IsActive)
                    .Select(x => x.PreservationSchedule)
                    .SingleOrDefault();

                if (query != null)
                    retval = query.Convert();                
            }
            finally
            {
                Dispose();
            }

            return retval;
        }


        /// <summary>
        /// </summary>
        /// <param name="idPreservationTask"></param>
        /// <param name="updateAlsoCorrelatedTasks"></param>
        public void ResetPreservationTask(Guid idPreservationTask, bool checkOnlyErrors, bool forceAutoInc)
        {
            try
            {
                db.Preservation_SP_ResetPreservationTask(idPreservationTask, checkOnlyErrors, forceAutoInc);
            }
            finally
            {
                Dispose();                
            }
        }

        public Obj.PreservationScheduleArchive AddPreservationScheduleArchive(Obj.PreservationScheduleArchive toAdd)
        {
            if (toAdd == null)
                throw new Exception("AddPreservationScheduleArchive - nessun parametro passato");

            if (toAdd.Archive == null)
            {
                if (toAdd.IdArchive == Guid.Empty)
                    throw new Exception("AddPreservationScheduleArchive - archivio non passato");
                toAdd.Archive = new Obj.DocumentArchive(toAdd.IdArchive);
            }

            if (toAdd.Schedule == null)
            {
                if (toAdd.IdSchedule == Guid.Empty)
                    throw new Exception("AddPreservationScheduleArchive - nessuno scadenziario passato");
                toAdd.Schedule = new Obj.PreservationSchedule { IdPreservationSchedule = toAdd.IdSchedule };
            }

            Obj.PreservationScheduleArchive retval = null;
            Guid idArchive = toAdd.Archive.IdArchive, idSchedule = toAdd.Schedule.IdPreservationSchedule;
            try
            {
                var entity = db.PreservationScheduleArchive
                    .SingleOrDefault<Model.PreservationScheduleArchive>(x=>x.IdArchive == idArchive);
                
                if (entity != null)
                {
                    entity.IdPreservationSchedule = idSchedule;
                    entity.IsActive = true;
                    entity.ModifiedDate = DateTime.Now;
                }
                else
                {
                    entity = new Model.PreservationScheduleArchive
                    {
                        IdArchive = idArchive,
                        IdPreservationSchedule = idSchedule,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                    };

                    db.PreservationScheduleArchive.AddObject(entity);
                }

                if (requireSave)
                {
                    db.SaveChanges();
                }

                retval = entity.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL);
            }
            finally
            {
                Dispose();
            }

            return retval;
        }
    }
}
