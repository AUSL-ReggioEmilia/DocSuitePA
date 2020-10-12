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
    Public Overridable Property Password As String
    Public Overridable Property LastUsedIdCategory As Short
    Public Overridable Property LastUsedIdRecipient As Short
    Public Overridable Property LastUsedIdContainer As Short
    Public Overridable Property Version As Short
    Public Overridable Property LastUsedIdDistributionList As Short
    Public Overridable Property LastUsedIdResolution As Integer
    Public Overridable Property DomainName As String
    Public Overridable Property AlternativePassword As String
    Public Overridable Property ServiceField As String
    Public Overridable Property LastUsedIdRole As Short
    Public Overridable Property LastUsedIdRoleUser As Short
    Public Overridable Property LastUsedYearReg As Short
    Public Overridable Property LastUsedNumberReg As Integer
    Public Overridable Property IdTenantAOO As Guid? Implements ISupportTenant.IdTenantAOO

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region

    End Class

