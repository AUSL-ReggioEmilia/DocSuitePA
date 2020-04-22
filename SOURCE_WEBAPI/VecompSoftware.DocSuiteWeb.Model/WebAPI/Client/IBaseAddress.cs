using System;

namespace VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
{
    public interface IBaseAddress
    {
        string AddressName { get; set; }

        Uri Address { get; set; }

        ICredential NetworkCredential { get; set; }

        TimeSpan? Timeout { get; set; }
    }
}
