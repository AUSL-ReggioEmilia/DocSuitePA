<Serializable()> _
Public Class ProtocolType
    Inherits DomainObject(Of Int32)

#Region "private data"
    Private _description As String

    Public Const INCOMING As Integer = -1
    Public Const OUTGOING As Integer = 1
    Public Const INCOMING_OUTGOING As Integer = 0
#End Region

#Region "Properties"
    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable ReadOnly Property ShortDescription() As String
        Get
            Select Case Id
                Case -1
                    Return "I"
                Case 0
                    Return "I/U"
                Case 1
                    Return "U"
                Case Else
                    Return String.Empty
            End Select
        End Get
    End Property

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub

    Public Sub New(ByVal idType As Integer, ByVal descriptionType As String)
        Id = idType
        Description = descriptionType
    End Sub
#End Region



End Class