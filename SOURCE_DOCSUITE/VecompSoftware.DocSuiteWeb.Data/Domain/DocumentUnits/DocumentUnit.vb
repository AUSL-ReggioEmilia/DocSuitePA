<Serializable()>
Public Class DocumentUnit
    Inherits AuditableDomainObject(Of Guid)
    Implements ISupportTenant

#Region " Constructor "
    Public Sub New()
        Collaborations = New List(Of Collaboration)()
        PolRequests = New List(Of POLRequest)()
    End Sub
#End Region

#Region " Properties "
    Public Overridable Property Year As Short
    Public Overridable Property Number As Integer
    Public Overridable Property Title As String
    Public Overridable Property Environment As Integer
    Public Overridable Property DocumentUnitName As String
    Public Overridable Property Subject As String
    Public Overridable Property Status As DocumentUnitStatus
    Public Overridable Property IdTenantAOO As Guid? Implements ISupportTenant.IdTenantAOO
    Public Overridable Property IdUDSRepository As Guid?
    Public Overridable Property Category As Category
    Public Overridable Property Container As Container
    Public Overridable Property PecMails As ICollection(Of PECMail)
    Public Overridable Property Collaborations As ICollection(Of Collaboration)
    Public Overridable Property PolRequests As ICollection(Of POLRequest)
#End Region

End Class