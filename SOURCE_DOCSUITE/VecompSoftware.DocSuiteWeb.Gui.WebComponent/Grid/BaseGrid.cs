using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid.Template;
using VecompSoftware.Helpers;

namespace VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
{
    public class BaseGrid : RadGrid
    {
        public BaseGrid() : base()
        { }

        #region Fields
        protected const string FILTERID = "FILTERID";

        protected const int IMAGE_COLUMN_WIDTH = 24;
        private bool _enableClearFilterButton = true;
        private bool _enableExport = true;
        private bool _enablePaging = true;
        private bool _enableHeaderSection = true;
        private bool _enableScrolling = true;
        #endregion

        #region Properties

        public virtual int CustomPageIndex
        {
            get
            {
                return CurrentPageIndex;
            }
            set
            {
                CurrentPageIndex = value;
            }
        }

        public bool EnableClearFilterButton
        {
            get
            {
                return _enableClearFilterButton;
            }
            set 
            {
                _enableClearFilterButton = value;
            }
        }

        public int YetAnotherPageIndex
        {
            get
            {
                return CustomPageIndex + 1;
            }
        }

        public bool EnablePagingButtons 
        {
            get { return _enablePaging; }
            set { _enablePaging = value; }
        }
        public bool EnableExportButtons
        {
            get { return _enableExport; }
            set { _enableExport = value; }
        }

        public bool EnableHeaderSection
        {
            get { return _enableHeaderSection; }
            set { _enableHeaderSection = value; }
        }

        public virtual bool EnableScrolling
        {
            get { return _enableScrolling; }
            set { _enableScrolling = value; }
        }

        public virtual bool ImpersonateCurrentUser { get; set; }
        #endregion

        #region Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);


            ClientSettings.AllowGroupExpandCollapse = true;
            ClientSettings.AllowDragToGroup = true;

            ConfigureSorting();
            ConfigurePaging();
            ConfigureExport();
            ConfigureStyle();
            ConfigureResizing();
            if(EnableScrolling) ConfiguringScrolling();
        }

        /// <summary>
        /// Registra la risorsa javascript per la chiusura automatica del menu
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //intercetta le chiamate ajax. In caso di esportazione annulla la chiamata facendo il postback.
            RadAjaxManager.GetCurrent(Page).ClientEvents.OnRequestStart = "mngRequestStarted";

            RadGrid dg = (RadGrid)this;
            // impedisco che le colonne template con immagine vengano sovrapposte
            // impostando una larghezza minima
            foreach (GridColumn column in dg.Columns)
            {
                if (((column) is GridTemplateColumn))
                {
                    if (!string.IsNullOrEmpty(column.HeaderImageUrl))
                    {
                        column.HeaderStyle.Width = Unit.Pixel(IMAGE_COLUMN_WIDTH);
                        column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    }
                }
                else if (((column) is GridClientSelectColumn))
                {
                    column.HeaderStyle.Width = Unit.Pixel(IMAGE_COLUMN_WIDTH);
                    column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                }
            }
        }
        #endregion

        #region Methods

        private void ConfiguringScrolling()
        {
            ClientSettings.Scrolling.UseStaticHeaders = true;
            ClientSettings.Scrolling.AllowScroll = true;
            ClientSettings.Scrolling.ScrollHeight = Unit.Percentage(100);
            ClientSettings.Scrolling.SaveScrollPosition = false;
            Width = Unit.Percentage(100);
            Height = Unit.Percentage(100);
            MasterTableView.Width = Unit.Percentage(100);
        }

        private void ConfigureExport()
        {
            ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML;
            ExportSettings.Pdf.AllowPrinting = true;
            ExportSettings.Pdf.FontType = Telerik.Web.Apoc.Render.Pdf.FontType.Subset;
            ExportSettings.Pdf.PaperSize = GridPaperSize.A4;
            ExportSettings.FileName = HttpUtility.UrlEncode("Esportazione");
            ExportSettings.ExportOnlyData = false;
        }

        private void ConfigureSorting()
        {
            SortingSettings.SortedAscToolTip = "Ordine Crescente";
            SortingSettings.SortedDescToolTip = "Ordine Decrescente";
            SortingSettings.SortToolTip = "Ordina";
            MasterTableView.AllowMultiColumnSorting = false;
            MasterTableView.AllowCustomSorting = true;
            AllowSorting = true;
        }

        private void ConfigurePaging()
        {
            MasterTableView.PagerStyle.Visible = false;
            MasterTableView.PagerStyle.Position = GridPagerPosition.Top;
            AllowPaging = true;
            MasterTableView.AllowCustomPaging = true;
            MasterTableView.CommandItemTemplate = new DSCommandItemTemplate(EnablePagingButtons, EnableExportButtons, EnableClearFilterButton, EnableHeaderSection, this);
            MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
        }

        private void ConfigureStyle()
        {
            MasterTableView.ItemStyle.CssClass = "Scuro";
            MasterTableView.ItemStyle.Font.Name = "Verdana";
            MasterTableView.AlternatingItemStyle.CssClass = "Chiaro";
            MasterTableView.AlternatingItemStyle.Font.Name = "Verdana";
        }

        private void ConfigureResizing()
        {
            ClientSettings.Resizing.AllowColumnResize = true;
            ClientSettings.Resizing.EnableRealTimeResize = false;
            ClientSettings.Resizing.ClipCellContentOnResize = false;
            ClientSettings.Resizing.ResizeGridOnColumnResize = true;
            ClientSettings.ClientMessages.DragToResize = "Ridimensiona";
        }

        public void DoExport(GridCommandEventArgs e)
        {
            DoExport(e, base.Columns, base.Items);
        }

        public void DoExport(GridCommandEventArgs e, GridColumnCollection Columns, GridDataItemCollection Items)
        {
            ExportSettings.ExportOnlyData = true;
            ExportSettings.OpenInNewWindow = true;
            ExportSettings.Excel.Format = GridExcelExportFormat.Html;
            ExportSettings.Pdf.PaperSize = GridPaperSize.A4;
            ExportSettings.Pdf.PageHeight = Unit.Parse("210mm");
            ExportSettings.Pdf.PageWidth = Unit.Parse("297mm");
            ExportSettings.Pdf.AllowPrinting = true;

            switch (e.CommandArgument.ToString())
            {
                case "Excel":
                    Task.Factory.StartNew(() => AsyncExportExcel(Columns, Items)).Wait();
                    break;
                case "Word":
                    Task.Factory.StartNew(() => AsyncExportWord(Columns, Items)).Wait();
                    break;
                case "Pdf":
                    Task.Factory.StartNew(() => AsyncExportPdf()).Wait();
                    break;
            }
        }

        private void AsyncExportExcel(GridColumnCollection Columns, GridDataItemCollection Items)
        {
            Table table = ExportHelper.TableFromGrid(Columns, Items);
            ExportHelper.ExportToExcel(table, Page, string.Format("{0}{1}", ExportSettings.FileName, FileHelper.XLS));
        }

        private void AsyncExportWord(GridColumnCollection Columns, GridDataItemCollection Items)
        {
            Table table = ExportHelper.TableFromGrid(Columns, Items);
            ExportHelper.ExportToWord(table, Page, string.Format("{0}{1}", ExportSettings.FileName, FileHelper.DOC));
        }

        private void AsyncExportPdf()
        {
            MasterTableView.ExportToPdf();
        }
        #endregion
    }
}
