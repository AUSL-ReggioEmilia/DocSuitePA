Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web

Partial Public Class ReslAutorizza
    Inherits ReslBasePage

#Region " Fields "

    Private _resolutionRoleType As ResolutionRoleType

#End Region

#Region " Properties "

    Public ReadOnly Property ResolutionRoleTypeId As Integer
        Get
            Return Request.QueryString.GetValue(Of Integer)("ResolutionRoleTypeId")
        End Get
    End Property

    Public ReadOnly Property ResolutionRoleType As ResolutionRoleType
        Get
            If IsNothing(_resolutionRoleType) Then
                _resolutionRoleType = Facade.ResolutionRoleTypeFacade.GetById(ResolutionRoleTypeId)
            End If
            Return _resolutionRoleType
        End Get
    End Property

    Protected ReadOnly Property ResolutionRoleTypeNotes As String
        Get
            Dim sb As New StringBuilder
            If ResolutionRoleType.SingleRole Then
                sb.Append("È possibile aggiungere un solo settore.")
                sb.Append(WebHelper.Br)
            End If

            Select Case ResolutionRoleType.RoleRestriction
                Case RoleRestrictions.OnlyMine
                    sb.Append("Possono essere aggiunti solo settori a cui appartiene l'operatore.")
                    sb.Append(WebHelper.Br)
                Case RoleRestrictions.Hierarchical
                    sb.Append("Possono essere aggiunti solo settori a cui appartiene l'operatore e relativi sotto-settori.")
                    sb.Append(WebHelper.Br)
            End Select
            Return sb.ToString()

        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        InitializeTab()
        If Not IsPostBack Then
            Initialize()
            Dim coll As Collaboration = Facade.CollaborationFacade.GetByResolution(CurrentResolution)
            If coll Is Nothing OrElse Facade.CollaborationFacade.GetSecretaryRoles(coll, DocSuiteContext.Current.User.FullUserName, CurrentTenant.TenantAOO.UniqueId).IsNullOrEmpty() Then
                cmdAddFromCollaboration.Visible = False
            End If

            If ResolutionEnv.DefaultAuthorizationRoles.Count = 0 Then
                cmdAddDefaults.Visible = False
            End If

            cmdAddFromContact.Visible = False
            If Not CurrentResolution.ResolutionContacts Is Nothing Then
                cmdAddFromContact.Visible = CurrentResolution.ResolutionContacts.Any(Function(f) f.Contact IsNot Nothing AndAlso f.Contact.Role IsNot Nothing)
            End If
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        Dim rolesAdded As ICollection(Of Integer) = uscAutorizza.RoleListAdded
        Dim rolesRemoved As ICollection(Of Integer) = uscAutorizza.RoleListRemoved

        Dim rrf As ResolutionRoleFacade = New ResolutionRoleFacade
        Dim rr As ResolutionRole

        If rolesRemoved IsNot Nothing Then
            For Each idRole As Integer In rolesRemoved
                rr = CurrentResolution.ResolutionRoles.FirstOrDefault(Function(r) r.Role.Id = idRole)
                If rr IsNot Nothing Then
                    CurrentResolution.ResolutionRoles.Remove(rr)
                End If
            Next
        End If

        If rolesAdded IsNot Nothing Then
            For Each idRole As Integer In rolesAdded
                Dim role As Role = Facade.RoleFacade.GetById(idRole)
                rr = New ResolutionRole()
                rr.Id.IdResolution = CurrentResolution.Id
                rr.Id.IdRole = idRole
                rr.Id.IdResolutionRoleType = ResolutionRoleType.Id
                rr.UniqueIdResolution = CurrentResolution.UniqueId
                rr.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                rr.RegistrationDate = DateTimeOffset.UtcNow
                rr.Role = role
                CurrentResolution.ResolutionRoles.Add(rr)
            Next
        End If

        Facade.ResolutionFacade.Update(CurrentResolution)
        If ResolutionEnv.IsLogEnabled Then
            Facade.ResolutionLogFacade.InsertRoles(CurrentResolution, rolesAdded, "Add")
            Facade.ResolutionLogFacade.InsertRoles(CurrentResolution, rolesRemoved, "Del")
        End If

        FacadeFactory.Instance.ResolutionFacade.SendUpdateResolutionCommand(CurrentResolution)

        Dim s As String = CommonShared.AppendSecurityCheck(String.Format("type=Resl&idResolution={0}&Action={1}", CurrentResolution.Id, Action))

        Response.Redirect(String.Format("{0}?{1}", GetViewPageName(CurrentResolution.Id), s))

    End Sub

    Private Sub UscAutorizzaRoleRemoving(sender As Object, args As RoleEventArgs) Handles uscAutorizza.RoleRemoving
        If Not CanAddRole(args.Role) Then
            args.Cancel = True
            AjaxAlert("Non è possibile eliminare il settore selezionato.")
        End If

    End Sub

    Private Sub CmdAddDefaultsClick(sender As Object, e As EventArgs) Handles cmdAddDefaults.Click
        If ResolutionEnv.DefaultAuthorizationRoles.Count > 0 Then
            AddRoles(ResolutionEnv.DefaultAuthorizationRoles)
        End If
    End Sub

    Private Sub CmdAddFromContactClick(sender As Object, e As EventArgs) Handles cmdAddFromContact.Click
        Dim roles As New List(Of Role)
        If Not CurrentResolution.ResolutionContacts Is Nothing Then
            For Each contact As ResolutionContact In CurrentResolution.ResolutionContacts
                If contact.Contact.Role IsNot Nothing AndAlso _
                    CurrentResolution.ResolutionRoles.FirstOrDefault(Function(f) f.Role IsNot Nothing AndAlso f.Role.Id = contact.Contact.Role.Id) Is Nothing Then
                    roles.Add(contact.Contact.Role)
                End If
            Next
        End If
        AddRoles(roles)
    End Sub

    Private Sub CmdAddFromCollaborationClick(sender As Object, e As EventArgs) Handles cmdAddFromCollaboration.Click
        Dim coll As Collaboration = Facade.CollaborationFacade.GetByResolution(CurrentResolution)
        AddRoles(Facade.CollaborationFacade.GetSecretaryRoles(coll, DocSuiteContext.Current.User.FullUserName, CurrentTenant.TenantAOO.UniqueId))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAddDefaults, MainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAddFromCollaboration, MainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAddFromContact, MainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, MainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, uscAutorizza)
    End Sub

    Private Sub InitializeTab()
        If CurrentResolution Is Nothing Then
            Throw New DocSuiteException(String.Format("Registrazione n. [{0}]", IdResolution), "Registrazione Inesistente")
        End If

        Title = String.Format("{0} - Autorizzazioni [{1}]", Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type), ResolutionRoleType.Name)

        uscResolution.CurrentResolution = CurrentResolution
        uscResolution.VisibleNumber = True
        uscResolution.VisibleDataRole = True
        uscResolution.ResolutionWorkflow.Visible = False
        uscResolution.VisibleComunication = False
        uscResolution.VisibleComunicationAssMgrAlternative = False
        uscResolution.VisibleComunicationDestPropAlternative = False

    End Sub

    Private Sub Initialize()
        Dim roles As New List(Of Role)
        If Not CurrentResolution.ResolutionRoles.IsNullOrEmpty() Then
            Dim resolutionRoles As IEnumerable(Of Role) = CurrentResolution.ResolutionRoles _
            .Where(Function(resolutionRole) resolutionRole.ResolutionRoleType IsNot Nothing AndAlso resolutionRole.ResolutionRoleType.Id = ResolutionRoleType.Id AndAlso resolutionRole.Role IsNot Nothing) _
            .Select(Function(resolutionRole) resolutionRole.Role) _
            .ToList()

            If Not resolutionRoles.IsNullOrEmpty() Then
                roles.AddRange(resolutionRoles)
            End If
        End If
        lblNote.Text = ResolutionRoleTypeNotes

        uscAutorizza.SourceRoles = roles
        uscAutorizza.DataBind()

        ' Impostazione da ResolutionRoleType
        uscAutorizza.RoleRestictions = ResolutionRoleType.RoleRestriction
        uscAutorizza.MultipleRoles = Not ResolutionRoleType.SingleRole
        uscAutorizza.MultiSelect = Not ResolutionRoleType.SingleRole

        uscAutorizza.Caption = ResolutionRoleType.Name

        rrtName.Text = ResolutionRoleType.Name
    End Sub

    Private Function CanAddRole(role As Role) As Boolean
        ' Verifico se posso rimuovere il Role corrente
        Select Case ResolutionRoleType.RoleRestriction
            Case RoleRestrictions.OnlyMine
                'Posso rimuovere solo se l'operatore ne fa parte
                Return Facade.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Resolution, role)
            Case RoleRestrictions.Hierarchical
                ' Posso rimuovere se è mio o se io sono parte di un settore superiore
                Return Facade.RoleFacade.CurrentUserHierarchicalCheck(DSWEnvironment.Resolution, role, role.IdTenantAOO)
        End Select
        Return True
    End Function

    Private Sub AddRoles(roles As IList(Of Role))
        Dim warn As Boolean
        For Each item As Role In roles
            If CanAddRole(item) Then
                uscAutorizza.AddRole(item, True, False, False, True)

                If ResolutionRoleType.SingleRole Then
                    ' Aggiungo solo il primo
                    Exit For
                End If
            Else
                warn = True
            End If
        Next

        If warn Then
            AjaxAlert("Non tutti i Settori sono stati aggiunti per mancanza di diritti.")
        End If
    End Sub

#End Region

End Class

