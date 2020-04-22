using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APISeriesItem = VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using NHibernate;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.DocumentSeries
{
    public class MapperDocumentSeriesEntity : BaseEntityMapper<DSW.DocumentSeries, APISeriesItem.DocumentSeries>
    {
        #region [ Fields ]
        private readonly MapperContainerEntity _mapperContainerEntity;
        #endregion

        #region [ Constructor ]
        public MapperDocumentSeriesEntity()
        {
            _mapperContainerEntity = new MapperContainerEntity();
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<DSW.DocumentSeries, DSW.DocumentSeries> MappingProjection(IQueryOver<DSW.DocumentSeries, DSW.DocumentSeries> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APISeriesItem.DocumentSeries TransformDTO(DSW.DocumentSeries entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare DocumentSeries se l'entità non è inizializzata");

            APISeriesItem.DocumentSeries apiDocumentSeries = new APISeriesItem.DocumentSeries();
            apiDocumentSeries.EntityId = entity.Id;
            apiDocumentSeries.PublicationEnabled = entity.PublicationEnabled.Value;
            apiDocumentSeries.SubsectionEnabled = entity.SubsectionEnabled;
            apiDocumentSeries.IdDocumentSeriesFamily = entity.Family.Id;
            apiDocumentSeries.RoleEnabled = entity.RoleEnabled;
            apiDocumentSeries.SortOrder = entity.SortOrder;
            apiDocumentSeries.AllowAddDocument = entity.AllowAddDocument;
            apiDocumentSeries.AllowNoDocument = entity.AllowNoDocument;
            apiDocumentSeries.RegistrationDate = entity.RegistrationDate;
            apiDocumentSeries.RegistrationUser = entity.RegistrationUser;
            apiDocumentSeries.LastChangedDate = entity.LastChangedDate;
            apiDocumentSeries.LastChangedUser = entity.LastChangedUser;
            apiDocumentSeries.Container = _mapperContainerEntity.MappingDTO(entity.Container);
            apiDocumentSeries.RegistrationUser = entity.RegistrationUser;
            apiDocumentSeries.RegistrationDate = entity.RegistrationDate;
            apiDocumentSeries.Name = entity.Name;

            return apiDocumentSeries;
        }
        #endregion
    }
}
