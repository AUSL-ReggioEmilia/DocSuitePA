Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models

Public Class ResolutionDisplayController
    Inherits BaseResolutionController
    Implements IDisplayController

#Region " Fields "

    Protected _uscReslDisplay As uscResolution
    Protected _uscReslBar As uscResolutionBar
    Protected _printScript As String

#End Region

#Region " Properties "
    Protected ReadOnly Property CurrentResolutionRights As ResolutionRights
#End Region

#Region " Constructors "

    Public Sub New(ByRef uscControl As uscResolution)
        _uscReslDisplay = uscControl
        CurrentResolutionRights = New ResolutionRights(uscControl.CurrentResolution)
    End Sub

    Public Sub New(ByRef uscControl As uscResolution, ByRef uscBar As uscResolutionBar)
        Me.New(uscControl)

        _uscReslBar = uscBar
    End Sub

#End Region

#Region " Events "

    Protected Overridable Sub ButtonChangeClickDelegate(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim s As String = String.Format("Type=Resl&IdResolution={0}", _uscReslDisplay.CurrentResolution.Id)
        _uscReslDisplay.Page.Response.Redirect("../Resl/ReslModifica.aspx?" & CommonShared.AppendSecurityCheck(s))
    End Sub

    Protected Overridable Sub ButtonRegistrationClickDelegate(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim s As String = String.Format("IdResolution={0}&Type=Resl", _uscReslDisplay.CurrentResolution.Id)
        _uscReslDisplay.Page.Response.Redirect(String.Format("../Resl/{0}?{1}", ReslBasePage.GetViewPageName(_uscReslDisplay.CurrentResolution.Id), CommonShared.AppendSecurityCheck(s)))
    End Sub

    Protected Overridable Sub ButtonLogClickDelegate(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim s As String = String.Format("IdResolution={0}&Type=Resl", _uscReslDisplay.CurrentResolution.Id)
        _uscReslDisplay.Page.Response.Redirect("ReslLog.aspx?" & CommonShared.AppendSecurityCheck(s))
    End Sub
    Protected Overridable Sub ButtonConfirmViewClickDelegate(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim message As String = "Presa visione da parte dell'utente "
        Facade.ResolutionLogFacade.Log(_uscReslDisplay.CurrentResolution, ResolutionLogType.CV, message)
        _uscReslBar.ButtonConfirmView.Enabled = False
        _uscReslBar.ButtonConfirmView.Text = "Presa visione"
    End Sub

#End Region

#Region " Methods "

    Protected Overridable Sub InitializeNonStandardPanels()
        _uscReslDisplay.VisibleImmediatelyExecutive = False
        _uscReslDisplay.VisibleProposerProtocolLink = False
        _uscReslDisplay.VisibleObjectPrivacy = False
        _uscReslDisplay.ResolutionOC.Visible = False
        _uscReslDisplay.VisibleCheckWebPublish = False
        _uscReslDisplay.VisibleAmmTraspMonitorLog = False
    End Sub

    Public Overridable Sub Initialize() Implements IDisplayController.Initialize
        _uscReslDisplay.VisibleNumber = True
        _uscReslDisplay.VisibleStatus = True
        _uscReslDisplay.VisibleObject = True
        _uscReslDisplay.VisibleComunication = True
        _uscReslDisplay.VisibleComunicationAssMgr = True
        _uscReslDisplay.VisibleComunicationAssMgrAlternative = True
        _uscReslDisplay.VisibleComunicationDestProp = True
        _uscReslDisplay.VisibleComunicationDestPropAlternative = True
        _uscReslDisplay.VisibleRoles = True
        _uscReslDisplay.VisibleOther = True
        InitializeNonStandardPanels()
        InitializeButtons()
    End Sub

    Protected Overridable Sub InitializeButtons()
        InitializeButtonHandlers()
        _uscReslBar.ButtonDoc5.Visible = False
        _uscReslDisplay.ResolutionWorkflow.ButtonProposta = _uscReslBar.ButtonProposal
        _uscReslDisplay.ButtonDoc4 = _uscReslBar.ButtonDoc4

    End Sub

    Protected Overridable Sub InitializeButtonHandlers()
        AddHandler _uscReslBar.ButtonChange.Click, AddressOf ButtonChangeClickDelegate
        AddHandler _uscReslBar.ButtonRegistration.Click, AddressOf ButtonRegistrationClickDelegate
        AddHandler _uscReslBar.ButtonLog.Click, AddressOf ButtonLogClickDelegate
        AddHandler _uscReslBar.ButtonConfirmView.Click, AddressOf ButtonConfirmViewClickDelegate

        _uscReslBar.ButtonDuplicate.Attributes.Add("onclientclick", "return OpenWindowDuplica();")
        _uscReslBar.ButtonDelete.Attributes.Add("onclientclick", "return OpenWindowElimina();")
        _uscReslBar.ButtonDocument.OnClientClick = "return OpenWindowSelDocument();"
    End Sub

    Public Overridable Sub Show() Implements IDisplayController.Show
        'Inizializzo Pulsanti
        If _uscReslDisplay.ButtonDoc4 IsNot Nothing Then
            _uscReslDisplay.ButtonDoc4.Attributes.Clear()
        End If
        _uscReslDisplay.VisibleAmmTraspMonitorLog = DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.TransparentMonitoringEnabled

        'Organo di Controllo
        If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id) Then
            _uscReslDisplay.VisibleODC = True
            _uscReslDisplay.InitializeOC()
        Else
            _uscReslDisplay.VisibleODC = False
        End If

        If _uscReslDisplay.ButtonDoc4 IsNot Nothing Then
            If _uscReslDisplay.ButtonDoc4.Attributes.Count = 0 Then WebUtils.ObjAttDisplayNone(_uscReslDisplay.ButtonDoc4)
        End If

        'Dati Economici
        If Facade.ResolutionFacade.IsManagedProperty("EconomicData", _uscReslDisplay.CurrentResolution.Type.Id) Then
            _uscReslDisplay.VisibleEconomyData = True
        Else
            _uscReslDisplay.VisibleEconomyData = False
        End If

        'Proponente + Responsabile
        If Facade.ResolutionFacade.IsManagedProperty("Proposer", _uscReslDisplay.CurrentResolution.Type.Id, "CONTACT") _
            Or Facade.ResolutionFacade.IsManagedProperty("Recipent", _uscReslDisplay.CurrentResolution.Type.Id, "CONTACT") Then
            _uscReslDisplay.LoadProposerReceipientContacts()
        Else
            _uscReslDisplay.VisibleComunicationDestProp = False
        End If

        'Assegnatario + Manager
        If Facade.ResolutionFacade.IsManagedProperty("Assegnee", _uscReslDisplay.CurrentResolution.Type.Id, "CONTACT") _
            Or Facade.ResolutionFacade.IsManagedProperty("Manager", _uscReslDisplay.CurrentResolution.Type.Id, "CONTACT") Then
            _uscReslDisplay.LoadAssigneeManagerContacts()
        Else
            _uscReslDisplay.VisibleComunicationAssMgr = False
        End If

        'Classificatore
        If Facade.ResolutionFacade.IsManagedProperty("Category", _uscReslDisplay.CurrentResolution.Type.Id) Then
            _uscReslDisplay.VisibleCategory = True
        Else
            _uscReslDisplay.VisibleCategory = False
        End If

        'Storico
        _uscReslDisplay.LoadHistoryMode()

        'Carica Autorizzazioni
        _uscReslDisplay.LoadRoles()

    End Sub

    Public Overridable Sub ShowButtons() Implements IDisplayController.ShowButtons
        Dim currentResolution As Resolution = _uscReslDisplay.CurrentResolution
        Dim currentFileResolution As FileResolution = Facade.FileResolutionFacade.GetByResolution(currentResolution)(0)
        Dim documentInfoViewerPage As String = String.Format("{0}/Viewers/DocumentInfoViewer.aspx?", DocSuiteContext.Current.CurrentTenant.DSWUrl)
        Dim biblosChainInfoViewerPage As String = String.Format("{0}/Viewers/BiblosChainInfoViewer.aspx?", DocSuiteContext.Current.CurrentTenant.DSWUrl)

        'Pulsante Duplicazione
        _uscReslBar.ButtonDuplicate.Visible = ResolutionEnv.IsInsertDuplicateEnabled

        'Pulsante Log
        _uscReslBar.ButtonLog.Visible = ResolutionEnv.IsLogEnabled AndAlso (CommonShared.HasGroupAdministratorRight() OrElse (If(String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.EnvGroupLogView), False, CommonShared.HasGroupLogViewRight())))

        'Pulsante Modifica
        Dim show As Boolean = CurrentResolutionRights.IsExecutive OrElse CurrentResolutionRights.IsAdoptable OrElse CurrentResolutionRights.IsAdministrable

        _uscReslBar.ButtonChange.Visible = show
        If _uscReslBar.ButtonChange.Visible AndAlso currentResolution.EffectivenessDate.HasValue Then
            Dim workflowType As String = Facade.TabMasterFacade.GetFieldValue("WorkflowType", DocSuiteContext.Current.ResolutionEnv.Configuration, 2)
            _uscReslBar.ButtonChange.Enabled = False
            If ResolutionEnv.ModifyExecutiveResolutionEnabled AndAlso Not String.IsNullOrEmpty(workflowType) Then
                _uscReslBar.ButtonChange.Enabled = currentResolution.WorkflowType.Equals(workflowType) AndAlso CurrentResolutionRights.IsExecutive
            End If
        End If
        'Pulsante Svuota Annessi
        _uscReslBar.ButtonFlushAnnexed.Visible = False
        If ResolutionEnv.EnableFlushAnnexed Then
            _uscReslBar.ButtonFlushAnnexed.Visible = CurrentResolutionRights.IsFlushAnnexedEnable
        End If
        'Pulsante Invio Mail
        _uscReslBar.ButtonMail.Visible = CurrentResolutionRights.IsViewable
        'Pulsante Invio Mail a Settori
        _uscReslBar.ButtonMailRoles.Visible = (currentResolution.ResolutionRoles.Count > 0)
        'Puslante Elimina
        _uscReslBar.ButtonDelete.Visible = (Not currentResolution.AdoptionDate.HasValue) AndAlso
            ResolutionRights.CheckIsCancelable(currentResolution)
        'Pulsante Annulla
        _uscReslBar.ButtonCancel.Visible = ResolutionRights.HasCancelRight(currentResolution)
        If _uscReslBar.ButtonCancel.Visible Then
            Dim script As String = "OpenWindowAnnulla('../Resl/ReslElimina.aspx?Titolo=Annulla Adozione&" & CommonShared.AppendSecurityCheck("&Type=Resl&idResolution=" & currentResolution.Id & "&Action=Ado") & "')"
            _uscReslBar.ButtonCancel.Attributes.Add("onclick", script)
        End If
        'Pulsante Pratica
        _uscReslBar.ButtonDocument.Visible = (Facade.ResolutionFacade.GetDocumentLinkedCount(currentResolution.Id) > 0)
        'Pulsante Registrazione
        _uscReslBar.ButtonRegistration.Text = Facade.ResolutionTypeFacade.GetDescription(currentResolution.Type)
        'Pulsante Stampa
        _uscReslBar.ButtonPrint.Attributes.Add("onclick", _printScript)

        'Pannelli Pulsanti
        _uscReslBar.PanelPreviewButtons.Visible = _uscReslDisplay.BasePage.Action.Eq("Insert")
        _uscReslBar.PanelExtraButtons.Visible = Not _uscReslDisplay.BasePage.Action.Eq("Insert")

        _uscReslBar.ButtonPublishWeb.Visible = False
        _uscReslBar.ButtonRevokeWeb.Visible = False

        If currentResolution.Status.Id = ResolutionStatusId.Annullato Then
            If CommonUtil.HasGroupAdministratorRight Then
                _uscReslBar.ButtonProposal.Visible = True
                _uscReslBar.PanelDocumentButtons.Visible = True
            Else
                _uscReslBar.ButtonProposal.Visible = False
                _uscReslBar.PanelDocumentButtons.Visible = False
            End If
        End If

        ' Verifico l'utente corrente abbia i permessi di visualizzazione per gli allegati riservati e
        ' l'atto corrente abbia almeno un allegato riservato.
        If CurrentResolutionRights.IsPrivacyAttachmentAllowed AndAlso currentFileResolution.IdPrivacyAttachments.HasValue Then
            Dim privacyAttachmentsDocumentInfo As New BiblosDocumentInfo(currentResolution.Location.ReslBiblosDSDB, currentFileResolution.IdPrivacyAttachments.Value)
            Dim queryString As NameValueCollection = privacyAttachmentsDocumentInfo.ToQueryString()
            queryString.Add("Label", "Allegati riservati")
            Dim url As String = String.Concat(biblosChainInfoViewerPage, CommonShared.AppendSecurityCheck(queryString.AsEncodedQueryString()))
            _uscReslBar.ButtonPrivacyAttachments.Attributes.Add("onclick", String.Format("window.location.href = '{0}'; return false;", url))
            _uscReslBar.ButtonPrivacyAttachments.Visible = True
        Else
            _uscReslBar.ButtonPrivacyAttachments.Visible = False
        End If

        If currentFileResolution.IdFrontespizio.HasValue AndAlso Not ResolutionEnv.DisableButtonFrontPubblicazione Then
            Dim frontespizioDocumentInfo As New BiblosDocumentInfo(currentResolution.Location.ReslBiblosDSDB, currentFileResolution.IdFrontespizio.Value)
            Dim queryString As NameValueCollection = frontespizioDocumentInfo.ToQueryString()
            queryString.Add("Public", "true")
            Dim url As String = String.Concat(documentInfoViewerPage, CommonShared.AppendSecurityCheck(queryString.AsEncodedQueryString()))
            _uscReslBar.ButtonFrontespizio.Attributes.Add("onclick", String.Format("window.location.href = '{0}'; return false;", url))
            _uscReslBar.ButtonFrontespizio.Visible = True
            _uscReslBar.ButtonFrontespizio.Text = "Front. pubblicazione"
        End If

        If currentFileResolution.IdFrontalinoRitiro.HasValue Then
            Dim frontespizioRitiroDocumentInfo As New BiblosDocumentInfo(currentResolution.Location.ReslBiblosDSDB, currentFileResolution.IdFrontalinoRitiro.Value)
            Dim queryString As NameValueCollection = frontespizioRitiroDocumentInfo.ToQueryString()
            queryString.Add("Public", "true")
            Dim url As String = String.Concat(documentInfoViewerPage, CommonShared.AppendSecurityCheck(queryString.AsEncodedQueryString()))
            _uscReslBar.ButtonUltimaPagina.Attributes.Add("onclick", String.Format("window.location.href = '{0}'; return false;", url))
            _uscReslBar.ButtonUltimaPagina.Visible = True
            _uscReslBar.ButtonUltimaPagina.Text = "Front. ritiro"
        End If

        '' Se ci sono documenti pubblicati da visualizzare li mostro
        Dim webPublicationFacade As New WebPublicationFacade()
        Dim webPublicationList As IList(Of WebPublication) = webPublicationFacade.GetByResolution(currentResolution)
        If webPublicationList IsNot Nothing AndAlso webPublicationList.Count > 0 Then
            Dim webPublication As WebPublication = webPublicationList(0)
            If webPublication.IDDocument > 0 Then
                ''Se è stato memorizzato 1 id allora visualizzo e carico il bottone
                Dim webPublicationDocumentInfo As BiblosDocumentInfo = New BiblosDocumentInfo(currentResolution.Location.ReslBiblosDSDB, webPublication.IDDocument)
                Dim docQueryString As String = String.Format("{0}&ViewOriginal={1}&Public=true", webPublicationDocumentInfo.ToQueryString().AsEncodedQueryString(), True)
                Dim url As String = String.Concat(documentInfoViewerPage, CommonShared.AppendSecurityCheck(docQueryString))
                _uscReslBar.ButtonPublishedDocument.Attributes.Add("onclick", String.Format("window.location.href = '{0}'; return false;", url))
                _uscReslBar.ButtonPublishedDocument.Visible = True
                _uscReslBar.ButtonPublishedDocument.Enabled = True
            End If
        End If

        'Gestione Status
        If (currentResolution.Status.Id <> ResolutionStatusId.Attivo) Then
            'Disabilito il tasto di modifica
            If (_uscReslBar.ButtonChange.Visible) Then
                _uscReslBar.ButtonChange.Enabled = False
            End If
            'Elimino il tasto di eliminazione
            'Non ha senso annullare un atto già annullato
            If (_uscReslBar.ButtonDelete.Visible) Then
                _uscReslBar.ButtonDelete.Visible = False
            End If
            If (_uscReslBar.ButtonCancel.Visible) Then
                _uscReslBar.ButtonCancel.Visible = False
            End If
            'Rimuovo l'invio
            If (_uscReslBar.ButtonMail.Visible) Then
                _uscReslBar.ButtonMail.Visible = False
            End If
            'Rimuovo aggiungi a pratiche
            If (_uscReslBar.ButtonDocument.Visible) Then
                _uscReslBar.ButtonDocument.Visible = False
            End If
            If (_uscReslBar.ButtonAddToPratica.Visible) Then
                _uscReslBar.ButtonAddToPratica.Visible = False
            End If
            If (_uscReslBar.ButtonFascicle.Visible) Then
                _uscReslBar.ButtonFascicle.Visible = False
            End If
            If (_uscReslBar.ButtonbtnToSeries.Visible) Then
                _uscReslBar.ButtonbtnToSeries.Visible = False
            End If
            If (_uscReslBar.RepeaterbtnRoles.Visible) Then
                _uscReslBar.RepeaterbtnRoles.Visible = False
            End If
        End If
    End Sub

    Public Sub RegisterPrintScript(ByVal script As String) Implements IDisplayController.RegisterPrintScript
        _printScript = script
    End Sub

#End Region

End Class
