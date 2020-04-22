using AmministrazioneTrasparente.Services;
using System;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente.Code
{
    public class BaseSeriesGridControl : BaseControl
    {
        #region [ Fields ]
        private readonly ParameterService _parameterService;
        #endregion

        #region [ Properties ]
        public bool StoricoEnabled { get; set; }
        public bool DynamicColumnsInGrid { get; set; }
        public ResultArchiveAttributeWSO GridStructure { get; set; }

        public bool ConstraintPanelCollapsedOnOpen
        {
            get { return this._parameterService.GetBoolean("ConstraintPanelCollapsedOnOpen"); }
        }
        #endregion

        #region [ Constructor ]
        public BaseSeriesGridControl()
            :base()
        {
            _parameterService = new ParameterService();
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}