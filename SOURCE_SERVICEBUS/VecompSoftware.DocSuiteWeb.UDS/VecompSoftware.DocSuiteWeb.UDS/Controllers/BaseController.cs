using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuiteWeb.UDS.Controllers
{
    public abstract class BaseController<T> : ApiController
    {
        private readonly IDSWDataContext _dswDataContext;

        private readonly DomainUserModel _domainUserModel;

        public BaseController(IDSWDataContext dswDataContext, ISecurity security)
        {
            _dswDataContext = dswDataContext;
            _domainUserModel = security.GetCurrentUser() ?? new DomainUserModel() { Name = "anonymous", Domain = "system" };
        }

        internal abstract IQueryable<T> GetDataSet(IDSWDataContext dataUnitOfWork, DomainUserModel domainUserModel, bool applySecurity, bool onlyToRead);

        [AcceptVerbs("OPTIONS")]
        [AllowAnonymous]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<PageResult<T>> GetAsync(ODataQueryOptions<T> opts)
        {
            return await PrivateGetAsync(opts, true, true, false);
        }

        public async Task<PageResult<T>> GetAsync(ODataQueryOptions<T> opts, string applySecurity, string onlyToRead)
        {
            return await PrivateGetAsync(opts, applySecurity.Equals("1"), true, onlyToRead.Equals("1"));
        }

        [HttpGet]
        public async Task<PageResult<T>> GetAsync(ODataQueryOptions<T> opts, string applySecurity)
        {
            return await PrivateGetAsync(opts, applySecurity.Equals("1"), true, false);
        }

        [HttpGet]
        public async Task<PageResult<T>> GetAsync(ODataQueryOptions<T> opts, bool isSearch)
        {
            return await PrivateGetAsync(opts, false, false, false);
        }

        private async Task<PageResult<T>> PrivateGetAsync(ODataQueryOptions<T> opts, bool applySecurity, bool applyInclude, bool onlyToRead)
        {
            return await Task.Run(() => {
                IQueryable<T> genericDataSet = GetDataSet(_dswDataContext, _domainUserModel, applySecurity, onlyToRead);
                IQueryable results = opts.ApplyTo(genericDataSet);
                if (applyInclude)
                {
                    results = results
                              .Include("UDSRepository")
                              .Include("UDSRepository.Container")
                              .Include("Category")
                              .Include("Documents");
                }
                return new PageResult<T>(
                  results as IEnumerable<T>,
                  Request.ODataProperties().NextLink,
                  Request.ODataProperties().TotalCount);
            });
        }

    }
}