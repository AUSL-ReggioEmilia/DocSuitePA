<Serializable()>
Public Class ResolutionHeader

#Region " Fields "

    Private _proxiedContainer As Container
    Private _proxiedLocation As Location

#End Region

#Region " Properties "

    Public Property Id As Integer
    Public Property CategoryName As String
    Public Property WorkflowType As String
    Public Property ManagedData As String
    Public Property WebDocFieldDocumentName As String
    Public Property WebDocFieldAttachmentName As String
    Public Property WebDocFieldDocumentOmissisName As String
    Public Property WebDocFieldAttachmentOmissisName As String
    Public Property ResolutionTypeCaption As String
    Public Property AdoptedDocumentName As String
    Public ReadOnly Property AdoptedDocument As Integer?
        Get
            Return IdDocument
        End Get
    End Property

    Public Property File As FileResolution
    Public Property Category As Category
#End Region

#Region " Proxied Properties "
    Public Property ContainerId As Integer?
    Public Property ContainerName As String
    Public Property ContainerPrivacy As Short?
    Public ReadOnly Property ProxiedContainer As Container
        Get
            If ContainerId.GetValueOrDefault(0) <> 0 AndAlso _proxiedContainer Is Nothing Then
                _proxiedContainer = New Container()
                With _proxiedContainer
                    .Id = ContainerId
                    .Name = ContainerName
                    .Privacy = ContainerPrivacy
                End With
            End If
            Return _proxiedContainer
        End Get
    End Property

    Public Property LocationId As Integer?
    Public Property LocationReslBiblosDSDB As String
    Public ReadOnly Property ProxiedLocation As Location
        Get
            If LocationId.GetValueOrDefault(0) <> 0 AndAlso _proxiedLocation Is Nothing Then
                _proxiedLocation = New Location()
                With _proxiedLocation
                    .Id = LocationId.Value
                    .ReslBiblosDSDB = LocationReslBiblosDSDB
                End With
            End If
            Return _proxiedLocation
        End Get
    End Property

    Public Property ReturnFromCollaboration As Integer
    Public Property ConfirmViewBy As String
    Public Property ReturnFromRetroStep As Integer

#End Region

#Region " Calculated Properties "

#End Region

#Region "Base Fields"
    Private _number As Integer?
    Private _serviceNumber As String
    Private _year As Nullable(Of Short)
    Private _object As String
    Private _proposeDate As Date?
    Private _leaveDate As Date?
    Private _effectivenessDate As Date?
    Private _responseDate As Date?
    Private _waitDate As Date?
    Private _confirmDate As Date?
    Private _warningDate As Date?
    Private _publishingDate As Date?
    Private _adoptionDate As Date?
    Private _documents As Nullable(Of Long)
    Private _registrationDate As Date?
    Private _isChecked As Boolean

    Private _alternativeProposer As String
    Private _alternativeManager As String
    Private _alternativeAssignee As String
    Private _alternativeRecipient As String

#End Region

#Region "Base Properties"


    Public Overridable Property Number() As Integer?
        Get
            Return _number
        End Get
        Set(ByVal value As Integer?)
            _number = value
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

    Public Overridable Property Year() As Nullable(Of Short)
        Get
            Return _year
        End Get
        Set(ByVal value As Nullable(Of Short))
            _year = value
        End Set
    End Property

    Public Overridable Property ResolutionObject() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
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

    Public Overridable Property LeaveDate() As Date?
        Get
            Return _leaveDate
        End Get
        Set(ByVal value As Date?)
            _leaveDate = value
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

    Public Overridable Property ResponseDate() As Date?
        Get
            Return _responseDate
        End Get
        Set(ByVal value As Date?)
            _responseDate = value
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

    Public Overridable Property ConfirmDate() As Date?
        Get
            Return _confirmDate
        End Get
        Set(ByVal value As Date?)
            _confirmDate = value
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

    Public Overridable Property PublishingDate() As Date?
        Get
            Return _publishingDate
        End Get
        Set(ByVal value As Date?)
            _publishingDate = value
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

    Public Overridable Property Documents() As Nullable(Of Long)
        Get
            Return _documents
        End Get
        Set(ByVal value As Nullable(Of Long))
            _documents = value
        End Set
    End Property

    Public Overridable Property RegistrationDate() As Date?
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As Date?)
            _registrationDate = value
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

    Public Overridable Property WorkflowStepDate As DateTimeOffset?

    Public Overridable Property Note As String

#End Region

#Region "Objects Fields"
    Private _type As ResolutionType
    Private _typeId As Nullable(Of Short)
    Private _status As ResolutionStatus
    Private _statusId As Nullable(Of Short)
    Private _controllerStatus As ControllerStatusResolution
    Private _controllerStatusAcronym As String

    Private _idFrontalinoRitiro As Integer?
#End Region

#Region "Objects Properties"
    Public Overridable Property ControllerStatus() As ControllerStatusResolution
        Get
            Return _controllerStatus
        End Get
        Set(ByVal value As ControllerStatusResolution)
            _controllerStatus = value
        End Set
    End Property

    Public Overridable Property IsChecked() As Boolean
        Get
            Return _isChecked
        End Get
        Set(ByVal value As Boolean)
            _isChecked = value
        End Set
    End Property

    Public Overridable Property ControllerStatusAcronym() As String
        Get
            Return _controllerStatusAcronym
        End Get
        Set(ByVal value As String)
            _controllerStatusAcronym = value
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

    Public Overridable Property TypeId() As Nullable(Of Short)
        Get
            Return _typeId
        End Get
        Set(ByVal value As Nullable(Of Short))
            _typeId = value
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

    Public Overridable Property StatusId() As Nullable(Of Short)
        Get
            Return _statusId
        End Get
        Set(ByVal value As Nullable(Of Short))
            _statusId = value
        End Set
    End Property





    Public Overridable Property IdFrontalinoRitiro() As Integer?
        Get
            Return _idFrontalinoRitiro
        End Get
        Set(ByVal value As Integer?)
            _idFrontalinoRitiro = value
        End Set
    End Property

#End Region

#Region "ASL-TO2 Fields"
    Private _ocSupervisoryBoard As Nullable(Of Boolean)
    Private _ocRegion As Nullable(Of Boolean)
    Private _ocManagement As Nullable(Of Boolean)
    Private _ocCorteConti As Nullable(Of Boolean)
    Private _ocOther As Nullable(Of Boolean)
    Private _supervisoryBoardWarningDate As Date?
    Private _proposerCode As String
    Private _idResolutionFile As Integer?

#End Region

#Region "ASL-TO2 Properties"

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

    Public Overridable Property OCCorteConti() As Nullable(Of Boolean)
        Get
            Return _ocCorteConti
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ocCorteConti = value
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

    Public Overridable Property SupervisoryBoardWarningDate() As Date?
        Get
            Return _supervisoryBoardWarningDate
        End Get
        Set(ByVal value As Date?)
            _supervisoryBoardWarningDate = value
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

    Public Overridable Property IdResolutionFile() As Integer?
        Get
            Return _idResolutionFile
        End Get
        Set(ByVal value As Integer?)
            _idResolutionFile = value
        End Set
    End Property

    Public Overridable Property CurrentUserTakeCharge As String
#End Region

#Region "AUSL-PC Fields"
    Private _inclusiveNumber As String
    Private _declineNote As String
#End Region

#Region "AUSL-PC Properties"
    Public Overridable Property InclusiveNumber() As String
        Get
            Return _inclusiveNumber
        End Get
        Set(ByVal value As String)
            _inclusiveNumber = value
        End Set
    End Property

    Public Overridable Property DeclineNote As String
        Get
            Return _declineNote
        End Get
        Set(value As String)
            _declineNote = value
        End Set
    End Property
#End Region

#Region "WS Fields"
    Private _location As Location
    Private _idDocument As Integer?
    Private _idAttachments As Integer?
    Private _subCategory As Category
#End Region

#Region "WS Properties"
    Public Overridable Property Location() As Location
        Get
            Return _location
        End Get
        Set(ByVal value As Location)
            _location = value
        End Set
    End Property

    Public Overridable Property IdDocument() As Integer?
        Get
            Return _idDocument
        End Get
        Set(ByVal value As Integer?)
            _idDocument = value
        End Set
    End Property

    Public Overridable Property IdAttachments() As Integer?
        Get
            Return _idAttachments
        End Get
        Set(ByVal value As Integer?)
            _idAttachments = value
        End Set
    End Property

    Public Overridable Property SubCategory() As Category
        Get
            Return _subCategory
        End Get
        Set(ByVal value As Category)
            _subCategory = value
        End Set
    End Property
#End Region

    Public Overridable ReadOnly Property CategoryCode() As String
        Get
            Dim _code As String = ""
            If _subCategory IsNot Nothing Then
                _code = _subCategory.FullCode.Replace("0", "")
            Else
                _code = Category.Code.ToString()
            End If
            Return _code.Replace("|", ".")
        End Get
    End Property

End Class
