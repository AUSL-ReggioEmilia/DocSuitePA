Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Report
Imports VecompSoftware.DocSuiteWeb.Facade.Report
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Namespace Prot
    Public Class ProtConcorsiViewGrid
        Inherits CommonBasePage

#Region " Fields "
        Private _protocolContactDictionary As IDictionary(Of Guid, Contact)
        Private _gridDataSource As IList(Of ProtocolHeader)
#End Region

#Region " Properties "

        Private ReadOnly Property GridDataSource As IList(Of ProtocolHeader)
            Get
                If _gridDataSource Is Nothing Then
                    _gridDataSource = CType(grdConcourse.DataSource, IList(Of ProtocolHeader))
                End If
                Return _gridDataSource
            End Get
        End Property

        Private ReadOnly Property ProtocolContactDictionary As IDictionary(Of Guid, Contact)
            Get
                If _protocolContactDictionary Is Nothing Then
                    _protocolContactDictionary = Facade.ProtocolFacade.GetProtocolContactDictionary(GridDataSource)
                End If
                Return _protocolContactDictionary
            End Get
        End Property
#End Region

#Region " Events "
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            SetResponseNoCache()
            InitReportButtons()
            InitializeAjax()

            If Not IsPostBack Then
                grdConcourse.DiscardFinder()
                If PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is ProtConcorsiPrint Then
                    grdConcourse.Finder = CType(PreviousPage, ProtConcorsiPrint).RicercaConcorsiFinder()
                    grdConcourse.CurrentPageIndex = 0
                    grdConcourse.CustomPageIndex = 0
                    grdConcourse.PageSize = grdConcourse.Finder.PageSize
                    grdConcourse.DataBindFinder()
                    lblHeader.Text = String.Format("Protocollo - Candidati per il concorso ""{0}"" ({1})", CType(PreviousPage, ProtConcorsiPrint).GetConcourseName(), CType((grdConcourse.DataSource.Count & "/" & grdConcourse.VirtualItemCount), String))
                Else
                    FileLogger.Warn(LoggerName, "PreviousPage non coerente.")
                    Throw New DocSuiteException("Errore in fase di ricerca.")
                End If
            End If
        End Sub

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(grdConcourse, grdConcourse, MasterDocSuite.AjaxDefaultLoadingPanel)

            For Each item As RepeaterItem In ReportRepeater.Items
                If item.ItemType = ListItemType.Item OrElse item.ItemType = ListItemType.AlternatingItem Then
                    Dim btn As Button = DirectCast(item.FindControl("btnReport"), Button)
                    AjaxManager.AjaxSettings.AddAjaxSetting(btn, grdConcourse, MasterDocSuite.AjaxDefaultLoadingPanel)
                    AjaxManager.AjaxSettings.AddAjaxSetting(btn, btnPanel, MasterDocSuite.AjaxFlatLoadingPanel)
                End If
            Next

        End Sub

        Private Sub GrdProtocolsItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles grdConcourse.ItemDataBound
            If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
                Exit Sub
            End If

            Dim boundHeader As ProtocolHeader = DirectCast(e.Item.DataItem, ProtocolHeader)
            Dim hiddenId As String = String.Format("{0}|{1}|{2}", boundHeader.UniqueId, boundHeader.Year, boundHeader.Number)

            With DirectCast(e.Item.FindControl("lbtViewProtocol"), LinkButton)
                .Text = boundHeader.FullProtocolNumber
                .CommandArgument = hiddenId

                If RedirectOnParentPage Then
                    Dim parameters As String = String.Format("UniqueId={0&Type=Prot", boundHeader.UniqueId)
                    parameters = CommonShared.AppendSecurityCheck(parameters)

                    Dim parentPageUrl As String = $"~/Prot/ProtVisualizza.aspx?{parameters}"
                    Dim parentPageScript As String = grdConcourse.GetRedirectParentPageScript(parentPageUrl)

                    .OnClientClick = parentPageScript
                End If
            End With

            Dim currentContact As Contact = GetContactByHeader(boundHeader)
            If currentContact Is Nothing Then
                Exit Sub
            End If
            DirectCast(e.Item.FindControl("lblProtocolContactLastName"), Label).Text = currentContact.LastName
            DirectCast(e.Item.FindControl("lblProtocolContactFirstName"), Label).Text = currentContact.FirstName

            DirectCast(e.Item.FindControl("lblMittDestPEC"), Label).Text = currentContact.CertifiedMail
            DirectCast(e.Item.FindControl("lblMittDestTelefono"), Label).Text = currentContact.TelephoneNumber
            DirectCast(e.Item.FindControl("lblMittDestDataNascita"), Label).Text = String.Format("{0:dd/MM/yyyy}", currentContact.BirthDate)
            If currentContact.Address IsNot Nothing Then
                DirectCast(e.Item.FindControl("lblMittDestIndirizzo"), Label).Text = String.Format("{0} {1} {2}", If(currentContact.Address.PlaceName IsNot Nothing, currentContact.Address.PlaceName.Description, String.Empty), currentContact.Address.Address, currentContact.Address.CivicNumber)
                DirectCast(e.Item.FindControl("lblMittDestLocalita"), Label).Text = String.Format("{0} {1}", currentContact.Address.ZipCode, currentContact.Address.City)
                DirectCast(e.Item.FindControl("lblMittDestProvincia"), Label).Text = currentContact.Address.CityCode

            End If
        End Sub

        Private Sub GrdProtocolsItemCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles grdConcourse.ItemCommand
            Select Case e.CommandName
                Case "ViewProtocol"
                    Dim split As String() = e.CommandArgument.ToString().Split("|"c)
                    Dim protocolId As Guid = Guid.Parse(split(0))
                    Dim protocolYear As Short = Short.Parse(split(1))
                    Dim protocolNumber As Integer = Integer.Parse(split(2))

                    Select Case Action
                        Case "CopyProtocolDocuments", "Fasc", "Resl"
                            Dim script As String = String.Format("CloseWindow('{0}');", e.CommandArgument)
                            AjaxManager.ResponseScripts.Add(script)

                        Case Else
                            Dim parameters As String = $"UniqueId={protocolId}&Type=Prot"
                            parameters = CommonShared.AppendSecurityCheck(parameters)
                            RedirectOnPage($"~/Prot/ProtVisualizza.aspx?{parameters}")
                    End Select
            End Select
        End Sub

        Protected Sub BtnRicevutaClick(sender As Object, e As EventArgs)
            Dim senderButton As Button = DirectCast(sender, Button)

            Dim finder As NHibernateProtocolFinder = CType(grdConcourse.Finder, NHibernateProtocolFinder)
            If Not ReportCurrentPage.Checked AndAlso finder.Count > DocSuiteContext.Current.ProtocolEnv.ReportRowCountLimit Then
                AjaxAlert("Il numero dei record è troppo alto per l'esportazione.")
                Return
            End If

            finder.EnablePaging = ReportCurrentPage.Checked
            finder.LoadFetchModeProtocolLogs = False
            finder.LoadFetchModeFascicleEnabled = False
            Dim reportName As String = senderButton.CommandArgument
            Dim protocols As IList(Of Protocol) = finder.DoSearch()
            Dim report As IReport(Of Protocol) = ReportFacade.GenerateReport(Of Protocol)(reportName, Nothing, protocols)

            Dim doc As DocumentInfo = report.ExportExcel()

            Dim file As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
            Dim script As String = "window.open('" & DocSuiteContext.Current.CurrentTenant.DSWUrl & "/Temp/" & file.Name & "');"
            AjaxManager.ResponseScripts.Add(script)
        End Sub

        Private Sub ReportRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles ReportRepeater.ItemDataBound
            If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
                Exit Sub
            End If

            Dim report As String = DirectCast(e.Item.DataItem, String)
            Dim name As String = Path.GetFileNameWithoutExtension(report)
            Dim btn As Button = DirectCast(e.Item.FindControl("btnReport"), Button)
            btn.Text = name.Split("_".ToCharArray())(1)
            btn.CommandArgument = Path.GetFileName(report)
        End Sub
        Private Sub btnExcel_Click(sender As Object, e As EventArgs) Handles btnExcel.Click
            'Test esportazione in modo automatico
            Dim report As IReport(Of BindGrid) = ReportFacade.GenerateReport(Of BindGrid)(New List(Of BindGrid) From {grdConcourse}, (From column As GridColumn In grdConcourse.Columns Select New Column(column)).ToList())

            Dim doc As DocumentInfo = report.ExportExcel()

            Dim file As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
            Dim script As String = "window.open('" & DocSuiteContext.Current.CurrentTenant.DSWUrl & "/Temp/" & file.Name & "');"
            AjaxManager.ResponseScripts.Add(script)
        End Sub
#End Region

#Region " Methods "
        Private Function GetContactByHeader(protocolHeader As ProtocolHeader) As Contact
            If ProtocolContactDictionary IsNot Nothing AndAlso ProtocolContactDictionary.Keys.Contains(protocolHeader.UniqueId) Then
                Return ProtocolContactDictionary.Item(protocolHeader.UniqueId)
            Else
                Return Nothing
            End If
        End Function

        Private ReadOnly Property RedirectOnParentPage() As Boolean
            Get
                If ViewState.Item("_redirect") Is Nothing Then
                    Return False
                Else
                    Return CBool(ViewState.Item("_redirect"))
                End If
            End Get
        End Property

        Private Sub RedirectOnPage(ByVal pag As String)
            If RedirectOnParentPage Then
                Dim script As String = "parent.location='" & pag & "';"
                AjaxManager.ResponseScripts.Add(script)
            Else
                Response.RedirectLocation = "parent"
                Response.Redirect(pag)
            End If
        End Sub

        Private Sub InitReportButtons()
            If ProtocolEnv.ConcourseExcelExportDynamic Then
                ' Pulsanti esportazione
                btnExcel.Icon.SecondaryIconUrl = ImagePath.SmallExcel
                btnExcel.CommandName = "Export"
                btnExcel.CommandArgument = "Excel"
                btnExcel.ToolTip = "Esporta in Excel"
                btnExcel.CausesValidation = False
                btnExcel.Visible = True
            ElseIf ProtocolEnv.ReportLibraryEnabled Then
                ReportCurrentPage.Visible = True
                Dim fullPath As String = If(Directory.Exists(ProtocolEnv.ReportLibraryPath), ProtocolEnv.ReportLibraryPath, Server.MapPath(ProtocolEnv.ReportLibraryPath))
                ' Carico i pulsanti

                Dim reports() As String = Directory.GetFiles(fullPath, "CONCORSIGRID_*.rdlc")
                ReportRepeater.DataSource = reports
                ReportRepeater.DataBind()
            End If
        End Sub

#End Region
    End Class
End Namespace