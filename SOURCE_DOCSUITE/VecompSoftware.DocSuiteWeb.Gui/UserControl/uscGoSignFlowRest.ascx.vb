Imports VecompSoftware.DocSuiteWeb.Data

Public Class uscGoSignFlowRest
    Inherits DocSuite2008BaseControl

    Public ReadOnly Property InfocertProxySignUrl As String
        Get
            Return DocSuiteContext.Current.InfocertProxySignUrl
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class