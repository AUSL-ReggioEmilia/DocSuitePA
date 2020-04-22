using System;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Monitors
{
    public class TransparentAdministrationMonitorLogModelMapper : BaseModelMapper<TransparentAdministrationMonitorLog, TransparentAdministrationMonitorLogModel>, ITransparentAdministrationMonitorLogModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public TransparentAdministrationMonitorLogModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #region [ Methods ]

        public override TransparentAdministrationMonitorLogModel Map(TransparentAdministrationMonitorLog entity, TransparentAdministrationMonitorLogModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Date = entity.Date;
            modelTransformed.Note = entity.Note;
            modelTransformed.Rating = entity.Rating;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.IdDocumentUnit = Guid.Empty;
            modelTransformed.DocumentUnitName = string.Empty;
            modelTransformed.DocumentUnitTitle = string.Empty;
            if (entity.DocumentUnit != null)
            {
                modelTransformed.IdDocumentUnit = entity.DocumentUnit.UniqueId;
                modelTransformed.DocumentUnitName = entity.DocumentUnit.DocumentUnitName;
                modelTransformed.DocumentUnitTitle = entity.DocumentUnit.Title;
            }
            modelTransformed.IdRole = entity.Role == null ? null : (short?)entity.Role.EntityShortId;
            modelTransformed.RoleName = entity.Role == null ? string.Empty : entity.Role.Name;
            return modelTransformed;
        }
        #endregion

    }
}