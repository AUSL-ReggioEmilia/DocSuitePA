using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid.Template
{
    public class DSCommandItemTemplate : ITemplate
    {
        #region Fields
        private int _pageCount = 0;
        
        private bool _enableExport = true;
        private bool _enableFilter = true;
        private bool _enablePaging = true;
        private bool _enableHeaderSection = true;
        
        protected RadButton firstImage;        
        protected RadButton prevImage;        
        protected RadButton nextImage;        
        protected RadButton lastImage;
        
        protected RadTextBox txtCurrentPage;
        protected RangeValidator rangeValidator;
        protected RadButton currentButton;
        
        protected RadButton clearFilterButton;
        
        protected RadButton excelButton; 
        protected RadButton excelFButton;
        protected RadButton wordButton;
        protected RadButton wordFButton; 
        protected RadButton pdfButton;

        private BaseGrid _grid;
        #endregion

        #region Properties
        public bool EnablePagingButtons 
        {
            get
            {
                return _enablePaging;
            }
            set 
            {
                _enablePaging = value;
            }
        }

        public bool EnableExportButtons
        {
            get
            {
                return _enableExport;
            }
            set
            {
                _enableExport = value;
            }
        }

        public bool EnableFilterButtons
        {
            get
            {
                return _enableFilter;
            }
            set
            {
                _enableFilter = value;
            }
        }
        /// <summary>
        /// Nasconde l'intestazione della griglia.
        /// </summary>
        public bool EnableHeaderSection
        {
            get
            {
                return _enableHeaderSection;
            }
            set
            {
                _enableHeaderSection = value;
            }
        }
        #endregion

        #region Constructors
        public DSCommandItemTemplate(bool enablePaging, bool enableExport, bool enableFilter, bool enableHeader, BaseGrid grid)
        {
            EnablePagingButtons = enablePaging;
            EnableExportButtons = enableExport;
            EnableFilterButtons = enableFilter;
            EnableHeaderSection = enableHeader;
            _grid = grid;
        }
        #endregion

        #region Methods
        private void EnableCustomPageButton(bool enable)
        {
            txtCurrentPage.ReadOnly = !enable;
            currentButton.Visible = enable;
            rangeValidator.Enabled = enable;
        }

        /// <summary>
        /// Crea controlli di paginazione
        /// </summary>
        /// <param name="container">TD.rgCommandCell</param>
        public void InstantiateIn(Control container)
        {
            if (EnableHeaderSection)
            {
                // Contenitore dei bottoni di navigazione (FIRST, PREV, NEXT, LAST) e della stringa Visualizzati N di M risultati.
                HtmlGenericControl commandItemTable = new HtmlGenericControl("div");
                commandItemTable.Attributes.Add("class", "commandItemTable navigationButtons");
                container.Controls.Add(commandItemTable);

                if (EnablePagingButtons)
                {
                    //carica informazioni paginazione
                    _pageCount = _grid.PageCount;

                    // Navigation button
                    firstImage = CreateActionButton(RadButtonType.LinkButton, "firstImage", "Page", "First", false, "Prima", "Vai alla prima pagina", "firstPage", true);
                    prevImage = CreateActionButton(RadButtonType.LinkButton, "prevImage", "Page", "Prev", false, "Precedente", "Vai alla pagina precedente", "prevPage", true);
                    nextImage = CreateActionButton(RadButtonType.LinkButton, "nextImage", "Page", "Next", false, "Successiva", "Vai alla pagina successiva", "nextPage", true);
                    lastImage = CreateActionButton(RadButtonType.LinkButton, "lastImage", "Page", "Last", false, "Ultima", "Vai all'ultima pagina", "lastPage", true);

                    commandItemTable.Controls.Add(firstImage);
                    commandItemTable.Controls.Add(prevImage);
                
                    // Vai a Pagina
                    commandItemTable.Controls.Add(CreatePanelSetPage());
                    container.Page.Session.Add("PageID", container.UniqueID.Substring(0, container.UniqueID.Length - 5) + txtCurrentPage.UniqueID);
                    container.Page.Session.Add("TargetID", container.UniqueID.Substring(0, container.UniqueID.Length - 5) + currentButton.UniqueID);

                    commandItemTable.Controls.Add(nextImage);
                    commandItemTable.Controls.Add(lastImage);
                
                    //Abilita/disabilita pulsanti
                    SetStatePageButtons();
                }

                ICollection dataSource = _grid.DataSource as ICollection;

                if (dataSource != null)
                {
                    // Espone il numero di elementi visualizzati nella pagina.
                    if (dataSource.Count >= 0 && dataSource.Count <= _grid.VirtualItemCount)
                    {
                        int current = dataSource.Count;
                        if (current >= (_grid.PageSize * (_grid.CustomPageIndex + 1)))
                            current = (_grid.PageSize * (_grid.CustomPageIndex + 1));

                        Label lit = new Label();
                        lit.Text = String.Format("Visualizzati {0} su {1} risultati", current, _grid.VirtualItemCount);
                        lit.Attributes.Add("class", "visualizzaRisultati");

                        commandItemTable.Controls.Add(lit);
                    }
                }
            
                if (EnableExportButtons)
                {
                    HtmlGenericControl rightButtons = new HtmlGenericControl("div");
                    rightButtons.Attributes.Add("class", "commandItemButtons exportButtons");

                    if (FilteringByColumnIsActive())
                    {
                        clearFilterButton = CreateActionButton(RadButtonType.LinkButton, "clearFilter", "ClearFilters", null, false, "Pulisci", "Pulisci Filtri", "pulisciFiltri");
                        rightButtons.Controls.Add(clearFilterButton);
                    }

                    excelButton = CreateActionButton(RadButtonType.LinkButton, "excelButton", "Export", "Excel", false, "Esporta pagina", "Esporta la pagina corrente della ricerca in Excel", "excelButtonExport", false);
                    excelFButton = CreateActionButton(RadButtonType.LinkButton, "excelFButton", "ExportFull", "Excel", false, "Esporta tutto", "Esporta tutto il risultato della ricerca in Excel", "excelButtonExport", false);
                    wordButton = CreateActionButton(RadButtonType.LinkButton, "wordButton", "Export", "Word", false, "Esporta pagina", "Esporta la pagina corrente della ricerca in Word", "wordButtonExport", false);
                    wordFButton = CreateActionButton(RadButtonType.LinkButton, "wordFButton", "Export", "Word", false, "Esporta tutto", "Esporta tutto il risultato della ricerca in Word", "wordButtonExport", false);
                    rightButtons.Controls.Add(excelButton);
                    rightButtons.Controls.Add(excelFButton);
                    rightButtons.Controls.Add(wordButton);
                    rightButtons.Controls.Add(wordFButton);

                    container.Controls.Add(rightButtons);
                }
            }            
        }

        private void SetStatePageButtons()
        {
            if(_pageCount == 1)
            {
                firstImage.Enabled = false;
                prevImage.Enabled = false;
                lastImage.Enabled = false;
                nextImage.Enabled = false;
                EnableCustomPageButton(true);                
            }
            else if (_grid.YetAnotherPageIndex == _pageCount)
                 {
                     firstImage.Enabled = true;
                     prevImage.Enabled = true;
                     lastImage.Enabled = false;
                     nextImage.Enabled = false;
                     EnableCustomPageButton(true);                     
                 }
                else if (_grid.YetAnotherPageIndex == 1)
                     {
                         firstImage.Enabled = false;
                         prevImage.Enabled = false;
                         lastImage.Enabled = true;
                         nextImage.Enabled = true;
                         EnableCustomPageButton(true);                         
                     }
                    else
                    {
                        firstImage.Enabled = true;
                        prevImage.Enabled = true;
                        lastImage.Enabled = true;
                        nextImage.Enabled = true;
                        EnableCustomPageButton(true);
                    }
        }

        private Panel CreatePanelSetPage()
        {
            txtCurrentPage = new RadTextBox() { ID = "txtCurrentPage", Width = Unit.Pixel(50), Text = _grid.YetAnotherPageIndex.ToString() };
            txtCurrentPage.Style.Add("text-align", "center");

            rangeValidator = new RangeValidator()
            {
                ID = "rangeValidator1",
                ControlToValidate = txtCurrentPage.ID,
                EnableClientScript = true,
                MinimumValue = "1",
                MaximumValue = _pageCount.ToString(),
                Display = ValidatorDisplay.Dynamic,
                Type = ValidationDataType.Integer                
            };
            rangeValidator.ErrorMessage = string.Format("Valore non compreso tra {0} - {1}", rangeValidator.MinimumValue, rangeValidator.MaximumValue);

            currentButton = new RadButton() { ID = "currentButton", Text = "Vai", CausesValidation = false, CommandName = "CustomChangePage" };

            Panel panel = new Panel();
            panel.Controls.Add(new LiteralControl("Pagina: "));
            panel.Controls.Add(txtCurrentPage);
            panel.Controls.Add(new LiteralControl(string.Format(" di {0} &nbsp;", _pageCount)));
            panel.Controls.Add(rangeValidator);
            panel.Controls.Add(currentButton);
            panel.DefaultButton = currentButton.ID;
            panel.Attributes.Add("class", "panelRangeValidator");
            panel.Wrap = false;
            
            return panel;
        }

        private RadButton CreateActionButton(RadButtonType type, string id, string commandName, string commandArgument, bool causesValidation, string text, string tooltip, string primaryIconCssClass, bool borderNavigationButton = true)
        {
            RadButton b = new RadButton()
            {
                ButtonType = type,
                ID = id,
                CssClass = (borderNavigationButton == true) ? "borderNavigationButton" : "",
                CommandName = commandName,
                CommandArgument = commandArgument,
                CausesValidation = causesValidation,
                Text = text,
                ToolTip = tooltip
            };
            b.Icon.PrimaryIconCssClass = primaryIconCssClass;
            
            return b;
        }

        private bool FilteringByColumnIsActive()
        {
            if (!_grid.AllowFilteringByColumn && !_grid.MasterTableView.AllowFilteringByColumn)
            {
                return false;
            }
            // Se abilitato cerco se c'è almeno una colonna con filtraggio abilitato
            foreach (object obj in _grid.Columns)
            {
                GridEditableColumn column = obj as GridEditableColumn;
                if (column != null && column.SupportsFiltering())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
