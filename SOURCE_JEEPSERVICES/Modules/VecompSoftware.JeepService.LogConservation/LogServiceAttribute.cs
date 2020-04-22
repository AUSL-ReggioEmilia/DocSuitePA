using System;

namespace VecompSoftware.JeepService.LogConservation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LogServiceAttribute : Attribute
    {
        private readonly string _name;

        public LogServiceAttribute(string name)
        {
            _name = name;
        }

        public virtual string Name => _name;
    }
}
