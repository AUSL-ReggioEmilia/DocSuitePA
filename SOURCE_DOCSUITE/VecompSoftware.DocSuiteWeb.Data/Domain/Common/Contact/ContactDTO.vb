<Serializable()> _
Public Class ContactDTO
    Inherits DomainObject(Of Int32)

    Public Enum ContactType
        Manual = 0
        Address = 1
    End Enum

#Region " Fields "

    Private _id As YearNumberIdCompositeKey
    Private _contact As Contact
    Private _copiaConoscenza As Boolean
    Private _type As ContactType
    Private _part As ContactTypeEnum
    Private _isLocked As Boolean

#End Region

#Region " Properties "

    Public Overridable Property IdManualContact As Guid?

    Public Overridable Property Contact() As Contact
        Get
            Return _contact
        End Get
        Set(ByVal value As Contact)
            _contact = value
        End Set
    End Property

    Public Overridable Property IsCopiaConoscenza() As Boolean
        Get
            Return _copiaConoscenza
        End Get
        Set(ByVal value As Boolean)
            _copiaConoscenza = value
        End Set
    End Property

    Public Overridable Property Type() As ContactType
        Get
            Return _type
        End Get
        Set(ByVal value As ContactType)
            _type = value
        End Set
    End Property

    ''' <summary> E chi lo sa? </summary>
    ''' <remarks>
    ''' TODO: che è sta cosa? verificare se si può eliminare
    ''' </remarks>
    Public Overridable Property ContactPart() As ContactTypeEnum
        Get
            Return _type
        End Get
        Set(ByVal value As ContactTypeEnum)
            _type = value
        End Set
    End Property

    Public Overridable Property IsLocked() As Boolean
        Get
            Return _isLocked
        End Get
        Set(value As Boolean)
            _isLocked = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
    End Sub

    Public Sub New(ByVal contact As Contact, ByVal idManualContact As Guid)
        Me.Contact = contact
        Me.Type = ContactType.Manual
        Me.IdManualContact = idManualContact
    End Sub

    Public Sub New(ByVal contact As Contact, ByVal type As ContactType, ByVal idManualContact As Guid)
        Me.Contact = contact
        Me.Id = contact.Id
        Me.Type = type
        Me.IdManualContact = idManualContact
    End Sub

    Public Sub New(ByVal contact As Contact, ByVal type As ContactType)
        Me.Contact = contact
        Me.Id = contact.Id
        Me.Type = type
    End Sub

#End Region

End Class
