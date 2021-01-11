Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class DossierInserimento
    Inherits CommonBasePage

#Region " Fields "
#End Region

#Region " Properties "
    Public ReadOnly Property DefaultCategory As Category
        Get
            Return Facade.CategoryFacade.GetRootAOOCategory(CurrentTenant.TenantAOO.UniqueId)
        End Get
    End Property
#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
    End Sub
#End Region

#Region " Methods "
#End Region

End Class