Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocmInfoStep
    Inherits DocmBasePage

#Region " Constants "
    Const PIPE As String = "|"
    Const SEP As Char = ","c
#End Region
   
#Region " Fields "

    Private _viewAll As Boolean = False
    Private _txtPStepValue As String
    Private _txtPIdOwnerValue As String
    Private _txtRRStepValue As String
    Private _txtRRIdOwnerValue As String
    Private _txtUserStepValue As String
    Private _txtUserAccountValue As String
    Private _txtPRStepValue As String
    Private _txtPRIdOwnerValue As String
    Private _txtRStepValue As String
    Private _txtRIdOwnerValue As String
    Private _txtRNStepValue As String
    Private _txtRNIdOwnerValue As String
    Private _txtCCStepValue As String
    Private _txtCCIdOwnerValue As String
    Private _txtIdRoleRightValue As String
    Private _txtIdRoleRightWValue As String
    Private _txtIdRoleRightMValue As String

#End Region

#Region " Properties"

    Public Property ViewAll() As Boolean
        Get
            Return _viewAll
        End Get
        Set(ByVal value As Boolean)
            _viewAll = value
        End Set
    End Property

    Public Property txtPStepValue() As String
        Get
            Return _txtPStepValue
        End Get
        Set(ByVal value As String)
            _txtPStepValue = value
        End Set
    End Property

    Public Property txtPIdOwnerValue() As String
        Get
            Return _txtPIdOwnerValue
        End Get
        Set(ByVal value As String)
            _txtPIdOwnerValue = value
        End Set
    End Property

    Public Property txtRRStepValue() As String
        Get
            Return _txtRRStepValue
        End Get
        Set(ByVal value As String)
            _txtRRStepValue = value
        End Set
    End Property

    Public Property txtRRIdOwnerValue() As String
        Get
            Return _txtRRIdOwnerValue
        End Get
        Set(ByVal value As String)
            _txtRRIdOwnerValue = value
        End Set
    End Property

    Public Property txtUserStepValue() As String
        Get
            Return _txtUserStepValue
        End Get
        Set(ByVal value As String)
            _txtUserStepValue = value
        End Set
    End Property

    Public Property txtUserAccountValue() As String
        Get
            Return _txtUserAccountValue
        End Get
        Set(ByVal value As String)
            _txtUserAccountValue = value
        End Set
    End Property

    Public Property txtPRStepValue() As String
        Get
            Return _txtPRStepValue
        End Get
        Set(ByVal value As String)
            _txtPRStepValue = value
        End Set
    End Property

    Public Property txtPRIdOwnerValue() As String
        Get
            Return _txtPRIdOwnerValue
        End Get
        Set(ByVal value As String)
            _txtPRIdOwnerValue = value
        End Set
    End Property

    Public Property txtRStepValue() As String
        Get
            Return _txtRStepValue
        End Get
        Set(ByVal value As String)
            _txtRStepValue = value
        End Set
    End Property

    Public Property txtRIdOwnerValue() As String
        Get
            Return _txtRIdOwnerValue
        End Get
        Set(ByVal value As String)
            _txtRIdOwnerValue = value
        End Set
    End Property

    Public Property txtRNStepValue() As String
        Get
            Return _txtRNStepValue
        End Get
        Set(ByVal value As String)
            _txtRNStepValue = value
        End Set
    End Property

    Public Property txtRNIdOwnerValue() As String
        Get
            Return _txtRNIdOwnerValue
        End Get
        Set(ByVal value As String)
            _txtRNIdOwnerValue = value
        End Set
    End Property

    Public Property txtCCStepValue() As String
        Get
            Return _txtCCStepValue
        End Get
        Set(ByVal value As String)
            _txtCCStepValue = value
        End Set
    End Property

    Public Property txtCCIdOwnerValue() As String
        Get
            Return _txtCCIdOwnerValue
        End Get
        Set(ByVal value As String)
            _txtCCIdOwnerValue = value
        End Set
    End Property

    Public Property txtIdRoleRightValue() As String
        Get
            Return _txtIdRoleRightValue
        End Get
        Set(ByVal value As String)
            _txtIdRoleRightValue = value
        End Set
    End Property

    Public Property txtIdRoleRightWValue() As String
        Get
            Return _txtIdRoleRightWValue
        End Get
        Set(ByVal value As String)
            _txtIdRoleRightWValue = value
        End Set
    End Property

    Public Property txtIdRoleRightMValue() As String
        Get
            Return _txtIdRoleRightMValue
        End Get
        Set(ByVal value As String)
            _txtIdRoleRightMValue = value
        End Set
    End Property

#End Region

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

#Region " Eventi"

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        GetQuerystringValues()
        Me.MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnAll, phTable)
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAll.Click
        ViewAll = True
        Initialize()
    End Sub

#End Region

#Region " Initialize"
    Private Sub Initialize()

        Dim tblNew As DSTable = New DSTable()

        CreaIntestazione(tblNew, "Prese in carico")
        GetDataFromStepTextbox(txtPStepValue, txtPIdOwnerValue, "txtPStep", "txtPIdOwner", tblNew)

        CreaIntestazione(tblNew, "Prese in Carico con Ricevuta di Ritorno")
        GetDataFromStepTextbox(txtRRStepValue, txtRRIdOwnerValue, "txtRRStep", "txtRRIdOwner", tblNew)

        CreaIntestazione(tblNew, "Assegnazione Utenti")
        GetDataFromUserTextbox(txtUserStepValue, txtUserAccountValue, "txtUserStep", "txtUserAccount", tblNew)

        CreaIntestazione(tblNew, "Prese in Carico con Richieste in Attesa")
        GetDataFromStepTextbox(txtPRStepValue, txtPRIdOwnerValue, "txtPRStep", "txtPRIdOwner", tblNew)

        CreaIntestazione(tblNew, "Richieste")
        GetDataFromStepTextbox(txtRStepValue, txtRIdOwnerValue, "txtRStep", "txtRIdOwner", tblNew)

        CreaIntestazione(tblNew, "Richieste Negative")
        GetDataFromStepTextbox(txtRNStepValue, txtRNIdOwnerValue, "txtRNStep", "txtRNIdOwner", tblNew)

        CreaIntestazione(tblNew, "Copia Conoscenza")
        GetDataFromStepTextbox(txtCCStepValue, txtCCIdOwnerValue, "txtCCStep", "txtCCIdOwner", tblNew)

        ''abilitati
        CreaIntestazione(tblNew, "Settori abilitati")
        GetDataFromStepTextbox("", txtIdRoleRightValue, "", "txtIdRoleRight", tblNew)

        ''workflow
        CreaIntestazione(tblNew, "Settori abilitati con Diritto WorkFlow")
        GetDataFromStepTextbox("", txtIdRoleRightWValue, "", "txtIdRoleRightW", tblNew)

        ''manager
        CreaIntestazione(tblNew, "Settori abilitati con Diritto Manager")
        GetDataFromStepTextbox("", txtIdRoleRightMValue, "", "txtIdRoleRightM", tblNew)

        Me.phTable.Controls.Add(tblNew.Table)
    End Sub
#End Region

#Region " Creazione tabella di visualizzazione"

    Private Sub CreaIntestazione(ByRef tblNew As DSTable, ByVal testo As String)

        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()

        tblNew.Width = Unit.Percentage(100)
        tblNew.CreateEmptyRow("tabella")
        tblNew.CurrentRow.CreateEmpytCell()
        tblNew.CurrentRow.CurrentCell.Text = testo

        cellStyle.ColumnSpan = 2
        cellStyle.Font.Bold = True
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left

        tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

    End Sub

    Private Sub CreaRiga(ByRef tblNew As DSTable, ByVal testo As String, Optional ByVal rowIndex As Integer = 0)

        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()

        tblNew.Width = Unit.Percentage(100)
        tblNew.CreateEmptyRow("chiaro")

        tblNew.CurrentRow.CreateEmpytCell()

        If rowIndex <> 0 Then
            tblNew.CurrentRow.CurrentCell.Text = rowIndex
        End If

        cellStyle.Font.Bold = False
        cellStyle.Width = Unit.Percentage(15)
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        tblNew.CurrentRow.CreateEmpytCell()
        tblNew.CurrentRow.CurrentCell.Text = testo

        cellStyle.Font.Bold = True
        cellStyle.Width = Unit.Percentage(85)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

    End Sub

#End Region

#Region " Estrazione dati"

    Private Sub GetDataFromStepTextbox(ByVal sStepTxtContent As String, ByVal sOwnerTxtContent As String, ByVal sStepTxtName As String, ByVal sOwnerTxtName As String, ByRef tblNew As DSTable)

        Dim owners As String() = Nothing
        Dim steps As String() = Nothing

        If sOwnerTxtContent <> String.Empty Then
            owners = sOwnerTxtContent.Split(SEP)
        End If

        If sStepTxtContent <> String.Empty Then
            steps = sStepTxtContent.Split(SEP)
        End If

        If owners IsNot Nothing Then
            For i As Integer = 0 To owners.Length - 1
                Dim idRole As Integer
                Dim role As Role
                Dim rolePath As String = String.Empty
                Dim row As String = ""

                If Integer.TryParse(owners(i).Replace(PIPE, String.Empty), idRole) Then
                    role = Facade.RoleFacade.GetById(idRole)

                    If Not role Is Nothing Then
                        SetRoleString(rolePath, role.FullIncrementalPath)
                    End If

                    row = rolePath
                    If ViewAll Then row &= " (" & role.Id & ")"
                End If
                If row.StartsWith(PIPE) Then row.Remove(0, 1)

                If sStepTxtContent <> String.Empty Then
                    CreaRiga(tblNew, row, steps(i).Replace(PIPE, String.Empty))
                Else
                    CreaRiga(tblNew, row)
                End If

            Next
            If ViewAll Then
                CreaRiga(tblNew, sStepTxtName & ": " & sStepTxtContent.Replace(PIPE, "'") & " " & sOwnerTxtName & ": " & sOwnerTxtContent.Replace(PIPE, "'"))
            End If

        End If

    End Sub

    Private Sub GetDataFromUserTextbox(ByVal sStepTxtContent As String, ByVal sOwnerTxtContent As String, ByVal sStepTxtName As String, ByVal sOwnerTxtName As String, ByRef tblNew As DSTable)
        Dim owners As String() = Nothing
        Dim steps As String() = Nothing

        If sOwnerTxtContent <> String.Empty Then
            owners = sOwnerTxtContent.Split(SEP)
        End If

        If sStepTxtContent <> String.Empty Then
            steps = sStepTxtContent.Split(SEP)
        End If

        If steps IsNot Nothing Then
            For i As Integer = 0 To steps.Length - 1
                CreaRiga(tblNew, owners(i).Replace(PIPE, String.Empty), steps(i).Replace(PIPE, String.Empty))
            Next i
            If ViewAll Then
                CreaRiga(tblNew, sStepTxtName & ": " & sStepTxtContent.Replace(PIPE, "'") & " " & sOwnerTxtName & ": " & sOwnerTxtContent.Replace(PIPE, "'"))
            End If
        End If

    End Sub

#End Region

#Region " Funzioni di supporto"

    Public Sub SetRoleString(ByRef Testo As String, ByVal FullIncremental As String)
        If FullIncremental = "" Then Exit Sub
        Dim incrementalArray As String()
        Dim idRole As Integer
        Dim role As Role

        incrementalArray = Split(FullIncremental, PIPE)
        idRole = Integer.Parse(incrementalArray(0))
        role = Facade.RoleFacade.GetById(idRole)

        If Not role Is Nothing Then
            If Testo <> "" Then Testo &= "/"
            Testo &= role.Name
            If incrementalArray.Length > 1 Then
                SetRoleString(Testo, FullIncremental.Substring(FullIncremental.IndexOf(PIPE) + 1))
            End If
        End If
    End Sub

    Private Sub GetQuerystringValues()
        txtPStepValue = Request.QueryString("txtPStep")
        txtPIdOwnerValue = Request.QueryString("txtPIdOwner")
        txtRRStepValue = Request.QueryString("txtRRStep")
        txtRRIdOwnerValue = Request.QueryString("txtRRIdOwner")
        txtUserStepValue = Request.QueryString("txtUserStep")
        txtUserAccountValue = Request.QueryString("txtUserAccount")
        txtPRStepValue = Request.QueryString("txtPRStep")
        txtPRIdOwnerValue = Request.QueryString("txtPRIdOwner")
        txtRStepValue = Request.QueryString("txtRStep")
        txtRIdOwnerValue = Request.QueryString("txtRIdOwner")
        txtRNStepValue = Request.QueryString("txtRNStep")
        txtRNIdOwnerValue = Request.QueryString("txtRNIdOwner")
        txtCCStepValue = Request.QueryString("txtCCStep")
        txtCCIdOwnerValue = Request.QueryString("txtCCIdOwner")
        txtIdRoleRightValue = Request.QueryString("txtIdRoleRight")
        txtIdRoleRightWValue = Request.QueryString("txtIdRoleRightW")
        txtIdRoleRightMValue = Request.QueryString("txtIdRoleRightM")
    End Sub

#End Region

End Class