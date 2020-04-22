Imports System.Linq
Imports System.Web
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.IO

Public Class TaskHeaderGrid
    Inherits CommonBasePage

#Region " Properties "

    Private ReadOnly Property TaskType As TaskTypeEnum
        Get
            Return CType([Enum].Parse(GetType(TaskTypeEnum), Request("TaskType")), TaskTypeEnum)
        End Get
    End Property

    Public Property ColumnSendingProcessStatusVisible() As Boolean
        Get
            Return dgTaskHeaders.Columns.FindByUniqueName(NameOf(TaskHeader.SendingProcessStatus)).Visible
        End Get
        Set(value As Boolean)
            dgTaskHeaders.Columns.FindByUniqueName(NameOf(TaskHeader.SendingProcessStatus)).Visible = value
        End Set
    End Property

    Public Property ColumnSendedStatusVisible() As Boolean
        Get
            Return dgTaskHeaders.Columns.FindByUniqueName(NameOf(TaskHeader.SendedStatus)).Visible
        End Get
        Set(value As Boolean)
            dgTaskHeaders.Columns.FindByUniqueName(NameOf(TaskHeader.SendedStatus)).Visible = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()

        MasterDocSuite.Title = "Elenco Task"

        If Not IsPostBack Then

            Initialize()
            Dim script As String = String.Concat(
                "var id_cmdViewProtocols = '#", cmdViewProtocols.ClientID, "';",
                "var id_cmdViewPECMails = '#", cmdViewPECMails.ClientID, "';",
                "var id_cmdViewPOLRequests = '#", cmdViewPOLRequests.ClientID, "';")
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "id_dyn_cmds", script, True)
            Dim finder As New TaskHeaderFinder

            finder.TaskType = Me.TaskType

            finder.SortExpressions.Add("RegistrationDate", "DESC")

            dgTaskHeaders.Finder = finder
            dgTaskHeaders.CurrentPageIndex = 0
            dgTaskHeaders.CustomPageIndex = 0
            dgTaskHeaders.PageSize = dgTaskHeaders.Finder.PageSize
            dgTaskHeaders.DataBindFinder()
        End If
    End Sub

    Private Sub dgTaskHeadersInit(sender As Object, e As EventArgs) Handles dgTaskHeaders.Init
        With DirectCast(dgTaskHeaders.Columns.FindByUniqueNameSafe("Status"), SuggestFilteringColumn)
            .DataSourceCombo = GetType(TaskStatusEnum).EnumToDescription()
            .DataTextCombo = "Value"
            .DataFieldCombo = "Key"
            .DataType = GetType(TaskStatusEnum)
        End With

        With DirectCast(dgTaskHeaders.Columns.FindByUniqueNameSafe(NameOf(TaskHeader.SendingProcessStatus)), SuggestFilteringColumn)
            .DataSourceCombo = GetType(TaskHeaderSendingProcessStatus).EnumToDescription()
            .DataTextCombo = "Value"
            .DataFieldCombo = "Key"
            .DataType = GetType(TaskHeaderSendingProcessStatus)
        End With

        With DirectCast(dgTaskHeaders.Columns.FindByUniqueNameSafe(NameOf(TaskHeader.SendedStatus)), SuggestFilteringColumn)
            .DataSourceCombo = GetType(TaskHeaderSendedStatus).EnumToDescription()
            .DataTextCombo = "Value"
            .DataFieldCombo = "Key"
            .DataType = GetType(TaskHeaderSendedStatus)
        End With
    End Sub

    Private Sub dgTaskHeaders_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles dgTaskHeaders.ItemCommand
        Select Case e.CommandName
            Case "ViewSummary"
                Select Case Me.TaskType
                    Case TaskTypeEnum.FastProtocolSender
                        Return
                    Case Else
                        Response.RedirectLocation = "parent"
                        Response.Redirect("../Task/TaskHeaderViewer.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Series&IdTaskHeader={0}", e.CommandArgument)))
                End Select
        End Select
    End Sub

    Private Sub dgTaskHeaders_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles dgTaskHeaders.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim bound As TaskHeader = DirectCast(e.Item.DataItem, TaskHeader)

        Dim cmdSummary As ImageButton = DirectCast(e.Item.FindControl("cmdViewSummary"), ImageButton)
        Dim cmdName As String = cmdSummary.CommandName
        cmdSummary.CommandName = String.Empty
        cmdSummary.Enabled = False
        If TaskType <> TaskTypeEnum.FastProtocolSender Then
            FixCursorOver(cmdSummary)
            cmdSummary.CommandArgument = bound.Id.ToString()
            cmdSummary.CommandName = cmdName
            cmdSummary.Enabled = True
        End If
        Select Case bound.Status
            Case TaskStatusEnum.Done
                cmdSummary.ImageUrl = ImagePath.SmallFlagGreen
            Case TaskStatusEnum.DoneWithErrors, TaskStatusEnum.DoneWithWarnings
                cmdSummary.ImageUrl = ImagePath.SmallFlagYellow
            Case TaskStatusEnum.OnError
                cmdSummary.ImageUrl = ImagePath.SmallFlagRed
            Case TaskStatusEnum.Queued
                cmdSummary.ImageUrl = ImagePath.SmallFlagBlue
        End Select

        If bound.Status.Equals(TaskStatusEnum.DoneWithErrors) OrElse bound.Status.Equals(TaskStatusEnum.DoneWithWarnings) OrElse bound.Status.Equals(TaskStatusEnum.OnError) Then
            Dim fileRecovery As String = String.Concat(ProtocolEnv.ImportDocumentSeriesRecoveryFolder, bound.Id, "_Recovery.xls")
            If (Not File.Exists(fileRecovery)) Then
                With DirectCast(e.Item.FindControl("BtnExcelRecovery"), HyperLink)
                    .Enabled = False
                End With
            Else
                With DirectCast(e.Item.FindControl("BtnExcelRecovery"), HyperLink)
                    .NavigateUrl = String.Concat(ProtocolEnv.ImportDocumentSeriesRecoveryFolder, bound.Id, "_Recovery.xls")
                    .Enabled = True
                End With

            End If
        End If

        If TaskType = TaskTypeEnum.FastProtocolSender Then
            DirectCast(e.Item, GridDataItem)(NameOf(TaskHeader.SendingProcessStatus)).Text = bound.SendingProcessStatus.GetDescription()
            DirectCast(e.Item, GridDataItem)(NameOf(TaskHeader.SendedStatus)).Text = bound.SendedStatus?.GetDescription()
            If bound.SendingProcessStatus <> TaskHeaderSendingProcessStatus.Complete Then
                DirectCast(e.Item, GridDataItem)(NameOf(TaskHeader.SendedStatus)).Text = String.Empty
            End If
        End If

        DirectCast(e.Item, GridDataItem)("Status").Text = bound.Status.GetDescription()
    End Sub

    Private Sub cmdNew_Click(sender As Object, e As EventArgs) Handles cmdNew.Click
        Select Case TaskType
            Case TaskTypeEnum.ImportExcelToAVCPAcquisti, TaskTypeEnum.ImportExcelToAVCPPagamenti
                Response.RedirectLocation = "parent"
                Response.Redirect("../Series/AvcpImport.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Series&TaskType={0}", Request("TaskType"))))
        End Select
    End Sub

    Private Sub cmdViewProtocols_Click(sender As Object, e As EventArgs) Handles cmdViewProtocols.Click
        Dim url As String = "../Prot/ProtRisultati.aspx?Type=Prot&CustomFinderType={0}"
        url = String.Format(url, SessionSearchController.SessionFinderType.FastProtocolSender)
        Dim finder As New ProtocolTaskHeaderFinder()
        finder.TaskHeaderIdIn = Me.GetSelectedKeyValues()
        SessionSearchController.SaveSessionFinder(finder, SessionSearchController.SessionFinderType.FastProtocolSender)
        ClearSessions(Of ProtRisultati)()

        Response.Redirect(url)
    End Sub

    Private Sub cmdViewPECMails_Click(sender As Object, e As EventArgs) Handles cmdViewPECMails.Click
        Dim url As String = String.Format("../PEC/PecOutgoingMails.aspx?Type=PEC&CustomFinderType={0}&IsMassive=true", SessionSearchController.SessionFinderType.FastPecSender)
        Dim finder As New NHibernatePECMailFinder()
        finder.TaskHeaderIdIn = GetSelectedKeyValues()
        finder.Direction = PECMailDirection.Outgoing
        finder.Actives = True
        Dim result As List(Of PECMailBox) = Facade.PECMailboxFacade.GetVisibleMailBoxes(False)
        If result IsNot Nothing AndAlso result.Count > 0 Then
            finder.MailboxIds = result.Select(Function(f) f.Id).ToArray()
        End If

        SessionSearchController.SaveSessionFinder(finder, SessionSearchController.SessionFinderType.FastPecSender)

        Response.Redirect(url)
    End Sub

    Private Sub cmdViewPOLRequests_Click(sender As Object, e As EventArgs) Handles cmdViewPOLRequests.Click
        Dim url As String = "../Prot/PosteWebRicerca.aspx?Type=Prot&CustomFinderType={0}"
        url = String.Format(url, SessionSearchController.SessionFinderType.FastPOLSender)
        Dim finder As New NHPosteOnlineRequestFinder()
        finder.TaskHeaderIdIn = Me.GetSelectedKeyValues()
        SessionSearchController.SaveSessionFinder(finder, SessionSearchController.SessionFinderType.FastPOLSender)
        ClearSessions(Of ProtRisultati)()

        Response.Redirect(url)
    End Sub

    Protected Sub cmdReset_Click(sender As Object, e As EventArgs) Handles cmdReset.Click
        Select Case TaskType
            Case TaskTypeEnum.DocSeriesImporter
                For Each id As Integer In GetSelectedKeyValues()
                    FacadeFactory.Instance.TaskHeaderFacade.Reset(id)
                Next
                dgTaskHeaders.DataBindFinder()
        End Select

    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        ' FG20141030: Impostare a False la property AllowRowSelect fa sollevare alla RadGrid tutta una serie di eccezioni legate alla presenza o meno della GridClientSelectColumn.
        Me.dgTaskHeaders.ClientSettings.Selecting.AllowRowSelect = True

        Select Case TaskType
            Case TaskTypeEnum.ImportExcelToAVCPAcquisti, TaskTypeEnum.ImportExcelToAVCPPagamenti
                Me.dgTaskHeaders.Columns.FindByUniqueName("Select").Visible = False
                Me.cmdNew.Visible = True
                Me.cmdNew.Enabled = CommonShared.UserConnectedBelongsTo(ProtocolEnv.AVCPImporterGroup)

            Case TaskTypeEnum.FastProtocolSender
                Me.dgTaskHeaders.AllowMultiRowSelection = True

                Me.cmdViewProtocols.Visible = True
                Me.cmdViewPECMails.Visible = True
                Me.cmdViewPOLRequests.Visible = True

                ColumnSendedStatusVisible = True
                ColumnSendingProcessStatusVisible = True

            Case TaskTypeEnum.DocSeriesImporter
                Me.dgTaskHeaders.AllowMultiRowSelection = True

            Case Else
        End Select
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgTaskHeaders, dgTaskHeaders, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdNew, dgTaskHeaders, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdNew, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdReset, dgTaskHeaders, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdReset, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Protected Function GetSelectedKeyValues() As List(Of Integer)
        Dim selectedItems As GridDataItem() = dgTaskHeaders.MasterTableView.GetSelectedItems()
        Dim keyValues As IEnumerable(Of Object) = selectedItems.Select(Function(i) i.GetDataKeyValue("Id"))
        Return keyValues.Cast(Of Integer).ToList()
    End Function

    Private Shared Sub FixCursorOver(target As ImageButton)
        target.Attributes.Add("onmouseover", "this.style.cursor='hand';")
        target.Attributes.Add("onmouseout", "this.style.cursor='default';")
    End Sub

#End Region

End Class