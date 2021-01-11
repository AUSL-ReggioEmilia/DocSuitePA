Public Class POLRequestMetaData
    ''' <summary>
    ''' When PushDocuments call was made against Serc Service
    ''' </summary>
    Public Property PushCalledAt As DateTimeOffset

    ''' <summary>
    ''' Last GetStatus call against Serc Service
    ''' </summary>
    Public Property LastGetStatusAt As DateTimeOffset

    ''' <summary>
    ''' If there will be no more GetStatus calls
    ''' </summary>
    Public Property DoneWithGetStatus As Boolean

    ''' <summary>
    ''' Flag that marks if the document from the get status response was saved
    ''' </summary>
    Public Property DocumentFromUrlCpfSaved As Boolean

    ''' <summary>
    ''' Flag that marks if the document from the get status response was saved
    ''' </summary>
    Public Property DocumentFromUrlCpfXmlSaved As Boolean

    ''' <summary>
    ''' The sender
    ''' </summary>
    Public Property PolRequestContactName As String

    ''' <summary>
    ''' The account name
    ''' </summary>
    Public Property PolAccountName As String

    Sub New()
        DocumentFromUrlCpfSaved = False
        DocumentFromUrlCpfXmlSaved = False
        DoneWithGetStatus = False
        PolRequestContactName = String.Empty
        PolAccountName = String.Empty
    End Sub
End Class
