Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class FascAddUDLink
    Inherits CommonBasePage

#Region "Fields"
    Private _uniqueIdUD As Guid?
#End Region

#Region "Properties"
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        uscFascicleLink.Rights = CType(ProtocolCategoryRightPositions.Manage, Short).ToString()
        If Not IsPostBack() Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub

    Private Sub Initialize()
    End Sub

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, CommonBasePage)(key)
    End Function

    Public Overloads Function GetKeyValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function
#End Region

End Class