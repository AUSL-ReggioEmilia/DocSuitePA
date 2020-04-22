Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq

Public Class DocumentSeriesItemRoleFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesItemRole, Integer, NHibernateDocumentSeriesItemRoleDao)

    Public Function AddRole(item As DocumentSeriesItem, role As Role, linkType As DocumentSeriesItemRoleLinkType, Optional needTransaction As Boolean = True) As DocumentSeriesItemRole
        'Verifico che non esista già il settore(e tipo) per la specifica serie documentale
        Dim existingRole As IList(Of DocumentSeriesItemRole) = _dao.GetItemRoles(item, role, linkType)
        If Not existingRole.IsNullOrEmpty() Then Return existingRole.Single()

        Dim tor As New DocumentSeriesItemRole()
        tor.UniqueIdDocumentSeriesItem = item.UniqueId
        tor.Item = item
        tor.Role = role
        tor.LinkType = linkType
        item.DocumentSeriesItemRoles.Add(tor)
        Save(tor, _dbName, needTransaction)
        Return tor
    End Function

    Public Function AddOwnerRole(item As DocumentSeriesItem, role As Role, Optional needTransaction As Boolean = True) As DocumentSeriesItemRole
        Return AddRole(item, role, DocumentSeriesItemRoleLinkType.Owner, needTransaction)
    End Function

    Public Function AddAuthorizedRole(item As DocumentSeriesItem, role As Role) As DocumentSeriesItemRole
        Return AddRole(item, role, DocumentSeriesItemRoleLinkType.Authorization)
    End Function

    Public Function AddKnowledgeRole(item As DocumentSeriesItem, role As Role) As DocumentSeriesItemRole
        Return AddRole(item, role, DocumentSeriesItemRoleLinkType.Knowledge)
    End Function

    Public Function GetRoles(item As DocumentSeriesItem, linkType As DocumentSeriesItemRoleLinkType) As IList(Of Role)
        Return _dao.GetRoles(item, linkType)
    End Function

    Public Function GetOwnerRoles(item As DocumentSeriesItem) As IList(Of Role)
        Return _dao.GetRoles(item, DocumentSeriesItemRoleLinkType.Owner)
    End Function

    Public Function GetAuthorizedRoles(item As DocumentSeriesItem) As IList(Of Role)
        Return _dao.GetRoles(item, DocumentSeriesItemRoleLinkType.Authorization)
    End Function

    Public Function GetKnowledgeRoles(item As DocumentSeriesItem) As IList(Of Role)
        Return _dao.GetRoles(item, DocumentSeriesItemRoleLinkType.Knowledge)
    End Function

    Public Sub RemoveOwnerRole(item As DocumentSeriesItem, role As Role)
        RemoveRole(item, role, DocumentSeriesItemRoleLinkType.Owner)
    End Sub

    Public Sub RemoveAuthorizationRole(item As DocumentSeriesItem, role As Role)
        RemoveRole(item, role, DocumentSeriesItemRoleLinkType.Authorization)
    End Sub

    Public Sub RemoveKnowledgeRole(item As DocumentSeriesItem, role As Role)
        RemoveRole(item, role, DocumentSeriesItemRoleLinkType.Knowledge)
    End Sub

    Public Sub RemoveRole(item As DocumentSeriesItem, role As Role, linkType As DocumentSeriesItemRoleLinkType?)
        Dim items As IList(Of DocumentSeriesItemRole) = _dao.GetItemRoles(item, role, linkType)
        If Not items.IsNullOrEmpty Then
            For Each itemRole As DocumentSeriesItemRole In items
                Delete(itemRole)
            Next
        End If
    End Sub

    Public Function GetRoleLabelsDictionary(dsiIdentifiers As List(Of Integer)) As IDictionary(Of Integer, List(Of String))
        Dim list As IList(Of DocumentSeriesItemRole) = _dao.GetOwnersByItems(dsiIdentifiers)
        Dim result As Dictionary(Of Integer, List(Of String)) = list.GroupBy(Function(dsir) dsir.Item.Id) _
                                                                .ToDictionary(Of Integer, List(Of String))(Function(keys) keys.First().Item.Id, _
                                                                                                           Function(values) values.Where(Function(dsir) dsir.Item.Id = values.First().Item.Id) _
                                                                                                              .Select(Function(dsir) dsir.Role.Name).ToList())
        Return result
    End Function

End Class
