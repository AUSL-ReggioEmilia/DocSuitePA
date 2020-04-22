using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI
{
    public interface IFacadeWebAPIBase<T> : IFacadeBase<T, Guid, WebAPIDto<T>>
        where T : IDSWEntity
    {
        void Save(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
