Partial Class CommFrameset
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

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
    End Sub

    Public Function Genera() As String
        Dim QueryString As String = Request.ServerVariables("QUERY_STRING").ToString()
        Dim i As Integer = InStr(QueryString, "Titolo")
        Dim ii As Integer = InStr(i + 1, QueryString, "&")
        Return Mid(QueryString, ii + 1)
    End Function
End Class


