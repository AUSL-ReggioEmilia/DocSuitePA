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

Public Class PECMailBoxCaselleMonitor
    Inherits PECBasePage
#Region " Fields "

#End Region

#Region " Properties "



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
        AjaxManager.AjaxSettings.AddAjaxSetting(grdInfoPecMail, grdInfoPecMail, MasterDocSuite.AjaxDefaultLoadingPanel)

    End Sub
    Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        DataBindMailGrid()
    End Sub

    Private Sub grdModuliPec_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdInfoPecMail.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim item As PECMailBoxPECReportDto = DirectCast(e.Item.DataItem, PECMailBoxPECReportDto)

        If (item Is Nothing) Then
            Return
        End If

        If ProtocolEnv.JeepServiceModuleWarningDaysThreshold > 0 AndAlso (Not item.LastJSProcessed.HasValue OrElse (item.LastJSProcessed.HasValue AndAlso (TimeSpan.FromTicks(DateTime.Now.Ticks).TotalDays - TimeSpan.FromTicks(item.LastJSProcessed.Value.Ticks).TotalDays) >= ProtocolEnv.JeepServiceModuleWarningDaysThreshold)) Then
            e.Item.BackColor = Drawing.Color.Yellow
        End If
        Dim lblCasellaPec As Label = DirectCast(e.Item.FindControl("lblCasellaPec"), Label)
        lblCasellaPec.Text = item.MailBoxName

        Dim lblLastProcess As Label = DirectCast(e.Item.FindControl("lblLastProcess"), Label)
        lblLastProcess.Text = If(item.LastJSProcessed.HasValue, String.Concat(item.LastJSProcessed.Value.ToShortDateString(), " ", item.LastJSProcessed.Value.ToLongTimeString()), String.Empty)

        Dim lblLastProcessNoErrors As Label = DirectCast(e.Item.FindControl("lblLastProcessNoErrors"), Label)
        lblLastProcessNoErrors.Text = If(item.LastJSErrorProcessed.HasValue, String.Concat(item.LastJSErrorProcessed.Value.ToShortDateString(), " ", item.LastJSErrorProcessed.Value.ToLongTimeString()), String.Empty)

        Dim lblPECInDrop As Label = DirectCast(e.Item.FindControl("lblPECInDrop"), Label)
        lblPECInDrop.Text = item.PECInDropCount

        Dim lblPECInError As Label = DirectCast(e.Item.FindControl("lblPECInError"), Label)
        lblPECInError.Text = item.PECErrorCount


    End Sub
#End Region

#Region " Methods "
    Private Sub DataBindMailGrid()
        grdJeepServiceHostDatabind()
    End Sub

    Private Sub grdJeepServiceHostDatabind()
        Dim PECMailBoxs As IList(Of PECMailBox) = FacadeFactory.Instance.PECMailboxFacade.GetAll()
        Dim lastRecordsWithoutError As IList(Of PECMailBoxReportDateDto) = FacadeFactory.Instance.PECMailboxLogFacade.GetLastRecordsWithoutError()
        Dim lastRecords As IList(Of PECMailBoxReportDateDto) = FacadeFactory.Instance.PECMailboxLogFacade.GetLastRecords()

        Dim lastRecordWithoutError As PECMailBoxReportDateDto
        Dim lastRecord As PECMailBoxReportDateDto
        Dim results As IList(Of PECMailBoxPECReportDto) = New List(Of PECMailBoxPECReportDto)()
        Dim report As PECMailBoxPECReportDto
        Dim finder As NHibernatePECMailBoxLogFinder
        Dim logResults As IList(Of VecompSoftware.DocSuiteWeb.Data.PECMailBoxLog)

        Dim lastLog As VecompSoftware.DocSuiteWeb.Data.PECMailBoxLog
        Dim eval As Double

        For Each mailbox As PECMailBox In PECMailBoxs

            lastRecord = lastRecords.SingleOrDefault(Function(f) f.Id = mailbox.Id)
            lastRecordWithoutError = lastRecordsWithoutError.SingleOrDefault(Function(f) f.Id = mailbox.Id)

            report = New PECMailBoxPECReportDto()
            report.MailBoxName = mailbox.MailBoxName
            report.LastJSProcessed = Nothing
            report.LastJSErrorProcessed = Nothing
            report.PECErrorCount = 0
            report.PECInDropCount = 0
            lastLog = Nothing

            finder = New NHibernatePECMailBoxLogFinder()
            finder.EnablePaging = False
            finder.ExcludeJSEvalActivities = False
            finder.IncludeJSEvalActivities = True
            finder.ExplicitDateTime = If(lastRecord IsNot Nothing, lastRecord.Date, DateTime.Today)
            finder.MailboxIds = New Short() {mailbox.Id}

            logResults = finder.DoSearch().OrderByDescending(Function(f) f.Date).ToList()

            lastLog = logResults.FirstOrDefault(Function(f) f.Type = PECMailBoxLogFacade.PecMailBoxLogType.TimeEval.ToString())
            If (lastLog IsNot Nothing) Then
                report.LastJSProcessed = lastLog.Date
            End If

            lastLog = logResults.FirstOrDefault(Function(f) f.Type = PECMailBoxLogFacade.PecMailBoxLogType.PECErrorEval.ToString())
            If (lastLog IsNot Nothing) Then
                eval = 0
                Double.TryParse(lastLog.Description, eval)
                report.PECErrorCount = eval
            End If

            lastLog = logResults.FirstOrDefault(Function(f) f.Type = PECMailBoxLogFacade.PecMailBoxLogType.PECReadedEval.ToString())
            If (lastLog IsNot Nothing) Then
                eval = 0
                Double.TryParse(lastLog.Description, eval)
                report.PECInDropCount = eval
            End If

            If (lastRecordWithoutError IsNot Nothing) Then
                report.LastJSErrorProcessed = lastRecordWithoutError.Date
            End If
            results.Add(report)
        Next

        grdInfoPecMail.DataSource = results
        grdInfoPecMail.DataBind()
    End Sub

#End Region

End Class

