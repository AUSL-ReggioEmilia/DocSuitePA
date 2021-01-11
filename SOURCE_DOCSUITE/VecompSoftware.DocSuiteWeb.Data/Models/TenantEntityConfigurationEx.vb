Imports System.Linq
Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers.ExtensionMethods

Public Module TenantEntityConfigurationEx

    <Extension()>
    Public Function GetFromType(Of T)(source As IDictionary(Of String, TenantEntityConfiguration)) As TenantEntityConfiguration
        Dim entityName As String = GetType(T).Name
        Return source.Where(Function(x) x.Key.Eq(entityName)).Select(Function(s) s.Value).SingleOrDefault()
    End Function
End Module
