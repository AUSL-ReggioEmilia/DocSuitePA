Imports System.Web
Imports System.Web.SessionState


Namespace Viewers.Handlers

    Public Class CollaborationDocumentHandler
        Inherits DocumentHandler
        Implements IRequiresSessionState

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            ElaborateDocument(context)
        End Sub

        Protected Overrides Function CheckRight() As Boolean
            'TODO: implementare logica specifica
            Return True
        End Function
        Protected Overrides Function CheckPrivacyRight() As Boolean
            'TODO: implementare logica specifica
            Return True
        End Function

    End Class

End Namespace