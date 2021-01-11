
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ServiceLogFacade
    Inherits BaseProtocolFacade(Of ServiceLog, Integer, NHibernateServiceLogDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class