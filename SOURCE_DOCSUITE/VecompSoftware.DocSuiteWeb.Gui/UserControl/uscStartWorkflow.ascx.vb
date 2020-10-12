Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports DSW = VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports System.Diagnostics
Imports System.Reflection

Partial Public Class uscStartWorkflow
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const SET_RECIPIENT_PROPERTIES As String = "SetRecipientProperties"
    Private Const UPDATE_CALLBACK As String = "uscStartWorkflow.updateCallback()"

    Private _environment As DSWEnvironment?
#End Region

#Region " Properties "
    Protected ReadOnly Property DSWVersion As String
        Get
            Return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
        End Get
    End Property

    Public Property Environment As DSWEnvironment
        Get
            If _environment Is Nothing Then
                _environment = DSWEnvironment.Any
            End If
            Return CType(_environment, DSWEnvironment)
        End Get
        Set(value As DSWEnvironment)
            _environment = value
        End Set
    End Property

    Public ReadOnly Property PageContent As Control
        Get
            Return pnlWorkflowStart
        End Get
    End Property

    Public Property TenantName As String
    Public Property TenantId As String
    Public Property ShowOnlyNoInstanceWorkflows As Boolean
    Public Property ControlReadOnlyEnable As Boolean
    Public Property ShowOnlyHasIsFascicleClosedRequired As Boolean
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            rgvDocumentLists.DataSource = New List(Of WorkflowReferenceBiblosModel)
        End If
    End Sub

    Protected Sub uscStartWorkflow_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, SET_RECIPIENT_PROPERTIES) Then
            If ajaxModel.Value.Count > 0 AndAlso CBool(ajaxModel.Value(0)) Then
                Session.Add("MultipleContacts", CBool(ajaxModel.Value(0)))
            Else
                Session.Remove("MultipleContacts")
            End If
        End If
        AjaxManager.ResponseScripts.Add(UPDATE_CALLBACK)
    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscStartWorkflow_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlWorkflowRepository, ddlWorkflowRepository)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadDocumentRest)
    End Sub
#End Region

End Class
