using System;
using VecompSoftware.BPM.Integrations.Model.Configurations;

namespace VecompSoftware.BPM.Integrations.Model.Interfaces
{
    public interface IServiceWake
    {
        void AddModule(ModuleConfiguration module);
        void Start(Action executeAction, Action closeAction);
        void Stop();
    }
}
