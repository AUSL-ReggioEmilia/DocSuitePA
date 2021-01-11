Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class DocumentTabStatusFacade
    Inherits BaseDocumentFacade(Of DocumentTabStatus, String, NHibernateDocumentTabStatusDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class