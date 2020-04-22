Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class DocumentNoteFacade
    Inherits BaseDocumentFacade(Of DocumentNote, Integer, NHibernateDocumentNoteDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class