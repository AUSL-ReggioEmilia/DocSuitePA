using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VecompSoftware.JeepService.Pec
{
    public class StatisticLog
    {
        public TimeSpan ElaboratedTime { get; set; }

        public int PECReaded { get; set; }

        public int PECError { get; set; }

        public int PECDone { get; set; }
    }
}
