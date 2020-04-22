Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class ProtMultiAutorizza
    Inherits ProtBasePage

    Private Function GetProtocolListSessionName() As String
        Return "ProtMultiAutorizza_ProtocolList"
    End Function

    Private _currentManageableRole As String
    Private ReadOnly Property CurrentManageableRole As String
        Get
            If String.IsNullOrEmpty(_currentManageableRole) Then
                If Not String.IsNullOrEmpty(CommonUtil.GroupProtocolManagerSelected) Then
                    ' Se ho una configurazione di Settori abilitati verifico di averne abilitato solo uno.
                    If CommonUtil.GroupProtocolManagerSelected.Replace("|", String.Empty).Split(","c).Length.Equals(1) Then
                        _currentManageableRole = CommonUtil.GroupProtocolManagerSelected.Replace("|", String.Empty)
                    End If
                Else
                    ' Altrimenti verifico se sono manager di un singolo settore.
                    Dim manageableRoles As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Manager, True)

                    If manageableRoles.Count = 1 Then
                        _currentManageableRole = manageableRoles(0).Id.ToString()
                    End If

                End If
            End If

            Return _currentManageableRole
        End Get
    End Property

    Private _currentProtocolList As IList(Of Protocol)
    Private Property CurrentProtocolList As IList(Of Protocol)
        Get
            If _currentProtocolList Is Nothing AndAlso Not String.IsNullOrEmpty(Session(GetProtocolListSessionName)) Then
                _currentProtocolList = New List(Of Protocol)

                Dim yearNumberList As String() = Session(GetProtocolListSessionName).Split(";"c)
                For Each yearNumber As String In yearNumberList
                    Dim currentYearNumber As String() = yearNumber.Split("|"c)
                    Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(currentYearNumber.GetValue(0), currentYearNumber.GetValue(1))

                    _currentProtocolList.Add(currentProtocol)
                Next
            End If

            Return _currentProtocolList
        End Get
        Set(value As IList(Of Protocol))
            _currentProtocolList = value
        End Set

    End Property

    Private Sub VerifyProtocolListRights()
        If CurrentProtocolList Is Nothing OrElse CurrentProtocolList.Count = 0 Then
            Throw New DocSuiteException("Errore verifica", "E' necessario selezionare almeno un protocollo.")
        ElseIf String.IsNullOrEmpty(CurrentManageableRole) Then
            Throw New DocSuiteException("Errore verifica", "E' necessario abilitare un solo settore da Utente>Settori abilitati.")
        Else
            Dim prevRights As ProtocolRights = Nothing
            Dim prevDirection As String = String.Empty
            For Each p As Protocol In CurrentProtocolList
                ' Verifico i permessi per ogni singolo protocollo
                Dim currentRights As New ProtocolRights(p)
                If Not currentRights.IsAuthorizable AndAlso Not currentRights.IsDistributable Then
                    Throw New DocSuiteException("Errore verifica", String.Format("Non si hanno permessi di autorizzazione per il protocollo {0}.", p.FullNumber))
                Else
                    ' Verifico che l'elenco dei protocolli sia omogeneo per permessi di autorizzazione.
                    If prevRights IsNot Nothing AndAlso (Not prevRights.IsAuthorizable.Equals(currentRights.IsAuthorizable) OrElse Not prevRights.IsDistributable.Equals(currentRights.IsDistributable)) Then
                        Throw New DocSuiteException("Errore verifica", "Non si è selezionato un elenco omogeneo per permessi di protocolli.")
                    End If
                End If
                prevRights = currentRights

                ' Verifico che l'elenco dei protocolli sia omogeneo per direzione.
                Dim currentDirection As String = p.Type.Id.ToString()
                If Not String.IsNullOrEmpty(prevDirection) AndAlso Not prevDirection.Equals(currentDirection) Then
                    Throw New DocSuiteException("Errore verifica", "Non si è selezionato un elenco omogeneo per direzione (ingresso/uscita) di protocolli.")
                End If
                prevDirection = currentDirection
            Next
        End If
    End Sub

#Region " Gestione ProtocolRoleUser. "

    ''' <summary>
    ''' Predispone l'usercontrol relativo alle autorizzazioni di distribuzione di protocollo.
    ''' </summary>
    ''' <param name="p_roles">Settori autorizzati</param>
    ''' <param name="p_roleUserViewMode">Modalità di visualizzazione</param>
    Private Overloads Sub BindProtocolRoleUsers(ByVal p_roles As IList(Of Role), ByVal p_roleUserViewMode As uscSettori.RoleUserViewMode?)
        If CurrentProtocolList.Count > 0 Then
            Dim currentRights As New ProtocolRights(CurrentProtocolList.Item(0))
            If ProtocolEnv.IsDistributionEnabled AndAlso currentRights.IsDistributable Then
                uscProtocolRoleUser.Visible = True
                uscProtocolRoleUser.Required = False
                uscProtocolRoleUser.Caption = "Autorizzazioni Responsabile Settore"
                uscProtocolRoleUser.Checkable = True
                uscProtocolRoleUser.TreeViewControl.CheckBoxes = uscProtocolRoleUser.Checkable
                uscProtocolRoleUser.CopiaConoscenzaEnabled = currentRights.IsContainerDistributable
                uscProtocolRoleUser.CurrentRoleUserViewMode = p_roleUserViewMode

                uscProtocolRoleUser.CurrentProtocol = CurrentProtocolList.Item(0)
                uscProtocolRoleUser.SourceRoles = p_roles.ToList()
                uscProtocolRoleUser.ViewDistributableManager = currentRights.IsDistributable AndAlso ProtocolEnv.ProtocolDistributionTypologies.Contains(uscProtocolRoleUser.CurrentProtocol.Type.Id)
                uscProtocolRoleUser.DataBindForRoleUser(Nothing, Nothing)
                uscAutorizza.ReadOnly = Not currentRights.IsDistributable

                If Not currentRights.IsContainerDistributable AndAlso currentRights.IsCurrentUserDistributionManager Then
                    ' Se l'utente corrente è solamente manager di distribuzione gli permetto di autorizzare solo sottosettori di quelli correntemente autorizzati.
                    uscAutorizza.ManageableRoles = uscProtocolRoleUser.GetManageableRolesParameter()
                End If
            Else
                uscProtocolRoleUser.Visible = False
            End If
        End If
    End Sub

    Private Overloads Sub BindProtocolRoleUsers(ByVal p_roleUserViewMode As uscSettori.RoleUserViewMode)
        BindProtocolRoleUsers(uscAutorizza.GetRoles(), p_roleUserViewMode)
    End Sub

    Private Overloads Sub BindProtocolRoleUsers()
        BindProtocolRoleUsers(uscAutorizza.GetRoles(), uscProtocolRoleUser.CurrentRoleUserViewMode)
    End Sub

    Private Enum ProtocolRoleUserColumns
        IdRole = 0
        GroupName = 1
        UserName = 2
        Account = 3
    End Enum

    ''' <summary> Aggiorno le autorizzazioni degli RoleUser per il protocollo corrente. </summary>
    Private Sub UpdateProtocolRoleUser(p_protocol As Protocol)
        If p_protocol.Roles IsNot Nothing AndAlso p_protocol.Roles.Count > 0 Then
            ' Popolo una lista con gli id degli RoleUser da autorizzare.
            Dim selectedProtocolRoleUsers As IList(Of String) = uscProtocolRoleUser.GetRoleValues(True, uscSettori.NodeTypeAttributeValue.RoleUser)

            For Each selectedId As String In selectedProtocolRoleUsers
                Dim pruk As New ProtocolRoleUserKey
                Dim roleUserNodeValue As String() = selectedId.Split("|"c)
                Dim idRole As Integer = Convert.ToInt32(roleUserNodeValue.GetValue(ProtocolRoleUserColumns.IdRole))

                pruk.Year = p_protocol.Year
                pruk.Number = p_protocol.Number
                pruk.IdRole = idRole
                pruk.GroupName = roleUserNodeValue.GetValue(ProtocolRoleUserColumns.GroupName)
                pruk.UserName = roleUserNodeValue.GetValue(ProtocolRoleUserColumns.UserName)

                Dim pru As New ProtocolRoleUser
                pru.Id = pruk
                pru.Account = roleUserNodeValue.GetValue(ProtocolRoleUserColumns.Account)
                pru.IsActive = 1
                pru.UniqueIdProtocol = p_protocol.UniqueId
                pru.Role = Facade.RoleFacade.GetById(idRole)
                pru.Protocol = p_protocol

                ' Popolo una lista con gli id dei settori autorizzati.
                Dim currentProtocolRoles As IList(Of Integer) = New List(Of Integer)
                For Each pr As ProtocolRole In p_protocol.Roles
                    currentProtocolRoles.Add(pr.Id.Id)
                Next

                If currentProtocolRoles.Contains(pruk.IdRole) Then
                    ' Autorizzo il RoleUser corrente.
                    p_protocol.RoleUsers.Add(pru)
                End If
            Next
        End If
    End Sub

    ''' <summary> Aggiorna il flag di copia conoscenza per i Role autorizzati. </summary>
    Private Sub UpdateProtocolRoleDistributionType(p_protocol As Protocol)
        If (p_protocol.Roles Is Nothing) OrElse p_protocol.Roles.Count <= 0 Then
            Exit Sub
        End If

        ' Popolo una lista con gli id dei Role di cui impostare la Copia Conoscenza dalle checkbox dei nodi di settore per i quali sono abilitato.
        Dim ccProtocolRoles As List(Of String) = uscProtocolRoleUser.GetRoleValues(True, uscSettori.NodeTypeAttributeValue.Role)

        Dim currentRights As New ProtocolRights(p_protocol)
        ' Se sono manager riporto le copie conoscenza dei nodi da me non modificabili e già impostati recuperandoli dall'attributo NodeCC.
        If Not currentRights.IsContainerDistributable AndAlso currentRights.IsCurrentUserDistributionManager Then
            Dim ccProtocolRolesByAttribute As New List(Of String)
            For Each node As RadTreeNode In uscProtocolRoleUser.TreeViewControl.Nodes
                uscSettori.GetCcRoleNodes(node, ccProtocolRolesByAttribute)
            Next
            ccProtocolRoles.AddRange(ccProtocolRolesByAttribute)
        End If

        For Each selectedId As String In ccProtocolRoles
            For Each pr As ProtocolRole In p_protocol.Roles
                ' Se il settore corrente è - OPPURE - è figlio di un settore in Copia Conoscenza, propago l'impostazione in cascata.
                If pr.Id.Id.ToString().Equals(selectedId) OrElse pr.Role.FullIncrementalPath.Contains(selectedId) Then
                    pr.Type = ProtocolRoleTypes.CarbonCopy
                    ' Non esco immediatamente dal ciclo poichè oltre al settore corrente potrei avere n-figli di cui impostare la Copia Conoscenza.
                End If
            Next
        Next
    End Sub

    ''' <summary> Aggiorna le autorizzazioni implicite dei Role. </summary>
    Private Sub UpdateImplicitProtocolRoles(protocol As Protocol)
        If (protocol.Roles Is Nothing) OrElse protocol.Roles.Count <= 0 Then
            Exit Sub
        End If

        ' Popolo una lista con gli id dei settori autorizzati esplicitamente.
        Dim currentProtocolRoles As IList(Of Integer) = New List(Of Integer)
        For Each pr As ProtocolRole In protocol.Roles
            currentProtocolRoles.Add(pr.Id.Id)
        Next

        Dim implicitProtocolRoles As String() = uscAutorizza.GetOldValues()
        For Each idRole As String In implicitProtocolRoles
            Dim temp As Integer = Integer.Parse(idRole)
            If Not currentProtocolRoles.Contains(temp) Then
                protocol.AddRole(Facade.RoleFacade.GetById(temp), DocSuiteContext.Current.User.FullUserName, DateTimeOffset.UtcNow, ProtocolDistributionType.Implicit)
            End If
        Next
    End Sub


    Private Sub ExecuteMultiDistribution()
        If Not ProtocolEnv.IsDistributionEnabled Then
            Exit Sub
        End If

        For Each p As Protocol In CurrentProtocolList
            ' Aggiorno autorizzazioni implicite Role (per foreign key).
            UpdateImplicitProtocolRoles(p)

            If p.Type.Id - 1 Then
                ' Aggiorno autorizzazioni RoleUser.
                UpdateProtocolRoleUser(p)
            End If

            ' Aggiorno flag Copia Conoscenza.
            UpdateProtocolRoleDistributionType(p)
        Next
    End Sub

#End Region

    Private Sub LogInsert()
        'Inserimento Logs
        Dim editedRoles As ICollection(Of Integer) = Nothing
        If ProtocolEnv.IsLogEnabled Then
            'Autorizzazioni Aggiunte
            editedRoles = uscAutorizza.RoleListAdded()
            For Each p As Protocol In CurrentProtocolList
                Facade.ProtocolLogFacade.InsertFullRolesLogWithoutRead(p, editedRoles, "Add")
            Next

            'Autorizzazioni Rimosse
            editedRoles = uscAutorizza.RoleListRemoved()
            For Each p As Protocol In CurrentProtocolList
                Facade.ProtocolLogFacade.InsertRolesLogWithoutRead(p, editedRoles, "Del")
            Next
        End If
    End Sub

    Private Sub InizializeSelRole()
        If Not CurrentProtocolList.IsNullOrEmpty() Then
            Dim availableRoles As New List(Of Role)
            If Not CurrentProtocolList.Item(0).Roles.IsNullOrEmpty() Then
                ' Recupera le autorizzazioni
                For Each pr As ProtocolRole In CurrentProtocolList.Item(0).Roles
                    If String.IsNullOrEmpty(pr.Rights) Then
                        availableRoles.Add(pr.Role)
                    End If
                Next
            End If
            uscAutorizza.CurrentProtocol = CurrentProtocolList.Item(0)
            uscAutorizza.SourceRoles = availableRoles
            uscAutorizza.DataBind()

            Dim currentRights As New ProtocolRights(CurrentProtocolList.Item(0))
            If currentRights.IsContainerDistributable OrElse CurrentProtocolList.Item(0).Type.Id.Equals(1) Then
                BindProtocolRoleUsers(availableRoles, uscSettori.RoleUserViewMode.Roles)
            Else
                BindProtocolRoleUsers(availableRoles, uscSettori.RoleUserViewMode.RoleUsers)
            End If
        End If
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequestHandler
        Dim currentRights As New ProtocolRights(CurrentProtocolList.Item(0))
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, uscAutorizza, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, GridProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, uscAutorizza)

        If ProtocolEnv.IsDistributionEnabled AndAlso currentRights.IsDistributable Then
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, uscProtocolRoleUser, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolRoleUser, uscProtocolRoleUser, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, uscProtocolRoleUser, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, GridProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
        End If
    End Sub

    Protected Sub AjaxRequestHandler(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        LoadGrid()
    End Sub

    Private Sub LoadGrid()
        GridProtocols.Visible = True
        GridProtocols.DataSource = CurrentProtocolList
        GridProtocols.DataBind()
    End Sub

    Protected Sub GridProtocols_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles GridProtocols.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item And e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As Protocol = DirectCast(e.Item.DataItem, Protocol)

        ' Aggiungo una label con il nome del protocollo a fianco della chkbox.
        Dim protocolDescription As Label = DirectCast(e.Item.FindControl("ProtocolDescription"), Label)
        Dim currentRegistrationDate As String = String.Format(DocSuiteContext.Current.ProtocolEnv.ProtRegistrationDateFormat, item.RegistrationDate.ToLocalTime())
        protocolDescription.Text = String.Format("{0} del {1} - {2}", item.FullNumber, currentRegistrationDate, item.ProtocolObject)

        ' Inserisco una colonna nascosta dalla quale reperire l'id del protocollo. Questo Id viene utilizzato per eliminare i protocolli autorizzati.
        Dim ProtocolId As HiddenField = DirectCast(e.Item.FindControl("ProtocolId"), HiddenField)
        ProtocolId.Value = item.Id.ToString()

        If (TypeOf (e.Item) Is GridDataItem) Then
            'Get the instance of the right type
            Dim dataBoundItem As GridDataItem = e.Item
            Dim chk As CheckBox = DirectCast(dataBoundItem("SelectColumn").Controls(0), CheckBox)
            If chk IsNot Nothing Then
                chk.Checked = True
                e.Item.Selected = True
            End If
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            ' Load data grid
            If ProtocolEnv.IsDistributionEnabled Then
                LoadGrid()
            End If

            ' Verifica permessi
            VerifyProtocolListRights()
            uscAutorizza.SearchByUserEnabled = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso Not DocSuiteContext.Current.ProtocolEnv.DistributionHierarchicalEnabled
            ' Predispongo la nuova valorizzazione dei settori autorizzabili dai manager di distribuzione.
            uscAutorizza.ResetManageableRoles()
            If ProtocolEnv.MultiDomainEnabled AndAlso ProtocolEnv.TenantAuthorizationEnabled Then
                uscAutorizza.TenantEnabled = True
            End If

            'BindProtocolList()
            InizializeSelRole()
        End If
    End Sub


    Private Sub btnConferma_Click(sender As Object, e As EventArgs) Handles btnConferma.Click
        ' Solo se vengono assegnate delle autorizzazioni posso rimuovere i protocolli.
        If Not uscAutorizza.GetRoles.IsNullOrEmpty() Then

            Dim protocolChecked As List(Of String) = New List(Of String)
            Dim yearNumberList As List(Of String) = New List(Of String)
            ' Verifico i protocolli selezionati e li deposito in una lista per poi filtrarli
            yearNumberList.AddRange(Session(GetProtocolListSessionName).Split(";"c))

            For Each item As GridDataItem In GridProtocols.MasterTableView.Items
                Dim chk As CheckBox = TryCast(item("selectColumn").Controls(0), CheckBox)
                If Not chk Is Nothing AndAlso chk.Checked Then
                    ' creo una lista con gli id da elaborare
                    Dim protocolId As String = DirectCast(item.FindControl("ProtocolId"), HiddenField).Value
                    protocolChecked.Add(protocolId)

                    ' Adeguo il formato della variabile di sessione YEAR|NUMBER con quello dell'Id del protocollo  YEAR/0000NUMBER
                    Dim currentYearNumber As String() = protocolId.Split("/"c)
                    Dim currentProtocol As String = String.Concat(currentYearNumber.GetValue(0), "|"c, CInt(currentYearNumber.GetValue(1)))
                    yearNumberList.RemoveAll(Function(f) f = currentProtocol)
                End If
            Next

            ' Aggiornamento della variabile di sessione
            Dim newProtocolListSession As String
            For Each l As String In yearNumberList
                newProtocolListSession = String.Concat(newProtocolListSession, String.Concat(l, ";"c))
            Next
            Session(GetProtocolListSessionName) = newProtocolListSession

            If CurrentProtocolList IsNot Nothing AndAlso Not CurrentProtocolList.IsNullOrEmpty() Then
                ' Verifica che ci sia almeno un settore autorizzato
                Dim currentRights As New ProtocolRights(CurrentProtocolList.Item(0))

                If currentRights.IsProtocolTypeDistributable AndAlso currentRights.IsDistributable Then
                    ' Verifico che se l'utente corrente è solamente manager di distribuzione ci sia almeno un settore autorizzato.
                    If Not currentRights.IsContainerDistributable AndAlso currentRights.IsCurrentUserDistributionManager AndAlso uscAutorizza.GetRoles.IsNullOrEmpty() Then
                        RadWindow.RadAlert("<b>E' necessario autorizzare almeno un settore.</b>.", 300, 100, "Error", "")
                        Exit Sub
                    End If
                End If

                ' Aggiornamento autorizzazioni per il protocollo.
                Dim currentProtocolSelected As List(Of Protocol) = CurrentProtocolList.Where(Function(p) protocolChecked.Contains(p.Id.ToString())).ToList()

                For Each protocol As Protocol In currentProtocolSelected
                    'workaround per verificare se post attività devo eliminare il log di lettura
                    Dim needReadLogRemove As Boolean = Facade.ProtocolLogFacade.SearchLog(protocol.Year, protocol.Number, DocSuiteContext.Current.User.FullUserName, ProtocolLogEvent.P1).Any()
                    Dim protRights As ProtocolRights = New ProtocolRights(protocol)
                    ' Solo per protocolli in entrata, rimuovo gli Users
                    If protRights.IsProtocolTypeDistributable Then
                        Facade.ProtocolFacade.RemoveRoleUserAuthorizations(protocol, uscProtocolRoleUser.GetRoleValues(False, uscSettori.NodeTypeAttributeValue.RoleUser))
                    End If

                    ' Aggiungo i settori (Log ok)
                    Facade.ProtocolFacade.AddRoleAuthorizations(protocol, uscAutorizza.RoleListAdded(), "E")
                    ' Rimuovo i settori (Log ok)
                    Facade.ProtocolFacade.RemoveRoleAuthorizations(protocol, uscAutorizza.RoleListRemoved())

                    ' Aggiorno i CC
                    If ProtocolEnv.IsDistributionEnabled Then
                        ' Log OK
                        ' Prima devono essere gestiti i CC non selezionati, poi quelli selezionati per po
                        Facade.ProtocolFacade.UpdateRoleAuthorization(protocol, uscProtocolRoleUser.GetFullIncrementalPathAttribute(False, uscSettori.NodeTypeAttributeValue.Role), False)
                        Facade.ProtocolFacade.UpdateRoleAuthorization(protocol, uscProtocolRoleUser.GetFullIncrementalPathAttribute(True, uscSettori.NodeTypeAttributeValue.Role), True)
                    End If

                    ' Solo per protocolli in entrata, aggiungo gli Users
                    If protRights.IsProtocolTypeDistributable Then
                        Facade.ProtocolFacade.AddRoleUserAuthorizations(protocol, uscProtocolRoleUser.GetRoleValues(True, uscSettori.NodeTypeAttributeValue.RoleUser))
                    End If

                    Facade.ProtocolFacade.UpdateOnly(protocol)
                    'Elimino eventuali log di lettura inseriti per l'utente
                    If needReadLogRemove Then
                        Dim logsToRemove As IList(Of ProtocolLog) = Facade.ProtocolLogFacade.SearchLog(protocol.Year, protocol.Number, DocSuiteContext.Current.User.FullUserName, ProtocolLogEvent.P1)
                        If logsToRemove IsNot Nothing Then
                            For Each logToRemove As ProtocolLog In logsToRemove
                                Try
                                    Facade.ProtocolLogFacade.Delete(logToRemove)
                                Catch
                                End Try
                            Next
                        End If
                    End If
                Next

                For Each protocol As Protocol In CurrentProtocolList
                    Facade.ProtocolFacade.SendUpdateProtocolCommand(protocol)
                Next

                ' Aggiorna la lista di protocolli
                CurrentProtocolList = CurrentProtocolList.Where(Function(p) Not protocolChecked.Contains(p.Id.ToString())).ToList()
                LoadGrid()
                InizializeSelRole()
            End If

            BindProtocolRoleUsers()

            If CurrentProtocolList.IsNullOrEmpty() Then
                RadWindow.RadAlert("<b>Autorizzazione completata.</b><br/>Tutti i protocolli sono stati distribuiti", 300, 100, "Info", "callBackFunction", "../App_Themes/DocSuite2008/imgset32/information.png")
            Else
                RadWindow.RadAlert("<b>Autorizzazione completata.</b><br/>Proseguiere con i protocolli rimanenti.", 300, 100, "Info", String.Empty, "../App_Themes/DocSuite2008/imgset32/information.png")
            End If
        Else
            RadWindow.RadAlert("<b>Nessuna autorizzazione selezionata</b>.", 300, 100, "Error", String.Empty)
        End If

    End Sub


    Private Sub uscAutorizza_RoleEdited(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles uscAutorizza.RolesAdded, uscAutorizza.RoleRemoved
        BindProtocolRoleUsers()
    End Sub

    Private Sub uscProtocolRoleUser_OnRoleUserViewManagersChanged(sender As Object, e As System.EventArgs) Handles uscProtocolRoleUser.OnRoleUserViewManagersChanged
        BindProtocolRoleUsers(uscProtocolRoleUser.CurrentRoleUserViewMode)
    End Sub

    Private Sub uscProtocolRoleUser_OnRoleUserViewModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles uscProtocolRoleUser.OnRoleUserViewModeChanged
        If uscProtocolRoleUser.CurrentRoleUserViewMode.Equals(CByte(uscSettori.RoleUserViewMode.RoleUsers)) Then
            BindProtocolRoleUsers(uscSettori.RoleUserViewMode.Roles)
        Else
            BindProtocolRoleUsers(uscSettori.RoleUserViewMode.RoleUsers)
        End If
    End Sub


End Class