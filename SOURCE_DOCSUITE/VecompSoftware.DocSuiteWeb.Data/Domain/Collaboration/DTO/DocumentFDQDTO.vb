
<Serializable()> _
Public Class DocumentFDQDTO

#Region "Fields"
    Private _collaboration As Integer
    Private _idDocument As Integer?
    Private _documentName As String
    Private _incremental As Nullable(Of Short)
    Private _documentType As String
    Private _object As String
    Private _idLocation As Integer?
#End Region

#Region "Pproperties"
    Public Property Collaboration() As Integer
        Get
            Return _collaboration
        End Get
        Set(ByVal value As Integer)
            _collaboration = value
        End Set
    End Property

    Public Property IdDocument() As Integer?
        Get
            Return _idDocument
        End Get
        Set(ByVal value As Integer?)
            _idDocument = value
        End Set
    End Property

    Public Property DocumentName() As String
        Get
            Return _documentName
        End Get
        Set(ByVal value As String)
            _documentName = value
        End Set
    End Property

    Public Property Incremental() As Nullable(Of Short)
        Get
            Return _incremental
        End Get
        Set(ByVal value As Nullable(Of Short))
            _incremental = value
        End Set
    End Property

    Public Property DocumentType() As String
        Get
            Return _documentType
        End Get
        Set(ByVal value As String)
            _documentType = value
        End Set
    End Property

    Public Property [Object]() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property

    Public Property IdLocation() As Integer?
        Get
            Return _idLocation
        End Get
        Set(ByVal value As Integer?)
            _idLocation = value
        End Set
    End Property

    Public ReadOnly Property FileName() As String
        Get
            Return CreateDocumentName()
        End Get
    End Property
#End Region

#Region "Functions"
    Public Overridable Function CreateDocumentName() As String
        Return Collaboration & "§" & Incremental.Value & "§" & DocumentName
    End Function
#End Region

#Region "Constructors"
    Public Sub New()
    End Sub
#End Region

End Class
