using System;
using System.Configuration;
using System.Globalization;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente.Tools
{
    public static class DocumentSeriesRadGrid
    {
        #region [ Fields ]

        public const string DefaultControlId = "RadGridItems";

        #endregion
        
        #region [ Properties ]

        public static int ElementForPage
        {
            get
            {
                int elementForPage;
                return Int32.TryParse(ConfigurationManager.AppSettings["ElementForPage"], out elementForPage)
                    ? elementForPage
                    : 10;
            }
        }

        #endregion

        #region [ Constructors ]

        public static RadGrid NewGrid()
        {
            return NewGrid(null);
        }

        public static RadGrid NewGrid(ResultArchiveAttributeWSO structure, bool? historyEnable = null)
        {
            var grid = new RadGrid
            {
                ID = DefaultControlId,
                AutoGenerateColumns = false,
                AllowPaging = true,
                AllowCustomPaging = true,
                PageSize = ElementForPage,
                Culture = new CultureInfo("it-IT")
            };

            grid.PagerStyle.PagerTextFormat = "{4} Pagina {0} di {1}";
            grid.PagerStyle.Mode = GridPagerMode.NextPrev;

            grid.ItemDataBound += GridItemDataBound;

            grid.PagerStyle.Mode = GridPagerMode.NextPrev;
            grid.PagerStyle.Position = GridPagerPosition.Top;

            grid.MasterTableView.Width = Unit.Percentage(100);

            var clnDetails = new GridTemplateColumn
            {
                ItemTemplate =
                    new LinkButtonTemplate("ShowDetails", "lupa.png", "Dettaglio & Documenti", historyEnable)
            };

            clnDetails.HeaderStyle.Width = new Unit(1, UnitType.Pixel);
            grid.MasterTableView.Columns.Add(clnDetails);

            var clnPublishingDate = new GridBoundColumn
            {
                DataField = "PublishingDate",
                HeaderText = "Data pubblicazione",
                DataFormatString = "{0:dd/MM/yyyy}"
            };
            clnPublishingDate.HeaderStyle.Width = new Unit(90, UnitType.Pixel);
            grid.MasterTableView.Columns.Add(clnPublishingDate);

            if (!historyEnable.HasValue || !historyEnable.Value)
            {
                var clnLastChangedDate = new GridTemplateColumn()
                {
                    DataField = "LastChangedDate",
                    HeaderText = "Data ultima modifica",
                    ItemTemplate = new HtmlLastChangedDateTemplate()
                };
                clnLastChangedDate.HeaderStyle.Width = new Unit(90, UnitType.Pixel);
                grid.MasterTableView.Columns.Add(clnLastChangedDate);
            }
            else
            {
                var clnRetireDate = new GridBoundColumn()
                {
                    DataField = "RetireDate",
                    HeaderText = "Data di ritiro",
                    DataFormatString = "{0:dd/MM/yyyy}"
                };
                clnRetireDate.HeaderStyle.Width = new Unit(90, UnitType.Pixel);
                grid.MasterTableView.Columns.Add(clnRetireDate);
            }            

            var clnSubject = new GridTemplateColumn()
            {
                DataField = "Subject",
                HeaderText = "Oggetto",
                ItemTemplate = new HtmlSubjectTemplate()
            };
            grid.MasterTableView.Columns.Add(clnSubject);


            if (structure != null)
                foreach (var dynamicData in structure.ArchiveAttributes)
                {
                    var column = new GridTemplateColumn
                    {
                        ItemTemplate = new DynamicColumnTemplate(dynamicData.Name),
                        HeaderText = dynamicData.Description
                    };

                    grid.MasterTableView.Columns.Add(column);
                }

            return grid;
        }

        #endregion

        #region [ Events ]

        static void GridItemDataBound(object sender, GridItemEventArgs e)
        {

        }

        #endregion
    }
}