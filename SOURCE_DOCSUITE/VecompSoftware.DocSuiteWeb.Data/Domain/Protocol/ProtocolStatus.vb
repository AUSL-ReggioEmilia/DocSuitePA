<Serializable()> _
Public Class ProtocolStatus
    Inherits DomainObject(Of String)

#Region " Fields "

    Private _incremental As Integer?
    Private _description As String
    Private _foreColor As String
    Private _backColor As String

#End Region

#Region " Properties "

    Public Overridable Property Incremental() As Integer?
        Get
            Return _incremental
        End Get
        Set(ByVal value As Integer?)
            _incremental = value
        End Set
    End Property

    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property ForeColor() As String
        Get
            Return _foreColor
        End Get
        Set(ByVal value As String)
            _foreColor = value
        End Set
    End Property

    Public Overridable Property BackColor() As String
        Get
            Return _backColor
        End Get
        Set(ByVal value As String)
            _backColor = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()

    End Sub

#End Region

End Class


