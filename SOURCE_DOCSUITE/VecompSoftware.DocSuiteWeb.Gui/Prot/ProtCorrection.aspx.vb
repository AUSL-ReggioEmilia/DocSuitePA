Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers

Public Class ProtCorrection
    Inherits ProtBasePage

#Region "Properties"
    Public ReadOnly Property CurrentAddedRoleList() As ArrayList
        Get
            If (CType(ViewState("CurrentAddedRoleList"), ArrayList)) Is Nothing Then
                ViewState("CurrentAddedRoleList") = New ArrayList()
            End If
            Return CType(ViewState("CurrentAddedRoleList"), ArrayList)
        End Get
    End Property

    Public ReadOnly Property CurrentRemovedRoleList() As ArrayList
        Get
            If (CType(ViewState("CurrentRemovedRoleList"), ArrayList)) Is Nothing Then
                ViewState("CurrentRemovedRoleList") = New ArrayList()
            End If
            Return CType(ViewState("CurrentRemovedRoleList"), ArrayList)
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        AddHandler uscMittenti.ItemsAdded, AddressOf uscContact_ContactAdded
        AddHandler uscDestinatari.ItemsAdded, AddressOf uscContact_ContactAdded
        AddHandler uscMittenti.ContactRemoved, AddressOf uscMittenti_ContactRemoved
        AddHandler uscDestinatari.ContactRemoved, AddressOf uscDestinatari_ContactRemoved

        AjaxManager.AjaxSettings.AddAjaxSetting(btnCorrection, pnlCorrectionContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCorrection, btnCorrection, MasterDocSuite.AjaxFlatLoadingPanel)

        uscMittenti.ButtonContactSmartVisible = ProtocolEnv.ContactSmartEnabled
        uscDestinatari.ButtonContactSmartVisible = ProtocolEnv.ContactSmartEnabled
        If Not Page.IsPostBack Then
            InitializePage()
        End If
    End Sub

    Private Sub RcbContainerItemsRequested(sender As RadComboBox, e As RadComboBoxItemsRequestedEventArgs) Handles rcbContainer.ItemsRequested
        BindContainers(e.Text)
    End Sub

    Private Sub btnCorrection_Click(sender As Object, e As EventArgs) Handles btnCorrection.Click

        ' Aggiorno Oggetto
        Dim PartialObject As String = StringHelper.Clean(uscObject.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        If Not (String.Equals(uscObject.Text, PartialObject)) Then
            uscObject.Text = PartialObject
            AjaxAlert("L'oggetto è stato ripulito in automatico dai caratteri speciali. Verificare che l'oggetto risultante sia valido")
            Exit Sub
        End If

        CurrentProtocol.ProtocolObject = PartialObject

        ' Salvo prima di pulire i campi
        Dim mittenti As IList(Of ContactDTO) = uscMittenti.GetContacts(True)
        Dim destinatari As IList(Of ContactDTO) = uscDestinatari.GetContacts(True)
        Dim newContainer As Container = Facade.ContainerFacade.GetById(Integer.Parse(rcbContainer.SelectedValue), False, "ProtDB")
        AddCorrectionLog(newContainer, mittenti, destinatari)

        ' Elimino i contatti già presenti
        CurrentProtocol.Contacts.Clear()
        CurrentProtocol.ManualContacts.Clear()
        'Aggiorno i settori
        RemoveRoleList()
        AddRoleList()
        Facade.ProtocolFacade.Update(CurrentProtocol)
        ' Aggiungo i nuovi contatti
        CurrentProtocol.AppendSenders(mittenti)
        CurrentProtocol.AppendRecipients(destinatari)
        ' Aggiorno i container
        CurrentProtocol.Container = newContainer
        ' Rigenero le firme dei documenti
        Facade.ProtocolFacade.RegenerateSignatures(CurrentProtocol)
        Facade.ProtocolFacade.Update(CurrentProtocol)
        Facade.ProtocolFacade.SendUpdateProtocolCommand(CurrentProtocol)
        Response.Redirect("../Prot/ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck("Year=" & CurrentProtocolYear & "&Number=" & CurrentProtocolNumber))

    End Sub

    Protected Sub uscContact_ContactAdded(sender As Object, e As EventArgs)
        Dim contactArgs As ContactsEventArgs = TryCast(e, ContactsEventArgs)
        If contactArgs Is Nothing Then
            Exit Sub
        End If

        If contactArgs.Contacts Is Nothing Then
            Exit Sub
        End If

        For Each contact As Contact In contactArgs.Contacts
            If contact.Role Is Nothing Then
                Continue For
            End If

            AddRolesByContact(contact)
        Next
    End Sub

    Protected Sub uscMittenti_ContactRemoved(sender As Object, e As EventArgs)
        Dim idContact As Integer = 0
        Select Case uscMittenti.LastRemovedNode.Attributes("ContactPart")
            Case "ADDRESSBOOK"
                idContact = Convert.ToInt32(uscMittenti.LastRemovedNode.Value)
        End Select

        If idContact.Equals(0) Then
            Exit Sub
        End If

        Dim lastRemoved As Contact = Facade.ContactFacade.GetById(idContact)
        If lastRemoved Is Nothing Then
            Exit Sub
        End If

        If lastRemoved.Role Is Nothing Then
            Exit Sub
        End If

        RemoveRolesByContact(lastRemoved)
    End Sub

    Protected Sub uscDestinatari_ContactRemoved(sender As Object, e As EventArgs)
        Dim idContact As Integer = 0
        Select Case uscDestinatari.LastRemovedNode.Attributes("ContactPart")
            Case "ADDRESSBOOK"
                idContact = Convert.ToInt32(uscDestinatari.LastRemovedNode.Value)
        End Select

        If idContact.Equals(0) Then
            Exit Sub
        End If

        Dim lastRemoved As Contact = Facade.ContactFacade.GetById(idContact)
        If lastRemoved Is Nothing Then
            Exit Sub
        End If

        If lastRemoved.Role Is Nothing Then
            Exit Sub
        End If

        RemoveRolesByContact(lastRemoved)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializePage()
        ' Varie
        uscProtocolPreview.CurrentProtocol = CurrentProtocol
        uscProtocolPreview.Initialize()
        uscObject.MaxLength = ProtocolEnv.ObjectMaxLength
        uscObject.Text = CurrentProtocol.ProtocolObject

        ' Contenitori
        BindContainers("")
        rcbContainer.Enabled = ProtocolEnv.ProtocolContainerEditable AndAlso Not (CurrentProtocol.Container.IsInvoiceEnable OrElse CurrentProtocolRights.IsRejected OrElse ProtocolEnv.ProtocolRejectionEnabled)
        rcbContainer.SelectedValue = CurrentProtocol.Container.Id.ToString()


        ' Contatti
        Select Case CurrentProtocol.Type.Id
            Case -1
                uscMittenti.IsRequired = True

                If ProtocolEnv.InnerContactRoot.HasValue Then
                    uscMittenti.ExcludeContacts = ProtocolEnv.InnerContactRoot.Value.AsList()
                    uscMittenti.ContactRoot = Nothing
                    uscDestinatari.ExcludeContacts = Nothing
                    uscDestinatari.ContactRoot = ProtocolEnv.InnerContactRoot.Value
                End If

            Case 1
                uscDestinatari.IsRequired = True

                If ProtocolEnv.InnerContactRoot.HasValue Then
                    uscMittenti.ExcludeContacts = Nothing
                    uscMittenti.ContactRoot = ProtocolEnv.InnerContactRoot.Value
                    uscDestinatari.ExcludeContacts = ProtocolEnv.InnerContactRoot.Value.AsList()
                    uscDestinatari.ContactRoot = Nothing
                End If

            Case 0
                uscMittenti.ExcludeContacts = Nothing
                uscMittenti.ContactRoot = ProtocolEnv.InnerContactRoot
                uscDestinatari.ExcludeContacts = Nothing
                uscDestinatari.ContactRoot = ProtocolEnv.InnerContactRoot

        End Select

        ' Mittenti
        uscMittenti.CurrentProtocol = CurrentProtocol
        uscMittenti.ReadOnly = False
        uscMittenti.EnableCompression = False
        uscMittenti.MultiSelect = True
        uscMittenti.ButtonSelectVisible = True
        uscMittenti.ButtonSelectDomainVisible = True
        uscMittenti.ButtonSelectOChartVisible = True
        uscMittenti.ButtonDeleteVisible = True
        uscMittenti.ButtonManualVisible = True
        uscMittenti.ButtonPropertiesVisible = True
        uscMittenti.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
        uscMittenti.ButtonIPAVisible = (Not String.IsNullOrEmpty(ProtocolEnv.LdapIndicePa))

        uscMittenti.DataSource = Facade.ProtocolFacade.GetSenders(CurrentProtocol)
        uscMittenti.DataBind()

        ' Destinatari
        uscDestinatari.CurrentProtocol = CurrentProtocol
        uscDestinatari.ReadOnly = False
        uscDestinatari.EnableCompression = False
        uscDestinatari.MultiSelect = True
        uscDestinatari.ButtonSelectVisible = True
        uscDestinatari.ButtonSelectDomainVisible = True
        uscDestinatari.ButtonSelectOChartVisible = True
        uscDestinatari.ButtonDeleteVisible = True
        uscDestinatari.ButtonManualVisible = True
        uscDestinatari.ButtonPropertiesVisible = True
        uscDestinatari.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
        uscDestinatari.ButtonIPAVisible = Not String.IsNullOrEmpty(ProtocolEnv.LdapIndicePa)

        uscDestinatari.DataSource = Facade.ProtocolFacade.GetRecipients(CurrentProtocol)
        uscDestinatari.DataBind()
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

    Private Sub AddRolesByContact(contact As Contact)
        If CurrentRemovedRoleList.Contains(contact.Role.Id) Then
            CurrentRemovedRoleList.Remove(contact.Role.Id)
        End If

        If Not CurrentAddedRoleList.Contains(contact.Role.Id) Then
            If CurrentProtocol.Roles.Any(Function(x) x.Role.Id.Equals(contact.Role.Id)) Then
                Exit Sub
            End If
            CurrentAddedRoleList.Add(contact.Role.Id)
        End If
    End Sub

    Private Sub RemoveRolesByContact(contact As Contact)
        If CurrentAddedRoleList.Contains(contact.Role.Id) Then
            CurrentAddedRoleList.Remove(contact.Role.Id)
        Else
            CurrentRemovedRoleList.Add(contact.Role.Id)
        End If
    End Sub

    Private Sub AddRoleAuthorization(idRole As Integer, distributionType As String)
        Dim role As Role = Facade.RoleFacade.GetById(idRole)
        CurrentProtocol.AddRole(role, DocSuiteContext.Current.User.FullUserName, DateTimeOffset.UtcNow, distributionType)

        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso Not role.Father Is Nothing AndAlso Not _
            Me.CurrentProtocol.Roles.Any(Function(x) x.Role.Id.Equals(role.Father.Id)) Then
            AddRoleAuthorization(role.Father.Id, ProtocolDistributionType.Implicit)
        End If
    End Sub

    Private Sub RemoveRoleAuthorization(role As Role)
        Me.CurrentProtocol.RemoveRole(role)

        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso Not role.Father Is Nothing AndAlso
            Not Me.CurrentProtocol.Roles.Where(Function(x) x.DistributionType.Eq(ProtocolDistributionType.Explicit)).Any(Function(x) x.Role.FullIncrementalPath.Contains(role.Father.Id.ToString())) Then
            RemoveRoleAuthorization(role.Father)
        End If
    End Sub

    Private Sub AddRoleList()
        ' Recupero i diritti sul protocollo
        Dim rights As New ProtocolRights(CurrentProtocol)
        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            If Not rights.IsDistributable Then
                Throw New DocSuiteException("Aggiungi Autorizzazioni", "Operatore senza diritti di Distribuzione")
            End If
        Else
            If Not rights.IsAuthorizable Then
                Throw New DocSuiteException("Aggiungi Autorizzazioni", "Operatore senza diritti di Autorizzazione")
            End If
        End If

        For Each idRole As Integer In Me.CurrentAddedRoleList
            AddRoleAuthorization(idRole, "E")
        Next
    End Sub

    Private Sub RemoveRoleList()
        For Each role As Role In From idRole As Integer In Me.CurrentRemovedRoleList Select Facade.RoleFacade.GetById(idRole)
            If CurrentProtocol.RoleUsers IsNot Nothing Then
                Dim temp As New List(Of ProtocolRoleUser)(CurrentProtocol.RoleUsers)
                For Each item As ProtocolRoleUser In temp
                    If item.Role.FullIncrementalPath.StartsWith(role.FullIncrementalPath) Then
                        CurrentProtocol.RoleUsers.Remove(item)
                    End If
                Next
            End If
            RemoveRoleAuthorization(role)
        Next
    End Sub

    Private Sub AddCorrectionLog(container As Container, mittenti As IList(Of ContactDTO), destinatari As IList(Of ContactDTO))
        If Not CurrentProtocol.Container.Id.Equals(container.Id) Then
            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PC,
                                            String.Format("Protocollo corretto da {0} e spostato da [{1} in {2}]", DocSuiteContext.Current.User.FullUserName, CurrentProtocol.Container.Name, container.Name))
        End If
        If Not CurrentProtocol.ProtocolObject.Eq(uscObject.Text) Then
            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PC, String.Format("Protocollo corretto da {0} e modificato Campo Oggetto", DocSuiteContext.Current.User.FullUserName))
        End If

        Dim currentMittenti As IList(Of ContactDTO) = CurrentProtocol.GetSenders()

        If (currentMittenti.Count <> mittenti.Count) OrElse
            Not currentMittenti.All(Function(f) mittenti.Any(Function(x) x.Contact.Id = f.Contact.Id)) OrElse
            Not mittenti.All(Function(f) currentMittenti.Any(Function(x) x.Contact.Id = f.Contact.Id)) Then

            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PC, String.Format("Protocollo corretto da {0} e modificati i Mittenti", DocSuiteContext.Current.User.FullUserName))
        End If

        Dim currentDestinatari As IList(Of ContactDTO) = CurrentProtocol.GetRecipients()
        If (currentDestinatari.Count <> destinatari.Count) OrElse
           Not currentDestinatari.All(Function(f) destinatari.Any(Function(x) x.Contact.Id = f.Contact.Id)) OrElse
           Not destinatari.All(Function(f) currentDestinatari.Any(Function(x) x.Contact.Id = f.Contact.Id)) Then

            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PC, String.Format("Protocollo corretto da {0} e modificati i Destinatari", DocSuiteContext.Current.User.FullUserName))
        End If
    End Sub

#End Region

End Class