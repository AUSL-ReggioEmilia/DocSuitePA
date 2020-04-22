Imports Telerik.Web.UI
Public Class uscMetadataRepository
    Inherits DocSuite2008BaseControl

#Region " Properties "

    Public ReadOnly Property PageContent As RadPageLayout
        Get
            Return pageContentDiv
        End Get
    End Property

#End Region


#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

#End Region


End Class