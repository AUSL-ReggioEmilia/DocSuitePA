
<ServiceContract()>
Public Interface IWebService

    <OperationContract()>
    Function Inserisci(ByVal tipoDocumento As String, ByVal titolo As String, ByVal dataAdozione As DateTime, ByVal oggetto As String, ByVal revocato As Short) As Long

    <OperationContract()>
    Function PubblicaPath(ByVal attoPath As String, ByVal nPubblicazione As Long, ByVal ritirato As Short, ByVal dataCaricamento As DateTime, ByVal oggetto As String) As Long

    <OperationContract()>
    Sub Revoca(ByVal nPubblicazione As Long)

End Interface



