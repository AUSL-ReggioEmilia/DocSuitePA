Imports VecompSoftware.DocSuiteWeb.Data

Public Interface ICommImport

    Function InserimentoProtocollo(ByVal Protocollo As Protocol, ByVal all As Boolean) As Boolean

    Function CheckFiles() As DataTable

End Interface