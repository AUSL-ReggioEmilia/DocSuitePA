Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ResolutionRecipientFacade
    Inherits BaseResolutionFacade(Of ResolutionRecipient, ResolutionRecipientCompositeKey, NHibernateResolutionRecipientDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class