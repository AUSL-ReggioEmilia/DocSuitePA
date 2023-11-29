Imports Telerik.Web.UI

Public Class uscTemplateCollaborationSelRest
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "

    Public ReadOnly Property MainPanel As Control
        Get
            Return pnlMainContent
        End Get
    End Property

    Public ReadOnly Property TreeComponent As RadDropDownTree
        Get
            Return ddtDocumentType
        End Get
    End Property

    Public Property TreeViewInitializationEnabled As Boolean = True

    Public Property FilterStatus As Integer? = Nothing

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class