Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ZebraPrinterFacade
    Inherits BaseProtocolFacade(Of ZebraPrinter, Integer, NHibernateZebraPrinterDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class