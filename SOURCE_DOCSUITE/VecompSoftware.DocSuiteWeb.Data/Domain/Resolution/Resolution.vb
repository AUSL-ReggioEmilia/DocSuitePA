Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Linq

<Serializable()> _
Public Class Resolution
    Inherits DomainObject(Of Int32)

    ''' <summary> Stati web dell'atto. </summary>
    ''' <remarks>ASL-TO2</remarks>
    Enum WebStateEnum
        None = 0
        Published = 1
        Revoked = 2
    End Enum

#Region " Fields "

    Private _container As Container
    Private _type As ResolutionType
    Private _year As Nullable(Of Short)
    Private _number As Integer?
    Private _workflowType As String
    Private _status As ResolutionStatus
    Private _adoptionDate As Date?
    Private _adoptionUser As String
    Private _publishingDate As Date?
    Private _publishingUser As String
    Private _effectivenessDate As Date?
    Private _effectivenessUser As String
    Private _leaveDate As Date?
    Private _leaveuser As String
    Private _warningDate As Date?
    Private _confirmDate As Date?
    Private _object As String
    Private _note As String
    Private _serviceNumber As String
    Private _idProposer As Nullable(Of Short)
    Private _lastChangedDate As DateTimeOffset?
    Private _idManager As Nullable(Of Short)
    Private _idAssignee As Nullable(Of Short)
    Private _lastChangedUser As String
    Private _lastChangedReason As String
    Private _controllerOpinion As String
    Private _confirmUser As String
    Private _warningUser As String
    Private _idLocation As Location
    Private _alternativeProposer As String
    Private _alternativeManager As String
    Private _alternativeAssignee As String
    Private _alternativeRecipient As String
    Private _frontispieceObject As String
    Private _controllerStatus As ControllerStatusResolution
    Private _proposeDate As Date?
    Private _proposeUser As String
    Private _proposerCode As String
    Private _waitDate As Date?
    Private _waitUser As String
    Private _responseProtocol As String
    Private _warningProtocol As String
    Private _confirmProtocol As String
    Private _responseDate As Date?
    Private _responseUser As String
    Private _send As Nullable(Of Boolean)
    Private _position As String
    Private _validityDateFrom As Date?
    Private _validityDateTo As Date?
    Private _bidType As BidType
    Private _supplierCode As String
    Private _supplierDescription As String
    Private _category As Category
    Private _subCategory As Category

    Private _checkPublication As String
    Private _file As FileResolution

    Private _declineNote As String
    Private _approvalNote As String

    Private _inclusiveNumber As String

    Private _ultimaPaginaDate As DateTimeOffset?

    'Pubblicazione
    Private _webPublicationDate As Date?
    Private _webState As Nullable(Of WebStateEnum)
    Private _webRevokeDate As Date?
    Private _webSPGuid As String
    Private _webPublicatedDocuments As String

    'AUSL-RE
    Private _isChecked As Nullable(Of Boolean)


#Region "Collections"
    Private _resolutionContacts As IList(Of ResolutionContact)
    Private _resolutionContactsRecipients As IList(Of ResolutionContact)
    Private _resolutionContactProposers As IList(Of ResolutionContact)
    Private _resolutionContactsAssignees As IList(Of ResolutionContact)
    Private _resolutionContactsManagers As IList(Of ResolutionContact)
    Private _resolutionLogs As IList(Of ResolutionLog)
    Private _resolutionWorkflows As IList(Of ResolutionWorkflow)
#End Region

#Region "ASL-TO2"
    Private _immediatelyExecutive As Nullable(Of Boolean)
    Private _ocSupervisoryBoard As Nullable(Of Boolean)
    Private _ocRegion As Nullable(Of Boolean)
    Private _ocManagement As Nullable(Of Boolean)
    Private _ocOther As Nullable(Of Boolean)
    Private _ocCorteConti As Nullable(Of Boolean)
    Private _otherDescription As String
    Private _proposerWarningDate As Date?
    Private _proposerWarningUser As String
    Private _proposerProtocolLink As String
    Private _publishingProtocolLink As String
    Private _regionProtocolLink As String
    Private _effectivenessProtocolLink As String
    Private _supervisoryBoardProtocolLink As String
    Private _supervisoryBoardOpinion As String
    Private _supervisoryBoardWarningDate As Date?
    Private _supervisoryBoardWarningUser As String
    Private _dgr As String
    Private _managementWarningDate As Date?
    Private _managementWarningUser As String
    Private _managementProtocolLink As String
    Private _objectPrivacy As String
    Private _corteDeiContiWarningDate As Date?
    Private _corteDeiContiProtocolLink As String
#End Region

#End Region

#Region "Public Properties"
    Public Overridable ReadOnly Property IdFull() As String
        Get
            Return String.Format("{0:0000000}", Id)
        End Get
    End Property

    Public Overridable Property Container() As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
        End Set
    End Property

    Public Overridable Property Type() As ResolutionType
        Get
            Return _type
        End Get
        Set(ByVal value As ResolutionType)
            _type = value
        End Set
    End Property

    Public Overridable Property Year() As Nullable(Of Short)
        Get
            Return _year
        End Get
        Set(ByVal value As Nullable(Of Short))
            _year = value
        End Set
    End Property

    ''' <summary>
    ''' Numero dell'atto, presente se protocollato.
    ''' </summary>
    ''' <value>Se non è impostato è una proposta di delibera/determina.</value>
    Public Overridable Property Number() As Integer?
        Get
            Return _number
        End Get
        Set(ByVal value As Integer?)
            _number = value
        End Set
    End Property

    Public Overridable ReadOnly Property NumberFormat(ByVal format As String) As String
        Get
            Return String.Format(format, Number)
        End Get
    End Property

    Public Overridable Property WorkflowType() As String
        Get
            Return _workflowType
        End Get
        Set(ByVal value As String)
            _workflowType = value
        End Set
    End Property

    Public Overridable Property Status() As ResolutionStatus
        Get
            Return _status
        End Get
        Set(ByVal value As ResolutionStatus)
            _status = value
        End Set
    End Property

    Public Overridable Property AdoptionDate() As Date?
        Get
            Return _adoptionDate
        End Get
        Set(ByVal value As Date?)
            _adoptionDate = value
        End Set
    End Property

    Public Overridable Property AdoptionUser() As String
        Get
            Return _adoptionUser
        End Get
        Set(ByVal value As String)
            _adoptionUser = value
        End Set
    End Property

    Public Overridable Property PublishingDate() As Date?
        Get
            Return _publishingDate
        End Get
        Set(ByVal value As Date?)
            _publishingDate = value
        End Set
    End Property

    Public Overridable Property PublishingUser() As String
        Get
            Return _publishingUser
        End Get
        Set(ByVal value As String)
            _publishingUser = value
        End Set
    End Property

    Public Overridable Property EffectivenessDate() As Date?
        Get
            Return _effectivenessDate
        End Get
        Set(ByVal value As Date?)
            _effectivenessDate = value
        End Set
    End Property

    Public Overridable Property EffectivenessUser() As String
        Get
            Return _effectivenessUser
        End Get
        Set(ByVal value As String)
            _effectivenessUser = value
        End Set
    End Property

    Public Overridable Property LeaveDate() As Date?
        Get
            Return _leaveDate
        End Get
        Set(ByVal value As Date?)
            _leaveDate = value
        End Set
    End Property

    Public Overridable Property Leaveuser() As String
        Get
            Return _leaveuser
        End Get
        Set(ByVal value As String)
            _leaveuser = value
        End Set
    End Property

    Public Overridable Property WarningDate() As Date?
        Get
            Return _warningDate
        End Get
        Set(ByVal value As Date?)
            _warningDate = value
        End Set
    End Property

    Public Overridable ReadOnly Property WarningDateFormat(ByVal format As String) As String
        Get
            Return FormatDateTime(WarningDate, format)
        End Get
    End Property

    Public Overridable Property ConfirmDate() As Date?
        Get
            Return _confirmDate
        End Get
        Set(ByVal value As Date?)
            _confirmDate = value
        End Set
    End Property

    Public Overridable ReadOnly Property ConfirmDateFormat(ByVal format As String) As String
        Get
            Return FormatDateTime(ConfirmDate, format)
        End Get
    End Property

    Public Overridable Property ResolutionObject() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property

    Public Overridable Property Note() As String
        Get
            Return _note
        End Get
        Set(ByVal value As String)
            _note = value
        End Set
    End Property

    Public Overridable Property ServiceNumber() As String
        Get
            Return _serviceNumber
        End Get
        Set(ByVal value As String)
            _serviceNumber = value
        End Set
    End Property

    Public Overridable Property IdProposer() As Nullable(Of Short)
        Get
            Return _idProposer
        End Get
        Set(ByVal value As Nullable(Of Short))
            _idProposer = value
        End Set
    End Property

    Public Overridable Property LastChangedDate As DateTimeOffset?
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property IdManager() As Nullable(Of Short)
        Get
            Return _idManager
        End Get
        Set(ByVal value As Nullable(Of Short))
            _idManager = value
        End Set
    End Property

    Public Overridable Property IdAssignee() As Nullable(Of Short)
        Get
            Return _idAssignee
        End Get
        Set(ByVal value As Nullable(Of Short))
            _idAssignee = value
        End Set
    End Property

    Public Overridable Property LastChangedUser As String
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property

    Public Overridable Property LastChangedReason() As String
        Get
            Return _lastChangedReason
        End Get
        Set(ByVal value As String)
            _lastChangedReason = value
        End Set
    End Property

    Public Overridable Property ControllerOpinion() As String
        Get
            Return _controllerOpinion
        End Get
        Set(ByVal value As String)
            _controllerOpinion = value
        End Set
    End Property

    Public Overridable Property ConfirmUser() As String
        Get
            Return _confirmUser
        End Get
        Set(ByVal value As String)
            _confirmUser = value
        End Set
    End Property

    Public Overridable Property WarningUser() As String
        Get
            Return _warningUser
        End Get
        Set(ByVal value As String)
            _warningUser = value
        End Set
    End Property

    Public Overridable Property Location() As Location
        Get
            Return _idLocation
        End Get
        Set(ByVal value As Location)
            _idLocation = value
        End Set
    End Property

    Public Overridable Property AlternativeProposer() As String
        Get
            Return _alternativeProposer
        End Get
        Set(ByVal value As String)
            _alternativeProposer = value
        End Set
    End Property

    Public Overridable Property AlternativeManager() As String
        Get
            Return _alternativeManager
        End Get
        Set(ByVal value As String)
            _alternativeManager = value
        End Set
    End Property

    Public Overridable Property AlternativeAssignee() As String
        Get
            Return _alternativeAssignee
        End Get
        Set(ByVal value As String)
            _alternativeAssignee = value
        End Set
    End Property

    Public Overridable Property AlternativeRecipient() As String
        Get
            Return _alternativeRecipient
        End Get
        Set(ByVal value As String)
            _alternativeRecipient = value
        End Set
    End Property

    Public Overridable Property FrontispieceObject() As String
        Get
            Return _frontispieceObject
        End Get
        Set(ByVal value As String)
            _frontispieceObject = value
        End Set
    End Property

    Public Overridable Property ControllerStatus() As ControllerStatusResolution
        Get
            Return _controllerStatus
        End Get
        Set(ByVal value As ControllerStatusResolution)
            _controllerStatus = value
        End Set
    End Property

    Public Overridable Property ProposeDate() As Date?
        Get
            Return _proposeDate
        End Get
        Set(ByVal value As Date?)
            _proposeDate = value
        End Set
    End Property

    Public Overridable Property ProposeUser() As String
        Get
            Return _proposeUser
        End Get
        Set(ByVal value As String)
            _proposeUser = value
        End Set
    End Property

    Public Overridable Property ProposerCode() As String
        Get
            Return _proposerCode
        End Get
        Set(ByVal value As String)
            _proposerCode = value
        End Set
    End Property

    Public Overridable Property WaitDate() As Date?
        Get
            Return _waitDate
        End Get
        Set(ByVal value As Date?)
            _waitDate = value
        End Set
    End Property

    Public Overridable ReadOnly Property WaitDateFormat(ByVal format As String) As String
        Get
            Return FormatDateTime(WaitDate, format)
        End Get
    End Property

    Public Overridable Property WaitUser() As String
        Get
            Return _waitUser
        End Get
        Set(ByVal value As String)
            _waitUser = value
        End Set
    End Property

    Public Overridable Property ResponseProtocol() As String
        Get
            Return _responseProtocol
        End Get
        Set(ByVal value As String)
            _responseProtocol = value
        End Set
    End Property

    Public Overridable Property WarningProtocol() As String
        Get
            Return _warningProtocol
        End Get
        Set(ByVal value As String)
            _warningProtocol = value
        End Set
    End Property

    Public Overridable Property ConfirmProtocol() As String
        Get
            Return _confirmProtocol
        End Get
        Set(ByVal value As String)
            _confirmProtocol = value
        End Set
    End Property

    Public Overridable Property ResponseDate() As Date?
        Get
            Return _responseDate
        End Get
        Set(ByVal value As Date?)
            _responseDate = value
        End Set
    End Property

    Public Overridable ReadOnly Property ResponseDateFormat(ByVal format As String) As String
        Get
            Return FormatDateTime(ResponseDate, format)
        End Get
    End Property

    Public Overridable Property ResponseUser() As String
        Get
            Return _responseUser
        End Get
        Set(ByVal value As String)
            _responseUser = value
        End Set
    End Property

    Public Overridable Property Send() As Nullable(Of Boolean)
        Get
            Return _send
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _send = value
        End Set
    End Property

    Public Overridable Property Position() As String
        Get
            Return _position
        End Get
        Set(ByVal value As String)
            _position = value
        End Set
    End Property

    Public Overridable Property ValidityDateFrom() As Date?
        Get
            Return _validityDateFrom
        End Get
        Set(ByVal value As Date?)
            _validityDateFrom = value
        End Set
    End Property

    Public Overridable ReadOnly Property ValidityDateFromFormat(ByVal format As String) As String
        Get
            Return FormatDateTime(ValidityDateFrom, format)
        End Get
    End Property

    Public Overridable Property ValidityDateTo() As Date?
        Get
            Return _validityDateTo
        End Get
        Set(ByVal value As Date?)
            _validityDateTo = value
        End Set
    End Property

    Public Overridable ReadOnly Property ValidityDateToFormat(ByVal format As String) As String
        Get
            Return FormatDateTime(ValidityDateTo, format)
        End Get
    End Property

    Public Overridable Property BidType() As BidType
        Get
            Return _bidType
        End Get
        Set(ByVal value As BidType)
            _bidType = value
        End Set
    End Property

    Public Overridable Property SupplierCode() As String
        Get
            Return _supplierCode
        End Get
        Set(ByVal value As String)
            _supplierCode = value
        End Set
    End Property

    Public Overridable Property SupplierDescription() As String
        Get
            Return _supplierDescription
        End Get
        Set(ByVal value As String)
            _supplierDescription = value
        End Set
    End Property

    Public Overridable Property Category() As Category
        Get
            Return _category
        End Get
        Set(ByVal value As Category)
            _category = value
        End Set
    End Property

    Public Overridable ReadOnly Property CategoryCode() As String
        Get
            Dim _code As String = String.Empty
            If SubCategory Is Nothing Then
                If Category IsNot Nothing Then
                    _code = (Category.FullCode.Replace("0", ""))
                End If
            Else
                _code = (SubCategory.FullCode.Replace("0", ""))
            End If
            Return _code.Replace("|", ".")
        End Get
    End Property

    Public Overridable Property SubCategory() As Category
        Get
            Return _subCategory
        End Get
        Set(ByVal value As Category)
            _subCategory = value
        End Set
    End Property

    Public Overridable Property CheckPublication() As Boolean
        Get
            If _checkPublication = 0 Or _checkPublication Is System.DBNull.Value Then
                Return False
            Else
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                _checkPublication = "1"
            Else
                _checkPublication = "0"
            End If
        End Set
    End Property

    Public Overridable Property File() As FileResolution
        Get
            Return _file
        End Get
        Set(ByVal value As FileResolution)
            _file = value
        End Set
    End Property

    ''' <summary>
    ''' Stringa separata da "|" rappresentante <see>Resolution.Id</see>,<see>Resolution.Location.Id</see> e <see>Resolution.ServiceNumber</see>
    ''' </summary>
    Public Overridable ReadOnly Property CalculatedLink() As String
        Get
            Return Id.ToString() & "|" & Location.Id.ToString() & "|" & ServiceNumber
        End Get
    End Property

    ''' <summary>
    ''' Data di pubblicazione web
    ''' </summary>
    Public Overridable Property WebPublicationDate() As Date?
        Get
            Return _webPublicationDate
        End Get
        Set(ByVal value As Date?)
            _webPublicationDate = value
        End Set
    End Property

    ''' <summary>
    ''' Stato di pubblicazione web
    ''' </summary>
    Public Overridable Property WebState() As Nullable(Of WebStateEnum)
        Get
            Return _webState
        End Get
        Set(ByVal value As Nullable(Of WebStateEnum))
            _webState = value
        End Set
    End Property

    Public Overridable Property WebPublicatedDocuments() As String
        Get
            Return _webPublicatedDocuments
        End Get
        Set(ByVal value As String)
            _webPublicatedDocuments = value
        End Set
    End Property

    Public Overridable Property WebRevokeDate() As Date?
        Get
            Return _webRevokeDate
        End Get
        Set(ByVal value As Date?)
            _webRevokeDate = value
        End Set
    End Property

    Public Overridable Property WebSPGuid() As String
        Get
            Return _webSPGuid
        End Get
        Set(ByVal value As String)
            _webSPGuid = value
        End Set
    End Property

    ''' <summary>Note di arretramento</summary>
    ''' <value>
    ''' Formato da:
    ''' 0 Messaggio
    ''' 1 Numero dello step che viene arretrato
    ''' 2 Nome dello step che viene arretrato
    ''' 3 Data di arretramento
    ''' </value>
    Public Overridable Property DeclineNote() As String
        Get
            Return _declineNote
        End Get
        Set(ByVal value As String)
            _declineNote = value
        End Set
    End Property

    Public Overridable Property ApprovalNote() As String
        Get
            Return _approvalNote
        End Get
        Set(ByVal value As String)
            _approvalNote = value
        End Set
    End Property

    Public Overridable Property InclusiveNumber() As String
        Get
            Return _inclusiveNumber
        End Get
        Set(value As String)
            _inclusiveNumber = value
        End Set
    End Property

    Public Overridable Property UltimaPaginaDate() As DateTimeOffset?
        Get
            Return _ultimaPaginaDate
        End Get
        Set(value As DateTimeOffset?)
            _ultimaPaginaDate = value
        End Set
    End Property

    Public Overridable Property WebPublications As IList(Of WebPublication)

    Public Overridable Property AdoptionRoleId As Short?

    Public Overridable Property ResolutionKind As ResolutionKind

    Public Overridable Property CategoryAPI As Category

#End Region

#Region "Public Properties: Collections"
    Public Overridable Property ResolutionContacts() As IList(Of ResolutionContact)
        Get
            Return _resolutionContacts
        End Get
        Set(ByVal value As IList(Of ResolutionContact))
            _resolutionContacts = value
        End Set
    End Property

    Public Overridable Property ResolutionContactsRecipients() As IList(Of ResolutionContact)
        Get
            Return _resolutionContactsRecipients
        End Get
        Set(ByVal value As IList(Of ResolutionContact))
            _resolutionContactsRecipients = value
        End Set
    End Property

    Public Overridable Property ResolutionContactProposers() As IList(Of ResolutionContact)
        Get
            Return _resolutionContactProposers
        End Get
        Set(ByVal value As IList(Of ResolutionContact))
            _resolutionContactProposers = value
        End Set
    End Property

    Public Overridable Property ResolutionContactsAssignees() As IList(Of ResolutionContact)
        Get
            Return _resolutionContactsAssignees
        End Get
        Set(ByVal value As IList(Of ResolutionContact))
            _resolutionContactsAssignees = value
        End Set
    End Property

    Public Overridable Property ResolutionContactsManagers() As IList(Of ResolutionContact)
        Get
            Return _resolutionContactsManagers
        End Get
        Set(ByVal value As IList(Of ResolutionContact))
            _resolutionContactsManagers = value
        End Set
    End Property


    Public Overridable Property ResolutionLogs() As IList(Of ResolutionLog)
        Get
            Return _resolutionLogs
        End Get
        Set(ByVal value As IList(Of ResolutionLog))
            _resolutionLogs = value
        End Set
    End Property

    Public Overridable Property ResolutionRoles As IList(Of ResolutionRole)

    Public Overridable Property ResolutionWorkflows() As IList(Of ResolutionWorkflow)
        Get
            Return _resolutionWorkflows
        End Get
        Set(ByVal value As IList(Of ResolutionWorkflow))
            _resolutionWorkflows = value
        End Set
    End Property

    Public Overridable Property ResolutionActivities As IList(Of ResolutionActivity)

    ''Collegamento ai messaggi
    Public Overridable Property Messages As IList(Of ResolutionMessage)

    'Collegamento ai settori per il proponente
    Public Overridable Property RoleProposer As Role

    ''' <summary>
    ''' Ritorna il numero totale di messaggi effettivi inviati (le e-mail vengono considerate multiple per ogni messaggio)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function MessagesCount() As Integer
        Return Messages.Sum(Function(msg) msg.Message.Emails.Count())
    End Function
#End Region

#Region "Public Properties: ASL-TO2"
    Public Overridable Property ImmediatelyExecutive() As Nullable(Of Boolean)
        Get
            Return _immediatelyExecutive
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _immediatelyExecutive = value
        End Set
    End Property

    Public Overridable Property OCSupervisoryBoard() As Nullable(Of Boolean)
        Get
            Return _ocSupervisoryBoard
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ocSupervisoryBoard = value
        End Set
    End Property

    Public Overridable Property OCRegion() As Nullable(Of Boolean)
        Get
            Return _ocRegion
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ocRegion = value
        End Set
    End Property

    Public Overridable Property OCManagement() As Nullable(Of Boolean)
        Get
            Return _ocManagement
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ocManagement = value
        End Set
    End Property

    Public Overridable Property OCOther() As Nullable(Of Boolean)
        Get
            Return _ocOther
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ocOther = value
        End Set
    End Property

    Public Overridable Property OCCorteConti() As Nullable(Of Boolean)
        Get
            Return _ocCorteConti
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ocCorteConti = value
        End Set
    End Property

    Public Overridable Property OtherDescription() As String
        Get
            Return _otherDescription
        End Get
        Set(ByVal value As String)
            _otherDescription = value
        End Set
    End Property

    Public Overridable Property ProposerWarningDate() As Date?
        Get
            Return _proposerWarningDate
        End Get
        Set(ByVal value As Date?)
            _proposerWarningDate = value
        End Set
    End Property

    Public Overridable ReadOnly Property ProposerWarningDateForamt(ByVal format As String) As String
        Get
            Return FormatDateTime(ProposerWarningDate, format)
        End Get
    End Property

    Public Overridable Property ProposerWarningUser() As String
        Get
            Return _proposerWarningUser
        End Get
        Set(ByVal value As String)
            _proposerWarningUser = value
        End Set
    End Property

    Public Overridable Property ProposerProtocolLink() As String
        Get
            Return _proposerProtocolLink
        End Get
        Set(ByVal value As String)
            _proposerProtocolLink = value
        End Set
    End Property

    Public Overridable Property PublishingProtocolLink() As String
        Get
            Return _publishingProtocolLink
        End Get
        Set(ByVal value As String)
            _publishingProtocolLink = value
        End Set
    End Property

    Public Overridable Property RegionProtocolLink() As String
        Get
            Return _regionProtocolLink
        End Get
        Set(ByVal value As String)
            _regionProtocolLink = value
        End Set
    End Property

    Public Overridable Property EffectivenessProtocolLink() As String
        Get
            Return _effectivenessProtocolLink
        End Get
        Set(ByVal value As String)
            _effectivenessProtocolLink = value
        End Set
    End Property

    Public Overridable Property SupervisoryBoardProtocolLink() As String
        Get
            Return _supervisoryBoardProtocolLink
        End Get
        Set(ByVal value As String)
            _supervisoryBoardProtocolLink = value
        End Set
    End Property

    Public Overridable Property SupervisoryBoardOpinion() As String
        Get
            Return _supervisoryBoardOpinion
        End Get
        Set(ByVal value As String)
            _supervisoryBoardOpinion = value
        End Set
    End Property

    ''' <summary>
    ''' Data di invio al collegio sindacale
    ''' </summary>
    Public Overridable Property SupervisoryBoardWarningDate() As Date?
        Get
            Return _supervisoryBoardWarningDate
        End Get
        Set(ByVal value As Date?)
            _supervisoryBoardWarningDate = value
        End Set
    End Property

    Public Overridable ReadOnly Property SupervisoryBoardWarningDateFormat(ByVal format As String) As String
        Get
            Return FormatDateTime(SupervisoryBoardWarningDate, format)
        End Get
    End Property

    Public Overridable Property SupervisoryBoardWarningUser() As String
        Get
            Return _supervisoryBoardWarningUser
        End Get
        Set(ByVal value As String)
            _supervisoryBoardWarningUser = value
        End Set
    End Property

    Public Overridable Property DGR() As String
        Get
            Return _dgr
        End Get
        Set(ByVal value As String)
            _dgr = value
        End Set
    End Property

    Public Overridable Property ManagementWarningDate() As Date?
        Get
            Return _managementWarningDate
        End Get
        Set(ByVal value As Date?)
            _managementWarningDate = value
        End Set
    End Property

    Public Overridable ReadOnly Property ManagementWarningDateFormat(ByVal format As String) As String
        Get
            Return FormatDateTime(ManagementWarningDate, format)
        End Get
    End Property

    Public Overridable Property ManagementWarningUser() As String
        Get
            Return _managementWarningUser
        End Get
        Set(ByVal value As String)
            _managementWarningUser = value
        End Set
    End Property

    Public Overridable Property ManagementProtocolLink() As String
        Get
            Return _managementProtocolLink
        End Get
        Set(ByVal value As String)
            _managementProtocolLink = value
        End Set
    End Property

    ''' <summary>
    ''' Oggetto dell'atto con gli omissis per la privacy
    ''' </summary>
    Public Overridable Property ResolutionObjectPrivacy() As String
        Get
            Return _objectPrivacy
        End Get
        Set(ByVal value As String)
            _objectPrivacy = value
        End Set
    End Property

    ''' <summary>
    ''' Organi di Controllo - Corte dei Conti
    ''' </summary>
    Public Overridable Property CorteDeiContiWarningDate() As Date?
        Get
            Return _corteDeiContiWarningDate
        End Get
        Set(ByVal value As Date?)
            _corteDeiContiWarningDate = value
        End Set
    End Property

    Public Overridable Property CorteDeiContiProtocolLink() As String
        Get
            Return _corteDeiContiProtocolLink
        End Get
        Set(ByVal value As String)
            _corteDeiContiProtocolLink = value
        End Set
    End Property

    Public Overridable Property ProposerProtocolCollaboration As Integer?

    Public Overridable Property SupervisoryBoardProtocolCollaboration As Integer?

#End Region

#Region "Public Properties: AUSL-RE"

    Public Overridable Property IsChecked() As Nullable(Of Boolean)
        Get
            Return _isChecked
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _isChecked = value
        End Set
    End Property

#End Region

    Public Overridable Property Amount As Single?
#Region " Constructor "

    Public Sub New()
        _type = New ResolutionType()
        _status = New ResolutionStatus()
        _container = New Container()
        _idLocation = New Location()
        _file = New FileResolution()
        _resolutionContacts = New List(Of ResolutionContact)
        _resolutionContactsRecipients = New List(Of ResolutionContact)
        _resolutionContactProposers = New List(Of ResolutionContact)
        _resolutionContactsAssignees = New List(Of ResolutionContact)
        _resolutionContactsManagers = New List(Of ResolutionContact)
        _resolutionLogs = New List(Of ResolutionLog)
        _ResolutionRoles = New List(Of ResolutionRole)
        _resolutionWorkflows = New List(Of ResolutionWorkflow)
        ResolutionActivities = New List(Of ResolutionActivity)
        Messages = New List(Of ResolutionMessage)
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

#Region " Methods "

    Public Overridable Sub AddProposer(ByVal contact As Contact)
        Dim rc As ResolutionContact = AddContact(Me, contact, ResolutionComunicationType.Proposer)
        _resolutionContactProposers.Add(rc)
    End Sub

    Public Overridable Sub AddManager(ByVal contact As Contact)
        Dim rc As ResolutionContact = AddContact(Me, contact, ResolutionComunicationType.Manager)
        _resolutionContactsManagers.Add(rc)
    End Sub

    Public Overridable Sub AddAssignee(ByVal contact As Contact)
        Dim rc As ResolutionContact = AddContact(Me, contact, ResolutionComunicationType.Assignee)
        _resolutionContactsAssignees.Add(rc)
    End Sub

    Public Overridable Sub AddRecipient(ByVal contact As Contact)
        Dim rc As ResolutionContact = AddContact(Me, contact, ResolutionComunicationType.Recipient)
        _resolutionContactsRecipients.Add(rc)
    End Sub

    Protected Shared Function AddContact(ByVal resolution As Resolution, ByVal contact As Contact, ByVal comunicationType As String) As ResolutionContact
        Dim rc As New ResolutionContact
        rc.Contact = contact
        rc.Id.ComunicationType = comunicationType
        rc.Resolution = resolution
        rc.UniqueIdResolution = resolution.UniqueId

        Dim insert As Boolean = True
        For Each reslContact As ResolutionContact In resolution.ResolutionContacts
            If reslContact.ComunicationType.Eq(rc.ComunicationType) AndAlso reslContact.Id.IdContact = rc.Id.IdContact Then
                insert = False
                Exit For
            End If
        Next

        If insert Then
            resolution.ResolutionContacts.Add(rc)
            Return rc
        End If
        Return Nothing
    End Function

    ''' <summary> Formatta un Collegamento a Protocollo secondo il formato format specificato [Y|N|L]). </summary>
    Public Shared Function FormatProtocolLink(ByVal link As String, ByVal format As String) As String
        If Not String.IsNullOrEmpty(link) Then
            Dim linkArray As String() = link.Split("|"c)
            Select Case format.ToUpper()
                Case "Y"
                    Return linkArray(0)
                Case "N"
                    Return linkArray(1)
                Case "L"
                    Return linkArray(2)
                Case Else
                    Return String.Format("{0}/{1:0000000}", linkArray(0), linkArray(1))
            End Select
        End If
        Return String.Empty
    End Function
#End Region

End Class