


Public Class ProtocolDocumentDTO

#Region "Fields"

    Private _serialized As String
    Private _documentType As String
    Private _documentName As String

#End Region

#Region "Properties"
   
    Public Overridable Property Serialized() As String
        Get
            Return _serialized
        End Get
        Set(ByVal value As String)
            _serialized = value
        End Set
    End Property
    Public Overridable Property DocumentType() As String
        Get
            Return _documentType
        End Get
        Set(ByVal value As String)
            _documentType = value
        End Set
    End Property
    Public Overridable Property DocumentName() As String
        Get
            Return _documentName
        End Get
        Set(ByVal value As String)
            _documentName = value
        End Set
    End Property

#End Region
End Class
