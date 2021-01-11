using System;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityRepositoryDto
    {
        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public int DSWEnvironment { get; set; }

        public string Alias { get; set; }

        public UDSEntityContainerDto Container { get; set; }
    }
}
