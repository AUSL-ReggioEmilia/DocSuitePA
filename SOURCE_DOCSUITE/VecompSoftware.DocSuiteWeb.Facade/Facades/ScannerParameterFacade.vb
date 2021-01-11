Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ScannerParameterFacade
    Inherits BaseProtocolFacade(Of ScannerParameter, Integer, NHibernateScannerParameterDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class