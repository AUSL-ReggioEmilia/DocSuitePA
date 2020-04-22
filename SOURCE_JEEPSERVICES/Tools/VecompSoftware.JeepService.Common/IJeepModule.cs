using System;
using System.Collections.Generic;

namespace VecompSoftware.JeepService.Common
{
    public interface IJeepModule
    {
        #region "Fields"

        #endregion

        #region "Properties"

        string Name { get; set; }
        bool Cancel { get; set; }
        bool Enabled { get; set; }
        bool IsBusy { get; }
        DateTime? LastExecution { get; }
        DateTime? NextExecution { get; set; }
        string Version { get; set; }

        #endregion

        #region "Events"

        event MessageEventArgs.MessageEventHandler Message;

        #endregion

        #region "Methods"

        void Initialize(List<Parameter> parameters);

        void ReceiveMessage(string customer, int version, String message);

        void DoWork();

        #endregion
        
    }
}
