<Serializable()> _
Public Class DocumentNote
    Inherits DomainObject(Of YearNumberIdCompositeKey)


#Region "private data"

    Private _position As String
    Private _idManager As Short
    Private _alternativeManager As String
    Private _note As String
    Private _role As Role

#End Region

#Region "Properties"
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

    Public Overridable Property Role() As Role
        Get
            Return _role
        End Get
        Set(ByVal value As Role)
            _role = value
            Id.Id = value.Id
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
    Public Overridable Property IdManager() As Short
        Get
            Return _idManager
        End Get
        Set(ByVal value As Short)
            _idManager = value
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
    Public Overridable Property Note() As String
        Get
            Return _note
        End Get
        Set(ByVal value As String)
            _note = value
        End Set
    End Property

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region

End Class

