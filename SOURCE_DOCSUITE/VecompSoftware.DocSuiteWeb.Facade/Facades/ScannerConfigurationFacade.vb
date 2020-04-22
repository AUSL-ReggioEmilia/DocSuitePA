Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ScannerConfigurationFacade
    Inherits BaseProtocolFacade(Of ScannerConfiguration, Integer, NHibernateScannerConfigurationDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overloads Sub SetDefaultConfiguration(ByVal p_entity As ScannerConfiguration)
        _dao.SetDefaultConfiguration(p_entity)
    End Sub

    Public Overloads Sub SetDefaultConfiguration(ByVal p_entityId As Integer)
        _dao.SetDefaultConfiguration(GetById(p_entityId))
    End Sub

    Public Function GetCurrentConfiguration() As ScannerConfiguration
        Dim clf As New ComputerLogFacade
        If clf.GetCurrent.ScannerConfiguration IsNot Nothing Then
            Return clf.GetCurrent.ScannerConfiguration
        Else
            Return GetDefaultConfiguration()
        End If
    End Function

    Public Function GetDefaultConfiguration() As ScannerConfiguration
        Return _dao.GetDefaultConfiguration()
    End Function

End Class