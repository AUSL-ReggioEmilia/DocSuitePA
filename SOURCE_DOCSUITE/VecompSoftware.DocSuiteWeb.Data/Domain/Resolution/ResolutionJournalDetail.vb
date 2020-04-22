<Serializable()> _
Public Class ResolutionJournalDetail
    Inherits DomainObject(Of Int32)

    Private _resolutionJournal As ResolutionJournal
    Private _resolution As Resolution


    Public Overridable Property ResolutionJournal() As ResolutionJournal
        Get
            Return _resolutionJournal
        End Get
        Set(ByVal value As ResolutionJournal)
            _resolutionJournal = value
        End Set
    End Property

    Public Overridable Property Resolution() As Resolution
        Get
            Return _resolution
        End Get
        Set(ByVal value As Resolution)
            _resolution = value
        End Set
    End Property

End Class
