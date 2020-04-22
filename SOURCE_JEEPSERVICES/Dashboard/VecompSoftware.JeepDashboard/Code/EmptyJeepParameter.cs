using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepDashboard.Code
{
    public class EmptyJeepParameter : JeepParametersBase
    {
        [ReadOnly(true)]
        [Category("Error")]
        [Description("Messaggio inerente i parametri vuoti.")]
        public string Message { get; private set; }

        [ReadOnly(true)]
        [Category("Error")]
        [Description("Eccezione legata all'impossibilità di caricare i parametri.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public Exception Exception { get; private set; }

        [ReadOnly(true)]
        [Category("Error")]
        [Description("Nome della classe dei parametri.")]
        public String ParameterClass { get; private set; }
        
        #region [ Constructor ]

        public EmptyJeepParameter(string message)
        {
            Message = message;
        }

        public EmptyJeepParameter(string message, Exception exception, string className)
        {
            Message = message;
            Exception = exception;
            ParameterClass = className;
        }
        
        #endregion
    }

}

