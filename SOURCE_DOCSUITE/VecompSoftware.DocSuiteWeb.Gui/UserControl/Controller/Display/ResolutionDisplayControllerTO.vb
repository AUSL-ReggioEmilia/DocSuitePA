Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

Public Class ResolutionDisplayControllerTO
    Inherits ResolutionDisplayController

#Region " Fields "

#End Region

#Region " Constructors "

    Public Sub New(ByRef uscControl As uscResolution)
        MyBase.New(uscControl)
    End Sub

    Public Sub New(ByRef uscControl As uscResolution, ByRef uscBar As uscResolutionBar)
        MyBase.New(uscControl, uscBar)
    End Sub

#End Region

#Region " Events "

    Protected Overridable Sub ButtonDeleteFrontespizioHandler(ByVal sender As Object, ByVal e As EventArgs)
        Dim currentResolution As Resolution = _uscReslDisplay.CurrentResolution
        Dim businesslogic As New BusinessLogicResolution(currentResolution)

        ' Rimozione frontalino da catena documentale
        businesslogic.RemoveFrontalino()

        ' Rimozione frontalino da catena parcheggio
        Dim files As FileResolution = Facade.FileResolutionFacade.GetByResolution(currentResolution)(0)
        files.IdFrontespizio = Nothing
        Facade.FileResolutionFacade.Save(files)

        _uscReslDisplay.ResolutionWorkflow.SetWorkflow()

        _uscReslBar.ButtonDeleteFrontespizio.Visible = False
        _uscReslBar.ButtonFrontespizio.Visible = False
    End Sub

    Protected Overridable Sub ButtonDeleteUltimaPaginaHandler(ByVal sender As Object, ByVal e As EventArgs)
        Dim currentResolution As Resolution = _uscReslDisplay.CurrentResolution
        Dim files As FileResolution = Facade.FileResolutionFacade.GetByResolution(currentResolution)(0)
        files.IdUltimaPagina = Nothing
        Facade.FileResolutionFacade.Save(files)
        _uscReslBar.ButtonDeleteUltimaPagina.Visible = False
        _uscReslBar.ButtonUltimaPagina.Visible = False
    End Sub

    Protected Sub ButtonTakeChargeHandler(sender As Object, args As EventArgs)
        Try
            Dim workflowUsers As ICollection(Of ResolutionWorkflowUser) = Facade.ResolutionWorkflowUserFacade.GetUsersByResolution(_uscReslDisplay.CurrentResolution)
            Dim logDescription As String
            For Each itemToRemove As ResolutionWorkflowUser In workflowUsers.ToList()
                logDescription = $"Rimossa presa in carico per l'utente {itemToRemove.Account}"
                Facade.ResolutionWorkflowUserFacade.Delete(itemToRemove)
                Facade.ResolutionLogFacade.Insert(_uscReslDisplay.CurrentResolution.Id, ResolutionLogType.RM, logDescription)
            Next

            If _uscReslBar.ButtonTakeCharge.CommandArgument.Eq("RemoveTakeCharge") Then
                _uscReslDisplay.SetTakeChargeVisibility(False, Nothing)
                _uscReslBar.ButtonTakeCharge.CommandArgument = "AddTakeCharge"
                _uscReslBar.ButtonTakeCharge.Text = "Conferma visione"
            Else
                Dim itemToInsert As ResolutionWorkflowUser = New ResolutionWorkflowUser()
                itemToInsert.Account = DocSuiteContext.Current.User.FullUserName
                itemToInsert.AuthorizationType = AuthorizationRoleType.Responsible
                itemToInsert.ResolutionWorkflow = Facade.ResolutionWorkflowFacade.GetAllByResolution(_uscReslDisplay.CurrentResolution.Id, True).Single()
                Facade.ResolutionWorkflowUserFacade.Save(itemToInsert)
                Facade.ResolutionLogFacade.Insert(_uscReslDisplay.CurrentResolution.Id, ResolutionLogType.RM, $"Determina presa in carico dall'utente {DocSuiteContext.Current.User.FullUserName}")

                _uscReslDisplay.SetTakeChargeVisibility(True, itemToInsert)
                _uscReslBar.ButtonTakeCharge.CommandArgument = "RemoveTakeCharge"
                _uscReslBar.ButtonTakeCharge.Text = "Rimuovi conferma visione"
            End If
            _uscReslDisplay.ResolutionWorkflow.SetWorkflow()
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, $"Error on take charge action: {ex.Message}", ex)
            _uscReslBar.BasePage.AjaxAlert($"Errore nell'esecuzione dell'attività di {_uscReslBar.ButtonTakeCharge.Text}")
        Finally
            _uscReslBar.BasePage.AjaxManager.ResponseScripts.Add("endTakeChargeAction();")
        End Try
    End Sub

#End Region

#Region " Methods "

    Public Overrides Sub Initialize()
        MyBase.Initialize()
        _uscReslDisplay.VisibleImmediatelyExecutive = ResolutionEnv.ImmediatelyExecutiveEnabled
        _uscReslDisplay.VisibleProposerProtocolLink = True
        _uscReslDisplay.ResolutionOC.Visible = True
        _uscReslDisplay.VisibleCheckWebPublish = DocSuiteContext.Current.ResolutionEnv.WebPublishEnabled

        ' Nascondo la Data ritiro pubblicazione (Non gestita a Torino)
        _uscReslDisplay.VisibleWebRetire = False
        _uscReslDisplay.VisibleAmmTraspMonitorLog = False
    End Sub

    Protected Overrides Sub InitializeButtons()
        InitializeButtonHandlers()
        _uscReslBar.ButtonDoc4.Visible = False
        _uscReslBar.ButtonDoc5.Visible = False
        _uscReslDisplay.ResolutionWorkflow.ButtonProposta = _uscReslBar.ButtonProposal
        _uscReslDisplay.ResolutionWorkflow.HasProtocol = True
        _uscReslDisplay.ResolutionOC.ButtonSupervisoryBoardFile = _uscReslBar.ButtonDoc4
        _uscReslDisplay.ResolutionOC.ButtonRegionFile = _uscReslBar.ButtonDoc5
    End Sub

    Protected Overrides Sub InitializeButtonHandlers()
        MyBase.InitializeButtonHandlers()

        AddHandler _uscReslBar.ButtonDeleteFrontespizio.Click, AddressOf ButtonDeleteFrontespizioHandler
        AddHandler _uscReslBar.ButtonDeleteUltimaPagina.Click, AddressOf ButtonDeleteUltimaPaginaHandler
        AddHandler _uscReslBar.ButtonTakeCharge.Click, AddressOf ButtonTakeChargeHandler

    End Sub

    Protected Overrides Sub InitializeNonStandardPanels()
        _uscReslDisplay.VisibleODC = False
    End Sub

    Public Overrides Sub ShowButtons()
        MyBase.ShowButtons()

        _uscReslBar.ButtonMailRoles.Visible = CurrentResolutionRights.IsViewable
        _uscReslBar.ButtonPrint.Visible = False
        _uscReslBar.ButtonDuplicate.Visible = False
        _uscReslBar.ButtonConfirmView.Visible = False
        _uscReslBar.ButtonTakeCharge.Visible = False

        _uscReslBar.ButtonLog.Visible = ResolutionEnv.IsLogEnabled AndAlso (CommonShared.HasGroupAdministratorRight() OrElse (If(String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.EnvGroupLogView), False, CommonShared.HasGroupLogViewRight())))

        Dim currentResolution As Resolution = _uscReslDisplay.CurrentResolution

        If Not _uscReslBar.ButtonChange.Visible Then
            _uscReslBar.ButtonChange.Visible = (CurrentResolutionRights.CanInsertInContainer AndAlso Not currentResolution.AdoptionDate.HasValue AndAlso String.IsNullOrEmpty(currentResolution.AdoptionUser)) OrElse
                                               CurrentResolutionRights.CanExecutiveModifyOC
        End If
        If Not _uscReslBar.ButtonChange.Enabled Then
            _uscReslBar.ButtonChange.Enabled = CurrentResolutionRights.CanExecutiveModifyOC
        End If


        If (currentResolution.Status.Id = ResolutionStatusId.Attivo) Then
            _uscReslBar.ButtonCancel.Visible = (Not currentResolution.AdoptionDate.HasValue) AndAlso ResolutionRights.CheckIsCancelable(currentResolution)

            If ResolutionEnv.ResolutionConfirmViewingRequiredEnabled Then
                Dim currentResolutionStep As TabWorkflow = Facade.TabWorkflowFacade.GetActive(currentResolution)
                Dim confirmViewResponsabilityGroups As ICollection(Of Integer) = Facade.TabWorkflowFacade.GetOperationStepConfirmViewResponsabilityGroups(currentResolutionStep)
                Dim securityGroups As ICollection(Of SecurityGroups) = Facade.SecurityGroupsFacade.GetByUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
                Dim alreadyConfirmed As Boolean = Facade.ResolutionLogFacade.GetlastResolutionLog(currentResolution.Id, ResolutionLogType.CV) IsNot Nothing
                If securityGroups.Any(Function(x) confirmViewResponsabilityGroups.Any(Function(xx) xx.Equals(x.Id))) Then
                    _uscReslBar.ButtonConfirmView.Visible = True
                    _uscReslBar.ButtonConfirmView.Enabled = Not alreadyConfirmed
                    If alreadyConfirmed Then
                        _uscReslBar.ButtonConfirmView.Text = "Presa visione"
                    End If
                End If
            Else
                If (currentResolution.Type.Id = ResolutionType.IdentifierDelibera AndAlso (Facade.ResolutionWorkflowFacade.GetActiveStep(currentResolution.Id) = 2S OrElse Facade.ResolutionWorkflowFacade.GetActiveStep(currentResolution.Id) = 3S)) Then
                    Dim isbtnVisible As Boolean = ResolutionRights.CheckIsExecutive(currentResolution)
                    _uscReslBar.ButtonCancel.Visible = isbtnVisible
                    _uscReslBar.ButtonConfirmView.Visible = isbtnVisible
                End If
            End If

            If _uscReslBar.ButtonCancel.Visible Then
                Dim script As String = "OpenWindowAnnulla('../Resl/ReslElimina.aspx?Titolo=Annulla Proposta&" & CommonShared.AppendSecurityCheck("&Type=Resl&idResolution=" & currentResolution.Id & "&Action=Ann") & "')"
                _uscReslBar.ButtonCancel.Attributes.Add("onclick", script)
                _uscReslBar.ButtonCancel.Text = "Annullamento"
            Else
                _uscReslBar.ButtonCancel.Visible = Not currentResolution.PublishingDate.HasValue AndAlso ResolutionRights.CheckIsCancelable(currentResolution)

                If _uscReslBar.ButtonCancel.Visible Then
                    Dim script As String = "OpenWindowAnnulla('../Resl/ReslElimina.aspx?Titolo=Annulla Adozione&" & CommonShared.AppendSecurityCheck("&Type=Resl&idResolution=" & currentResolution.Id & "&Action=Ado") & "')"
                    _uscReslBar.ButtonCancel.Attributes.Add("onclick", script)
                    _uscReslBar.ButtonCancel.Text = "Annullamento"
                End If
            End If
        Else
            _uscReslBar.ButtonCancel.Visible = False
        End If

        If currentResolution.Type.Id.Equals(ResolutionType.IdentifierDetermina) AndAlso currentResolution.Status.Id = ResolutionStatusId.Attivo AndAlso Not ResolutionEnv.ResolutionConfirmViewingRequiredEnabled Then
            Dim currentResolutionStep As TabWorkflow = Facade.TabWorkflowFacade.GetActive(currentResolution)
            If currentResolutionStep.Description.Eq(WorkflowStep.AFF_GEN_CHECK_STEP_DESCRIPTION) Then
                Dim flowResponsabilityRoles As ICollection(Of Integer) = Facade.TabWorkflowFacade.GetOperationStepFlowResponsabilityRoles(currentResolutionStep)
                _uscReslBar.ButtonTakeCharge.Visible = flowResponsabilityRoles.Count > 0 AndAlso CurrentResolutionRights.HasCurrentStepFlowResponsabilityRights
                _uscReslBar.ButtonCancel.Visible = _uscReslBar.ButtonCancel.Visible AndAlso CurrentResolutionRights.HasCurrentStepFlowResponsabilityRights
                _uscReslBar.ButtonChange.Visible = _uscReslBar.ButtonChange.Visible AndAlso CurrentResolutionRights.HasCurrentStepFlowResponsabilityRights
                _uscReslBar.ButtonDelete.Visible = _uscReslBar.ButtonDelete.Visible AndAlso CurrentResolutionRights.HasCurrentStepFlowResponsabilityRights
                _uscReslBar.ButtonTakeCharge.Text = "Conferma visione"
                _uscReslBar.ButtonTakeCharge.CommandArgument = "AddTakeCharge"
                If CurrentResolutionRights.IsResponsibleUser Then
                    _uscReslBar.ButtonTakeCharge.Text = "Rimuovi conferma visione"
                    _uscReslBar.ButtonTakeCharge.CommandArgument = "RemoveTakeCharge"
                End If
            End If
        End If

        ' Visualizzazione Frontespizio e Ultima Pagina
        Dim files As FileResolution = Facade.FileResolutionFacade.GetByResolution(currentResolution)(0)

        If files.IdFrontespizio.HasValue AndAlso Not ResolutionEnv.DisableButtonFrontPubblicazione Then
            _uscReslBar.ButtonFrontespizio.Visible = True

            Dim viewerScript As String = GetFileResolutionViewerScript("idFrontespizio", "Frontespizio")
            _uscReslBar.ButtonFrontespizio.Attributes.Add("onclick", viewerScript)
        End If

        If files.IdUltimaPagina.HasValue Then
            _uscReslBar.ButtonUltimaPagina.Visible = True

            Dim viewerScript As String = GetFileResolutionViewerScript("idUltimaPagina", "Ultima pagina")
            _uscReslBar.ButtonUltimaPagina.Attributes.Add("onclick", viewerScript)
        End If

        _uscReslBar.ButtonLastPageUpload.Visible = False
        If currentResolution.EffectivenessDate.HasValue AndAlso Not currentResolution.UltimaPaginaDate.HasValue AndAlso CurrentResolutionRights.IsAdoptable Then
            _uscReslBar.ButtonLastPageUpload.Visible = True
            Dim script As String = String.Concat("return OpenWindowLastPage('../Resl/ReslUltimaPagina.aspx?Type=Resl&IdResolution=", currentResolution.Id, "')")
            _uscReslBar.ButtonLastPageUpload.Attributes.Add("onclick", script)
        End If
    End Sub

    Private Function GetFileResolutionViewerScript(fieldName As String, description As String) As String
        Dim url As String = String.Format("{0}/viewers/FileResolutionViewer.aspx?", DocSuiteContext.Current.CurrentTenant.DSWUrl) & CommonShared.AppendSecurityCheck(String.Format("idResolution={0}&field={1}&description={2}", _uscReslDisplay.CurrentResolution.Id, fieldName, HttpUtility.UrlEncode(description)))
        Return String.Format("window.location.href='{0}'; return false;", url)
    End Function

    Public Overrides Sub Show()
        _uscReslDisplay.LoadImmediatelyExecutive()
        _uscReslDisplay.LoadPubblicationInternet()
        _uscReslDisplay.VisibleAmmTraspMonitorLog = DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.TransparentMonitoringEnabled

        'Organo di Controllo
        If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id) Then
            _uscReslDisplay.ResolutionOC.LoadOCList()
        End If

        If DocSuiteContext.Current.ResolutionEnv.CheckOCValidations Then
            'Organo di Controllo: Collegio Sindacale
            If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id, "CS") Then
                _uscReslDisplay.ResolutionOC.InitializeOCSupervisoryBoard()
            End If

            'Organo di Controllo: Corte dei Conti
            If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id, "CC") Then
                If _uscReslDisplay.CurrentResolution.OCCorteConti.GetValueOrDefault(False) Then
                    _uscReslDisplay.ResolutionOC.LoadOCCorteDeiConti()
                End If
            End If

            'Organo di Controllo: Regione
            If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id, "REG") Then
                If _uscReslDisplay.CurrentResolution.OCRegion.GetValueOrDefault(False) Then
                    _uscReslDisplay.ResolutionOC.InitializeOCRegion()
                End If
            End If

            'Organo di Controllo: Gestione
            If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id, "GEST") Then
                If _uscReslDisplay.CurrentResolution.OCManagement.GetValueOrDefault(False) Then
                    _uscReslDisplay.ResolutionOC.LoadOCManagement()
                End If
            End If

            'Organo di Controllo: Altro
            If Facade.ResolutionFacade.IsManagedProperty("OCData", _uscReslDisplay.CurrentResolution.Type.Id, "ALTRO") Then
                If _uscReslDisplay.CurrentResolution.OCOther.GetValueOrDefault(False) Then
                    _uscReslDisplay.ResolutionOC.LoadOCOther()
                End If
            End If
        End If

        'Oggetto Privacy: visualizzato solo quando il contenitore ha il flag Privacy alzato
        If _uscReslDisplay.CurrentResolution.Container.Privacy.HasValue Then
            _uscReslDisplay.VisibleObjectPrivacy = (_uscReslDisplay.CurrentResolution.Container.Privacy.Value = 1) AndAlso CurrentResolutionRights.IsPreviewable
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

        _uscReslDisplay.SetTakeChargeVisibility(False, Nothing)
        If _uscReslDisplay.CurrentResolution.Type.Id.Equals(ResolutionType.IdentifierDetermina) Then
            Dim currentResolutionStep As TabWorkflow = Facade.TabWorkflowFacade.GetActive(_uscReslDisplay.CurrentResolution)
            If currentResolutionStep.Description.Eq(WorkflowStep.AFF_GEN_CHECK_STEP_DESCRIPTION) Then
                Dim currentResponsibleUser As ResolutionWorkflowUser = Facade.ResolutionWorkflowUserFacade.GetUsersByResolution(_uscReslDisplay.CurrentResolution).FirstOrDefault()
                If (currentResponsibleUser IsNot Nothing) Then
                    _uscReslDisplay.SetTakeChargeVisibility(True, currentResponsibleUser)
                End If
            End If
        End If
    End Sub

#End Region

End Class
