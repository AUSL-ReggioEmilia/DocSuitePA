Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports Telerik.Web.UI
Imports System.Linq
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class UserCollVersioning
    Inherits UserBasePage

#Region " Properties "

    Private ReadOnly Property idCollaboration As Integer
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer)("idCollaboration", -1)
        End Get
    End Property

#End Region

#Region " Methods "

    Private Sub initializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(gvCollVersioning, gvCollVersioning, MasterDocSuite.AjaxLoadingPanelSearch)
    End Sub

    Private Sub Initialize()
        Title = "Collaborazione non specificata - Versioning"
        If idCollaboration > 0 Then
            Title = String.Format("Collaborazione n. {0} - Versioning", idCollaboration)

            Dim versionings As IList(Of CollaborationVersioning) = Facade.CollaborationVersioningFacade.GetByCollaboration(idCollaboration)
            gvCollVersioning.PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
            gvCollVersioning.DataSource = versionings
            gvCollVersioning.Finder = New NHibernateCollaborationVersioningFinder(idCollaboration)
            gvCollVersioning.DataBind()
        End If
    End Sub

    Private Sub SetOpenDocument(ByVal collaborationIncremental As Short, ByVal incremental As Short)
        Dim collaboration As Collaboration = Facade.CollaborationFacade.GetById(idCollaboration)
        If collaboration Is Nothing Then
            Return
        End If

        Dim cv As CollaborationVersioning = collaboration.CollaborationVersioning.SingleOrDefault(Function(x) x.CollaborationIncremental.Equals(collaborationIncremental) AndAlso x.Incremental.Equals(incremental))
        Dim document As BiblosDocumentInfo = Facade.CollaborationVersioningFacade.GetBiblosDocument(cv, True)
        Dim query As String = "servername={0}&guid={1}&label={2}&ignorestate=true"
        query = String.Format(query, document.Server, document.ChainId, "CollaborationVersioning")
        Dim viewerUrl As String = "~/Viewers/BiblosViewer.aspx?" & CommonShared.AppendSecurityCheck(query)

        Response.Redirect(viewerUrl)
    End Sub

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        initializeAjax()

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub gvCollVersioning_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles gvCollVersioning.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As CollaborationVersioning = DirectCast(e.Item.DataItem, CollaborationVersioning)

        With DirectCast(e.Item.FindControl("lnkButton"), LinkButton)
            .Text = boundHeader.DocumentName
            .CommandArgument = String.Format("{0}|{1}", boundHeader.CollaborationIncremental, boundHeader.Incremental)
        End With
    End Sub

    Private Sub gvCollVersioning_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles gvCollVersioning.ItemCommand
        If e.CommandName.Eq("Docu") Then
            Dim args As String() = e.CommandArgument.ToString().Split("|"c)
            SetOpenDocument(Convert.ToInt16(args(0)), Convert.ToInt16(args(1)))
        End If
    End Sub

#End Region

End Class