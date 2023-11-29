using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public enum UDSFieldListStatus : short
    {
        Invalid = 0,
        Active = 1,
        Inactive = Active * 2
    }
}
