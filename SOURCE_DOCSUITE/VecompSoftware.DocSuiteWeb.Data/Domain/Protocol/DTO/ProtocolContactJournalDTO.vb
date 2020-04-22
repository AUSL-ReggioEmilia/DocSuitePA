Public Class ProtocolContactJournalDTO

#Region "Fields"
    Private _year As Short
    Private _number As Integer
    Private _contactId As Integer
    Private _contactDescr As String
    Private _contactFullPath As String
#End Region

#Region "Properties"
    Public Overridable Property Year() As Short
        Get
            Return _year
        End Get
        Set(ByVal value As Short)
            _year = value
        End Set
    End Property

    Public Overridable Property Number() As Integer
        Get
            Return _number
        End Get
        Set(ByVal value As Integer)
            _number = value
        End Set
    End Property

    Public Overridable Property ContactId() As Integer
        Get
            Return _contactId
        End Get
        Set(ByVal value As Integer)
            _contactId = value
        End Set
    End Property

    Public Overridable Property ContactDescription() As String
        Get
            Return _contactDescr
        End Get
        Set(ByVal value As String)
            _contactDescr = value
        End Set
    End Property

    Public Overridable Property ContactFullPath() As String
        Get
            Return _contactFullPath
        End Get
        Set(ByVal value As String)
            _contactFullPath = value
        End Set
    End Property
#End Region

#Region "Predicate Class"
    Public Class ProtocolContactJournalDTOPredicate

#Region "Fields"
        Private _predContact As ProtocolContactJournalDTO
        Private _prot As Protocol
#End Region

        Public Sub New(ByVal predicateContact As ProtocolContactJournalDTO)
            _predContact = predicateContact
        End Sub

        Public Sub New(ByVal predicateProtocol As Protocol)
            _prot = predicateProtocol
        End Sub

#Region "Compare Functions"
        Public Function CompareYearAndNumber(ByVal contact As ProtocolContactJournalDTO) As Boolean
            Return (_predContact.Year = contact.Year And _predContact.Number = _predContact.Number)
        End Function

        Public Function CompareYearAndNumberProtocol(ByVal contact As ProtocolContactJournalDTO) As Boolean
            Return (_prot.Year = contact.Year And _prot.Number = contact.Number)
        End Function
#End Region

    End Class
#End Region


End Class
