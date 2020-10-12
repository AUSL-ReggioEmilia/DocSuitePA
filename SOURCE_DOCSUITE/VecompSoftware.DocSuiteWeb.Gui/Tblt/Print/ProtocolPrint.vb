Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web

Public Class ProtocolPrint
    Inherits BasePrint

#Region "Fields"
    Private _listid As New List(Of Guid)
#End Region

#Region "properties"

    Public Property ListId() As List(Of Guid)
        Get
            Return _listid
        End Get
        Set(ByVal value As List(Of Guid))
            _listid = value
        End Set
    End Property


#End Region


    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa Elenco Selezionati"
        Stampa()
    End Sub

#Region "Private Methods"

    Private Sub Contatti(ByVal contacts As Object, ByRef s As String)

        Dim spazio As String = ""

        If TypeOf (contacts) Is IList(Of ProtocolContactManual) Then

            For Each contatto As ProtocolContactManual In contacts
                If s <> "" Then
                    s &= "<BR>"
                End If
                spazio = spazio.PadLeft(contatto.Contact.Level, "."c)
                s &= spazio & Facade.ContactFacade.FormatContact(contatto.Contact)
                If Not contatto.Contact.Children Is Nothing Then Contatti(contatto.Contact.Children, s)
            Next

        ElseIf TypeOf (contacts) Is Contact Then
            Dim contact As Contact = CType(contacts, Contact)
            Dim parent As Contact = contact.Parent
            If parent IsNot Nothing Then
                Contatti(parent, s)
            End If

            If s <> "" Then
                s &= "<BR>"
            End If
            spazio = spazio.PadLeft(contact.Level, "."c)
            s &= spazio & Facade.ContactFacade.FormatContact(contact)

        ElseIf TypeOf (contacts) Is IList(Of ProtocolContact) Then

            For Each contatto As ProtocolContact In contacts
                Dim parent As Contact = contatto.Contact.Parent
                If parent IsNot Nothing Then
                    Contatti(parent, s)
                End If

                If s <> "" Then
                    s &= "<BR>"
                End If
                spazio = spazio.PadLeft(contatto.Contact.Level, "."c)
                s &= spazio & Facade.ContactFacade.FormatContact(contatto.Contact)
            Next

        End If

    End Sub

    Private Sub Stampa()
        Dim type As ArrayList
        Dim row As ArrayList
        Dim s As String = String.Empty

        'riga1
        row = New ArrayList()
        row.Add("Stampa dei Protocolli")

        type = New ArrayList()
        type.Add("100-B-L-8")

        CreateRow(TablePrint, row, type, "tabella", 8)

        'riga2

        row = New ArrayList()
        row.Add("P. Numero")
        row.Add("Tipo")
        row.Add("P. Doc.")
        row.Add("Data Doc.")
        row.Add("Oggetto")
        row.Add("Classificatore")
        row.Add("Mitt/Dest")
        row.Add("Stato")

        type = New ArrayList()
        type.Add("8-B-C-O")
        type.Add("5-B-L-O")
        type.Add("8-B-L-O")
        type.Add("8-B-C-O")
        type.Add("36-B-L-O")
        type.Add("10-B-L-O")
        type.Add("20-B-L-O")
        type.Add("5-B-L-O")

        CreateRow(TablePrint, row, type, "", 0)


        For Each id As Guid In ListId
            Dim protocol As Protocol = Facade.ProtocolFacade.GetById(id)

            row = New ArrayList()
            row.Add(protocol.Id.ToString() & WebHelper.Br & String.Format("{0:dd/MM/yyyy}", protocol.RegistrationDate.ToLocalTime()))
            row.Add(ProtocolTypeFacade.CalcolaTipoProtocollo(protocol.Type.Id))
            row.Add(If(Not String.IsNullOrEmpty(protocol.DocumentProtocol), Replace(protocol.DocumentProtocol, "|", "!"), WebHelper.Space))
            row.Add(protocol.DocumentDate)
            row.Add(protocol.ProtocolObject)
            row.Add(protocol.Category.Name)

            type = New ArrayList()
            type.Add("8-N-C-O")
            type.Add("5-N-L-W")
            type.Add("8-N-L-O")
            type.Add("8-N-C-O")
            type.Add("36-N-L-W")
            type.Add("10-N-L-W")
            type.Add("20-N-L-W")
            type.Add("5-N-L-O")


            'da rubrica
            s = ""
            If protocol.Contacts.Count > 0 Then
                Contatti(protocol.Contacts, s)
            End If

            'manuali
            If protocol.ManualContacts.Count > 0 Then
                Contatti(protocol.ManualContacts, s)
            End If

            'Recipient
            If protocol.Recipients.Count > 0 Then

                For Each contatto As Recipient In protocol.Recipients
                    If Not String.IsNullOrEmpty(s) Then
                        s &= "<BR>"
                    End If
                    s &= StringHelper.ReplaceCrLf(contatto.FullName)
                Next
            End If

            If Not String.IsNullOrEmpty(s) Then
                Dim s1 As String = StringHelper.ReplaceCrLf(protocol.AlternativeRecipient)
                If Not String.IsNullOrEmpty(s1) Then
                    s &= "<BR>" & s1
                End If
            Else
                s &= StringHelper.ReplaceCrLf(protocol.AlternativeRecipient)
            End If

            row.Add(s)


            '--Stato
            If protocol.IdStatus.HasValue Then row.Add(ProtocolFacade.GetStatusDescription(protocol.IdStatus.Value))
            CreateRow(TablePrint, row, type, "", 0)
        Next

    End Sub

    Private Sub CreateRow(ByRef table As DSTable, ByVal title As ArrayList, ByVal type As ArrayList, ByVal css As String, ByVal span As Integer)

        Dim cellstyle As DSTableCellStyle
        Dim bold As String
        Dim size As String
        Dim alignH As String
        Dim alignV As String

        table.CreateEmptyRow(css)

        For i As Integer = 0 To title.Count - 1


            Dim app() As String = CType(type(i), String).Split("-"c)

            size = app(0)
            bold = app(1)
            alignH = app(2)
            alignV = app(3)

            table.CurrentRow.CreateEmpytCell()
            table.CurrentRow.CurrentCell.Text = title(i)

            cellstyle = New DSTableCellStyle()
            If span <> 0 Then
                cellstyle.ColumnSpan = span
            End If
            cellstyle.Font.Bold = If(bold = "B", True, False)
            cellstyle.Width = Unit.Percentage(If(size <> 0, size, 0))
            cellstyle.LineBox = True

            Select Case alignH
                Case "L" : cellstyle.HorizontalAlignment = HorizontalAlign.Left
                Case "R" : cellstyle.HorizontalAlignment = HorizontalAlign.Right
                Case "C" : cellstyle.HorizontalAlignment = HorizontalAlign.Center
            End Select

            Select Case alignV
                Case "T" : cellstyle.VerticalAlignment = VerticalAlign.Top
                Case "M" : cellstyle.VerticalAlignment = VerticalAlign.Middle
                Case "B" : cellstyle.VerticalAlignment = VerticalAlign.Bottom
                Case "W" : cellstyle.Wrap = True
            End Select

            table.CurrentRow.CurrentCell.ApplyStyle(cellstyle)

        Next

    End Sub

#End Region

End Class
