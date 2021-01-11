using System;
using System.Diagnostics;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;

namespace VecompSoftware.DocSuiteWeb.Common.Helpers
{
    public class ObjectEventHelper
    {
        public static TraceEventType ConvertToTraceEvent(ObjectEventType objectEvent)
        {
            switch (objectEvent)
            {
                case ObjectEventType.Critical:
                    return TraceEventType.Critical;

                case ObjectEventType.Error:
                    return TraceEventType.Error;

                case ObjectEventType.Warning:
                    return TraceEventType.Warning;

                case ObjectEventType.Information:
                    return TraceEventType.Information;

                default:
                    return TraceEventType.Verbose;
            }
        }

        public static ObjectEventType ConvertToObjectEvent(TraceEventType traceEvent)
        {
            switch (traceEvent)
            {
                case TraceEventType.Critical:
                    return ObjectEventType.Critical;

                case TraceEventType.Error:
                    return ObjectEventType.Error;

                case TraceEventType.Warning:
                    return ObjectEventType.Warning;

                case TraceEventType.Information:
                    return ObjectEventType.Information;

                case TraceEventType.Verbose:
                    return ObjectEventType.Debug;

                default:
                    throw new ArgumentOutOfRangeException("traceEvent");
            }
        }
    }
}
