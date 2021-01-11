Imports System.Web.UI
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Logging
Imports System.Web
Imports VecompSoftware.WebAPIManager.Exceptions
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles

Public Class BindGrid
    Inherits BaseGrid

#Region " Fields "

    Protected IsGrouping As Boolean = False
    Protected IsPaging As Boolean = False

    Private _finder As IFinder

#End Region

#Region " Properties "

    Public Property Finder As IFinder
        Get
            If _finder Is Nothing Then
                _finder = GetFinderFromSession()
            End If
            Return _finder
        End Get
        Set(ByVal value As IFinder)
            _finder = value
            PageSize = _finder.PageSize
            If _finder IsNot Nothing Then
                ReflectionHelper.SetProperty(_finder, "EnableTableJoin", True)
            End If
            PersistFinder()
        End Set
    End Property


    Public Overrides Property CustomPageIndex As Integer
        Get
            Return Finder.CustomPageIndex
        End Get
        Set(ByVal value As Integer)
            Finder.CustomPageIndex = value
            PersistFinder()
        End Set
    End Property

    Public Property Criteria As Object
        Get
            Return Page.Session(ID + "_criteria")
        End Get
        Set(ByVal value As Object)
            Page.Session(ID + "_criteria") = value
        End Set
    End Property

    Private Property ImpersonationAction As Func(Of IFinder, Func(Of Object), Object)

    Private Property ImpersonationCounterAction As Func(Of IFinder, Func(Of Integer), Integer)

#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    ' Gestione finder in sessione
    Private Function GetFinderSessionIdentifier() As String
        Return String.Format("{0}.{1}.IFinder", Page.GetType().BaseType.FullName, ClientID)
    End Function

    Private Function GetFinderFromSession() As IFinder
        If HttpContext.Current Is Nothing Then
            Return Nothing
        End If
        Dim identifier As String = GetFinderSessionIdentifier()
        Return DirectCast(HttpContext.Current.Session.Item(identifier), IFinder)
    End Function

    Public Sub PersistFinder()
        If HttpContext.Current Is Nothing Then
            Return
        End If
        Dim identifier As String = GetFinderSessionIdentifier()
        HttpContext.Current.Session.Item(identifier) = Finder
        _finder = Nothing
        VirtualItemCount = 0 ' In questo modo viene rifatto il conteggio dei record ai fini della paginazione.
    End Sub

    Public Sub DiscardFinder()
        Dim identifier As String = GetFinderSessionIdentifier()
        HttpContext.Current.Session.Item(identifier) = Nothing
        _finder = Nothing
    End Sub

    Public Sub ReplaceFinder(finderToReplace As IFinder)
        If finderToReplace Is Nothing Then
            DiscardFinder()
        Else
            Finder = finderToReplace
        End If
    End Sub

    ''' <summary> Mostra a video il messaggio dell'eccezione </summary>
    Private Sub PrintSearchException(ByVal ex As Exception)
        Dim message As String = ex.Message
        If ex.InnerException IsNot Nothing Then
            message = ex.InnerException.Message
        End If
        FileLogger.Error(LogName.FileLog, message, ex)
        RadAjaxManager.GetCurrent(Page).Alert("Errore in ricerca a causa di: " & StringHelper.ReplaceAlert(message, True))
    End Sub

    Private Sub PrintWebAPIException(ByVal ex As WebAPIException(Of ICollection(Of WebAPIDto(Of Object))))
        Dim message As String = ex.Message
        If ex.InnerException IsNot Nothing Then
            message = ex.InnerException.Message
        End If
        FileLogger.Error(LogName.FileLog, message, ex)
        RadAjaxManager.GetCurrent(Page).Alert(String.Format("Non è stato possibile ricevere informazioni da {0}.", ex.TenantName))
    End Sub

    Private Function FinderExecuteDoSearchHeader(finderToExecute As Object) As Object
        Try
            Finder = finderToExecute
            If ImpersonateCurrentUser Then
                RaiseEvent NeedImpersonation(Me, New EventArgs())
                If TypeOf Finder Is FascicleFinder AndAlso finderToExecute.FromPostMethod Then
                    Return ExecuteSearchWithImpersonation(Of Object)(AddressOf finderToExecute.GetFromPostMethod)
                End If
                Return ExecuteSearchWithImpersonation(Of Object)(AddressOf finderToExecute.DoSearchHeader)
            Else
                If TypeOf Finder Is FascicleFinder AndAlso finderToExecute.FromPostMethod Then
                    Return finderToExecute.GetFromPostMethod()
                End If
                Return finderToExecute.DoSearchHeader()
            End If
        Catch ex_ea As AggregateException
            For Each ex As Exception In ex_ea.InnerExceptions
                If TypeOf ex Is WebAPIException(Of ICollection(Of WebAPIDto(Of Object))) Then
                    PrintWebAPIException(ex)
                    Return CType(ex, WebAPIException(Of ICollection(Of WebAPIDto(Of Object)))).Results
                Else
                    FileLogger.Error(LogName.FileLog, ex.Message, ex)
                End If
            Next
        Catch ex As WebAPIException(Of ICollection(Of WebAPIDto(Of Object)))
            PrintWebAPIException(ex)
            Return ex.Results
        Catch ex As Exception
            PrintSearchException(ex)
            FileLogger.Warn(LogName.FileLog, String.Format("Errore nell'uso del finder [{0}]", finderToExecute.GetType()), ex)
        End Try
        Return New ArrayList()
    End Function

    Private Function FinderExecuteCount(finderToExecute As Object) As Integer
        Try
            Finder = finderToExecute
            If ImpersonateCurrentUser Then
                RaiseEvent NeedImpersonation(Me, New EventArgs())
                Return ExecuteCounterWithImpersonation(AddressOf finderToExecute.Count)
            Else
                Return finderToExecute.Count()
            End If
        Catch ex As Exception
            PrintSearchException(ex)
        End Try
    End Function

    Public Sub SetImpersonationAction(func As Func(Of IFinder, Func(Of Object), Object))
        ImpersonationAction = func
    End Sub

    Public Sub SetImpersonationCounterAction(func As Func(Of IFinder, Func(Of Integer), Integer))
        ImpersonationCounterAction = func
    End Sub

    Private Function ExecuteSearchWithImpersonation(Of T)(func As Func(Of T)) As T
        If ImpersonationAction Is Nothing Then
            Return func()
        End If

        Return ImpersonationAction(Finder, func)
    End Function

    Private Function ExecuteCounterWithImpersonation(func As Func(Of Integer)) As Integer
        If ImpersonationCounterAction Is Nothing Then
            Return func()
        End If

        Return ImpersonationCounterAction(Finder, func)
    End Function

    ''' <summary> Esegue il sort automatico della griglia in base alle espressioni di sort memorizzate </summary>
    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        MyBase.OnInit(e)
        AddHandler NeedDataSource, AddressOf NeedDataSourceDelegate
    End Sub


    Protected Overrides Sub OnItemCommand(ByVal e As GridCommandEventArgs)
        Select Case e.CommandName
            Case "Filter"
                'MasterTableView.AllowMultiColumnSorting = True
                CustomPageIndex = 0
                FinderFiltering(e)
                DataBindFinder()
                'MasterTableView.AllowMultiColumnSorting = False
                Exit Select
            Case "CustomChangePage"
                Dim newPageIndex As Integer
                If Integer.TryParse(DirectCast(e.Item.FindControl("txtCurrentPage"), RadTextBox).Text, newPageIndex) Then
                    CustomPageIndex = newPageIndex - 1
                    RaiseEvent CustomPageIndexChanged(Me, New EventArgs())
                    DataBindFinder()
                End If
            Case "Export"
                DoExport(e, Me.Columns, Me.Items)
            Case "ExportFull"
                Dim oldPageSize = Finder.PageSize
                Dim oldPageIndex = Finder.CustomPageIndex
                Dim oldGridPageSize = Me.PageSize
                Dim oldGridPageCount = Me.PageCount
                Finder.PageSize = Me.PageCount * Me.PageSize
                Finder.CustomPageIndex = 0
                Me.DataBindFinder()
                Dim gridColumns As GridColumnCollection = Me.Columns
                Dim gridItems As GridDataItemCollection = Me.Items
                Me.PageSize = oldGridPageSize
                Finder.PageSize = oldPageSize
                Finder.CustomPageIndex = oldPageIndex
                Me.DataBindFinder()
                DoExport(e, gridColumns, gridItems)
            Case "Page"
                DoPaging(e)
            Case "ClearFilters"
                FilterHelper.RemoveFilterClientState(Page.Session, CommonUtil.GetInstance().AppTempPath, CustomPageIndex + 1)
                Finder.FilterExpressions.Clear()
                PersistFinder()
                DataBindFinder()
            Case "CustomFilter"
                CustomPageIndex = 0
                Dim commands As IList(Of IFilterExpression) = DirectCast(e.CommandArgument, IList(Of IFilterExpression))
                For Each command As IFilterExpression In commands
                    Dim filterExpressions As IDictionary(Of String, IFilterExpression) = ReflectionHelper.GetProperty(Finder, "FilterExpressions")
                    FilterHelper.UpdateFilter(filterExpressions, command)
                Next
                DataBindFinder()

            Case Else
                MyBase.OnItemCommand(e)
        End Select
    End Sub

    Protected Sub NeedDataSourceDelegate(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs)
        If Not IsPaging Then
            NeedDataSourceBinding()
        End If
    End Sub

    ''' <summary> Esegue la paginazione della griglia leggendo i parametri di dimensione </summary>
    Protected Overrides Sub OnSortCommand(ByVal e As GridSortCommandEventArgs)
        MyBase.OnSortCommand(e)
        FinderSorting(e)
    End Sub

    Protected Overrides Sub OnItemCreated(ByVal e As GridItemEventArgs)
        MyBase.OnItemCreated(e)

        If Not (TypeOf e.Item Is GridFilteringItem) OrElse (Finder Is Nothing) Then
            Exit Sub
        End If

        If Finder.FilterExpressions Is Nothing Then
            Exit Sub
        End If

        Dim filteringItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
        For Each columnName As String In Finder.FilterExpressions.Keys
            Try
                Dim control As Control = FilterHelper.GetFilterControl(Of Control)(filteringItem(columnName).Controls(0))
                FilterHelper.SetFilterValue(control, Finder.FilterExpressions(columnName).FilterValue)
            Catch ex As Exception
                FileLogger.Error(LogName.FileLog, ex.Message, ex)
            End Try
        Next
    End Sub

    Private Sub BindGrid_GroupsChanging(ByVal source As Object, ByVal e As GridGroupsChangingEventArgs) Handles Me.GroupsChanging
        IsGrouping = True
        If e.Action = GridGroupsChangingAction.Ungroup Then
            IsGrouping = False
        End If
    End Sub

    Private Sub FinderFiltering(ByVal e As GridCommandEventArgs)
        'annullo il comando
        e.Canceled = True

        'Istanzia il builder dei filtri dopo averlo recuperato
        Dim command As Pair = DirectCast(e.CommandArgument, Pair)
        Dim filterSetter As New FilterSetter(Columns.FindByUniqueName(command.Second), Finder)
        'imposta il filtro
        Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
        filterSetter.SetFilter(command, filterItem)

        'evidenzia il filtro corrente
        FilterHelper.HighlightFilterMenu(command.First, FilterMenu)

        'aggiorna filtro nel ViewState
        PersistFinder()
    End Sub

    Private Sub FinderSorting(ByVal e As GridSortCommandEventArgs)
        Finder.SortExpressions.Clear()

        Dim sortOrder As String
        If e.NewSortOrder = GridSortOrder.None AndAlso e.OldSortOrder = GridSortOrder.Descending Then
            sortOrder = "ASC"
        Else
            sortOrder = If(e.NewSortOrder = GridSortOrder.Ascending, "ASC", "DESC")
        End If
        Finder.SortExpressions.Add(e.SortExpression, sortOrder)

        PersistFinder()
    End Sub


    Private Sub AttachSortExpressionFromFinder()
        Dim dic As IDictionary(Of String, String) = Finder.SortExpressions
        If dic.Keys.Count <= 0 Then
            Exit Sub
        End If

        '' Il codice precedente inseriva gli ordinamenti n-volte
        '' Questo verifica solo se ci sono sort expression da collegare e nel caso lo fa una volta sola
        If Columns.Cast(Of GridColumn)().Any(Function(col) dic.Keys.Contains(col.SortExpression)) Then
            MasterTableView.SortExpressions.Clear()
            For Each sortExpression As KeyValuePair(Of String, String) In dic
                MasterTableView.SortExpressions.AddSortExpression(String.Format("{0} {1}", sortExpression.Key, sortExpression.Value))
            Next
        End If
    End Sub

    Private Sub DoPaging(ByVal e As GridCommandEventArgs)
        MyBase.OnItemCommand(e)

        IsPaging = True
        Select Case e.CommandArgument
            Case "First"
                CustomPageIndex = 0
                RaiseEvent CustomPageIndexChanged(Me, New EventArgs())
                DataBindFinder()

            Case "Prev"
                CustomPageIndex = CustomPageIndex - 1
                RaiseEvent CustomPageIndexChanged(Me, New EventArgs())
                DataBindFinder()

            Case "Next"
                CustomPageIndex = CustomPageIndex + 1
                RaiseEvent CustomPageIndexChanged(Me, New EventArgs())
                DataBindFinder()

            Case "Last"
                CustomPageIndex = PageCount - 1
                RaiseEvent CustomPageIndexChanged(Me, New EventArgs())
                DataBindFinder()

            Case Else
                IsPaging = False
        End Select
    End Sub

    Private Sub NeedDataSourceBinding()
        If Finder Is Nothing Then
            Exit Sub
        End If

        DataSource = FinderExecuteDoSearchHeader(Finder)
        If DataSource.Count > 0 Then
            InitializeVirtualItemCount()

            If ((MasterTableView.GroupByExpressions.Count > 0) OrElse IsGrouping) Then
                CurrentPageIndex = 0
            End If
            AttachSortExpressionFromFinder()
        End If
    End Sub

    ''' <summary> Esegue la ricerca attraverso il finder interno eseguendo anche il count dei risultati totali </summary>
    Public Sub DataBindFinder()
        DataSource = FinderExecuteDoSearchHeader(Finder)
        If DataSource.Count > 0 Then
            InitializeVirtualItemCount()

            If ((MasterTableView.GroupByExpressions.Count > 0) OrElse IsGrouping) Then
                CurrentPageIndex = 0
            End If
            AttachSortExpressionFromFinder()
        End If

        DataBind()
    End Sub

    Private Sub InitializeVirtualItemCount()
        If VirtualItemCount = 0 Then
            VirtualItemCount = FinderExecuteCount(Finder)
        End If
    End Sub

    ''' <summary> Esegue solo la ricerca attraverso il finder interno. </summary>
    ''' <param name="externalCount">Count dei risultati totali</param>
    Public Sub DataBindFinderWithExtCount(ByVal externalCount As Integer)

        DataSource = FinderExecuteDoSearchHeader(Finder)

        If DataSource.Count > 0 Then
            VirtualItemCount = externalCount
            AttachSortExpressionFromFinder()
        End If

        DataBind()
    End Sub

    Public Sub SaveSettings()
        Dim myCookie As New HttpCookie(ClientID & "_GridSettings")
        myCookie(ClientID & "_state") = GridSettingsPersister.SaveSettings(Me)
        Page.Response.Cookies.Add(myCookie)
    End Sub

    Public Sub LoadSettings(ByVal settings As String)
        Dim myCookie As HttpCookie = Page.Request.Cookies(ClientID & "_GridSettings")
        If (myCookie IsNot Nothing AndAlso myCookie(ClientID & "_state") IsNot Nothing) Then
            GridSettingsPersister.LoadSettings(Me, myCookie(ClientID & "_state"))
        End If
    End Sub

#End Region

#Region " Events "

    Private Sub BindGrid_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' FG20130909: Necessario per reimpostare la pagina precedente in "History Back".
        Page.Response.Cache.SetAllowResponseInBrowserHistory(False)
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Response.Cache.SetNoStore()
        Page.Response.Cache.SetValidUntilExpires(True)
    End Sub

#End Region

    Public Event CustomPageIndexChanged As EventHandler

    Public Sub OnCustomPageIndexChanged(e As EventArgs)
        RaiseEvent CustomPageIndexChanged(Me, e)
    End Sub

    Public Event NeedImpersonation As EventHandler

    Public Sub OnNeedImpersonation(e As EventArgs)
        RaiseEvent NeedImpersonation(Me, e)
    End Sub

End Class