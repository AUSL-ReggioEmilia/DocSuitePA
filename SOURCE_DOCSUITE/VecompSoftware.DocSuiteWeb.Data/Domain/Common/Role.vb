Imports Newtonsoft.Json
Imports System.Linq

<Serializable()> _
Public Class Role
    Inherits DomainObject(Of Integer)
    Implements IAuditable, ISupportLogicDelete, ISupportRangeDelete, ISupportChanged

#Region " Properties "

    Public Overridable Property Name As String

    Public Overridable Property UriSharepoint As String

    Public Overridable Property IsActive As Short Implements ISupportLogicDelete.IsActive

    Public Overridable Property ActiveFrom As Date? Implements ISupportRangeDelete.ActiveFrom

    Public Overridable Property ActiveTo As Date? Implements ISupportRangeDelete.ActiveTo

    Public Overridable Property Collapsed As Short

    Public Overridable Property EMailAddress As String

    Public Overridable Property TenantId As Guid

    Public Overridable Property IdRoleTenant As Short
    <JsonIgnore()>
    Public Overridable Property Father As Role

    Public Overridable Property FullIncrementalPath As String
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    <JsonIgnore()>
    Public Overridable Property ServiceCode As String
    <JsonIgnore()>
    Public Overridable Property HasChildren As Integer
    <JsonIgnore()>
    Public Overridable Property RoleGroups As IList(Of RoleGroup)
    <JsonIgnore()>
    Public Overridable Property Protocols As IList(Of ProtocolRole)
    <JsonIgnore()>
    Public Overridable Property TenantRoles As IList(Of TenantRole)
    <JsonIgnore()>
    Public Overridable Property RoleUsers As IList(Of RoleUser)
    <JsonIgnore()>
    Public Overridable Property Mailboxes As IList(Of PECMailBox)
    <JsonIgnore()>
    Public Overridable Property RoleNames As IList(Of RoleName)
    <JsonIgnore()>
    Public Overridable Property Children As IList(Of Role)
    <JsonIgnore()>
    Public Overridable Property IsChanged As Short Implements ISupportChanged.IsChanged
    <JsonIgnore()>
    Public Overridable ReadOnly Property Level As Short
        Get
            Dim array As String() = Split(FullIncrementalPath, "|")
            Return CType((array.Length - 1), Short)
        End Get
    End Property
    <JsonIgnore()>
    Public Overridable ReadOnly Property FullIncrementalPathArray As String()
        Get
            Return Split(FullIncrementalPath, "|")
        End Get
    End Property

    <JsonIgnore()>
    Public Overridable ReadOnly Property FullDescription As String
        Get
            Return String.Format("{0} {1}", Me.Name, If(String.IsNullOrEmpty(Me.ServiceCode), String.Empty, String.Format("({0})", Me.ServiceCode)))
        End Get
    End Property

#End Region

#Region " Methods "

    ''' <summary> Ritorna il primo <see>RoleGroup</see> col nome richiesto </summary>
    ''' <param name="groupName">Nome del gruppo</param>
    ''' <returns><see>Nothing</see> se non trovato</returns>
    ''' <remarks>Non è detto che il nome del gruppo corrisponda al nome del gruppo specificato dall'IdGroup</remarks>
    Public Overridable Function GetRoleGroup(ByVal groupName As String) As RoleGroup
        Return RoleGroups.FirstOrDefault(Function(f) String.Compare(f.Name, groupName, StringComparison.InvariantCultureIgnoreCase) = 0)
    End Function

    ''' <summary> Ritorna l'elenco delle caselle PEC abilitate effettivamente per l'invio di PEC </summary>
    Public Overridable Function GetSendEnabledMailBoxes() As List(Of PECMailBox)
        Dim sendEnabledMailboxes As List(Of PECMailBox) = New List(Of PECMailBox)
        For Each mailbox As PECMailBox In Mailboxes
            If (mailbox.IsSendingEnabled()) Then sendEnabledMailboxes.Add(mailbox)
        Next
        Return sendEnabledMailboxes
    End Function

    Public Overridable Function IsActiveRange() As Boolean Implements ISupportRangeDelete.IsActiveRange
        Return (Not ActiveFrom.HasValue AndAlso Not ActiveTo.HasValue) OrElse (ActiveFrom.Value < DateTime.Now AndAlso DateTime.Now < ActiveTo.Value)
    End Function

#End Region

#Region " Constructor "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class
