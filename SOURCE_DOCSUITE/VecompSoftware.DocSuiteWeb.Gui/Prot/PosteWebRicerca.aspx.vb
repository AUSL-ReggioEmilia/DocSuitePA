Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade.Report
Imports VecompSoftware.DocSuiteWeb.Report
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Namespace Prot.PosteWeb
    Partial Public Class Ricerca
        Inherits ProtBasePage

#Region "Properties"
        Private _currentFinder As NHPosteOnlineRequestFinder

        Public ReadOnly Property CurrentFinder As NHPosteOnlineRequestFinder
            Get
                If _currentFinder Is Nothing Then
                    Dim obj As Object = SessionSearchController.LoadSessionFinder(SessionFinderType)
                    _currentFinder = If(obj IsNot Nothing, CType(obj, NHPosteOnlineRequestFinder), Facade.PosteOnlineRequestFinder)
                End If
                Return _currentFinder
            End Get
        End Property

        Private ReadOnly Property SessionFinderType As SessionSearchController.SessionFinderType
            Get
                Return Request.QueryString.GetValueOrDefault("CustomFinderType", SessionSearchController.SessionFinderType.PosteOnlineRequestFinder)
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            AjaxManager.AjaxSettings.AddAjaxSetting(dgPosteRequestContact, dgPosteRequestContact, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdPosteWebRefresh, dgPosteRequestContact, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnExcel, dgPosteRequestContact, MasterDocSuite.AjaxDefaultLoadingPanel)

            If Not IsPostBack Then
                Title = "Ricerca in Poste Online"
                Dim accounts As IList(Of POLAccount)
                If CommonShared.HasGroupsWithPosteWebAccountRestrictionNoneRight Then
                    accounts = Facade.PosteOnLineAccountFacade.GetAll()
                Else
                    accounts = Facade.PosteOnLineAccountFacade.GetUserAccounts()
                End If
                ddlPolAccount.DataSource = accounts
                ddlPolAccount.DataBind()
                If accounts.Count > 1 Then
                    ddlPolAccount.Items.Insert(0, New ListItem("Selezionare Tutti", ""))
                End If

                ddlType.Items.Clear()
                ddlType.Items.Add(New ListItem("Tutti", "0"))
                For Each requestType As POLRequestType In [Enum].GetValues(GetType(POLRequestType))
                    ddlType.Items.Add(New ListItem(requestType.ToString("G"), requestType.ToString("D")))
                Next
                If SessionFinderType <> SessionSearchController.SessionFinderType.PosteOnlineRequestFinder Then
                    dtpSentDateFrom.Clear()
                    dtpSentDateTo.Clear()
                    trSession.Visible = True
                End If
                If SessionFinderType = SessionSearchController.SessionFinderType.PosteOnlineRequestFinder AndAlso Not dtpSentDateFrom.SelectedDate.HasValue Then
                    dtpSentDateFrom.SelectedDate = Date.Today.AddDays(-30)
                End If
                If SessionFinderType = SessionSearchController.SessionFinderType.PosteOnlineRequestFinder AndAlso Not dtpSentDateTo.SelectedDate.HasValue Then
                    dtpSentDateTo.SelectedDate = Date.Today.AddDays(1).AddSeconds(-1)
                End If

                ' Pulsanti esportazione
                btnExcel.Icon.SecondaryIconUrl = ImagePath.SmallExcel
                btnExcel.CommandName = "Export"
                btnExcel.CommandArgument = "Excel"
                btnExcel.ToolTip = "Esporta in Excel"
                btnExcel.CausesValidation = False
            End If

            DataBindMailGrid()
        End Sub

        Private Sub CmdPosteWebRefreshClick(ByVal sender As Object, ByVal e As EventArgs) Handles cmdPosteWebRefresh.Click
            DataBindMailGrid()
        End Sub

        Protected Sub dgPosteRequestContact_OnItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgPosteRequestContact.ItemDataBound
            If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
                Exit Sub
            End If

            Dim polRequest As POLRequestRecipientHeader = DirectCast(e.Item.DataItem, POLRequestRecipientHeader)

            With DirectCast(e.Item.FindControl("imgType"), Image)
                .ImageUrl = GetPolRequestTypeImage(polRequest.RequestType)
                .AlternateText = GetPolRequestTypeText(polRequest.RequestType)
            End With

            If polRequest.ProtocolId.HasValue Then
                With DirectCast(e.Item.FindControl("imgShowProt"), LinkButton)
                    .PostBackUrl = $"ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={polRequest.ProtocolId}&Type=Prot")}"
                    .Text = ProtocolFacade.ProtocolFullNumber(polRequest.ProtocolYear.Value, polRequest.ProtocolNumber.Value)
                End With

            End If
        End Sub

        Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles btnExcel.Click
            Dim report As IReport(Of RadGrid) = ReportFacade.GenerateReport(Of RadGrid)(New List(Of RadGrid) From {dgPosteRequestContact}, (From column As GridColumn In dgPosteRequestContact.Columns Select New Column(column)).ToList())
            Dim doc As DocumentInfo = report.ExportExcel("Ricerca_PosteOnLine.xls")

            Dim file As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
            Dim script As String = String.Concat("window.open('", DocSuiteContext.Current.CurrentTenant.DSWUrl, "/Temp/", file.Name, "');")
            AjaxManager.ResponseScripts.Add(script)
        End Sub

#End Region

#Region " Methods "

        Private Sub DataBindMailGrid()
            Dim filterSent As Boolean? = Nothing
            If Not String.IsNullOrEmpty(rbtSent.SelectedValue) Then
                filterSent = CBool(rbtSent.SelectedValue)
            End If

            Dim requestType As POLRequestType? = Nothing
            If (Not ddlType.SelectedValue.Eq("0")) Then
                requestType = DirectCast([Enum].Parse(GetType(POLRequestType), ddlType.SelectedValue), POLRequestType)
            End If

            Dim accounts As New List(Of Integer)
            If String.IsNullOrEmpty(ddlPolAccount.SelectedValue) Then
                For Each item As ListItem In ddlPolAccount.Items
                    If Not String.IsNullOrEmpty(item.Value) Then
                        accounts.Add(Integer.Parse(item.Value))
                    End If
                Next
            Else
                If Not String.IsNullOrEmpty(ddlPolAccount.SelectedValue) Then
                    accounts.Add(Integer.Parse(ddlPolAccount.SelectedValue))
                End If
            End If


            CurrentFinder.Sent = filterSent
            CurrentFinder.DateSentFrom = dtpSentDateFrom.SelectedDate
            CurrentFinder.DateSentTo = dtpSentDateTo.SelectedDate
            CurrentFinder.Type = requestType
            CurrentFinder.Accounts = accounts

            dgPosteRequestContact.DataSource = CurrentFinder.DoSearchHeader()
            dgPosteRequestContact.DataBind()
        End Sub

        Public Shared Function GetPolRequestTypeText(ByVal requestType As POLRequestType?) As String
            If Not requestType.HasValue Then
                Return "Nessuna tipologia"
            End If

            Select Case requestType
                Case POLRequestType.Lettera
                    Return "Lettera"
                Case POLRequestType.Raccomandata
                    Return "Raccomandata"
                Case POLRequestType.Telegramma
                    Return "Telegramma"
                Case POLRequestType.Serc
                    Return "Serc"
                Case Else
                    Return "Tipologia non prevista"
            End Select
        End Function

        Public Shared Function GetPolRequestTypeImage(ByVal requestType As POLRequestType?) As String
            If Not requestType.HasValue Then
                Return "~/Comm/Images/PosteWeb/NonPrevista.gif"
            End If

            Select Case requestType
                Case POLRequestType.Lettera
                    Return "~/Comm/Images/PosteWeb/Lettera.gif"
                Case POLRequestType.Raccomandata
                    Return "~/Comm/Images/PosteWeb/Raccomandata.gif"
                Case POLRequestType.Telegramma
                    Return "~/Comm/Images/PosteWeb/Telegramma.gif"
                Case Else
                    Return "~/Comm/Images/PosteWeb/NonPrevista.gif"
            End Select
        End Function

#End Region

    End Class

End Namespace