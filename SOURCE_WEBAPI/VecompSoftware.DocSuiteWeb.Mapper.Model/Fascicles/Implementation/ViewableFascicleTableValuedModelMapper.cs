using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class ViewableFascicleTableValuedModelMapper : BaseModelMapper<ViewableFascicleTableValuedModel, FascicleModel>, IViewableFascicleTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public ViewableFascicleTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override FascicleModel Map(ViewableFascicleTableValuedModel entity, FascicleModel entityTransformed)
        {
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Title = entity.FascicleLinkLabel;

            return entityTransformed;
        }
    }
}
