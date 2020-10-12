Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class uscTenantsSelRest
    Inherits DocSuite2008BaseControl

#Region " Fields "
#End Region

#Region "Properties"

    Public ReadOnly Property CurrentTenantId As Guid
        Get
            Return CurrentTenant.UniqueId
        End Get
    End Property

    Public Property MultiselectionEnabled As Boolean
    Public ReadOnly Property PageContent As RadDropDownTree
        Get
            Return rddtTenantTree
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If MultiselectionEnabled = True Then
            rddtTenantTree.CheckBoxes = 1
        Else
            rddtTenantTree.CheckBoxes = 0
        End If
    End Sub
#End Region

#Region " Methods "
#End Region

End Class