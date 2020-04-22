<Serializable()> _
Public Class ProtocolFull

    Private _protocolMetaData As Protocol
    Private _principalDocument As ProtocolFile
    Private _attachments As ProtocolFile()

    Public Property ProtocolMetaData() As Protocol
        Get
            Return _protocolMetaData
        End Get
        Set(ByVal value As Protocol)
            _protocolMetaData = value
        End Set
    End Property

    Public Property PrincipalDocument() As ProtocolFile
        Get
            Return _principalDocument
        End Get
        Set(ByVal value As ProtocolFile)
            _principalDocument = value
        End Set
    End Property

    Public Property Attachments() As ProtocolFile()
        Get
            Return _attachments
        End Get
        Set(ByVal value As ProtocolFile())
            _attachments = value
        End Set
    End Property

End Class
