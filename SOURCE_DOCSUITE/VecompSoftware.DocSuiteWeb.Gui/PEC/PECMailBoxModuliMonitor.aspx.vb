Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.PECMails

Public Class PECMailBoxModuliMonitor
    Inherits PECBasePage

#Region " Fields "
    Private _currentJeepServiceHostFacade As JeepServiceHostFacade
#End Region

#Region " Properties "
    Public ReadOnly Property CurrentJeepServiceHostFacade As JeepServiceHostFacade
        Get
            If _currentJeepServiceHostFacade Is Nothing Then
                _currentJeepServiceHostFacade = New JeepServiceHostFacade()
            End If
            Return _currentJeepServiceHostFacade
        End Get
    End Property


#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            DataBindMailGrid()
        End If
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(grdModuliPec, grdModuliPec, MasterDocSuite.AjaxDefaultLoadingPanel)

    End Sub
    Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        DataBindMailGrid()
    End Sub

    Private Sub grdModuliPec_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdModuliPec.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim item As PECMailBoxModuleReportDto = DirectCast(e.Item.DataItem, PECMailBoxModuleReportDto)

        If (item Is Nothing) Then
            Return
        End If

        If ProtocolEnv.JeepServiceModuleWarningDaysThreshold > 0 AndAlso (Not item.LastJSProcessed.HasValue OrElse (item.LastJSProcessed.HasValue AndAlso (TimeSpan.FromTicks(DateTime.Now.Ticks).TotalDays - TimeSpan.FromTicks(item.LastJSProcessed.Value.Ticks).TotalDays) >= ProtocolEnv.JeepServiceModuleWarningDaysThreshold)) Then
            e.Item.BackColor = Drawing.Color.Yellow
        End If

        Dim lblHostname As Label = DirectCast(e.Item.FindControl("lblHostname"), Label)
        lblHostname.Text = item.ModuleName

        Dim lblCasellaPec As Label = DirectCast(e.Item.FindControl("lblCasellaPec"), Label)
        lblCasellaPec.Text = item.MailBoxName

        Dim lblLastProcess As Label = DirectCast(e.Item.FindControl("lblLastProcess"), Label)
        lblLastProcess.Text = If(item.LastJSProcessed.HasValue, String.Concat(item.LastJSProcessed.Value.ToShortDateString(), " ", item.LastJSProcessed.Value.ToLongTimeString()), String.Empty)

        Dim lblDurate As Label = DirectCast(e.Item.FindControl("lblDurate"), Label)
        lblDurate.Text = Convert.ToInt32(item.EvalTime).ToString()

        Dim lblEsito As Label = DirectCast(e.Item.FindControl("lblEsito"), Label)
        lblEsito.Text = item.Result

    End Sub
#End Region

#Region " Methods "
    Private Sub DataBindMailGrid()
        grdJeepServiceHostDatabind()
    End Sub

    Private Sub grdJeepServiceHostDatabind()
        Dim PECMailBoxs As IList(Of PECMailBox) = FacadeFactory.Instance.PECMailboxFacade.GetAll()
        Dim results As IList(Of PECMailBoxModuleReportDto) = New List(Of PECMailBoxModuleReportDto)()
        Dim lastRecords As IList(Of PECMailBoxReportDateDto) = FacadeFactory.Instance.PECMailboxLogFacade.GetLastRecords()
        Dim jeepServiceHosts As IList(Of JeepServiceHost) = CurrentJeepServiceHostFacade.GetAll()
        Dim lastRecord As PECMailBoxReportDateDto
        Dim report As PECMailBoxModuleReportDto
        Dim finder As NHibernatePECMailBoxLogFinder
        Dim defaultJS As JeepServiceHost = CurrentJeepServiceHostFacade.GetDefaultHost()
        Dim logResults As IList(Of VecompSoftware.DocSuiteWeb.Data.PECMailBoxLog)
        Dim lastLog As VecompSoftware.DocSuiteWeb.Data.PECMailBoxLog
        Dim eval As Double

        For Each mailbox As PECMailBox In PECMailBoxs

            lastRecord = lastRecords.SingleOrDefault(Function(f) f.Id = mailbox.Id)

            report = New PECMailBoxModuleReportDto()
            report.MailBoxName = mailbox.MailBoxName
            report.LastJSProcessed = Nothing

            If (Not mailbox.IdJeepServiceOutgoingHost.HasValue) Then
                report.ModuleName = defaultJS.Hostname
            Else
                report.ModuleName = jeepServiceHosts.Single(Function(f) f.Id = mailbox.IdJeepServiceOutgoingHost.Value).Hostname
            End If

            finder = New NHibernatePECMailBoxLogFinder()
            finder.EnablePaging = False
            finder.ExcludeJSEvalActivities = False
            finder.IncludeJSEvalActivities = True
            finder.ExplicitDateTime = If(lastRecord IsNot Nothing, lastRecord.Date, DateTime.Today)
            finder.MailboxIds = New Short() {mailbox.Id}
            logResults = finder.DoSearch().OrderByDescending(Function(f) f.Date).ToList()

            lastLog = Nothing
            lastLog = logResults.FirstOrDefault(Function(f) f.Type = PECMailBoxLogFacade.PecMailBoxLogType.TimeEval.ToString())
            If (lastLog IsNot Nothing) Then
                report.LastJSProcessed = lastLog.Date
                eval = 0
                Double.TryParse(lastLog.Description, eval)
                report.EvalTime = eval
                lastLog = logResults.FirstOrDefault(Function(f) f.Type = PECMailBoxLogFacade.PecMailBoxLogType.ErrorEval.ToString() AndAlso f.Date = lastLog.Date)
                report.Result = "OK"
                If (lastLog IsNot Nothing) Then
                    report.Result = lastLog.Description
                End If
            End If
            results.Add(report)
        Next
        grdModuliPec.DataSource = results
        grdModuliPec.DataBind()
    End Sub

#End Region

End Class