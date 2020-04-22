Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class ProtocolRejectedRoleFacade
    Inherits BaseProtocolFacade(Of ProtocolRejectedRole, String, NHibernateProtocolRejectedRoleDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub FixRejectedRoles(rejectedRoles As IList(Of ProtocolRejectedRole))
        For Each item As ProtocolRejectedRole In rejectedRoles
            item.Status = ProtocolRoleStatus.Fixed
            UpdateNeedAuditable(item, True)
        Next
    End Sub

    Public Sub DeactivatedRejectedRole(rejectedRole As ProtocolRejectedRole)
        rejectedRole.Status = ProtocolRoleStatus.Deactivated
        UpdateNeedAuditable(rejectedRole, True)
    End Sub

    Public Sub RejectProtocols(protocol As Protocol, roleIdsToRemove As IList(Of Integer))
        For Each id As Integer In roleIdsToRemove
            Dim protocolRole As ProtocolRole = protocol.Roles.Where(Function(r) r.Role.Id.Equals(id)).FirstOrDefault()
            If protocolRole Is Nothing Then
                Exit Sub
            End If
            Dim rejectedRole As ProtocolRejectedRole = New ProtocolRejectedRole()
            rejectedRole.Year = protocolRole.Year
            rejectedRole.Number = protocolRole.Number
            rejectedRole.UniqueId = protocolRole.UniqueId
            rejectedRole.Role = protocolRole.Role
            rejectedRole.Protocol = protocolRole.Protocol
            rejectedRole.UniqueIdProtocol = protocolRole.UniqueIdProtocol
            rejectedRole.Type = protocolRole.Type
            rejectedRole.NoteType = protocolRole.NoteType
            rejectedRole.DistributionType = protocolRole.DistributionType
            rejectedRole.Note = protocolRole.Note
            rejectedRole.Status = ProtocolRoleStatus.Refused
            rejectedRole.Rights = protocolRole.Rights
            'Questo controllo verifica che il settore che si sta rifiutando non sia già presente nella ProtocolRejectedRoles.
            'Se si attiva questo controllo, bisogna fare in modo poi che non sia più possibile autorizzare il protocollo ai settori rifiutati.
            'If _dao.GetByRole(protocolRole.Role.Id, protocolRole.Year, protocolRole.Number) Is Nothing Then
            '    Save(rejectedRole)
            'End If
            Save(rejectedRole)
        Next
        Factory.ProtocolFacade.RemoveRoleAuthorizations(protocol, roleIdsToRemove)
        'Invio comando di modifica alle web api
        Factory.ProtocolFacade.SendUpdateProtocolCommand(protocol)
    End Sub


End Class