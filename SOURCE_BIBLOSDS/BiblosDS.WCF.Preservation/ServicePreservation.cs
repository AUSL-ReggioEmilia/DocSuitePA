using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Objects.Enums;
using VecompSoftware.ServiceContract.BiblosDS.Preservations;
using VecompSoftware.BiblosDS.Model.Parameters;

namespace BiblosDS.WCF.Preservation
{
    [ServiceBehavior(
      ConcurrencyMode = ConcurrencyMode.Single,
      InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class ServicePreservation : IServicePreservation
    {
        private static List<IServicePreservationCallback> _callbackList = new List<IServicePreservationCallback>();
        private PreservationService prContext;
        protected PreservationService PreservationContext
        {
            get
            {
                if (this.prContext == null)
                {
                    this.prContext = new PreservationService();
                    this.prContext.OnPulse += new EventHandler<PreservationService.PulseEventArgs>(Context_OnPulse);
                }

                return this.prContext;
            }
        }

        private void Context_OnPulse(object sender, PreservationService.PulseEventArgs e)
        {
            try
            {
                IServicePreservationCallback guest = OperationContext.Current.GetCallbackChannel<IServicePreservationCallback>();

                if (!_callbackList.Contains(guest))
                {
                    _callbackList.Add(guest);
                }

                _callbackList.Where(x => x == guest).ToList().ForEach(
                    delegate(IServicePreservationCallback callback)
                    {
                        try
                        {
                            callback.Pulse(e.Method, e.Message, e.Progress);
                        }
                        catch
                        {
                            try { _callbackList.Remove(callback); }
                            catch { }

                            //TODO log
                        }
                    });
            }
            catch
            {
                //TODO log                
            }
        }

        private Guid GetIdPreservationArchiveFromName(string archiveName)
        {
            return this.PreservationContext.GetIdPreservationArchiveFromName(archiveName);
        }

        private Guid GetIdPreservationRoleFromKeyCode(int keyCode)
        {
            return this.PreservationContext.GetIdPreservationRoleFromKeyCode(keyCode);
        }

        private Guid GetIdPreservationExceptionFromDescription(string exceptionName)
        {
            return this.PreservationContext.GetIdPreservationExceptionFromDescription(exceptionName);
        }

        public BindingList<PreservationArchiveInfoResponse> GetPreservationArchives(string domainUserName)
        {
            return this.PreservationContext.GetPreservationArchives(domainUserName);
        }

        public BindingList<PreservationUserRole> GetUserRoleInArchive(string domainUserName, string archiveName)
        {
            return this.PreservationContext.GetUserRoleInArchive(domainUserName, GetIdPreservationArchiveFromName(archiveName));
        }

        public Dictionary<string, string> GetPreservationParameter(string archiveName)
        {
            var archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new FaultException("Nessun archivio con il nome archivio: " + archiveName);
            return this.PreservationContext.GetPreservationParameter(archive.IdArchive);
        }

        public string GetFirstPreservationParameter(string paramKey)
        {
            return this.PreservationContext.GetFirstPreservationParameter(paramKey);
        }

        public bool IsUserInRoleOnArchive(string archiveName, string domainUserName, int roleId)
        {
            return this.PreservationContext.IsUserInRole(GetIdPreservationArchiveFromName(archiveName), domainUserName, GetIdPreservationRoleFromKeyCode(roleId));
        }

        public bool IsUserInRole(string domainUserName, int roleId)
        {
            return this.PreservationContext.IsUserInRole(domainUserName, GetIdPreservationRoleFromKeyCode(roleId));
        }

        public BindingList<PreservationTaskGroup> GetPreservationTaskGroup(string archiveName)
        {
            return this.PreservationContext.GetPreservationTaskGroup(archiveName);
        }

        public void SetTaskGroupClosed(Guid idTaskGroup, string archiveName)
        {
            this.PreservationContext.SetTaskGroupClosed(idTaskGroup, GetIdPreservationArchiveFromName(archiveName));
        }

        public Guid GetScheduleFromTaskGroup(Guid idTaskGroup)
        {
            return this.PreservationContext.GetScheduleFromTaskGroup(idTaskGroup);
        }

        public BindingList<PreservationSchedule> GetSchedule(Nullable<Guid> idSchedule = null)
        {
            return this.PreservationContext.GetSchedule(idSchedule);
        }

        public void ResetPreparedPreservation(Guid idPreservation)
        {
            this.PreservationContext.ResetPreparedPreservation(idPreservation);
        }

        public BindingList<Document> FindPreparedPreservationObjects(DateTime dateFrom, DateTime dateTo, Guid idArchive)
        {
            return this.PreservationContext.FindPreparedPreservationObjects(dateFrom, dateTo, idArchive);
        }

        public string GetPreservationName(Guid idPreservation)
        {
            return this.PreservationContext.GetPreservationName(idPreservation);
        }

        public void MarkPreservationAsSigned(Guid idPreservation, byte[] signedFile, byte[] timeStampFile)
        {
            try
            {
                this.PreservationContext.MarkPreservationAsSigned(idPreservation, signedFile, timeStampFile);
            }
            catch (Exception ex)
            {
                if (ex is FaultException)
                    throw ex;
                else
                    throw new FaultException(ex.Message);
            }
        }

        public void ClosePreservation(Guid idPreservation)
        {
            this.PreservationContext.ClosePreservation(idPreservation);
        }

        public PreservationInfoResponse AbortConservation(Guid idPreservation)
        {
            return this.PreservationContext.AbortPreservation(idPreservation);
        }

        public bool CheckPreservationExceptions(Guid idPreservation, out string exceptions)
        {
            List<string> errors = new List<string>();
            BiblosDS.Library.Common.Objects.Preservation pres;
            var result =  this.PreservationContext.CheckPreservationExceptions(idPreservation, out errors, null, out pres);
            exceptions = string.Join(Environment.NewLine, errors);
            return result;
        }

        public void CreatePreservationIndexFile(Guid idPreservation, string fileName)
        {
            this.PreservationContext.CreatePreservationIndexFile(idPreservation, fileName);
        }

        public PreservationInfoResponse GetPreservationInfo(string archiveName, Nullable<Guid> idTaskGroup)
        {
            try
            {
                var idArchive = GetIdPreservationArchiveFromName(archiveName);

                return this.PreservationContext.GetPreservationInfo(idArchive, idTaskGroup);
            }
            catch (Exception ex)
            {
                if (ex is FaultException)
                    throw ex;
                else
                    throw new FaultException(ex.Message);
            }
        }

        public PreservationUser GetPreservationUserForArchive(string archiveName, string domainUser)
        {
            return this.PreservationContext.GetPreservationUserForArchive(GetIdPreservationArchiveFromName(archiveName), domainUser);
        }

        public int GetSelectedDocumentsNumber(string archiveName, DateTime startDate, DateTime endDate)
        {
            return this.PreservationContext.GetSelectedDocumentsNumber(GetIdPreservationArchiveFromName(archiveName), startDate, endDate);
        }

        public BindingList<DocumentAttribute> GetAttributes(string archiveName)
        {
            return this.PreservationContext.GetAttributes(GetIdPreservationArchiveFromName(archiveName));
        }

        public string CopyCompEDViewerIndexFile(Guid idPreservation, string workingDirectory)
        {
            return this.PreservationContext.CopyCompEDViewerIndexFile(idPreservation, workingDirectory);
        }

        public BindingList<PreservationHoliday> GetHolidays(Nullable<Guid> idPreservationHolidays)
        {
            return this.PreservationContext.GetHolidays(idPreservationHolidays);
        }

        public string CreatePreservationClosingFile(Guid idPreservation, string workingDir, string exceptions)
        {
            return this.PreservationContext.CreatePreservationClosingFile(idPreservation, workingDir, exceptions);
        }

        public BindingList<PreservationTaskGroup> GetTaskGroup(Nullable<Guid> idPreservationTaskGroup = null)
        {
            return this.PreservationContext.GetTaskGroup(idPreservationTaskGroup);
        }

        public BindingList<PreservationAlert> GetPreservationAlert(Nullable<Guid> idAlert, Nullable<Guid> idTaskType, Nullable<Guid> idAlertType, Nullable<Guid> idTask, bool orderByOffset = false)
        {
            return this.PreservationContext.GetPreservationAlert(idAlert, idTaskType, idAlertType, idTask, orderByOffset);
        }

        public BindingList<PreservationTaskGroupType> GetTaskGroupTypes(Nullable<Guid> idTaskGroupType)
        {
            return this.PreservationContext.GetTaskGroupTypes(idTaskGroupType);
        }

        public BindingList<PreservationUser> GetPreservationUser(Nullable<Guid> idUser, string archiveName)
        {
            return this.PreservationContext.GetPreservationUser(idUser, this.GetIdPreservationArchiveFromName(archiveName));
        }

        public BindingList<PreservationTaskType> GetPreservationTaskTypes(Nullable<Guid> idPreservationTaskType)
        {
            return this.PreservationContext.GetPreservationTaskTypes(idPreservationTaskType);
        }

        public BindingList<PreservationTask> GetTasksFromTaskGroup(Guid idTaskGroup)
        {
            return this.PreservationContext.GetTasksFromTaskGroup(idTaskGroup);
        }

        public PreservationTaskGroup AddPreservationTaskGroup(PreservationTaskGroup toAdd, string archiveName)
        {
            return this.PreservationContext.AddPreservationTaskGroup(toAdd, prContext.GetIdPreservationArchiveFromName(archiveName));
        }

        public PreservationTask AddPreservationTask(PreservationTask toAdd, string archiveName)
        {
            return this.PreservationContext.AddPreservationTask(toAdd, this.GetIdPreservationArchiveFromName(archiveName));
        }

        public PreservationAlert AddPreservationAlert(PreservationAlert toAdd)
        {
            return this.PreservationContext.AddPreservationAlert(toAdd);
        }

        public Document GetPreservedDocument(Guid idPreservation, string name)
        {
            return this.PreservationContext.GetPreservedDocument(idPreservation, name);
        }

        public BiblosDS.Library.Common.Objects.Preservation GetPreservation(Guid idPreservation)
        {
            return this.PreservationContext.GetPreservation(idPreservation, false);
        }

        public BindingList<PreservationTaskType> GetPreservationTaskTypesAndPreservationScheduleTaskTypes(Nullable<Guid> idPreservationSchedule, string archiveName)
        {
            return this.PreservationContext.GetPreservationTaskTypesAndPreservationScheduleTaskTypes(idPreservationSchedule, this.GetIdPreservationArchiveFromName(archiveName));
        }

        public PreservationInfoResponse CreatePreservation(string archiveName, string domainUser, Guid idGruppoTask, DateTime dataInizio, DateTime dataFine, bool verifyOnly)
        {
            return this.PreservationContext.CreatePreservation(this.GetIdPreservationArchiveFromName(archiveName), domainUser, idGruppoTask, dataInizio, dataFine, verifyOnly);
        }

        public PreservationFileInfoResponse GetPreservationClosingFileInfo(Guid preservationId)
        {
            return this.PreservationContext.GetPreservationClosingFileInfo(preservationId);
        }

        public PreservationInfoResponse ResetPreservation(Guid idPreservation, string domainUser)
        {
            return this.PreservationContext.ResetPreservation(idPreservation, domainUser);
        }

        public BindingList<BiblosDS.Library.Common.Objects.Preservation> GetPreservations(string archiveName, int take, int skip, out int totalPreservations)
        {
            return this.PreservationContext.GetPreservations(GetIdPreservationArchiveFromName(archiveName), take, skip, out totalPreservations);
        }

        public BindingList<BiblosDS.Library.Common.Objects.Preservation> GetPreservationsToSign(string archiveName, int take, int skip, out int totalPreservations)
        {
            return this.PreservationContext.GetPreservationsToSign(GetIdPreservationArchiveFromName(archiveName), take, skip, out totalPreservations);
        }

        public BindingList<PreservationJournalingActivity> GetPreservationJournalingActivities(Guid? idJournalingActivity)
        {
            return this.PreservationContext.GetPreservationJournalingActivities(idJournalingActivity);
        }

        public void UpdatePreservationJournaling(PreservationJournaling toUpdate)
        {
            this.UpdatePreservationJournaling(toUpdate);
        }

        public void AddVerifyPreservationToJournaling(Guid idPreservation, string domainUser)
        {
            try
            {
                this.PreservationContext.AddVerifyPreservationToJournaling(idPreservation, domainUser);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationJournaling AddPreservationJournaling(PreservationJournaling toAdd)
        {
            return this.PreservationContext.AddPreservationJournaling(toAdd);
        }

        public Nullable<DateTime> GetPreservationJournalingLastPrintManualActivityDate()
        {
            return this.PreservationContext.GetPreservationJournalingLastPrintManualActivityDate();
        }

        public BindingList<PreservationStorageDeviceStatus> GetPreservationStorageDeviceStatus(Nullable<Guid> idStatus)
        {
            return this.PreservationContext.GetPreservationStorageDeviceStatus(idStatus);
        }

        public BindingList<PreservationJournaling> GetPreservationJournalings(string archiveName, Guid? idPreservation, DateTime? startDate, DateTime? endDate, int skip, int take, out int journalingsInArchive)
        {
            Guid? idArchive = null;
            if (!string.IsNullOrEmpty(archiveName))
            {
                idArchive = this.PreservationContext.GetIdPreservationArchiveFromName(archiveName);
            }

            return this.PreservationContext.GetPreservationJournalings(idArchive, idPreservation, startDate, endDate, null, skip, take, out journalingsInArchive);
        }

        public BindingList<BiblosDS.Library.Common.Objects.Preservation> GetPreservationsFromJournaling(Guid idJournaling, int skip, int take, out int preservationsCount)
        {
            return this.PreservationContext.GetPreservationsFromJournaling(idJournaling, skip, take, out preservationsCount);
        }

        public BindingList<BiblosDS.Library.Common.Objects.Preservation> GetPreservationsFromArchive(string archiveName, int take, int skip, out long totalItems)
        {
            return this.PreservationContext.GetPreservationsFromArchive(this.GetIdPreservationArchiveFromName(archiveName), take, skip, out totalItems);
        }

        public CryptoType GetUsedCryptography(byte[] encryptedBuffer)
        {
            return this.PreservationContext.GetUsedCryptography(encryptedBuffer);
        }

        public string GetHashFromFile(string filePathName, bool useSHA256)
        {
            return this.PreservationContext.GetHashFromFile(filePathName, useSHA256);
        }

        public string GetHashFromBuffer(byte[] buffer, bool useSHA256)
        {
            return this.PreservationContext.GetHashFromBuffer(buffer, useSHA256);
        }

        public BindingList<BiblosDS.Library.Common.Objects.ArchiveCompany> GetArchiveCompany(Guid? idArchive, string companyName)
        {
            try
            {
                return this.PreservationContext.GetArchiveCompany(idArchive, companyName);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
        }

        public BindingList<BiblosDS.Library.Common.Objects.ArchiveCompany> GetArchiveCompanyByUser(Guid? idArchive, string companyName, string username)
        {
            try
            {
                return this.PreservationContext.GetArchiveCompanyByUser(idArchive, companyName, username);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
        }


        public bool IsAlive()
        {
            PreservationContext.IsAlive();
            return true;
        }


        public PreservationInfoResponse CreatePreservationByTask(PreservationTask task)
        {
            try
            {
                return this.PreservationContext.CreatePreservation(task);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
        }
    }
}
