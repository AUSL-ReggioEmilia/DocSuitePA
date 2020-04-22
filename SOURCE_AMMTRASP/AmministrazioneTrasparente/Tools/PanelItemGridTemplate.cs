using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente.Tools
{
    public class PanelItemGridTemplate : ITemplate
    {
        private readonly bool _historyEnabled;
        private readonly bool _dynamicColumnsInGrid;
        private readonly bool _storicoEnabled;
        private readonly ResultArchiveAttributeWSO _gridStructure;

        public RadGrid Grid { get; set; }

        public PanelItemGridTemplate(bool historyEnabled, bool dynamicColumnsInGrid, bool storicoEnabled, ResultArchiveAttributeWSO gridStructure)
        {
            _historyEnabled = historyEnabled;
            _dynamicColumnsInGrid = dynamicColumnsInGrid;
            _storicoEnabled = storicoEnabled;
            _gridStructure = gridStructure;
        }

        public void InstantiateIn(Control container)
        {
            RadGrid grid;
            if (_historyEnabled)
            {
                grid = _dynamicColumnsInGrid ? DocumentSeriesRadGrid.NewGrid(_gridStructure, _storicoEnabled) : DocumentSeriesRadGrid.NewGrid(null, _storicoEnabled);
            }
            else
            {
                grid = _dynamicColumnsInGrid ? DocumentSeriesRadGrid.NewGrid(_gridStructure) : DocumentSeriesRadGrid.NewGrid();
            }

            grid.ClientSettings.Scrolling.AllowScroll = true;
            grid.ClientSettings.Scrolling.UseStaticHeaders = false;

            Grid = grid;

            container.Controls.Add(grid);
        }
    }
}