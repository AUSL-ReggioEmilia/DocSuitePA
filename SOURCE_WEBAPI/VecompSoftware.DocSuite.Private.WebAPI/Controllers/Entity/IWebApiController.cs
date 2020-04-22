using Microsoft.AspNet.OData.Query;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Service.Entity;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity
{
    public interface IWebApiController<TEntity, TService> : IHttpController, IDisposable
        where TEntity : DSWBaseEntity, new()
        where TService : IEntityBaseService<TEntity>
    {
        Task<IQueryable<TEntity>> GetAsync(ODataQueryOptions opts);

        Task<IHttpActionResult> PostAsync(TEntity entity);

        Task<IHttpActionResult> PutAsync(TEntity entity);

        Task<IHttpActionResult> PutAsync([FromBody]TEntity entity, UpdateActionType actionType);

        Task<IHttpActionResult> DeleteAsync([FromBody]TEntity entity, DeleteActionType actionType);

        Task<IHttpActionResult> DeleteAsync(TEntity entity);
    }
}
