Imports System.Web
Imports System.Web.SessionState
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Namespace Viewers.Handlers
    Public Class CollaborationDocumentHandler
        Inherits DocumentHandler
        Implements IRequiresSessionState

        Private _idCollaboration As Integer

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            _idCollaboration = context.Request.QueryString.GetValue(Of Integer)("parent")
            ElaborateDocument(context)
        End Sub

        Protected Overrides Function CheckRight() As Boolean
            Return CurrentODataFacade.HasCollaborationViewableRight(_idCollaboration)
        End Function
        Protected Overrides Function CheckPrivacyRight() As Boolean
            'TODO: implementare logica specifica
            Return True
        End Function

    End Class

End Namespace