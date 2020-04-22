<Serializable()> _
Public Class ProtocolFile

    Private _idBiblos As Integer
    Private _fileName As String
    Private _extension As String
    Private _file As Byte()

    Public Property IdBiblos() As Integer
        Get
            Return _idBiblos
        End Get
        Set(ByVal value As Integer)
            _idBiblos = value
        End Set
    End Property

    Public Property FileName() As String
        Get
            Return _fileName
        End Get
        Set(ByVal value As String)
            _fileName = value
        End Set
    End Property

    Public Property File() As Byte()
        Get
            Return _file
        End Get
        Set(ByVal value As Byte())
            _file = value
        End Set
    End Property

    Public Property Extension() As String
        Get
            Return _extension
        End Get
        Set(ByVal value As String)
            _extension = value
        End Set
    End Property

End Class
