Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Desks
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Model.Parameters

Public Class DeskBasePage
    Inherits CommonBasePage

#Region "Fields"
    ''' <summary><see cref="CommonBasePage.Type"/> da usare come default</summary>
    Public Const DefaultType As String = "Desk"
    Private _facade As FacadeFactory
    Private _currentDeskId As Guid?
    Private _deskFacade As DeskFacade
    Private _currentDesk As Desk
    Private _currentFinderQueryString As String
    Private _currentDeskRigths As DeskRightsUtil
    Private _deskDocumentFacade As DeskDocumentFacade
    Private _deskRoleUserFacade As DeskRoleUserFacade
    Private _currentDeskDocumentId As Guid?
    Private _currentDeskDocument As DeskDocument
    Private _deskDocumentEndorsmentFacade As DeskDocumentEndorsementFacade
    Private _meDeskRoleUser As DeskRoleUser
    Private _deskStoryBoardFacade As DeskStoryBoardFacade
    Private _deskDocumentVersionfacade As DeskDocumentVersionFacade
    Private _deskMessageFacade As DeskMessageFacade
    Private _currentDeskLogFacade As DeskLogFacade

#End Region

#Region "Properties"
    Protected ReadOnly Property CurrentDeskLogFacade As DeskLogFacade
        Get
            If _currentDeskLogFacade Is Nothing Then
                _currentDeskLogFacade = New DeskLogFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentDeskLogFacade
        End Get
    End Property

    Protected Overridable ReadOnly Property CurrentDeskFacade As DeskFacade
        Get
            If _deskFacade Is Nothing Then
                _deskFacade = New DeskFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskFacade
        End Get
    End Property


    Protected Overridable ReadOnly Property CurrentDeskMessageFacade As DeskMessageFacade
        Get
            If _deskMessageFacade Is Nothing Then
                _deskMessageFacade = New DeskMessageFacade()
            End If
            Return _deskMessageFacade
        End Get
    End Property

    Protected Overridable ReadOnly Property CurrentDeskDocumentFacade As DeskDocumentFacade
        Get
            If _deskDocumentFacade Is Nothing Then
                _deskDocumentFacade = New DeskDocumentFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskDocumentFacade
        End Get
    End Property
    Protected Overridable ReadOnly Property CurrentDeskStoryBoardFacade As DeskStoryBoardFacade
        Get
            If _deskStoryBoardFacade Is Nothing Then
                _deskStoryBoardFacade = New DeskStoryBoardFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskStoryBoardFacade
        End Get
    End Property

    Protected Overridable ReadOnly Property CurrentDeskRoleUserFacade As DeskRoleUserFacade
        Get
            If _deskRoleUserFacade Is Nothing Then
                _deskRoleUserFacade = New DeskRoleUserFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskRoleUserFacade
        End Get
    End Property

    Protected Overridable ReadOnly Property CurrentDeskDocumentEndorsmentFacade As DeskDocumentEndorsementFacade
        Get
            If _deskDocumentEndorsmentFacade Is Nothing Then
                _deskDocumentEndorsmentFacade = New DeskDocumentEndorsementFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskDocumentEndorsmentFacade
        End Get
    End Property

    Protected Overridable ReadOnly Property CurrentDeskDocumentVersionFacade As DeskDocumentVersionFacade
        Get
            If _deskDocumentVersionfacade Is Nothing Then
                _deskDocumentVersionfacade = New DeskDocumentVersionFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskDocumentVersionfacade
        End Get
    End Property

    Public Overrides ReadOnly Property Facade As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory("ProtDB")
            End If
            Return _facade
        End Get
    End Property

    Protected ReadOnly Property CurrentDeskId As Guid?
        Get
            If Not _currentDeskId.HasValue Then
                _currentDeskId = GetKeyValue(Of Guid?)("DeskId")
            End If
            Return _currentDeskId
        End Get
    End Property

    Protected ReadOnly Property CurrentFinderQueryString As String
        Get
            If String.IsNullOrEmpty(_currentFinderQueryString) Then
                _currentFinderQueryString = GetKeyValue(Of String)("Finder")
            End If
            Return _currentFinderQueryString
        End Get
    End Property

    Protected ReadOnly Property CurrentDesk As Desk
        Get
            If _currentDesk Is Nothing AndAlso CurrentDeskId.HasValue Then
                _currentDesk = CurrentDeskFacade.GetById(CurrentDeskId.Value)
            End If
            Return _currentDesk
        End Get
    End Property

    Protected ReadOnly Property CurrentDeskDocumentId As Guid?
        Get
            If Not _currentDeskDocumentId.HasValue Then
                _currentDeskDocumentId = GetKeyValue(Of Guid?)("DeskDocumentId")
            End If
            Return _currentDeskDocumentId
        End Get
    End Property

    Protected ReadOnly Property CurrentDeskDocument As DeskDocument
        Get
            If _currentDeskDocument Is Nothing AndAlso CurrentDeskId.HasValue Then
                _currentDeskDocument = CurrentDeskDocumentFacade.GetById(CurrentDeskDocumentId.Value)
            End If
            Return _currentDeskDocument
        End Get
    End Property

    Protected Overloads ReadOnly Property CurrentDeskRigths As DeskRightsUtil
        Get
            If _currentDeskRigths Is Nothing AndAlso CurrentDesk IsNot Nothing Then
                _currentDeskRigths = New DeskRightsUtil(CurrentDesk, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentDeskRigths
        End Get
    End Property

    Protected Overloads ReadOnly Property MeDeskRoleUser As DeskRoleUser
        Get
            If _meDeskRoleUser Is Nothing AndAlso CurrentDesk IsNot Nothing Then
                _meDeskRoleUser = CurrentDesk.DeskRoleUsers.SingleOrDefault(Function(x) x.AccountName.Eq(DocSuiteContext.Current.User.FullUserName))
            End If
            Return _meDeskRoleUser
        End Get
    End Property
#End Region

#Region "Constructor"
    Public Sub New()

    End Sub
#End Region

#Region "Methods"

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, DeskBasePage)(key)
    End Function

    Protected Sub SendInvitedMessage(desk As Desk, user As DeskRoleUserResult)
        Dim contacts As List(Of MessageContactEmail) = New List(Of MessageContactEmail)
        Dim adUser As AccountModel = CommonAD.GetAccount(user.UserName)
        If (adUser IsNot Nothing AndAlso Not String.IsNullOrEmpty(adUser.Email)) Then
            contacts.Add(FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(CommonUtil.GetInstance().UserDescription, DocSuiteContext.Current.User.FullUserName, FacadeFactory.Instance.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True), MessageContact.ContactPositionEnum.Sender))
            contacts.Add(FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(adUser.Name, DocSuiteContext.Current.User.FullUserName, adUser.Email, MessageContact.ContactPositionEnum.Recipient))
            Dim mailSubject As String = String.Format("Notifica dal Tavolo {0}", desk.Name)
            Dim mailBody As String = String.Format("Sei stato invitato al tavolo {0} che scadrà il {1} col ruolo di {2}", desk.Name, desk.ExpirationDate.Value.ToShortDateString(), user.PermissionType.Value.GetDescription())
            Dim email As MessageEmail = FacadeFactory.Instance.MessageEmailFacade.CreateEmailMessage(contacts, mailSubject, mailBody, False)
            CurrentDeskMessageFacade.Save(New DeskMessage() With {.Desk = desk, .Message = email})
            Dim idMessage As Integer = FacadeFactory.Instance.MessageEmailFacade.SendEmailMessage(email)
            FileLogger.Info(LoggerName, String.Format("{0}: DeskBase.SendInvitedMessage - Tavolo [{1}] per {2} : Mail inserita in coda di invio [id {3}]", DocSuiteContext.Current.User.FullUserName, desk.Id, user.UserName, idMessage))

        End If
    End Sub

    Protected Sub SendApprovalRequested(desk As Desk, user As DeskRoleUserResult)
        Dim contacts As List(Of MessageContactEmail) = New List(Of MessageContactEmail)
        Dim adUser As AccountModel = CommonAD.GetAccount(user.UserName)
        If (adUser IsNot Nothing AndAlso Not String.IsNullOrEmpty(adUser.Email)) Then
            contacts.Add(FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(CommonUtil.GetInstance().UserDescription, DocSuiteContext.Current.User.FullUserName, FacadeFactory.Instance.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True), MessageContact.ContactPositionEnum.Sender))
            contacts.Add(FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(adUser.Name, DocSuiteContext.Current.User.FullUserName, adUser.Email, MessageContact.ContactPositionEnum.Recipient))
            Dim mailSubject As String = String.Format("Notifica dal Tavolo {0}", desk.Name)
            Dim mailBody As String = String.Format("E' stata richiesta la tua approvazione al tavolo {0}", desk.Name, desk.ExpirationDate.Value.ToShortDateString())
            Dim email As MessageEmail = FacadeFactory.Instance.MessageEmailFacade.CreateEmailMessage(contacts, mailSubject, mailBody, False)
            CurrentDeskMessageFacade.Save(New DeskMessage() With {.Desk = desk, .Message = email})
            Dim idMessage As Integer = FacadeFactory.Instance.MessageEmailFacade.SendEmailMessage(email)
            FileLogger.Info(LoggerName, String.Format("{0}: DeskBase.SendInvitedMessage - Tavolo [{1}] per {2} : Mail inserita in coda di invio [id {3}]", DocSuiteContext.Current.User.FullUserName, desk.Id, user.UserName, idMessage))

        End If
    End Sub

    Protected Sub ResetCurrentDesk()
        _currentDesk = Nothing
    End Sub

    Protected Function GetControlTemplateDocumentVisibility(chainType As ChainType) As Boolean
        If Not ProtocolEnv.TemplateDocumentVisibilities.Any(Function(x) x.Name.Eq(NameOf(Desk))) Then
            Return False
        End If

        Dim modelVisibility As TemplateDocumentVisibilityConfiguration = ProtocolEnv.TemplateDocumentVisibilities.First(Function(x) x.Name.Eq(NameOf(Desk)))
        Return modelVisibility.VisibilityChains.ContainsKey(chainType) AndAlso modelVisibility.VisibilityChains(chainType)
    End Function
#End Region
End Class
