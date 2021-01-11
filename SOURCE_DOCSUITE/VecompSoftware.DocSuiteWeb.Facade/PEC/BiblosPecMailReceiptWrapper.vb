Imports VecompSoftware.DocSuiteWeb.Data

Public Class BiblosPecMailReceiptWrapper
    Private _pecReceipt As PECMailReceipt
    Private ReadOnly _parent As BiblosPecMailWrapper

    ''' <summary>
    ''' Costruttore di base, richiede un PECMailAttachment di riferimento e in modalità lazy calcola tutto il resto
    ''' </summary>
    ''' <param name="pecReceipt"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal pecReceipt As PECMailReceipt, checkSignedEvaluateStream As Boolean)
        _pecReceipt = pecReceipt
        _parent = New BiblosPecMailWrapper(_pecReceipt.Parent, checkSignedEvaluateStream)
    End Sub

    Public ReadOnly Property Parent As BiblosPecMailWrapper
        Get
            Return _parent
        End Get
    End Property

    Public ReadOnly Property ReceiptType As String
        Get
            Return _pecReceipt.ReceiptType
        End Get
    End Property
End Class
