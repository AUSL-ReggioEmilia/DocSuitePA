Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DossierVisualizza
    Inherits DossierBasePage


#Region " Fields "

    Private Const DOSSIER_INITIALIZE_CALLBACK As String = "dossierVisualizza.initializeCallback()"
    Private _location As Location
#End Region

#Region " Properties "

    Public ReadOnly Property IsLogEnabled As Boolean
        Get
            Return ProtocolEnv.IsLogEnabled
        End Get
    End Property

    Public ReadOnly Property IsMiscellaneaLocationEnabled As Boolean
        Get
            Return HasMiscellaneaLocation IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property HasMiscellaneaLocation As Location
        Get
            If _location Is Nothing Then
                _location = Facade.LocationFacade.GetById(ProtocolEnv.DossierMiscellaneaLocation)
            End If
            Return _location
        End Get
    End Property

    Protected ReadOnly Property IdWorkflowActivity As String
        Get
            Dim _idWorkflowActivity As String = Request.QueryString.GetValueOrDefault("IdWorkflowActivity", String.Empty)
            Return _idWorkflowActivity
        End Get
    End Property

    Protected ReadOnly Property WorkflowEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.WorkflowManagerEnabled
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
        End If
    End Sub

    Protected Sub DossierVisualizzaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "Initialize"
                Initialize()
        End Select
    End Sub

#End Region


#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AddHandler AjaxManager.AjaxRequest, AddressOf DossierVisualizzaAjaxRequest
    End Sub

    Private Sub Initialize()
    End Sub

#End Region



End Class