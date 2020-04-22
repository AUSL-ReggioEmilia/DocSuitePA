Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Public Class Base
    Inherits MasterPage

#Region " Properties "

    Public ReadOnly Property ScriptManager As RadScriptManager
        Get
            Return scmMasterPage
        End Get
    End Property

    Public ReadOnly Property AjaxManager() As RadAjaxManager
        Get
            Return masterAjaxManager
        End Get
    End Property

    Public ReadOnly Property DefaultWindowManager As RadWindowManager
        Get
            Return alertManager
        End Get
    End Property


    Public ReadOnly Property AjaxDefaultLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return DefaultLoadingPanel
        End Get
    End Property


    Public ReadOnly Property AjaxFlatLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return FlatLoadingPanel
        End Get
    End Property

    <Obsolete("Evitare se possibile.")>
    Public ReadOnly Property HtmlBody() As HtmlGenericControl
        Get
            Return body
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        ' Imposto i valori della generic window
        alertManager.Width = DocSuiteContext.Current.ProtocolEnv.ModalWidth
        alertManager.Height = DocSuiteContext.Current.ProtocolEnv.ModalHeight
        alertManager.Behaviors = WindowBehaviors.Close
        alertManager.Modal = True
        alertManager.DestroyOnClose = True
        alertManager.VisibleStatusbar = False
        alertManager.CenterIfModal = True
        ' Inserisco la classe del tipo di maschera, lowercase perchè XHTML
        If Not String.IsNullOrEmpty(Request.QueryString("Type")) Then
            body.Attributes("class") = Request.QueryString("Type").ToLower()
        Else
            body.Attributes("class") = ProtBasePage.DefaultType.ToLower()
        End If
    End Sub

#End Region

End Class