Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocSuiteTenantListener
    Inherits DocSuiteBaseListener(Of ISupportTenant)

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

#Region " Methods "
    Public Overrides Sub Handle(entity As ISupportTenant)
        entity.IdTenantAOO = CurrentTenant.TenantAOO.UniqueId
    End Sub
#End Region

End Class
