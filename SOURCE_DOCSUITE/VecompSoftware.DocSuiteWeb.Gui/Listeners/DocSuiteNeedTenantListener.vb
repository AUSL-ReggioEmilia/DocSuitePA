Imports VecompSoftware.DocSuiteWeb.Entity.Tenants

Public Class DocSuiteNeedTenantListener
    Inherits DocSuiteBaseListener(Of Action(Of Tenant))


#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

#Region " Methods "
    Public Overrides Sub Handle(lambda As Action(Of Tenant))
        lambda(CurrentTenant)
    End Sub
#End Region

End Class
