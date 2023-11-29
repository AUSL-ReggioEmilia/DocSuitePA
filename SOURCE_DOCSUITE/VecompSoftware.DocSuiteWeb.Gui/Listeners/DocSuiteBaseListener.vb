Imports System.Collections.Generic
Imports System.Linq
Imports System.Security.Principal
Imports System.Threading
Imports System.Web
Imports System.Web.SessionState
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Tenants

Public MustInherit Class DocSuiteBaseListener(Of T)

#Region " Fields "
    Private _tenantLock As Object = New Object
    Private _currentTenantFacade As TenantFacade
#End Region

#Region " Properties "
    Public ReadOnly Property CurrentTenantFacade As TenantFacade
        Get
            If _currentTenantFacade Is Nothing Then
                _currentTenantFacade = New TenantFacade()
            End If
            Return _currentTenantFacade
        End Get
    End Property

    Public ReadOnly Property CurrentTenant As Tenant
        Get
            Dim httpSession As HttpSessionState = HttpContext.Current.Session
            If httpSession Is Nothing Then
                Return Nothing
            End If

            If httpSession(CommonShared.USER_CURRENT_TENANT) Is Nothing Then
                httpSession.Add(CommonShared.USER_CURRENT_TENANT, CurrentTenantFacade.GetCurrentTenant())
            End If

            Return DirectCast(httpSession(CommonShared.USER_CURRENT_TENANT), Tenant)
        End Get
    End Property
#End Region

#Region " Methods "
    Public MustOverride Sub Handle(obj As T)
#End Region

End Class
