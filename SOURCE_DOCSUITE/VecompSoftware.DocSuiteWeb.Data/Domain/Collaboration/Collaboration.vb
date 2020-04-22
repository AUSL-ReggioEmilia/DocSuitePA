Imports System.Linq
Imports VecompSoftware.Helpers

<Serializable()>
Public Class Collaboration
    Inherits AuditableDomainObject(Of Int32)

#Region " Fields "


#End Region

#Region " Properties "

    ''' <summary> Tipo di collaborazione </summary>
    ''' <value> Mappata su <see cref="CollaborationDocumentType"/>. </value>
    ''' <remarks> In realtà è un char sul DB </remarks>
    Public Overridable Property DocumentType As String

    ''' <summary> Priorità </summary>
    ''' <value> N-ormale B-assa A-lta </value>
    ''' <remarks> In realtà è un char sul DB </remarks>
    Public Overridable Property IdPriority As String

    ''' <summary> Stato collaborazione. </summary>
    ''' <remarks> Usare per quanto possibile <see cref="CollaborationStatusType"/>. </remarks>
    Public Overridable Property IdStatus As String

    Public Overridable Property SignCount As Short?

    Public Overridable Property MemorandumDate As Date?

    Public Overridable Property CollaborationObject As String

    Public Overridable Property Location As Location

    Public Overridable Property Note As String

    Public Overridable Property Year As Short?

    Public Overridable Property Number As Integer?

    Public Overridable Property TemplateName As String

    Public Overridable Property Resolution As Resolution

    Public Overridable Property DocumentSeriesItem As DocumentSeriesItem

    Public Overridable Property Protocol As Protocol

    Public Overridable Property PublicationUser As String

    Public Overridable Property PublicationDate As Date?

    Public Overridable Property RegistrationName As String

    Public Overridable Property RegistrationEMail As String

    Public Overridable ReadOnly Property ProtocolString() As String
        Get
            If (Year.HasValue And Number.HasValue) Then
                Return String.Format("{0}/{1:0000000}", Year.Value.ToString(), Number.Value)
            End If
            Return String.Empty
        End Get
    End Property

    Public Overridable ReadOnly Property Prosecutable As Boolean
        Get
            Dim sign As CollaborationSign = GetFirstCollaborationSignActive()
            If Not sign Is Nothing AndAlso sign.IsRequired Then
                Return sign.SignDate.HasValue
            End If
            Return True
        End Get
    End Property

    Public Overridable Property CollaborationSigns As IList(Of CollaborationSign)

    Public Overridable Property CollaborationUsers As IList(Of CollaborationUser)

    Public Overridable Property CollaborationAggregates As IList(Of CollaborationAggregate)

    Public Overridable Property CollaborationVersioning As IList(Of CollaborationVersioning)

    Public Overridable Property CollaborationLogs As IList(Of CollaborationLog)

    Public Overridable Property SourceProtocolYear As Short?

    Public Overridable Property SourceProtocolNumber As Integer?

    Public Overridable Property SourceProtocol As Protocol

    Public Overridable Property AlertDate As Date?

    Public Overridable Property IdWorkflowInstance As Guid?
    Public Overridable ReadOnly Property HasProtocol As Boolean
        Get
            Return Year.HasValue AndAlso Number.HasValue
        End Get
    End Property
    Public Overridable ReadOnly Property HasSourceProtocol As Boolean
        Get
            Return SourceProtocolYear.HasValue AndAlso SourceProtocolNumber.HasValue
        End Get
    End Property

    ''' Riferimento circolare a DLL VecompSoftware.DocSuiteWeb.Data.Entity
    ''' Public Property Desks() As ICollection(Of Desk)

#End Region

#Region " Constructors "

    Public Sub New()
        '' Riferimento circolare a DLL VecompSoftware.DocSuiteWeb.Data.Entity
        ''Desks = New Collection(Of Desk)
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

#Region " Methods "

    Public Overridable Function GetDocumentVersioning() As List(Of CollaborationVersioning)
        If CollaborationVersioning Is Nothing OrElse Not CollaborationVersioning.Any() Then
            Return Nothing
        End If

        Dim collVersList As New List(Of CollaborationVersioning)
        For Each collVers As CollaborationVersioning In CollaborationVersioning
            If collVers.CollaborationIncremental = 0 Then
                collVersList.Add(collVers)
            End If
        Next
        Const orderBy As String = "Incremental DESC"
        Dim comparer As DynamicComparer(Of CollaborationVersioning) = New DynamicComparer(Of CollaborationVersioning)(orderBy)
        collVersList.Sort(comparer)

        Return collVersList
    End Function

    Public Overridable Function GetFirstDocumentVersioning() As Integer
        Dim collVersList As List(Of CollaborationVersioning) = GetDocumentVersioning()
        If collVersList IsNot Nothing Then
            Return collVersList(0).IdDocument
        End If
        Return 0
    End Function

    Public Overridable Function GetFirstDocumentVersioningName() As String
        Dim collVersList As List(Of CollaborationVersioning) = GetDocumentVersioning()
        If collVersList IsNot Nothing Then
            Return collVersList(0).DocumentName
        End If
        Return String.Empty
    End Function

    Public Overridable Function GetFirstCollaborationSignActive() As CollaborationSign
        For Each collSign As CollaborationSign In CollaborationSigns
            If collSign.IsActive = 1 Then
                Return collSign
            End If
        Next
        Return Nothing
    End Function

    Public Overridable Function GetRequiredSigns() As IList(Of CollaborationSign)
        Dim lst As IList(Of CollaborationSign) = New List(Of CollaborationSign)
        ' Aggiunge alla lista delle firme solo quelle attive, segnate come obblsigatorie e non ancora eseguite
        For Each cs As CollaborationSign In CollaborationSigns
            If cs.IsActive AndAlso cs.IsRequired.HasValue AndAlso (cs.IsRequired.Value AndAlso Not cs.SignDate.HasValue) Then
                lst.Add(cs)
            End If
        Next

        Return lst
    End Function

    Public Overridable Sub SetSourceProtocol(sourceProtocolYear As Short?, sourceProtocolNumber As Integer?)
        If Not sourceProtocolYear.HasValue AndAlso Not sourceProtocolNumber.HasValue Then
            Exit Sub
        End If
        If {sourceProtocolYear.HasValue, sourceProtocolNumber.HasValue}.Distinct().Count() = 2 Then
            Throw New InvalidOperationException("Anno e numero del protocollo di origine devono essere entrambi valorizzati.")
        End If

        Me.SourceProtocolYear = sourceProtocolYear
        Me.SourceProtocolNumber = sourceProtocolNumber
    End Sub

    Public Overridable Sub SetSourceProtocol(ynck As YearNumberCompositeKey)
        Me.SetSourceProtocol(ynck.Year, ynck.Number)
    End Sub

    Public Overridable Sub SetSourceProtocol(protocol As Protocol)
        Me.SetSourceProtocol(protocol.Year, protocol.Number)
    End Sub

#End Region

End Class


