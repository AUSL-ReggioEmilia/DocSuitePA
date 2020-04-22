<Serializable()> _
Public Class ResolutionJournalTemplate
    Inherits AuditableDomainObject(Of Integer)


    Public Overridable Property IsEnabled As Boolean?
    Public Overridable Property Code As String
    Public Overridable Property Description As String
    Public Overridable Property Location As Location
    Public Overridable Property TemplateFile As String
    Public Overridable Property TemplateSource As String
    Public Overridable Property TemplateGroup As String
    Public Overridable Property Pagination As Boolean?
    Public Overridable Property MultipleSign As Boolean?
    Public Overridable Property SignatureFormat As String

    Public Overridable Property Specifications As IList(Of ResolutionJournalTemplateSpecification)

End Class