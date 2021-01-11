Public Class DocumentObjectHeader

#Region "Fields"
    Private _idBiblos As Integer
    Private _step As String = String.Empty
    Private _folder As String = String.Empty
    Private _version As Integer
#End Region

#Region "Properties"
    Public Property IdBiblos() As Integer
        Get
            Return _idBiblos
        End Get
        Set(ByVal value As Integer)
            _idBiblos = value
        End Set
    End Property

    Public Property StepDescription() As String
        Get
            Return _step
        End Get
        Set(ByVal value As String)
            _step = value
        End Set
    End Property

    Public Property Folder() As String
        Get
            Return _folder
        End Get
        Set(ByVal value As String)
            _folder = value
        End Set
    End Property

    Public Property Version() As Integer
        Get
            Return _version
        End Get
        Set(ByVal value As Integer)
            _version = value
        End Set
    End Property
#End Region

End Class
