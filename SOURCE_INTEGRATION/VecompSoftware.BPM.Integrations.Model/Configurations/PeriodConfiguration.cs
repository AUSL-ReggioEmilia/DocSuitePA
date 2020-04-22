using System;

namespace VecompSoftware.BPM.Integrations.Model.Configurations
{
    public class PeriodConfiguration
    {
        /// <summary>
        /// Timer name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Timer interval
        /// </summary>
        public TimeSpan Period { get; set; }

        /// <summary>
        /// Timer start
        /// </summary>
        public TimeSpan StartHour { get; set; }

        /// <summary>
        /// Timer end
        /// </summary>
        public TimeSpan EndHour { get; set; }

    }
}
