Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports System.IO
Imports Telerik.Web.UI

Public Class TaskHeaderViewer
    Inherits CommonBasePage

    Private _taskHeader As TaskHeader

    Private ReadOnly Property TaskHeader As TaskHeader
        Get
            If _taskHeader Is Nothing Then
                Dim idTaskHeader As Integer = Request.QueryString.GetValue(Of Integer)("IdTaskHeader")
                _taskHeader = FacadeFactory.Instance.TaskHeaderFacade.GetById(idTaskHeader)
            End If
            Return _taskHeader
        End Get
    End Property

    Private ReadOnly Property TaskHeaderPath As String
        Get
            If Me.TaskHeader.Parameters.IsNullOrEmpty() Then
                Return Nothing
            End If

            Return TaskHeader.Parameters.First(Function(p) p.ParameterKey.Eq("FilePath")).Value
        End Get
    End Property

    Private ReadOnly Property TaskHeaderOriginalFileName As String
        Get
            If Me.TaskHeader.Parameters.IsNullOrEmpty() Then
                Return Nothing
            End If

            Return TaskHeader.Parameters.First(Function(p) p.ParameterKey.Eq("OriginalFileName")).Value
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjax()
        If Not IsPostBack Then
            LoadTaskHeader()
        End If
    End Sub

    Private Sub LoadTaskHeader()
        Dim finder As New TaskDetailFinder
        finder.PageSize = 20
        finder.TaskHeader = Me.TaskHeader

        finder.SortExpressions.Add("RegistrationDate", "ASC")

        dgTaskDetails.Finder = finder
        dgTaskDetails.CurrentPageIndex = 0
        dgTaskDetails.CustomPageIndex = 0
        dgTaskDetails.PageSize = dgTaskDetails.Finder.PageSize
        dgTaskDetails.DataBindFinder()

        lblCode.Text = TaskHeader.Code
        lblRegistrationDate.Text = String.Format("{0:dd/MM/yyyy HH:ss}", TaskHeader.RegistrationDate)
        lblStatus.Text = TaskHeader.Status.GetDescription()

        linkDocumenti.NavigateUrl = TaskHeaderPath
        linkDocumenti.Text = TaskHeaderOriginalFileName
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgTaskDetails, dgTaskDetails, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCancel, dgTaskDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRetry, dgTaskDetails, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCancel, tblHead, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRetry, tblHead, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCancel, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRetry, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub


    Private Sub dgTaskDetails_Init(sender As Object, e As EventArgs) Handles dgTaskDetails.Init
        With DirectCast(dgTaskDetails.Columns.FindByUniqueNameSafe("DetailType"), SuggestFilteringColumn)
            .DataSourceCombo = GetType(DetailTypeEnum).EnumToDescription()
            .DataTextCombo = "Value"
            .DataFieldCombo = "Key"
            .DataType = GetType(DetailTypeEnum)
        End With
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click

        Dim _taskType As TaskTypeEnum = TaskHeader.TaskType

        FacadeFactory.Instance.TaskHeaderFacade.Delete(TaskHeader)

        Response.RedirectLocation = "parent"
        Response.Redirect("../Task/TaskHeaderGrid.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Series&TaskType={0}", _taskType.ToString)))

    End Sub

    Private Sub cmdRetry_Click(sender As Object, e As EventArgs) Handles cmdRetry.Click
        TaskHeader.Status = TaskStatusEnum.Queued
        TaskHeader.Details.Clear()
        FacadeFactory.Instance.TaskHeaderFacade.Update(TaskHeader)

        LoadTaskHeader()
    End Sub

    Private Sub dgTaskDetails_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles dgTaskDetails.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim bound As TaskDetail = DirectCast(e.Item.DataItem, TaskDetail)
        e.Item.Cells(dgTaskDetails.Columns.FindByUniqueName("DetailType").OrderIndex).Text = bound.DetailType.GetDescription()
    End Sub
End Class