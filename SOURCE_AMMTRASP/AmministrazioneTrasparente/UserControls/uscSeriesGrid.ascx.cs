using AmministrazioneTrasparente.Code;
using AmministrazioneTrasparente.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente.UserControls
{
    public partial class uscSeriesGrid : BaseSeriesGridControl
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public event EventHandler NeedDataSource = delegate { };
        public event EventHandler NeedItemCount = delegate { };
        #endregion

        #region [ Events ]
        protected void Page_Load(object sender, EventArgs e)
        {
            RadGrid grid;
            if (BaseMaster.HistoryEnable)
            {
                grid = DynamicColumnsInGrid ? DocumentSeriesRadGrid.NewGrid(GridStructure, StoricoEnabled) : DocumentSeriesRadGrid.NewGrid(null, StoricoEnabled);
            }
            else
            {
                grid = DynamicColumnsInGrid ? DocumentSeriesRadGrid.NewGrid(GridStructure) : DocumentSeriesRadGrid.NewGrid();
            }

            grid.ClientSettings.Scrolling.AllowScroll = true;
            grid.ClientSettings.Scrolling.UseStaticHeaders = false;

            grid.NeedDataSource += GridNeedDataSource;

            GridPlaceHolder.Controls.Add(grid);
            BaseMaster.AjaxManager.AjaxSettings.AddAjaxSetting(grid, grid, BaseMaster.AjaxLoadingPanel);
            if(!IsPostBack)
            {
                NeedItemCount(grid, e);
            }
        }
        #endregion

        #region [ Methods ]
        void GridNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RadGrid grid = sender as RadGrid;
            if (grid == null)
            {
                return;
            }

            NeedDataSource(sender, e);
        }
        #endregion        
    }
}