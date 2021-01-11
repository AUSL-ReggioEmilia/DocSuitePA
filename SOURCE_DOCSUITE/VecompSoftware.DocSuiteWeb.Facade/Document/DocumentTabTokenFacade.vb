Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class DocumentTabTokenFacade
    Inherits BaseDocumentFacade(Of DocumentTabToken, String, NHibernateDocumentTabTokenDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class