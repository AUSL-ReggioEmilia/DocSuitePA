Imports VecompSoftware.DocSuiteWeb.Data

Public Class OChartItemWorkflowFacade
    Inherits FacadeNHibernateBase(Of OChartItemWorkflow, Guid, NHibernateOChartItemWorkflowDao)

#Region " Costructor "
    Private _userName As String
    Public Sub New(userName As String)
        MyBase.New()
        _userName = userName
    End Sub
#End Region

#Region "Methods"

#End Region

End Class
