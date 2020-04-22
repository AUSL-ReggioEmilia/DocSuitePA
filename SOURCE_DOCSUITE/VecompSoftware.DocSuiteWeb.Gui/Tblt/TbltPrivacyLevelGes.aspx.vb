Imports System.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Class TbltPrivacyLevelGes
    Inherits CommonBasePage

#Region "Fields"

    Private _idPrivacyLevel As Guid?
#End Region

#Region "Properties"

    Protected ReadOnly Property IdPrivacyLevel As Guid?
        Get
            If _idPrivacyLevel Is Nothing Then
                _idPrivacyLevel = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Guid?)("IdPrivacyLevel", Nothing)
            End If
            Return _idPrivacyLevel
        End Get
    End Property
#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        If Not IsPostBack Then
        End If
    End Sub
#End Region

#Region "Methods"

#End Region

End Class

