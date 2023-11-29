Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports VecompSoftware.Helpers.Web

''' <summary> Stampa del registro giornaliero. </summary>
<Serializable()>
Public Class ProtJournalPrint
    Inherits BasePrint

#Region " Fields "

    Private _containersName As String
    Private _idContainers As String
    Private _idStatus As Integer?
    Private _regDateFrom As Date?
    Private _regDateTo As Date?
    Private _contactList As List(Of ProtocolContactJournalDTO)
    Private _contactManualList As List(Of ProtocolContactJournalDTO)
    Private _contacts As List(Of Contact)
    Private _lastDate As Date = Nothing

#End Region

#Region " Properties "

    Public Property IdContainers() As String
        Get
            Return _idContainers
        End Get
        Set(ByVal value As String)
            _idContainers = value
        End Set
    End Property

    Public Property RegistrationDateFrom() As Date?
        Get
            Return _regDateFrom
        End Get
        Set(ByVal value As Date?)
            _regDateFrom = value
        End Set
    End Property

    Public Property RegistrationDateTo() As Date?
        Get
            Return _regDateTo
        End Get
        Set(ByVal value As Date?)
            _regDateTo = value
        End Set
    End Property

    Public Property IdStatus() As Integer?
        Get
            Return _idStatus
        End Get
        Set(ByVal value As Integer?)
            _idStatus = value
        End Set
    End Property

    Public Property ContainersName() As String
        Get
            Return _containersName
        End Get
        Set(ByVal value As String)
            _containersName = value
        End Set
    End Property

#End Region

#Region " Constructor "

    Public Sub New()
        'Inizializzo il Buffer
        TablePrint.InitializeBuffer("ProtJournalPrint")
    End Sub

#End Region

#Region "Create Rows"

    ''' <summary> Riga elenco contenitori </summary>
    Private Sub CreateContainersRow(ByRef tbl As DSTable, ByVal text As String)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.Wrap = True
        cellStyle.ColumnSpan = 9
        cellStyle.LineBox = True

        'crea riga contenitori
        tbl.CreateEmptyRow("Prnt-Tabella")
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = text
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.FlushBuffer()
    End Sub

    Private Sub CreateDataRow(ByRef tbl As DSTable, ByVal [date] As DateTime)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.ColumnSpan = 9
        cellStyle.LineBox = True

        'crea riga contenitori
        tbl.CreateEmptyRow("Prnt-Tabella")
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = "Data: " & String.Format("{0:dd/MM/yyyy}", [date])
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        tbl.FlushBuffer()
    End Sub

    Private Sub CreateProtocolRow(ByRef tbl As DSTable, ByVal protocol As Protocol)

        'stile cella

        'crea riga protocollo
        tbl.CreateEmptyRow()

        'crea cella Protocollo
        Dim text As New StringBuilder()
        text.AppendFormat("{0}<BR>{1:dd/MM/yyyy}", ProtocolFacade.ProtocolFullNumber(protocol.Year, protocol.Number), protocol.RegistrationDate.ToLocalTime())
        CreateProtocolCell(tbl, text.ToString(), Unit.Percentage(10), True, HorizontalAlign.Center, True, , True)

        'crea cella Tipo
        text = New StringBuilder(ProtocolTypeFacade.CalcolaTipoProtocollo(protocol.Type.Id))
        CreateProtocolCell(tbl, text.ToString(), Unit.Percentage(5), False, HorizontalAlign.Center, True, , True)

        'crea cella Protocollo Origine
        text = New StringBuilder(protocol.DocumentProtocol)
        text.Replace("|", "/")
        text.Append(WebHelper.Br)
        text.Append(protocol.DocumentDate.ToString())
        CreateProtocolCell(tbl, text.ToString(), Unit.Percentage(10), False, HorizontalAlign.Center, True, , True)

        'crea cella Categoria
        CreateProtocolCell(tbl, protocol.Category.Name, Unit.Percentage(25), False, HorizontalAlign.Left, True, , True)

        'Crea cella Mittenti/Destinatari
        'Contatti
        Dim p As New ProtocolContactJournalDTO.ProtocolContactJournalDTOPredicate(protocol)
        Dim contactList As List(Of ProtocolContactJournalDTO) = _contactList.FindAll(New Predicate(Of ProtocolContactJournalDTO)(AddressOf p.CompareYearAndNumberProtocol))
        text = New StringBuilder()
        GetContactHierarchyDescription(text, contactList)

        Dim contactManualList As List(Of ProtocolContactJournalDTO) = _contactManualList.FindAll(New Predicate(Of ProtocolContactJournalDTO)(AddressOf p.CompareYearAndNumberProtocol))
        For Each manual As ProtocolContactJournalDTO In contactManualList
            AttachContactDescriptionToString(text, manual.ContactDescription, manual.ContactFullPath)
        Next

        If text.Length <> 0 Then
            text.Append(protocol.AlternativeRecipient)
            If text.Length <> 0 Then
                text.Insert(0, WebHelper.Br)
            End If
        Else
            text.Append(protocol.AlternativeRecipient)
        End If
        StringHelper.ReplaceCrLf(text, " ")
        CreateProtocolCell(tbl, text.ToString(), Unit.Percentage(40), False, HorizontalAlign.Left, True, , True)

        'crea cella Stato
        If (protocol.IdStatus.HasValue) Then CreateProtocolCell(tbl, ProtocolFacade.GetStatusDescription(protocol.IdStatus.Value), Unit.Percentage(10), False, HorizontalAlign.Left, True, , True)
        tbl.FlushBuffer()

        'crea riga Oggetto
        tbl.CreateEmptyRow()
        CreateProtocolCell(tbl, String.Empty, Unit.Percentage(10), True, HorizontalAlign.Left, True)
        CreateProtocolCell(tbl, StringHelper.ReplaceCrLf(protocol.ProtocolObject), Unit.Percentage(90), False, HorizontalAlign.Left, True, 5, True)
        tbl.FlushBuffer()

        'crea riga annullamento
        If protocol.IdStatus.HasValue AndAlso protocol.IdStatus.Value = ProtocolStatusId.Annullato Then
            tbl.CreateEmptyRow()
            CreateProtocolCell(tbl, String.Empty, Unit.Percentage(10), True, HorizontalAlign.Left, True)
            text = New StringBuilder()
            text.AppendFormat("Estremi Annullamento: {0}", StringHelper.ReplaceCrLf(protocol.LastChangedReason))
            CreateProtocolCell(tbl, text.ToString(), Unit.Percentage(90), False, HorizontalAlign.Left, True, 5, True)
            tbl.FlushBuffer()
        End If
    End Sub

    Private Sub CreateProtocolHeader(ByRef tbl As DSTable)
        'crea riga instestazione protocollo
        tbl.CreateEmptyRow()
        'crea cella Protocollo
        CreateProtocolCell(tbl, "Protocollo", Unit.Percentage(10), True, HorizontalAlign.Center, True, , True)
        'crea cella Tipo
        CreateProtocolCell(tbl, "Tipo", Unit.Percentage(5), True, HorizontalAlign.Center, True, , True)
        'crea cella Protocollo Origine
        CreateProtocolCell(tbl, "P.Origin", Unit.Percentage(10), True, HorizontalAlign.Center, True, , True)
        'crea cella Categoria
        CreateProtocolCell(tbl, "Classificatore", Unit.Percentage(25), True, HorizontalAlign.Left, True, , True)
        'Crea cella Mittenti/Destinatari
        CreateProtocolCell(tbl, "Mittenti/Destinatari", Unit.Percentage(40), True, HorizontalAlign.Left, True, , True)
        'crea cella Stato
        CreateProtocolCell(tbl, "Stato", Unit.Percentage(10), True, HorizontalAlign.Left, True, , True)
        tbl.FlushBuffer()
        'crea riga Oggetto
        tbl.CreateEmptyRow()
        CreateProtocolCell(tbl, String.Empty, Unit.Percentage(10), True, HorizontalAlign.Left, True, , True)
        CreateProtocolCell(tbl, "Oggetto", Unit.Percentage(90), True, HorizontalAlign.Left, True, 5, True)
        tbl.FlushBuffer()
    End Sub
#End Region

#Region "Create Cells"

    ''' <summary> Creazione cella Protocollo </summary>
    Private Sub CreateProtocolCell(ByRef tbl As DSTable, ByVal text As String, ByVal width As Unit, ByVal bold As Boolean, ByVal alignment As HorizontalAlign, ByVal lineBox As Boolean, Optional ByVal colspan As Integer = 0, Optional ByVal wrap As Boolean = False)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = width
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = alignment
        cellStyle.LineBox = lineBox
        cellStyle.ColumnSpan = colspan
        cellStyle.Wrap = wrap

        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = text
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "IPrint Implementation"

    Public Overrides Sub DoPrint()
        CreatePrint()
    End Sub

#End Region

#Region "Retrieve Contacts to Print"
    Private Sub GetContactHierarchyDescription(ByRef s As StringBuilder, ByVal contactList As List(Of ProtocolContactJournalDTO))
        For Each contactdto As ProtocolContactJournalDTO In contactList
            Dim ids As List(Of Integer) = StringHelper.ConvertStringToList(Of Integer)(contactdto.ContactFullPath, "|"c)
            If Not String.IsNullOrEmpty(contactdto.ContactFullPath) OrElse ids IsNot Nothing Then
                ' se è presente il FullIncrementalPath nel contatto originale prelevo tutti i contatti padre
                For i As Integer = 0 To ids.Count - 1
                    CheckAndAttach(s, ids(i))
                Next
            Else
                ' se ho solo l'id del contatto procedo con l'estrazione del nominativo
                CheckAndAttach(s, contactdto.ContactId)
            End If
        Next
    End Sub

    Private Sub CheckAndAttach(ByRef str As StringBuilder, contactId As Integer)
        Dim dummy As New Contact()
        dummy.Id = contactId
        Dim p1 As New ContactPredicate(dummy)
        ' TODO: togliere sta roba
        Dim finded As Contact = _contacts.Find(New Predicate(Of Contact)(AddressOf p1.CompareId))
        If finded Is Nothing Then
            finded = Facade.ContactFacade.GetById(contactId)
            If finded Is Nothing Then
                ' In caso di database sporco, giochiamo sporco!
                Throw New DocSuiteException("Errore ricerca contatto", String.Format("Impossibile trovare il contatto [{0}].", contactId))
            End If
            _contacts.Add(finded)
        End If
        AttachContactDescriptionToString(str, finded.Description, finded.FullIncrementalPath)
    End Sub

    Private Shared Sub AttachContactDescriptionToString(ByRef str As StringBuilder, ByVal description As String, ByVal fullIncrementalPath As String)
        If str.Length <> 0 Then
            str.Append(WebHelper.Br)
        End If
        Dim spazio As String = String.Empty
        spazio = spazio.PadLeft(StringHelper.CountChar(fullIncrementalPath, "|"c), "."c)
        str.Append(spazio)
        str.Append(Replace(description, "|", " "))
    End Sub

#End Region

#Region "Private Functions"
    Private Sub CreatePrint()
        Dim protocols As IList(Of Protocol)

        'Recupero le informazioni sui protocolli
        protocols = Facade.ProtocolFacade.GetJournalPrint(IdContainers, RegistrationDateFrom, RegistrationDateTo, IdStatus)
        _contactList = Facade.ProtocolContactFacade.GetJournalPrint(IdContainers, RegistrationDateFrom, RegistrationDateTo, IdStatus)
        _contactManualList = Facade.ProtocolContactManualFacade.GetJournalPrint(IdContainers, RegistrationDateFrom, RegistrationDateTo, IdStatus)
        _contacts = New List(Of Contact)
        If protocols.Count > 25000 Then
            _contacts = Facade.ContactFacade.GetAll()
        End If

        If (protocols.Count > 0) Then
            CreateContainersRow(TablePrint, "Contenitori: " & ContainersName)
            For i As Integer = 0 To protocols.Count - 1
                CreateRow(protocols(i))
            Next
        End If
    End Sub

    Private Sub CreateRow(ByVal p As Protocol)
        ' Devo confrontare solo la componente "Data" dei campi in quanto nella DSW2010 viene salvata anche l'ora di registrazione
        If (p.RegistrationDate.ToLocalTime().Date <> _lastDate.Date) Then
            'Data
            CreateDataRow(TablePrint, p.RegistrationDate.ToLocalTime().DateTime)
            'Intestazione
            CreateProtocolHeader(TablePrint)

            _lastDate = p.RegistrationDate.ToLocalTime().DateTime
        End If
        CreateProtocolRow(TablePrint, p)
    End Sub
#End Region

End Class
