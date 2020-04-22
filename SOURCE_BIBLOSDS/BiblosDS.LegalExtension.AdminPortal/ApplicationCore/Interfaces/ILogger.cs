using System;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces
{
    public interface ILogger
    {
        void Debug(string message);
        void Debug(string message, Exception exception);
        void Error(string message);
        void Error(string message, Exception exception);
        void Info(string message);
        void Warn(string message);
        void Warn(string message, Exception exception);
    }
}
