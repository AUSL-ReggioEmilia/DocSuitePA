Imports System.Collections.Generic
Imports System.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Public Class TbltContainerGes
    Inherits CommonBasePage
#Region "Fields"

    Private _idUCategory As String
#End Region

#Region "Properties"
    Public ReadOnly Property IdUCategory As String
        Get
            If _idUCategory Is Nothing Then
                _idUCategory = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("CategoryID", Nothing)
            End If
            Return _idUCategory
        End Get
    End Property
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        If Not IsPostBack Then
            grdContainers.DataSource = New List(Of String)
        End If
    End Sub

End Class