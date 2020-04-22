
<Serializable()> _
Public Class DocumentSeriesItemRole
    Inherits DomainObject(Of Int32)

    Public Overridable Property UniqueIdDocumentSeriesItem As Guid

    Public Overridable Property Item As DocumentSeriesItem
    Public Overridable Property Role As Role
    Public Overridable Property LinkType As DocumentSeriesItemRoleLinkType

End Class