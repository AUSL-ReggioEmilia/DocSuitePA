<Serializable()> _
Public Class RoleGroup
    Inherits GroupRights

#Region " Fields "

    Private _role As Role
    Private _fascicleRights As String

#End Region

#Region " Properties "

    Public Overridable Property Role() As Role
        Get
            Return _role
        End Get
        Set(ByVal value As Role)
            _role = value
        End Set
    End Property

    Public Overridable Property ProtocolRights() As RoleProtocolRights
        Get
            Return New RoleProtocolRights(_protocolRights)
        End Get
        Set(ByVal value As RoleProtocolRights)
            _protocolRights = value.ToString()
        End Set
    End Property

    Public Overridable ReadOnly Property ProtocolRightString As String
        Get
            Return _protocolRights
        End Get
    End Property

    Public Overridable Property FascicleRights As String
        Get
            Return _fascicleRights
        End Get
        Set(value As String)
            _fascicleRights = value
        End Set
    End Property
#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
        ProtocolRights = New RoleProtocolRights()
    End Sub

#End Region

End Class
