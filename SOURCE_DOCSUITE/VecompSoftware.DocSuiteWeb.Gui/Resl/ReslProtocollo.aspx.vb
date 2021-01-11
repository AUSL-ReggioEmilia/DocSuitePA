Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ReslProtocollo
    Inherits ReslBasePage

#Region " Fields "

    Private _currentProtocol As Protocol
    Private _currentFinder As NHibernateProtocolFinder
    Private Const ReslProtocolloNhibernateProtocolFinderConst As String = "ReslProtocollo_NHibernateProtocolFinder"

#End Region

#Region " Properties "

    Public Property CurrentProtocol() As Protocol
        Get
            Return _currentProtocol
        End Get
        Set(ByVal value As Protocol)
            _currentProtocol = value
        End Set
    End Property

    Public ReadOnly Property ManagerId() As String
        Get
            Return Request.QueryString("ManagerID")
        End Get
    End Property

    Public Property CurrentFinder As NHibernateProtocolFinder
        Get
            If _currentFinder Is Nothing AndAlso Session(ReslProtocolloNhibernateProtocolFinderConst) IsNot Nothing Then
                _currentFinder = CType(Session(ReslProtocolloNhibernateProtocolFinderConst), NHibernateProtocolFinder)
            End If
            Return _currentFinder
        End Get
        Set(value As NHibernateProtocolFinder)
            _currentFinder = value
            Session(ReslProtocolloNhibernateProtocolFinderConst) = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjaxSettings()

        If Not Page.IsPostBack Then
            Initialize()
        End If

        If Action.Eq("Modify") Then
            LoadProtocol()
        End If
    End Sub

    Protected Sub ReslProtocolloAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim args As String() = e.Argument.Split("|"c)
        ddlYear.SelectedValue = args(0)
        rcbNumber.Text = args(1)
        LoadProtocol()
    End Sub

    Private Sub BtnSelezionaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSeleziona.Click
        LoadProtocol()
    End Sub

    Private Sub DdlYearSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlYear.SelectedIndexChanged
        CurrentFinder.Year = Short.Parse(ddlYear.SelectedValue)
        CurrentFinder.NumberLike = String.Empty
        rcbNumber.Items.Clear()
        rcbNumber.Text = String.Empty

        uscProtPreview.Visible = False
    End Sub

    Private Sub RcbNumberSelectedIndexChanged(o As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles rcbNumber.SelectedIndexChanged
        LoadProtocol()
    End Sub

    Private Sub RcbNumberItemsRequested(o As Object, e As RadComboBoxItemsRequestedEventArgs) Handles rcbNumber.ItemsRequested
        ' Filtro per numero
        CurrentFinder.NumberLike = e.Text

        Dim protocolNumbers As List(Of Integer) = (From protocol In CurrentFinder.DoSearch() Select protocol.Number).ToList()
        If protocolNumbers Is Nothing Then
            Exit Sub
        End If

        rcbNumber.Items.Clear()
        rcbNumber.DataSource = protocolNumbers
        rcbNumber.DataBind()
    End Sub

    Private Sub BtnInserimentoClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimento.Click
        Dim year As Int32 = 0
        Dim number As Int32 = 0
        Dim numberstr As String = rcbNumber.SelectedValue
        If (String.IsNullOrEmpty(numberstr)) Then
            numberstr = rcbNumber.Text
        End If
        If (Not Int32.TryParse(ddlYear.SelectedValue, year) OrElse Not Int32.TryParse(numberstr, number)) Then
            VecompSoftware.Services.Logging.FileLogger.Debug(VecompSoftware.Services.Logging.LogName.FileLog, String.Format("{0} - {1} ", year, numberstr))
            AjaxAlert("Selezionare il protocollo")
            Return
        End If
        CurrentProtocol = Facade.ProtocolFacade.GetById(year, number, False)
        If CurrentProtocol Is Nothing Then
            AjaxAlert(String.Concat("Protocollo ", year, "/", number, " non trovato"))
            Return
        End If

        Dim sLink As String = ProtocolFacade.GetCalculatedLink(_currentProtocol)
        MasterDocSuite.AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", sLink))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ddlYear)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rcbNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscProtPreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnInserimento)

        AjaxManager.AjaxSettings.AddAjaxSetting(ddlYear, rcbNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlYear, uscProtPreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlYear, btnInserimento)

        AjaxManager.AjaxSettings.AddAjaxSetting(rcbNumber, uscProtPreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbNumber, btnInserimento)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, rcbNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, uscProtPreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, btnInserimento)

        AddHandler AjaxManager.AjaxRequest, AddressOf ReslProtocolloAjaxRequest
    End Sub

    Private Sub Initialize()
        pnlAggiungi.Visible = False
        btnInserimento.Visible = False
        btnModifica.Visible = False

        lblTitolo.Text = "Seleziona protocollo"
        Select Case Action
            Case "Insert"
                Title = "Delibera - Inserimento Collegamento Protocollo"
                btnInserimento.Visible = True
                btnInserimento.Enabled = False
                pnlAggiungi.Visible = True
                LoadYears()
                CurrentFinder = GetFinder()
            Case "Modify"
                ' NOOP
        End Select
    End Sub

    Private Sub LoadYears()
        ddlYear.Items.Clear()
        Dim currentYear As Short = CType(Date.Now.Year, Short)
        '' Se l'anno è al massimo precedente di 10 all'anno corrente allora lo utilizza
        '' altrimenti prende 10 anni prima
        Dim startYear As Short = If(ResolutionEnv.ReslProtocolStartYear >= (currentYear - 10S), ResolutionEnv.ReslProtocolStartYear, currentYear - 10S)
        While currentYear >= startYear
            ddlYear.Items.Add(currentYear.ToString)
            currentYear -= 1S
        End While

        If ddlYear.Items.Count > 0 Then
            ddlYear.SelectedIndex = 0
        End If
    End Sub

    Private Function GetFinder() As NHibernateProtocolFinder
        Dim finder As New NHibernateProtocolFinder("ProtDB")
        finder.EnablePaging = True
        finder.PageSize = ResolutionEnv.ReslProtocolMaxResults
        finder.Year = Short.Parse(ddlYear.SelectedValue)
        finder.IdStatus = ProtocolStatusId.Attivo
        finder.SortExpressions.Add("P.Year", "Desc")
        finder.SortExpressions.Add("P.Number", "Desc")

        '' Aggiungo i check sulla sicurezza
        CommonInstance.ApplyProtocolFinderSecurity(finder, SecurityType.Read, True)

        Return finder
    End Function

    Private Sub LoadProtocol()
        btnInserimento.Enabled = False

        If Not String.IsNullOrEmpty(rcbNumber.Text) Then
            Dim year As Short = Short.Parse(ddlYear.SelectedValue)
            Dim number As Integer = Integer.Parse(rcbNumber.Text)
            CurrentProtocol = Facade.ProtocolFacade.GetById(year, number, False)
            If CurrentProtocol Is Nothing Then
                AjaxAlert(String.Format("Protocollo n. {0}{1}Protocollo inesistente", ProtocolFacade.ProtocolFullNumber(year, number), Environment.NewLine))
                Exit Sub
            End If
            If Not (New ProtocolRights(CurrentProtocol).IsEditable) Then
                AjaxAlert(String.Format("Protocollo n. {0}{1}Mancano i diritti necessari", ProtocolFacade.ProtocolFullNumber(year, number), Environment.NewLine))
                Exit Sub
            End If

            uscProtPreview.CurrentProtocol = CurrentProtocol
            uscProtPreview.Visible = True
            uscProtPreview.Initialize()

            btnInserimento.Enabled = True
        Else
            uscProtPreview.Visible = False
        End If
    End Sub

#End Region
End Class