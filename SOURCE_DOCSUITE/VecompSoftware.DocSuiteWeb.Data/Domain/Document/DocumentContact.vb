<Serializable()> _
Public Class DocumentContact
    Inherits DomainObject(Of YearNumberIdCompositeKey)
    
#Region " Fields "

    Private _contact As Contact
    Private _document As Document

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

    Public Overridable Property Contact() As Contact
        Get
            Return _contact
        End Get
        Set(ByVal value As Contact)
            _contact = value
            Id.Id = value.Id
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

    Public Overridable Property Incremental() As Short

    Public Overridable Property RegistrationUser() As String

    Public Overridable Property RegistrationDate() As DateTime?

#End Region

#Region " Constructors "

    Public Sub New()
        Id = New YearNumberIdCompositeKey()
    End Sub

#End Region

End Class

