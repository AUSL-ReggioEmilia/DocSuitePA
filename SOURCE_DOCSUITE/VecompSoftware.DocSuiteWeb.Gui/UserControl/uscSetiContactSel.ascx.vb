Imports Telerik.Web.UI

Public Class uscSetiContactSel
    Inherits DocSuite2008BaseControl

    Public ReadOnly Property PageContentDiv As RadButton
        Get
            Return btnOpenSetiContact
        End Get
    End Property
    Public Property MetadataAddId As String
    Public Property MetadataEditId As String
    Public Property FascicleInsertCommonIdEvent As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

End Class