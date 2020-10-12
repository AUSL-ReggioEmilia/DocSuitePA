Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.DTO.Protocols
Imports VecompSoftware.Services.Logging

Public Class ProtNoteGes
    Inherits ProtBasePage

#Region "Properties"
    Public Property RoleNoteChanged As IList(Of ProtocolRoleNoteModel)
        Get
            If ViewState("RoleNoteChanged") Is Nothing Then
                ViewState("RoleNoteChanged") = New List(Of ProtocolRoleNoteModel)
            End If
            Return DirectCast(ViewState("RoleNoteChanged"), IList(Of ProtocolRoleNoteModel))
        End Get
        Set(value As IList(Of ProtocolRoleNoteModel))
            ViewState("RoleNoteChanged") = value
        End Set
    End Property

    Private Property PreviousNodeSelected As ProtocolRole
        Get
            Return TryCast(ViewState("PreviousNodeSelected"), ProtocolRole)
        End Get
        Set(value As ProtocolRole)
            ViewState("PreviousNodeSelected") = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        InitializeControls()
        If Not IsPostBack Then
            Initialize()
            SelectedRoleChange(uscAutorizza.TreeViewControl, New RadTreeNodeEventArgs(uscAutorizza.TreeViewControl.SelectedNode))
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click

        Try
            ' verifico se ci sono Annessi (non parte integrante) aggiunti
            If uscAnnexes.DocumentInfosAdded.Count > 0 Then
                If DocSuiteContext.Current.ProtocolEnv.IsConservationEnabled Then
                    CurrentProtocol.ConservationStatus = "M"c
                End If
                Facade.ProtocolFacade.AddAnnexes(CurrentProtocol, uscAnnexes.DocumentInfosAdded)
            End If

            Dim roleIdsToRemove As IList(Of Integer) = New List(Of Integer)

            For Each roleChanged As ProtocolRoleNoteModel In RoleNoteChanged
                Dim protRole As ProtocolRole = CurrentProtocol.Roles.SingleOrDefault(Function(s) s.Role.Id.Equals(roleChanged.IdRole))
                If protRole Is Nothing Then Continue For
                If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso Not roleChanged.Status.Equals(ProtocolRoleStatus.ToEvaluate) Then
                    If roleChanged.Status.Equals(Convert.ToInt16(ProtocolRoleStatus.Refused)) Then
                        If String.IsNullOrEmpty(roleChanged.Note) Then
                            AjaxAlert("Sono presenti dei settori rifiutati senza note. Le note sono obbligatorie nel caso di rifiuto.")
                            Exit Sub
                        End If
                        roleIdsToRemove.Add(roleChanged.IdRole)
                        CreateRejectedChangeLog(protRole, CType(roleChanged.Status, ProtocolRoleStatus), roleChanged.Note)
                    ElseIf roleChanged.Status.Equals(Convert.ToInt16(ProtocolRoleStatus.Accepted)) Then
                        protRole.Status = CType(roleChanged.Status, ProtocolRoleStatus)
                        CreateRejectedChangeLog(protRole, CType(roleChanged.Status, ProtocolRoleStatus), roleChanged.Note)
                    End If
                End If
                CreateFieldChangeLog(String.Format("Note settore {0} ({1})", protRole.Role.Id, protRole.Role.Name), protRole.Note, roleChanged.Note)
                protRole.Note = roleChanged.Note
                protRole.LastChangedDate = DateTimeOffset.UtcNow
                protRole.LastChangedUser = DocSuiteContext.Current.User.FullUserName

                If ProtocolEnv.ProtocolNoteReservedRoleEnabled Then
                    protRole.NoteType = CType(roleChanged.NoteType, ProtocolRoleNoteType)
                End If
            Next

            'Modifico eventuali valori non settati in view state
            If uscAutorizza.SelectedRole IsNot Nothing AndAlso Not RoleNoteChanged.Any(Function(r) r.IdRole.Equals(uscAutorizza.SelectedRole.Id)) Then
                Dim protRole As ProtocolRole = CurrentProtocol.Roles.SingleOrDefault(Function(s) s.Role.Id.Equals(uscAutorizza.SelectedRole.Id))
                If protRole IsNot Nothing Then
                    protRole.Note = txtNote.Text
                    protRole.LastChangedDate = DateTimeOffset.UtcNow
                    protRole.LastChangedUser = DocSuiteContext.Current.User.FullUserName

                    If ProtocolEnv.ProtocolNoteReservedRoleEnabled Then
                        protRole.NoteType = CType(Convert.ToInt16(cbVisibilityNoteRole.Checked), ProtocolRoleNoteType)
                    End If

                    If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso Not String.IsNullOrEmpty(rblAcceptance.SelectedValue) Then
                        If Convert.ToInt16(rblAcceptance.SelectedValue).Equals(Convert.ToInt16(ProtocolRoleStatus.Refused)) AndAlso Not roleIdsToRemove.Contains(protRole.Role.Id) Then
                            If String.IsNullOrEmpty(protRole.Note) Then
                                AjaxAlert("Sono presenti dei settori rifiutati senza note. Le note sono obbligatorie nel caso di rifiuto.")
                                Exit Sub
                            End If
                            roleIdsToRemove.Add(protRole.Role.Id)
                        ElseIf Convert.ToInt16(rblAcceptance.SelectedValue).Equals(Convert.ToInt16(ProtocolRoleStatus.Accepted)) Then
                            protRole.Status = CType(rblAcceptance.SelectedValue, ProtocolRoleStatus)
                        End If
                        CreateRejectedChangeLog(protRole, CType(rblAcceptance.SelectedValue, ProtocolRoleStatus), txtNote.Text)
                    End If
                End If
            End If

            If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso roleIdsToRemove.Count() > 0 Then
                Facade.ProtocolRejectedRoleFacade.RejectProtocols(CurrentProtocol, roleIdsToRemove)
            Else
                Facade.ProtocolFacade.Update(CurrentProtocol)
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in fase di salvataggio Note di settore di Protocollo.", ex)
            AjaxAlert("Errore in fase di salvataggio Note di settore di Protocollo.")
            Exit Sub
        End Try

        Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub


    Public Sub SelectedRoleChange(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)
        SavePreviousNodeOnViewState()

        txtNote.Visible = False
        rblAcceptance.Visible = False
        cbVisibilityNoteRole.Visible = False

        If uscAutorizza.SelectedRole Is Nothing Then Exit Sub

        txtNote.Visible = True
        If ProtocolEnv.RefusedProtocolAuthorizationEnabled Then
            rblAcceptance.Visible = True
            rblAcceptance.ClearSelection()
        End If

        If ProtocolEnv.ProtocolNoteReservedRoleEnabled Then
            cbVisibilityNoteRole.Visible = True
            cbVisibilityNoteRole.Checked = False
        End If

        Dim protRole As ProtocolRole = CurrentProtocol.Roles.SingleOrDefault(Function(s) s.Role.Id.Equals(uscAutorizza.SelectedRole.Id))
        If protRole Is Nothing Then Exit Sub

        If rblAcceptance.Visible Then
            rblAcceptance.Enabled = True
            If Not protRole.Status.Equals(ProtocolRoleStatus.ToEvaluate) Then
                rblAcceptance.Enabled = False
            End If
        End If

        txtNote.Text = protRole.Note

        If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso rblAcceptance.Visible AndAlso Not protRole.Status.Equals(ProtocolRoleStatus.ToEvaluate) Then
            rblAcceptance.SelectedValue = Convert.ToInt16(protRole.Status).ToString()
        End If

        If ProtocolEnv.ProtocolNoteReservedRoleEnabled AndAlso cbVisibilityNoteRole.Visible AndAlso Not protRole.NoteType.Equals(ProtocolRoleNoteType.Accessible) Then
            cbVisibilityNoteRole.Checked = Convert.ToBoolean(protRole.NoteType)
        End If

        PreviousNodeSelected = protRole

        If RoleNoteChanged.Any(Function(r) r.IdRole.Equals(protRole.Role.Id)) Then
            Dim roleChanged As ProtocolRoleNoteModel = RoleNoteChanged.First(Function(r) r.IdRole.Equals(protRole.Role.Id))
            txtNote.Text = roleChanged.Note
            If roleChanged.Status.HasValue Then
                rblAcceptance.SelectedValue = roleChanged.Status.Value.ToString()
            End If

            If roleChanged.NoteType.HasValue Then
                cbVisibilityNoteRole.Checked = Convert.ToBoolean(roleChanged.NoteType.Value)
            End If

        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, txtNote, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, rblAcceptance, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, cbVisibilityNoteRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAllegati)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAnnexes)
        AddHandler uscAutorizza.TreeViewControl.NodeClick, AddressOf SelectedRoleChange
    End Sub

    Private Sub Initialize()
        If ProtocolEnv.RefusedProtocolAuthorizationEnabled Then
            Title = "Protocollo - Accettazione e Note"
        End If

        ' Recupero Documento principale
        Dim doc As BiblosDocumentInfo = ProtocolFacade.GetDocument(CurrentProtocol)
        If doc IsNot Nothing Then
            uscDocumento.LoadDocumentInfo(doc)
        End If
        ' Recupero Allegati
        Dim attachs() As BiblosDocumentInfo = ProtocolFacade.GetAttachments(CurrentProtocol)
        If attachs.Length > 0 Then
            uscAllegati.LoadDocumentInfo(attachs.Cast(Of DocumentInfo).ToList())
        End If
        ' Recupero Annessi (non parte integrante)
        Dim annexes() As BiblosDocumentInfo = ProtocolFacade.GetAnnexes(CurrentProtocol)
        If annexes.Length > 0 Then
            uscAnnexes.LoadDocumentInfo(annexes.Cast(Of DocumentInfo).ToList())
        End If

        'inizializza settori
        If CurrentProtocol.Roles IsNot Nothing Then
            Dim roleRights As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, 1, True)

            Dim roles As New List(Of Role)
            For Each protRole As ProtocolRole In CurrentProtocol.Roles
                If roleRights.Contains(protRole.Role) Then
                    roles.Add(protRole.Role)
                End If
            Next

            uscAutorizza.SourceRoles = roles
            uscAutorizza.DataBind()
        End If

    End Sub

    Private Sub InitializeControls()
        If CurrentProtocol Is Nothing Then
            Throw New DocSuiteException("Errore lettura Protocollo", String.Format("Impossibile trovare protocollo [{0}]", CurrentProtocolId))
        End If
        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso Facade.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, 2, True).Count = 0 Then
            Throw New DocSuiteException("Errore diritti Protocollo", String.Format("Mancano diritti di Autorizzazione sul protocollo [{0}]", CurrentProtocol.FullNumber))
        End If

        ' Blocco la modifica a chi non è autorizzato
        If Not CurrentProtocolRights.CanAddDocuments Then
            uscAnnexes.ReadOnly = True
        Else
            uscAnnexes.SignButtonEnabled = ProtocolEnv.IsFDQEnabled
            ' Copia da protocollo
            uscAnnexes.ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled
            uscAnnexes.ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled
            ' Copia da atto
            If (DocSuiteContext.Current.IsResolutionEnabled) Then
                uscAnnexes.ButtonCopyResl.Visible = ResolutionEnv.CopyReslDocumentsEnabled
            End If
        End If

        If ProtocolEnv.RefusedProtocolAuthorizationEnabled Then
            colAcceptanceRole.Style.Add("display", "normal")
        End If

        If ProtocolEnv.ProtocolNoteReservedRoleEnabled Then
            colVisibilityNoteRole.Style.Add("display", "normal")
        End If

        ' Visualizza dati protocollo  
        uscProtocollo.CurrentProtocol = CurrentProtocol
        uscProtocollo.VisibleProtocollo = True
        uscProtocollo.VisibleOggetto = True
        uscProtocollo.VisibleAltri = True
        If CurrentProtocol.Type.Id = -1 Then
            uscProtocollo.VisibleProtocolloMittente = True
        End If
    End Sub

    Private Sub CreateFieldChangeLog(ByVal message As String, ByVal oldValue As String, ByVal newValue As String)
        If oldValue.Eq(newValue) Then
            Exit Sub
        End If

        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, message & " (old): " & oldValue)
        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, message & " (new): " & newValue)
    End Sub

    Private Sub CreateRejectedChangeLog(protocolRole As ProtocolRole, status As ProtocolRoleStatus, note As String)
        Dim message As String = String.Empty
        If status.Equals(ProtocolRoleStatus.Accepted) Then
            message = "accettato"
        ElseIf status.Equals(ProtocolRoleStatus.Refused) Then
            message = "rifiutato"
        End If
        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.AR,
                                                String.Format("Prot.{0} - Settore {1} {2} ({3}) con Note: {4}",
                                                              CurrentProtocol.FullNumber, message, protocolRole.Role.Id, protocolRole.Role.Name, note))
    End Sub


    Private Sub SavePreviousNodeOnViewState()
        If PreviousNodeSelected IsNot Nothing Then
            Dim roleToUpdate As ProtocolRole = CurrentProtocol.Roles.SingleOrDefault(Function(x) x.Id.Equals(PreviousNodeSelected.Id))
            If roleToUpdate IsNot Nothing Then
                If roleToUpdate.Note.GetValueOrEmpty().Equals(txtNote.Text) AndAlso
                    ((String.IsNullOrEmpty(rblAcceptance.SelectedValue) AndAlso roleToUpdate.Status = ProtocolRoleStatus.ToEvaluate) OrElse
                    roleToUpdate.Status = Convert.ToInt32(rblAcceptance.SelectedValue)) AndAlso 'then
                    ((Not cbVisibilityNoteRole.Checked AndAlso roleToUpdate.NoteType = ProtocolRoleNoteType.Accessible) OrElse
                    roleToUpdate.NoteType = Convert.ToInt16(cbVisibilityNoteRole.Checked)) Then

                    If RoleNoteChanged.Any(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id)) Then
                        Dim itemToRemove As ProtocolRoleNoteModel = RoleNoteChanged.First(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id))
                        RoleNoteChanged.Remove(itemToRemove)
                    End If
                    Dim node As RadTreeNode = uscAutorizza.TreeViewControl.FindNodeByValue(roleToUpdate.Role.Id.ToString())
                    node.Text = roleToUpdate.Role.Name
                Else
                    If RoleNoteChanged.Any(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id)) Then
                        RoleNoteChanged.First(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id)).Note = txtNote.Text
                        If Not String.IsNullOrEmpty(rblAcceptance.SelectedValue) Then
                            RoleNoteChanged.First(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id)).Status = Convert.ToInt16(rblAcceptance.SelectedValue)
                        End If

                        If ProtocolEnv.ProtocolNoteReservedRoleEnabled Then
                            RoleNoteChanged.First(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id)).NoteType = Convert.ToInt16(cbVisibilityNoteRole.Checked)
                        End If

                    Else
                        Dim itemToAdd As ProtocolRoleNoteModel = New ProtocolRoleNoteModel()
                        itemToAdd.IdRole = roleToUpdate.Role.Id
                        itemToAdd.Note = txtNote.Text
                        If Not String.IsNullOrEmpty(rblAcceptance.SelectedValue) Then
                            itemToAdd.Status = Convert.ToInt16(rblAcceptance.SelectedValue)
                        End If

                        If ProtocolEnv.ProtocolNoteReservedRoleEnabled Then
                            itemToAdd.NoteType = Convert.ToInt16(cbVisibilityNoteRole.Checked)
                        End If

                        RoleNoteChanged.Add(itemToAdd)
                        Dim node As RadTreeNode = uscAutorizza.TreeViewControl.FindNodeByValue(roleToUpdate.Role.Id.ToString())
                        node.Text = String.Format("{0} *", node.Text)
                    End If
                End If
            End If
        End If
    End Sub

#End Region

End Class