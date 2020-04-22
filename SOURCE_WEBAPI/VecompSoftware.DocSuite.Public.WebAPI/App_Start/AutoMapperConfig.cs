using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;
using VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains;
using VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Commons;
using VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.PECMails;
using VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Protocols;
using VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Resolutions;
using VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Workflows;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using Domains = VecompSoftware.DocSuite.Public.Core.Models.Domains;
using Entities = VecompSoftware.DocSuiteWeb.Entity;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI
{
    public static class AutoMapperConfig
    {
        public static IMapper RegisterMappings(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            AppDomain myDomain = Thread.GetDomain();

            //Get all Profiles
            IEnumerable<Profile> profiles = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t, unitOfWork, security));

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                foreach (Profile profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
                cfg.CreateMap<byte?, bool?>().ConvertUsing(new ByteToBooleanTypeConverter());
                cfg.CreateMap<Entities.Commons.ActiveType, bool>().ConvertUsing(new ActiveTypeTypeConverter());
                cfg.CreateMap<string, Domains.Commons.ContactType>().ConvertUsing(new ContactTypeTypeConverter());
                cfg.CreateMap<string, Domains.Protocols.ComunicationType>().ConvertUsing(new ComunicationTypeTypeConverter());
                cfg.CreateMap<short, Domains.Protocols.ProtocolStatusType>().ConvertUsing(new ProtocolStatusTypeConverter());
                cfg.CreateMap<Entities.Protocols.ProtocolType, Domains.Protocols.ProtocolType>().ConvertUsing(new ProtocolTypeTypeConverter());
                cfg.CreateMap<Entities.PECMails.PECType?, Domains.PECMails.PECType>().ConvertUsing(new PECTypeTypeConverter());
                cfg.CreateMap<byte, Domains.PECMails.PECActiveType>().ConvertUsing(new PECActiveTypeTypeConverter());
                cfg.CreateMap<ResolutionStatus, ResolutionStatusType>().ConvertUsing(new ResolutionStatusTypeConverter());
                cfg.CreateMap<byte, ResolutionType>().ConvertUsing(new ResolutionTypeConverter());
                cfg.CreateMap<Entities.Workflows.WorkflowStatus, WorkflowStatus>().ConvertUsing(new WorkflowStatusTypeConverter());
                cfg.CreateMap<ProtocolUserType, Domains.Protocols.ProtocolUserType>().ConvertUsing(new ProtocolUserTypeTypeConverter());
            });

            //Create a mapper that will be used by the DI container
            IMapper mapper = config.CreateMapper();
            return mapper;
            //Register the DI interfaces with their implementation
            //For<IMapperConfiguration>().Use(config);
            //For<IMapper>().Use(mapper);
        }
    }
}