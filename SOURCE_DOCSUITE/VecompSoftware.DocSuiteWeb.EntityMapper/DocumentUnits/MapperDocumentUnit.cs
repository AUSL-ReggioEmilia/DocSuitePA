using NHibernate;
using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using DSW = VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.DocumentUnits
{
    public class MapperDocumentUnit : BaseEntityMapper<DSW.DocumentUnit, DocumentUnit>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public MapperDocumentUnit()
        {
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.DocumentUnit, DSW.DocumentUnit> MappingProjection(IQueryOver<DSW.DocumentUnit, DSW.DocumentUnit> queryOver)
        {
            throw new System.NotImplementedException();
        }

        protected override DocumentUnit TransformDTO(DSW.DocumentUnit entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare DocumentUnit se l'entità non è inizializzata");
            }

            DocumentUnit documentUnit = new DocumentUnit
            {
                DocumentUnitName = entity.DocumentUnitName,
                Environment = entity.Environment,
                Number = entity.Number,
                Status = (DocumentUnitStatus)entity.Status,
                Subject = entity.Subject,
                RegistrationDate = entity.RegistrationDate,
                RegistrationUser = entity.RegistrationUser,
                Year = entity.Year,
                LastChangedDate = entity.LastChangedDate,
                LastChangedUser = entity.LastChangedUser,
                UniqueId = entity.Id,
                Title = entity.Title,
            };

            if (entity.IdTenantAOO.HasValue)
            {
                documentUnit.TenantAOO = new Entity.Tenants.TenantAOO() { UniqueId = entity.IdTenantAOO.Value };
            }
            
            return documentUnit;
        }
        #endregion

    }
}
