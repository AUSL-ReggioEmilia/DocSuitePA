Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ProtExportResult
    Inherits ProtBasePage

#Region "Fields"
    Dim data As DateTime
    Dim moduleName As String
#End Region

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        data = Request.QueryString("Data")
        moduleName = Request.QueryString("Module")

        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Initialize"
    Private Sub Initialize()

        Dim errors As IList(Of UserError) = Facade.UserErrorFacade.SearchByModule(DocSuiteContext.Current.User.UserName, moduleName, data)

        DG.DataSource = errors
        DG.DataBind()
    End Sub
#End Region



End Class