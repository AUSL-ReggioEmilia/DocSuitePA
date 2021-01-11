using AmministrazioneTrasparente.Code;
using AmministrazioneTrasparente.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace AmministrazioneTrasparente.UserControls
{
    public partial class uscSeriesGridConstraints : BaseSeriesGridControl
    {
        #region [ Fields ]
        private const string NO_CONSTRAINT_TITLE = "Altri documenti";
        private const string PANEL_TOOLTIP = "Clicca per visualizzare o nascondere la lista dei risultati";
        public const string GRID_CONSTRAINT_ATTRIBUTE = "Constraint";
        #endregion

        #region [ Properties ]
        public ICollection<string> Constraints
        {
            get
            {
                if (ViewState["Constraints"] == null)
                {
                    return new List<string>();
                }
                return (ICollection<string>)ViewState["Constraints"];
            }
            set
            {
                ViewState["Constraints"] = value;
            }
        }        

        public event EventHandler NeedDataSource = delegate { };
        public event EventHandler NeedItemCount = delegate { };
        #endregion

        #region [ Events ]
        protected void Page_Load(object sender, EventArgs e)
        {            
            RadPanelItem panelItem;
            RadPanelItem contentPanelItem;
            foreach (string constraint in Constraints)
            {
                string tmpConstraint = constraint;
                if (string.IsNullOrEmpty(tmpConstraint))
                {
                    tmpConstraint = NO_CONSTRAINT_TITLE;
                }

                PanelItemGridTemplate itemGridTemplate = new PanelItemGridTemplate(BaseMaster.HistoryEnable, DynamicColumnsInGrid, StoricoEnabled, GridStructure);
                panelItem = rpbConstraints.FindItemByValue(tmpConstraint);
                if (panelItem == null)
                {
                    panelItem = new RadPanelItem(tmpConstraint)
                    {
                        Value = tmpConstraint,
                        Expanded = !ConstraintPanelCollapsedOnOpen,
                        ToolTip = PANEL_TOOLTIP
                    };

                    contentPanelItem = new RadPanelItem()
                    {
                        Expanded = true
                    };
                    panelItem.Items.Add(contentPanelItem);
                }

                contentPanelItem = panelItem.Items[0];
                itemGridTemplate.InstantiateIn(contentPanelItem);

                RadGrid grid = contentPanelItem.FindControl(itemGridTemplate.Grid.ID) as RadGrid;
                grid.Attributes.Add(GRID_CONSTRAINT_ATTRIBUTE, constraint);
                grid.NeedDataSource += GridNeedDataSource;

                if (rpbConstraints.FindItemByValue(tmpConstraint) == null)
                {
                    rpbConstraints.Items.Add(panelItem);
                }
                
                if (!IsPostBack)
                {
                    NeedItemCount(grid, e);
                    panelItem.Text = string.Concat(tmpConstraint, " (", grid.VirtualItemCount.ToString(), grid.VirtualItemCount == 1 ? " elemento" : " elementi", " da visualizzare)");
                }
                
                BaseMaster.AjaxManager.AjaxSettings.AddAjaxSetting(grid, grid, BaseMaster.AjaxLoadingPanel);
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