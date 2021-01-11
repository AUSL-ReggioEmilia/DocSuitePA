<Serializable()>
Public Class FascicleDocumentUnit
    Inherits AuditableDomainObject(Of Guid)

#Region "Properties"

    Public Overridable Property ReferenceType As ReferenceType
    Public Overridable Property IdDocumentUnit As Guid
    Public Overridable Property Fascicle As Fascicle
    Public Overridable Property IdFascicleFolder As Guid

#End Region

#Region "Ctor/init"
    Public Sub New()
    End Sub
#End Region

End Class


