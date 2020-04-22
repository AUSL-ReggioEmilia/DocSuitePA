Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtConcoursePrint
    Inherits BasePrint

#Region "Fields"
    Private _dateFrom As Nullable(Of Date)
    Private _dateTo As Nullable(Of Date)
    Private _categoryPath As String
    Private _orderType As NHibernateProtocolDao.ConcourseOrder
    Private _orderText As String
#End Region

#Region "Properties"
    Public Property DateFrom() As Nullable(Of Date)
        Get
            Return _dateFrom
        End Get
        Set(ByVal value As Nullable(Of Date))
            _dateFrom = value
        End Set
    End Property

    Public Property DateTo() As Nullable(Of Date)
        Get
            Return _dateTo
        End Get
        Set(ByVal value As Nullable(Of Date))
            _dateTo = value
        End Set
    End Property

    Public Property CategoryPath() As String
        Get
            Return _categoryPath
        End Get
        Set(ByVal value As String)
            _categoryPath = value
        End Set
    End Property

    Public Property OrderClause() As NHibernateProtocolDao.ConcourseOrder
        Get
            Return _orderType
        End Get
        Set(ByVal value As NHibernateProtocolDao.ConcourseOrder)
            _orderType = value
        End Set
    End Property

    Public Property OrderText() As String
        Get
            Return _orderText
        End Get
        Set(ByVal value As String)
            _orderText = value
        End Set
    End Property
#End Region

#Region "Create Rows"
    Private Sub CreateRowHeader(ByRef tbl As DSTable, ByVal textType As String, ByVal textCount As String)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.ColumnSpan = 7

        'crea riga Tipo ordinamento
        tbl.CreateEmptyRow()
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = textType
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        'crea riga dettaglio anagrafiche
        tbl.CreateEmptyRow()
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = textCount
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreateRowContact(ByRef tbl As DSTable, ByRef prot As Protocol)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Protocollo
        tbl.CurrentRow.CreateEmpytCell()
        CreateProtocolCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = prot.FullNumber

        ''crea cella Data
        'tbl.CurrentRow.CreateEmpytCell()
        'CreateDataCellStyle(tbl.CurrentRow.CurrentCell)
        'tbl.CurrentRow.CurrentCell.Text = prot.RegistrationDate.DateTime

        'crea celle dettaglio contatti
        For Each contact As ProtocolContact In prot.Contacts
            'aggangia dettali contatto
            AttachContactDetails(tbl, contact.Contact)
        Next

        For Each contact As ProtocolContactManual In prot.ManualContacts
            'aggangia dettali contatto
            AttachContactDetails(tbl, contact.Contact)
        Next
    End Sub

    Private Sub CreaIntestazionePartecipanti(ByRef tbl As DSTable)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Protocollo
        tbl.CurrentRow.CreateEmpytCell()
        CreateProtocolCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Protocollo"

        ''crea cella Data
        'tbl.CurrentRow.CreateEmpytCell()
        'CreateDataCellStyle(tbl.CurrentRow.CurrentCell)
        'tbl.CurrentRow.CurrentCell.Font.Bold = True
        'tbl.CurrentRow.CurrentCell.Text = "Data"

        'crea cella Nome/Cognome
        tbl.CurrentRow.CreateEmpytCell()
        CreatePersonNameCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Cognome Nome"

        'crea cella Data di nascita
        tbl.CurrentRow.CreateEmpytCell()
        CreatePersonBirthDateCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Data Nasc."

        'crea cella Indirizzo
        tbl.CurrentRow.CreateEmpytCell()
        CreatePersonAddressCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Indirizzo"

        'crea cella città
        tbl.CurrentRow.CreateEmpytCell()
        CreatePersonCityCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Località"

        'crea cella Provincia
        tbl.CurrentRow.CreateEmpytCell()
        CreatePersonCityCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Prov."

        'crea cella Provincia
        tbl.CurrentRow.CreateEmpytCell()
        CreatePersonCityCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Prov."

        'crea cella Indirizzo PEC
        tbl.CurrentRow.CreateEmpytCell()
        CreatePersonCityCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "PEC"

        'crea cella Telefono
        tbl.CurrentRow.CreateEmpytCell()
        CreatePersonCityCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Telefono"
    End Sub

    Private Sub AttachContactDetails(ByRef tbl As DSTable, ByVal contact As Contact)

        If Not String.IsNullOrEmpty(contact.Description) Then
            'crea cella Nome/Cognome
            tbl.CurrentRow.CreateEmpytCell()
            CreatePersonNameCellStyle(tbl.CurrentRow.CurrentCell)
            tbl.CurrentRow.CurrentCell.Text = Replace(contact.Description, "|", " ")

            'crea cella Data di nascita
            tbl.CurrentRow.CreateEmpytCell()
            CreatePersonBirthDateCellStyle(tbl.CurrentRow.CurrentCell)
            tbl.CurrentRow.CurrentCell.Text = String.Format("{0:dd/MM/yyyy}", contact.BirthDate)

            'crea cella Indirizzo
            If contact.Address IsNot Nothing Then
                Dim address As Address = contact.Address
                tbl.CurrentRow.CreateEmpytCell()
                CreatePersonAddressCellStyle(tbl.CurrentRow.CurrentCell)
                If address.PlaceName IsNot Nothing Then
                    tbl.CurrentRow.CurrentCell.Text = address.PlaceName.Description & " "
                End If
                tbl.CurrentRow.CurrentCell.Text &= address.Address & " " & address.CivicNumber

                'crea cella città
                tbl.CurrentRow.CreateEmpytCell()
                CreatePersonCityCellStyle(tbl.CurrentRow.CurrentCell)
                tbl.CurrentRow.CurrentCell.Text = address.ZipCode & " " & address.City

                'crea cella Provincia
                tbl.CurrentRow.CreateEmpytCell()
                CreatePersonCityCellStyle(tbl.CurrentRow.CurrentCell)
                tbl.CurrentRow.CurrentCell.Text = address.CityCode
            End If
            'crea cella Indirizzo PEC
            tbl.CurrentRow.CreateEmpytCell()
            CreateCertifiedEMailAddressCellStyle(tbl.CurrentRow.CurrentCell)
            tbl.CurrentRow.CurrentCell.Text = contact.CertifiedMail

            'crea cella Telefono
            tbl.CurrentRow.CreateEmpytCell()
            CreateCertifiedEMailAddressCellStyle(tbl.CurrentRow.CurrentCell)
            tbl.CurrentRow.CurrentCell.Text = contact.TelephoneNumber
        End If
    End Sub
#End Region

#Region "Contact Cell Styles"
    'Numero protocollo
    Private Sub CreateProtocolCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(8)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Data protocollo
    Private Sub CreateDataCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(8)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Nome/Cognome contatto
    Private Sub CreatePersonNameCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(30)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Data di nascita contatto
    Private Sub CreatePersonBirthDateCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(8)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Indirizzo contatto
    Private Sub CreatePersonAddressCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(28)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreateCertifiedEMailAddressCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(28)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Città contatto
    Private Sub CreatePersonCityCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(15)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Provincia contatto
    Private Sub CreatePersonZipCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(3)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        ' Imposto il titolo della stampa
        TitlePrint = "Elenco Partecipanti Concorso"
        Try
            Dim temp As String() = CategoryPath.Split("|"c)
            Dim categoryId As Integer = temp(temp.Length - 1)
            Dim category As Category = Facade.CategoryFacade.GetById(categoryId)
            If category IsNot Nothing Then
                Dim title As String = String.Format("Elenco Partecipanti Concorso<br />Classificazione: {0} {1}", Facade.CategoryFacade.GetCodeDotted(category), category.Name)
                TitlePrint = title
            End If
        Catch ex As Exception
            FileLogger.Warn(LogName.FileLog, "Si è verficato un errore recuperando il titolo della stampa.", ex)
        End Try

        Dim list As List(Of Protocol) = Nothing

        list = Facade.ProtocolFacade().GetProtocolForConcourse(DateFrom, DateTo, CategoryPath)
        If list IsNot Nothing AndAlso list.Count > 0 Then
            ' ordino i risultati
            Select Case OrderClause
                Case NHibernateProtocolDao.ConcourseOrder.ProtocolNumber
                    list.Sort(New ProtocolNumberComparer)
                Case NHibernateProtocolDao.ConcourseOrder.Alphabetic
                    list.Sort(New ProtocolContactComparer)
            End Select

            CreateHeader(list.Count)
            CreatePrint(list)
        End If
    End Sub
#End Region

#Region "Private Function"
    Private Sub CreateHeader(ByVal listCount As Integer)
        Dim textType As String = OrderText & " "
        If DateFrom.HasValue Then
            textType &= "Da Data " & String.Format("{0:dd/MM/yyyy}", DateFrom.Value)
        End If
        If DateTo.HasValue Then
            textType &= "A Data " & String.Format("{0:dd/MM/yyyy}", DateTo.Value)
        End If

        Dim textCount As String = "Totale N. Protocolli/Anagrafiche : " & listCount
        CreateRowHeader(TablePrint, textType, textCount)
    End Sub

    Private Sub CreatePrint(ByRef list As IList(Of Protocol))
        CreaIntestazionePartecipanti(TablePrint)
        For Each prot As Protocol In list
            ' Query nhibernate mal formattata, correggo come indicatomi
            prot = Facade.ProtocolFacade.GetById(prot.Id)
            CreateRowContact(TablePrint, prot)
        Next
    End Sub

    Public Class ProtocolNumberComparer
        Implements IComparer(Of Protocol)

        Public Function Compare(p1 As Protocol, p2 As Protocol) As Integer Implements IComparer(Of Protocol).Compare
            Return String.Compare(p1.FullNumber, p2.FullNumber)
        End Function
    End Class

    Public Class ProtocolContactComparer
        Implements IComparer(Of Protocol)

        Public Function Compare(p1 As Protocol, p2 As Protocol) As Integer Implements IComparer(Of Protocol).Compare
            Return String.Compare(p1.Contacts(0).Contact.Description, p2.Contacts(0).Contact.Description)
        End Function
    End Class

#End Region

End Class
