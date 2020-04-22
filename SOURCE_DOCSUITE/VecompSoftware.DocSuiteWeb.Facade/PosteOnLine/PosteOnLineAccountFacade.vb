Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports System.Linq

<DataObject()>
Public Class PosteOnLineAccountFacade
    Inherits BaseProtocolFacade(Of POLAccount, Int32, NHibernatePosteOnlineAccountDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetUserAccounts() As IList(Of POLAccount)
        Dim accounts As IList(Of POLAccount) = New List(Of POLAccount)
        If CommonShared.HasGroupAdministratorRight Then
            accounts = GetAll()
        Else
            Dim roles As IList(Of Role) = New RoleFacade().GetUserRoles(DSWEnvironment.Protocol, 1, Nothing)
            accounts = GetByRoles(roles)
        End If
        Return accounts.OrderBy(Function(o) o.Name).ToList()
    End Function

    Public Function GetByRoles(ByRef roles As IList(Of Role)) As IList(Of POLAccount)
        Return _dao.GetByRoles(roles)
    End Function

End Class

