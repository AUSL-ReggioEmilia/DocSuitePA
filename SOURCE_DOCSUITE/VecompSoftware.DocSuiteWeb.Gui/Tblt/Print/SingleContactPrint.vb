Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public Class SingleContactPrint
    Inherits BasePrint

#Region "Fields"
    Private _contact As Contact = Nothing
#End Region

#Region "Properties"
    Public Property Contact() As Contact
        Get
            Return _contact
        End Get
        Set(ByVal value As Contact)
            _contact = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New(ByVal idContact As Integer)
        _contact = Facade.ContactFacade.GetById(idContact)
    End Sub
#End Region


#Region "DoPrint"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa dei Contatti"
        StampaGerarchiaContatto()
    End Sub
#End Region

#Region "Creazione Righe"
    Private Sub CreaIntestazioneContatto(ByRef tbl As DSTable)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.VerticalAlignment = VerticalAlign.Middle
        cellStyle.LineBox = True
        'crea riga
        tbl.CreateEmptyRow("Prnt-Tabella")
        'Contatto
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(40)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.CurrentRow.CurrentCell.Text = "Contatto"
        'Nr. Telefono
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(20)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.CurrentRow.CurrentCell.Text = "Nr. Telefono"
        'E-Mail
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(20)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.CurrentRow.CurrentCell.Text = "E-Mail"
        'Indirizzo
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(20)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.CurrentRow.CurrentCell.Text = "Indirizzo"

        '**** DG - 2011-08-19 - Segn. 1096 - stampa dei contatti da rubrica
        'PEC
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(20)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.CurrentRow.CurrentCell.Text = "PEC"
        '**** DG
    End Sub

    Private Sub CreaRigaContatto(ByRef tbl As DSTable, ByVal contact As Contact)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.Wrap = False
        cellStyle.LineBox = True
        'crea riga
        tbl.CreateEmptyRow()

        'Contatto
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(40)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        Dim description As String = String.Empty
        If Not String.IsNullOrEmpty(contact.Description) Then
            description = Replace(contact.Description, "|", " ")
        End If
        tbl.CurrentRow.CurrentCell.Text = GetIndentationString(contact.Level) & description

        'Nr. Telefono
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(20)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.CurrentRow.CurrentCell.Text = contact.TelephoneNumber

        'E-Mail
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(20)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.CurrentRow.CurrentCell.Text = contact.EmailAddress

        'Indirizzo
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(20)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        If contact.Address IsNot Nothing Then
            tbl.CurrentRow.CurrentCell.Text = contact.Address.Address & " " & contact.Address.CivicNumber & " " & _
                                            contact.Address.ZipCode & " " & contact.Address.City
        Else
            tbl.CurrentRow.CurrentCell.Text = String.Empty
        End If

        '**** DG - 2011-08-19 - Segn. 1096 - stampa dei contatti da rubrica
        'PEC
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(20)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        If contact.CertifiedMail IsNot Nothing Then
            tbl.CurrentRow.CurrentCell.Text = contact.CertifiedMail
        End If
        '**** DG

    End Sub

    Private Function GetIndentationString(ByVal level As Integer) As String
        Dim indentation As String = String.Empty
        Dim i As Integer

        For i = 0 To level
            indentation &= ".  "
        Next

        Return indentation
    End Function
#End Region

#Region "Funzioni di Stampa"
    Private Sub StampaGerarchiaContatto()
        Dim contacts As IList(Of Contact)

        contacts = Facade.ContactFacade.GetContactByIncrementalFather(Contact.Id, True)

        CreaIntestazioneContatto(TablePrint)
        For Each contact As Contact In contacts
            CreaRigaContatto(TablePrint, contact)
        Next
    End Sub
#End Region
End Class
