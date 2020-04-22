<Serializable()> _
Public Class DocumentObject
    Inherits DomainObject(Of YearNumberIncrCompositeKey)
    Implements IAuditable

#Region " Fields "
    Private _incrementalFolder As Short
    Private _validIncremental As Nullable(Of Short)
    Private _ordinalPosition As Short
    Private _step As Short
    Private _subStep As Short
    Private _idObjectType As String
    Private _description As String
    Private _documentDate As Date?
    Private _object As String
    Private _reason As String
    Private _note As String
    Private _idBiblos As Integer
    Private _link As String
    Private _idObjectStatus As String
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _document As Document
    Private _documentVersionings As IList(Of DocumentVersioning)

#End Region

#Region " Properties "

    Public Overridable Property Year() As Short
        Get
            Return Id.Year
        End Get
        Set(ByVal value As Short)
            Id.Year = value
        End Set
    End Property
    Public Overridable Property Number() As Integer
        Get
            Return Id.Number
        End Get
        Set(ByVal value As Integer)
            Id.Number = value
        End Set
    End Property

    Public Overridable Property Incremental() As Short
        Get
            Return Id.Incremental
        End Get
        Set(ByVal value As Short)
            Id.Incremental = value
        End Set
    End Property
    Public Overridable Property Document() As Document
        Get
            Return _document
        End Get
        Set(ByVal value As Document)
            _document = value
            Id.Year = value.Year
            Id.Number = value.Number
        End Set
    End Property
    Public Overridable Property IncrementalFolder() As Short
        Get
            Return _incrementalFolder
        End Get
        Set(ByVal value As Short)
            _incrementalFolder = value
        End Set
    End Property
    Public Overridable Property ValidIncremental() As Nullable(Of Short)
        Get
            Return _validIncremental
        End Get
        Set(ByVal value As Nullable(Of Short))
            If value.HasValue Then
                _validIncremental = value
            End If
        End Set
    End Property
    Public Overridable Property OrdinalPosition() As Short
        Get
            Return _ordinalPosition
        End Get
        Set(ByVal value As Short)
            _ordinalPosition = value
        End Set
    End Property
    Public Overridable Property DocStep() As Short
        Get
            Return _step
        End Get
        Set(ByVal value As Short)
            _step = value
        End Set
    End Property
    Public Overridable Property SubStep() As Short
        Get
            Return _subStep
        End Get
        Set(ByVal value As Short)
            _subStep = value
        End Set
    End Property
    Public Overridable Property idObjectType() As String
        Get
            Return _idObjectType
        End Get
        Set(ByVal value As String)
            _idObjectType = value
        End Set
    End Property

    ''' <summary>Nome del file</summary>
    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property DocumentDate() As Date?
        Get
            Return _documentDate
        End Get
        Set(ByVal value As Date?)
            _documentDate = value
        End Set
    End Property
    Public Overridable Property DocObject() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property
    Public Overridable Property Reason() As String
        Get
            Return _reason
        End Get
        Set(ByVal value As String)
            _reason = value
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
    Public Overridable Property idBiblos() As Integer
        Get
            Return _idBiblos
        End Get
        Set(ByVal value As Integer)
            _idBiblos = value
        End Set
    End Property
    Public Overridable Property Link() As String
        Get
            Return _link
        End Get
        Set(ByVal value As String)
            _link = value
        End Set
    End Property
    Public Overridable Property idObjectStatus() As String
        Get
            Return _idObjectStatus
        End Get
        Set(ByVal value As String)
            _idObjectStatus = value
        End Set
    End Property
    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property
    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property DocumentVersionings() As IList(Of DocumentVersioning)
        Get
            Return _documentVersionings
        End Get
        Set(ByVal value As IList(Of DocumentVersioning))
            _documentVersionings = value
        End Set
    End Property

#End Region

#Region "Descriptions"
    Public Overridable ReadOnly Property StepDescription() As String
        Get
            Return DocStep & "<BR>" & If(SubStep = 0, "", "." & SubStep)
        End Get
    End Property

    Public Overridable ReadOnly Property StepLinearDescription() As String
        Get
            Return DocStep & "" & If(SubStep = 0, "", "." & SubStep)
        End Get
    End Property

    Public Overridable ReadOnly Property DocumentDateDescription() As String
        Get
            Return String.Format("{0:dd/MM/yyyy}", DocumentDate)
        End Get
    End Property

    Public Overridable ReadOnly Property RegistrationUserDateDescription() As String
        Get
            Return RegistrationUser & " " & String.Format("{0:dd/MM/yyyy}", RegistrationDate)
        End Get
    End Property

    Public Overridable ReadOnly Property LastChangeUserDateDescription() As String
        Get
            Return LastChangedUser & " " & String.Format("{0:dd/MM/yyyy}", LastChangedDate)
        End Get
    End Property

#End Region

#Region " Constructors "
    Public Sub New()
        Id = New YearNumberIncrCompositeKey()
    End Sub
#End Region

End Class

