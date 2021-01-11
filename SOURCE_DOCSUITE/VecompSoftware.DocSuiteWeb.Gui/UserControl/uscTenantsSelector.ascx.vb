Imports Telerik.Web.UI

Public Class uscTenantsSelector
    Inherits DocSuite2008BaseControl

    Public ReadOnly Property RadWindowTenantSelector As RadWindow
        Get
            Return rwTenantSelector
        End Get
    End Property
    Public ReadOnly Property PecMailBoxCombo As RadComboBox
        Get
            Return cmbSelectPecMailBox
        End Get
    End Property
    Public ReadOnly Property btnAvviaFlusso As RadButton
        Get
            Return btnContainerSelectorOk
        End Get
    End Property
    Public ReadOnly Property WorkflowCombo As RadComboBox
        Get
            Return cmbWorkflowRepositories
        End Get
    End Property
    Public ReadOnly Property TenantsCombo As RadComboBox
        Get
            Return cmbSelectAzienda
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

End Class