Imports System.Collections.Generic
Imports Newtonsoft.Json

<Serializable()> _
Public Class SecurityGroups
    Inherits AuditableDomainObject(Of Integer)
    Implements IAuditable

    Public Const DEFAULT_ALL_USER As String = "Tutti gli utenti attivi nei domini configurati"

#Region " Properties "
    Public Overridable Property GroupName As String
    <JsonIgnore()>
    Public Overridable Property Parent As SecurityGroups
    Public Overridable Property FullIncrementalPath As String
    Public Overridable Property LogDescription As String
    Public Overridable Property TenantId As Guid
    Public Overridable Property IdSecurityGroupTenant As Integer
    Public Overridable Property HasAllUsers As Boolean
    <JsonIgnore()>
    Public Overridable Property SecurityUsers As IList(Of SecurityUsers)
    <JsonIgnore()>
    Public Overridable Property Children As IList(Of SecurityGroups)
    Public Overridable Property ContainerGroup As IList(Of ContainerGroup)
    Public Overridable Property RoleGroup As IList(Of RoleGroup)
#End Region

#Region " Constructors "

    Public Sub New()
        SecurityUsers = New List(Of SecurityUsers)
        ContainerGroup = New List(Of ContainerGroup)
        RoleGroup = New List(Of RoleGroup)
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

#Region " Methods  "

    ''' <summary> Conversione in stringa dell'oggetto </summary>
    ''' <remarks>ATTENZIONE: viene usato in mancanza di interfaccia in comune tra ADgroup e SecurityGroups</remarks>
    Public Overrides Function ToString() As String
        Return GroupName
    End Function

#End Region

End Class
