Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Stampa dei settori (Normale) </summary>
Public Class RolePrint
    Inherits BasePrint

#Region "Fields"
    Private _roles As IList(Of Integer)
#End Region

#Region "Properties"
    Public Property RolesID() As IList(Of Integer)
        Get
            If _roles Is Nothing Then
                _roles = New List(Of Integer)
            End If
            Return _roles
        End Get
        Set(ByVal value As IList(Of Integer))
            _roles = value
        End Set
    End Property
#End Region

#Region "DoPrint"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa dei Settori"
        StampaRuoli()
    End Sub
#End Region

#Region "Creazione Righe"
    Private Sub CreaRigaRuolo(ByRef tbl As DSTable, ByVal text As String, ByVal isActive As Integer, ByVal lineBox As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'crea riga
        Select Case isActive
            Case 0
                tbl.CreateEmptyRow("Prnt-Grigio")
            Case 1
                tbl.CreateEmptyRow("Prnt-Tabella")
        End Select
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = text
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = lineBox
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreaRigaGruppo(ByRef tbl As DSTable, ByVal Text As String)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'crea riga
        tbl.CreateEmptyRow()
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = Text
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "Funzioni di Stampa"
    Private Sub StampaGruppi(ByVal role As Role, Optional ByVal level As String = "")
        For Each group As RoleGroup In role.RoleGroups
            CreaRigaGruppo(TablePrint, level & group.Name)
        Next
    End Sub

    Private Sub StampaRuoli()
        Dim roles As IList(Of Role) = New List(Of Role)()

        For Each id As Integer In RolesID
            roles.Add(Facade.RoleFacade.GetById(id))
        Next

        For Each role As Role In roles
            Dim eMail As String = ""
            '--Ruolo
            If Not String.IsNullOrEmpty(role.EMailAddress) Then
                eMail = " - " & role.EMailAddress
            End If
            CreaRigaRuolo(TablePrint, String.Format("{0}{1} ({2})", role.Name, eMail, role.Id), role.IsActive, True)
            '--Sottoruolo
            StampaGruppi(role)
            CreateTreeRolePrint(role)
        Next
    End Sub

    Private Sub CreateTreeRolePrint(ByVal role As Role)
        Dim strLevel As String = String.Empty
        Dim text As String

        Dim roles As IList(Of Role) = Facade.RoleFacade.GetItemsByParentId(role.Id)
        If roles.Count > 0 Then
            For Each childrenRole As Role In roles
                Dim eMail As String = ""
                If Not String.IsNullOrEmpty(childrenRole.EMailAddress) Then
                    eMail = " - " & childrenRole.EMailAddress
                End If
                strLevel = strLevel.PadLeft(childrenRole.Level, "."c) & " "
                text = String.Format("{0}{1}{2} ({3})", strLevel, childrenRole.Name, eMail, childrenRole.Id)
                CreaRigaRuolo(TablePrint, text, role.IsActive, False)

                StampaGruppi(childrenRole, strLevel)
                CreateTreeRolePrint(childrenRole)
            Next
        End If
    End Sub
#End Region

End Class
