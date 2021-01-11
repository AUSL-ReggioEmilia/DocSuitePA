Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Ennesima griglia </summary>
''' <remarks> Possibilmente non usare più </remarks>
Public MustInherit Class BaseGridBar
    Inherits DocSuite2008BaseControl
    Implements IGridBar

    Protected Delegate Sub SetReadDelegate(ByVal Year As Short, ByVal Number As Integer, ByVal LogType As String, ByVal LogDescription As String)
    Public Delegate Sub SetReadEventHandler(ByVal sender As Object, ByVal e As SelectedEventArgs)

    Public Event SetRead As SetReadEventHandler

#Region " Fields "

    Protected _grid As RadGrid = Nothing
    Protected _hasWorkflow As Boolean = False
    Private _ajaxEnabled As Boolean = True
    Private _loadingPanel As RadAjaxLoadingPanel = Nothing

    Private _logType As String = String.Empty
    Private _logDescription As String = String.Empty

    Protected _setReadDelegate As SetReadDelegate

#End Region

#Region " Properties "

    Public Property Grid() As RadGrid Implements IGridBar.Grid
        Get
            Return _grid
        End Get
        Set(ByVal value As RadGrid)
            _grid = value
        End Set
    End Property

    Public Property HasWorkflow() As Boolean Implements IGridBar.HasWorkflow
        Get
            Return _hasWorkflow
        End Get
        Set(ByVal value As Boolean)
            _hasWorkflow = value
        End Set
    End Property

    Public MustOverride ReadOnly Property DeselectButton() As Button Implements IGridBar.DeselectButton
    Public MustOverride ReadOnly Property PrintButton() As Button Implements IGridBar.PrintButton
    Public MustOverride ReadOnly Property DocumentsButton() As Button Implements IGridBar.DocumentsButton
    Public MustOverride ReadOnly Property SelectButton() As Button Implements IGridBar.SelectButton
    Public MustOverride ReadOnly Property SetReadButton() As Button Implements IGridBar.SetReadButton

    Public MustOverride ReadOnly Property LeftPanel() As Panel Implements IGridBar.LeftPanel
    Public MustOverride ReadOnly Property MiddlePanel() As Panel Implements IGridBar.MiddlePanel
    Public MustOverride ReadOnly Property RightPanel() As Panel Implements IGridBar.RightPanel

    Public Property AjaxEnabled() As Boolean
        Get
            Return _ajaxEnabled
        End Get
        Set(ByVal value As Boolean)
            _ajaxEnabled = value
        End Set
    End Property

    Public Property AjaxLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return _loadingPanel
        End Get
        Set(ByVal value As RadAjaxLoadingPanel)
            _loadingPanel = value
        End Set
    End Property

    Protected Property LogType() As String
        Get
            Return _logType
        End Get
        Set(ByVal value As String)
            _logType = value
        End Set
    End Property

    Protected Property LogDescription() As String
        Get
            Return _logDescription
        End Get
        Set(ByVal value As String)
            _logDescription = value
        End Set
    End Property

    Protected Property SetReadFunction() As SetReadDelegate
        Get
            Return _setReadDelegate
        End Get
        Set(ByVal value As SetReadDelegate)
            _setReadDelegate = value
        End Set
    End Property

#End Region

#Region " Methods "

    Protected Overridable Sub Initialize()
        If Grid IsNot Nothing Then
            InitializeAjaxSettings()
            AttachEvents()
            ConfigureSetReadProperties()
        End If
    End Sub

    Protected Overridable Sub InitializeAjaxSettings()
        If AjaxEnabled Then
            If AjaxLoadingPanel IsNot Nothing Then
                AjaxManager.AjaxSettings.AddAjaxSetting(SelectButton, Grid, AjaxLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(DeselectButton, Grid, AjaxLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(PrintButton, Grid, AjaxLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(SetReadButton, Grid, AjaxLoadingPanel)
            Else
                AjaxManager.AjaxSettings.AddAjaxSetting(SelectButton, Grid)
                AjaxManager.AjaxSettings.AddAjaxSetting(DeselectButton, Grid)
                AjaxManager.AjaxSettings.AddAjaxSetting(PrintButton, Grid)
                AjaxManager.AjaxSettings.AddAjaxSetting(SetReadButton, Grid)
            End If
        End If
    End Sub

    Protected Overridable Sub AttachEvents()
        AddHandler SelectButton.Click, AddressOf btnSelectAll_Click
        AddHandler DeselectButton.Click, AddressOf btnDeselectAll_Click
        AddHandler PrintButton.Click, AddressOf btnPrint_Click
        AddHandler SetReadButton.Click, AddressOf btnSetRead_Click
    End Sub

    Protected Overridable Sub SelectOrDeselectAll(ByVal Selected As Boolean) Implements IGridBar.SelectOrDeselectAll
        For Each item As GridDataItem In _grid.Items
            ' TODO: ma come è possibile? :(
            Dim cb As CheckBox = item.FindControl("cbSelect")
            ' Deseleziono tutti, ma seleziono solo quelli visibili '**REMOVE**
            If cb.Enabled AndAlso (Not Selected OrElse cb.Visible) Then
                cb.Checked = Selected
            End If
        Next
    End Sub

    Public Overridable Sub Show() Implements IGridBar.Show
        SelectButton.Visible = True
        DocumentsButton.Visible = True
        DeselectButton.Visible = True
        PrintButton.Visible = True
    End Sub

    Public Overridable Sub Hide() Implements IGridBar.Hide
        LeftPanel.Visible = False
        MiddlePanel.Visible = False
        RightPanel.Visible = False
    End Sub

    Protected MustOverride Sub Print() Implements IGridBar.Print

    Public MustOverride Function GetSelectedItems() As IList Implements IGridBar.GetSelectedItems

    Protected MustOverride Sub ConfigureSetReadProperties()

#End Region

#Region " Events "

    Protected Sub btnDeselectAll_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.SelectOrDeselectAll(False)
    End Sub

    Private Sub btnSelectAll_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.SelectOrDeselectAll(True)
    End Sub

    Private Sub btnPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.Print()
    End Sub

    Protected Overridable Sub SetReadedSelectedItems()
        Dim selected As Boolean = False
        Dim errorMessage As String = String.Empty

        Dim listId As IList(Of YearNumberCompositeKey) = GetSelectedItems()
        Try
            For Each key As YearNumberCompositeKey In listId
                SetReadFunction()(key.Year.Value, key.Number.Value, LogType, LogDescription)
                selected = True
            Next
        Catch ex As Exception
            errorMessage = "Errore durante la segnature come già letti"
            selected = False
        End Try

        SendSetReadEvent(selected, errorMessage)
    End Sub

    Protected Sub SendSetReadEvent(selected As Boolean, errorMessage As String)
        RaiseEvent SetRead(Me, New SelectedEventArgs(selected, errorMessage))
    End Sub

    Private Sub btnSetRead_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        SetReadedSelectedItems()
    End Sub
    
#End Region

End Class
