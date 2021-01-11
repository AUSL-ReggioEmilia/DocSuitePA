
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class PECMailContentFacade
    Inherits BaseProtocolFacade(Of PECMailContent, Integer, NHibernatePECMailContentDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByMail(pec As PECMail) As PECMailContent
        Return _dao.GetByMail(pec)
    End Function

End Class
