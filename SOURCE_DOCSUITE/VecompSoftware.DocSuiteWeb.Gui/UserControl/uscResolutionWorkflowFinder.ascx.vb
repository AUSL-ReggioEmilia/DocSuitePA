Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class uscResolutionWorkflowFinder
    Inherits DocSuite2008BaseControl

#Region " Fields "

    ReadOnly _finder As New NHibernateResolutionWorkflowFinder("ReslDB")
    Dim _contacts As IList(Of ContactDTO)
    Dim _protocols As IList(Of Protocol)
    Private _containerResolutionTypeFacade As ContainerResolutionTypeFacade

#End Region

#Region " Properties "

    Public ReadOnly Property Count() As Long
        Get
            Return _finder.Count()
        End Get
    End Property

    Public Property PageIndex() As Integer
        Get
            Return _finder.PageIndex
        End Get
        Set(ByVal value As Integer)
            _finder.PageIndex = value
        End Set
    End Property

    Public Property PageSize() As Integer
        Get
            Return _finder.PageSize
        End Get
        Set(ByVal value As Integer)
            _finder.PageSize = value
        End Set
    End Property

    Public ReadOnly Property Finder() As NHibernateResolutionWorkflowFinder
        Get
            BindData()
            Return _finder
        End Get
    End Property

    Public ReadOnly Property Tipologia() As Short
        Get
            Return Short.Parse(rblTipologia.SelectedValue)
        End Get
    End Property

    Public ReadOnly Property MyStep() As Short
        Get
            Dim sRet As Short
            If Tipologia = ResolutionType.IdentifierDelibera Then
                sRet = Short.Parse(rblFlusso.SelectedValue)
            Else
                sRet = Short.Parse(rblFlussoDet.SelectedValue)
            End If
            Return sRet
        End Get
    End Property

    Public ReadOnly Property PubDate() As DateTime?
        Get
            Return PublishingDate.SelectedDate
        End Get
    End Property

    Public ReadOnly Property CollWarningDate() As DateTime?
        Get
            Return CollegioWarningDate.SelectedDate
        End Get
    End Property

    Public ReadOnly Property TextProtocollo() As String
        Get
            Dim protocols As IList(Of Protocol) = uscProtocollo.GetProtocols()
            If protocols IsNot Nothing AndAlso protocols.Count > 0 Then
                Return ProtocolFacade.GetCalculatedLink(protocols(0))
            End If

            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property ContainerResolutionTypeFacade As ContainerResolutionTypeFacade
        Get
            If _containerResolutionTypeFacade Is Nothing Then
                _containerResolutionTypeFacade = New ContainerResolutionTypeFacade()
            End If
            Return _containerResolutionTypeFacade
        End Get
    End Property

    Public ReadOnly Property CheckOmissis As Boolean
        Get
            Return chbOmissis.Checked
        End Get
    End Property

    Private Const CONFIGURATION_RICERCAFLUSSOJSONCONFIG_FILE_PATH As String = "~/Config/RicercaFlussoConfig.json"

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
            VisualizzaCampiRicercaLettera(CType([Enum].Parse(GetType(TOWorkflow), rblFlusso.SelectedValue), TOWorkflow))

            pnlPropInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Proposer", ResolutionType.IdentifierDetermina, "CONTACT")
            uscPropInterop.ContactRoot = DocSuiteContext.Current.ResolutionEnv.ProposerContact
            pnlCategory.Visible = Facade.ResolutionFacade.IsManagedProperty("Category", ResolutionType.IdentifierDelibera)
        End If
    End Sub

    Private Sub RblFlussoSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles rblFlusso.SelectedIndexChanged
        InitializeContainer()
        Call VisualizzaCampiRicercaLettera(CType([Enum].Parse(GetType(TOWorkflow), rblFlusso.SelectedValue), TOWorkflow))
    End Sub

    Private Sub RblFlussoDetSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles rblFlussoDet.SelectedIndexChanged
        InitializeContainer()
        Call VisualizzaCampiRicercaLettera(CType([Enum].Parse(GetType(TOWorkflow), rblFlussoDet.SelectedValue), TOWorkflow))
    End Sub

    Private Sub RblTipologiaSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles rblTipologia.SelectedIndexChanged
        rblFlusso.Visible = (rblTipologia.SelectedValue = ResolutionType.IdentifierDelibera)
        rblFlussoDet.Visible = (rblTipologia.SelectedValue = ResolutionType.IdentifierDetermina)

        pnlOCRegion.Visible = (rblTipologia.SelectedValue = ResolutionType.IdentifierDelibera)

        Dim selectedStepValue As TOWorkflow = CType([Enum].Parse(GetType(TOWorkflow), rblFlusso.SelectedValue), TOWorkflow)
        If Convert.ToInt16(rblTipologia.SelectedValue) = ResolutionType.IdentifierDetermina Then
            selectedStepValue = CType([Enum].Parse(GetType(TOWorkflow), rblFlussoDet.SelectedValue), TOWorkflow)
        End If

        InitializeContainer()
        Call VisualizzaCampiRicercaLettera(selectedStepValue)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        With AjaxManager.AjaxSettings
            .AddAjaxSetting(rblTipologia, pnlRadioButtons)
            .AddAjaxSetting(rblTipologia, pnlControls)
            .AddAjaxSetting(rblFlusso, pnlControls)
            .AddAjaxSetting(rblFlussoDet, pnlControls)
        End With
    End Sub

    Private Sub Initialize()

        For Each pair As KeyValuePair(Of String, String) In DocSuiteContext.Current.RicercaFlussoConfiguration(ResolutionType.IdentifierDelibera.ToString())
            If Not (DocSuiteContext.Current.ResolutionEnv.AutomaticActivityStepEnabled AndAlso pair.Value.Equals("Esecutività")) Then
                rblFlusso.Items.Add(New ListItem(pair.Value, pair.Key))
            End If
        Next

        For Each pair As KeyValuePair(Of String, String) In DocSuiteContext.Current.RicercaFlussoConfiguration(ResolutionType.IdentifierDetermina.ToString())
            If Not (DocSuiteContext.Current.ResolutionEnv.AutomaticActivityStepEnabled AndAlso pair.Value.Equals("Esecutività")) Then
                rblFlussoDet.Items.Add(New ListItem(pair.Value, pair.Key))
            End If
        Next

        rblFlusso.SelectedValue = rblFlusso.Items(0).Value
        rblFlussoDet.SelectedValue = rblFlussoDet.Items(0).Value
        'Put user code to initialize the page here
        rblTipologia.Items.Add(Facade.ResolutionTypeFacade.DeliberaCaption)
        rblTipologia.Items(0).Value = ResolutionType.IdentifierDelibera.ToString()
        rblTipologia.Items.Add(Facade.ResolutionTypeFacade.DeterminaCaption)
        rblTipologia.Items(1).Value = ResolutionType.IdentifierDetermina.ToString()
        rblTipologia.Items(0).Selected = True
        rblFlusso.Visible = True
        rblFlussoDet.Visible = False

        chbOmissis.Text = String.Format("Inverti Oggetto/{0} per omissis", ResolutionEnv.ResolutionObjectPrivacyLabel)

        pnlOCRegion.Visible = rblTipologia.Items(0).Selected

        InitializeContainer()
        pnlCategorySearch.Visible = False

        pnlProposta.Visible = False
        pnlAdottata.Visible = False
        pnlAdottataIntervalloReq.Visible = False
        pnlOC.Visible = False
        pnlPubblicata.Visible = False
        pnlEsecutivita.Visible = False
        pnlContenitore.Visible = True
        pnlAltre.Visible = Facade.ResolutionFacade.IsManagedProperty("Proposer", ResolutionType.IdentifierDetermina, "CONTACT")
        pnlCategory.Visible = True
        pnlAdoptionYearNumber.Visible = False
        pnlOmissis.Visible = False
        pnlContainerMultiSelect.Visible = False

        txtAdoptionYear.Text = DateTime.Now.Year.ToString()
    End Sub

    Private Sub InitializeContainer(Optional defaultResolutionRightPosition As ResolutionRightPositions = ResolutionRightPositions.Adoption)
        Dim tmpId As Integer
        ddlContainer.Items.Clear()
        rcbContainerMultiSelect.Items.Clear()

        If ResolutionEnv.UseContainerResolutionType Then
            ' Creo la lista in base all'associazione contenuta sulla tabella ContainerResolutionType
            Dim selectedType As Short = Short.Parse(rblTipologia.SelectedValue)
            Dim containers As IList(Of ContainerResolutionType) = ContainerResolutionTypeFacade.GetAllowedContainers(selectedType, 1, defaultResolutionRightPosition)

            For Each container As ContainerResolutionType In containers.Distinct()
                Dim label As String = container.container.Name
                If CType([Enum].Parse(GetType(TOWorkflow), rblFlusso.SelectedValue), TOWorkflow).Equals(TOWorkflow.RicercaFlussoInvioAdozioneCollegioSindacaleFirmaDigitale) AndAlso
                        container.container.ContainerProperties.Any(Function(x) x.Name.Equals("LinkedContainers")) AndAlso
                    container.container.ContainerProperties.Single(Function(x) x.Name.Equals("LinkedContainers")) IsNot Nothing Then
                    label = String.Concat(label, " (raggruppato)")
                End If
                ddlContainer.Items.Add(New ListItem(label, container.container.Id.ToString()))
                rcbContainerMultiSelect.Items.Add(New RadComboBoxItem(container.container.Name, container.container.Id.ToString()))
            Next

            If ddlContainer.Items.Count.Equals(1) Then
                ddlContainer.SelectedIndex = 0
            End If
        Else
            Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Resolution, defaultResolutionRightPosition, True)
            If containers IsNot Nothing AndAlso containers.Count > 0 Then
                WebUtils.ObjDropDownListAdd(ddlContainer, "", "")
                For Each cont As Container In From cont1 In containers Where tmpId <> cont1.Id
                    WebUtils.ObjDropDownListAdd(ddlContainer, cont.Name, cont.Id.ToString())
                    tmpId = cont.Id
                    rcbContainerMultiSelect.Items.Add(New RadComboBoxItem(cont.Name, cont.Id.ToString()))
                Next
            Else
                Throw New DocSuiteException(Facade.TabMasterFacade.TreeViewCaption & " Inserimento", "Utente senza diritti di Inserimento", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If
        End If
    End Sub

    Private Sub VisualizzaCampiRicercaLettera(ByVal svalue As TOWorkflow)
        pnlProposta.Visible = False
        pnlAdottata.Visible = False
        pnlAdottataIntervallo.Visible = False
        pnlAdottataIntervalloReq.Visible = False
        pnlOC.Visible = False
        pnlPubblicata.Visible = False
        pnlEsecutivita.Visible = False
        pnlProtocollo.Visible = False
        pnlContenitore.Visible = True
        pnlAltre.Visible = Facade.ResolutionFacade.IsManagedProperty("Proposer", ResolutionType.IdentifierDetermina, "CONTACT")
        pnlCategory.Visible = True
        pnlAdoptionYearNumber.Visible = False
        pnlOmissis.Visible = False
        pnlContainerMultiSelect.Visible = False
        Select Case svalue
            Case TOWorkflow.RicercaFlussoAdozione
                pnlProposta.Visible = True
                pnlContenitore.Visible = False
                pnlContainerMultiSelect.Visible = True
                rcbContainerMultiSelectValidator.Enabled = True
                InitializeContainer()
            Case TOWorkflow.RicercaFlussoInvioAvvenutaAdozione
                pnlAdottataIntervallo.Visible = True
                cvContainer.Enabled = False
                ClearLinkedContainers()
            Case TOWorkflow.RicercaFlussoInvioAdozioneCollegioSindacaleFirmaDigitale
                If rblTipologia.SelectedValue = ResolutionType.IdentifierDelibera Then
                    pnlAdottata.Visible = True
                Else
                    pnlAdottataIntervalloReq.Visible = True
                End If
                'Nascodo il pannello del contenitore
                pnlContenitore.Visible = rblTipologia.SelectedValue = ResolutionType.IdentifierDelibera OrElse Not ResolutionEnv.UseContainerResolutionType
                If pnlContenitore.Visible Then
                    InitializeContainer()
                End If
                cvContainer.Enabled = False
            Case TOWorkflow.RicercaFlussoInvioAdozioneOrganiControllo
                If rblTipologia.SelectedValue = ResolutionType.IdentifierDelibera Then
                    pnlAdottata.Visible = True
                Else
                    pnlAdottataIntervallo.Visible = True
                End If
                ClearLinkedContainers()
            Case TOWorkflow.RicercaFlussoEsecutivita
                pnlPubblicata.Visible = True
                pnlContenitore.Visible = Short.Parse(rblTipologia.SelectedValue) = ResolutionType.IdentifierDelibera
                ClearLinkedContainers()
            Case TOWorkflow.RicercaFlussoPubblicazione
                Select Case CShort(rblTipologia.SelectedValue)
                    Case ResolutionType.IdentifierDelibera
                        pnlOC.Visible = True
                    Case ResolutionType.IdentifierDetermina
                        lblProtocollo.Text = "Prot. Trasmissione Adozione al CS"
                        pnlProtocollo.Visible = True
                        pnlContenitore.Visible = False
                End Select
                ClearLinkedContainers()
            Case TOWorkflow.RicercaFlussoUltimaPagina
                pnlContenitore.Visible = False
                cvContainer.Enabled = False
                pnlContainerMultiSelect.Visible = True
                rcbContainerMultiSelectValidator.Enabled = True
                pnlCategory.Visible = False
                pnlProposta.Visible = False
                pnlAdottataIntervallo.Visible = True
                pnlAdoptionYearNumber.Visible = True
                pnlOmissis.Visible = True

            Case TOWorkflow.RicercaRaccoltaFirmeAdozione
                pnlContenitore.Visible = False
                cvContainer.Enabled = False
                pnlContainerMultiSelect.Visible = True
                rcbContainerMultiSelectValidator.Enabled = True
                pnlProposta.Visible = True
                pnlCategory.Visible = False
                ClearLinkedContainers()
        End Select
    End Sub

    Private Sub ClearLinkedContainers()
        If pnlContenitore.Visible Then
            For Each item As ListItem In ddlContainer.Items
                item.Text = item.Text.Replace("(raggruppato)", "")
            Next
        End If
    End Sub

    Private Sub BindData()
        'Tipo di ricerca
        _finder.EagerLog = False
        If rblTipologia.SelectedValue = ResolutionType.IdentifierDelibera Then
            _finder.Delibera = True
            _finder.WorkflowStepTo = CType([Enum].Parse(GetType(TOWorkflow), rblFlusso.SelectedValue), TOWorkflow)
        End If
        If rblTipologia.SelectedValue = ResolutionType.IdentifierDetermina Then
            _finder.Determina = True
            _finder.WorkflowStepTo = CType([Enum].Parse(GetType(TOWorkflow), rblFlussoDet.SelectedValue), TOWorkflow)
        End If

        'idstatus sempre a 0
        _finder.IdStatus = 0

        'stepAttivo sempre a true
        _finder.StepAttivo = True

        'Container
        If _finder.WorkflowStepTo.Equals(TOWorkflow.RicercaFlussoUltimaPagina) OrElse _finder.WorkflowStepTo.Equals(TOWorkflow.RicercaRaccoltaFirmeAdozione) OrElse _finder.WorkflowStepTo.Equals(TOWorkflow.RicercaFlussoAdozione) Then
            _finder.ContainerIds = String.Join(",", rcbContainerMultiSelect.CheckedItems.Cast(Of RadComboBoxItem)().Select(Function(s) s.Value))
        Else
            If (pnlContenitore.Visible) Then
                _finder.ContainerIds = ddlContainer.SelectedItem.Value
                'ASL-TO: includo i contenitori privacy collegati per la generazione della lettera di trasmissione al CS
                If _finder.WorkflowStepTo.Equals(TOWorkflow.RicercaFlussoInvioAdozioneCollegioSindacaleFirmaDigitale) Then
                    Dim container As Container = Facade.ContainerFacade.GetById(Convert.ToInt32(ddlContainer.SelectedItem.Value))
                    Dim linkedContainerIds As IList(Of Integer) = New ContainerEnv(DocSuiteContext.Current, container).LinkedContainers
                    If linkedContainerIds IsNot Nothing AndAlso linkedContainerIds.Count > 0 Then
                        linkedContainerIds.Add(container.Id)
                        _finder.ContainerIds = String.Join(",", linkedContainerIds)
                    End If
                End If
            End If

        End If

        If _finder.WorkflowStepTo.Equals(TOWorkflow.RicercaFlussoUltimaPagina) Then
            If Not String.IsNullOrEmpty(txtAdoptionYear.Text) Then
                _finder.Year = txtAdoptionYear.Text
            End If

            If Not String.IsNullOrEmpty(txtNumberFrom.Text) Then
                _finder.NumberFrom = Convert.ToInt32(txtNumberFrom.Text)
            End If

            If Not String.IsNullOrEmpty(txtNumberTo.Text) Then
                _finder.NumberTo = Convert.ToInt32(txtNumberTo.Text)
            End If

            _finder.CheckLastPageDate = DocSuiteContext.Current.ResolutionEnv.UniqueLastPageGenerationEnabled

        End If

        'Proposta da a
        If pnlProposta.Visible Then
            _finder.DateFrom = ProposeDate_From.SelectedDate
            _finder.DateTo = ProposeDate_To.SelectedDate
        End If

        'Adottata il
        If pnlAdottata.Visible Then
            _finder.InDate = AdoptionDate.SelectedDate
        End If

        'Adottata da a
        If pnlAdottataIntervallo.Visible Then
            _finder.DateFrom = AdoptionDate_From.SelectedDate
            _finder.DateTo = AdoptionDate_To.SelectedDate
        End If

        If pnlAdottataIntervalloReq.Visible Then
            _finder.DateFrom = AdoptionDate_From_Req.SelectedDate
            _finder.DateTo = AdoptionDate_To_Req.SelectedDate
        End If

        'Inviata il
        If pnlOC.Visible Then
            _finder.InDate = CollegioWarningDate.SelectedDate
        End If

        'Pubblicata il
        If pnlPubblicata.Visible Then
            _finder.InDate = PublishingDate.SelectedDate
        End If

        'Esecutiva il
        If pnlEsecutivita.Visible Then
            _finder.InDate = EffectivenessDate.SelectedDate
        End If

        'Interop
        If pnlPropInterop.Visible Then
            _contacts = uscPropInterop.GetContacts(False)

            If _contacts.Count > 0 Then
                _finder.InteropProposers = _contacts(0).Contact.FullIncrementalPath
            End If
        End If

        'Protocollo
        If pnlProtocollo.Visible Then
            _protocols = uscProtocollo.GetProtocols()

            If _protocols.Count > 0 Then
                _finder.ProtocolLink = ProtocolFacade.GetCalculatedLink(_protocols(0))
            End If
        End If

        'Classificatore
        If pnlCategory.Visible Then
            If uscCategory.HasSelectedCategories Then
                _finder.Categories = uscCategory.SelectedCategories.First().FullIncrementalPath
            End If
            _finder.IncludeChildCategories = chbCategoryChild.Checked
        End If

        'OCRegion
        If pnlOCRegion.Visible Then
            _finder.OCRegion = chbOCRegion.Checked
        End If

    End Sub

    Public Function DoSearch() As IList(Of Resolution)
        BindData()
        Return _finder.DoSearch()
    End Function

    Public Function DoSearch(ByVal sortOrder As GridSortOrder) As IList(Of Resolution)
        BindData()
        Select Case sortOrder
            Case GridSortOrder.Ascending
                Return _finder.DoSearch(NHibernateResolutionFinder.SearchOrder.ASC)
            Case GridSortOrder.Descending
                Return _finder.DoSearch(NHibernateResolutionFinder.SearchOrder.DESC)
            Case Else
                Return _finder.DoSearch()
        End Select
    End Function

    Public Function DoSort(ByVal source As Object, ByVal e As GridSortCommandEventArgs) As IList(Of Resolution)
        For Each gse As GridSortExpression In source.MasterTableView.SortExpressions
            _finder.SortExpressions.Add(gse.FieldName, gse.SortOrder.ToString())
        Next
        If _finder.SortExpressions.ContainsKey(e.SortExpression) Then
            _finder.SortExpressions(e.SortExpression) = e.NewSortOrder.ToString()
        Else
            _finder.SortExpressions.Add(e.SortExpression, e.NewSortOrder.ToString())
        End If
        BindData()
        Return _finder.DoSearch()
    End Function

#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class