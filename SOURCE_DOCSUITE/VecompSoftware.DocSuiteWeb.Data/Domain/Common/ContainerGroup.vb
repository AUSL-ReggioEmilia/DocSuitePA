<Serializable()>
Public Class ContainerGroup
    Inherits GroupRights

#Region " Fields "

    Private _container As Container
    Private _deskRights As String
    Private _udsRights As String
    Private _fascicleRights As String

#End Region

#Region " Properties "

    ''' <summary> Contenitore </summary>
    Public Overridable Property Container() As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
        End Set
    End Property

    Public Overridable Property DeskRights As String
        Get
            Return _deskRights
        End Get
        Set(value As String)
            _deskRights = value
        End Set
    End Property

    Public Overridable Property UDSRights As String
        Get
            Return _udsRights
        End Get
        Set(value As String)
            _udsRights = value
        End Set
    End Property

    Public Overridable Property FascicleRights As String
        Get
            Return _fascicleRights
        End Get
        Set(value As String)
            _fascicleRights = value
        End Set
    End Property

    Public Overridable Property PrivacyLevel As Integer
#End Region

#Region " Constructors "

    Public Sub New()
    End Sub

#End Region

End Class