using System;

namespace VecompSoftware.BPM.Integrations.Modules
{
    public interface IModule : IDisposable
    {
        string Identifier { get; }
        bool Cancel { get; }
        bool IsBusy { get; }
        void OnExecute();
        void Stop();
    }
}
