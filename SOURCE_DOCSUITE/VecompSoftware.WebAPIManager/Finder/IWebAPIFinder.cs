using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;

namespace VecompSoftware.WebAPIManager.Finder
{
    public interface IWebAPIFinder<T, THeader> : IFinder<T>
    {
        bool EnableTableJoin { get; set; }
        bool EnablePaging { get; set; }
        bool EnableTopOdata { get; set; }
        Guid? UniqueId { get; set; }
        ICollection<WebAPIDto<THeader>> DoSearchHeader();
        new ICollection<WebAPIDto<T>> DoSearch();

        void ResetDecoration();

    }
}
