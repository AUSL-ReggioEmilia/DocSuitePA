using System;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.Service
{
    public interface IService<TContent, TResult> : IDisposable, IBaseService
        where TContent : class
        where TResult : class
    {
        TResult Create(TContent content);

        TResult Update(TContent content);

        bool Delete(TContent content);

        IQueryable<TContent> Queryable(bool optimization = false);
    }
}