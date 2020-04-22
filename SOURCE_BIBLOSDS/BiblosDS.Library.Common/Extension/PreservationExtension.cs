using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using BibDSModel = BiblosDS.Library.Common.Model;
using System.ComponentModel;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using BiblosDS.Library.Common.Objects.Enums;

namespace System
{
    public static class PreservationExtension
    {
        public static string GetOrderAttributeValueString(this IEnumerable<DocumentAttributeValue> attrs)
        {
            List<string> res = new List<string>();
            foreach (var val in attrs.OrderBy(x => x.Attribute.KeyOrder.GetValueOrDefault()))
            {
                if (val == null || val.Value == null)
                    continue;
                res.Add(val.Value.ToString().ToUpper());
            }
            return string.Join("|", res);
        }

        public static long GetOrderAttributeValue(this IEnumerable<DocumentAttributeValue> attrs)
        {
            //if (attrs.Count() > 1)
            //{
            //    logger.WarnFormat("Attributo non univoco: {0}", string.Join("|", attrs.Select(x => x.Attributes.Name)));
            //}
            var val = attrs.FirstOrDefault();
            if (val == null || val.Value == null)
                return 0;
            if (val.Value is Int64 || val.Value is double)
                return (long)((Int64)val.Value);
            else
            {
                long res = 0;
                long.TryParse(val.Value.ToString(), out res);
                return res;
            }
        }
        #region MODELLO -> OGGETTO BIBLOS

        internal static Preservation ConvertPreservation(this BibDSModel.PreservationJournalingTableValuedResult valuedResult)
        {
            if (valuedResult == null || !valuedResult.Preservation_IdPreservation.HasValue)
            {
                return null;
            }
            return new Preservation()
            {
                CloseDate = valuedResult.Preservation_CloseDate,
                EndDate = valuedResult.Preservation_EndDate,
                IdArchive = valuedResult.Preservation_IdArchive.Value,
                IdPreservation = valuedResult.Preservation_IdPreservation.Value,
                IdPreservationUser = valuedResult.Preservation_IdPreservationUser,
                IndexHash = valuedResult.Preservation_IndexHash,
                Label = valuedResult.Preservation_Label,
                LastVerifiedDate = valuedResult.Preservation_LastVerifiedDate,
                Path = valuedResult.Preservation_Path,
                StartDate = valuedResult.Preservation_StartDate,
                PreservationDate = valuedResult.Preservation_PreservationDate,
                PreservationSize = valuedResult.Preservation_PreservationSize,
                IdDocumentCloseFile = valuedResult.Preservation_IdDocumentClose,
                IdDocumentIndexFile = valuedResult.Preservation_IdDocumentIndex,
                IdDocumentIndexFileXML = valuedResult.Preservation_IdDocumentIndexXml,
                IdDocumentIndexFileXSLT = valuedResult.Preservation_IdDocumentIndexXSLT,
                IdDocumentSignedCloseFile = valuedResult.Preservation_IdDocumentCloseSigned,
                IdDocumentSignedIndexFile = valuedResult.Preservation_IdDocumentIndedSigned,
                LastSectionalValue = valuedResult.Preservation_LastSectionalValue,
                IdArchiveBiblosStore = valuedResult.Preservation_IdArchiveBiblosStore,
                LockOnDocumentInsert = valuedResult.Preservation_LockOnDocumentInsert,
            };
        }

        internal static Preservation Convert(this BibDSModel.PreservationTableValuedResult valuedResult)
        {
            if (valuedResult == null)
            {
                return null;
            }
            return new Preservation()
            {
                CloseContent = valuedResult.CloseContent,
                CloseDate = valuedResult.CloseDate,
                EndDate = valuedResult.EndDate,
                IdArchive = valuedResult.IdArchive,
                IdPreservation = valuedResult.IdPreservation,
                IdPreservationUser = valuedResult.IdPreservationUser,
                IndexHash = valuedResult.IndexHash,
                Label = valuedResult.Label,
                LastVerifiedDate = valuedResult.LastVerifiedDate,
                Path = valuedResult.Path,
                StartDate = valuedResult.StartDate,
                PreservationDate = valuedResult.PreservationDate,
                PreservationSize = valuedResult.PreservationSize,
                IdDocumentCloseFile = valuedResult.IdDocumentClose,
                IdDocumentIndexFile = valuedResult.IdDocumentIndex,
                IdDocumentIndexFileXML = valuedResult.IdDocumentIndexXml,
                IdDocumentIndexFileXSLT = valuedResult.IdDocumentIndexXSLT,
                IdDocumentSignedCloseFile = valuedResult.IdDocumentCloseSigned,
                IdDocumentSignedIndexFile = valuedResult.IdDocumentIndedSigned,
                LastSectionalValue = valuedResult.LastSectionalValue,
                IdArchiveBiblosStore = valuedResult.IdArchiveBiblosStore,
                LockOnDocumentInsert = valuedResult.LockOnDocumentInsert,
                Archive = valuedResult.ConvertArchive(),
                TaskGroup = valuedResult.ConvertPreservationTaskGroup(),
                Task = valuedResult.ConvertPreservationTask(),
                User = valuedResult.ConvertPreservationUser()
            };
        }

        internal static Preservation Convert(this BibDSModel.Preservation pre, int level = 0, int deeplevel = 5, params Type[] ignoredTypes)
        {
            if (pre == null || level > deeplevel)
                return null;

            var docs = new BindingList<Document>();

            var retval = new Preservation
            {
                //Archive = pre.Archive.Convert(level + 1, deeplevel),
                CloseContent = pre.CloseContent,
                CloseDate = pre.CloseDate,
                Documents = docs,
                EndDate = pre.EndDate,
                IdArchive = pre.IdArchive,
                IdPreservation = pre.IdPreservation,
                IdPreservationTaskGroup = pre.IdPreservationTaskGroup,
                IdPreservationUser = pre.IdPreservationUser,
                IndexHash = pre.IndexHash,
                Label = pre.Label,
                LastVerifiedDate = pre.LastVerifiedDate,
                Path = pre.Path,
                StartDate = pre.StartDate,
                PreservationDate = pre.PreservationDate,
                PreservationSize = pre.PreservationSize,
                IdDocumentCloseFile = pre.IdDocumentClose,
                IdDocumentIndexFile = pre.IdDocumentIndex,
                IdDocumentIndexFileXML = pre.IdDocumentIndexXml,
                IdDocumentIndexFileXSLT = pre.IdDocumentIndexXSLT,
                IdDocumentSignedCloseFile = pre.IdDocumentCloseSigned,
                IdDocumentSignedIndexFile = pre.IdDocumentIndedSigned,
                LastSectionalValue = pre.LastSectionalValue,
                IdArchiveBiblosStore = pre.IdArchiveBiblosStore,
                LockOnDocumentInsert = pre.LockOnDocumentInsert
                //TaskGroup = pre.PreservationTaskGroup.Convert(level + 1, deeplevel),
                //Task = pre.PreservationTask.Convert(level + 1, deeplevel),
                //User = pre.PreservationUser.Convert(level + 1, deeplevel),
                //PreservationJournalings = pre.PreservationJournaling.Convert(level, deeplevel),
            };

            if (ignoredTypes == null)
                ignoredTypes = new Type[0];

            if (!ignoredTypes.Contains(typeof(DocumentArchive)))
                retval.Archive = pre.Archive.Convert(level + 1, deeplevel);

            if (!ignoredTypes.Contains(typeof(PreservationTaskGroup)))
                retval.TaskGroup = pre.PreservationTaskGroup.Convert(level + 1, deeplevel);

            if (!ignoredTypes.Contains(typeof(PreservationTask)))
                retval.Task = pre.PreservationTask.Convert(level + 1, deeplevel);

            if (!ignoredTypes.Contains(typeof(PreservationUser)))
                retval.User = pre.PreservationUser.Convert(level + 1, deeplevel);

            if (!ignoredTypes.Contains(typeof(PreservationJournaling)))
                retval.PreservationJournalings = pre.PreservationJournaling.Convert(level, deeplevel);

            if (!ignoredTypes.Contains(typeof(Document)))
            {
                foreach (var d in pre.PreservationDocuments)
                {
                    docs.Add(d.Document.Convert(level + 1, deeplevel, null));
                }
                retval.Documents = docs;
            }

            retval.ClearModifiedField();
            return retval;
        }

        internal static PreservationJournaling Convert(this BibDSModel.PreservationJournalingTableValuedResult valuedResult)
        {
            if (valuedResult == null)
            {
                return null;
            }

            return new PreservationJournaling()
            {
                DateActivity = valuedResult.DateActivity,
                DateCreated = valuedResult.DateCreated,
                DomainUser = valuedResult.DomainUser,
                User = valuedResult.ConvertPreservationUser(),
                IdPreservation = valuedResult.IdPreservation,
                IdPreservationJournaling = valuedResult.IdPreservationJournaling,
                IdPreservationJournalingActivity = valuedResult.IdPreservationJournalingActivity,
                Preservation = valuedResult.ConvertPreservation(),
                PreservationJournalingActivity = valuedResult.ConvertPreservationJournalingActivity(),
                Notes = valuedResult.Notes,
            };
        }

        internal static PreservationJournaling Convert(this BibDSModel.PreservationJournaling jou, int level = 0, int deeplevel = 5)
        {
            if (jou == null || level > deeplevel)
                return null;

            return new PreservationJournaling
            {
                DateActivity = jou.DateActivity,
                DateCreated = jou.DateCreated,
                DomainUser = jou.DomainUser,
                User = jou.PreservationUser.Convert(level + 1, deeplevel),
                IdPreservation = jou.IdPreservation,
                IdPreservationJournaling = jou.IdPreservationJournaling,
                IdPreservationJournalingActivity = jou.IdPreservationJournalingActivity,
                Preservation = jou.Preservation.Convert(level + 1, deeplevel),
                PreservationJournalingActivity = jou.PreservationJournalingActivity.Convert(level + 1, deeplevel),
                Notes = jou.Notes,
            };
        }

        internal static BindingList<PreservationJournaling> Convert(this EntityCollection<BibDSModel.PreservationJournaling> jou, int level = 0, int deeplevel = 5)
        {
            if (jou == null || level > deeplevel)
                return null;

            var retval = new BindingList<PreservationJournaling>();

            foreach (var entity in jou)
            {
                retval.Add(entity.Convert(level, deeplevel));
            }

            return retval;
        }

        internal static PreservationJournalingActivity ConvertPreservationJournalingActivity(this BibDSModel.PreservationJournalingTableValuedResult valuedResult)
        {
            if (valuedResult == null)
            {
                return null;
            }

            return new PreservationJournalingActivity()
            {
                Description = valuedResult.PreservationJournalingActivity_Description,
                IdPreservationJournalingActivity = valuedResult.PreservationJournalingActivity_IdPreservationJournalingActivity,
                IsUserActivity = valuedResult.PreservationJournalingActivity_IsUserActivity.GetValueOrDefault(false),
                KeyCode = valuedResult.PreservationJournalingActivity_KeyCode,
            };
        }

        internal static PreservationJournalingActivity Convert(this BibDSModel.PreservationJournalingActivity act, int level = 0, int deeplevel = 5)
        {
            if (act == null || level > deeplevel)
                return null;

            return new PreservationJournalingActivity
            {
                Description = act.Description,
                IdPreservationJournalingActivity = act.IdPreservationJournalingActivity,
                IsUserActivity = act.IsUserActivity.HasValue && act.IsUserActivity.Value,
                KeyCode = act.KeyCode,
            };
        }

        internal static PreservationUser ConvertPreservationUser(this BibDSModel.PreservationTableValuedResult valuedResult)
        {
            if (valuedResult == null || valuedResult.PreservationUser_IdPreservationUser == null)
                return null;

            return new PreservationUser()
            {
                IdPreservationUser = valuedResult.PreservationUser_IdPreservationUser.Value,
                Address = valuedResult.PreservationUser_Address,
                DomainUser = valuedResult.PreservationUser_DomainUser,
                EMail = valuedResult.PreservationUser_Email,
                Enabled = valuedResult.PreservationUser_Enable.GetValueOrDefault(),
                FiscalId = valuedResult.PreservationUser_FiscalId,
                Name = valuedResult.PreservationUser_Name,
                Surname = valuedResult.PreservationUser_Surname,
            };
        }

        internal static PreservationUser ConvertPreservationUser(this BibDSModel.PreservationJournalingTableValuedResult valuedResult)
        {
            if (valuedResult == null)
                return null;

            return new PreservationUser()
            {
                IdPreservationUser = valuedResult.PreservationUser_IdPreservationUser,
                Address = valuedResult.PreservationUser_Address,
                DomainUser = valuedResult.PreservationUser_DomainUser,
                EMail = valuedResult.PreservationUser_Email,
                Enabled = valuedResult.PreservationUser_Enable,
                FiscalId = valuedResult.PreservationUser_FiscalId,
                Name = valuedResult.PreservationUser_Name,
                Surname = valuedResult.PreservationUser_Surname,
            };
        }

        internal static PreservationUser Convert(this BibDSModel.PreservationUser usr, int level = 0, int deeplevel = 5)
        {
            if (usr == null || level > deeplevel)
                return null;

            var retval = new PreservationUser
            {
                IdPreservationUser = usr.IdPreservationUser,
                Address = usr.Address,
                DomainUser = usr.DomainUser,
                EMail = usr.Email,
                Enabled = usr.Enable,
                FiscalId = usr.FiscalId,
                Name = usr.Name,
                Surname = usr.Surname,
            };


            retval.UserRoles = usr.PreservationUserRole.Convert(level + 1, deeplevel);
            return retval;
        }

        internal static PreservationUserRole Convert(this BibDSModel.PreservationUserRole rol, int level = 0, int deeplevel = 5)
        {
            if (rol == null || level > deeplevel)
                return null;

            return new PreservationUserRole
            {
                //Archive = rol.Archive.Convert(level + 1, deeplevel),
                Archive = null,
                IdArchive = rol.IdArchive,
                IdPreservationRole = rol.IdPreservationRole,
                IdPreservationUser = rol.IdPreservationUser,
                IdPreservationUserRole = rol.IdPreservationUserRole,
                //PreservationRole = rol.PreservationRole.Convert(level + 1, deeplevel),
                //PreservationUser = rol.PreservationUser.Convert(level + 1, deeplevel),
            };
        }

        internal static BindingList<PreservationUserRole> Convert(this IEnumerable<BibDSModel.PreservationUserRole> rol, int level = 0, int deeplevel = 5)
        {
            if (rol == null || level > deeplevel)
                return null;

            BindingList<PreservationUserRole> retval = new BindingList<PreservationUserRole>();

            foreach (var entity in rol)
            {
                retval.Add(entity.Convert(level, deeplevel));
            }

            return retval;
        }

        internal static BindingList<PreservationUserRole> Convert(this EntityCollection<BibDSModel.PreservationUserRole> rol, int level = 0, int deeplevel = 5)
        {
            if (rol == null || level > deeplevel)
                return null;

            BindingList<PreservationUserRole> retval = new BindingList<PreservationUserRole>();

            foreach (var entity in rol)
            {
                retval.Add(entity.Convert(level, deeplevel));
            }

            return retval;
        }

        internal static PreservationSchedule Convert(this BibDSModel.PreservationSchedule sch, int level = 0, int deeplevel = 5)
        {
            if (sch == null || level > deeplevel)
                return null;

            var retval = new PreservationSchedule
            {
                Active = sch.Active == 1,
                FrequencyType = sch.FrequencyType,
                IdPreservationSchedule = sch.IdPreservationSchedule,
                Name = sch.Name,
                Period = sch.Period,
                ValidWeekDays = sch.ValidWeekDays,
                PreservationScheduleTaskTypes = new BindingList<PreservationScheduleTaskType>(),
                Default = sch.IsDefault == 1,
            };

            if (sch.PreservationSchedule_TaskType != null)
            {
                foreach (var tt in sch.PreservationSchedule_TaskType)
                {
                    retval.PreservationScheduleTaskTypes.Add(tt.Convert(level + 1, deeplevel));
                }
            }

            return retval;
        }

        internal static PreservationRole Convert(this BibDSModel.PreservationRole rol, int level = 0, int deeplevel = 5)
        {
            if (rol == null || level > deeplevel)
                return null;

            var roles = new PreservationRole
            {
                KeyCode = rol.KeyCode,
                AlertEnabled = rol.AlertEnable,
                Enabled = rol.Enable,
                IdPreservationRole = rol.IdPreservationRole,
                Name = rol.Name,
                UserRoles = new BindingList<PreservationUserRole>(),
            };

            foreach (var usrRole in rol.PreservationUserRole)
            {
                roles.UserRoles.Add(usrRole.Convert(level + 1, deeplevel));
            }

            return roles;
        }

        internal static PreservationTask ConvertPreservationTask(this BibDSModel.PreservationTableValuedResult valuedResult)
        {
            if (valuedResult == null || valuedResult.PreservationTask_IdPreservationTask == null)
                return null;

            return new PreservationTask()
            {
                EndDocumentDate = valuedResult.PreservationTask_EndDocumentDate,
                EstimatedDate = valuedResult.PreservationTask_EstimatedDate.GetValueOrDefault(DateTime.MinValue),
                ExecutedDate = valuedResult.PreservationTask_ExecutedDate,
                IdPreservationTask = valuedResult.PreservationTask_IdPreservationTask.Value,
                StartDocumentDate = valuedResult.PreservationTask_StartDocumentDate,
                CorrelatedTasks = new BindingList<PreservationTask>(),
                HasError = valuedResult.PreservationTask_HasError.GetValueOrDefault(),
                ErrorMessages = valuedResult.PreservationTask_ErrorMessages,
                Executed = valuedResult.PreservationTask_Executed.GetValueOrDefault(),
                Enabled = valuedResult.PreservationTask_Enabled.GetValueOrDefault(),
                ActivationPin = valuedResult.PreservationTask_ActivationPin,
                IdPreservation = valuedResult.PreservationTask_IdPreservation,
                IdCorrelatedPreservationTask = valuedResult.PreservationTask_IdCorrelatedPreservationTask,
                LockDate = valuedResult.PreservationTask_LockDate
            };
        }

        internal static PreservationTask Convert(this BibDSModel.PreservationTask tsk, int level = 0, int deeplevel = 5)
        {
            if (tsk == null || level > deeplevel)
                return null;

            var retval = new PreservationTask
            {
                Archive = tsk.Archive.Convert(level + 1, deeplevel),
                EndDocumentDate = tsk.EndDocumentDate,
                EstimatedDate = tsk.EstimatedDate,
                ExecutedDate = tsk.ExecutedDate,
                IdPreservationTask = tsk.IdPreservationTask,
                StartDocumentDate = tsk.StartDocumentDate,
                TaskType = tsk.PreservationTaskType.Convert(level + 1, deeplevel),
                User = tsk.PreservationUser.Convert(level + 1, deeplevel),
                TaskGroup = tsk.PreservationTaskGroup.Convert(level + 1, deeplevel),
                Alerts = tsk.PreservationAlert.Convert(level, deeplevel),
                CorrelatedTasks = new BindingList<PreservationTask>(),
                HasError = tsk.HasError,
                ErrorMessages = tsk.ErrorMessages,
                Executed = tsk.Executed,
                Enabled = tsk.Enabled,
                ActivationPin = tsk.ActivationPin,
                IdPreservation = tsk.IdPreservation,
                IdCorrelatedPreservationTask = tsk.IdCorrelatedPreservationTask,
                LockDate = tsk.LockDate
            };

            if (tsk.PreservationTask1 != null && tsk.PreservationTask1.Any())
            {
                retval.CorrelatedTasks = tsk.PreservationTask1.Convert(level, deeplevel);
            }

            return retval;
        }

        internal static BindingList<PreservationTask> Convert(this EntityCollection<BibDSModel.PreservationTask> tsk, int level = 0, int deepLevel = 5)
        {
            var retval = new BindingList<PreservationTask>();

            if (tsk != null && level <= deepLevel)
            {
                foreach (var task in tsk)
                {
                    retval.Add(task.Convert(level, deepLevel));
                }
            }

            return retval;
        }

        internal static PreservationTaskType Convert(this BibDSModel.PreservationTaskType tsk, int level = 0, int deepLevel = 5)
        {
            if (tsk == null || level > deepLevel)
                return null;

            PreservationTaskTypes tipologia;
            try
            {
                tipologia = (PreservationTaskTypes)Enum.Parse(typeof(PreservationTaskTypes), tsk.KeyCode);
            }
            catch
            {
                tipologia = PreservationTaskTypes.Unknown;
            }

            var retval = new PreservationTaskType
            {
                Description = tsk.Description,
                Period = tsk.Period,
                IdPreservationTaskType = tsk.IdPreservationTaskType,
                Roles = new BindingList<PreservationRole>(),
                ScheduleTaskTypes = new BindingList<PreservationScheduleTaskType>(),
                Type = tipologia,
            };

            foreach (var item in tsk.PreservationTaskRole)
            {
                retval.Roles.Add(item.PreservationRole.Convert(level + 1, deepLevel));
            }

            foreach (var item in tsk.PreservationSchedule_TaskType)
            {
                retval.ScheduleTaskTypes.Add(item.Convert(level + 1, deepLevel));
            }


            return retval;
        }

        internal static BindingList<PreservationTaskType> Convert(this EntityCollection<BibDSModel.PreservationTaskType> tt, int level = 0, int deepLevel = 5)
        {
            var retval = new BindingList<PreservationTaskType>();

            if (tt != null && level <= deepLevel)
            {
                foreach (var tipo in tt)
                {
                    retval.Add(tipo.Convert(level, deepLevel));
                }
            }

            return retval;
        }

        internal static PreservationTaskGroup ConvertPreservationTaskGroup(this BibDSModel.PreservationTableValuedResult valuedResult)
        {
            if (valuedResult == null || valuedResult.PreservationTaskGroup_IdPreservationTaskGroup == null)
                return null;

            return new PreservationTaskGroup()
            {
                Closed = valuedResult.PreservationTaskGroup_Closed,
                EstimatedExpiry = valuedResult.PreservationTaskGroup_EstimatedExpiry,
                Expiry = valuedResult.PreservationTaskGroup_Expiry.GetValueOrDefault(DateTime.MinValue),
                IdPreservationTaskGroup = valuedResult.PreservationTaskGroup_IdPreservationTaskGroup.Value,
                IdPreservationTaskGroupType = valuedResult.PreservationTaskGroup_IdPreservationTaskGroupType.GetValueOrDefault(Guid.Empty),
                IdPreservationUser = valuedResult.PreservationTaskGroup_IdPreservationUser.GetValueOrDefault(Guid.Empty),
                Name = valuedResult.PreservationTaskGroup_Name,
            };
        }

        internal static PreservationTaskGroup Convert(this BibDSModel.PreservationTaskGroup grp, int level = 0, int deepLevel = 5)
        {
            if (grp == null || level > deepLevel)
                return null;

            var retval = new PreservationTaskGroup
            {
                Closed = grp.Closed,
                EstimatedExpiry = grp.EstimatedExpiry,
                Expiry = grp.Expiry,
                GroupType = grp.PreservationTaskGroupType.Convert(level + 1, deepLevel),
                IdPreservationSchedule = grp.IdPreservationSchedule,
                IdPreservationTaskGroup = grp.IdPreservationTaskGroup,
                IdPreservationTaskGroupType = grp.IdPreservationTaskGroupType,
                IdPreservationUser = grp.IdPreservationUser,
                Name = grp.Name,
                Schedule = grp.PreservationSchedule.Convert(level + 1, deepLevel),
                User = grp.PreservationUser.Convert(level + 1, deepLevel),
                Tasks = grp.PreservationTask.Convert(level, deepLevel),
            };

            return retval;
        }

        internal static PreservationTaskGroupType Convert(this BibDSModel.PreservationTaskGroupType typ, int level = 0, int deepLevel = 5)
        {
            if (typ == null || level > deepLevel)
                return null;

            return new PreservationTaskGroupType
            {
                Description = typ.Description,
                IdPreservationTaskGroupType = typ.IdPreservationTaskGroupType,
            };
        }

        internal static PreservationExceptionType Convert(this BibDSModel.PreservationExceptionType exx, int level = 0, int deeplevel = 5)
        {
            if (exx == null || level > deeplevel)
                return null;

            return new PreservationExceptionType
            {
                Description = exx.Description,
                IdPreservationExceptionType = exx.IdPreservationExceptionType,
                IsFail = exx.IsFail,
            };
        }

        internal static PreservationException Convert(this BibDSModel.PreservationException exx, int level = 0, int deeplevel = 5)
        {
            if (exx == null || level > deeplevel)
                return null;

            return new PreservationException
            {
                Description = exx.Description,
                ExceptionType = exx.PreservationExceptionType.Convert(level, deeplevel),
                IdPreservationException = exx.IdPreservationException,
                IdPreservationExceptionCorrelated = exx.IdPreservationExceptionCorrelated,
                IdPreservationExceptionType = exx.IdPreservationExceptionType,
                IsBlocked = exx.IsBlocked.HasValue ? exx.IsBlocked.Value : false,
            };
        }

        internal static PreservationHoliday Convert(this BibDSModel.PreservationHolidays hol, int level = 0, int deeplevel = 5)
        {
            if (hol == null || level > deeplevel)
                return null;

            return new PreservationHoliday
            {
                Description = hol.Description,
                HolidayDate = hol.HolidayDate,
                IdPreservationHolidays = hol.IdPreservationHolidays,
            };
        }

        internal static PreservationAlertType Convert(this BibDSModel.PreservationAlertType alr, int level = 0, int deeplevel = 5, params Type[] ignoredTypes)
        {
            if (alr == null || level > deeplevel)
                return null;

            var retval = new PreservationAlertType
            {
                AlertText = alr.AlertText,
                IdPreservationAlertType = alr.IdPreservationAlertType,
                Offset = alr.Offset,
                TaskTypes = new BindingList<PreservationTaskType>(),
                Role = alr.PreservationRole.Convert(level + 1, deeplevel)
            };

            if (ignoredTypes != null && !ignoredTypes.Contains(typeof(PreservationAlert)))
                retval.Alerts = alr.PreservationAlert.Convert(level + 1, deeplevel);
            if (alr.PreservationAlertTask != null)
            {
                foreach (var item in alr.PreservationAlertTask)
                {
                    retval.TaskTypes.Add(item.PreservationTaskType.Convert(level + 1, deeplevel));
                }
            }

            return retval;
        }

        internal static PreservationScheduleTaskType Convert(this BibDSModel.PreservationSchedule_TaskType stt, int level = 0, int deeplevel = 5)
        {
            if (stt == null || level > deeplevel)
                return null;

            return new PreservationScheduleTaskType
            {
                IdPreservationSchedule = stt.IdPreservationSchedule,
                IdPreservationTaskType = stt.IdPreservationTaskType,
                Offset = stt.Offset.HasValue ? stt.Offset.Value : (short)0,
                Schedule = stt.PreservationSchedule.Convert(level + 1, deeplevel),
                TaskType = stt.PreservationTaskType.Convert(level + 1, deeplevel),
            };
        }

        internal static PreservationAlert Convert(this BibDSModel.PreservationAlert alr, int level = 0, int deeplevel = 5)
        {
            if (alr == null || level > deeplevel)
                return null;

            return new PreservationAlert
            {
                AlertType = alr.PreservationAlertType.Convert(level + 1, deeplevel),
                AlertDate = alr.AlertDate,
                ForwardFrequency = alr.ForwardFrequency,
                IdPreservationAlert = alr.IdPreservationAlert,
                MadeDate = alr.MadeDate,
                Task = alr.PreservationTask.Convert(level + 1, deeplevel),
            };
        }

        internal static BindingList<PreservationAlert> Convert(this EntityCollection<BibDSModel.PreservationAlert> alr, int level = 0, int deeplevel = 5)
        {
            var retval = new BindingList<PreservationAlert>();

            if (alr != null && level <= deeplevel)
            {
                foreach (var alert in alr)
                {
                    retval.Add(alert.Convert(level, deeplevel));
                }
            }

            return retval;
        }

        internal static PreservationStorageDevice Convert(this BibDSModel.PreservationStorageDevice sto, int level = 0, int deeplevel = 5, params Type[] ignoredTypes)
        {
            if (sto == null || level > deeplevel)
                return null;

            var ret = new PreservationStorageDevice
            {
                DateCreated = sto.DateCreated,
                DateStorageDevice = sto.DateStorageDevice,
                IdPreservationStorageDevice = sto.IdPreservationStorageDevice,
                Label = sto.Label,
                LastVerifyDate = sto.LastVerifyDate,
                Location = sto.Location,
                //PreservationsInDevice = sto.PreservationInStorageDevice.Convert(level + 1, deeplevel, ignoredTypes),
                //Status = sto.PreservationStorageDeviceStatus.Convert(level + 1, deeplevel),
                User = new PreservationUser { DomainUser = sto.DomainUser },
                OriginalPreservationStorageDevice = (sto.PreservationStorageDevice2 != null) ? sto.PreservationStorageDevice2.Convert(level + 1, deeplevel, ignoredTypes) : null,
                MinDate = sto.MinDate,
                MaxDate = sto.MaxDate,
                EntratelCompleteFileName = sto.EntratelCompleteFileName,
                EntratelUploadDate = sto.EntratelUploadDate,
                Company = sto.Company,
            };

            var tipiIgnorati = ignoredTypes ?? new Type[0];

            if (!tipiIgnorati.Contains(typeof(PreservationInStorageDevice)))
                ret.PreservationsInDevice = sto.PreservationInStorageDevice.Convert(level + 1, deeplevel, ignoredTypes);

            if (!tipiIgnorati.Contains(typeof(PreservationStorageDeviceStatus)))
                ret.Status = sto.PreservationStorageDeviceStatus.Convert(level + 1, deeplevel);

            return ret;
        }

        internal static PreservationInStorageDevice Convert(this BibDSModel.PreservationInStorageDevice sto, int level = 0, int deeplevel = 5, params Type[] ignoredTypes)
        {
            if (sto == null || level > deeplevel)
                return null;

            var ret = new PreservationInStorageDevice
            {
                //Device = sto.PreservationStorageDevice.Convert(level + 1, deeplevel),
                Path = sto.Path,
                //Preservation = sto.Preservation.Convert(level + 1, deeplevel),
            };

            var tipiIgnorati = ignoredTypes ?? new Type[0];

            if (!tipiIgnorati.Contains(typeof(PreservationStorageDevice)))
                ret.Device = sto.PreservationStorageDevice.Convert(level + 1, deeplevel, ignoredTypes);

            if (!tipiIgnorati.Contains(typeof(Preservation)))
                ret.Preservation = sto.Preservation.Convert(level + 1, deeplevel, ignoredTypes);

            return ret;
        }

        internal static BindingList<PreservationInStorageDevice> Convert(this EntityCollection<BibDSModel.PreservationInStorageDevice> sto, int level = 0, int deeplevel = 5, params Type[] ignoredTypes)
        {
            var ret = new BindingList<PreservationInStorageDevice>();

            if (sto != null && level <= deeplevel)
            {
                foreach (var item in sto)
                {
                    ret.Add(item.Convert(level, deeplevel, ignoredTypes));
                }
            }

            return ret;
        }

        internal static PreservationStorageDeviceStatus Convert(this BibDSModel.PreservationStorageDeviceStatus sta, int level = 0, int deeplevel = 5)
        {
            if (sta == null || level > deeplevel)
                return null;

            return new PreservationStorageDeviceStatus
            {
                IdPreservationStorageDeviceStatus = sta.IdPreservationStorageDeviceStatus,
                KeyCode = sta.KeyCode,
                Value = sta.Value,
            };
        }

        internal static ArchiveCompany Convert(this BibDSModel.ArchiveCompany com, int level = 0, int deeplevel = 5)
        {

            if (com == null || level > deeplevel)
                return null;

            return new ArchiveCompany
            {
                CompanyName = (com.Company != null) ? com.Company.CompanyName : string.Empty,
                IdArchive = com.IdArchive,
                WorkingDir = com.WorkingDir,
                XmlFileTemplatePath = com.XmlFileTemplatePath,
                TemplateXSLTFile = com.TemplateXSLTFile,
                AwardBatchXSLTFile = com.AwardBatchXSLTFile,
                Company = (com.Company != null) ? com.Company.Convert(level + 1, deeplevel) : null,
                Archive = (com.Archive != null) ? com.Archive.Convert(level + 1, deeplevel) : null,
            };
        }

        internal static PreservationScheduleArchive Convert(this BibDSModel.PreservationScheduleArchive sch, int level = 0, int deeplevel = 5)
        {
            if (sch == null || level > deeplevel)
                return null;

            return new PreservationScheduleArchive
            {
                IdArchive = sch.IdArchive,
                IdSchedule = sch.IdPreservationSchedule,
                CreatedDate = sch.CreatedDate,
                ModifiedDate = sch.ModifiedDate,
                Schedule = (sch.PreservationSchedule != null) ? sch.PreservationSchedule.Convert(level + 1, deeplevel) : null,
                Archive = (sch.Archive != null) ? sch.Archive.Convert(level + 1, deeplevel) : null,
            };
        }

        #endregion

        #region OGGETTO BIBLOS -> MODELLO

        internal static BibDSModel.Archive ConvertPreservation(this DocumentArchive arch, BibDSModel.BiblosDS2010Entities db)
        {
            if (arch == null)
                return null;

            //var retval = (original == null) ? new BibDSModel.Archive() : original;

            //if (original == null)
            //{

            //    if (arch.Storage != null)
            //        retval.ArchiveStorage = db.ArchiveStorage.Where(x => x.IdStorage == arch.Storage.IdStorage);

            //    var attrs = db.Attributes.Where(x => x.IdArchive == arch.IdArchive);

            //    if (retval.Attributes == null && attrs.Count() > 0)
            //        retval.Attributes = new EntityCollection<BibDSModel.Attributes>();

            //    foreach (var i in attrs)
            //    {
            //        if (!retval.Document.Contains(i))
            //            retval.Attributes.Add(i);
            //    }

            //    var docs=db.Document.Where(x => x.IdArchive == arch.IdArchive);
            //    if (retval.Document == null && docs.Count() > 0)
            //        retval.Document = new EntityCollection<BibDSModel.Document>();

            //    foreach (var doc in docs)
            //    {
            //        if (!retval.Document.Contains(doc))
            //            retval.Document.Add(doc);
            //    }

            //    var pres = db.Preservation.Where(x => x.IdArchive == arch.IdArchive);

            //    if (retval.Preservation == null && pres.Count() > 0)
            //        retval.Preservation = new EntityCollection<BibDSModel.Preservation>();

            //    foreach (var p in pres)
            //    {
            //        if (!retval.Preservation.Contains(p))
            //            retval.Preservation.Add(p);
            //    }

            //    retval.IdArchive = arch.IdArchive;
            //}

            //retval.AuthorizationAssembly = arch.AuthorizationAssembly;
            //retval.AuthorizationClassName = arch.AuthorizationClassName;
            //retval.AutoVersion = arch.AutoVersion;
            //retval.EnableSecurity = arch.EnableSecurity;
            //retval.IsLegal = arch.IsLegal;
            ////retval.LastAutoIncValue = arch.las
            //retval.LastIdBiblos = arch.LastIdBiblos;
            //retval.LowerCache = arch.LowerCache;
            //retval.MaxCache = arch.MaxCache;
            //retval.Name = arch.Name;
            //retval.PathCache = arch.PathCache;
            //retval.PathPreservation = arch.PathPreservation;
            //retval.PathTransito = arch.PathTransito;
            //retval.UpperCache = arch.UpperCache;

            var retval = db.Archive.Where(x => x.IdArchive == arch.IdArchive).FirstOrDefault();

            return retval;
        }

        internal static BibDSModel.Preservation Convert(this Preservation pre, BibDSModel.BiblosDS2010Entities db, BibDSModel.Preservation original = null, int level = 0, int deeplevel = 5)
        {
            if (pre == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new BibDSModel.Preservation() : original;

            // retval.Archive = pre.Archive.ConvertPreservation(db);
            // retval.Archive = pre.Archive; 
            retval.CloseContent = pre.CloseContent;
            retval.CloseDate = pre.CloseDate;
            retval.IndexHash = pre.IndexHash;
            retval.Label = pre.Label;
            retval.LastVerifiedDate = pre.LastVerifiedDate;
            retval.Path = pre.Path;
            retval.PreservationDate = pre.PreservationDate;
            retval.IdPreservationUser = pre.IdPreservationUser;
            retval.IdPreservation = pre.IdPreservation;

            //var journaling = pre.PreservationJournalings.Convert(db, (original != null) ? original.PreservationJournaling : null, level, deeplevel);
            //foreach (var journal in journaling)
            //{
            //    retval.PreservationJournaling.Add(journal);
            //}

            return retval;
        }

        internal static BibDSModel.PreservationUser Convert(this PreservationUser usr, BibDSModel.BiblosDS2010Entities db, BibDSModel.PreservationUser original = null, int level = 0, int deeplevel = 5)
        {
            if (usr == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new BibDSModel.PreservationUser() : original;

            retval.Address = usr.Address;
            retval.DomainUser = usr.DomainUser;
            retval.Email = usr.EMail;
            retval.Enable = usr.Enabled;
            retval.FiscalId = usr.FiscalId;
            retval.Name = usr.Name;
            retval.Surname = usr.Surname;
            retval.PreservationUserRole = usr.UserRoles.Convert(db, null, level, deeplevel);

            return retval;
        }

        internal static BibDSModel.PreservationUserRole Convert(this PreservationUserRole rol, BibDSModel.BiblosDS2010Entities db, BibDSModel.PreservationUserRole original = null, int level = 0, int deeplevel = 5)
        {
            if (rol == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new BibDSModel.PreservationUserRole() : original;

            retval.Archive = rol.Archive.ConvertPreservation(db);

            retval.PreservationRole = rol.PreservationRole.Convert(db, retval.PreservationRole, level + 1, deeplevel);

            if (rol.PreservationUser != null && rol.PreservationUser.IdPreservationUser != Guid.Empty)
                retval.IdPreservationUser = rol.PreservationUser.IdPreservationUser;
            else if (rol.IdPreservationUser != Guid.Empty)
                retval.IdPreservationUser = rol.IdPreservationUser;

            if (rol.PreservationRole != null && rol.PreservationRole.IdPreservationRole != Guid.Empty)
                retval.IdPreservationRole = rol.PreservationRole.IdPreservationRole;
            else if (rol.IdPreservationRole != Guid.Empty)
                retval.IdPreservationRole = rol.IdPreservationRole;

            if (retval.IdPreservationUserRole == Guid.Empty)
                retval.IdPreservationUserRole = rol.IdPreservationUserRole;

            return retval;
        }

        internal static EntityCollection<BibDSModel.PreservationUserRole> Convert(this BindingList<PreservationUserRole> rol, BibDSModel.BiblosDS2010Entities db, EntityCollection<BibDSModel.PreservationUserRole> original = null, int level = 0, int deeplevel = 5)
        {
            if (rol == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new EntityCollection<BibDSModel.PreservationUserRole>() : original;

            foreach (var entity in rol)
            {
                retval.Add(entity.Convert(db, null, level, deeplevel));
            }

            return retval;
        }

        internal static BibDSModel.PreservationRole Convert(this PreservationRole rol, BibDSModel.BiblosDS2010Entities db, BibDSModel.PreservationRole original = null, int level = 0, int deeplevel = 5)
        {
            if (rol == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new BibDSModel.PreservationRole() : original;

            retval.KeyCode = (short)rol.KeyCode;
            retval.AlertEnable = rol.AlertEnabled;
            retval.Enable = rol.Enabled;
            retval.Name = rol.Name;

            if (rol.IdPreservationRole != Guid.Empty)
                retval.IdPreservationRole = rol.IdPreservationRole;

            return retval;
        }

        internal static BibDSModel.PreservationTaskType Convert(this PreservationTaskType typ, BibDSModel.BiblosDS2010Entities db, BibDSModel.PreservationTaskType original = null, int level = 0, int deeplevel = 5)
        {
            if (typ == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new BibDSModel.PreservationTaskType() : original;

            retval.IdPreservationTaskType = typ.IdPreservationTaskType;
            retval.Description = typ.Description;
            retval.Period = typ.Period;
            retval.KeyCode = ((int)typ.Type).ToString();

            return retval;
        }

        internal static EntityCollection<BibDSModel.PreservationTaskType> Convert(this BindingList<PreservationTaskType> typ, BibDSModel.BiblosDS2010Entities db, EntityCollection<BibDSModel.PreservationTaskType> original = null, int level = 0, int deeplevel = 5)
        {
            if (typ == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new EntityCollection<BibDSModel.PreservationTaskType>() : original;

            foreach (var entity in typ)
            {
                retval.Add(entity.Convert(db, null, level + 1, deeplevel));
            }

            return retval;
        }

        internal static BibDSModel.PreservationJournaling Convert(this PreservationJournaling jou, BibDSModel.BiblosDS2010Entities db, BibDSModel.PreservationJournaling original = null, int level = 0, int deeplevel = 5)
        {
            if (jou == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new BibDSModel.PreservationJournaling() : original;

            retval.IdPreservationJournaling = jou.IdPreservationJournaling;
            retval.IdPreservation = jou.IdPreservation;
            retval.IdPreservationJournalingActivity = jou.IdPreservationJournalingActivity;
            retval.IdPreservationUser = jou.User.IdPreservationUser;
            retval.Notes = jou.Notes;
            retval.DomainUser = jou.DomainUser;
            retval.DateActivity = jou.DateActivity;
            retval.DateCreated = jou.DateCreated;

            return retval;
        }

        internal static EntityCollection<BibDSModel.PreservationJournaling> Convert(this BindingList<PreservationJournaling> jou, BibDSModel.BiblosDS2010Entities db, EntityCollection<BibDSModel.PreservationJournaling> original = null, int level = 0, int deeplevel = 5)
        {
            if (jou == null || level > deeplevel)
                return null;

            var retval = (original == null) ? new EntityCollection<BibDSModel.PreservationJournaling>() : original;
            BibDSModel.PreservationJournaling journal = null;

            foreach (var entity in jou)
            {
                if (original != null)
                {
                    journal = original
                        .Where(x => x.IdPreservationJournaling == entity.IdPreservationJournaling)
                        .SingleOrDefault();
                }

                retval.Add(entity.Convert(db, journal, level + 1, deeplevel));
            }

            return retval;
        }

        #endregion
    }
}
