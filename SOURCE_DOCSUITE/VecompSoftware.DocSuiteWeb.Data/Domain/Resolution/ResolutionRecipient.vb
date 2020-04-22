<Serializable()> _
Public Class ResolutionRecipient
    Inherits DomainObject(Of ResolutionRecipientCompositeKey)

#Region "private data"
    Private _resolution As Resolution
    Private _resolutionRecipient As Recipient
#End Region

#Region "Properties"
    Public Overridable Property Resolution() As Resolution
        Get
            Return _resolution
        End Get
        Set(ByVal value As Resolution)
            _resolution = value
        End Set
    End Property

    Public Overridable Property Recipient() As Recipient
        Get
            Return _resolutionRecipient
        End Get
        Set(ByVal value As Recipient)
            _resolutionRecipient = value
        End Set
    End Property
#End Region

#Region "Ctor/init"
    Public Sub New()
        Id = New ResolutionRecipientCompositeKey()
    End Sub
#End Region

End Class

