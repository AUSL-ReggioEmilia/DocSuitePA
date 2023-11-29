Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class TemplateUserCollCartellaGestione
    Inherits UserBasePage

    Private Const INSERT_ACTION As String = "Insert"
    Private Const EDIT_ACTION As String = "Edit"

    ''' <summary>
    ''' Query parameter which will determine the parent (folder or fixed template) of the current template being created.
    ''' Important: This parameter is used only when creating a new template, editing the parent is not possible
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ParentId As Guid?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("ParentId")) Then
                Return Guid.Parse(Request.QueryString("ParentId"))
            End If
            Return Nothing
        End Get
    End Property

    ''' <summary>
    ''' Query paremeter representing the template being edited.
    ''' Important: This parameter is used only when editing a template, creating a new template will not receive a template id parameter
    ''' </summary>
    Public ReadOnly Property TemplateId As Guid?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("TemplateId")) Then
                Return Guid.Parse(Request.QueryString("TemplateId"))
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub Initialize()
        If Not CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateCollaborationGroups) AndAlso Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Utente non abilitato alla gestione del template di Collaborazione")
        End If

        If String.IsNullOrEmpty(Action) OrElse Not (Action.Eq(INSERT_ACTION) OrElse Action.Eq(EDIT_ACTION)) Then
            Throw New DocSuiteException("Action type non corretto")
        End If
    End Sub

End Class