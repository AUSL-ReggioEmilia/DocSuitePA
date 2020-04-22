<Serializable()> _
Public Class ResolutionJournalTemplateSpecification
    Inherits DomainObject(Of Int32)


    Public Overridable Property Template As ResolutionJournalTemplate
    Public Overridable Property Container As Container
    Public Overridable Property ServiceCode As String
    Public Overridable Property ReslType As ResolutionType

End Class
