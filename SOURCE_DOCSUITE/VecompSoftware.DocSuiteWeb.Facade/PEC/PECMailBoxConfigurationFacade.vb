
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class PECMailBoxConfigurationFacade
    Inherits BaseProtocolFacade(Of PECMailBoxConfiguration, Integer, NHibernatePECMailBoxConfigurationDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

End Class
