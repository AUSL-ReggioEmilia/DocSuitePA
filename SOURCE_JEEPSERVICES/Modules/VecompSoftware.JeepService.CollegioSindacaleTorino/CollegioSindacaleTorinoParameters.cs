using System.ComponentModel;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.CollegioSindacaleTorino
{
    public class CollegioSindacaleTorinoParameters : JeepParametersBase
    {
        #region [ Properties ]

        /// <summary> Directory dove depositare gli zip generati. </summary>
        [DefaultValue("C:\\Temp\\PECOC")]
        [Description("Directory dove depositare gli zip generati.")]
        [Category("General")]
        public string ExtractFolder { get; set; }

        #endregion
    }
}
