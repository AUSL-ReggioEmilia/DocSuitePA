using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.UDS
{
    public enum UDSFieldListStatus : short
    {
        Invalid = 0,
        Active = 1,
        Inactive = Active * 2
    }
}
