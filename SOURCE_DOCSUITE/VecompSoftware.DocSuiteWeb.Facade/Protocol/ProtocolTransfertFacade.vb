Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ProtocolTransfertFacade
    Inherits BaseProtocolFacade(Of ProtocolTransfert, Integer, NHibernateProtocolTransferDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class