Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web
Imports System.Collections.Generic
Imports System.Linq

Partial Class CommUtenti
    Inherits CommBasePage

#Region " Fields "

    Dim _tablePrint As DSTable

#End Region

#Region " Properties "

    Private ReadOnly Property IDRole() As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer?)("IDRole", Nothing)
        End Get
    End Property

    Private ReadOnly Property TablePrint() As DSTable
        Get
            If _tablePrint Is Nothing Then
                _tablePrint = New DSTable()
                _tablePrint.Width = Unit.Percentage(100)
            End If
            Return _tablePrint
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        If Not IDRole.HasValue Then
            Exit Sub
        End If

        'Recupero tutti i gruppi del ruolo
        Dim role As Role = Facade.RoleFacade.GetById(IDRole.Value)

        If role IsNot Nothing Then
            For Each roleGroup As RoleGroup In role.RoleGroups
                'GroupName
                If String.IsNullOrEmpty(roleGroup.Name) Then
                    Exit Sub
                End If
                CreateRow(TablePrint, roleGroup.Name, "Titolo", True)

                '--Utenti
                CreateRow(TablePrint, "Utenti", "tabella", True)

                Dim userList As List(Of String) = New List(Of String)
                If roleGroup.SecurityGroup Is Nothing Then
                    userList.Add("Il gruppo non è collegato ad un SecurityGroup.")
                Else
                    If (roleGroup.SecurityGroup.HasAllUsers) Then
                        userList.Add(SecurityGroups.DEFAULT_ALL_USER)
                    End If
                    userList.AddRange(roleGroup.SecurityGroup.SecurityUsers.OrderBy(Function(x) x.Description).Select(Function(f) String.Format("{0}\{1} - ({2})", f.UserDomain, f.Account, f.Description)))
                End If

                For Each userItem As String In userList
                    CreateRow(TablePrint, userItem, "Chiaro", False)
                Next userItem
                '--
                CreateRow(TablePrint, WebHelper.Space, "Chiaro", False)
            Next
        Else
            CreateRow(TablePrint, "Nessun gruppo associato", "Chiaro", False)
        End If
        tblCellUtenti.Controls.Add(TablePrint.Table)
    End Sub

    Private Sub CreateRow(ByRef tbl As DSTable, ByVal text As String, ByVal cssClass As String, ByVal lineBox As Boolean)
        Dim cellStyle As New DSTableCellStyle()

        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = lineBox

        tbl.CreateEmptyRow(cssClass)
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = text
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

#End Region

End Class


