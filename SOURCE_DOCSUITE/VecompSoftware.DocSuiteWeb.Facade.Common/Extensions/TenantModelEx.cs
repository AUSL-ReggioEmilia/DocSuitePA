using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
{
    public static class TenantModelEx
    {
        public static IReadOnlyCollection<TenantModel> GetActualTenants<T>(this IReadOnlyCollection<TenantModel> source)
        {
            string endpointName = typeof(T).Name;
            return source.Where(s => s.CurrentTenant || 
                    (!s.WebApiClientConfig.EndPoints.Any(x => x.EndpointName == endpointName) 
                    && s.Entities.Any(f => f.Key == endpointName && f.Value.IsActive))).ToList();
        }

        public static ICollection<WebAPIServiceDto> GetServiceDto(this TenantModel source)
        {
            ICollection<WebAPIServiceDto> dtos = new List<WebAPIServiceDto>();
            IBaseAddress address = null;
            foreach (IWebApiControllerEndpoint endpoint in source.WebApiClientConfig.EndPoints)
            {
                address = source.WebApiClientConfig.Addresses.Single(s => s.AddressName.Equals(endpoint.AddressName, StringComparison.InvariantCultureIgnoreCase));
                WebAPIServiceDto serviceDto = new WebAPIServiceDto()
                {
                    Name = endpoint.EndpointName,
                    WebAPIUrl = Path.Combine(address.Address.AbsoluteUri, endpoint.ControllerName)
                };
                dtos.Add(serviceDto);
            }

            foreach (KeyValuePair<string, TenantEntityConfiguration> entity in source.Entities.Where(x => x.Value.IsActive))
            {
                WebAPIServiceDto serviceDto = dtos.FirstOrDefault(x => x.Name.Equals(entity.Key, StringComparison.InvariantCultureIgnoreCase));
                if (serviceDto == null)
                    serviceDto = new WebAPIServiceDto()
                    {
                        Name = entity.Key
                    };

                serviceDto.ODATAUrl = Path.Combine(source.ODATAUrl, entity.Value.ODATAControllerName);
                if (!dtos.Any(x => x.Name.Equals(entity.Key, StringComparison.InvariantCultureIgnoreCase)))
                    dtos.Add(serviceDto);
            }

            return dtos;
        }
    }
}
