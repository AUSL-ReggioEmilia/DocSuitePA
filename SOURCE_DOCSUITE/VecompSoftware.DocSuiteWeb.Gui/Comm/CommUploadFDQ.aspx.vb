Imports System.Text
Imports System.IO
Imports VecompSoftware.Services.Logging

Partial Class CommUploadFDQ
    Inherits CommBasePage


#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

#Region " Fields and Properties. "

    Private _documentName As String
    Private ReadOnly Property GetDocumentName As String
        Get
            If _documentName Is Nothing Then
                _documentName = String.Empty
                If Request("DOCUMENTNAME") IsNot Nothing Then
                    Dim encodedDocumentName As String = ParseContent(Request("DOCUMENTNAME"))
                    Dim encoding As New UTF8Encoding()
                    Dim documentNameBytes As Byte() = Convert.FromBase64String(encodedDocumentName)
                    documentNameBytes = Convert.FromBase64String(encoding.GetString(documentNameBytes))
                    _documentName = encoding.GetString(documentNameBytes)
                End If
            End If
            Return _documentName
        End Get
    End Property

    Private _content As Byte()
    Private ReadOnly Property GetContent As Byte()
        Get
            If _content Is Nothing Then
                If Request("CONTENT") IsNot Nothing Then
                    Dim encodedContent As String = ParseContent(Request("CONTENT"))
                    _content = Convert.FromBase64String(encodedContent)
                End If
            End If
            Return _content
        End Get
    End Property

    Private _documentPath As String
    Private ReadOnly Property GetDocumentPath As String
        Get
            If _documentPath Is Nothing Then
                _documentPath = String.Empty
                If Not String.IsNullOrEmpty(GetDocumentName) Then
                    _documentPath = CommonInstance.AppTempPath & GetDocumentName
                End If
            End If
            Return _documentPath
        End Get
    End Property

#End Region


    ''' <summary>
    ''' Non ho la più pallida idea del perchè una cosa del genere possa essere di qualche utilità...
    ''' ma nel dubbio la lascio li.
    ''' </summary>
    ''' <param name="p_input">Stringa di input</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ParseContent(p_input As String) As String
        Dim retval As String = p_input
        retval = retval.Replace("@", "+")
        retval = retval.Replace("#", "/")

        Return retval
    End Function


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If GetContent IsNot Nothing Then
                File.WriteAllBytes(GetDocumentPath, GetContent)
                Session("_fileFDQ") = String.Empty
                Session("_fileFDQ") = GetDocumentName
                'Response.Clear()
                Response.Write(String.Empty)
            End If
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            Response.Write(GetDocumentPath & vbNewLine & ex.Message)
        Finally
            'Response.Close()
            Response.End()
        End Try
    End Sub

End Class


