using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace VecompSoftware.DocSuite.Private.WebAPI.AssemblyResolvers
{
    public class UDSAssemblyResolver : IAssembliesResolver
    {
        public ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            if (File.Exists(WebApiApplication.UDSAssemblyFileName) && !baseAssemblies.Any(f => f.FullName == WebApiApplication.UDSAssemblyFullName))
            {
                Assembly udsAssembly = Assembly.LoadFrom(WebApiApplication.UDSAssemblyFileName);
                baseAssemblies.Add(udsAssembly);
            }
            return baseAssemblies;
        }
    }
}