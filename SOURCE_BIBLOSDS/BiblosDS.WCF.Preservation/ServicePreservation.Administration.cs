using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Preservation.Services;
using System.ServiceModel.Activation;
using System.ComponentModel;
using System.ServiceModel;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Utility;


namespace BiblosDS.WCF.Preservation
{
    public partial class ServicePreservation
    {
        public BindingList<DocumentArchive> GetAdministratedArchives(bool onlyPreservedArchives)
        {
            try
            {
                BindingList<DocumentArchive> retval;

                if (onlyPreservedArchives)
                    retval = new PreservationService().GetArchivesWithPreservations();
                else
                {
                    retval = ArchiveService.GetArchives();
                }

                return new BindingList<DocumentArchive>(retval.OrderBy(x => x.Name).ToList());
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public BindingList<PreservationUser> GetPreservationUsersInArchiveByRole(Guid idRole, string archiveName)
        {
            try
            {
                return new PreservationService().GetPreservationUsersInArchiveByRole(idRole, archiveName);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public BindingList<PreservationRole> GetRoles(Guid? idRole)
        {
            try
            {
                return new PreservationService().GetRoles(idRole);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationUser(PreservationUser user, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationUser(user, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationUser AddPreservationUser(PreservationUser user, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.AddPreservationUser(user, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationUserRole AddPreservationUserRole(PreservationUserRole userRole, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                if (!string.IsNullOrEmpty(archiveName))
                {
                    if (userRole.Archive == null)
                        userRole.Archive = new DocumentArchive();

                    userRole.Archive.IdArchive = svc.GetIdPreservationArchiveFromName(archiveName);
                }

                return svc.AddPreservationUserRole(userRole, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }          
        }

        public void DeletePreservationUserRolesByPreservationUser(Guid idPreservationUser)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationUserRolesByPreservationUser(idPreservationUser);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void DeletePreservationUser(Guid idPreservationUser)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationUser(idPreservationUser);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationSchedule(PreservationSchedule sched, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationSchedule(sched, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void DeletePreservationSchedule(Guid idPreservationSchedule)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationSchedule(idPreservationSchedule);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationSchedule AddPreservationSchedule(PreservationSchedule sched, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.AddPreservationSchedule(sched, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public void DeletePreservationSchedule_TaskTypeBySchedule(Guid idPreservationSchedule)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationSchedule_TaskTypeBySchedule(idPreservationSchedule);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public BindingList<PreservationTaskType> GetTaskTypesByUserRole(Nullable<Guid> idPreservationUserRole, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.GetTaskTypesByUserRole(idPreservationUserRole, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationTaskType(PreservationTaskType taskType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationTaskType(taskType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationTaskType AddPreservationTaskType(PreservationTaskType taskType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.AddPreservationTaskType(taskType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public BindingList<PreservationTaskGroup> GetDetailedPreservationTaskGroup(Nullable<Guid> idTaskGroup, string archiveName, int maxReturnedValues)
        {
            try
            {
                var svc = new PreservationService();
                return svc.GetDetailedPreservationTaskGroup(idTaskGroup, svc.GetIdPreservationArchiveFromName(archiveName), maxReturnedValues);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationTaskGroupExpiryByTask(Guid idPreservationTask, DateTime newEstimatedExpiry, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationTaskGroupExpiryByTask(idPreservationTask, newEstimatedExpiry, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public BindingList<BiblosDS.Library.Common.Objects.Preservation> GetPreservationsByUserAndTask(Guid idPreservationUser, string taskName, string archiveName)
        {
            throw new NotImplementedException();
            //return this.PreservationSvc.GetPreservationsByUserAndTask(idPreservationTask, "", this.PreservationSvc.GetIdPreservationArchiveFromName(archiveName));
        }

        public PreservationHoliday AddPreservationHoliday(PreservationHoliday holiday, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.AddPreservationHoliday(holiday, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationHoliday(PreservationHoliday holiday, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationHoliday(holiday, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void DeletePreservationHoliday(Guid idPreservationHoliday, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationHoliday(idPreservationHoliday, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public BindingList<PreservationAlertType> GetPreservationAlertTypes(Nullable<Guid> idPreservationAlertType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.GetPreservationAlertTypes(idPreservationAlertType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationAlertType(PreservationAlertType alertType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationAlertType(alertType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public PreservationAlertType AddPreservationAlertType(PreservationAlertType alertType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.AddPreservationAlertType(alertType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void DeletePreservationAlertType(Guid idPreservationAlertType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationAlertType(idPreservationAlertType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public BindingList<PreservationAlertType> GetPreservationAlertTypesByTaskType(Nullable<Guid> idPreservationAlertType, Guid idPreservationTaskType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.GetPreservationAlertTypesByTaskType(idPreservationAlertType, idPreservationTaskType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationRole(PreservationRole role, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationRole(role, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationRole AddPreservationRole(PreservationRole role, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.AddPreservationRole(role, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void AddPreservationParameter(string label, string value, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                if (!string.IsNullOrEmpty(archiveName))
                    svc.AddPreservationParameter(label, value, svc.GetIdPreservationArchiveFromName(archiveName));
                else
                    throw new FaultException("Non e' possibile aggiungere un parametro di conservazione senza specificare l'archivio associato.");
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
            
        }

        public void UpdatePreservationParameter(string label, string value, string filterName, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                if (!string.IsNullOrEmpty(archiveName))
                    svc.UpdatePreservationParameter(label, value, filterName, svc.GetIdPreservationArchiveFromName(archiveName));
                else
                    svc.UpdatePreservationParameter(label, value, filterName, null);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public void DeletePreservationParameter(string label, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                if (!string.IsNullOrEmpty(archiveName))
                    svc.DeletePreservationParameter(label, svc.GetIdPreservationArchiveFromName(archiveName));
                else
                    svc.DeletePreservationParameter(label, null);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationExpireResponse GetPreservationExpire(Guid idSchedule, Guid idTaskGroupType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.GetPreservationExpire(idSchedule, idTaskGroupType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationAsSigned(Guid idPreservation, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationAsSigned(idPreservation, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void UpdatePreservationPath(Guid idPreservation, string path, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationPath(idPreservation, path, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
            
        }

        public void DeletePreservationTaskGroup(Guid idTaskGroup, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationTaskGroup(idTaskGroup, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public void UpdatePreservationTaskGroupTypeDescription(Guid idTaskGroupType, string description)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationTaskGroupTypeDescription(idTaskGroupType, description);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public PreservationTaskGroupType AddPreservationTaskGroupType(PreservationTaskGroupType groupType, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                return svc.AddPreservationTaskGroupType(groupType, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public BindingList<PreservationStorageDevice> GetPreservationStorageDevices(Nullable<Guid> idPreservationStorageDevice)
        {
            try
            {
                var svc = new PreservationService();
                return svc.GetPreservationStorageDevices(idPreservationStorageDevice);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public BindingList<PreservationInStorageDevice> GetPreservationsInStorageDevices(Guid? idPreservation, Guid? idPreservationStorageDevice, int skip, int take, out int totalItems)
        {
            try
            {
                var svc = new PreservationService();
                return svc.GetPreservationsInStorageDevices(idPreservation, idPreservationStorageDevice, skip, take, out totalItems);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationInStorageDevice AddPreservationInStorageDevice(PreservationInStorageDevice toAdd)
        {
            try
            {
                var svc = new PreservationService();
                return svc.AddPreservationInStorageDevice(toAdd);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public void DeletePreservationInStorageDevice(PreservationInStorageDevice preservationInStorageDevice)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationInStorageDevice(preservationInStorageDevice);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public void DeletePreservationStorageDevice(PreservationStorageDevice preservationStorageDevice)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationStorageDevice(preservationStorageDevice);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }            
        }

        public PreservationStorageDevice AddPreservationStorageDevice(PreservationStorageDevice toAdd)
        {
            var svc = new PreservationService();
            try
            {
                return svc.AddPreservationStorageDevice(toAdd);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
        }

        public PreservationStorageDeviceStatus PreservationStorageDeviceChangeStatus(Guid idPreservation, PreservationStatus preservationStatus)
        {
            try
            {
                var svc = new PreservationService();
                return svc.PreservationStorageDeviceChangeStatus(idPreservation, preservationStatus);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
            
        }

        public void DeletePreservationJournaling(Guid idJournaling, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                svc.DeletePreservationJournaling(idJournaling, svc.GetIdPreservationArchiveFromName(archiveName));
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public BindingList<PreservationStorageDevice> GetPreservationStorageDeviceFromLabel(string label, string archiveName)
        {
            try
            {
                var svc = new PreservationService();
                if (string.IsNullOrEmpty(archiveName))
                {
                    return svc.GetPreservationStorageDeviceFromLabel(label, null);
                }
                else
                {
                    return svc.GetPreservationStorageDeviceFromLabel(label, svc.GetIdPreservationArchiveFromName(archiveName));
                }
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }           
        }

        public BindingList<PreservationStorageDevice> GetPreservationStorageDevicesFromDates(Guid? idPreservationStorageDevice, DateTime? minDate, DateTime? maxDate, string username, int skip, int take, out long totalItems)
        {
            try
            {
                return this.PreservationContext.GetPreservationStorageDevicesFromDates(idPreservationStorageDevice, minDate, maxDate, username, skip, take, out totalItems);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }                 
        }

        public void UpdatePreservationStorageDeviceLastVerifyDate(Guid idStorageDevice, Nullable<DateTime> verifyDate)
        {
            try
            {
                var svc = new PreservationService();
                svc.UpdatePreservationStorageDeviceLastVerifyDate(idStorageDevice, verifyDate);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }          
        }

        public bool CreateArchivePreservationMark(Guid idStorageDevice)
        {
            try
            {
                var svc = new PreservationService();
                return svc.CreateArchivePreservationMark(idStorageDevice);
            }
            catch (Exception ex)
            {               
                throw CheckExceptionToThrow(ex);
            }            
        }

        public byte[] GetArchivePreservationMarkFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize)
        {
            return this.PreservationContext.GetArchivePreservationMarkFile(idPreservationStorageDevice, skip, take, out fileSize);
        }

        public byte[] GetClosingFilesTimeStampFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize)
        {
            try
            {
                var svc = new PreservationService();
                return svc.GetClosingFilesTimeStampFile(idPreservationStorageDevice, skip, take, out fileSize);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }  
        }

        public bool TimeStampArchivePreservationMarkFile(Guid idPreservationStorageDevice, byte[] timeStampedFile, bool isInfoCamere)
        {
            return this.PreservationContext.TimeStampArchivePreservationMarkFile(idPreservationStorageDevice, timeStampedFile, isInfoCamere);
        }

        public byte[] GetTimeStampedArchivePreservationMarkFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize)
        {
            return this.PreservationContext.GetTimeStampedArchivePreservationMarkFile(idPreservationStorageDevice, skip, take, out fileSize);
        }

        public int GetPreservationStorageDevicesCount()
        {
            return this.PreservationContext.GetPreservationStorageDevicesCount();
        }

        public string GetSHA1Mark(byte[] content)
        {
            return UtilityService.GetHash(content, false);
        }

        public string GetSHA256Mark(byte[] content)
        {
            return UtilityService.GetHash(content, true);
        }

        public BindingList<BiblosDS.Library.Common.Objects.Preservation> GetPreservationsFromArchiveAndDates(string archiveName, DateTime minDate, DateTime maxDate, int take, int skip, out long totalItems)
        {
            return this.PreservationContext.GetPreservationsFromArchiveAndDates(this.PreservationContext.GetIdPreservationArchiveFromName(archiveName),
                minDate,
                maxDate,
                take,
                skip,
                out totalItems);
        }

        public PreservationSchedule GetDefaultPreservationSchedule()
        {
            return this.PreservationContext.GetDefaultPreservationSchedule();
        }

        public string UploadEntratelFile(string clientFileName, Guid idPreservationStorageDevice, byte[] fileContent)
        {
            try
            {
                return this.PreservationContext.UploadEntratelFile(clientFileName, idPreservationStorageDevice, fileContent);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
        }

        public byte[] DownloadEntratelFile(Guid idPreservationStorageDevice, long skip, int take, out long fileLenght)
        {
            try
            {
                return this.PreservationContext.DownloadEntratelFile(idPreservationStorageDevice, skip, take, out fileLenght);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
        }

        public string GetCurrentArchivePreservationMarkXmlTemplate(string companyName)
        {
            try
            {
                return PreservationContext.GetCurrentArchivePreservationMarkXmlTemplate(companyName);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
        }

        public void ChangeCurrentArchivePreservationMarkXmlTemplate(string xmlContent, string companyName)
        {
            try
            {
                PreservationContext.ChangeCurrentArchivePreservationMarkXmlTemplate(xmlContent, companyName);
            }
            catch (Exception ex)
            {
                throw CheckExceptionToThrow(ex);
            }
        }

        private Exception CheckExceptionToThrow(Exception ex)
        {
            if (ex is FaultException)
                return ex;
            else
                return new FaultException(ex.Message);
        }
    }
}
