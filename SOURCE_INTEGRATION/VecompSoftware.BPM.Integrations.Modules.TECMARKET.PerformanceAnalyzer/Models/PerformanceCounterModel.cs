using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.PerformanceAnalyzer.Models
{
    public class PerformanceCounterModel
    {
        public PerformanceCounter Counter { get; set; }
        public int Threshold { get; set; }
    }
}
