using System.Collections.Generic;

namespace JeepService.Classes
{
    class Spool
    {
        private Dictionary<string, TimerWork> _timers;
        public Dictionary<string, TimerWork> Timers
        {
            get { return _timers ?? (_timers = new Dictionary<string, TimerWork>()); }
            set { _timers = value; }
        }

        private readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        public Spool(string id)
        {
            _id = id;
        }
    }
}
