Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Linq

Public Class ProtReassignment
    Inherits ProtBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        uscDestinatari.CurrentProtocol = CurrentProtocol
        If Not Page.IsPostBack Then
            If Not CurrentProtocolRights.IsRejected Then
                Throw New DocSuiteException("Riassegnazione Protocollo", "Impossibile accedere alla pagina, il protocollo non è rigettato.")
            End If

            InitializePage()
        End If
    End Sub

    Private Sub RcbContainerItemsRequested(sender As RadComboBox, e As RadComboBoxItemsRequestedEventArgs) Handles rcbContainer.ItemsRequested
        BindContainers(e.Text)
    End Sub

    Private Sub rcbContainer_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles rcbContainer.SelectedIndexChanged
        BindOChartForContainer()
    End Sub

    Private Sub ProtReassignmentAjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        Dim args As String() = e.Argument.Split("|"c)
        If args.IsNullOrEmpty() Then
            Throw New DocSuiteException("Riassegnazione Protocollo", "AjaxRequest non valida: parametri mancanti.")
        End If

        Dim senderId As String = args(0)
        Select senderId
            Case "ROLES"
                If Not args.Length.Equals(3) Then
                    Throw New Exception("AjaxRequest non valida: " & e.Argument)
                End If

                Dim actionName As String = args(1)
                Dim argValue As String = args(2)
                Select Case actionName
                    Case "ADD"
                        Dim roleIds As IEnumerable(Of Integer) = argValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(Function(s) Integer.Parse(s))
                        Dim roles As IEnumerable(Of Role) = roleIds.Select(Function(i) FacadeFactory.Instance.RoleFacade.GetById(i))
                        myRolecontrol.AddItems(roles.ToList(), Function(r) Not r.IsActiveRange())
                End Select
        End Select
    End Sub

    Private Sub TbRoleControlButtonClick(sender As Object, e As RadToolBarEventArgs) Handles tbRoleControl.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)

        Select Case sourceControl.CommandName
            Case "REMOVE"
                Dim role As Role = myRolecontrol.GetSelectedItem()
                If myRolecontrol.IsExplicit(role) Then
                    myRolecontrol.RemoveItem(role)
                End If
        End Select
    End Sub

    Private Sub btnReassign_Click(sender As Object, e As EventArgs) Handles btnReassign.Click
        If (rcbContainer.SelectedValue = ProtocolEnv.ProtocolRejectionContainerId.ToString()) Then
            AjaxAlert("Spostare dal contenitore [{0}].", Facade.ProtocolFacade.RejectionContainer.Name)
            Exit Sub
        End If
        ' Aggiorno le autorizzazioni
        Dim rolesToRemove As List(Of Role) = CurrentProtocol.Roles.Select(Function(x) x.Role).ToList()
        For Each role As Role In rolesToRemove
            Facade.ProtocolFacade.RemoveRoleAuthorization(CurrentProtocol, role)
        Next
        Dim rolesToAdd As IList(Of Role) = myRolecontrol.GetItems()
        For Each role As Role In rolesToAdd
            Facade.ProtocolFacade.AddRoleAuthorization(CurrentProtocol, role, "E")
        Next
        Facade.ProtocolFacade.UpdateOnly(CurrentProtocol)
        ' Estraggo i destinatari da aggiungere
        Dim recipientsToAdd As IList(Of ContactDTO) = uscDestinatari.GetContacts(True)
        ' Elimino i destinatari da rubrica già presenti
        Dim pcList As List(Of ProtocolContact) = CurrentProtocol.Contacts.ToList()
        pcList.RemoveAll(Function(pc) pc.ComunicationType <> ProtocolContactCommunicationType.Recipient)
        For Each protocolContact As ProtocolContact In pcList
            CurrentProtocol.Contacts.Remove(protocolContact)
        Next
        ' Elimino i destinatari manuali già presenti
        Dim pcmList As List(Of ProtocolContactManual) = CurrentProtocol.ManualContacts.ToList()
        pcmList.RemoveAll(Function(pcm) pcm.ComunicationType <> ProtocolContactCommunicationType.Recipient)
        For Each protocolContactManual As ProtocolContactManual In pcmList
            CurrentProtocol.ManualContacts.Remove(protocolContactManual)
        Next

        Facade.ProtocolFacade.UpdateOnly(CurrentProtocol)
        ' Aggiungo i nuovi destinatari
        For Each contact As ContactDTO In recipientsToAdd
            Select Case contact.ContactPart
                Case ContactTypeEnum.AddressBook
                    ProtocolContactFacade.BindContactToProtocol(CurrentProtocol, contact.Contact, ProtocolContactCommunicationType.Recipient, contact.IsCopiaConoscenza)
                Case Else
                    ProtocolContactManualFacade.BindContactToProtocol(CurrentProtocol, contact.Contact, ProtocolContactCommunicationType.Recipient, contact.IsCopiaConoscenza)
            End Select
        Next
        ' Aggiorno i container
        CurrentProtocol.Container = Facade.ContainerFacade.GetById(Integer.Parse(rcbContainer.SelectedValue), False, "ProtDB")
        ' Cambio lo stato
        Facade.ProtocolFacade.Activation(CurrentProtocol)
        ' Rigenero le firme dei documenti
        Facade.ProtocolFacade.RegenerateSignatures(CurrentProtocol)

        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PR, String.Format("Protocollo riattivato da {0} e spostato in [{1}]", DocSuiteContext.Current.User.FullUserName, CurrentProtocol.Container.Name))

        Facade.ProtocolFacade.Update(CurrentProtocol)
        Response.Redirect($"../Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf ProtReassignmentAjaxRequest

        AjaxManager.AjaxSettings.AddAjaxSetting(tbRoleControl, myRolecontrol, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbContainer, uscDestinatari, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbContainer, myRolecontrol, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDestinatari, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, myRolecontrol, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub InitializePage()
        ' Varie
        lbTitle.Text = "Protocollo rigettato"
        lblYear.Text = CurrentProtocol.Year.ToString()
        lblNumber.Text = CurrentProtocol.Number.ToString.PadLeft(7, "0"c)
        lblRegistrationDate.Text = String.Format(ProtocolEnv.ProtRegistrationDateFormat, CurrentProtocol.RegistrationDate.ToLocalTime())
        imgReject.ImageUrl = ImagePath.SmallReject
        lblReject.Text = If(String.IsNullOrEmpty(CurrentProtocol.LastChangedReason), "Nessun motivo esplicito.", CurrentProtocol.LastChangedReason)

        ' Contenitori
        BindContainers("")
        rcbContainer.SelectedValue = CurrentProtocol.Container.Id.ToString()

        ' Destinatari
        Select Case CurrentProtocol.Type.Id
            Case -1
                If ProtocolEnv.InnerContactRoot.HasValue Then
                    uscDestinatari.ExcludeContacts = Nothing
                    uscDestinatari.ContactRoot = ProtocolEnv.InnerContactRoot.Value
                End If

            Case 1
                uscDestinatari.IsRequired = True

                If ProtocolEnv.InnerContactRoot.HasValue Then
                    uscDestinatari.ExcludeContacts = ProtocolEnv.InnerContactRoot.Value.AsList()
                    uscDestinatari.ContactRoot = Nothing
                End If

            Case 0
                uscDestinatari.ExcludeContacts = Nothing
                uscDestinatari.ContactRoot = ProtocolEnv.InnerContactRoot

        End Select

        uscDestinatari.ReadOnly = False
        uscDestinatari.EnableCompression = False
        uscDestinatari.MultiSelect = True
        uscDestinatari.ButtonSelectVisible = True
        uscDestinatari.ButtonSelectDomainVisible = DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
        uscDestinatari.ButtonDeleteVisible = True
        uscDestinatari.ButtonManualVisible = True
        uscDestinatari.ButtonPropertiesVisible = True
        uscDestinatari.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
        uscDestinatari.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled

        uscDestinatari.DataSource = Facade.ProtocolFacade.GetRecipients(CurrentProtocol)
        uscDestinatari.DataBind()

        ' Settori
        myRolecontrol.Clear()
        If Not CurrentProtocol.Roles.IsNullOrEmpty() Then
            For Each pr As ProtocolRole In CurrentProtocol.Roles
                myRolecontrol.LoadItem(pr.Role, String.Empty, Not pr.Role.IsActive OrElse Not pr.Role.IsActiveRange())
            Next
        End If
    End Sub

    Private Sub BindContainers(filter As String)
        rcbContainer.Items.Clear()
        Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetManageableContainers(filter, CurrentProtocol.Container)
        For Each c As Container In availableContainers
            rcbContainer.Items.Add(New RadComboBoxItem(c.Name, c.Id.ToString()))
        Next
        If availableContainers.Count = 1 Then
            ' Se è disponibile un singolo contenitore lo seleziono di default.
            rcbContainer.SelectedValue = availableContainers.Item(0).Id.ToString()
        End If
    End Sub

    Public Function GetWindowParameters() As String
        Dim parameters As New StringBuilder
        parameters.AppendFormat("DSWEnvironment={0}", DSWEnvironment.Protocol)

        If Not String.IsNullOrEmpty(Type) Then
            parameters.AppendFormat("&Type={0}", Type)
        End If
        parameters.AppendFormat("&MultiSelect={0}", True)
        parameters.AppendFormat("&isActive={0}", True)

        Return parameters.ToString()
    End Function

    Private Function GetSelectedContainer() As Container
        Dim selectedId As Integer
        If Integer.TryParse(rcbContainer.SelectedValue, selectedId) Then
            Return FacadeFactory.Instance.ContainerFacade.GetById(selectedId)
        End If
        Return Nothing
    End Function

    Private Sub BindOChartForProtocolIn(selectedContainer As Container)
        ' Recupero i CONDIVISORI della risorsa contenitore.
        Dim sharers As IEnumerable(Of OChartItem) = CommonShared.EffectiveOChart.Items.FindResourceSharers(selectedContainer)
        If sharers.IsNullOrEmpty() Then
            Return
        End If

        uscDestinatari.DataSource.Clear()
        uscDestinatari.DataBind()
        myRolecontrol.Clear()

        ' Precompilo i destinatari con i contatti recuperati dai CONDIVISORI della risorsa contenitore.
        Dim itemContacts As IEnumerable(Of OChartItemContact) = sharers.SelectMany(Function(s) s.Contacts)
        If Not itemContacts.IsNullOrEmpty() Then
            Dim contacts As IEnumerable(Of Integer) = itemContacts.Select(Function(c) c.Contact.Id)
            Dim renewedContacts As IEnumerable(Of Contact) = contacts.Select(Function(c) FacadeFactory.Instance.ContactFacade.GetById(c))
            Dim dtos As IEnumerable(Of ContactDTO) = renewedContacts.Select(Function(c) New ContactDTO(c, ContactDTO.ContactType.Address))
            uscDestinatari.DataSource = dtos.ToList()
            uscDestinatari.DataBind()
        End If

        ' Precompilo le autorizzazioni ai settori con i settori recuperati dai CONDIVISORI della risorsa contenitore.
        Dim itemRoles As IEnumerable(Of OChartItemRole) = sharers.SelectMany(Function(r) r.Roles)
        If Not itemRoles.IsNullOrEmpty() Then
            Dim roles As IEnumerable(Of Integer) = itemRoles.Select(Function(r) r.Role.Id)
            Dim renewedRoles As IEnumerable(Of Role) = FacadeFactory.Instance.RoleFacade.GetByIds(roles.ToList())
            myRolecontrol.Clear()
            myRolecontrol.LoadItems(renewedRoles, Function(r) Not r.IsActiveRange())
        End If
    End Sub

    Private Sub BindOChartForContainer()
        If Not DocSuiteContext.Current.ProtocolEnv.OChartProtocolPreloadingEnabled Then
            Return
        End If

        Dim availableProtocolTypes As New List(Of Integer) From {-1, 0}
        If Not availableProtocolTypes.Contains(CurrentProtocol.Type.Id) Then
            Return
        End If

        Dim selectedContainer As Container = GetSelectedContainer()
        If selectedContainer Is Nothing Then
            Return
        End If

        Select Case CurrentProtocol.Type.Id
            Case -1 ' Ingresso
                BindOChartForProtocolIn(selectedContainer)
            Case 0 ' Tra uffici
                BindOChartForProtocolIn(selectedContainer)

            Case Else
                Return
        End Select
    End Sub

#End Region

End Class