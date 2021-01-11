Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ResolutionRoleTypeFacade
    Inherits BaseResolutionFacade(Of ResolutionRoleType, Integer, NHibernateResolutionRoleTypeDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetEnabled() As IList(Of ResolutionRoleType)
        Return _dao.GetEnables()
    End Function

End Class
