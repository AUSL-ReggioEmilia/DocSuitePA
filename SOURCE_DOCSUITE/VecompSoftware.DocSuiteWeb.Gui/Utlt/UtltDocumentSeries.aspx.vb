Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports VecompSoftware.DocSuiteWeb.Gui.Resl
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade.Formattables
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.Web

Public Class UtltDocumentSeries
    Inherits CommonBasePage

#Region " Fields "
    Private _availableContainers As IEnumerable(Of Container)
#End Region


#Region " Properties "

    ''' <summary> Contenitori su cui l'operatore ha diritti di inserimento </summary>
    ''' <remarks> Solo in caso di Insert o FromResolution </remarks>
    Private ReadOnly Property AvailableContainer() As IEnumerable(Of Container)
        Get
            If _availableContainers Is Nothing Then
                _availableContainers = Facade.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, New List(Of Integer)({DocumentSeriesContainerRightPositions.Insert, DocumentSeriesContainerRightPositions.Draft}), True)
            End If
            Return _availableContainers
        End Get
    End Property

    Public Property IdDocumentSeries As Integer
        Get
            If Not Session.Item("IdDocumentSeries") Is Nothing Then
                Return Session.Item("IdDocumentSeries")
            Else
                Return Nothing
            End If
        End Get
        Set(value As Integer)
            Session.Item("IdDocumentSeries") = value
        End Set
    End Property

#End Region


#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()

        If Not Page.IsPostBack Then
            LoadAvailableDocumentSeries()
            InitializeGrid()
        End If
    End Sub

    Private Sub ddlDocumentSeries_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumentSeries.SelectedIndexChanged
        Dim selected As Integer = Convert.ToInt32(ddlDocumentSeries.SelectedValue)
        IdDocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(selected).Id
        InitializeGrid()
    End Sub

    Private Sub cmdUpdate_Click(sender As Object, e As EventArgs) Handles cmdUpdate.Click
        If dgDocSeries.SelectedItems.Count > 1 Then
            ShowWindows(RadWindowAlert)
            Exit Sub
        End If
        Dim rowId As Integer
        For Each item As GridDataItem In dgDocSeries.SelectedItems
            rowId = Convert.ToInt32(TryCast(item.FindControl("lblId"), Label).Text)
        Next
        ShowWindows(UpdateDocumentalSeries)
        Dim checked As Boolean = Facade.DocumentSeriesIncrementalFacade.GetDocumentIncrementalSeriesById(rowId).IsOpen
        'chkIsDocSeriesOpen.Checked = checked

        If checked Then
            btnUpdateDocSeriesStatus.Text = "Chiudi serie documentale"
        Else
            btnUpdateDocSeriesStatus.Text = "Apri serie documentale"
        End If

    End Sub

    Private Sub btnUpdateDocSeriesStatus_Click(sender As Object, e As EventArgs) Handles btnUpdateDocSeriesStatus.Click


        If dgDocSeries.SelectedItems.Count > 1 Then
            ShowWindows(RadWindowAlert)
            Exit Sub
        End If
        Dim rowId As Integer
        For Each item As GridDataItem In dgDocSeries.SelectedItems
            rowId = Convert.ToInt32(TryCast(item.FindControl("lblId"), Label).Text)
        Next
        Dim checked As Boolean = Facade.DocumentSeriesIncrementalFacade.GetDocumentIncrementalSeriesById(rowId).IsOpen
        Dim currentDocumentIncrementalSeries As New DocumentSeriesIncremental
        currentDocumentIncrementalSeries = Facade.DocumentSeriesIncrementalFacade.GetById(rowId)
        Facade.DocumentSeriesIncrementalFacade.UpdateDocumentSeriesIncremental(currentDocumentIncrementalSeries, Not checked)
        InitializeGrid()
        CloseWindows(UpdateDocumentalSeries)

    End Sub


    Private Sub cmdAddYear_Click(sender As Object, e As EventArgs) Handles cmdAddYear.Click
        Dim year As Integer = getNextYear(IdDocumentSeries)
        If Facade.DocumentSeriesIncrementalFacade.CountOpenDocumentIncrementalSeriesByYear(IdDocumentSeries, year) > 0 Then
            ShowWindows(windowsJustAddYear)
            Exit Sub
        End If
        ShowWindows(windowsAddYear)

    End Sub

    Private Sub cmdYear_Click(sender As Object, e As EventArgs) Handles cmdYear.Click
        'aggiungi anno passando come parametro l'id della serie doc
        Dim year As Integer = getNextYear(IdDocumentSeries)
        Dim CurrentDocumentSeries As New DocumentSeries
        CurrentDocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(Integer.Parse(ddlDocumentSeries.SelectedValue))
        Facade.DocumentSeriesIncrementalFacade.AddDocumentSeriesIncremental(year, CurrentDocumentSeries)
        dgDocSeries.Rebind()
    End Sub

    Private Function getNextYear(idDocumentSeries As Integer) As Integer
        Dim dsi As DocumentSeriesIncremental = Facade.DocumentSeriesIncrementalFacade.GetLastDocumentSeriesIncremental(idDocumentSeries)
        Dim year As Integer = Date.Now.Year
        If Not IsNothing(dsi) Then
            year = CInt(dsi.Year)
        End If
        year = year + 1
        If year > Date.Now.Year + 1 Then
            year = Date.Now.Year
        End If
        Return year
    End Function

    Private Sub dgDocSeriesItemItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles dgDocSeries.ItemDataBound
        If Not (e.Item.ItemType = GridItemType.Item OrElse e.Item.ItemType = GridItemType.AlternatingItem) Then
            Return
        End If

        Dim DocumentSeriesInc As DocumentSeriesIncremental = CType(e.Item.DataItem, DocumentSeriesIncremental)

        With DirectCast(e.Item.FindControl("chkIsOpen"), Image)
            If DocumentSeriesInc.IsOpen.Equals(False) Then
                .ImageUrl = "~/App_Themes/DocSuite2008/imgset16/cancel.png"
            End If
        End With

        With DirectCast(e.Item.FindControl("lblId"), Label)
            .Text = DocumentSeriesInc.Id
        End With
        With DirectCast(e.Item.FindControl("lblDocumentSeries"), Label)
            .Text = DocumentSeriesInc.DocumentSeries.Name
        End With

        With DirectCast(e.Item.FindControl("lblYear"), Label)
            .Text = DocumentSeriesInc.Year.ToString
        End With

        With DirectCast(e.Item.FindControl("lblLastUsedNumber"), Label)
            .Text = DocumentSeriesInc.LastUsedNumber.ToString
        End With
    End Sub


#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, ddlDocumentSeries, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, ddlContainerArchive, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, dgDocSeries, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgDocSeries, dgDocSeries, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdUpdate, dgDocSeries, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAddYear, dgDocSeries, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdateDocSeriesStatus, dgDocSeries, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdUpdate, btnUpdateDocSeriesStatus)
    End Sub

    Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Try
            'LoadAvailableDocumentSeries()
            InitializeGrid()
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Format("Errore in fase di esecuzione Behaviour Command: {0}", e.Argument), ex)
            AjaxManager.Alert("Errore in fase di esecuzione commando.")
        End Try
    End Sub

    Private Sub ShowWindows(window As RadWindow)
        Dim script As String = "function f(){$find(""" + window.ClientID + """).show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);"
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, True)
    End Sub

    Private Sub CloseWindows(window As RadWindow)
        Dim script As String = "function f(){$find(""" + window.ClientID + """).close(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);"
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, True)
    End Sub

    Private Sub InitializeGrid()
        dgDocSeries.DataSource = Facade.DocumentSeriesIncrementalFacade.GetAllDocumentIncrementalSeries(IdDocumentSeries)
        dgDocSeries.DataBind()
    End Sub

    Private Sub LoadAvailableDocumentSeries()
        Dim results As IEnumerable(Of Container) = AvailableContainer.Where(Function(container) String.IsNullOrEmpty(ddlContainerArchive.SelectedValue) OrElse (container.Archive IsNot Nothing AndAlso container.Archive.Id = Integer.Parse(ddlContainerArchive.SelectedValue))).ToList()

        ddlDocumentSeries.DataValueField = "Id"
        ddlDocumentSeries.DataTextField = "Name"
        ddlDocumentSeries.DataSource = results
        ddlDocumentSeries.DataBind()
        If (Not results.IsNullOrEmpty AndAlso results.Count() = 1) Then
            ddlDocumentSeries.SelectedIndex = 0
            Dim CurrentDocumentSeriesItem As New DocumentSeriesItem()
            CurrentDocumentSeriesItem.DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(Integer.Parse(ddlDocumentSeries.SelectedValue))
            ddlDocumentSeries_SelectedIndexChanged(Me.Page, Nothing)
        Else
            ddlDocumentSeries.Items.Insert(0, "")
        End If
    End Sub

#End Region

End Class