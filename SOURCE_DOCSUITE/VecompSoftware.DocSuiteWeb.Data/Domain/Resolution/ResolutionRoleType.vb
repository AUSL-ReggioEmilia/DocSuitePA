<Serializable()> _
Public Class ResolutionRoleType
    Inherits DomainObject(Of Integer)

    Public Overridable Property Name As String

    Public Overridable Property SingleRole As Boolean

    Public Overridable Property RoleRestriction As RoleRestrictions

    Public Overridable Property RightsGuaranteed As String

    Public Overridable Property RightsAdded As String

    Public Overridable Property RightPosition As ResolutionRightPositions?

    Public Overridable Property RightDistributed As ResolutionRightPositions?

    Public Overridable Property Enabled As Boolean

    Public Overridable Property SortOrder As Integer

    Public Overridable Property CommandName As String
    
End Class