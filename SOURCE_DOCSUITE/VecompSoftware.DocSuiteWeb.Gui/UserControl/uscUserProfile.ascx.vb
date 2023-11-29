Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Tenants
Imports VecompSoftware.DocSuiteWeb.Model.Documents.Signs
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Analytics.Models.AdaptiveSearches
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Logging

Public Class uscUserProfile
    Inherits DocSuite2008BaseControl


#Region "Fields"
    Private Const PAGE_TITLE_FORMAT As String = "Configurazione profilo dell'utente {0}"
    Private Const USER_CONFIGURATION_TAB_NAME As String = "UserConfigurationTab"
    Private Const ADAPTIVE_PROTOCOL_SEARCH_TAB_NAME As String = "AdaptiveProtSearchConfigurationTab"

    Dim _currentADEmail As String
    Private _currentUserLog As UserLog
    Private _adaptiveSearchControls As ICollection(Of AdaptiveSearchMappingControl)
    Private _currentContact As Contact
#End Region

#Region "Properties"
    Private ReadOnly Property CurrentADEmail As String
        Get
            If String.IsNullOrEmpty(_currentADEmail) Then
                Dim domain As String = DocSuiteContext.Current.CurrentDomainName
                Dim username As String = Account
                If Account.Contains("\"c) Then
                    domain = Account.Split("\"c)(0)
                    username = Account.Split("\"c)(1)
                End If
                _currentADEmail = Facade.UserLogFacade.EmailOfUser(username, domain, False)
            End If
            Return _currentADEmail
        End Get
    End Property

    Private ReadOnly Property CurrentUserLog As UserLog
        Get
            If _currentUserLog Is Nothing Then
                _currentUserLog = Facade.UserLogFacade.GetByUser(Account)
            End If
            Return _currentUserLog
        End Get
    End Property

    Private ReadOnly Property DefaultProtocolSearchControls() As IDictionary(Of String, String)
        Get
            If ViewState("DefaultProtocolSearchControls") Is Nothing Then
                Dim defaultControls As UserSearchModel = Facade.UserLogFacade.GetProtocolUserSearchControls(CurrentUserLog)
                ViewState("DefaultProtocolSearchControls") = defaultControls.SearchControls
            End If
            Return DirectCast(ViewState("DefaultProtocolSearchControls"), IDictionary(Of String, String))
        End Get
    End Property

    Private ReadOnly Property CurrentActiveTab As String
        Get
            Dim userTab As RadTab = radTabStrip.FindTabByValue(USER_CONFIGURATION_TAB_NAME)
            Return If(userTab.Selected, USER_CONFIGURATION_TAB_NAME, ADAPTIVE_PROTOCOL_SEARCH_TAB_NAME)
        End Get
    End Property

    Public Shadows ReadOnly Property MasterDocSuite() As DocSuite2008
        Get
            Return CType(Me.Page.Master, DocSuite2008)
        End Get
    End Property

    Public Property Account As String

    Public Property MakeReload As Boolean

    Public Property SignSaveModalityDisabled As Boolean

    Private ReadOnly Property CurrentContact As Contact
        Get
            If _currentContact Is Nothing Then
                _currentContact = Facade.ContactFacade.GetContactByIncrementalFatherAndSearchCode(ProtocolEnv.FascicleContactId, Account, True)
            End If
            Return _currentContact
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
            LoadUserTenants()
        End If
        If ProtocolEnv.SMSPecNotificationEnabled AndAlso ProtocolEnv.SMSUserProfileReadOnly AndAlso Not String.IsNullOrEmpty(txtMobilePhone.Text) Then
            txtMobilePhone.Enabled = False
        End If

        If SignSaveModalityDisabled OrElse ProtocolEnv.RemoteSignDelegateEnabled Then
            rlbSignSaveModality.SelectedValue = "2"
            rlbSignSaveModality.Enabled = False
        End If
    End Sub

    Protected Sub rlbSelectCompany_ItemSelected(sender As Object, e As DropDownListEventArgs) Handles rlbSelectCompany.ItemSelected
        CurrentUserLog.CurrentTenantId = New Guid(e.Value)
        Try
            Facade.UserLogFacade.Update(CurrentUserLog)
            Dim tenantFacade As TenantFacade = New TenantFacade()
            CurrentTenant = tenantFacade.GetCurrentTenant()
            ScriptManager.RegisterClientScriptBlock(Page, GetType(Page), "changeCompany", String.Format("changeCompanyDisplay('{0}');", MakeReload), True)
        Catch ex As Exception
            FileLogger.Error(LoggerName, "ConsoleUser: Errore nell'inserimento dati in UserLog", ex)
            AjaxAlert("Errore nel salvataggio dati utente")
        End Try
    End Sub

    Private Sub BtnSalva_Click(sender As Object, e As EventArgs) Handles btnSalva.Click
        If CurrentActiveTab.Eq(USER_CONFIGURATION_TAB_NAME) Then
            CurrentUserLog.UserMail = txtEmail.Text
            If ProtocolEnv.SMSPecNotificationEnabled Then
                If Not txtMobilePhone.Text.All(Function(s) Char.IsDigit(s)) OrElse (txtMobilePhone.Text.Length < 6 OrElse txtMobilePhone.Text.Length > 15) Then
                    AjaxAlert("Inserire un numero di cellulare valido")
                    Exit Sub
                End If
                CurrentUserLog.MobilePhone = txtMobilePhone.Text
            End If
        Else
            If ProtocolEnv.ProtocolSearchAdaptiveEnabled Then
                Dim model As IDictionary(Of String, ICollection(Of String)) = Facade.UserLogFacade.GetDefaultAdaptiveSearchControls(CurrentUserLog)
                If model Is Nothing Then
                    model = New Dictionary(Of String, ICollection(Of String))
                End If

                Dim selectedControls As ICollection(Of String) = treeControls.CheckedNodes.Select(Function(d) d.Value).ToList()
                model.AddSafe(UserLogFacade.PROTOCOL_ADAPTIVE_SEARCH_KEY, selectedControls)
                If Not String.IsNullOrEmpty(CurrentUserLog.AdaptiveSearchEvaluated) Then
                    Dim evaluatedModel As IDictionary(Of String, UserSearchModel) = JsonConvert.DeserializeObject(Of IDictionary(Of String, UserSearchModel))(CurrentUserLog.AdaptiveSearchEvaluated)
                    If evaluatedModel.ContainsKey(UserLogFacade.PROTOCOL_ADAPTIVE_SEARCH_KEY) Then
                        evaluatedModel(UserLogFacade.PROTOCOL_ADAPTIVE_SEARCH_KEY).SearchControls = evaluatedModel(UserLogFacade.PROTOCOL_ADAPTIVE_SEARCH_KEY).SearchControls _
                                                                                            .Where(Function(x) selectedControls.Contains(x.Key)) _
                                                                                            .Select(Function(s) New With {.Key = s.Key, .Value = s.Value}) _
                                                                                            .ToDictionary(Function(d) d.Key, Function(d) d.Value)

                        evaluatedModel(UserLogFacade.PROTOCOL_ADAPTIVE_SEARCH_KEY).CreatedDate = Date.UtcNow.Date
                        CurrentUserLog.AdaptiveSearchEvaluated = JsonConvert.SerializeObject(evaluatedModel)
                    End If
                End If
                CurrentUserLog.DefaultAdaptiveSearchControls = JsonConvert.SerializeObject(model)
            End If
        End If

        If ProtocolEnv.RemoteSignEnabled Then
            HandleSignOptionSave()
        End If

        If ProtocolEnv.FascicleEnabled Then
            Dim contact As Contact = CurrentContact
            If CurrentContact IsNot Nothing Then
                contact.FiscalCode = txtFiscalCode.Text
                Facade.ContactFacade.Update(contact)
            End If
        End If

        Try
            Facade.UserLogFacade.Update(CurrentUserLog)
            AjaxAlert("Dati salvati correttamente")
        Catch ex As Exception
            FileLogger.Error(LoggerName, "ConsoleUser: Errore nell'inserimento dati in UserLog", ex)
            AjaxAlert("Errore nel salvataggio dati utente")
        End Try
    End Sub

    Private Sub cmdRetrieveADEmail_Click(sender As Object, e As EventArgs) Handles btnRetrieveADEmail.Click
        Try
            txtEmail.Text = CurrentADEmail
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in fase di recupero email da AD per l'utente corrente.", ex)
            AjaxAlert("Errore in fase di recupero email da AD per l'utente corrente: " & ex.Message)
        End Try
    End Sub

    Protected Sub treeControls_NodeCheck(sender As Object, e As RadTreeNodeEventArgs) Handles treeControls.NodeCheck
        uscProtRicerca.ChangeControlEnable(e.Node.Value, e.Node.Checked)
    End Sub

    Protected Sub rlbSignType_SelectedIndexChanged(sender As Object, e As DropDownListEventArgs) Handles rlbSignType.SelectedIndexChanged
        ChangeSelectedSignType(False)
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()
        If CurrentUserLog Is Nothing Then
            FileLogger.Warn(LoggerName, String.Format("Utente {0} non configurato in UserLog. Creo l'utente in fase di inizializzazione.", Account))
            Dim userLog As UserLog = New UserLog With {
                .Id = Account,
                .CurrentTenantId = Guid.Empty
            }
            Facade.UserLogFacade.Save(userLog)
            _currentUserLog = Nothing
        End If

        Me.Page.Title = String.Format(PAGE_TITLE_FORMAT, Account)
        pnlMobilePhone.Visible = ProtocolEnv.SMSPecNotificationEnabled
        ' Carico i dati dell'utente corrente
        lblUser.Text = Account
        ' Email da AD
        Dim mail As String = GetCurrentUserEmail()
        Dim mailExist As Boolean = Not String.IsNullOrEmpty(mail)

        txtEmail.Text = mail
        txtEmail.ReadOnly = mailExist AndAlso Not ProtocolEnv.EnableUserProfile

        btnRetrieveADEmail.Visible = ProtocolEnv.EnableUserProfile
        btnSalva.Enabled = (Not mailExist OrElse ProtocolEnv.EnableUserProfile) OrElse ProtocolEnv.SMSPecNotificationEnabled OrElse ProtocolEnv.ProtocolSearchAdaptiveEnabled OrElse ProtocolEnv.RemoteSignEnabled

        If ProtocolEnv.SMSPecNotificationEnabled Then
            Dim domain As String = DocSuiteContext.Current.CurrentDomainName
            Dim username As String = Account
            If Account.Contains("\"c) Then
                domain = Account.Split("\"c)(0)
                username = Account.Split("\"c)(1)
            End If
            txtMobilePhone.Text = Facade.UserLogFacade.MobilePhoneOfUser(username, domain)
        End If

        radTabStrip.FindTabByValue("AdaptiveProtSearchConfigurationTab").Visible = False
        If ProtocolEnv.ProtocolSearchAdaptiveEnabled Then
            radTabStrip.FindTabByValue("AdaptiveProtSearchConfigurationTab").Visible = True
            treeControls.DataSource = GetAdaptiveSearchControls()
            treeControls.DataBind()
            SetDefaultAdaptiveControls()
        End If

        trSignInfo.Visible = False
        If ProtocolEnv.RemoteSignEnabled Then
            trSignInfo.Visible = True
            ChangeSelectedSignType(True)
            If Not DocSuiteContext.Current.HasInfocertProxySign Then
                rlbSignType.FindItemByValue("2").Remove()
                rlbSignType.FindItemByValue("4").Remove()
                rlbSignType.FindItemByValue("5").Remove()
            End If
            If Not DocSuiteContext.Current.HasArubaActalisSign Then
                rlbSignType.FindItemByValue("1").Remove()
                rlbSignType.FindItemByValue("3").Remove()
            End If
            If Not DocSuiteContext.Current.HasInfocertProxySignLocal Then
                If (Not rlbSignType.FindItemByValue("5") Is Nothing) Then
                    rlbSignType.FindItemByValue("5").Remove()
                End If
            End If
        End If

            If Not ProtocolEnv.EnableUserProfile Then
            If mailExist Then
                BindWarningPanel("Utente con mail presente in AD - modifica e-mail non possibile", "warningAreaLow")
            Else
                BindWarningPanel("Utente con mail non presente AD - modifica e-mail possibile", "successAreaLow")
            End If
        End If

        If ProtocolEnv.FascicleEnabled Then
            trFiscalCode.Visible = True
            If CurrentContact IsNot Nothing Then
                txtFiscalCode.Text = CurrentContact.FiscalCode
            End If
        End If
    End Sub

    Private Sub HandleSignOptionSave()
        Dim signOption As RemoteSignProperty = New RemoteSignProperty()

        If CurrentUserLog.UserProfile IsNot Nothing AndAlso cbSignDefault.Checked Then
            Dim currentUserProfile As Model.Documents.Signs.UserProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(CurrentUserLog.UserProfile)
            For Each userProfileRemoteSignProperty As RemoteSignProperty In currentUserProfile.Value.Values
                userProfileRemoteSignProperty.IsDefault = False
            Next
            CurrentUserLog.UserProfile = JsonConvert.SerializeObject(currentUserProfile)
        End If

        signOption.IsDefault = cbSignDefault.Checked
        RemoveSignDelegations()
        Select Case rlbSignType.SelectedValue
            Case "1", "2"
                signOption.Alias = txtSignAlias.Text
                signOption.StorageInformationType = CType(Convert.ToInt32(rlbSignSaveModality.SelectedValue), StorageInformationType)
                signOption = SelectStorageType(signOption, "signOptionPin", txtSignPin)
                If rlbSignType.SelectedValue = "1" Then
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_CERTIFICATEID, txtArubaCertificateId.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_DELEGATED_DOMAIN, txtArubaDelegatedDomain.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_DELEGATED_PASSWORD, txtSignPassword.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_DELEGATED_USER, txtArubaDelegatedUser.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_OTP_AUTHTYPE, txtArubaOTPAuthType.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_USER, txtSignAlias.Text)
                    signOption.OTP = txtArubaOTPPassword.Text
                End If
                If rlbSignType.SelectedValue = "2" Then
                    signOption.OTPType = CType(Convert.ToInt32(rlbOTPType.SelectedValue), OTPType)
                End If
            Case "3", "4"
                signOption.StorageInformationType = CType(Convert.ToInt32(rlbSignSaveModality.SelectedValue), StorageInformationType)
                signOption = SelectStorageType(signOption, "signOptionPassword", txtSignPassword)
                signOption.Alias = txtSignAlias.Text
                If rlbSignType.SelectedValue = "3" Then
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_CERTIFICATEID, txtArubaCertificateId.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_DELEGATED_DOMAIN, txtArubaDelegatedDomain.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_DELEGATED_PASSWORD, txtSignPassword.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_DELEGATED_USER, txtArubaDelegatedUser.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_OTP_AUTHTYPE, txtArubaOTPAuthType.Text)
                    signOption.CustomProperties.Add(RemoteSignProperty.ARUBA_USER, txtArubaUser.Text)
                    signOption.OTP = txtArubaOTPPassword.Text
                    signOption.Alias = txtArubaUser.Text
                End If
                signOption.Delegations = RemoteDelegations()
                signOption.BeenDelegated = RemoteBeenDelegated()
        End Select

        Dim userProfile As Model.Documents.Signs.UserProfile
        Dim provider As ProviderSignType = CType(Convert.ToInt32(rlbSignType.SelectedValue), ProviderSignType)
        If String.IsNullOrEmpty(CurrentUserLog.UserProfile) Then
            userProfile = New Model.Documents.Signs.UserProfile
            userProfile.Value = New Dictionary(Of ProviderSignType, RemoteSignProperty)()
            userProfile.DefaultProvider = provider
            userProfile.Value.Add(provider, signOption)
        Else
            userProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(CurrentUserLog.UserProfile)
            userProfile.DefaultProvider = provider
            Dim existingSignOption As KeyValuePair(Of ProviderSignType, RemoteSignProperty)? = Nothing
            If userProfile.Value.Any(Function(x) x.Key = provider) Then
                existingSignOption = userProfile.Value.First(Function(x) x.Key = provider)
            End If
            If existingSignOption.HasValue Then
                userProfile.Value(existingSignOption.Value.Key) = signOption
            Else
                userProfile.Value.Add(provider, signOption)
            End If
        End If

        CurrentUserLog.UserProfile = JsonConvert.SerializeObject(userProfile)
    End Sub

    Private Sub RemoveSignDelegations()
        Dim newListRemoteDelegate As IList(Of ContactDTO) = uscRemoteDelegations.GetContacts(True)
        Dim listRemoteDelegateToRemove As IDictionary(Of String, DelegateUser) = Nothing
        Dim userProfile As Model.Documents.Signs.UserProfile
        Dim provider As ProviderSignType = CType(Convert.ToInt32(rlbSignType.SelectedValue), ProviderSignType)
        If Not String.IsNullOrEmpty(CurrentUserLog.UserProfile) Then
            userProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(CurrentUserLog.UserProfile)
            userProfile.DefaultProvider = provider
            Dim existingSignOption As KeyValuePair(Of ProviderSignType, RemoteSignProperty)? = Nothing
            If userProfile.Value.Any(Function(x) x.Key = provider) Then
                existingSignOption = userProfile.Value.First(Function(x) x.Key = provider)
            End If
            If existingSignOption.HasValue Then
                If Not existingSignOption.Value.Value.Delegations Is Nothing Then
                    listRemoteDelegateToRemove = existingSignOption.Value.Value.Delegations
                    For Each contactDTO As ContactDTO In newListRemoteDelegate
                        listRemoteDelegateToRemove.Remove(contactDTO.Contact.Code)
                    Next
                End If
            End If
            If Not listRemoteDelegateToRemove Is Nothing Then
                For Each contactDelegate As KeyValuePair(Of String, DelegateUser) In listRemoteDelegateToRemove
                    Dim delegateUser As UserLog = Facade.UserLogFacade.GetByUser(contactDelegate.Key)
                    Dim delegateUserProfile As Model.Documents.Signs.UserProfile
                    Dim delegateUserprovider As ProviderSignType = CType(Convert.ToInt32(rlbSignType.SelectedValue), ProviderSignType)
                    If delegateUser IsNot Nothing AndAlso Not String.IsNullOrEmpty(delegateUser.UserProfile) Then
                        delegateUserProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(delegateUser.UserProfile)
                        If delegateUserProfile.Value.Any(Function(x) x.Key = delegateUserprovider) Then
                            existingSignOption = delegateUserProfile.Value.First(Function(x) x.Key = delegateUserprovider)
                            If existingSignOption.HasValue Then
                                If Not existingSignOption.Value.Value.BeenDelegated Is Nothing Then
                                    Dim userListRemoteDelegate As IDictionary(Of String, DelegateUser) = existingSignOption.Value.Value.BeenDelegated
                                    userListRemoteDelegate.Remove(DocSuiteContext.Current.User.FullUserName)
                                    existingSignOption.Value.Value.BeenDelegated = userListRemoteDelegate
                                    delegateUserProfile.Value(existingSignOption.Value.Key) = existingSignOption.Value.Value
                                End If
                            End If
                        End If
                        delegateUser.UserProfile = JsonConvert.SerializeObject(delegateUserProfile)
                        Facade.UserLogFacade.Update(delegateUser)
                    End If
                Next
            End If
        End If
    End Sub

    Private Function RemoteBeenDelegated() As IDictionary(Of String, DelegateUser)
        Dim remoteSingFromDelegate As IDictionary(Of String, DelegateUser) = Nothing
        If CurrentUserLog Is Nothing OrElse String.IsNullOrWhiteSpace(CurrentUserLog.UserProfile) Then
            Return remoteSingFromDelegate
        End If
        Dim userProfile As Model.Documents.Signs.UserProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(CurrentUserLog.UserProfile)
        Dim provider As ProviderSignType = CType(Convert.ToInt32(rlbSignType.SelectedValue), ProviderSignType)
        Dim existingSignOption As KeyValuePair(Of ProviderSignType, RemoteSignProperty)? = Nothing
        If userProfile.Value.Any(Function(x) x.Key = provider) Then
            existingSignOption = userProfile.Value.First(Function(x) x.Key = provider)
            If existingSignOption.HasValue Then
                If Not existingSignOption.Value.Value.BeenDelegated Is Nothing Then
                    remoteSingFromDelegate = existingSignOption.Value.Value.BeenDelegated
                End If
            End If
        End If
        Return remoteSingFromDelegate
    End Function

    Private Function RemoteDelegations() As IDictionary(Of String, DelegateUser)
        Dim dicDelegateUser As New Dictionary(Of String, DelegateUser)
        Dim whoDelegate As New DelegateUser
        whoDelegate.Account = DocSuiteContext.Current.User.FullUserName
        whoDelegate.DisplayName = DocSuiteContext.Current.User.UserName
        For Each contact As ContactDTO In uscRemoteDelegations.GetContacts(True)
            Dim contactDelegateModel As New DelegateUser
            contactDelegateModel.Account = contact.Contact.Code
            contactDelegateModel.DisplayName = contact.Contact.Description
            contactDelegateModel.DelegatedByUser = DocSuiteContext.Current.User.FullUserName
            dicDelegateUser.Add(contact.Contact.Code, contactDelegateModel)

            Dim delegateUser As UserLog = Facade.UserLogFacade.GetByUser(contact.Contact.Code)

            If delegateUser Is Nothing Then
                Continue For
            End If

            Dim delegateUserProfile As Model.Documents.Signs.UserProfile
            Dim delegateUserprovider As ProviderSignType = CType(Convert.ToInt32(rlbSignType.SelectedValue), ProviderSignType)
            Dim existingSignOption As KeyValuePair(Of ProviderSignType, RemoteSignProperty)? = Nothing
            Dim listDelegateFromUserModel As IDictionary(Of String, DelegateUser)

            If String.IsNullOrEmpty(delegateUser.UserProfile) Then
                delegateUserProfile = New Model.Documents.Signs.UserProfile
                delegateUserProfile.Value = New Dictionary(Of ProviderSignType, RemoteSignProperty)()
                delegateUserProfile.DefaultProvider = delegateUserprovider
                AddDelegateCustomProperty(whoDelegate, delegateUserProfile, delegateUserprovider)
            Else
                delegateUserProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(delegateUser.UserProfile)
                If delegateUserProfile.Value.Any(Function(x) x.Key = delegateUserprovider) Then
                    existingSignOption = delegateUserProfile.Value.First(Function(x) x.Key = delegateUserprovider)
                End If
                If existingSignOption.HasValue Then
                    If existingSignOption.Value.Value.BeenDelegated Is Nothing Then
                        listDelegateFromUserModel = New Dictionary(Of String, DelegateUser)
                        listDelegateFromUserModel.Add(whoDelegate.Account, whoDelegate)
                        existingSignOption.Value.Value.BeenDelegated = listDelegateFromUserModel
                    Else
                        listDelegateFromUserModel = existingSignOption.Value.Value.BeenDelegated
                        If Not listDelegateFromUserModel.ContainsKey(whoDelegate.Account) Then
                            listDelegateFromUserModel.Add(whoDelegate.Account, whoDelegate)
                        End If
                    End If
                    existingSignOption.Value.Value.BeenDelegated = listDelegateFromUserModel
                    delegateUserProfile.Value(existingSignOption.Value.Key) = existingSignOption.Value.Value
                Else
                    AddDelegateCustomProperty(whoDelegate, delegateUserProfile, delegateUserprovider)
                End If
            End If
            delegateUser.UserProfile = JsonConvert.SerializeObject(delegateUserProfile)
            Facade.UserLogFacade.Update(delegateUser)
        Next
        Return dicDelegateUser
    End Function

    Private Sub AddDelegateCustomProperty(whoDelegate As DelegateUser, ByRef delegateUserProfile As Model.Documents.Signs.UserProfile, delegateUserprovider As ProviderSignType)
        Dim dicDelegateUser As New Dictionary(Of String, DelegateUser)
        dicDelegateUser.Add(whoDelegate.Account, whoDelegate)
        Dim signOptionDelegate As RemoteSignProperty = New RemoteSignProperty()
        signOptionDelegate.OTPType = CType(Convert.ToInt32(rlbOTPType.SelectedValue), OTPType)
        signOptionDelegate.BeenDelegated = dicDelegateUser
        delegateUserProfile.Value.Add(delegateUserprovider, signOptionDelegate)
    End Sub

    Private Sub SetValuesForDefaultSignType(defaultSignType As KeyValuePair(Of ProviderSignType, RemoteSignProperty)?)
        If Not defaultSignType.HasValue OrElse (defaultSignType.HasValue AndAlso defaultSignType.Value.Value Is Nothing) Then
            uscRemoteDelegations.DataSource = Nothing
            uscRemoteBeenDelegated.DataSource = Nothing
            uscRemoteDelegations.DataBind()
            uscRemoteBeenDelegated.DataBind()

            txtSignAlias.Text = String.Empty
            txtSignPin.Text = String.Empty
            txtSignPassword.Text = String.Empty
            cbSignDefault.Checked = False
            If rlbSignType.SelectedValue = ProviderSignType.ArubaAutomatic OrElse rlbSignType.SelectedValue = ProviderSignType.InfocertAutomatic Then
                trRemoteSignDelegate.Visible = ProtocolEnv.RemoteSignDelegateEnabled
            End If
            Return
        End If

        Dim signProperty As KeyValuePair(Of ProviderSignType, RemoteSignProperty) = defaultSignType.Value
        cbSignDefault.Checked = signProperty.Value.IsDefault

        txtSignAlias.Text = String.Empty
        If signProperty.Key = ProviderSignType.ArubaRemote OrElse signProperty.Key = ProviderSignType.InfocertRemote Then
            txtSignAlias.Text = signProperty.Value.Alias
            rlbSignSaveModality.SelectedValue = signProperty.Value.StorageInformationType.ToString("D")
            If signProperty.Key = ProviderSignType.InfocertRemote Then
                rlbOTPType.SelectedValue = signProperty.Value.OTPType.ToString("D")
            End If
            GetByStorageType(defaultSignType, "signOptionPin", txtSignPin)
        End If

        txtArubaOTPPassword.Text = String.Empty
        If signProperty.Key = ProviderSignType.ArubaAutomatic OrElse signProperty.Key = ProviderSignType.InfocertAutomatic Then
            If signProperty.Key = ProviderSignType.InfocertAutomatic Then
                txtSignAlias.Text = signProperty.Value.Alias
            End If
            rlbSignSaveModality.SelectedValue = signProperty.Value.StorageInformationType.ToString("D")
            GetByStorageType(defaultSignType, "signOptionPassword", txtSignPassword)
            txtArubaOTPPassword.Text = signProperty.Value.OTP
            trRemoteSignDelegate.Visible = ProtocolEnv.RemoteSignDelegateEnabled
        End If

        If signProperty.Key = ProviderSignType.ArubaAutomatic OrElse signProperty.Key = ProviderSignType.ArubaRemote Then
            txtArubaCertificateId.Visible = True
            txtArubaDelegatedDomain.Visible = signProperty.Key = ProviderSignType.ArubaAutomatic
            txtArubaDelegatedUser.Visible = signProperty.Key = ProviderSignType.ArubaAutomatic
            txtArubaOTPAuthType.Visible = True
            txtArubaUser.Visible = signProperty.Key = ProviderSignType.ArubaAutomatic

            txtArubaCertificateId.Text = String.Empty
            If signProperty.Value.CustomProperties.ContainsKey(RemoteSignProperty.ARUBA_CERTIFICATEID) Then
                txtArubaCertificateId.Text = signProperty.Value.CustomProperties(RemoteSignProperty.ARUBA_CERTIFICATEID)
            End If
            txtArubaDelegatedDomain.Text = String.Empty
            If signProperty.Value.CustomProperties.ContainsKey(RemoteSignProperty.ARUBA_DELEGATED_DOMAIN) Then
                txtArubaDelegatedDomain.Text = signProperty.Value.CustomProperties(RemoteSignProperty.ARUBA_DELEGATED_DOMAIN)
            End If
            txtArubaDelegatedUser.Text = String.Empty
            If signProperty.Value.CustomProperties.ContainsKey(RemoteSignProperty.ARUBA_DELEGATED_USER) Then
                txtArubaDelegatedUser.Text = signProperty.Value.CustomProperties(RemoteSignProperty.ARUBA_DELEGATED_USER)
            End If
            txtArubaOTPAuthType.Text = String.Empty
            If signProperty.Value.CustomProperties.ContainsKey(RemoteSignProperty.ARUBA_OTP_AUTHTYPE) Then
                txtArubaOTPAuthType.Text = signProperty.Value.CustomProperties(RemoteSignProperty.ARUBA_OTP_AUTHTYPE)
            End If
            txtArubaUser.Text = String.Empty
            If signProperty.Value.CustomProperties.ContainsKey(RemoteSignProperty.ARUBA_USER) Then
                txtArubaUser.Text = signProperty.Value.CustomProperties(RemoteSignProperty.ARUBA_USER)
            End If
        End If

        uscRemoteDelegations.DataSource = Nothing
        uscRemoteBeenDelegated.DataSource = Nothing

        If Not signProperty.Value.Delegations Is Nothing AndAlso signProperty.Value.Delegations.Any Then
            uscRemoteDelegations.DataSource = ListUserDelegate(signProperty.Value.Delegations)
        End If

        If Not signProperty.Value.BeenDelegated Is Nothing AndAlso signProperty.Value.BeenDelegated.Any Then
            uscRemoteBeenDelegated.DataSource = ListUserDelegate(signProperty.Value.BeenDelegated)
        End If

        uscRemoteBeenDelegated.DataBind()
        uscRemoteDelegations.DataBind()
    End Sub

    Private Function ListUserDelegate(listContactDelegate As IDictionary(Of String, DelegateUser)) As List(Of ContactDTO)
        Dim listUsersDelegate As New List(Of ContactDTO)
        For Each contactDelegate As KeyValuePair(Of String, DelegateUser) In listContactDelegate
            Dim contactDTO As New ContactDTO
            Dim contact As New Contact()
            With contact
                .Description = contactDelegate.Value.DisplayName
                .Code = contactDelegate.Value.Account
                .ContactType = New ContactType(ContactType.Aoo)
            End With
            contactDTO.Contact = contact
            contactDTO.Type = contactDTO.ContactType.Manual
            listUsersDelegate.Add(contactDTO)
        Next
        Return listUsersDelegate
    End Function

    Private Sub ChangeSelectedSignType(isInit As Boolean)
        trRemoteSignDelegate.Visible = False
        Dim defaultSignOption As KeyValuePair(Of ProviderSignType, RemoteSignProperty)? = Nothing
        Dim provider As ProviderSignType
        If Not String.IsNullOrEmpty(CurrentUserLog.UserProfile) Then
            provider = CType(Convert.ToInt32(rlbSignType.SelectedValue), ProviderSignType)
            Dim userProfile As Model.Documents.Signs.UserProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(CurrentUserLog.UserProfile)
            If isInit Then
                provider = userProfile.DefaultProvider
            End If
            If userProfile.Value.Any(Function(x) x.Key = provider) Then
                defaultSignOption = userProfile.Value.FirstOrDefault(Function(x) x.Key = provider)
            End If
            If isInit AndAlso defaultSignOption.HasValue Then
                rlbSignType.SelectedValue = defaultSignOption.Value.Key.ToString("D")
            End If
            SetValuesForDefaultSignType(defaultSignOption)
        End If
    End Sub

    Private Function GetCurrentUserEmail() As String
        Dim mail As String = String.Empty
        If ProtocolEnv.EnableUserProfile Then
            If CurrentUserLog Is Nothing Then
                Return CurrentADEmail
            End If

            mail = CurrentUserLog.UserMail
            If String.IsNullOrEmpty(mail) Then
                ' Recupero da Dominio
                mail = CurrentADEmail
            End If
        Else
            mail = CurrentADEmail
            If String.IsNullOrEmpty(mail) Then
                Dim domain As String = DocSuiteContext.Current.CurrentDomainName
                Dim username As String = Account
                If Account.Contains("\"c) Then
                    domain = Account.Split("\"c)(0)
                    username = Account.Split("\"c)(1)
                End If
                ' Recupero da UserLog
                mail = Facade.UserLogFacade.EmailOfUser(username, domain, True)
            End If
        End If
        Return mail
    End Function

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSalva, pnlConsoleUserContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSalva, btnSalva, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(radTabStrip, radMultiPage, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(treeControls, uscProtRicerca)
        AjaxManager.AjaxSettings.AddAjaxSetting(rlbSignType, rlbSignSaveModality)
    End Sub

    Private Sub BindWarningPanel(ByVal errorMessage As String, cssClass As String)
        WarningLabel.Text = errorMessage
        WarningPanel.CssClass = cssClass
    End Sub

    Public Function GetAdaptiveSearchControls() As ICollection(Of AdaptiveSearchMappingControl)
        Dim controls As ICollection(Of AdaptiveSearchMappingControl) = DocSuiteContext.Current.ProtocolDefaultAdaptiveSearchConfigurations
        controls = controls.Where(Function(x) Not x.Id.Eq("chbContactChild") AndAlso Not x.Id.Eq("chbCategoryChild")) _
            .Where(Function(x) (Not ProtocolEnv.RolesUserProfileEnabled AndAlso Not x.Id.Eq("uscSettore")) OrElse ProtocolEnv.RolesUserProfileEnabled) _
            .Where(Function(x) (Not ProtocolEnv.ProtParzialeEnabled AndAlso Not x.Id.Eq("cbIncomplete")) OrElse ProtocolEnv.ProtParzialeEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsClaimEnabled AndAlso Not x.Id.Eq("rblClaim")) OrElse ProtocolEnv.IsClaimEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsInteropEnabled AndAlso Not x.Id.Eq("UscContattiSel1")) OrElse ProtocolEnv.IsInteropEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsTableDocTypeEnabled AndAlso Not x.Id.Eq("idDocType")) OrElse ProtocolEnv.IsTableDocTypeEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsStatusEnabled AndAlso Not x.Id.Eq("ProtocolStatus")) OrElse ProtocolEnv.IsStatusEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsPackageEnabled AndAlso Not x.Id.Eq("Origin") AndAlso Not x.Id.Eq("Lot") AndAlso Not x.Id.Eq("Package") AndAlso Not x.Id.Eq("Incremental")) OrElse ProtocolEnv.IsPackageEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsInvoiceEnabled AndAlso Not x.Id.Eq("InvoiceNumber") AndAlso Not x.Id.Eq("InvoiceDateFrom") AndAlso Not x.Id.Eq("InvoiceDateTo") _
                                        AndAlso Not x.Id.Eq("AccountingSectional") AndAlso Not x.Id.Eq("AccountingYear") AndAlso Not x.Id.Eq("AccountingNumber")) OrElse ProtocolEnv.IsInvoiceEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsProtSearchTitleEnabled AndAlso Not x.Id.Eq("cmbTitoliStudio") AndAlso Not x.Id.Eq("txtBirthDateFrom") AndAlso Not x.Id.Eq("txtBirthDateTo") _
                                        AndAlso Not x.Id.Eq("txtCity")) OrElse ProtocolEnv.IsProtSearchTitleEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsLogStatusEnabled AndAlso Not x.Id.Eq("chbNoRead")) OrElse ProtocolEnv.IsLogStatusEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsPECEnabled AndAlso Not x.Id.Eq("hasIngoingPec")) OrElse ProtocolEnv.IsPECEnabled) _
            .Where(Function(x) (Not ProtocolEnv.IsDistributionEnabled AndAlso Not x.Id.Eq("IsProtocolDistribution")) OrElse ProtocolEnv.IsDistributionEnabled) _
            .Where(Function(x) (Not ProtocolEnv.ProtocolHighlightEnabled AndAlso Not x.Id.Eq("cbProtocolHighlight")) OrElse ProtocolEnv.ProtocolHighlightEnabled).ToList()

        Return controls
    End Function

    Private Sub SetDefaultAdaptiveControls()
        uscProtRicerca.ChangeControlEnable("UscContattiSel1", False)
        For Each control As KeyValuePair(Of String, String) In DefaultProtocolSearchControls
            Dim node As RadTreeNode = treeControls.FindNodeByValue(control.Key)
            If node IsNot Nothing Then
                node.Checked = True
                uscProtRicerca.ChangeControlEnable(node.Value, True)
            End If
        Next
    End Sub

    Private Sub LoadUserTenants()
        rlbSelectCompany.Visible = True
        Try
            Dim results As ICollection(Of WebAPIDto(Of Tenant)) = WebAPIImpersonatorFacade.ImpersonateFinder(New UserTenantFinder(DocSuiteContext.Current.Tenants),
                    Function(impersonationType, finder)
                        finder.OnlyInternalTenants = True
                        finder.ResetDecoration()
                        finder.EnablePaging = False
                        Return finder.DoSearch()
                    End Function)

            If results IsNot Nothing AndAlso results.Count > 0 Then
                For Each item As Tenant In results.Select(Function(r) r.Entity)
                    rlbSelectCompany.Items.Add(New DropDownListItem(item.CompanyName, item.UniqueId.ToString()))
                Next
            Else
                Dim resultDefault As WebAPIDto(Of Tenant) = WebAPIImpersonatorFacade.ImpersonateFinder(New TenantFinder(DocSuiteContext.Current.Tenants),
                    Function(impersonationType, finder)
                        finder.IncludeContainers = False
                        finder.IncludeTenantAOO = True
                        finder.ResetDecoration()
                        finder.EnablePaging = False
                        Return finder.DoSearch()
                    End Function).FirstOrDefault()
                rlbSelectCompany.Items.Add(New DropDownListItem(resultDefault.Entity.CompanyName, resultDefault.Entity.UniqueId.ToString()))
                rlbSelectCompany.Enabled = False
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, "ConsoleUser: LoadUserTenants ", ex)
        End Try

        If CurrentUserLog.CurrentTenantId = Guid.Empty Then
            Exit Sub
        End If
        Dim tenantId As String = CurrentUserLog.CurrentTenantId.ToString()
        Dim dd As DropDownListItem = rlbSelectCompany.Items.FirstOrDefault(Function(f) f.Value = tenantId)
        If dd IsNot Nothing Then
            dd.Selected = True
        End If
        rlbSelectCompany.Enabled = rlbSelectCompany.Items.Count > 0
    End Sub

    Private Function SelectStorageType(signOptionProperty As RemoteSignProperty, sessionVariable As String, controlTextBox As TextBox) As RemoteSignProperty
        Select Case signOptionProperty.StorageInformationType
            Case StorageInformationType.Forget

            Case StorageInformationType.Session
                Dim signType As String = CType(rlbSignType.SelectedValue, ProviderSignType).ToString()
                AjaxManager.ResponseScripts.Add(String.Format("saveToSession(""{0}"")", controlTextBox.Text))
            Case StorageInformationType.Forever
                If sessionVariable = "signOptionPin" Then
                    signOptionProperty.PIN = controlTextBox.Text
                Else
                    signOptionProperty.Password = controlTextBox.Text
                End If
        End Select
        Return signOptionProperty
    End Function

    Private Sub GetByStorageType(defaultSignType As KeyValuePair(Of ProviderSignType, RemoteSignProperty)?, sessionVariable As String, controlTextBox As TextBox)
        If defaultSignType.Value.Value.StorageInformationType = StorageInformationType.Forever Then
            If sessionVariable = "signOptionPin" Then
                controlTextBox.Text = defaultSignType.Value.Value.PIN
                controlTextBox.Attributes.Add("value", defaultSignType.Value.Value.PIN)
            Else
                controlTextBox.Text = defaultSignType.Value.Value.Password
                controlTextBox.Attributes.Add("value", defaultSignType.Value.Value.Password)
            End If
        ElseIf defaultSignType.Value.Value.StorageInformationType = StorageInformationType.Session Then
            Dim signOption As IDictionary(Of String, String)
            If Session(sessionVariable) IsNot Nothing AndAlso Session(sessionVariable) <> "null" Then
                signOption = JsonConvert.DeserializeObject(Of IDictionary(Of String, String))(CType(Session(sessionVariable), String))
                Dim existingInput As String = signOption(defaultSignType.Value.Key.ToString("D"))
                If Not String.IsNullOrEmpty(existingInput) Then
                    controlTextBox.Text = existingInput
                End If
            End If
        End If
    End Sub

    Public Sub AjaxAlert(ByVal message As String)
        AjaxAlert(message, True)
    End Sub

    Private Sub AjaxAlert(ByVal message As String, ByVal checkJavascript As Boolean)
        If checkJavascript Then
            message = StringHelper.ReplaceAlert(message)
        End If

        If AjaxManager IsNot Nothing Then
            AjaxManager.Alert(message)
        Else
            WebHelper.Alert(Me.Page, message)
        End If
    End Sub
#End Region
End Class