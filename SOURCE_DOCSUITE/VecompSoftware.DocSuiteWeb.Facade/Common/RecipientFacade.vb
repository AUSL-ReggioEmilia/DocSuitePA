Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject(), Serializable()> _
Public Class RecipientFacade
    Inherits CommonFacade(Of Recipient, Integer, NHibernateRecipientDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

End Class