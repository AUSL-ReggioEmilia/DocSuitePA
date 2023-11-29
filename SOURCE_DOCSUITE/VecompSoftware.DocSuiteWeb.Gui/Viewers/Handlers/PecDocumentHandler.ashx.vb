Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Web
Imports System.Web.SessionState
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
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
        Dim currentDocument As DocumentInfo = Nothing
        If currentPecMail.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager AndAlso Not String.IsNullOrEmpty(currentPecMail.MailContent) Then
            Dim referenceId As Guid = context.Request.QueryString.GetValue(Of Guid)("ReferenceId")
            currentPecMail = FacadeFactory.Instance.PECMailFacade.GetByUniqueId(referenceId)

            Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(currentPecMail.MailContent)
            Dim results As String = New FacadeElsaWebAPI(DocSuiteContext.Current.ProtocolEnv.DocSuiteNextElsaBaseURL).StartPreparePECMailDocumentsWorkflow(currentPecMail.UniqueId, docs)
            Dim documentInfoModel As DocumentInfoModel = docs.Single(Function(f) f.DocumentId = currentPecMail.IDMailContent)
            currentDocument = New DocumentProxyDocumentInfo(currentPecMail.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath) With {
                .DelegateExternalInitializer = New Action(Sub() DelegateElsaInitialize(currentPecMail))
            }
        Else
            currentDocument = DocumentInfoFactory.BuildDocumentInfo(context.Request.QueryString)
        End If

        If DocSuiteContext.Current.ProtocolEnv.PECMainDocumentConservationDownload AndAlso original Then
            Dim currentDocumentId As Guid = Guid.Empty
            If currentDocument.GetType().Equals(GetType(BiblosDocumentInfo)) Then
                Dim currentBiblosDocument As BiblosDocumentInfo = DirectCast(currentDocument, BiblosDocumentInfo)
                currentDocumentId = currentBiblosDocument.DocumentId
            End If
            If currentDocument.GetType().Equals(GetType(DocumentProxyDocumentInfo)) Then
                Dim currentBiblosDocument As DocumentProxyDocumentInfo = DirectCast(currentDocument, DocumentProxyDocumentInfo)
                currentDocumentId = currentBiblosDocument.DocumentId
            End If

            If currentDocumentId <> Guid.Empty AndAlso currentDocumentId.Equals(currentPecMail.IDMailContent) OrElse currentDocumentId.Equals(currentPecMail.IDPostacert) Then
                Dim mailWrapper As BiblosPecMailWrapper = New BiblosPecMailWrapper(currentPecMail, DocSuiteContext.Current.ProtocolEnv.CheckSignedEvaluateStream)
                Dim queryStringCollection As NameValueCollection = New NameValueCollection From {
                    mailWrapper.MailContent.ToQueryString(),
                    {"Original", original.ToString()},
                    {"Download", download.ToString()},
                    {"Signature", signature}
                }

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

    Protected Sub DelegateElsaInitialize(pec As PECMail)
        If pec.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager AndAlso Not String.IsNullOrEmpty(pec.MailContent) Then
            Dim str As String = New FacadeElsaWebAPI(DocSuiteContext.Current.ProtocolEnv.DocSuiteNextElsaBaseURL).StartPreparePECMailDocumentsWorkflow(pec.UniqueId, JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(pec.MailContent))
        End If
    End Sub

End Class