using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.WebAPI
{
    public class WebAPIDtoMapper<T> : BaseEntityMapper<T, WebAPIDto<T>>
    {
        public WebAPIDtoMapper()
            :base()
        {

        }

        public WebAPIDto<T> TransformDTO(T entity, TenantModel tenant)
        {
            if (entity == null)
            {
                throw new ArgumentException(string.Format("Impossibile trasformare {0} se l'entità non è inizializzata", nameof(T)));
            };
           
            WebAPIDto<T> model = new WebAPIDto<T>()
            {
                Entity = entity,
                TenantModel = tenant
            };

            return model;
        }

        protected override IQueryOver<T, T> MappingProjection(IQueryOver<T, T> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override WebAPIDto<T> TransformDTO(T entity)
        {
            return TransformDTO(entity);
        }
    }
}
