using System;
using System.Net.Http;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;

namespace VecompSoftware.WebAPIManager.Finder
{
    public interface IImpersonateWebAPIFinder
    {
        void SetCustomAuthenticationInizializer(Func<ICredential, HttpClientHandler> authenticationInitializer);
    }
}
