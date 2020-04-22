Public Class ProtImportList
    Inherits ProtBasePage

    Shadows Type As String = String.Empty

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Type = Request.QueryString("ImportType")
        Dim sContainer As String = Request.QueryString("Container")
        If Not Me.IsPostBack Then
            Me.ViewState("SortExpression") = "FILEXML"
            Me.ViewState("SortAscending") = "ASC"
            Me.ViewState("Container") = sContainer
            Initialize(sContainer)
        End If
    End Sub

    Private Sub Initialize(ByVal sContainer As String)
        Dim dtFile As DataTable = Nothing

        ''EMANUELE: distinzione tra importazione fatture e lettere
        Select Case (Type)
            Case "Fattura"
                Dim oImport As New CommImport(sContainer, Type)
                dtFile = oImport.CheckFiles()
            Case "Lettera"
                Dim oImport As New CommImportLettera(CommonInstance)
                dtFile = oImport.CheckFiles()
            Case "Massiva"
                Dim oImport As New CommImport("", Type)
                dtFile = oImport.CheckFiles()
            Case "Excel"
                Dim oImport As New CommImport("", Type)
                dtFile = oImport.CheckFiles(True)
        End Select
        ''

        If dtFile.Rows.Count > 0 Then
            tblRicerca.Visible = False
            DG.Visible = True
            DG.DataSource = dtFile
            DG.DataBind()
            Title = "Protocollo Import - Stato File per Importazione (" & dtFile.Rows.Count & ")"
        Else
            DG.Visible = False
            tblRicerca.Visible = True
            Title = "Protocollo Import - Stato File per Importazione (0)"
        End If

    End Sub

    Private Sub DG_SortCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles DG.SortCommand
        Dim SortExpression As String = Me.ViewState("SortExpression")
        Dim SortAscending As String = Me.ViewState("SortAscending")
        Me.ViewState("SortExpression") = e.SortExpression
        If e.SortExpression = SortExpression Then
            Me.ViewState("SortAscending") = If(SortAscending = "ASC", "DESC", "ASC")
        Else
            Me.ViewState("SortAscending") = "ASC"
        End If
        Initialize(ViewState("Container"))
    End Sub

    Private Sub DG_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles DG.ItemCreated
        If e.Item.ItemType = ListItemType.Header Then
            Dim SortExpression As String = Me.ViewState("SortExpression")
            Dim SortAscending As String = Me.ViewState("SortAscending")
            Dim SortOrder As String = If(SortAscending = "ASC", " 5", " 6")
            Dim i As Integer = 0
            For i = 0 To DG.Columns.Count - 1
                If SortExpression = DG.Columns(i).SortExpression Then
                    Dim lbl As New Label
                    lbl.Font.Name = "webdings"
                    lbl.Font.Size = FontUnit.XSmall
                    lbl.Text = SortOrder
                    e.Item.Cells(i).Controls.Add(lbl)
                    Exit For
                End If
            Next i
        End If
    End Sub

End Class
