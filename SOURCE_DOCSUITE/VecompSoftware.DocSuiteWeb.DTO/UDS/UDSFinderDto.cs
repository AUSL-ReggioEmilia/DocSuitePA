using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    public class UDSFinderDto
    {
        public Guid? IdRepository { get; set; }
        public IList<UDSFinderExpressionDto> Filters { get; set; }

        public UDSFinderDto()
        {
            this.Filters = new List<UDSFinderExpressionDto>();
        }
    }
}
