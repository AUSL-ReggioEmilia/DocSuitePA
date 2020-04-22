Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

Public Class OChartItemRoleFacade
    Inherits FacadeNHibernateBase(Of OChartItemRole, Guid, NHibernateOChartItemRoleDao)

#Region " Costructor "
    Private _userName As String
    Public Sub New(userName As String)
        MyBase.New()
        _userName = userName
    End Sub
#End Region

    Public Function GetVariations(source As OChart, destination As OChart) As IList(Of OChartItemRole)
        Return _dao.GetVariations(source, destination)
    End Function

End Class
