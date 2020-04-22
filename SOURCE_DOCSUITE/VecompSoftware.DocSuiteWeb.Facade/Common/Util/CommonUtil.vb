Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.Hosting
Imports System.Linq
Imports System.Net.Mail
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.DocSuiteWeb.Model.Parameters

''' <summary> Classe di utilità per la gestione pratiche che racchiude alcuni metodi comuni. </summary>
Public Class CommonUtil
    Inherits CommonShared

#Region " Fields "

    Private Shared _instance As CommonUtil
    Private _envGroupSelected As String
    Private _tempDirectory As DirectoryInfo
    Private _appTempHttpAspx As String

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Crea una nuova CommonUtil
    ''' </summary>
    ''' <remarks> Note: Constructor is 'protected' </remarks>
    Public Sub New()
        If HttpContext.Current Is Nothing Then
            Exit Sub
        End If

        InitializePaths(HttpContext.Current.Request)
    End Sub

#End Region

#Region " Properties "

    ''' <summary>  </summary>
    ''' <remarks> Scrivania: finder ante litteram </remarks>
    Public Property ProtDiaryLogDateFrom() As DateTime?

    ''' <summary>  </summary>
    ''' <remarks> Scrivania: finder ante litteram </remarks>
    Public Property ProtDiaryLogDateTo() As DateTime?

    ''' <summary>  </summary>
    ''' <remarks> Scrivania: finder ante litteram </remarks>
    Public Property DocmDiaryLogDateFrom() As DateTime?

    ''' <summary>  </summary>
    ''' <remarks> Scrivania: finder ante litteram </remarks>
    Public Property DocmDiaryLogDateTo() As DateTime?

    ''' <summary>  </summary>
    ''' <remarks> Scrivania: finder ante litteram </remarks>
    Public Property ReslDiaryLogDateFrom() As DateTime?

    ''' <summary>  </summary>
    ''' <remarks> Scrivania: finder ante litteram </remarks>
    Public Property ReslDiaryLogDateTo() As DateTime?

    Property UserDescription() As String
        Get
            Return DSUserDescription
        End Get
        Set(ByVal value As String)
            DSUserDescription = value
        End Set
    End Property

    Property UserMail As String
        Get
            Return DsUserMail
        End Get
        Set(ByVal value As String)
            DsUserMail = value
        End Set
    End Property

    Property UserComputer() As String
        Get
            Return DSUserComputer
        End Get
        Set(ByVal value As String)
            DSUserComputer = value
        End Set
    End Property

    Property HomeDirectory() As String
        Get
            Return HttpContext.Current.Session("HomeDirectory")
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session("HomeDirectory") = value
        End Set
    End Property

    ''' <summary> Path fisico dell'applicazione. </summary>
    ''' <remarks>
    ''' TODO: Verificare perchè non è stato usato Request.ApplicationPath
    ''' </remarks>
    Property AppPath As String

    ''' <summary> Path fisico alla cartella temporanea. </summary>
    Property AppTempPath As String

    ReadOnly Property TempDirectory As DirectoryInfo
        Get
            If _tempDirectory Is Nothing Then
                _tempDirectory = New DirectoryInfo(AppTempPath)
            End If
            Return _tempDirectory
        End Get
    End Property

    Property AppTempHttp As String


    ReadOnly Property AppServerName() As String
        Get
            Return HttpContext.Current.Request.ServerVariables("SERVER_NAME")
        End Get
    End Property

    Property AppAccessOk As Boolean

    Public ReadOnly Property DocmConnection() As String
        Get
            Return DocSuiteContext.Current.DocumentEnv.ConnectionString
        End Get
    End Property

    Public ReadOnly Property ProtConnection() As String
        Get
            Return DocSuiteContext.Current.ProtocolEnv.ConnectionString
        End Get
    End Property

    Public ReadOnly Property ReslConnection() As String
        Get
            Return DocSuiteContext.Current.ResolutionEnv.ConnectionString
        End Get
    End Property

    ''' <summary> Indica se le pratiche sono abilitate. </summary>
    ReadOnly Property DocmEnabled() As Boolean
        Get
            Return Not String.IsNullOrEmpty(DocmConnection)
        End Get
    End Property

    ''' <summary> Indica se il protocollo è abilitato. </summary>
    ReadOnly Property ProtEnabled() As Boolean
        Get
            Return Not String.IsNullOrEmpty(ProtConnection)
        End Get
    End Property

    ''' <summary> Indica se gli atti sono abilitati. </summary>
    ReadOnly Property ReslEnabled() As Boolean
        Get
            Return Not String.IsNullOrEmpty(ReslConnection)
        End Get
    End Property

#End Region

#Region " Methods "

    ''' <summary> Inizializza i parametri dell'utente. </summary>
    Public Sub LoadUserConfiguration()
        Dim s As String = HttpContext.Current.Request.ServerVariables("REMOTE_HOST")
        If Not String.IsNullOrEmpty(s) AndAlso s.ContainsIgnoreCase(UserDomain) AndAlso s.ContainsIgnoreCase(".") Then
            s = s.Substring(0, s.IndexOf(".", StringComparison.OrdinalIgnoreCase))
        End If
        DSUserComputer = s
        UserFullName = HttpContext.Current.User.Identity.Name
        UserSessionId = HttpContext.Current.Session.SessionID.ToString()
    End Sub

    Public Shared Sub Dispose()
        _instance = Nothing
    End Sub

    Private Function GetAppTempPath() As String
        If Not String.IsNullOrWhiteSpace(DocSuiteContext.ExternalAppTempPath) Then
            Return DocSuiteContext.ExternalAppTempPath
        End If
        Return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Temp\")
    End Function

    Public Sub InitializePaths(ByVal request As HttpRequest)
        AppPath = HostingEnvironment.ApplicationPhysicalPath
        AppTempPath = GetAppTempPath()
        AppTempHttp = String.Format("{0}/{1}", DocSuiteContext.Current.CurrentTenant.DSWUrl.TrimEnd("/"c), "Temp/")
    End Sub

    ''' <summary> Istanza singletone della <see cref="CommonUtil"/> </summary>
    ''' <remarks> Use 'Lazy initialization'. </remarks>
    Public Shared Function GetInstance() As CommonUtil
        If _instance Is Nothing Then
            _instance = New CommonUtil()
        End If
        Return _instance
    End Function

    ''' <summary> Cancella tutti i files temporanei dell'utente del tipo richiesto. </summary>
    ''' <remarks> NOTE: i files devono essere scritti come "nomeutente-"</remarks>
    Public Function UserDeleteTemp(ByVal type As TempType) As Boolean
        Dim tipologia As String
        Select Case type
            Case TempType.F
                tipologia = "*"
            Case TempType.I
                tipologia = "Insert-"
            Case TempType.V
                tipologia = "View-"
            Case TempType.P
                tipologia = "Print-"
            Case TempType.M
                tipologia = "Mail-"
            Case Else
                Exit Function
        End Select

        Dim sFile As String
        Try
            For Each sFile In Directory.GetFiles(AppTempPath, String.Format("{0}-{1}*.*", DocSuiteContext.Current.User.UserName, tipologia))
                File.Delete(sFile)
            Next
        Catch ex As Exception
            FileLogger.Warn(LogName.FileLog, "Errore cancellazione file temporanei", ex)
        End Try
    End Function


    Public Function VerifyProtocolSecurity(ByRef sRoles As String, ByRef sContainers As String, ByRef accounts As String, ByVal securityType As SecurityType) As Boolean
        Dim rf As New RoleFacade("ProtDB")
        Dim cf As New ContainerFacade("ProtDB")

        Dim rightContainer As ProtocolContainerRightPositions
        Dim rightRoles As ProtocolRoleRightPositions = ProtocolRoleRightPositions.Enabled
        Select Case securityType
            Case securityType.Read
                rightContainer = ProtocolContainerRightPositions.Preview
                If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                    rightRoles = ProtocolRoleRightPositions.Manager
                End If
            Case securityType.Write
                rightContainer = ProtocolContainerRightPositions.Insert
            Case securityType.Distribute
                rightContainer = ProtocolContainerRightPositions.DocDistribution
        End Select

        Dim containers As IList(Of Container) = cf.GetContainers(DSWEnvironment.Protocol, rightContainer, Nothing)

        Dim ids As New List(Of String)
        If Not containers.IsNullOrEmpty() Then
            ids = containers.Select(Function(c) c.Id.ToString()).ToList()
        End If
        sContainers = String.Join(",", ids)

        Dim onlyActive As Boolean?

        If (securityType <> SecurityType.Read) Then
            onlyActive = True
        End If

        If (DocSuiteContext.Current.ProtocolEnv.DisabledRolesRights) Then
            onlyActive = Nothing
        End If

        Dim roles As IList(Of Role) = rf.GetUserRoles(DSWEnvironment.Protocol, rightRoles, onlyActive)
        Dim idRoles As New List(Of String)
        If Not roles.IsNullOrEmpty() Then
            idRoles = roles.Select(Function(r) r.Id.ToString()).ToList()
        End If
        sRoles = String.Join(",", idRoles)

        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            accounts = DocSuiteContext.Current.User.FullUserName
        End If

        Return Not String.IsNullOrEmpty(sContainers) OrElse Not String.IsNullOrEmpty(sRoles) OrElse Not String.IsNullOrEmpty(accounts)
    End Function

    Public Sub InitializeDistributionRoles()
        If (DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled OrElse DocSuiteContext.Current.ProtocolEnv.RolesUserProfileEnabled) Then
            If (GroupProtocolManagerSelected Is Nothing) Then
                Dim managers As IList(Of Role) = FacadeFactory.Instance.RoleFacade().GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Manager, True)
                GroupProtocolManagerSelected = String.Empty
                If managers IsNot Nothing AndAlso managers.Count > 0 Then
                    GroupProtocolManagerSelected = String.Concat(String.Join("|,", managers.Select(Function(s) String.Concat("|", s.Id))), "|")
                End If
            End If
            If (GroupProtocolNotManagerSelected Is Nothing) Then
                Dim nonManagers As IList(Of Role) = FacadeFactory.Instance.RoleFacade().GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Enabled, True)
                GroupProtocolNotManagerSelected = String.Empty
                If nonManagers IsNot Nothing AndAlso nonManagers.Count > 0 Then
                    GroupProtocolNotManagerSelected = String.Concat(String.Join("|,", nonManagers.Select(Function(s) String.Concat("|", s.Id))), "|")
                End If
            End If
        End If
    End Sub

    ''' <summary> Inizializzazione utente. </summary>
    Public Function InitializeUser() As Boolean

        ' Description
        UserDescription = CommonAD.GetDisplayName(DocSuiteContext.Current.User.FullUserName)
        If String.IsNullOrEmpty(UserDescription) Then
            UserDescription = DocSuiteContext.Current.User.FullUserName
        End If
        UserMail = FacadeFactory.Instance.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, UserDomain, DocSuiteContext.Current.ProtocolEnv.UserLogEmail)
        InitializeDistributionRoles()
    End Function

    Public Sub ExcludeInvoiceContainer(ByRef finder As NHibernateProtocolFinder)
        If DocSuiteContext.Current.ProtocolEnv.InvoiceProtocolContainerIdentifiers.Any() Then
            Dim securityContainers As List(Of Integer) = finder.SecurityContainers?.Split(","c).[Select](AddressOf Integer.Parse).ToList()

            If securityContainers IsNot Nothing Then
                finder.SecurityContainers = String.Join(",", securityContainers.Except(DocSuiteContext.Current.ProtocolEnv.InvoiceProtocolContainerIdentifiers))
            End If
        End If
    End Sub

    Public Sub OnlyInvoiceContainer(ByRef finder As NHibernateProtocolFinder)
        If DocSuiteContext.Current.ProtocolEnv.InvoiceProtocolContainerIdentifiers.Any() Then
            Dim securityContainers As List(Of Integer) = finder.SecurityContainers?.Split(","c).[Select](AddressOf Integer.Parse).ToList()

            If securityContainers IsNot Nothing Then
                finder.SecurityContainers = String.Join(",", securityContainers.Where(Function(f) DocSuiteContext.Current.ProtocolEnv.InvoiceProtocolContainerIdentifiers.Contains(f)))
            End If
        End If
    End Sub

    Public Function ApplyProtocolFinderSecurity(ByRef finder As NHibernateProtocolFinder, ByVal secType As SecurityType, Optional IsProtocolSearch As Boolean = False) As Boolean
        If Not DocSuiteContext.Current.ProtocolEnv.IsSecurityEnabled Then
            Return True
        End If

        Dim roles As String = String.Empty
        Dim containers As String = String.Empty
        Dim account As String = String.Empty
        If Not VerifyProtocolSecurity(roles, containers, account, secType) Then
            Throw New DocSuiteException("Problema in verifica diritti", "Mancanza di diritti per eseguire ricerche nel modulo Protocollo.")
        End If

        finder.SecurityEnabled = True
        finder.DistributionEnable = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
        finder.SecurityRoles = roles
        finder.SecurityContainers = containers
        finder.RoleUser = account

        If (GroupProtocolManagerSelected Is Nothing OrElse GroupProtocolNotManagerSelected Is Nothing) Then
            InitializeDistributionRoles()
        End If

        Dim manageableRoles As String = String.Empty
        Dim nonManageableRoles As String = String.Empty
        If Not String.IsNullOrEmpty(GroupProtocolManagerSelected) Then
            manageableRoles = Replace(GroupProtocolManagerSelected, "|", String.Empty)
        End If
        If Not String.IsNullOrEmpty(GroupProtocolNotManagerSelected) Then
            nonManageableRoles = Replace(GroupProtocolNotManagerSelected, "|", String.Empty)
        End If

        Dim rf As New RoleFacade("ProtDB")

        Dim managerRoleIds As List(Of String) = New List(Of String)()
        Dim nonManageableRoleIds As List(Of String) = New List(Of String)()

        Dim onlyActive As Boolean? = True
        If (DocSuiteContext.Current.ProtocolEnv.DisabledRolesRights) Then
            onlyActive = Nothing
        End If

        If IsProtocolSearch Then
            Dim temproles As IList(Of Role) = rf.GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Manager, onlyActive)
            managerRoleIds = temproles.Select(Function(x) x.Id.ToString()).ToList()
            manageableRoles = String.Join(",", managerRoleIds)

            temproles = rf.GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Enabled, onlyActive)
            nonManageableRoleIds = temproles.Select(Function(x) x.Id.ToString()).ToList()
            nonManageableRoleIds = nonManageableRoleIds.Except(managerRoleIds).ToList()
            nonManageableRoles = String.Join(",", nonManageableRoleIds)
        End If


        If DocSuiteContext.Current.ProtocolEnv.RolesUserProfileEnabled Then
            finder.SecurityRoles = nonManageableRoles
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            Return True
        End If


        finder.SecurityRoles = manageableRoles
        finder.SecurityNonManageableRoles = nonManageableRoles

        Return True
    End Function


    ''' <summary> Verifica se ci sono permessi e imposta il finder con questi </summary>
    ''' <remarks> 
    ''' TODO: togliere <see cref="page"/> e mettere una <see cref="DocSuiteException"/>
    ''' </remarks>
    Public Function ApplyResolutionFinderSecurity(ByRef page As Page, ByRef finder As NHibernateResolutionFinder, Optional right As ResolutionRightPositions = ResolutionRightPositions.Preview) As Boolean
        If Not DocSuiteContext.Current.ResolutionEnv.Security Then
            Return True
        End If

        Dim resolutionContainers As IList(Of Container) = New ContainerFacade("ReslDB").GetContainers(DSWEnvironment.Resolution, right, Nothing)

        Dim roles As String
        Dim userRoles As IList(Of Role) = New RoleFacade("ReslDB").GetUserRoles(DSWEnvironment.Resolution, ResolutionRoleRightPositions.Enabled, True)
        Dim roleIds As List(Of String) = userRoles.Select(Function(x) x.Id.ToString()).ToList()
        roles = String.Join(",", roleIds)
        'If String.IsNullOrEmpty(ResolutionUtil.GetInstance.EnvGroupSelected) Then

        'Else
        '    roles = Replace(ResolutionUtil.GetInstance.EnvGroupSelected, "|", String.Empty)
        'End If

        'Verifica diritti + ruoli
        If Not resolutionContainers.IsNullOrEmpty() OrElse Not String.IsNullOrEmpty(roles) Then
            finder.Roles = roles

            If Not String.IsNullOrEmpty(finder.ContainerIds) Then
                Dim containerId As Integer = 0
                If Integer.TryParse(finder.ContainerIds, containerId) Then
                    finder.SelectedContainerId = containerId
                End If
            End If

            finder.ContainerIds = String.Join(",", resolutionContainers.Select(Function(x) x.Id.ToString()))

            Return True
        End If
        'non ci sono diritti per eseguire la ricera
        WebHelper.Alert(page, "Diritti insufficienti per la ricerca nel modulo Atti")
        Return False
    End Function

    Public Function ApplyPecMailFinderSecurity(ByRef finder As NHibernatePECMailFinder) As Boolean
        Dim userRoles As IList(Of Role) = New RoleFacade().GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.PEC, True)
        Dim roleIds As List(Of String) = userRoles.Select(Function(x) x.Id.ToString()).ToList()
        If (Not String.IsNullOrEmpty(GroupProtocolNotManagerSelected)) Then
            Dim ids As List(Of String) = GroupProtocolNotManagerSelected.Split("|".ToCharArray()).Select(Function(f) f).ToList()
            roleIds = roleIds.Intersect(ids).ToList()
            'roleIds = roleIds.Where(Function(f) ids.Contains(f)).ToList()
        End If
        finder.Roles = String.Join(",", roleIds)
    End Function

    Public Function VerifyDocumentSecurity(ByRef sRoles As String, ByRef sContainers As String, ByVal securityType As SecurityType) As Boolean
        Dim roleFacade As New RoleFacade("DocmDB")
        Dim containerFacade As New ContainerFacade("DocmDB")

        Dim right As DocumentContainerRightPositions
        Select Case securityType
            Case securityType.Read
                right = DocumentContainerRightPositions.Preview
            Case securityType.Write
                right = DocumentContainerRightPositions.Insert
        End Select

        Dim containers As IList(Of Container) = containerFacade.GetContainers(DSWEnvironment.Document, right, Nothing)
        If Not containers.IsNullOrEmpty() Then
            Dim ids As List(Of String) = containers.Select(Function(c) c.Id.ToString()).ToList()
            sContainers = String.Join(",", ids)
        End If

        If String.IsNullOrEmpty(CommonShared.GroupPaperworkSelected) Then
            Dim roles As IList(Of Role) = roleFacade.GetUserRoles(DSWEnvironment.Document, DocumentRoleRightPositions.Workflow, True)
            sRoles = String.Join(",", roles.Select(Function(x) x.Id.ToString()))
        Else
            sRoles = Replace(CommonShared.GroupPaperworkSelected, "|", "")
        End If

        'Verifica diritti + ruoli
        Return Not (String.IsNullOrEmpty(sContainers) AndAlso String.IsNullOrEmpty(sRoles))
    End Function

    Public Function ApplyDocumentFinderSecurity(ByRef finder As NHibernateDocumentFinder) As Boolean
        Dim roles As String = String.Empty
        Dim containers As String = String.Empty

        If Not DocSuiteContext.Current.DocumentEnv.IsSecurityEnabled Then
            Return True
        End If

        If VerifyDocumentSecurity(roles, containers, SecurityType.Read) Then
            finder.SecurityEnabled = DocSuiteContext.Current.DocumentEnv.IsSecurityEnabled
            finder.SecurityRoles = roles
            finder.SecurityContainers = containers
            Return True
        Else
            'non ci sono diritti per eseguire la ricera
            Return False
        End If
    End Function

    ''' <summary>Verifica se l'utente corrente è un super admin</summary>
    ''' <param name="errorMessage">Motivo dell'autorizzazione negata</param>
    ''' <remarks>Mi assicuro di controllare il lancio di eccezioni per evitare di visualizzare le righe di codice</remarks>
    Public Shared Function IsSuperAdmin(ByRef errorMessage As String) As Boolean
        Dim authorization As Boolean = False

#If DEBUG Then
        authorization = True
#Else
        Try
            If CommonShared.UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.SuperAdmin) Then
                For Each item As String In DocSuiteContext.Current.ProtocolEnv.DSWEnable.Split("|".ToCharArray())
                    If DocSuiteContext.Current.User.FullUserName.Eq(item) Then
                        authorization = True
                    End If
                Next
            End If

            If Not authorization Then
                errorMessage = "Utente non abilitato"
            End If
        Catch wx As Exception
            ' Uso un blocco try catch per evitare di mostrare l'implementazione a video in caso di errore
            errorMessage = "Errore di autenticazione"
        End Try
#End If
        Return authorization
    End Function



    Public Function BiblosExistFile(ByRef documents As IList(Of DocumentInfo), Optional ByVal tempPath As Boolean = True) As Boolean
        Return documents.All(Function(document) document.Exists())
    End Function

    Public Shared Sub SendMail(sender As String, recipients As IList(Of String), recipientsCC As IList(Of String), recipientsBcc As IList(Of String), subject As String, body As String)
        Dim mail As New MailMessage
        mail.IsBodyHtml = True

        Dim mailSender As String = sender
        If String.IsNullOrEmpty(mailSender) Then
            mailSender = DocSuiteContext.Current.ProtocolEnv.ProtPecSendSender
        End If
        If String.IsNullOrEmpty(mailSender) Then
            Throw New ArgumentNullException("Mittente mancante o non specificato.")
        End If
        mail.From = New MailAddress(mailSender)

        ' Destinatari
        If recipients IsNot Nothing AndAlso recipients.Count > 0 Then
            For Each recipient As String In recipients
                mail.To.Add(recipient)
            Next
        Else
            Throw New ArgumentNullException("Destinatario mancante o non specificato.")
        End If

        ' Destinatari in copia conoscenza
        If recipientsCC IsNot Nothing AndAlso recipientsCC.Count > 0 Then
            For Each recipientCC As String In recipientsCC
                mail.CC.Add(recipientCC)
            Next
        End If

        ' Destinatari in copia carbone
        If recipientsBcc IsNot Nothing AndAlso recipientsBcc.Count > 0 Then
            For Each recipientBcc As String In recipientsBcc
                mail.Bcc.Add(recipientBcc)
            Next
        End If

        mail.Subject = subject
        mail.Body = body

        Dim smtpServer As String = DocSuiteContext.Current.ProtocolEnv.MailSmtpServer
        If String.IsNullOrEmpty(smtpServer) Then
            Throw New ArgumentNullException("Smtp Server mancante o non specificato.")
        End If
        Dim server As New SmtpClient(smtpServer)
        server.UseDefaultCredentials = True
        server.Send(mail)
    End Sub

    Public Shared Sub SendMail(sender As String, recipient As String, subject As String, body As String)
        Dim recipients As New List(Of String)
        recipients.Add(recipient)
        SendMail(sender, recipients, Nothing, Nothing, subject, body)
    End Sub

    ''' <summary> Pulisce i file temporanei </summary>
    ''' <remarks> Solo per utente corrente e più vecchi di ProtocolEnv.TimeIntervalForCleaningTempUser /></remarks>
    Public Shared Function ClearTemporaryDirectory(tempPath As String, username As String, minutesOld As Integer) As Boolean
        Try
            For Each tempFileName As String In Directory.GetFiles(tempPath, username & "-**.*")
                Dim diffTime As TimeSpan = DateTime.Now - File.GetLastAccessTime(tempFileName)
                If Convert.ToInt32(diffTime.TotalMinutes) >= minutesOld Then
                    File.Delete(tempFileName)
                End If
            Next
        Catch ex As UnauthorizedAccessException
            FileLogger.Warn(LogName.FileLog, String.Format("Non è stato possibile eliminare i file nella temp, non si dispongono di sufficienti diritti per [{0}]", tempPath), ex)
            Return False
        Catch ex As Exception
            FileLogger.Warn(LogName.FileLog, "Non è stato possibile eliminare i file nella temp.", ex)
            Return False
        End Try
        Return True
    End Function

    Public Sub SetGroupRight(ByRef field As String, ByVal right As Integer, ByVal value As Boolean)
        If value Then
            Mid$(field, right, 1) = "1"
        Else
            Mid$(field, right, 1) = "0"
        End If
    End Sub

    ''' <summary>
    ''' Recupera la ActionPage associata ad una specifica WorkflowActivity
    ''' </summary>
    ''' <param name="workflowActivityArea"></param>
    ''' <param name="workflowAction"></param>
    ''' <returns></returns>
    Public Function GetWorkflowActionPage(workflowActivityArea As WorkflowActivityArea, workflowAction As WorkflowActivityAction) As String
        Dim workflowOperation As WorkflowOperationConfig = DocSuiteContext.Current.DocSuiteWorkflowOperation
        If workflowOperation IsNot Nothing AndAlso Not workflowOperation.Areas.IsNullOrEmpty() AndAlso workflowOperation.Areas.ContainsKey(workflowActivityArea.ToString()) Then
            Dim workflowArea As WorkflowOperationAction = workflowOperation.Areas(workflowActivityArea.ToString())
            If Not workflowArea.Actions.IsNullOrEmpty() AndAlso workflowArea.Actions.ContainsKey(workflowAction.GetDescription()) Then
                Return workflowArea.Actions(workflowAction.ToString()).Page
            End If
        End If
        Return String.Empty
    End Function

    ''' <summary>
    ''' Recupera le ActionOptions associate ad una specifica WorkflowActivity
    ''' </summary>
    ''' <param name="workflowActivityArea"></param>
    ''' <param name="workflowAction"></param>
    ''' <returns></returns>
    Public Function GetWorkflowActionOptions(workflowActivityArea As WorkflowActivityArea, workflowAction As WorkflowActivityAction) As IDictionary(Of String, String)
        Dim workflowOperation As WorkflowOperationConfig = DocSuiteContext.Current.DocSuiteWorkflowOperation
        If workflowOperation IsNot Nothing AndAlso Not workflowOperation.Areas.IsNullOrEmpty() AndAlso workflowOperation.Areas.ContainsKey(workflowActivityArea.ToString()) Then
            Dim workflowArea As WorkflowOperationAction = workflowOperation.Areas(workflowActivityArea.ToString())
            If Not workflowArea.Actions.IsNullOrEmpty() AndAlso workflowArea.Actions.ContainsKey(workflowAction.GetDescription()) Then
                Return workflowArea.Actions(workflowAction.ToString()).Options
            End If
        End If
        Return New Dictionary(Of String, String)
    End Function

#End Region

End Class