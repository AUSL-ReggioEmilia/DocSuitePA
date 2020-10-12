Imports NHibernate

Public Class DocSuiteTenantDefaultFilterListener
    Inherits DocSuiteBaseListener(Of ISession)

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

#Region " Methods "
    Public Overrides Sub Handle(session As ISession)
        If CurrentTenant IsNot Nothing Then
            session.EnableFilter("TenantFilter").SetParameter("tenantAOOId", CurrentTenant.TenantAOO.UniqueId)
        End If
    End Sub
#End Region

End Class
