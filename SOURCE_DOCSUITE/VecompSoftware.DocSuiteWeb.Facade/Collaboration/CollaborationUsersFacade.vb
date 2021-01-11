Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

<ComponentModel.DataObject()> _
Public Class CollaborationUsersFacade
    Inherits BaseProtocolFacade(Of CollaborationUser, Guid, NHibernateCollaborationUsersDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetByCollaboration(ByVal collaborationId As Integer, ByVal destinationFirst As Boolean?, ByVal destinationType As DestinatonType?) As IList(Of CollaborationUser)
        Return _dao.GetByCollaboration(collaborationId, destinationFirst, destinationType)
    End Function

    ''' <summary>
    ''' Ricerca tutti i destinatari diretti di una collaborazione (quelli inseriti in fase di creazione della colalborazione)
    ''' </summary>
    ''' <param name="collaborationId">Collaborazione di riferimento nella quale ricercare i contatti</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDirectWorkers(ByVal collaborationId As Integer) As IList(Of CollaborationUser)
        Return _dao.GetByCollaboration(collaborationId, Nothing, DestinatonType.P)
    End Function

    Public Function GetByAccount(username As String) As IList(Of CollaborationUser)
        Return _dao.GetByAccount(username)
    End Function

    ''' <summary>
    ''' Il metodo verifica se l'utente corrente è configurato come segreteria dei direttori nel disegno di Collaborazione (di una collaborazione specifica), in base ai settori indicati nel parametro
    ''' Metodo specifico per AUSL-PC
    ''' </summary>
    ''' <returns></returns>
    Public Function IsCollaborationDirectorSecretary(collaboration As Collaboration) As Boolean
        Dim role As Role
        For Each roleId As Short In DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Roles.Values
            If collaboration.CollaborationUsers.Any(Function(c) c.DestinationType = "S" AndAlso c.IdRole.HasValue AndAlso c.IdRole.Value = roleId) Then
                role = Factory.RoleFacade.GetById(roleId)
                If role.RoleUsers.Any(Function(r) r.Type.Eq(RoleUserType.S.ToString()) AndAlso r.Account.Eq(DocSuiteContext.Current.User.FullUserName)) Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    Public Function IsDirectorSecretary() As Boolean
        Dim role As Role
        For Each roleId As Short In DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Roles.Values
            role = Factory.RoleFacade.GetById(roleId)
            If role IsNot Nothing AndAlso role.RoleUsers.Any(Function(r) r.Type.Eq(RoleUserType.S.ToString()) AndAlso r.Account.Eq(DocSuiteContext.Current.User.FullUserName)) Then
                Return True
            End If
        Next
        Return False
    End Function

#End Region

End Class