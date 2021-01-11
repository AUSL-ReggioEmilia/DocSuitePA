using System;

namespace VecompSoftware.JeepService.Common
{
    public class InfiniteMarshalByRefObject : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
