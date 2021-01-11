Public Class DocmSommarioL
    Inherits DocmBasePage

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
            Initialize()
        End If
    End Sub

    Private Sub Initialize()

        MasterDocSuite.TitleVisible = False

        Dim type As String = String.Empty
        Dim row As String = String.Empty
        type = "10-B-C|90-B-L"
        row = "../Docm/Images/RoleOnP.gif|" & "Presa in Carico"
        WebUtils.ObjTableAdd("Comm", tblLegenda, row, type, False, "Chiaro", Nothing)
        row = "../Docm/Images/RoleOnPR.gif|" & "Presa in Carico e Richiesta in Attesa"
        WebUtils.ObjTableAdd("Comm", tblLegenda, row, type, False, "Chiaro", Nothing)
        row = "../Docm/Images/RoleOnRR.gif|" & "Presa in Carico con Restituzione"
        WebUtils.ObjTableAdd("Comm", tblLegenda, row, type, False, "Chiaro", Nothing)
        row = "../Docm/Images/RoleOnR.gif|" & "Richiesta"
        WebUtils.ObjTableAdd("Comm", tblLegenda, row, type, False, "Chiaro", Nothing)
        row = "../Docm/Images/RoleOnRN.gif|" & "Richiesta con Esito negativo"
        WebUtils.ObjTableAdd("Comm", tblLegenda, row, type, False, "Chiaro", Nothing)
        row = "../Docm/Images/RoleOnMR.gif|" & "Richiesta e Richiesta con Esito negativo"
        WebUtils.ObjTableAdd("Comm", tblLegenda, row, type, False, "Chiaro", Nothing)
    End Sub

End Class
