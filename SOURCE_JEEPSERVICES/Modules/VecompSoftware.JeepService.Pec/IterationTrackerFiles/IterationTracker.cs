using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.Pec.IterationTrackerFiles
{
    public class IterationTracker
    {
        private readonly List<IterationDescriptor> _iterations;
        public IterationTracker()
        {
            _iterations = new List<IterationDescriptor>();
        }

        public IterationDescriptor AddIteration()
        {
            IterationDescriptor newItem = new IterationDescriptor { IterationIndex = _iterations.Count };
            _iterations.Add(newItem);
            return newItem;
        }

        public void Log(string loggerName)
        {
            FileLogger.Info(loggerName, "**** Logging Iteration Tracker acquired information ****");
            FileLogger.Info(loggerName, $"**** Number of attempts: {_iterations.Count} ****");

            LogFailedIterations(loggerName);
            LogSuccededIteration(loggerName);

            FileLogger.Info(loggerName, "********************************************************");
        }

        private void LogFailedIterations(string loggerName)
        {
            foreach (IterationDescriptor iterationDescriptor in _iterations.Where(x => x.Message.Length > 0).OrderBy(x => x.IterationIndex))
            {
                FileLogger.Info(loggerName, $"Attempt {iterationDescriptor.IterationIndex + 1} failed on status: {iterationDescriptor.Status} {Environment.NewLine}" +
                                            $"Reason: {iterationDescriptor.Message} {Environment.NewLine}");
            }
        }

        private void LogSuccededIteration(string loggerName)
        {
            foreach (IterationDescriptor iterationDescriptor in _iterations.Where(x => x.Message.Length == 0))
            {
                FileLogger.Info(loggerName, $"Attempt {iterationDescriptor.IterationIndex + 1} succeded {Environment.NewLine}");
            }
        }
    }
}
