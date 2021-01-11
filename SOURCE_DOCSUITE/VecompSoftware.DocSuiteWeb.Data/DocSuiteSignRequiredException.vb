Public Class DocSuiteSignRequiredException
    Inherits Exception
#Region "[ Fields ]"

    Private Const DOCUMENT_TO_SIGN_REQUIRED As String = "Il documento '{0}' deve essere firmato"
#End Region

#Region "[ Property ]"

    Public Property Document As String
#End Region

#Region "[ Constructor ]"

    Public Sub New(ByVal document As String)
        MyBase.New(String.Format(DOCUMENT_TO_SIGN_REQUIRED, document))
        Me.Document = document
    End Sub

#End Region

End Class
