using System.Diagnostics;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.PerformanceAnalyzer.Models
{
    public class PerformanceCounterModel
    {
        public PerformanceCounter Counter { get; set; }
        public int Threshold { get; set; }
    }
}
