using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Objects.Enums;
using System.Data.Common;
using BiblosDS.Library.Common.DB;
using BiblosDS.Library.Helper;

namespace BiblosDS.Library.Common.Preservation.Services
{
    public partial class PreservationService
    {
        public BindingList<PreservationTask> CreatePreservationTask(BindingList<PreservationTask> tasks)
        {
            logger.Info("CreatePreservationTask - START");
            try
            {
                var retval = DbProvider.CreatePreservationTask(tasks);
                logger.Info("CreatePreservationTask - END");
                return retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public BindingList<PreservationTask> UpdatePreservationTask(BindingList<PreservationTask> tasks, bool updateCorrelatedTasks)
        {
            logger.InfoFormat("UpdatePreservationTask - Aggiorna task correlati? {0}", updateCorrelatedTasks ? "S" : "N");
            try
            {
                var retval = DbProvider.UpdatePreservationTask(tasks, updateCorrelatedTasks);
                logger.Info("UpdatePreservationTask - END");
                return retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public PreservationTask GetPreservationTask(Guid idTask)
        {
            logger.InfoFormat("GetPreservationTask - id task {0}", idTask);
            try
            {
                var ret = DbProvider.GetPreservationTask(idTask);
                logger.InfoFormat("GetPreservationTask - the task was {0}. Returning to caller.", (ret == null) ? "NOT FOUNDED" : "FOUNDED");
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public List<PreservationTask> GetPreservationVerify(Guid[] idArchives, bool? inError = null)
        {
            logger.DebugFormat("GetPreservationVerify");
            try
            {
                return DbProvider.GetPreservationVerify(idArchives, inError);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public List<PreservationTask> GetPreservationVerify(Guid[] idArchives, int skip, int take, out int total, bool? inError = null)
        {
            logger.InfoFormat("GetPreservationVerify - skip {0} take {1}", skip, take);
            total = 0;

            try
            {
                return DbProvider.GetPreservationVerify(idArchives, skip, take, out total, inError);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("END");
            }
        }

        public PreservationTaskResponse GetAllChildPreservationTasks(Guid idTask, int skip, int take)
        {
            logger.InfoFormat("GetPreservationTasks - skip {0} take {1}", skip, take);
            var retval = new PreservationTaskResponse();
            var numRecord = 0L;

            try
            {
                PreservationTask currentTask = DbProvider.GetPreservationTask(idTask);
                idTask = currentTask.IdCorrelatedPreservationTask ?? idTask;
                retval.Tasks = DbProvider.GetAllChildPreservationTasks(idTask, out numRecord, take, skip);
                foreach (var item in retval.Tasks)
                {
                    if (item.TaskType.Type == PreservationTaskTypes.Verify)
                        item.CanExecute = (item.Enabled && DateTime.Compare(DateTime.Now, item.EstimatedDate) >= 0) && (!item.Executed || item.HasError);
                    else
                        item.CanExecute = retval.Tasks.Any(x => x.TaskType.Type == PreservationTaskTypes.Verify && x.Executed && !x.HasError) && (!item.Executed || item.HasError);
                }
                retval.TotalRecords = numRecord;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.Info("GetPreservationTasks - END");
            return retval;
        }


        public ICollection<PreservationTask> GetPreservationActiveTasks(ICollection<Guid> idArchives)
        {
            logger.InfoFormat("GetPreservationActiveTaskTasks - IdArchives", string.Join(", ", idArchives));
            try
            {
                return DbProvider.GetPreservationActiveTasks(idArchives);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("GetPreservationActiveTaskTasks - END");
            }
        }

        public PreservationTaskResponse GetPreservationTasks(BindingList<DocumentArchive> archives, int skip, int take)
        {
            logger.InfoFormat("GetPreservationTasks - skip {0} take {1}", skip, take);
            var retval = new PreservationTaskResponse();
            var numRecord = 0L;

            try
            {
                retval.Tasks = DbProvider.GetPreservationTasks(archives, out numRecord, take, skip);
                retval.TotalRecords = numRecord;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.Info("GetPreservationTasks - END");
            return retval;
        }

        public BindingList<PreservationTaskDatesResponse> GetNextPreservationTaskDatesForArchive(Guid idArchive, BindingList<PreservationTaskTypes> types)
        {
            logger.InfoFormat("GetNextPreservationTaskDatesForArchive - id archive {0}", idArchive);
            try
            {
                var retval = DbProvider.GetNextPreservationTaskDatesForArchive(idArchive, types);
                logger.Info("GetNextPreservationTaskDatesForArchive - END");
                return retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public bool EnablePreservationTask(Guid idTaskToEnable)
        {
            bool retval = false;
            logger.InfoFormat("EnablePreservationTask - id task {0} ", idTaskToEnable);
            try
            {
                retval = DbProvider.EnablePreservationTask(idTaskToEnable);
                logger.Info("EnablePreservationTaskByActivationPin - END");
                return retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.InfoFormat("EnablePreservationTaskByActivationPin - END {0}", (retval) ? "Ok" : "Ko");
            }
        }

        public bool EnablePreservationTaskByActivationPin(Guid idTaskToEnable, Guid activationPin, short mininumDaysOffset)
        {
            bool retval = false;
            logger.InfoFormat("EnablePreservationTaskByActivationPin - id task {0} , pin {1} , offset {2}", idTaskToEnable, activationPin, mininumDaysOffset);
            try
            {
                retval = DbProvider.EnablePreservationTaskByActivationPin(idTaskToEnable, activationPin, mininumDaysOffset);
                logger.Info("EnablePreservationTaskByActivationPin - END");
                return retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.InfoFormat("EnablePreservationTaskByActivationPin - END {0}", (retval) ? "Ok" : "Ko");
            }
        }


        public PreservationSchedule GetPreservationScheduleWithinArchive(Guid idArchive)
        {
            PreservationSchedule retval = null;

            logger.InfoFormat("GetPreservationScheduleWithinArchive - id archive {0}", idArchive);

            try
            {
                retval = DbProvider.GetPreservationScheduleWithinArchive(idArchive);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("GetPreservationScheduleWithinArchive - END");
            }

            return retval;
        }

        public void ResetErrorFlagForPreservationTask(Guid idPreservationTask, bool updateAlsoCorrelatedTasks)
        {
            logger.InfoFormat("ResetErrorFlagForPreservationTask - id task {0}, update also parent and children {2}", idPreservationTask, updateAlsoCorrelatedTasks ? "YES" : "NO");

            try
            {
                DbProvider.ResetPreservationTask(idPreservationTask, true, PreservationHelper.GetForceAutoInc(ArchiveConfigFile));
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("ResetErrorFlagForPreservationTask - END");
            }
        }

        public void ResetPreservationTask(Guid idPreservationTask, bool updateAlsoCorrelatedTasks)
        {
            logger.InfoFormat("ResetPreservationTask - id task {0}, update also parent and children {2}", idPreservationTask, updateAlsoCorrelatedTasks ? "YES" : "NO");

            try
            {
                DbProvider.ResetPreservationTask(idPreservationTask, false, PreservationHelper.GetForceAutoInc(ArchiveConfigFile));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("ResetPreservationTask - END");
            }
        }

        public PreservationScheduleArchive AddPreservationScheduleArchive(PreservationScheduleArchive toAdd)
        {
            logger.Info("AddPreservationScheduleArchive - START");
            
            PreservationScheduleArchive ret = null;
            try
            {
                ret = DbProvider.AddPreservationScheduleArchive(toAdd);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("ResetErrorFlagForPreservationTask - END");
            }

            return ret;
        }

        public void UpdatePreservationVerifyResetWarningsAndTaskErrors(Guid idPreservation)
        {
            logger.InfoFormat("UpdatePreservationVerifyResetWarningsAndTaskErrors - id {0}", idPreservation);
            try
            {
                DbProvider.UpdatePreservationVerifyResetWarningsAndTaskErrors(idPreservation);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("END");
            }
        }

        public void DeletePreservationVerifyResetTaskErrors(Guid idPreservation)
        {
            logger.InfoFormat("DeletePreservationVerifyResetTaskErrors - id {0}", idPreservation);
            try
            {
                DbProvider.DeletePreservationVerifyResetTaskErrors(idPreservation);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("END");
            }
        }
    }
}
