﻿using System;
using System.Net.Http;
using VecompSoftware.DaoManager;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.WebAPIManager.Dao
{
    public interface IWebAPIDao<T> : IWebServiceDao<T, Guid>
    {
        WebAPIHelper Context { get; }
        void SetCustomAuthenticationInizializer(Func<ICredential, HttpClientHandler> authenticationInitializer);
    }
}
