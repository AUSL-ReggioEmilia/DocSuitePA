Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionDisplayControllerPC
    Inherits ResolutionDisplayController

#Region " Costructor "

    Public Sub New(ByRef uscControl As uscResolution)
        MyBase.New(uscControl)
    End Sub

    Public Sub New(ByRef uscControl As uscResolution, ByRef uscBar As uscResolutionBar)
        MyBase.New(uscControl, uscBar)
    End Sub

#End Region

#Region " Events "

    Protected Overridable Sub ButtonDeleteFrontespizioHandler(ByVal sender As Object, ByVal e As EventArgs)
        ' TODO: verificare se usato
        Dim currentResolution As Resolution = _uscReslDisplay.CurrentResolution
        Dim businesslogic As New BusinessLogicResolution(currentResolution)

        ' Rimozione frontalino da catena documentale
        businesslogic.RemoveFrontalino()

        ' Rimozione frontalino da catena parcheggio
        Dim files As FileResolution = Facade.FileResolutionFacade.GetByResolution(currentResolution)(0)
        files.IdFrontespizio = Nothing
        Facade.FileResolutionFacade.Save(files)
        _uscReslBar.ButtonDeleteFrontespizio.Visible = False
        _uscReslBar.ButtonFrontespizio.Visible = False
    End Sub

    Protected Overridable Sub ButtonDeleteUltimaPaginaHandler(ByVal sender As Object, ByVal e As EventArgs)
        ' TODO: verificare se usato
        Dim currentResolution As Resolution = _uscReslDisplay.CurrentResolution
        Dim files As FileResolution = Facade.FileResolutionFacade.GetByResolution(currentResolution)(0)
        files.IdUltimaPagina = Nothing
        Facade.FileResolutionFacade.Save(files)
        _uscReslBar.ButtonDeleteUltimaPagina.Visible = False
        _uscReslBar.ButtonUltimaPagina.Visible = False
    End Sub

#End Region

#Region " Methods "

    Public Overrides Sub Initialize()
        MyBase.Initialize()
        _uscReslDisplay.VisibleImmediatelyExecutive = DocSuiteContext.Current.ResolutionEnv.ImmediatelyExecutiveEnabled 'EF 20120111 Rimuove la tabella "immediatamente esecutiva"
        _uscReslDisplay.VisibleProposerProtocolLink = False 'EF 20120117 Rimuove la scritta di invio ai servizi di protocollo dalla visualizzazione della scheda
        _uscReslDisplay.ResolutionOC.Visible = True
        _uscReslDisplay.VisibleCheckWebPublish = DocSuiteContext.Current.ResolutionEnv.WebPublishEnabled

        'EF 20120112 Rimozione caselle di comunicazione dal sommario
        _uscReslDisplay.VisibleComunication = True
        _uscReslDisplay.VisibleComunicationAssMgr = False
        _uscReslDisplay.VisibleComunicationAssMgrAlternative = False
        _uscReslDisplay.VisibleComunicationDestProp = False
        _uscReslDisplay.VisibleComunicationDestPropAlternative = True

        ' Nascondo la Data ritiro pubblicazione (Non gestita a Torino)
        _uscReslDisplay.VisibleWebRetire = False
        _uscReslDisplay.VisibleAmmTraspMonitorLog = False

    End Sub

    Protected Overrides Sub InitializeButtons()
        InitializeButtonHandlers()
        _uscReslBar.ButtonDoc4.Visible = False
        _uscReslBar.ButtonDoc5.Visible = False
        _uscReslDisplay.ResolutionWorkflow.ButtonProposta = _uscReslBar.ButtonProposal
        _uscReslDisplay.ResolutionWorkflow.HasProtocol = False 'Disattiva la colonna protocollo
        _uscReslDisplay.ResolutionWorkflow.HasMessages = True 'Attiva la colonna messaggi
        _uscReslDisplay.ResolutionOC.ButtonSupervisoryBoardFile = _uscReslBar.ButtonDoc4
        _uscReslDisplay.ResolutionOC.ButtonRegionFile = _uscReslBar.ButtonDoc5
    End Sub

    Protected Overrides Sub InitializeButtonHandlers()
        MyBase.InitializeButtonHandlers()
    End Sub

    Protected Overrides Sub InitializeNonStandardPanels()
        _uscReslDisplay.VisibleODC = False
    End Sub

    Public Overrides Sub ShowButtons()
        MyBase.ShowButtons()
        Dim currentResolution As Resolution = _uscReslDisplay.CurrentResolution
        _uscReslBar.ButtonMailRoles.Visible = False
        _uscReslBar.ButtonPrint.Visible = False
        _uscReslBar.ButtonDuplicate.Visible = False
        _uscReslBar.ButtonCancel.Visible = False

        _uscReslBar.ButtonLog.Visible = ResolutionEnv.IsLogEnabled AndAlso (CommonShared.HasGroupAdministratorRight() OrElse (If(String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.EnvGroupLogView), False, CommonShared.HasGroupLogViewRight())))

        If (currentResolution.Status.Id = ResolutionStatusId.Attivo AndAlso ResolutionEnv.UncomplianceRevokeResolutionEnabled) Then
            _uscReslBar.ButtonCancel.Visible = (currentResolution.AdoptionDate.HasValue AndAlso
                       ResolutionRights.CheckIsExecutive(currentResolution) AndAlso
                       ResolutionRights.CheckIsCancelable(currentResolution))
        ElseIf (currentResolution.Status.Id = ResolutionStatusId.Attivo) Then
            _uscReslBar.ButtonCancel.Visible = (currentResolution.AdoptionDate.HasValue AndAlso
                             ResolutionRights.CheckIsExecutive(currentResolution) AndAlso
                             ResolutionRights.CheckIsCancelable(currentResolution)) AndAlso Not currentResolution.PublishingDate.HasValue
        End If

        If _uscReslBar.ButtonCancel.Visible Then
            Dim script As String = "OpenWindowAnnulla('../Resl/ReslElimina.aspx?Titolo=Annulla Adozione&" & CommonShared.AppendSecurityCheck("&Type=Resl&idResolution=" & currentResolution.Id & "&Action=Ado") & "')"
            _uscReslBar.ButtonCancel.Attributes.Add("onclick", script)
            _uscReslBar.ButtonCancel.Text = "Annulla atto"
        End If

        'Visualizzazione del bottone per la visualizzazione del frontalino di adozione generato automaticamente
        _uscReslBar.ButtonFrontespizio.Visible = False
        If (currentResolution.WorkflowType = Facade.TabMasterFacade.GetFieldValue(TabMasterFacade.WorkflowTypeField, "AUSL-PC", currentResolution.Type.Id) AndAlso
            currentResolution.AdoptionDate.HasValue And Not currentResolution.EffectivenessDate.HasValue) Then

            Dim files As FileResolution = Facade.FileResolutionFacade.GetByResolution(currentResolution)(0)
            If files.IdResolutionFile.HasValue AndAlso Not ResolutionEnv.DisableButtonFrontPubblicazione Then
                _uscReslBar.ButtonFrontespizio.Text = "Rel. adozione"
                _uscReslBar.ButtonFrontespizio.Visible = True

                Dim viewerPage As String = String.Format("{0}/viewers/FileResolutionViewer.aspx?", DocSuiteContext.Current.CurrentTenant.DSWUrl)
                Dim querystring As String = String.Format("idResolution={0}&field=idResolutionFile&description=Rel.%20Adozione", currentResolution.Id)
                Dim temp As String = String.Concat(viewerPage, CommonShared.AppendSecurityCheck(querystring))

                _uscReslBar.ButtonFrontespizio.OnClientClick = String.Format("window.location.href = '{0}'; return false;", temp)
            End If
        End If

        'Visualizzo il bottone di gestione della pubblicazione sse:
        '-> L'atto è già stato pubblicato
        '-> Si possiedono i diritti di Ufficio Dirigenziale (2)
        Dim currentWorkflowStep As TabWorkflow
        Dim activeStep As Short = Facade.ResolutionWorkflowFacade.GetActiveStep(currentResolution.Id)
        Facade.TabWorkflowFacade.GetByStep(currentResolution.WorkflowType, activeStep, currentWorkflowStep)
        'Verifico se è già presente un id nella tabella WebPublication
        _uscReslBar.ButtonPubblicaRevoca.Visible = Facade.ResolutionFacade.IsPublicated(currentResolution) AndAlso currentResolution.Status.Id = ResolutionStatusId.Attivo AndAlso ResolutionRights.CheckIsExecutive(currentResolution) AndAlso currentResolution.PublishingDate.HasValue

        'Attivo il bottone di gestione della pubblicazione sse:
        '-> Il bottone è stato visualizzato (quindi significa che l'atto è già stato pubblicato)
        '-> Non sono passati più di 15 gg dalla data di prima pubblicazione
        _uscReslBar.ButtonPubblicaRevoca.Enabled = _uscReslBar.ButtonPubblicaRevoca.Visible AndAlso DateTime.Now <= currentResolution.PublishingDate.Value.AddDays(15)
        If (Not _uscReslBar.ButtonPubblicaRevoca.Enabled AndAlso _uscReslBar.ButtonPubblicaRevoca.Visible) Then
            _uscReslBar.ButtonPubblicaRevoca.ToolTip = "Non è possibile revocare una pubblicazione dopo 15gg dall'inserimento all'albo"
        End If
        'Cerco lo step con il link alla pagina
        Dim adozioneWorkflowStep As TabWorkflow = Facade.TabWorkflowFacade.GetByDescription("Adozione", currentResolution.WorkflowType)
        If (_uscReslBar.ButtonPubblicaRevoca.Enabled AndAlso adozioneWorkflowStep.PubblicaRevocaPage IsNot Nothing) Then
            Dim page As String = adozioneWorkflowStep.PubblicaRevocaPage
            Dim querystring As String = String.Format("idResolution={0}&Type={1}&ReslType={2}&Action={3}&Step={4}", currentResolution.Id, "Resl", currentResolution.Type.Id, "Revoke", activeStep)
            Dim script As String = String.Format("return uscWorkflow_OpenWindow('windowWorkflow', ""{0}?{1}""); return false;", page, CommonShared.AppendSecurityCheck(querystring))
            _uscReslBar.ButtonPubblicaRevoca.Attributes.Add("onclick", script)
        End If

    End Sub

    Public Overrides Sub Show()
        _uscReslDisplay.LoadImmediatelyExecutive()
        _uscReslDisplay.LoadPubblicationInternet()
        _uscReslDisplay.VisibleAmmTraspMonitorLog = DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.TransparentMonitoringEnabled

        'Organo di Controllo
        If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id) Then
            _uscReslDisplay.ResolutionOC.LoadOCList()

            'Disattivo le spunte del controllo di Gestione e della Corte dei Conti
            _uscReslDisplay.ResolutionOC.VisibleOCControlloGestioneCorteConti = False
            'Disattivo le spunte del controllo "Altro"
            _uscReslDisplay.ResolutionOC.VisibleOCChkAltro = False
            'Attivo le spunte del controllo Conferenza dei Sindaci
            _uscReslDisplay.ResolutionOC.VisibleOCChkConfSindaci = True
            'Disattivo la riga relativa al rilievo del collegio sindacale
            _uscReslDisplay.ResolutionOC.VisibleOCRilievoCollegioSindacale = False
            'Disattivo la data di Scadenza della Regione
            _uscReslDisplay.ResolutionOC.VisibleWaitDate = False
            'Disattivo il testo DGR
            _uscReslDisplay.ResolutionOC.VisibleDGR = False
            'Disattivo le note di Approvazione
            _uscReslDisplay.ResolutionOC.VisibleApprovalNote = False
            'Disattivo le note di Decadimento
            _uscReslDisplay.ResolutionOC.VisibleDeclineNote = False
            'Disattivo la data di Ricezione della Regione
            _uscReslDisplay.ResolutionOC.VisibleConfirmDate = False
            'Disattivo il commento della Regione
            _uscReslDisplay.ResolutionOC.VisibleCommentoRegione = False
            'Disattivo la riga del commento della regione
            _uscReslDisplay.ResolutionOC.VisibleRegionCommento = False
            'Attivo i dati di invio chiarimenti della Regione
            _uscReslDisplay.ResolutionOC.VisibleRegionInvioChiarimenti = True
            'Disattivo i dati di ricezione e scadenza della Regione
            _uscReslDisplay.ResolutionOC.VisibleRegionRicezioneScadenza = False
            'Attivo il protocollo di risposta della Regione
            _uscReslDisplay.ResolutionOC.VisibleResponseProtocolLink = True
            _uscReslDisplay.ResolutionOC.AltroLabel = "Conferenza dei Sindaci."
            _uscReslDisplay.ResolutionOC.RispostaDefinitivaRegione = "Risposta finale della Regione:"
            _uscReslDisplay.ResolutionOC.DataRispostaRegione = "Data risposta della Regione:"
        End If

        'Organo di Controllo: Collegio Sindacale
        If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id, "CS") Then
            If _uscReslDisplay.CurrentResolution.OCSupervisoryBoard.GetValueOrDefault(False) Then
                _uscReslDisplay.ResolutionOC.InitializeOCSupervisoryBoard()
            End If
        Else
            _uscReslDisplay.ResolutionOC.UnLoadCS()
        End If

        If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id, "CONFSIND") Then
            If _uscReslDisplay.CurrentResolution.OCManagement.GetValueOrDefault(False) Then
                _uscReslDisplay.ResolutionOC.LoadOCConfSindaci()
            End If
        Else
            _uscReslDisplay.ResolutionOC.UnloadCONFSIND()
        End If

        'Organo di Controllo: Regione
        If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id, "REG") Then
            If _uscReslDisplay.CurrentResolution.OCRegion.GetValueOrDefault(False) Then
                _uscReslDisplay.ResolutionOC.InitializeOCRegion()
            End If
        Else
            _uscReslDisplay.ResolutionOC.UnloadREG()
        End If

        'Oggetto Privacy: visualizzato solo quando il contenitore ha il flag Privacy alzato
        _uscReslDisplay.VisibleObjectPrivacy = Not String.IsNullOrEmpty(_uscReslDisplay.CurrentResolution.ResolutionObjectPrivacy)

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

#End Region

End Class
