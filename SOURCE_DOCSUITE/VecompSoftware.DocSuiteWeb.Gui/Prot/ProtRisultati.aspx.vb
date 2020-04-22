Imports System.Collections.Generic
Imports System.IO
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Report
Imports VecompSoftware.DocSuiteWeb.Facade.Report
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class ProtRisultati
    Inherits ProtBasePage

#Region " Properties "

    Private ReadOnly Property Message() As String
        Get
            Return Request.QueryString("Message")
        End Get
    End Property

    Private ReadOnly Property SessionFinderType As SessionSearchController.SessionFinderType
        Get
            Return Request.QueryString.GetValueOrDefault(Of SessionSearchController.SessionFinderType)("CustomFinderType", SessionSearchController.SessionFinderType.ProtFinderType)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Action.Eq("Fasc") OrElse Action.Eq("CopyProtocolDocuments") Then
            MasterDocSuite.TitleVisible = False
        End If
        AddHandler uscProtocolGrid.Grid.DataBound, AddressOf DataSourceChanged
        InitializeAjaxSettings()

        Select Case Action
            Case "Docm", "Resl", "Fasc", "CopyProtocolDocuments"
                uscProtocolGridBar.Hide()
                ReportButtons.Visible = False
                uscProtocolGrid.ColumnClientSelectVisible = False
            Case Else
                uscProtocolGrid.ColumnProtocolContactVisible = DocSuiteContext.Current.ProtocolEnv.ContactToProtocolGridVisible
                uscProtocolGrid.ColumnProtocolRegistrationUserVisible = DocSuiteContext.Current.ProtocolEnv.RegistrationUserToProtocolGridVisible
                uscProtocolGridBar.Grid = uscProtocolGrid.Grid
                uscProtocolGridBar.AjaxEnabled = True
                uscProtocolGridBar.AjaxLoadingPanel = MasterDocSuite.AjaxDefaultLoadingPanel
                uscProtocolGridBar.Show()
        End Select

        If Action <> "Resl" Then
            InitReportButtons()
        End If

        If Not IsPostBack Then
            If Action.Eq("RequiredField") Then
                AjaxAlert("Dettagliare filtri:{0}Specificare: {1}", Environment.NewLine, Message)
                Return
            End If
        End If
    End Sub

    Protected Sub ProtRisultatiAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument.Replace("~", "'"), "|", 2)
        Select Case arguments(0)
            Case "InitialPageLoad"
                DoSearch()
        End Select
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvProtocols As BindGrid = uscProtocolGrid.Grid
        If gvProtocols.DataSource IsNot Nothing Then
            Title = String.Format("Protocollo - Risultati ({0}/{1})", gvProtocols.DataSource.Count, gvProtocols.VirtualItemCount)
        Else
            Title = "Protocollo - Nessun Risultato"
        End If
        MasterDocSuite.Title = Title
        MasterDocSuite.HistoryTitle = Title
        AjaxManager.ResponseScripts.Add("if(GetRadWindow()) {GetRadWindow().set_title(""" & Title & """);}")
    End Sub

    Private Sub BtnRicevutaClick(sender As Object, e As EventArgs)
        Dim senderButton As Button = DirectCast(sender, Button)

        Dim finder As NHibernateProtocolFinder = CType(uscProtocolGrid.Grid.Finder, NHibernateProtocolFinder)
        If Not ReportCurrentPage.Checked AndAlso finder.Count > DocSuiteContext.Current.ProtocolEnv.ReportRowCountLimit Then
            AjaxAlert("Il numero dei record è troppo alto per l'esportazione.")
            Return
        End If

        finder.EnablePaging = ReportCurrentPage.Checked
        finder.LoadFetchModeFascicleEnabled = False
        finder.LoadFetchModeProtocolLogs = False
        Dim reportName As String = senderButton.CommandArgument

        Dim protocols As IList(Of Protocol) = finder.DoSearch()
        Dim report As IReport(Of Protocol) = ReportFacade.GenerateReport(Of Protocol)(reportName, New Dictionary(Of String, String) From {{"Emergenza", CType(True, String)}}, protocols)

        Dim doc As DocumentInfo = report.ExportExcel()

        Dim file As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
        Dim temp As New TempFileDocumentInfo(file)

        Dim url As String = ResolveUrl("~/Viewers/DocumentInfoViewer.aspx?" & CommonShared.AppendSecurityCheck(temp.ToQueryString().AsEncodedQueryString()))

        Server.Transfer(url)
    End Sub
#End Region

#Region " Methods "

    Private Sub InitReportButtons()
        If Not DocSuiteContext.Current.ProtocolEnv.ReportLibraryEnabled OrElse DocSuiteContext.Current.ProtocolEnv.DisableProtRisultatiReportButtons Then
            Exit Sub
        End If

        ReportCurrentPage.Visible = True
        Dim fullPath As String = If(Directory.Exists(ProtocolEnv.ReportLibraryPath), ProtocolEnv.ReportLibraryPath, Server.MapPath(ProtocolEnv.ReportLibraryPath))
        ' Carico i pulsanti
        For Each report As String In Directory.GetFiles(fullPath, "REPORTGRID_*.rdlc")
            Dim name As String = Path.GetFileNameWithoutExtension(report)
            Dim btn As New Button
            btn.Text = name.Split({"_"c})(1)
            btn.CommandArgument = Path.GetFileName(report)
            btn.Width = New Unit(120, UnitType.Pixel)
            ReportButtons.Controls.Add(btn)
            AddHandler btn.Click, AddressOf BtnRicevutaClick
        Next
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.EnablePageHeadUpdate = True
        AddHandler AjaxManager.AjaxRequest, AddressOf ProtRisultatiAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGridBar.ExportButton, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub DoSearch()
        If uscProtocolGrid.Grid.Finder Is Nothing Then
            Dim finder As NHibernateProtocolFinder = CType(SessionSearchController.LoadSessionFinder(SessionFinderType), NHibernateProtocolFinder)
            uscProtocolGrid.Grid.Finder = finder
        End If

        BindLogData(uscProtocolGrid.Grid.Finder)
        uscProtocolGrid.Grid.PageSize = uscProtocolGrid.Grid.Finder.PageSize

        uscProtocolGrid.Grid.MasterTableView.SortExpressions.Clear()
        ' Per la distribuzione ENPACL l'ordinamento di default è la data di registrazione
        If DocSuiteContext.Current.ProtocolEnv.CorporateAcronym.Contains("ENPACL") Then
            uscProtocolGrid.Grid.MasterTableView.SortExpressions.AddSortExpression("RegistrationDate ASC")
            uscProtocolGrid.Grid.MasterTableView.AllowMultiColumnSorting = True
        End If

        FileLogger.SetLogicalThreadProperty("ProtocolSearch.Start", Date.Now.ToString()) ' Salvo l'inizio della QRY

        uscProtocolGrid.Grid.DataBindFinder()

        FileLogger.SetLogicalThreadProperty("ProtocolSearch.Count", Convert.ToString(uscProtocolGrid.Grid.VirtualItemCount)) ' Salvo il numero dei record trovati
        FileLogger.SetLogicalThreadProperty("ProtocolSearch.End", DateTime.Now.ToString()) ' Salvo l'inizio della QRY

        uscProtocolGrid.Grid.Visible = True
        uscProtocolGrid.Grid.MasterTableView.AllowMultiColumnSorting = False

        ' Scrivo la riga di LOG
        FileLogger.Info("ProtocolSearch", "Risultati ricerca")
    End Sub

    Private Sub BindLogData(finder As NHibernateProtocolFinder)
        Try
            FileLogger.SetLogicalThreadProperty("DocSuiteWeb.UserConnected", DocSuiteContext.Current.User.FullUserName)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.Year", finder.Year)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.Number", finder.Number)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.RegistrationDateFrom", finder.RegistrationDateFrom)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.RegistrationDateTo", finder.RegistrationDateTo)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.ProtocolNotReaded", finder.ProtocolNotReaded)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.IdTypes", finder.IdTypes)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.IdLocation", finder.IdLocation)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.IdContainer", finder.IdContainer)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.DocumentDateFrom", finder.DocumentDateFrom)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.DocumentDateTo", finder.DocumentDateTo)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.DocumentProtocol", finder.DocumentProtocol)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.DocumentName", finder.DocumentName)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.ProtocolObject", finder.ProtocolObject)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.ProtocolObjectSearch", finder.ProtocolObjectSearch.ToString())
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.Note", finder.Note)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.Recipient", finder.Recipient)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.EnableRecipientContains", finder.EnableRecipientContains)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.Subject", finder.Subject)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.ServiceCategory", finder.ServiceCategory)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.Classifications", finder.Classifications)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.IncludeChildClassifications", finder.IncludeChildClassifications)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.ProtocolStatusCancel", finder.ProtocolStatusCancel)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.IncludeIncomplete", finder.IncludeIncomplete)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.ProtocolNoRoles", finder.ProtocolNoRoles)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.RegistrationUser", finder.RegistrationUser)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.IdDocType", finder.IdDocType)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.IsClaim", finder.IsClaim)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.EnableInvoiceSearch", finder.EnableInvoiceSearch)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.InvoiceNumber", finder.InvoiceNumber)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.InvoiceDateFrom", finder.InvoiceDateFrom)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.InvoiceDateTo", finder.InvoiceDateTo)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.AccountingSectional", finder.AccountingSectional)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.AccountingYear", finder.AccountingYear)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.AccountingNumber", finder.AccountingNumber)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.Contacts", finder.Contacts)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.IncludeChildContacts", finder.IncludeChildContacts)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.EnablePackageSearch", finder.EnablePackageSearch)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.PackageOrigin", finder.PackageOrigin)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.Package", finder.Package)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.PackageLot", finder.PackageLot)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.PackageIncremental", finder.PackageIncremental)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.AdvancedStatus", finder.AdvancedStatus)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.AssignedToMe", finder.AssignedToMe)
            FileLogger.SetLogicalThreadProperty("ProtocolSearch.AssignedToMeCC", finder.AssignedToMeCC)


        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in log ricerca: " + ex.Message, ex)
        End Try
    End Sub

#End Region

End Class