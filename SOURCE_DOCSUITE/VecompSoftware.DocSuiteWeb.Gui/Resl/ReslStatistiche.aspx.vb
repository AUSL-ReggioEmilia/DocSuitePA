Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ReslStatistiche
    Inherits ReslBasePage

#Region " Fields "

    Private _resolutionByType As ArrayList
    Private _resolutionByContainer As ArrayList
    Private _resolutionByOC As ArrayList
    Private _resolutionByProposer As ArrayList
    Private _resolutionByRecipient As ArrayList

#End Region

#Region " Properties "

    Public Property ResolutionByType() As ArrayList
        Get
            Return _resolutionByType
        End Get
        Set(ByVal value As ArrayList)
            _resolutionByType = value
        End Set
    End Property

    Public Property ResolutionByContainer() As ArrayList
        Get
            Return _resolutionByContainer
        End Get
        Set(ByVal value As ArrayList)
            _resolutionByContainer = value
        End Set
    End Property

    Public Property ResolutionByOC() As ArrayList
        Get
            Return _resolutionByOC
        End Get
        Set(ByVal value As ArrayList)
            _resolutionByOC = value
        End Set
    End Property

    Public Property ResolutionByProposer() As ArrayList
        Get
            Return _resolutionByProposer
        End Get
        Set(ByVal value As ArrayList)
            _resolutionByProposer = value
        End Set
    End Property

    Public Property ResolutionByRecipient() As ArrayList
        Get
            Return _resolutionByRecipient
        End Get
        Set(ByVal value As ArrayList)
            _resolutionByRecipient = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        'Inizializzazione
        ResolutionByType = New ArrayList()
        ResolutionByOC = New ArrayList()
        ResolutionByContainer = New ArrayList()
        ResolutionByProposer = New ArrayList()
        ResolutionByRecipient = New ArrayList()

        'Bind filtri statistiche
        Dim finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB")
        finder.EnableStatus = False
        'data adozione
        finder.AdoptionDateFrom = rdpAdoptionDateFrom.SelectedDate
        finder.AdoptionDateTo = rdpAdoptionDateTo.SelectedDate
        'data proposta
        finder.ProposerDateFrom = rdpProposeDateFrom.SelectedDate
        finder.ProposerDateTo = rdpProposeDateTo.SelectedDate
        'tipologia
        Select Case ddlType.SelectedValue
            Case ResolutionType.IdentifierDelibera
                finder.Delibera = True
            Case ResolutionType.IdentifierDetermina
                finder.Determina = True
        End Select
        'contenitore
        finder.ContainerIds = ddlContainer.SelectedValue
        'contatti
        'TODO: check contacts

        'imposta header
        lblHeader.Text = "Totale n. Delibere e Determine"
        lblHeader.Text &= GetHeader()

        'dettaglio totali
        lblTotal.Text = finder.DoStat("active")(0)
        'dettaglio totali erratta registrazione
        lblTotalRegisterError.Text = finder.DoStat("activeregistrationerror")(0)

        'dettaglio per tipologia
        Dim typeList As ArrayList = finder.DoStat("type")
        For Each resl As Object In typeList
            ResolutionByType.Add(New Object() {GetAdoptionType(resl(0)), resl(2)})
        Next
        typeList = finder.DoStat("typeregistrationerror")
        For Each resl As Object In typeList
            ResolutionByType.Add(New Object() {GetAdoptionType(resl(0)) & " (errata registrazione)", resl(2)})
        Next
        rpType.DataSource = ResolutionByType
        rpType.DataBind()

        'dettaglio per contenitore
        ResolutionByContainer = finder.DoStat("container")
        rpContainer.DataSource = ResolutionByContainer
        rpContainer.DataBind()

        'dettaglio per organo di controllo

        Dim ocList As ArrayList = finder.DoStat("ocsupervisoryboard")
        If ocList.Count > 0 Then
            ResolutionByOC.Add(New Object() {"Controllo del Collegio Sindacale (art. 14, Legge 24.1.95 n. 10)", ocList(0)(1)})
        End If
        ocList = finder.DoStat("occorteconti")
        If ocList.Count > 0 Then
            ResolutionByOC.Add(New Object() {"Controllo della Corte dei Conti", ocList(0)(1)})
        End If
        ocList = finder.DoStat("ocmanagement")
        If ocList.Count > 0 Then
            ResolutionByOC.Add(New Object() {"Controllo di Gestione (Legge 30.07.04 n. 191.)", ocList(0)(1)})
        End If
        ocList = finder.DoStat("ocregion")
        If ocList.Count > 0 Then
            ResolutionByOC.Add(New Object() {"Controllo della Regione (LR 31/92)", ocList(0)(1)})
        End If

        ocList = finder.DoStat("ocother")
        If ocList.Count > 0 Then
            ResolutionByOC.Add(New Object() {"Altro", ocList(0)(1)})
        End If
        rpOC.DataSource = ResolutionByOC
        rpOC.DataBind()

        'dettaglio contatti
        If chkShowContact.Checked Then
            'proponente
            Dim contactList As ArrayList = finder.DoStat("proposer")
            For Each contact As Object In contactList
                ResolutionByProposer.Add(New Object() {GetAdoptionType(contact(2)) & " per " & contact(1), contact(0)})
            Next
            rpProposer.DataSource = ResolutionByProposer
            rpProposer.DataBind()

            contactList = finder.DoStat("recipient")
            For Each contact As Object In contactList
                ResolutionByRecipient.Add(New Object() {GetAdoptionType(contact(2)) & " inviate a " & contact(1), contact(0)})
            Next
            rpRecipient.DataSource = ResolutionByRecipient
            rpRecipient.DataBind()
        End If

        'mostra pannello
        pnlResolution.Visible = True
    End Sub

    Private Sub btnExcel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExcel.Click
        'ricreo la tabella statistiche
        btnRicerca_Click(sender, e)
        'esporta in excel
        ExportHelper.ExportToExcel(pnlResolution, Me, "ReslStatistiche.xls")
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRicerca, pnlResolution)
    End Sub

    Private Sub Initialize()
        ddlType.DataValueField = "Id"
        ddlType.DataTextField = "Description"
        rdpAdoptionDateTo.SelectedDate = Date.Now
        rdpAdoptionDateFrom.SelectedDate = DateAdd(DateInterval.Day, -(Date.Now.Day - 1), Date.Now)
        BindContainers()
    End Sub

    Protected Sub BindContainers()
        Dim containers As IList(Of ContainerRightsDto) = Facade.ContainerFacade.GetAllRights("Resl", True)
        If containers.Count > 0 Then
            For Each container As ContainerRightsDto In containers
                ddlContainer.Items.Add(New ListItem(container.Name, container.ContainerId.ToString()))
            Next
        End If
    End Sub

    Private Function GetAdoptionType(ByVal idType As Integer) As String
        Select Case idType
            Case ResolutionType.IdentifierDelibera
                Return "Delibere adottate"
            Case ResolutionType.IdentifierDetermina
                Return "Determine adottate"
            Case Else
                Return "Delibere e Determine adottate"
        End Select
    End Function

    Private Function GetHeader() As String
        Dim header As String = String.Empty
        header &= If(rdpAdoptionDateFrom.SelectedDate.HasValue, "Dal: " & rdpAdoptionDateFrom.SelectedDate.GetValueOrDefault(), "")
        header &= If(rdpAdoptionDateTo.SelectedDate.HasValue, " al: " & rdpAdoptionDateTo.SelectedDate.GetValueOrDefault(), "")
        header &= If(ddlType.SelectedItem.Text <> "", ", Tipologia: " & ddlType.SelectedItem.Text, "")
        header &= If(ddlContainer.SelectedItem.Text <> "", ", Contenitore: " & ddlContainer.SelectedItem.Text, "")
        If header.StartsWith(",") Then
            header = header.Remove(0, 1)
        End If
        header = If(header <> "", " (" & header & ")", header)

        Return header
    End Function
#End Region

End Class