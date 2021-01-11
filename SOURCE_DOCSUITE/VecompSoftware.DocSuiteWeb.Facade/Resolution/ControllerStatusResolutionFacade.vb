
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ControllerStatusResolutionFacade
    Inherits BaseResolutionFacade(Of ControllerStatusResolution, Short, NHibernateControllerStatusResolutionDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class