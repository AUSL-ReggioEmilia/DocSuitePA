Imports System.Collections.Specialized
Imports System.Web
Imports System.Web.Services
Imports System.Web.SessionState
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers.Handlers
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class PecDocumentHandler
    Inherits DocumentHandler
    Implements IRequiresSessionState

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        Dim original As Boolean = context.Request.QueryString.GetValueOrDefault(Of Boolean)("Original", False)
        Dim download As Boolean = context.Request.QueryString.GetValueOrDefault(Of Boolean)("Download", False)
        Dim signature As String = context.Request.QueryString.GetValueOrDefault(Of String)("Signature", String.Empty)

        Dim parentId As Integer = context.Request.QueryString.GetValue(Of Integer)("parent")

        Dim currentPecMail As PECMail = FacadeFactory.Instance.PECMailFacade.GetById(parentId)
        Dim currentDocument As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(context.Request.QueryString)

        If currentDocument.GetType().Equals(GetType(BiblosDocumentInfo)) AndAlso DocSuiteContext.Current.ProtocolEnv.PECMainDocumentConservationDownload AndAlso original Then
            Dim currentBiblosDocument As BiblosDocumentInfo = DirectCast(currentDocument, BiblosDocumentInfo)
            If currentBiblosDocument.DocumentId.Equals(currentPecMail.IDMailContent) OrElse currentBiblosDocument.DocumentId.Equals(currentPecMail.IDPostacert) Then
                Dim mailWrapper As BiblosPecMailWrapper = New BiblosPecMailWrapper(currentPecMail, DocSuiteContext.Current.ProtocolEnv.CheckSignedEvaluateStream)
                Dim queryStringCollection As NameValueCollection = New NameValueCollection()
                queryStringCollection.Add(mailWrapper.MailContent.ToQueryString())
                queryStringCollection.Add("Original", original.ToString())
                queryStringCollection.Add("Download", download.ToString())
                queryStringCollection.Add("Signature", signature)

                context.Response.Redirect(String.Format("{0}/Viewers/Handlers/DocumentInfoHandler.ashx/{1}?{2}", DocSuiteContext.Current.CurrentTenant.DSWUrl, FileHelper.FileNameToUrl(mailWrapper.MailContent.Name),
                                                        CommonShared.AppendSecurityCheck(queryStringCollection.AsEncodedQueryString())))
                Exit Sub
            End If
        End If

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