<Serializable()> _
Public Class ResolutionLetter

#Region "Fields"
    Private _id As Integer
    Private _number As Integer?
    Private _serviceNumber As String
    Private _year As Nullable(Of Short)
    Private _containerId As Integer
    Private _adoptionDate As Date?
    Private _publishingDate As Date?
    Private _effectivenessDate As Date?
    Private _resolutionObject As String
    Private _proposerCode As String
    Private _headingLetter As String
    Private _oCSupervisoryBoard As Boolean
    Private _oCRegion As Boolean
    Private _oCManagement As Boolean
    Private _oCOther As Boolean
    Private _otherDescription As String
#End Region

#Region "Properties"
    Public Property Id() As Integer
        Get
            Return _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property

    Public Property Number() As Integer?
        Get
            Return _number
        End Get
        Set(ByVal value As Integer?)
            _number = value
        End Set
    End Property

    Public Property ServiceNumber() As String
        Get
            Return _serviceNumber
        End Get
        Set(ByVal value As String)
            _serviceNumber = value
        End Set
    End Property

    Public Property Year() As Nullable(Of Short)
        Get
            Return _year
        End Get
        Set(ByVal value As Nullable(Of Short))
            _year = value
        End Set
    End Property

    Public Property ContainerId() As Integer
        Get
            Return _containerId
        End Get
        Set(ByVal value As Integer)
            _containerId = value
        End Set
    End Property

    Public Property AdoptionDate() As Date?
        Get
            Return _adoptionDate
        End Get
        Set(ByVal value As Date?)
            _adoptionDate = value
        End Set
    End Property

    Public Property PublishingDate() As Date?
        Get
            Return _publishingDate
        End Get
        Set(ByVal value As Date?)
            _publishingDate = value
        End Set
    End Property

    Public Property EffectivenessDate() As Date?
        Get
            Return _effectivenessDate
        End Get
        Set(ByVal value As Date?)
            _effectivenessDate = value
        End Set
    End Property

    Public Property ResolutionObject() As String
        Get
            Return _resolutionObject
        End Get
        Set(ByVal value As String)
            _resolutionObject = value
        End Set
    End Property

    Public Property ProposerCode() As String
        Get
            Return _proposerCode
        End Get
        Set(ByVal value As String)
            _proposerCode = value
        End Set
    End Property

    Public Property HeadingLetter() As String
        Get
            Return _headingLetter
        End Get
        Set(ByVal value As String)
            _headingLetter = value
        End Set
    End Property

    Public Property OCSupervisoryBoard() As Boolean
        Get
            Return _oCSupervisoryBoard
        End Get
        Set(ByVal value As Boolean)
            _oCSupervisoryBoard = value
        End Set
    End Property

    Public Property OCRegion() As Boolean
        Get
            Return _oCRegion
        End Get
        Set(ByVal value As Boolean)
            _oCRegion = value
        End Set
    End Property

    Public Property OCManagement() As Boolean
        Get
            Return _oCManagement
        End Get
        Set(ByVal value As Boolean)
            _oCManagement = value
        End Set
    End Property

    Public Property OCOther() As Boolean
        Get
            Return _oCOther
        End Get
        Set(ByVal value As Boolean)
            _oCOther = value
        End Set
    End Property

    Public Property OtherDescription() As String
        Get
            Return _otherDescription
        End Get
        Set(ByVal value As String)
            _otherDescription = value
        End Set
    End Property
#End Region

End Class
