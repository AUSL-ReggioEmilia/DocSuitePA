''' <summary> Tipo di collaborazione </summary>
Public Enum CollaborationDocumentType
    ''' <summary> Protocollo </summary>
    P
    ''' <summary> Delibera </summary>
    D
    ''' <summary> Atto/Determina </summary>
    A
    ''' <summary> Ordine del giorno </summary>
    O
    ''' <summary>
    ''' Serie Documentali
    ''' </summary>
    S
    ''' <summary>
    ''' UOIA
    ''' </summary>
    U
    ''' <summary>
    ''' Workflow
    ''' </summary>
    W
    ''' <summary>
    ''' UDS
    ''' </summary>
    UDS
End Enum

'todo: prevedere se è possibile utilizzare una classe (vedi sotto) per gestire i codici invece di un enumeratore. Utilizzo: CollaborationDocumentType.Protocollo.Code
'Public NotInheritable Class CollaborationDocumentType

'#Region "Fields"
'    Private _code As String
'#End Region

'#Region "Properties"
'    Public Shared ReadOnly Property Protocollo As CollaborationDocumentType
'        Get
'            Return New CollaborationDocumentType("P")
'        End Get
'    End Property

'    Public Shared ReadOnly Property Delibera As CollaborationDocumentType
'        Get
'            Return New CollaborationDocumentType("D")
'        End Get
'    End Property

'    Public Shared ReadOnly Property Atto As CollaborationDocumentType
'        Get
'            Return New CollaborationDocumentType("A")
'        End Get
'    End Property

'    Public Shared ReadOnly Property OrdineDelGiorno As CollaborationDocumentType
'        Get
'            Return New CollaborationDocumentType("O")
'        End Get
'    End Property

'    Public Shared ReadOnly Property SerieDocumentale As CollaborationDocumentType
'        Get
'            Return New CollaborationDocumentType("S")
'        End Get
'    End Property

'    Public Property Code As String
'        Get
'            Return _code
'        End Get
'        Private Set(value As String)
'            _code = value
'        End Set
'    End Property
'#End Region

'#Region "Constructor"
'    Private Sub New(itemCode As String)
'        Code = itemCode
'    End Sub
'#End Region

'End Class