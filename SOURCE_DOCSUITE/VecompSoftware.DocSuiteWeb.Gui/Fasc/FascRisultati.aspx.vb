Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class FascRisultati
    Inherits FascBasePage

#Region "Fields"

#End Region

#Region "Properties"

    Public ReadOnly Property SelectableFaciclesThreshold As Integer
        Get
            Return ProtocolEnv.SelectableProtocolThreshold
        End Get
    End Property

    Public ReadOnly Property ChoiseFolderEnabled() As String
        Get
            Return Request.QueryString.Get("ChoiseFolderEnabled")
        End Get
    End Property

    Public ReadOnly Property SelectedFascicleFolderId() As String
        Get
            Return Request.QueryString.Get("SelectedFascicleFolderId")
        End Get
    End Property

    Public ReadOnly Property CurrentFascicleId() As String
        Get
            Return Request.QueryString.Get("CurrentFascicleId")
        End Get
    End Property

    Public ReadOnly Property BackButtonEnabled() As Boolean
        Get
            Return GetKeyValueOrDefault("BackButtonEnabled", False)
        End Get
    End Property

    Public ReadOnly Property EnableSessionFilterLoading() As Boolean
        Get
            Return GetKeyValueOrDefault("EnableSessionFilterLoading", False)
        End Get
    End Property

    Public ReadOnly Property DynamicMetadataEnabled() As Boolean
        Get
            Return GetKeyValueOrDefault("DynamicMetadataEnabled", False)
        End Get
    End Property

    Public Property MetadataColumns As List(Of BaseFieldModel)
        Get
            Return DirectCast(ViewState("MetadataColumns"), List(Of BaseFieldModel))
        End Get
        Set(ByVal value As List(Of BaseFieldModel))
            ViewState("MetadataColumns") = value
        End Set
    End Property

    Public Property MetadataModel As MetadataDesignerModel
        Get
            Return DirectCast(ViewState("MetadataModel"), MetadataDesignerModel)
        End Get
        Set(ByVal value As MetadataDesignerModel)
            ViewState("MetadataModel") = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Action.Eq("SearchFascicles") Then
            MasterDocSuite.TitleVisible = False
            uscFascicleGrid.ColumnClientSelectVisible = False
            btnDeselectAll.Visible = False
            btnDocuments.Visible = False
            btnSelectAll.Visible = False
            backBtn.Visible = BackButtonEnabled
        End If
        If Not IsPostBack Then
            Initialize()
            DoSearch()
        End If
        RefreshDynamicMetadataColumns()
    End Sub

    Protected Sub BtnExportAll_OnClicked(sender As Object, e As EventArgs) Handles btnExportAll.Click
        Dim oldPageSize As Integer = uscFascicleGrid.Grid.PageSize
        Dim oldPageIndex As Integer = uscFascicleGrid.Grid.CustomPageIndex
        Dim oldGridPageSize As Integer = uscFascicleGrid.Grid.PageSize
        Dim oldGridPageCount As Integer = uscFascicleGrid.Grid.PageCount
        uscFascicleGrid.Grid.Finder.PageSize = uscFascicleGrid.Grid.PageCount * uscFascicleGrid.Grid.PageSize
        uscFascicleGrid.Grid.Finder.CustomPageIndex = 0
        uscFascicleGrid.Grid.DataBindFinder()
        Dim metadataColumns As GridColumnCollection = New GridColumnCollection(uscFascicleGrid.Grid.MasterTableView)
        Dim gridColumns As GridColumnCollection = uscFascicleGrid.Grid.Columns
        Dim gridItems As GridDataItemCollection = uscFascicleGrid.Grid.Items
        uscFascicleGrid.Grid.PageSize = oldGridPageSize
        uscFascicleGrid.Grid.Finder.PageSize = oldPageSize
        uscFascicleGrid.Grid.Finder.CustomPageIndex = oldPageIndex
        uscFascicleGrid.Grid.DataBindFinder()

        gridColumns = New GridColumnCollection(uscFascicleGrid.Grid.MasterTableView, New ArrayList(gridColumns.OfType(Of GridColumn).Where(Function(c) c.Visible AndAlso c.Display AndAlso Not c.UniqueName.Eq("ClientSelectColumn")).ToList()))
        Dim metadataRepository As Entity.Commons.MetadataRepository = WebAPIImpersonatorFacade.ImpersonateFinder(New MetadataRepositoryFinder(DocSuiteContext.Current.CurrentTenant),
                Function(impersonationType, finder)
                    finder.UniqueId = CType(uscFascicleGrid.Grid.Finder, FascicleFinder).FascicleFinderModel.IdMetadataRepository
                    finder.EnablePaging = False
                    Return finder.DoSearch().Select(Function(x) x.Entity).FirstOrDefault()
                End Function)

        If (metadataRepository IsNot Nothing AndAlso metadataRepository.JsonMetadata IsNot Nothing) Then
            Dim metadataModel As MetadataDesignerModel = JsonConvert.DeserializeObject(Of MetadataDesignerModel)(metadataRepository.JsonMetadata)
            Dim metadataFields As List(Of BaseFieldModel) = New List(Of BaseFieldModel)
            If metadataModel IsNot Nothing Then
                metadataFields.AddRange(metadataModel.BoolFields)
                metadataFields.AddRange(metadataModel.ContactFields)
                metadataFields.AddRange(metadataModel.DateFields)
                metadataFields.AddRange(metadataModel.DiscussionFields)
                metadataFields.AddRange(metadataModel.EnumFields)
                metadataFields.AddRange(metadataModel.NumberFields)
                metadataFields.AddRange(metadataModel.TextFields)
            End If

            For Each metadataColumn As BaseFieldModel In metadataFields
                If gridColumns.FindByUniqueNameSafe(metadataColumn.KeyName.Replace(" ", "")) IsNot Nothing Then
                    gridColumns.Remove(gridColumns.FindByUniqueNameSafe(metadataColumn.KeyName.Replace(" ", "")))
                End If

                Dim gridColumn As GridBoundColumn = New GridBoundColumn()
                gridColumn.HeaderText = metadataColumn.Label
                gridColumn.DataField = metadataColumn.KeyName
                gridColumn.UniqueName = metadataColumn.KeyName.Replace(" ", "")
                If metadataModel.BoolFields.Any(Function(x) x.KeyName.Eq(metadataColumn.KeyName)) Then
                    gridColumn.DataFormatString = NameOf(MetadataDesignerModel.BoolFields)
                End If
                If metadataModel.DateFields.Any(Function(x) x.KeyName.Eq(metadataColumn.KeyName)) Then
                    gridColumn.DataFormatString = NameOf(MetadataDesignerModel.DateFields)
                End If
                metadataColumns.Add(gridColumn)
            Next
        End If

        Dim table As Table = CreateExportableTable(gridColumns, metadataColumns, gridItems)
        ExportHelper.ExportToExcel(table, Page, String.Concat(uscFascicleGrid.Grid.ExportSettings.FileName, FileHelper.XLS))
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvFascicles As BindGrid = uscFascicleGrid.Grid

        If gvFascicles.DataSource IsNot Nothing Then
            lblHeader.Text = String.Format("Fascicoli - Risultati ({0}/{1})", gvFascicles.DataSource.Count, gvFascicles.VirtualItemCount)
        Else
            lblHeader.Text = "Fascicoli - Nessun Risultato"
        End If
        MasterDocSuite.HistoryTitle = lblHeader.Text
    End Sub

    Private Sub ImpersonationFinderDelegate(ByVal source As Object, ByVal e As EventArgs)
        uscFascicleGrid.Grid.SetImpersonationAction(AddressOf ImpersonateGridCallback)
        uscFascicleGrid.Grid.SetImpersonationCounterAction(AddressOf ImpersonateGridCallback)
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        MasterDocSuite.TitleVisible = False
        btnExportAll.Visible = DynamicMetadataEnabled
    End Sub

    Private Sub InitializeAjaxSettings()
        AddHandler uscFascicleGrid.Grid.NeedImpersonation, AddressOf ImpersonationFinderDelegate
        AddHandler uscFascicleGrid.Grid.DataBound, AddressOf DataSourceChanged

        AjaxManager.AjaxSettings.AddAjaxSetting(uscFascicleGrid.Grid, uscFascicleGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscFascicleGrid.Grid, lblHeader)
    End Sub

    Private Sub DoSearch()
        Dim setSortExpression As Boolean = False
        If uscFascicleGrid.Grid.Finder Is Nothing OrElse EnableSessionFilterLoading Then
            uscFascicleGrid.Grid.Finder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.FascFinderType), FascicleFinder)
            setSortExpression = True
        End If
        If setSortExpression Then
            uscFascicleGrid.Grid.Finder.SortExpressions("Entity.StartDate") = "desc"
        End If

        If DynamicMetadataEnabled Then
            Dim dataSource As ICollection(Of WebAPIDto(Of FascicleModel)) = uscFascicleGrid.Grid.GetDataSource(Of FascicleModel)()
            MetadataColumns = New List(Of BaseFieldModel)

            Dim metadataRepository As Entity.Commons.MetadataRepository = WebAPIImpersonatorFacade.ImpersonateFinder(New MetadataRepositoryFinder(DocSuiteContext.Current.CurrentTenant),
                Function(impersonationType, finder)
                    finder.UniqueId = CType(uscFascicleGrid.Grid.Finder, FascicleFinder).FascicleFinderModel.IdMetadataRepository
                    finder.EnablePaging = False
                    Return finder.DoSearch().Select(Function(x) x.Entity).FirstOrDefault()
                End Function)

            If (metadataRepository IsNot Nothing AndAlso metadataRepository.JsonMetadata IsNot Nothing) Then
                MetadataModel = JsonConvert.DeserializeObject(Of MetadataDesignerModel)(metadataRepository.JsonMetadata)
                If MetadataModel IsNot Nothing Then
                    MetadataColumns.AddRange(MetadataModel.BoolFields.Where(Function(x) x.ShowInResults).ToList())
                    MetadataColumns.AddRange(MetadataModel.ContactFields.Where(Function(x) x.ShowInResults).ToList())
                    MetadataColumns.AddRange(MetadataModel.DateFields.Where(Function(x) x.ShowInResults).ToList())
                    MetadataColumns.AddRange(MetadataModel.DiscussionFields.Where(Function(x) x.ShowInResults).ToList())
                    MetadataColumns.AddRange(MetadataModel.EnumFields.Where(Function(x) x.ShowInResults).ToList())
                    MetadataColumns.AddRange(MetadataModel.NumberFields.Where(Function(x) x.ShowInResults).ToList())
                    MetadataColumns.AddRange(MetadataModel.TextFields.Where(Function(x) x.ShowInResults).ToList())
                End If

                For Each metadataColumn As BaseFieldModel In MetadataColumns
                    If uscFascicleGrid.Grid.Columns.FindByUniqueNameSafe(metadataColumn.KeyName.Replace(" ", "")) Is Nothing Then
                        Dim gridColumn As GridBoundColumn = New GridBoundColumn()
                        gridColumn.HeaderText = metadataColumn.Label
                        gridColumn.DataField = metadataColumn.KeyName
                        gridColumn.UniqueName = metadataColumn.KeyName.Replace(" ", "")
                        uscFascicleGrid.Grid.MasterTableView.Columns.Add(gridColumn)
                    End If
                Next

                uscFascicleGrid.CurrentMetadataModel = MetadataModel
                uscFascicleGrid.CurrentMetadataColumns = MetadataColumns
            End If
            uscFascicleGrid.Grid.Rebind()
        Else
            uscFascicleGrid.Grid.DataBindFinder()
        End If

        uscFascicleGrid.Grid.Visible = True
    End Sub

    Private Function ImpersonateGridCallback(Of TResult)(finder As IFinder, callback As Func(Of TResult)) As TResult
        Return WebAPIImpersonatorFacade.ImpersonateFinder(Of FascicleFinder, TResult)(finder,
                        Function(impersonationType, wfinder)
                            Return callback()
                        End Function)
    End Function

    Private Sub RefreshDynamicMetadataColumns()
        uscFascicleGrid.CurrentMetadataModel = MetadataModel
        uscFascicleGrid.CurrentMetadataColumns = MetadataColumns
        If MetadataColumns IsNot Nothing AndAlso MetadataColumns.Any() Then
            For Each metadataColumn As BaseFieldModel In MetadataColumns
                Dim column As GridColumn = uscFascicleGrid.Grid.Columns.FindByUniqueNameSafe(metadataColumn.KeyName.Replace(" ", ""))
                If column IsNot Nothing AndAlso TypeOf column Is GridBoundColumn Then
                    Dim gridColumn As GridBoundColumn = CType(column, GridBoundColumn)
                    gridColumn.HeaderText = metadataColumn.Label
                    gridColumn.DataField = metadataColumn.KeyName
                End If
            Next
        End If
    End Sub

    Private Function CreateExportableTable(gridColumns As GridColumnCollection, metadataColumns As GridColumnCollection, gridItems As GridDataItemCollection) As Table
        Dim customTable As DSTable = New DSTable()
        customTable.Table.Style.Add("border-collapse", "collapse")
        customTable.CreateEmptyRow()

        ' Adding existing columns from grid
        Dim columnUniqueNames As ICollection(Of String) = New List(Of String)
        Dim cellHeaderStyle As DSTableCellStyle
        For Each column As GridColumn In gridColumns
            cellHeaderStyle = New DSTableCellStyle()
            cellHeaderStyle.Font.Bold = True
            cellHeaderStyle.LineBox = True

            customTable.CurrentRow.CreateEmpytCell()
            customTable.CurrentRow.CurrentCell.Text = column.HeaderText
            customTable.CurrentRow.CurrentCell.ApplyStyle(cellHeaderStyle)
            columnUniqueNames.Add(column.UniqueName)
        Next

        ' Adding metadata columns from metadata repository
        For Each column As GridColumn In metadataColumns
            cellHeaderStyle = New DSTableCellStyle()
            cellHeaderStyle.Font.Bold = True
            cellHeaderStyle.LineBox = True

            customTable.CurrentRow.CreateEmpytCell()
            customTable.CurrentRow.CurrentCell.Text = column.HeaderText
            customTable.CurrentRow.CurrentCell.ApplyStyle(cellHeaderStyle)
            columnUniqueNames.Add(column.UniqueName)
        Next

        Dim cellItemStyle As DSTableCellStyle = New DSTableCellStyle()
        cellItemStyle.LineBox = True
        Dim currentGridColumn As GridColumn
        Dim metadataSource As FascicleModel
        Dim metadataValues As List(Of MetadataValueModel)
        Dim metadataMatchExpression As Func(Of MetadataValueModel, Integer, Boolean) = Function(x, i) columnUniqueNames.ElementAt(i).Eq(x.KeyName?.Replace(" ", ""))
        Dim currentMetadataValue As MetadataValueModel
        For Each gridDataItem As GridDataItem In gridItems
            customTable.CreateEmptyRow()
            For i As Integer = 0 To customTable.Rows(0).Cells.Count - 1
                customTable.CurrentRow.CreateEmpytCell()
                customTable.CurrentRow.CurrentCell.ApplyStyle(cellItemStyle)

                currentGridColumn = gridColumns.FindByUniqueNameSafe(columnUniqueNames.ElementAt(i))
                If (currentGridColumn IsNot Nothing) Then
                    customTable.CurrentRow.CurrentCell.Text = CellToString(gridDataItem.Cells.Item(currentGridColumn.OrderIndex))
                Else
                    metadataSource = DirectCast(gridDataItem.DataItem, WebAPIDto(Of FascicleModel)).Entity
                    If (metadataSource IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(metadataSource.MetadataValues)) Then
                        metadataValues = JsonConvert.DeserializeObject(Of List(Of MetadataValueModel))(metadataSource.MetadataValues)
                        If metadataValues.Any(Function(x) metadataMatchExpression(x, i)) Then
                            currentMetadataValue = metadataValues.Single(Function(x) metadataMatchExpression(x, i))
                            currentGridColumn = metadataColumns.FindByUniqueName(currentMetadataValue.KeyName?.Replace(" ", ""))
                            customTable.CurrentRow.CurrentCell.Text = FormatExportedMetadataValue(currentGridColumn, currentMetadataValue)
                        End If
                    End If
                End If
            Next
        Next
        Return customTable.Table
    End Function

    Private Function CellToString(cell As TableCell) As String
        Dim retval As String = String.Empty

        If cell.Controls.Count > 1 Then

            Select Case cell.Controls(1).[GetType]().ToString()
                Case "System.Web.UI.WebControls.Image",
                     "System.Web.UI.WebControls.ImageButton"
                    retval = ExportHelper.ImageToString(ReflectionHelper.GetProperty(cell.Controls(1), "ImageUrl").ToString())
                Case Else
                    retval = ReflectionHelper.GetProperty(cell.Controls(1), "Text").ToString()
            End Select
        ElseIf cell.Controls IsNot Nothing AndAlso cell.Controls.Count > 0 AndAlso cell.Controls(0).[GetType]().ToString().Equals("System.Web.UI.DataBoundLiteralControl") Then
            retval = (CType(cell.Controls(0), DataBoundLiteralControl)).Text
        Else
            retval = cell.Text
        End If

        Return retval
    End Function

    Private Function FormatExportedMetadataValue(gridColumn As GridColumn, metadataValue As MetadataValueModel) As String
        If (String.IsNullOrWhiteSpace(metadataValue.Value)) Then
            Return String.Empty
        End If

        If (gridColumn.GetType() Is GetType(GridBoundColumn)) Then
            Dim gridBoundColumn As GridBoundColumn = DirectCast(gridColumn, GridBoundColumn)
            If (Not String.IsNullOrWhiteSpace(gridBoundColumn.DataFormatString) AndAlso gridBoundColumn.DataFormatString.Eq(NameOf(MetadataDesignerModel.BoolFields))) Then
                Return If(metadataValue.Value.ToLower().Equals("true"), "Vero", "Falso")
            End If

            Dim data As Date
            If (Not String.IsNullOrWhiteSpace(gridBoundColumn.DataFormatString) AndAlso gridBoundColumn.DataFormatString.Eq(NameOf(MetadataDesignerModel.DateFields)) _
                AndAlso DateTime.TryParseExact(metadataValue.Value, "yyyy-MM-dd", Globalization.CultureInfo.CurrentCulture, Globalization.DateTimeStyles.None, data)) Then
                Return data.ToString("dd/MM/yyyy")
            End If
        End If
        Return metadataValue.Value
    End Function
#End Region

End Class