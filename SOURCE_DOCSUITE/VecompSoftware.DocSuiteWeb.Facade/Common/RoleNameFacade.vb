Imports VecompSoftware.DocSuiteWeb.Data

Public Class RoleNameFacade
    Inherits CommonFacade(Of RoleName, Integer, NHibernateRoleNameDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#End Region

#Region " Methods "




    Public Function GetRoleNameByIdRole(ByVal IdRole As Integer) As RoleName
        Return _dao.GetRoleNameByIdRole(IdRole)
    End Function

    Public Function GetRoleNameByIdRoleDate(ByVal IdRole As Integer) As RoleName
        Return _dao.GetRoleNameByIdRoleDate(IdRole)
    End Function

    Public Function GetRoleNamesByIdRole(ByVal IdRole As Integer) As IList(Of RoleName)
        Return _dao.GetRoleNamesByIdRole(IdRole)
    End Function

    Public Function GetRoleNamesByIncrementalForDate(ByVal IdRole As Integer) As IList(Of RoleName)
        Return _dao.GetRoleNamesByIncrementalForDate(IdRole)
    End Function

    Public Function GetRoleNamesByValidDate(ByVal IdRole As Integer, ByVal SelectedDate As DateTime, tenantId As String) As RoleName
        Return _dao.GetRoleNamesByValidDate(IdRole, SelectedDate, tenantId)
    End Function

    Public Function GetRoleNamesHistoryByValidDate(ByVal IdRole As Integer, ByVal SelectedDate As DateTime) As RoleName
        Return _dao.GetRoleNamesHistoryByValidDate(IdRole, SelectedDate)
    End Function

    Public Function GetRoleNamesOlder(ByVal IdRole As Integer) As RoleName
        Return _dao.GetRoleNamesOlder(IdRole)
    End Function

#End Region

End Class
