<Serializable()>
Public Class GroupRights
    Inherits AuditableDomainObject(Of Guid)

    Public Const EmptyRights As String = "00000000000000000000"

#Region " Fields "

    Private _name As String
    Protected _protocolRights As String = EmptyRights
    Protected _resolutionRights As String
    Protected _documentRights As String
    Private _group As SecurityGroups

#End Region

#Region " Properties "

    Public Overridable Property DocumentSeriesRights As String

    ''' <summary> Nome del gruppo associato al contenitore </summary>
    ''' <remarks>
    ''' Se <see>ProtocolEnv.IsSecurityGroupEnabled</see> è abilitato il nome da cercare è quello di <see>SecurityGroup.Id o GroupName</see>
    ''' </remarks>
    Public Overridable Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Overridable Property ResolutionRights() As String
        Get
            Return _resolutionRights
        End Get
        Set(ByVal value As String)
            _resolutionRights = value
        End Set
    End Property


    Public Overridable Property DocumentRights() As String
        Get
            Return _documentRights
        End Get
        Set(ByVal value As String)
            _documentRights = value
        End Set
    End Property

    Public Overridable Property ProtocolRightsString() As String
        Get
            Return _protocolRights
        End Get
        Set(ByVal value As String)
            _protocolRights = value
        End Set
    End Property

    Public Overridable Property SecurityGroup() As SecurityGroups
        Get
            Return _group
        End Get
        Set(ByVal value As SecurityGroups)
            _group = value
        End Set
    End Property

#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.Id = Guid.NewGuid()
    End Sub

#End Region

End Class
