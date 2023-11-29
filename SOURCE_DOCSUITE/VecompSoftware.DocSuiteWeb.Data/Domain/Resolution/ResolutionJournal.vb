''' <summary> Rappresenta un registro dei provvedimenti. </summary>
''' <remarks>Funzionalità nata per torino</remarks>
<Serializable()> _
Public Class ResolutionJournal
    Inherits AuditableDomainObject(Of Int32)
    Implements ISupportBooleanLogicDelete

#Region " Fields "
    Private _year As Integer
    Private _month As Integer
    Private _template As ResolutionJournalTemplate
    Private _iDDocument As Integer
    Private _firstPage As Integer
    Private _lastPage As Integer
    Private _iDSignedDocument As Integer?
    Private _signdate As Date?
    Private _signUser As String
    Private _resolutionJournalDetails As IList(Of ResolutionJournalDetail)
    Private _isActive As Boolean
    Private _startID As Integer?
    Private _endID As Integer?
#End Region

#Region " Constructors "
    Public Sub New()
        _resolutionJournalDetails = New List(Of ResolutionJournalDetail)
    End Sub
#End Region

#Region " Properties "
    Public Overridable Property Year As Short
        Get
            Return _year
        End Get
        Set(value As Short)
            _year = value
        End Set
    End Property
    Public Overridable Property Month As Integer
        Get
            Return _month
        End Get
        Set(value As Integer)
            _month = value
        End Set
    End Property
    Public Overridable Property Template As ResolutionJournalTemplate
        Get
            Return _template
        End Get
        Set(value As ResolutionJournalTemplate)
            _template = value
        End Set
    End Property

    ''' <summary>
    ''' Identificativo della catena documentale relativa al documento
    ''' </summary>
    Public Overridable Property IDDocument As Integer
        Get
            Return _iDDocument
        End Get
        Set(value As Integer)
            _iDDocument = value
        End Set
    End Property
    Public Overridable Property FirstPage As Integer
        Get
            Return _firstPage
        End Get
        Set(value As Integer)
            If value < 0 Then
                Throw New Exception("Invalid value for FirstPage property")
            End If
            _firstPage = value
        End Set
    End Property
    Public Overridable Property LastPage As Integer
        Get
            Return _lastPage
        End Get
        Set(value As Integer)
            _lastPage = value
        End Set
    End Property

    ''' <summary>
    ''' Identificativo della catena documentale relativa al documento firmato
    ''' </summary>
    Public Overridable Property IDSignedDocument As Integer?
        Get
            Return _iDSignedDocument
        End Get
        Set(value As Integer?)
            _iDSignedDocument = value
        End Set
    End Property
    Public Overridable Property Signdate As Date?
        Get
            Return _signdate
        End Get
        Set(value As Date?)
            _signdate = value
        End Set
    End Property
    Public Overridable Property SignUser As String
        Get
            Return _signUser
        End Get
        Set(value As String)
            _signUser = value
        End Set
    End Property
    Public Overridable Property StartID As Integer?
        Get
            Return _startID
        End Get
        Set(value As Integer?)
            _startID = value
        End Set
    End Property
    Public Overridable Property EndID As Integer?
        Get
            Return _endID
        End Get
        Set(value As Integer?)
            _endID = value
        End Set
    End Property
    Public Overridable Property ResolutionJournalDetails As IList(Of ResolutionJournalDetail)
        Get
            Return _resolutionJournalDetails
        End Get
        Set(value As IList(Of ResolutionJournalDetail))
            _resolutionJournalDetails = value
        End Set
    End Property
    Public Overridable ReadOnly Property Resolutions As List(Of Resolution)
        Get
            Dim list As New List(Of Resolution)
            For Each item As ResolutionJournalDetail In ResolutionJournalDetails
                list.Add(item.Resolution)
            Next
            Return list
        End Get
    End Property
#End Region

#Region " Methods "
    ''' <summary>
    ''' Imposta mese e anno attraverso un <see>Date</see>
    ''' </summary>
    Public Overridable Sub SetMonthYear([date] As DateTime)
        Year = [date].Year
        Month = [date].Month
    End Sub

    Public Overridable Sub AddResolution(resolution As Resolution)
        Dim detail As New ResolutionJournalDetail()
        detail.ResolutionJournal = Me
        detail.Resolution = resolution

        Me.ResolutionJournalDetails.Add(detail)
    End Sub

#End Region

#Region " ISupportBooleanLogicDelete "
    Public Overridable Property IsActive() As Boolean Implements ISupportBooleanLogicDelete.IsActive
        Get
            Return _isActive
        End Get
        Set(ByVal value As Boolean)
            _isActive = value
        End Set
    End Property

#End Region


End Class
