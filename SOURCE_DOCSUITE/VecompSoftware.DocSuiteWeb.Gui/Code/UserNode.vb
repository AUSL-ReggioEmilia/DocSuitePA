Public Class UserNode

#Region " Fields "
    Private _id As String
    Private _text As String
    Private _idParent As String
#End Region

#Region " Properties "
    Public Property Id() As String
        Get
            Return _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property

    Public Property Text() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            _text = value
        End Set
    End Property

    Public Property IdParent() As String
        Get
            Return _idParent
        End Get
        Set(ByVal value As String)
            _idParent = value
        End Set
    End Property
#End Region

#Region " Constructors "
    Public Sub New(ByVal Id As String, ByVal Text As String, ByVal IdParent As String)
        _id = Id
        _text = Text
        _idParent = IdParent
    End Sub
#End Region

End Class