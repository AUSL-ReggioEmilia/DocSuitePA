using System;
using VecompSoftware.JeepService.Common;

namespace JeepService.Classes
{
    public class TimerWork : InfiniteMarshalByRefObject
    {
        public TimerType Type { get; private set; }

        public string Id { get; set; }

        public int Duetime { get; set; }

        public int Period { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public TimerWork(TimerType t)
        {
            Type = t;
        }

        public override string ToString()
        {
            return String.Format("{0} - tipo {1}",Id, Type);
        }
    }
}
