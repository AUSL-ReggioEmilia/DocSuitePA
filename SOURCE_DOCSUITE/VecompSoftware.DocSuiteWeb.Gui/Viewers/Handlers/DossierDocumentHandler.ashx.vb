Imports System.Web
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Web.SessionState
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.Common.OData
Imports VecompSoftware.DocSuiteWeb.Data

Namespace Viewers.Handlers

    Public Class DossierDocumentHandler
        Inherits DocumentHandler
        Implements IRequiresSessionState

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            _currentHttpContext = context
            'TODO IMPLEMENTARE CONTROLLO DIRITTI VISUALIZZAZIONE INSERTI
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