using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.UI;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder;
using VecompSoftware.Helpers;
using VecompSoftware.Services.Logging;


namespace VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
{
    public class BindGrid : BaseGrid
    {
        #region Fields
        protected bool IsGrouping = false;
        protected bool IsPaging = false;
        private IFinder _finder;
        #endregion

        #region Properties
        /// <summary>
        /// Finder sulla quale è basata la griglia.
        /// </summary>
        public IFinder Finder
        {
            get
            {
                if (_finder == null)
                    _finder = GetFinderFromSession();
                return _finder;
            }
            set
            {
                _finder = value;
                PageSize = _finder.PageSize;
                if (_finder != null)
                    _finder.EnableTableJoin = true;
                PersistFinder();
            }
        }

        /// <summary>
        /// PageIndex personalizzato
        /// </summary>
        public override int CustomPageIndex
        {
            get { return Finder.CustomPageIndex; }
            set
            {
                if (Finder != null)
                {
                    Finder.CustomPageIndex = value;
                    PersistFinder();
                }
            }
        }

        #endregion

        #region Events
        public EventHandler CustomPageIndexChanged;
        #endregion

        #region Delegate
        /// <summary>
        /// Delego il chiamante a specificare la chiamata della funzione DataBindFinder
        /// </summary>
        public Action DelegateDataBindingFinder { get; set; }
        /// <summary>
        /// Delego il chiamante a specificare la chiamata della funzione FinderExecuteDoSearchHeader
        /// </summary>
        public Func<ICollection> DelegateDoSearchHeader { get; set; }

        public Action<GridSortCommandEventArgs> DelegateCreateSortingExpression { get; set; }

        #endregion

        #region Constructor
        public BindGrid() : base()
        { }
        #endregion

        #region Methods

        /// <summary>
        /// Logga le informazioni del finder
        /// </summary>
        /// <returns></returns>
        private string GetFinderSessionIdentifier()
        {
            return String.Format("{0}.{1}.IFinder", Page.GetType().BaseType.FullName, ClientID);
        }
        
        /// <summary>
        /// Legge il finder non templatizzato dalla sessione
        /// </summary>
        /// <returns></returns>
        private IFinder GetFinderFromSession()
        {
            if (HttpContext.Current == null)
                return null;
            string identifier = GetFinderSessionIdentifier();
            return (IFinder)HttpContext.Current.Session[identifier];
        }

        /// <summary>
        /// Memorizza il finder nella sessione. 
        /// Viene usato il finder non templatizzato
        /// </summary>
        public void PersistFinder()
        {
            if (HttpContext.Current == null)
                return;
            string identifier = GetFinderSessionIdentifier();
            HttpContext.Current.Session[identifier] = Finder;
            _finder = null;
            VirtualItemCount = 0; //In questo modo viene rifatto il conteggio dei record ai fini della paginazione.
        }

        /// <summary>
        /// Pulisce la sessione dal finder 
        /// </summary>
        public void DiscardFinder()
        {
            string identifier = GetFinderSessionIdentifier();
            HttpContext.Current.Session[identifier] = null;
            _finder = null;
        }

        /// <summary>
        /// Pulisce la sessione dal finder e ne impostata una nuovo
        /// </summary>
        /// <param name="finderToReplace"></param>
        public void ReplaceFinder(IFinder finderToReplace)
        {
            if(finderToReplace == null)
                DiscardFinder();
            else
                Finder = finderToReplace;
        }

        /// <summary>
        /// Mostra a video il messaggio dell'eccezione
        /// </summary>
        /// <param name="ex"></param>
        private void PrintSearchException(Exception ex)
        {
            string message = ex.Message;

            if(ex.InnerException != null) // Concateno sia l'errore che la inner exception
                message = string.Format("{0}{1}{2}", message, Environment.NewLine, ex.InnerException.Message);
            
            RadAjaxManager.GetCurrent(Page).Alert(string.Format("Errore in ricerca a causa di: {0}",StringHelper.ReplaceAlert(message, true)));
        }

        /// <summary>
        /// Questo metodo mi permette di isolare la gestione della ricerca e di settare il finder nella property.
        /// Questo metodo viene delegato allo sviluppatore. E' necessario implementarlo per eseguire il DoSearchHeader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THeader"></typeparam>
        /// <returns></returns>
        public ICollection<THeader> FinderExecuteDoSearchHeader<T, THeader>()
        {
            IFinder<T, THeader> finderToExecute = null;
            try
            {
                finderToExecute = Finder as IFinder<T, THeader>;
                if (finderToExecute != null)
                {
                    if (ImpersonateCurrentUser)
                    {
                        return ExecuteSearchWithImpersonation<ICollection<THeader>>(finderToExecute.DoSearchHeader);
                    }
                    else
                    {
                        return finderToExecute.DoSearchHeader();
                    }                    
                }                    
                return new List<THeader>();
            }
            catch (Exception ex)
            {
                PrintSearchException(ex);
                FileLogger.Warn(LogName.FileLog, String.Format("Errore nell'uso del finder [{0}]", finderToExecute.GetType()), ex);
                return null;
            }
        }

        /// <summary>
        /// Dato un finder, esegue la conta delle righe
        /// </summary>
        /// <param name="finderToExecute"></param>
        /// <returns></returns>        
        private int FinderExecuteCount(IFinder finderToExecute)
        {
            try
            {
                if (finderToExecute != null)
                {
                    Finder = finderToExecute;
                    if (ImpersonateCurrentUser)
                    {
                        return ExecuteSearchWithImpersonation<int>(finderToExecute.Count);
                    }
                    else
                    {
                        return finderToExecute.Count();
                    }                    
                }
                return 0;
            }
            catch (Exception ex)
            {
                PrintSearchException(ex);
                return default(int);
            }
        }

        private THeader ExecuteSearchWithImpersonation<THeader>(Func<THeader> func)
        {
            WindowsIdentity wi = (WindowsIdentity)HttpContext.Current.User.Identity;
            using (WindowsImpersonationContext wic = wi.Impersonate())
            using (ExecutionContext.SuppressFlow())
            {
                return func();
            }
        }

        /// <summary>
        /// Esegue il sort automatico della griglia in base alle espressioni di sort memorizzate
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.NeedDataSource += NeedDataSourceBinding;
        }

        /// <summary>
        /// Esegue i comandi presenti sulla griglia.
        /// 
        /// Filter
        /// CustomChangePage
        /// Export
        /// ExportFull
        /// Page
        /// ClearFilter
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemCommand(GridCommandEventArgs e)
        {
            switch(e.CommandName)
            {
                case "Filter" :
                    {
                        CustomPageIndex = 0;
                        FinderFiltering(e);
                        DelegateDataBindingFinder();
                        break;
                    }
                case "CustomChangePage":
                    {
                        int newPageIndex;
                        if (int.TryParse(((RadTextBox)e.Item.FindControl("txtCurrentPage")).Text, out newPageIndex))
                        {
                            CustomPageIndex = newPageIndex - 1;
                            DelegateDataBindingFinder();
                        }
                        break;
                    }
                case "Export" : 
                    {
                        DoExport(e);
                        break;
                    }
                case "ExportFull" :
                    {
                        // TODO: riscrivere la procedura di export. Utilizzare una copia del finder per estrarre i dati da mettere sull'esportazione e mappare le colonne già presenti nella griglia.

                        // Il finder viene chiamato 2 volte:
                        // 1) la prima chiamata serve per riempire tutta la griglia e poi lanciare l'export.
                        // 2) La seconda chiamata invece serve per resettare i dati in gliglia e mettere solo quelli visualizzabili.
                        int oldPageSize = Finder.PageSize, oldPageIndex = Finder.CustomPageIndex,
                            oldGridPageSize = this.PageSize, oldGridPageCount = this.PageCount;
                        // 1)
                        base.PageSize = this.PageCount * this.PageSize;
                        base.CustomPageIndex = 0;
                        Finder.PageSize = this.PageCount * this.PageSize;                        
                        Finder.CustomPageIndex = 0;
                        DelegateDataBindingFinder();
                        GridColumnCollection gridColumns = this.Columns;
                        GridDataItemCollection gridItems = this.Items;

                        // 2)
                        this.PageSize = oldGridPageSize;
                        Finder.PageSize = oldPageSize;
                        Finder.CustomPageIndex = oldPageIndex;
                        DelegateDataBindingFinder();

                        // esporto le colonne e gli elementi presenti nella griglia completa.
                        DoExport(e, gridColumns, gridItems);                        
                        break;
                    }
                case "Page":
                    {
                        DoPaging(e);
                        break;
                    }
                case "ClearFilters":
                    {
                        // TODO: rifare il metodo di pulizia filtri. 
                        /*
                        FilterHelper.RemoveFilterClientState(Page.Session, CommonUtil.GetInstance().AppTempPath, CustomPageIndex + 1);
                        */
                        
                        Finder.FilterExpressionsClear();                        
                        PersistFinder();
                        DelegateDataBindingFinder();                        
                        break;
                    }
                default:
                    {
                        base.OnItemCommand(e);
                        break;
                    }
            }
        }

        /// <summary>
        /// Esegue la paginazione della griglia leggendo i parametri di dimensione
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSortCommand(GridSortCommandEventArgs e)
        {
            base.OnSortCommand(e);
            DelegateCreateSortingExpression(e);
        }

        protected override void OnItemCreated(GridItemEventArgs e)
        {
            base.OnItemCreated(e);
            
            if (!(e.Item is GridFilteringItem) || Finder == null)
                return;
            
            // TODO rivedere l'espressione di filtraggio. 
            /*
               If Finder.FilterExpressions Is Nothing Then
                   Exit Sub
               End If
               
               Dim filteringItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
               For Each columnName As String In Finder.FilterExpressions.Keys
                   Dim control As Control = FilterHelper.GetFilterControl(Of Control)(filteringItem(columnName).Controls(0))
                   FilterHelper.SetFilterValue(control, Finder.FilterExpressions(columnName).FilterValue)
               Next
             */

            // TODO: rivedere il metodo di creazione dell'oggetto            
        }

        private void BindGrid_GroupsChanging(object source, GridGroupsChangingEventArgs e)
        {
            IsGrouping = true;
            if (e.Action == GridGroupsChangingAction.Ungroup)
                IsGrouping = false;
        }

        private void FinderFiltering(GridCommandEventArgs e)
        {
            // annullo il comando
            e.Canceled = true;
            
            // TODO: rivedere la logica di applicazione dei filtri
            
            //aggiorna filtro nel ViewState
            PersistFinder();             
        }

        /// <summary>
        /// Esegue il sort di una colonna nella griglia.
        /// Reperisco dalla griglia il nome della colonna da ordinare.
        /// Creo l'espressione Lambda corretta e la inserisco nel finder e poi lo persisto nella sessione.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THeader"></typeparam>
        /// <param name="e"></param>
        public void FinderSorting<T, THeader>(GridSortCommandEventArgs e)
        {            
            // TODO: verificare gli ordinamenti sui campi dell'entità Header. In questo momento l'ordinamento funziona solo se il campo è settato sull'entità padre

            IFinder<T, THeader> finderToExecute = Finder as IFinder<T, THeader>;
            finderToExecute.SortExpressionsClear();
            SortDirection sortOrder;
            if (e.NewSortOrder == GridSortOrder.None && e.OldSortOrder == GridSortOrder.Descending)
                sortOrder = SortDirection.Descending;
            else
                sortOrder = e.NewSortOrder == GridSortOrder.Ascending ? SortDirection.Ascending : SortDirection.Descending;
            
            string parameterName = e.SortExpression;
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            Expression<Func<T, object>> expr = Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(param, parameterName), typeof(object)), param);
            finderToExecute.SortExpressions.Add(new SortExpression<T>(){Expression = expr, Direction = sortOrder});
            Finder = finderToExecute;

            PersistFinder();
        }
        
        /// <summary>
        /// Esegue la paginazione
        /// </summary>
        /// <param name="e"></param>
        private void DoPaging(GridCommandEventArgs e)
        {
            base.OnItemCommand(e);
            
            IsPaging = true;
            switch (e.CommandArgument.ToString())
            {
                case "First":
                    CustomPageIndex = 0;
                    if (CustomPageIndexChanged != null)
                    {
                        CustomPageIndexChanged(this, new EventArgs());
                    }

                    DelegateDataBindingFinder();

                    break;
                case "Prev":
                    CustomPageIndex = CustomPageIndex - 1;
                    if (CustomPageIndexChanged != null)
                    {
                        CustomPageIndexChanged(this, new EventArgs());
                    }

                    DelegateDataBindingFinder();

                    break;
                case "Next":
                    CustomPageIndex = CustomPageIndex + 1;
                    if (CustomPageIndexChanged != null)
                    {
                        CustomPageIndexChanged(this, new EventArgs());
                    }

                    DelegateDataBindingFinder();

                    break;
                case "Last":
                    CustomPageIndex = PageCount - 1;
                    if (CustomPageIndexChanged != null)
                    {
                        CustomPageIndexChanged(this, new EventArgs());
                    }

                    DelegateDataBindingFinder();

                    break;
                default:
                    IsPaging = false;
                    break;
            }
        }

        /// <summary>
        /// Binding del data source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void NeedDataSourceBinding(object source, GridNeedDataSourceEventArgs e)
        {
            ICollection results = DelegateDoSearchHeader();
            DataSource = results;

            if (results != null)
            {
                InitializeVirtualItemCount(Finder);
                
                if (((MasterTableView.GroupByExpressions.Count > 0) || IsGrouping))
                {
                    CurrentPageIndex = 0;
                }
            }            
        }

        /// <summary>
        /// Esegue la ricerca attraverso il finder interno eseguendo anche il count dei risultati totali
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THeader"></typeparam>
        public void DataBindFinder<T, THeader>()
        {
            IFinder<T, THeader> finderToExecute = Finder as IFinder<T, THeader>;
            if (finderToExecute == null)
            {
                return;
            }
            ICollection<THeader> results = FinderExecuteDoSearchHeader<T, THeader>();
            DataSource = results;
            
            if (results != null)
            {
                InitializeVirtualItemCount(Finder);
                
                if (((MasterTableView.GroupByExpressions.Count > 0) || IsGrouping))
                {
                    CurrentPageIndex = 0;
                }
                //TODO: Inserire l'ordinamento qui per la parte di esportazione
            }
            
            DataBind();
        }
        
        /// <summary>
        /// Inizializza il conteggio delle righe in griglia
        /// </summary>
        /// <param name="finderToExecute"></param>
        private void InitializeVirtualItemCount(IFinder finderToExecute)
        {
            if (VirtualItemCount == 0)
            {
                VirtualItemCount = FinderExecuteCount(finderToExecute);
            }
        }

        /// <summary>
        /// Esegue solo la ricerca attraverso il finder interno.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THeader"></typeparam>
        /// <param name="finderToExecute"></param>
        /// <param name="externalCount">Count dei risultati totali</param>
        public void DataBindFinderWithExtCount<T, THeader>(IFinder<T, THeader> finderToExecute, int externalCount)
        {
            ICollection<THeader> results = FinderExecuteDoSearchHeader<T, THeader>();
            DataSource = results;

            if (results != null)
            {
                VirtualItemCount = externalCount;                
            }
            DataBind();
        }

        public void SaveSettings()
        {
            // TODO: salvare i filtri sulla griglia

            /*
            HttpCookie myCookie = new HttpCookie(ClientID + "_GridSettings");
            myCookie(ClientID + "_state") = GridSettingsPersister.SaveSettings(this);
            Page.Response.Cookies.Add(myCookie);
            */
        }

        public void LoadSettings(string settings)
        {
            // TODO: caricare i filtri sulla griglia

            /*
            HttpCookie myCookie = Page.Request.Cookies(ClientID + "_GridSettings");
            if ((myCookie != null && myCookie(ClientID + "_state") != null))
            {
                GridSettingsPersister.LoadSettings(this, myCookie(ClientID + "_state"));
            }
             */
        }
        #endregion

        /// <summary>
        /// Necessario per reimpostare la pagina precedente in "History Back".
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Page.Response.Cache.SetNoStore();
            Page.Response.Cache.SetValidUntilExpires(true);
        }
    }
}
