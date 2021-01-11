Imports System.Collections.Generic
Imports System.Linq
Imports System.Collections.ObjectModel
Imports iTextSharp.text
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class ReslFirmaUltimaPagina
    Inherits ReslBasePage
    Implements ISignMultipleDocuments

#Region "Fields"
    Private Const PAGE_TITLE As String = "Firma ultima pagina"
    Private Const SAVE_SIGNED_DOCUMENTS_ACTION As String = "SAVEDOCUMENTS"
    Private Const INITIAL_PAGE_LOAD As String = "INITIALPAGELOAD"
    Private _containerResolutionTypeFacade As ContainerResolutionTypeFacade
#End Region

#Region "Properties"

    Public Property CurrentSelectedResolution As ICollection(Of Resolution)
        Get
            If ViewState("CurrentSelectedResolution") Is Nothing Then
                ViewState("CurrentSelectedResolution") = New Collection(Of Resolution)()
            End If
            Return DirectCast(ViewState("CurrentSelectedResolution"), ICollection(Of Resolution))
        End Get
        Set(value As ICollection(Of Resolution))
            ViewState("CurrentSelectedResolution") = value
        End Set
    End Property

    Public ReadOnly Property DateFrom As DateTime?
        Get
            Return GetKeyValue(Of DateTime?)("DateFrom")
        End Get
    End Property

    Public ReadOnly Property DateTo As DateTime?
        Get
            Return GetKeyValue(Of DateTime?)("DateTo")
        End Get
    End Property

    Public Property AutomaticImportDocument As Boolean
        Get
            Dim param As Boolean? = DirectCast(Session("AutomaticImportDocument"), Boolean?)
            Return param.HasValue AndAlso param.Value
        End Get
        Set(value As Boolean)
            If value.Equals(Nothing) Then
                Session.Remove("AutomaticImportDocument")
            Else
                Session("AutomaticImportDocument") = value
            End If
        End Set
    End Property

    Public ReadOnly Property DocumentsToSign() As IList(Of MultiSignDocumentInfo) Implements ISignMultipleDocuments.DocumentsToSign
        Get
            Dim list As New List(Of MultiSignDocumentInfo)
            For Each resolution As Resolution In CurrentSelectedResolution
                Dim fileresolution As FileResolution = Facade.FileResolutionFacade.GetByResolution(resolution).FirstOrDefault()
                Dim reslLocation As Location = Facade.ResolutionFacade.GetResolutionLocation(resolution.Id)
                ' Creazione oggetto per pagina di firma multipla
                Dim docs As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(reslLocation.ReslBiblosDSDB, fileresolution.IdUltimaPagina.Value)
                For Each documentInfo As BiblosDocumentInfo In docs
                    Dim msdi As New MultiSignDocumentInfo(documentInfo)
                    msdi.GroupCode = resolution.InclusiveNumber
                    If resolution.Type.Id = ResolutionType.IdentifierDetermina Then
                        msdi.Mandatory = False
                    Else
                        msdi.Mandatory = True
                    End If
                    msdi.DocType = "Ultima Pagina"
                    msdi.Description = resolution.InclusiveNumber.ToString()
                    msdi.IdOwner = resolution.Id.ToString()

                    list.Add(msdi)
                Next
            Next
            Return list
        End Get
    End Property

    Public ReadOnly Property ReturnUrl() As String Implements ISignMultipleDocuments.ReturnUrl
        Get
            Return String.Format("~/Resl/ReslFirmaUltimaPagina.aspx?Type=Resl&DateFrom={0}&DateTo={1}", rdpDateFrom.SelectedDate, rdpDateTo.SelectedDate)
        End Get
    End Property
    Public ReadOnly Property SignAction As String Implements ISignMultipleDocuments.SignAction
        Get
            Return String.Empty
        End Get
    End Property

    ''' <summary> Indica se la pagina è tornata dalla firma multipla senza operazioni </summary>
    Private ReadOnly Property UndoFromMultiSign As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)(MultipleSign.MultiSignUndoQuerystring, False)
        End Get
    End Property

    Private Property SignedComplete As Boolean
        Get
            Dim sessionParam As Object = Session("signedComplete")
            If sessionParam IsNot Nothing Then
                Return DirectCast(sessionParam, Boolean)
            End If
            Return False
        End Get
        Set(value As Boolean)
            If value = Nothing Then
                Session.Remove("signedComplete")
            Else
                Session("signedComplete") = value
            End If
        End Set
    End Property

    Private Property SignedDocuments As ICollection(Of MultiSignDocumentInfo)
        Get
            Dim sessionParam As Object = Session("signedDocuments")
            If sessionParam IsNot Nothing Then
                Return DirectCast(sessionParam, ICollection(Of MultiSignDocumentInfo))
            End If
            Session("signedDocuments") = New Collection(Of MultiSignDocumentInfo)
            Return DirectCast(Session("signedDocuments"), ICollection(Of MultiSignDocumentInfo))
        End Get
        Set(value As ICollection(Of MultiSignDocumentInfo))
            If value Is Nothing Then
                Session.Remove("signedDocuments")
            Else
                Session("signedDocuments") = value
            End If
        End Set
    End Property
    Public ReadOnly Property ContainerResolutionTypeFacade As ContainerResolutionTypeFacade
        Get
            If _containerResolutionTypeFacade Is Nothing Then
                _containerResolutionTypeFacade = New ContainerResolutionTypeFacade()
            End If
            Return _containerResolutionTypeFacade
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub ManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case SAVE_SIGNED_DOCUMENTS_ACTION
                SaveSignedDocuments()
                SignedDocuments = Nothing
                FindDataToSign()
                AjaxManager.ResponseScripts.Add("ResponseEnd();")
            Case INITIAL_PAGE_LOAD
                If DateFrom.HasValue OrElse DocSuiteContext.Current.ResolutionEnv.DefaulLoadLastPageEnabled Then
                    FindDataToSign()
                End If

                If Not UndoFromMultiSign AndAlso AutomaticImportDocument Then
                    InitializeSaveSignedDocuments()
                End If
                AutomaticImportDocument = Nothing
        End Select
    End Sub

    Protected Sub BtnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click

        If Not DocSuiteContext.Current.ResolutionEnv.DefaulLoadLastPageEnabled Then
            If Not (rdpAdoptionDateFrom.SelectedDate.HasValue OrElse rdpDateFrom.SelectedDate.HasValue) Then
                AjaxAlert("Inserire almeno uno dei campi 'Adottate dal' o 'Pubblicate dal'.")
                Exit Sub
            End If
        End If

        If Not Page.IsValid Then
            AjaxAlert("Errore nella validazione dei dati della pagina")
            Exit Sub
        End If

        FindDataToSign()
    End Sub

    Protected Sub Tipologia_Checked(ByVal sender As Object, ByVal e As EventArgs) Handles rblAtto.SelectedIndexChanged
        InitializeContainer()
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf ManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPageContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, tblResolutionsToSign)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnSign)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, pnlPageContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, tblResolutionsToSign)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, btnSign)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ddlReslContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(pnlType, ddlReslContainer)
    End Sub

    Private Sub Initialize()
        Title = PAGE_TITLE
        btnSign.Enabled = False
        btnSign.PostBackUrl = String.Format("{0}?saveResponse=True", MultipleSignBasePage.GetMultipleSignUrl())
        rdpDateFrom.SelectedDate = DateFrom
        rdpDateTo.SelectedDate = DateTo
        CurrentSelectedResolution = Nothing

        rblAtto.Items.Add(Facade.ResolutionTypeFacade.DeliberaCaption)
        rblAtto.Items(0).Value = ResolutionType.IdentifierDelibera.ToString()
        rblAtto.Items.Add(Facade.ResolutionTypeFacade.DeterminaCaption)
        rblAtto.Items(1).Value = ResolutionType.IdentifierDetermina.ToString()

        rblAtto.Items(0).Selected = True

        InitializeContainer()

        'Verifico se provengo dalla pagina di firma e se evvettivamente è stata eseguita una firma
        If SignedComplete Then
            SignedComplete = Nothing
            SignedDocuments = Nothing
            AutomaticImportDocument = True
            Dim sourcePage As MultipleSignBasePage = TryCast(PreviousPage, MultipleSignBasePage)
            If sourcePage IsNot Nothing Then
                SignedDocuments = sourcePage.SignedDocuments
            End If
            Response.Redirect(ReturnUrl, True)
        End If

        SignedComplete = Nothing

    End Sub

    Private Sub InitializeContainer()
        Dim tmpId As Integer
        ddlReslContainer.Items.Clear()
        ddlReslContainer.Items.Add(New DropDownListItem("", ""))
        Dim reslType As Short

        If Not Short.TryParse(rblAtto.SelectedValue, reslType) Then
            AjaxAlert("Errore durante la selezione della tipologia atto")
            Return
        End If

        If ResolutionEnv.UseContainerResolutionType Then
            ' Creo la lista in base all'associazione contenuta sulla tabella ContainerResolutionType            
            Dim containersList As IList(Of ContainerResolutionType) = ContainerResolutionTypeFacade.GetAllowedContainers(reslType, 1, ResolutionRightPositions.Adoption)

            For Each container As ContainerResolutionType In containersList
                ddlReslContainer.Items.Add(New DropDownListItem(container.container.Name, container.container.Id.ToString()))
            Next

            If ddlReslContainer.Items.Count.Equals(1) Then
                ddlReslContainer.SelectedIndex = 0
            End If
        Else
            Dim containersList As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Adoption, True)
            If containersList IsNot Nothing AndAlso containersList.Count > 0 Then
                For Each cont As Container In From cont1 In containersList.Distinct() Where tmpId <> cont1.Id
                    ddlReslContainer.Items.Add(New DropDownListItem(cont.Name, cont.Id.ToString()))
                    tmpId = cont.Id
                Next
            Else
                Throw New DocSuiteException(Facade.TabMasterFacade.TreeViewCaption & " Inserimento", "Utente senza diritti di Inserimento", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If
        End If
    End Sub

    Public Sub FindDataToSign()
        ' Eseguo la ricerca
        tblResolutionsToSign.Rows.Clear()
        CurrentSelectedResolution = Nothing
        Dim managedDateTo As DateTime = If(rdpDateTo.SelectedDate.HasValue, rdpDateTo.SelectedDate.Value, DateTime.Today)
        Dim selectedContainerId As Integer? = If(String.IsNullOrEmpty(ddlReslContainer.SelectedValue), CType(Nothing, Integer?), Integer.Parse(ddlReslContainer.SelectedValue))
        Dim managedDateFrom As DateTime? = If(rdpDateFrom.SelectedDate.HasValue, rdpDateFrom.SelectedDate.Value.AddSeconds(-1), CType(Nothing, DateTime?))

        Dim adoptionDateFrom As DateTime? = If(rdpAdoptionDateFrom.SelectedDate.HasValue, rdpAdoptionDateFrom.SelectedDate.Value.AddSeconds(-1), CType(Nothing, DateTime?))
        Dim adoptionDateTo As DateTime? = If(rdpAdoptionDateTo.SelectedDate.HasValue, rdpAdoptionDateTo.SelectedDate.Value, CType(Nothing, DateTime?))

        Dim reslType As Short

        If Not Short.TryParse(rblAtto.SelectedValue, reslType) Then
            AjaxAlert("Errore durante la selezione della tipologia atto")
            Return
        End If

        Dim resolutions As IList(Of Resolution) = Facade.ResolutionFacade.GetPublicated(managedDateFrom, managedDateTo.AddDays(1).AddSeconds(-1), adoptionDateFrom, adoptionDateTo, selectedContainerId, reslType)
        'Inizializzo la tabella
        tblResolutionsToSign.Rows.AddRaw(Nothing, {3}, {40, 60, 20}, {"Numero", "Oggetto", "Adottata"}, {"head", "head", "head"})
        If resolutions.Count > 0 Then
            For Each resolution As Resolution In resolutions
                CurrentSelectedResolution.Add(resolution)
                'Creo il controllo link
                Dim resolutionLink As HyperLink = New HyperLink()
                Dim params As String = CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&Type=Resl", resolution.Id))
                Dim resolutionUrl As String = String.Format("{0}?{1}", GetViewPageName(resolution.Id), params)
                resolutionLink.Text = Facade.ResolutionFacade.SqlResolutionGetNumber(idResolution:=resolution.Id, complete:=True, currentResolution:=resolution)
                resolutionLink.NavigateUrl = resolutionUrl

                'Creo la label dell'oggetto e della data adozione
                Dim resolutionSubject As Label = New Label()
                resolutionSubject.Text = resolution.ResolutionObject
                Dim resolutionAdoptionDate As Label = New Label()
                resolutionAdoptionDate.Text = resolution.AdoptionDate.Value.ToString("dd/MM/yyyy")
                tblResolutionsToSign.Rows.AddRaw(Nothing, {3}, {40, 60, 20}, {resolutionLink, resolutionSubject, resolutionAdoptionDate}, Nothing)
            Next
        End If
        btnSign.Enabled = CurrentSelectedResolution.Count > 0
        If Not CurrentSelectedResolution.Count > 0 Then
            tblResolutionsToSign.Rows.AddRaw(Nothing, {3}, Nothing, {"Nessun risultato nel periodo selezionato.", String.Empty, String.Empty}, Nothing)
        End If
    End Sub

    Private Sub InitializeSaveSignedDocuments()
        Const alertMessage As String = "Si vuole procedere con il salvataggio dei documenti firmati?"
        AjaxAlertConfirm(alertMessage, "SaveDocuments()", "", True)
    End Sub

    Private Sub SaveSignedDocuments()
        If SignedDocuments.Count > 0 Then
            For Each document As MultiSignDocumentInfo In SignedDocuments
                Dim resolution As Resolution = Facade.ResolutionFacade.GetById(Integer.Parse(document.IdOwner))
                Dim fileResolution As FileResolution = Facade.FileResolutionFacade.GetByResolution(resolution).FirstOrDefault()
                If fileResolution Is Nothing Then
                    Throw New DocumentException("Nessuna definizione dei documenti trovata")
                End If

                Dim file As FileDocumentInfo = DirectCast(document.DocumentInfo, FileDocumentInfo)
                Dim signature As String = Facade.ResolutionFacade.SqlResolutionGetNumber(idResolution:=resolution.Id, complete:=True)
                file.Name = "UltimaPagina.Pdf.P7M"
                file.Signature = signature

                If file.Attributes.Any(Function(k) k.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                    file.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE) = 0
                Else
                    file.Attributes.Add(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, 0)
                End If

                Dim lastPage As BiblosDocumentInfo = file.ArchiveInBiblos(resolution.Location.ReslBiblosDSDB, fileResolution.IdResolutionFile.Value)

                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    FacadeFactory.Instance.ResolutionLogFacade.Insert(resolution, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", file.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE), "documento", file.Name, lastPage.DocumentId))
                End If

                fileResolution.IdUltimaPagina = lastPage.BiblosChainId
                fileResolution.IdUltimaPagina = Nothing
                Facade.FileResolutionFacade.UpdateOnly(fileResolution)

                ' Imposto la data di creazione dell'ultima pagina
                resolution.UltimaPaginaDate = DateTimeOffset.UtcNow

                resolution.WebRevokeDate = DateTime.Today()
                resolution.WebState = Resolution.WebStateEnum.Revoked
                Facade.ResolutionFacade.UpdateOnly(resolution)
            Next
            AjaxAlert("Processo di firma concluso con successo")
            Exit Sub
        End If
        AjaxAlert("Errore nel recupero dei documenti firmati da salvare")
    End Sub

#End Region
End Class