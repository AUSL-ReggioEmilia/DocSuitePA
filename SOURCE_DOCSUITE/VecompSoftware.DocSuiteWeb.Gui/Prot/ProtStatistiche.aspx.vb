Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports Telerik.Web.UI

Public Class ProtStatistiche
    Inherits ProtBasePage

    Private Class Stat

        Public Overridable Property Name As String

        Public Overridable Property FullCode As String

        Public Overridable Property Protocols As Integer

        Public Sub New()

        End Sub

    End Class

#Region " Fields "

    Dim vFinder As NHibernateProtocolFinder = New NHibernateProtocolFinder("ProtDB")
    Dim vProtocols As IList = New ArrayList()
    Dim vProtocolsByStatus As IList = New ArrayList()
    Dim vProtocolsAttivi As IList = New ArrayList()
    Dim vProtocolsByContainer As IList = New ArrayList()
    Dim vProtocolsByUser As IList = New ArrayList()


#End Region

#Region " Properties "

    Public Property Protocols() As IList
        Get
            Return vProtocols
        End Get
        Set(ByVal value As IList)
            vProtocols = value
        End Set
    End Property

    Public Property ProtocolsByStatus() As IList
        Get
            Return vProtocolsByStatus
        End Get
        Set(ByVal value As IList)
            vProtocolsByStatus = value
        End Set
    End Property

    Public Property ProtocolsAttivi() As IList
        Get
            Return vProtocolsAttivi
        End Get
        Set(ByVal value As IList)
            vProtocolsAttivi = value
        End Set
    End Property

    Public Property ProtocolsByContainer() As IList
        Get
            Return vProtocolsByContainer
        End Get
        Set(ByVal value As IList)
            vProtocolsByContainer = value
        End Set
    End Property

    Public Property ProtocolsByUser() As IList
        Get
            Return vProtocolsByUser
        End Get
        Set(ByVal value As IList)
            vProtocolsByUser = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        CommonInstance.UserDeleteTemp(TempType.P)
        ' Inizializza gli oggetti della pagina
        InitializeAjax()
        If Not IsPostBack Then
            InizializeControls()
        End If
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs)
        Select Case Action
            Case "P" : GetStatsProtocollo() ' Statistiche per protocollo
            Case "C" : GetStatsClassificatore(String.Empty, String.Empty) ' Statistiche per classificatore
        End Select
    End Sub

    ''' <summary> Popola ogni riga della datagrid con il codice categoria, la descrizione e le statistiche dei protocolli. </summary>
    Private Sub gvClassifications_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles gvClassifications.ItemDataBound
        If (e.Item.ItemType = GridItemType.Item OrElse e.Item.ItemType = GridItemType.AlternatingItem) Then
            Dim vFullCode As String = e.Item.DataItem.FullCode
            Dim vName As String = e.Item.DataItem.Name
            Dim vProtocolsCount As String = e.Item.DataItem.Protocols
            ' Colonna full code
            CType(e.Item, GridDataItem)("FullCode").Text = SetCodice(vFullCode)
            ' Colonna descrizione
            CType(e.Item, GridDataItem)("Name").Text = SetDescrizione(vFullCode, vName)
            ' Colonna totale
            CType(e.Item, GridDataItem)("Protocols").Text = vProtocolsCount
        End If
    End Sub

    ''' <summary> Gestisce il sort sulle colonne della datagrid </summary>
    Protected Sub gvClassifications_SortCommand(ByVal source As Object, ByVal e As GridSortCommandEventArgs) Handles gvClassifications.SortCommand
        Dim order As String = "ASC"
        If e.NewSortOrder.ToString().ToUpper = "DESCENDING" Then
            order = "DESC"
        End If
        GetStatsClassificatore(e.SortExpression, order)
    End Sub

    Protected Sub BtnExportExcel_Click(ByVal sender As Object, ByVal e As EventArgs)
        'ricreo la tabella statistiche
        btnRicerca_Click(sender, e)

        'esporta in excel
        Select Case Action
            Case "P"
                ExportHelper.ExportToExcel(pnlProtocollo, Me, "StatisticheProtocollo.xls")
            Case "C"
                gvClassifications.MasterTableView.ExportToExcel()
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRicerca, pnlProtocollo, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRicerca, pnlClassificatore, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(gvClassifications, gvClassifications, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    ''' <summary> Statistiche per protocollo </summary>
    Private Sub GetStatsProtocollo()
        ' Filtra per data
        vFinder.RegistrationDateFrom = txtSelDateFrom.SelectedDate
        vFinder.RegistrationDateTo = txtSelDateTo.SelectedDate
        ' Filtra per utente
        vFinder.RegistrationUser = txtRegistrationUser.Text.Trim()
        ' Filtra per tipo
        Dim types As List(Of Integer) = New List(Of Integer)
        Dim typeSelected As Integer = Integer.MaxValue
        If Not String.IsNullOrEmpty(ddlType.SelectedValue) AndAlso Integer.TryParse(ddlType.SelectedValue, typeSelected) AndAlso typeSelected <> Integer.MaxValue AndAlso
            (DocSuiteContext.Current.ProtocolEnv.IsInterOfficeEnabled OrElse (Not DocSuiteContext.Current.ProtocolEnv.IsInterOfficeEnabled AndAlso typeSelected <> 0)) Then
            types.Add(typeSelected)
        End If

        vFinder.IdTypes = types
        ' Filtra per contenitore
        vFinder.IdContainer = ddlContainer.SelectedValue
        ' Filtra per sottocategorie se selezionato
        If chbCategoryChild.Checked And uscClassificatore.CategoryID <> 0 Then
            vFinder.IncludeChildClassifications = True
        End If

        'Statistiche non filtrate per stato protocollo
        vFinder.NoStatus = True
        ' filtra per categorie
        If uscClassificatore.HasSelectedCategories Then
            vFinder.Classifications = uscClassificatore.SelectedCategories.First().FullIncrementalPath
        End If
        ' Statistiche protocolli totali
        Protocols = vFinder.DoStat(String.Empty)
        lblTotal.Text = Protocols(0).ToString()
        ' Statistiche protocolli per stato
        ProtocolsByStatus = vFinder.DoStat("status")
        For Each p As IList In ProtocolsByStatus
            p(0) = ProtocolFacade.GetStatusDescription(DirectCast(p(0), Integer))
        Next
        rpStatus.DataSource = ProtocolsByStatus
        rpStatus.DataBind()

        'Statistiche filtrate per stato protocollo
        vFinder.NoStatus = False
        vFinder.IdStatus = ProtocolStatusId.Attivo
        ' Statistiche protocolli attivi per tipo
        ProtocolsAttivi = vFinder.DoStat("type")
        rpAttivi.DataSource = ProtocolsAttivi
        rpAttivi.DataBind()
        ' Statistiche protocolli attivi per contenitore
        ProtocolsByContainer = vFinder.DoStat("container")
        rpContainer.DataSource = ProtocolsByContainer
        rpContainer.DataBind()
        ' Statistiche protocolli attivi per utente
        ProtocolsByUser = vFinder.DoStat("user")
        rpUtente.DataSource = ProtocolsByUser
        rpUtente.DataBind()
        ' visualizza tabella dei risultati
        pnlProtocollo.Visible = True
    End Sub

    ''' <summary> Statistiche per classificatore </summary>
    Private Sub GetStatsClassificatore(ByVal sortColumn As String, ByVal sortOrder As String)
        ' Lista delle categorie

        Dim vStats As List(Of Stat) = New List(Of Stat)
        ' Ricerca e visualizza le categorie senza protocolli
        Dim hideEmptyCategories As Boolean = chbNulle.Checked
        ' Ricerca categorie
        Dim vClassifications As IList = Facade.CategoryFacade.GetStatsCategory(txtSelDateFrom.SelectedDate, txtSelDateTo.SelectedDate, txtRegistrationUser.Text.Trim(), ddlType.SelectedValue, ddlContainer.SelectedValue, hideEmptyCategories, sortColumn, sortOrder)
        ' Crea una classe stat per ogni categoria
        For i As Integer = 0 To vClassifications.Count - 1
            Dim stat As New Stat()
            stat.FullCode = vClassifications.Item(i)(0).FullCode
            stat.Name = vClassifications.Item(i)(0).Name
            stat.Protocols = vClassifications.Item(i)(1)
            vStats.Add(stat)
        Next
        ' Popola la datagrid
        gvClassifications.Visible = True
        gvClassifications.DataSource = vStats
        gvClassifications.DataBind()
        ' Visualizza la datagrid
        pnlClassificatore.Visible = True
    End Sub

    ''' <summary> Formatta il codice classificatore </summary>
    ''' <param name="Codice">String: Full code categoria</param>
    ''' <returns>String: Full code nel formato c.c.c </returns>
    Public Function SetCodice(ByVal codice As String) As String
        Dim s As String = ""
        If codice <> "" Then
            Dim a As Array = Split(codice, "|")
            Dim i As Integer
            For i = 0 To a.Length - 1
                s &= Int(a(i)) & "."
            Next
            s = Left(s, Len(s) - 1)
        End If
        Return s
    End Function

    ''' <summary> Formatta la descrizione del classificatore </summary>
    ''' <param name="Codice">String: Full code categoria</param>
    ''' <param name="Descrizione">String: Descrizione categoria</param>
    ''' <returns>String: Descrizione nel formato d. d. d.</returns>
    Public Function SetDescrizione(ByVal codice As String, ByVal descrizione As String) As String
        Dim sDescrizione As String = ""
        Dim aPipe As Array = Nothing
        If codice <> "" Then
            aPipe = Split(codice, "|")
        End If
        For i As Integer = 1 To aPipe.Length - 1
            sDescrizione &= ".  "
        Next
        sDescrizione &= descrizione
        Return sDescrizione
    End Function

    Private Sub InizializeControls()
        ddlType.DataValueField = "Id"
        ddlType.DataTextField = "Description"
        ddlType.DataSource = Facade.ProtocolTypeFacade().GetTypes(includeInOut:=True, includeEmpty:=True)
        ddlType.DataBind()

        ' Carico solo i container attivi
        ddlContainer.Items.Add("")
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetAllRightsDistinct("Prot", True)
        For Each container As Container In containers
            ddlContainer.Items.Add(New ListItem(container.Name, container.Id.ToString()))
        Next

        ' Pulsanti esportazione
        btnExcel.Icon.SecondaryIconUrl = ImagePath.SmallExcel
        btnExcel.CommandName = "Export"
        btnExcel.CommandArgument = "Excel"
        btnExcel.ToolTip = "Esporta in Excel"
        btnExcel.CausesValidation = False

        Select Case Action
            Case "P" ' Statistiche protocollo - imposta titolo pagina, pulsati e righe
                Title = "Protocollo - Statistiche"
                pnlProtocollo.Visible = True
                rowSelClassificatore.Visible = True
                rowChbNulle.Visible = False
                btnStampa.Visible = True
            Case "C" ' Statistiche classificatore - imposta titolo pagina, pulsanti datagrid
                Title = "Classificatore - Statistiche"
                pnlClassificatore.Visible = True
                rowSelClassificatore.Visible = False
                rowChbNulle.Visible = True
        End Select
        ' Date iniziali
        txtSelDateFrom.SelectedDate = Date.Now.AddDays(-Date.Now.Day + 1)
        txtSelDateTo.SelectedDate = Date.Today
    End Sub

#End Region

End Class
