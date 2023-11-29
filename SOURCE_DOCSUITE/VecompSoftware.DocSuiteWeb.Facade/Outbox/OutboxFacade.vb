Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class OutboxFacade
    Inherits BaseProtocolFacade(Of OutboxMessage, Guid, NHibernateOutboxDao)

    Public Sub New()
        MyBase.New()
    End Sub
End Class