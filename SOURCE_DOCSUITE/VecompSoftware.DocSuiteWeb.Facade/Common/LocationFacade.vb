Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class LocationFacade
    Inherits CommonFacade(Of Location, Integer, NHibernateLocationDao)
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

End Class