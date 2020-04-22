Imports VecompSoftware.DocSuiteWeb.Data

Namespace PEC

    Public Interface IHavePecMail
        ReadOnly Property PecMails() As IEnumerable(Of PECMail)
    End Interface

End Namespace