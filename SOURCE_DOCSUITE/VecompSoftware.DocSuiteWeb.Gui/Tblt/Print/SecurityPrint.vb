Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports System.Linq
Imports VecompSoftware.Helpers.Web

''' <summary> Classe base per la costruzione delle stampe con visualizzazione della sicurezza </summary>
Public MustInherit Class SecurityPrint
    Inherits BasePrint

    Public MustOverride Overrides Sub DoPrint()

#Region "Creazione Righe"
    ''' <summary>
    ''' Crea Riga di intestazione di una sezione (Settore, Contenitore, ecc...)
    ''' </summary>
    ''' <param name="tbl">tabella a cui aggangiare la riga</param>
    ''' <param name="text">testo da visualizzare</param>
    ''' <remarks></remarks>
    Protected Overridable Sub CreateSecurityRow(ByRef tbl As DSTable, ByRef text As String)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'crea riga
        tbl.CreateEmptyRow("Prnt-TabellaLbl")
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = text
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.ColumnSpan = 6
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    ''' <summary>
    ''' Crea Riga che identifica uno spazio ta le sezioni
    ''' </summary>
    ''' <param name="tbl">tabella a cui aggangiare la riga</param>
    ''' <param name="text">testo da visualizzare</param>
    ''' <param name="lineBox">True aggiunge un bordo alla riga, False di default</param>
    ''' <param name="cssClass">True aggiunge il css predefinito alla riga, False di default</param>
    ''' <param name="txtBold">True imposta il testo a bold, False di default</param>
    ''' <remarks></remarks>
    Protected Overridable Sub CreateSpaceRow(ByRef tbl As DSTable, Optional ByRef text As String = "", Optional ByVal lineBox As Boolean = False, Optional ByVal cssClass As Boolean = False, Optional ByVal txtBold As Boolean = False)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'crea riga
        If cssClass Then
            tbl.CreateEmptyRow("Prnt-Tabella")
        Else
            tbl.CreateEmptyRow()
        End If
        'crea cella sinistra
        tbl.CurrentRow.CreateEmpytCell()
        'stile cella sinistra
        cellStyle.Width = Unit.Percentage(5)
        cellStyle.Font.Bold = txtBold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = lineBox
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'crea cella destra
        tbl.CurrentRow.CreateEmpytCell()
        If String.IsNullOrEmpty(text) Then
            text = WebHelper.Space
        End If
        tbl.CurrentRow.CurrentCell.Text = text
        'stile cella sinistra
        cellStyle.Width = Unit.Percentage(95)
        cellStyle.Font.Bold = txtBold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = lineBox
        cellStyle.ColumnSpan = 5
        cellStyle.Wrap = True
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    ''' <summary>
    ''' Crea Riga che identifica un gruppo 
    ''' </summary>
    ''' <param name="tbl">tabella a cui aggangiare la riga</param>
    ''' <param name="gruppo">nome del gruppo</param>
    ''' <remarks></remarks>
    Protected Overridable Sub CreateGroupSecurityRow(ByRef tbl As DSTable, ByVal gruppo As String)
        CreateSpaceRow(tbl, gruppo, True, True, True)
    End Sub

    ''' <summary>
    ''' Crea Riga di intestazione utenti
    ''' </summary>
    ''' <param name="tbl">tabella a cui aggangiare la riga</param>
    ''' <param name="title">testo da inserire nella riga</param>
    ''' <remarks></remarks>
    Protected Overridable Sub CreateUserSecurityRow(ByRef tbl As DSTable, ByVal title As String)
        CreateSpaceRow(tbl, title, True, False, True)
    End Sub

    ''' <summary>
    ''' Crea riga lista di utenti
    ''' </summary>
    ''' <param name="tbl">tabella a cui aggangiare la riga</param>
    ''' <param name="utenti">lista di utenti</param>
    ''' <remarks></remarks>
    Protected Overridable Sub CreateUserListSecurityRow(ByRef tbl As DSTable, ByVal utenti As String)
        CreateSpaceRow(tbl, utenti, False, False, False)
    End Sub

    ''' <summary>
    ''' Crea riga con le informazioni sui diritti
    ''' </summary>
    ''' <param name="tbl">tabella a cui aggangiare la riga</param>
    ''' <param name="modulo">testo per la sezione Modulo</param>
    ''' <param name="diritti">testo per la sezione Diritti</param>
    ''' <param name="textBold">True imposta il testo a bold</param>
    ''' <param name="lineBox">True crea un bordo attorno alla riga</param>
    ''' <remarks></remarks>
    Protected Overridable Sub CreateRightsRow(ByRef tbl As DSTable, ByVal modulo As String, ByVal diritti As String, ByVal textBold As Boolean, ByVal lineBox As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'crea riga
        tbl.CreateEmptyRow("Prnt-Chiaro")
        'crea prima cella
        tbl.CurrentRow.CreateEmpytCell()
        'stile prima cella
        cellStyle.Width = Unit.Percentage(5)
        cellStyle.Font.Bold = textBold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = lineBox
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'crea seconda cella (Modulo)
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = modulo
        'stile seconda cella (Modulo)
        cellStyle.Width = Unit.Percentage(20)
        cellStyle.Font.Bold = textBold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = lineBox
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'crea terza cella (Diritti)
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = diritti
        'stile seconda cella (Diritti)
        cellStyle.Width = Unit.Percentage(75)
        cellStyle.Font.Bold = textBold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.ColumnSpan = 4
        cellStyle.LineBox = lineBox
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "Funzioni di Stampa"

    Protected Delegate Function TraslitteraDelegate(environment As DSWEnvironment, group As GroupRights) As String

    Private _traslittera As TraslitteraDelegate

    Protected Property TraslitteraDiritti() As TraslitteraDelegate
        Get
            Return _traslittera
        End Get
        Set(ByVal value As TraslitteraDelegate)
            _traslittera = value
        End Set
    End Property

    Protected Sub CreateGroupsSection(ByRef tbl As DSTable, ByVal group As GroupRights)
        Dim rights As String = String.Empty

        CreateGroupSecurityRow(TablePrint, "Gruppo: " & group.Name)
        'Intestazione Modulo - Diritti
        CreateRightsRow(TablePrint, "Modulo", "Diritti", True, True)
        'Appendo i diritti per i vari moduli

        If DocSuiteContext.Current.IsDocumentEnabled Then
            rights = TraslitteraDiritti(DSWEnvironment.Document, group)
            If Not String.IsNullOrEmpty(rights) Then
                CreateRightsRow(TablePrint, "Pratiche", rights, False, False)
            End If
        End If
        If DocSuiteContext.Current.IsProtocolEnabled Then
            rights = TraslitteraDiritti(DSWEnvironment.Protocol, group)
            If Not String.IsNullOrEmpty(rights) Then
                CreateRightsRow(TablePrint, "Protocollo", rights, False, False)
            End If
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            rights = TraslitteraDiritti(DSWEnvironment.Resolution, group)
            If Not String.IsNullOrEmpty(rights) Then
                CreateRightsRow(TablePrint, Facade.TabMasterFacade.TreeViewCaption, rights, False, False)
            End If
        End If

        'Aggiungo gli utenti
        CreateUserSecurityRow(TablePrint, "Utenti")
        Dim listUser As String = GetUsersList(group)
        If String.IsNullOrEmpty(listUser) Then
            listUser = "Nessun utente abbinato."
        End If
        CreateUserListSecurityRow(TablePrint, listUser)
    End Sub

#End Region

    ''' <summary>
    ''' Recupera la lista (descrittiva) degli utenti abbinati al gruppo specificato.
    ''' </summary>
    ''' <param name="group">Gruppo di cui recuperare le utenze</param>
    Private Function GetUsersList(group As GroupRights) As String
        Dim sb As New StringBuilder

        If group.SecurityGroup IsNot Nothing Then
            For Each user As SecurityUsers In Facade.SecurityUsersFacade.GetUsersByGroup(group.SecurityGroup).OrderBy(Function(x) x.Description)
                If sb.Length > 0 Then
                    sb.Append(", ")
                End If
                sb.Append(String.Format("{0}\{1} - ({2})", user.UserDomain, user.Account, user.Description))
            Next
        Else
            sb.Append("Nessuno SecurityGroup abbinato.")
        End If

        Return sb.ToString()
    End Function

End Class
