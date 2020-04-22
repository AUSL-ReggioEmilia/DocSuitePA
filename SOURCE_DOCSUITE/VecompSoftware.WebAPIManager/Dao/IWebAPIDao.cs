using System;
using VecompSoftware.DaoManager;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.WebAPIManager.Dao
{
    public interface IWebAPIDao<T> : IWebServiceDao<T, Guid>
    {
        WebAPIHelper Context { get; }

    }
}
