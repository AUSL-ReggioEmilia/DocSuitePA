using System;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess.Models
{
    public class EventLogModel
    {
        #region [ Fields ]

        private DateTime _logDate;

        #endregion

        #region [ Properties ]
        public DateTime LogDateTime 
        {
            get
            {
                if (_logDate == null)
                {
                    _logDate = Convert.ToDateTime(LogDate);
                }
                return _logDate;
            }
        }

        public string LogDate { get; set; }
        public string LogType { get; set; }
        public string LogSource { get; set; }
        public string LogDescription { get; set; }

        #endregion

    }
}
