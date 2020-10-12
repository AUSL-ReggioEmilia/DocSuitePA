Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class UDSInvoiceMove
    Inherits UDSBasePage


#Region " Fields "

#End Region
#Region " Properties "

#End Region
#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim results As ICollection(Of WebAPIDto(Of Tenant)) = WebAPIImpersonatorFacade.ImpersonateFinder(New UserTenantFinder(DocSuiteContext.Current.Tenants),
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.EnablePaging = False
                        Return finder.DoSearch()
                    End Function)

            rlbSelectCompany.Items.Add(New DropDownListItem(" ", String.Empty))

            For Each item As Tenant In results.Select(Function(r) r.Entity).Where(Function(f) CurrentTenant.UniqueId <> f.UniqueId)
                rlbSelectCompany.Items.Add(New DropDownListItem(item.CompanyName, item.TenantName))
            Next
            rlbSelectCompany.SelectedIndex = 0
        End If
    End Sub
#End Region

#Region " Methods "
    Sub InitializeAjax()

    End Sub
#End Region

End Class