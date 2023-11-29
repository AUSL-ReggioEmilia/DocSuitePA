<Serializable()> _
Public Class Parameter
    Inherits AuditableDomainObject(Of Integer)
    Implements ISupportTenant


#Region "private data"

#End Region

#Region "Properties"

    Public Overridable Property LastUsedResolutionYear As Short
    Public Overridable Property LastUsedResolutionNumber As Short
    Public Overridable Property LastUsedBillNumber As Short
    Public Overridable Property LastUsedYear As Short
    Public Overridable Property LastUsedNumber As Integer
    Public Overridable Property Locked As Boolean
    Public Overridable Property LastUsedIdCategory As Short
    Public Overridable Property LastUsedIdContainer As Short
    Public Overridable Property LastUsedIdResolution As Integer
    Public Overridable Property LastUsedIdRole As Short
    Public Overridable Property LastUsedIdRoleUser As Short
    Public Overridable Property IdTenantAOO As Guid? Implements ISupportTenant.IdTenantAOO

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region

    End Class

