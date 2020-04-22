Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class FascicleDocumentUnitFacade
    Inherits BaseProtocolFacade(Of FascicleDocumentUnit, Integer, NHibernateFascicleDocumentUnitsDao)

    Public Sub New()
        MyBase.New()
    End Sub


    Public Function GetByProtocol(protocol As Protocol) As IList(Of FascicleDocumentUnit)
        Return _dao.GetByProtocol(protocol)
    End Function

End Class