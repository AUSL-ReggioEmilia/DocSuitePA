Imports VecompSoftware.DocSuiteWeb.Facade

Public Class ProtDuplica
    Inherits ProtBasePage

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
        If Not Me.IsPostBack Then
            MasterDocSuite.TitleVisible = False
            Initialize()
        End If
    End Sub

    Private Sub Initialize()
        cbContenitore.Attributes("Value") = ProtocolDuplicaOption.Container
        cbMittente.Attributes("Value") = ProtocolDuplicaOption.Senders
        cbDestinatario.Attributes("Value") = ProtocolDuplicaOption.Recipients
        cbOggetto.Attributes("Value") = ProtocolDuplicaOption.Object
        cbClassificazione.Attributes("Value") = ProtocolDuplicaOption.Category
        cbAltre.Attributes("Value") = ProtocolDuplicaOption.Other
        cbDocType.Attributes("Value") = ProtocolDuplicaOption.DocType
        cbRoles.Attributes("Value") = ProtocolDuplicaOption.Roles

        pnlDocType.Visible = ProtocolEnv.IsTableDocTypeEnabled
        pnlRoles.Visible = ProtocolEnv.IsAuthorizInsertEnabled
        pnlInterop.Visible = ProtocolEnv.IsInteropEnabled
    End Sub

    Private Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        Dim s As String = "0000000000"
        SetCheck(s, CShort(cbContenitore.Attributes("Value")), cbContenitore.Checked)
        SetCheck(s, CShort(cbMittente.Attributes("Value")), cbMittente.Checked)
        SetCheck(s, CShort(cbDestinatario.Attributes("Value")), cbDestinatario.Checked)
        SetCheck(s, CShort(cbOggetto.Attributes("Value")), cbOggetto.Checked)
        SetCheck(s, CShort(cbClassificazione.Attributes("Value")), cbClassificazione.Checked)
        SetCheck(s, CShort(cbAltre.Attributes("Value")), cbAltre.Checked)

        If ProtocolEnv.IsTableDocTypeEnabled Then
            SetCheck(s, CShort(cbDocType.Attributes("Value")), cbDocType.Checked)
        End If
        If ProtocolEnv.IsAuthorizInsertEnabled Then
            SetCheck(s, CShort(cbRoles.Attributes("Value")), cbRoles.Checked)
        End If

        Me.MasterDocSuite.AjaxManager.ResponseScripts.Add("CloseWindow('" & s & "');")
    End Sub

    Private Sub SetCheck(ByRef field As String, ByVal right As Short, ByVal value As Boolean)
        If value Then
            Mid$(field, right, 1) = "1"
        Else
            Mid$(field, right, 1) = "0"
        End If
    End Sub
End Class