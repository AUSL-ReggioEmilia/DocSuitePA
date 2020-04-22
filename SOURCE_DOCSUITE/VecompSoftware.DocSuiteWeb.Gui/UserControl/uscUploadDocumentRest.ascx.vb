Imports Telerik.Web.UI

Public Class uscUploadDocumentRest
    Inherits DocSuite2008BaseControl

#Region " Properties "
    Private _multipleUploadEnabled As String
    Public Property MultipleUploadEnabled() As String
        Get
            Return _multipleUploadEnabled
        End Get
        Set(ByVal value As String)
            _multipleUploadEnabled = value
        End Set
    End Property

    Public ReadOnly Property uploadDocumentComponent As RadAsyncUpload
        Get
            Return asyncUploadDocument
        End Get
    End Property
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class