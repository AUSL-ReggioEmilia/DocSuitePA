using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Commons
{
    public class ActiveTypeTypeConverter : ITypeConverter<ActiveType, bool>
    {
        public bool Convert(ActiveType source, bool destination, ResolutionContext context)
        {
            return source == ActiveType.Active;
        }
    }
}