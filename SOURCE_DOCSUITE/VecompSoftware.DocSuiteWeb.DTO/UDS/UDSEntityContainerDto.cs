using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityContainerDto
    {
        public UDSEntityContainerDto()
        {
            UDSRepositories = new List<UDSEntityRepositoryDto>();
        }

        public int EntityShortId { get; set; }

        public string Name { get; set; }

        public IList<UDSEntityRepositoryDto> UDSRepositories { get; set; }
        
    }
}
