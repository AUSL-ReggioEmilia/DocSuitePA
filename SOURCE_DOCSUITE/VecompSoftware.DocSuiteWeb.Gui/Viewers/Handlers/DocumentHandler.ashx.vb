Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.ServiceModel
Imports System.Text
Imports System.Web
Imports System.Web.Hosting
Imports iText.Kernel.Pdf
Imports iText.Kernel.Utils
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.Common.OData
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.LimilabsMail
Imports VecompSoftware.Helpers.PDF
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.DocumentsService
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports SC = VecompSoftware.Services.StampaConforme

Namespace Viewers.Handlers

    Public MustInherit Class DocumentHandler
        Implements IHttpHandler

#Region " Fields "
        Private _facadeFactory As FacadeFactory
        Protected _currentHttpContext As HttpContext = Nothing
        Private _currentODataFacade As New ODataFacade()
        Private _documentUnit As Entity.DocumentUnits.DocumentUnit
        Private _checkPrivacy As Boolean?
        Private Const ERROR_TEMPLATE_NAME As String = "error.pdf"
        Private Const PRIVACY_ERROR_TEMPLATE_NAME As String = "privacy_error.pdf"
        Private Const RIGHTS_ERROR_TEMPLATE_PATH As String = "~/ExceptionTemplate/rights_error.pdf"
        Private Const RIGHTS_ERROR_TEMPLATE_NAME As String = "rights_error.pdf"
        Private Const PARER_ERROR_TEMPLATE_NAME As String = "parer_error.pdf"
        Private Const PARER_ERROR_TEMPLATE_PATH As String = "~/ExceptionTemplate/parer_error.pdf"
        Private Const StampaConformeLog As String = "StampaConforme.FileLogger"
#End Region

#Region " Properties "


        Protected ReadOnly Property CurrentDocumentUnit As Entity.DocumentUnits.DocumentUnit
            Get
                If _documentUnit Is Nothing AndAlso UniqueId.HasValue AndAlso UniqueId.Value <> Guid.Empty Then
                    _documentUnit = WebAPIImpersonatorFacade.ImpersonateFinder(New DocumentUnitFinder(DocSuiteContext.Current.Tenants),
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.EnablePaging = False
                                finder.IdDocumentUnit = UniqueId.Value
                                finder.ExpandContainer = True
                                finder.ExpandRoles = ExpandDocumentUnitRoles
                                If Environment.HasValue AndAlso Environment >= 100 Then
                                    finder.ExpandUDSRepository = True
                                End If
                                Return finder.DoSearch().Select(Function(f) f.Entity).SingleOrDefault()
                            End Function)
                End If
                Return _documentUnit
            End Get
        End Property

        Protected ReadOnly Property UniqueId As Guid?
            Get
                If CurrentHttpContext IsNot Nothing Then
                    Return CurrentHttpContext.Request.QueryString.GetValueOrDefault(Of Guid)("UniqueId", Nothing)
                End If
                Return Nothing
            End Get
        End Property

        Protected ReadOnly Property Environment As Integer?
            Get
                If CurrentHttpContext IsNot Nothing Then
                    Return CurrentHttpContext.Request.QueryString.GetValueOrDefault(Of Integer)("Environment", Nothing)
                End If
                Return Nothing
            End Get
        End Property

        Protected ReadOnly Property IsPublic As Boolean
            Get
                If CurrentHttpContext IsNot Nothing Then
                    Return CurrentHttpContext.Request.QueryString.GetValueOrDefault("IsPublic", False)
                End If
                Return False
            End Get
        End Property

        Protected ReadOnly Property Miscellanea As Boolean
            Get
                If CurrentHttpContext IsNot Nothing Then
                    Return CurrentHttpContext.Request.QueryString.GetValueOrDefault("Miscellanea", False)
                End If
                Return False
            End Get
        End Property

        Protected ReadOnly Property DocumentName As String
            Get
                If CurrentHttpContext IsNot Nothing Then
                    Return CurrentHttpContext.Request.QueryString.GetValueOrDefault(Of String)("Name", String.Empty)
                End If
                Return String.Empty
            End Get
        End Property

        Protected ReadOnly Property DocumentGuid As Guid?
            Get
                If CurrentHttpContext IsNot Nothing Then
                    Return CurrentHttpContext.Request.QueryString.GetValueOrDefault(Of Guid)("Guid", Nothing)
                End If
                Return Guid.Empty
            End Get
        End Property

        Protected ReadOnly Property DocumentIdGuid As Guid?
            Get
                If CurrentHttpContext IsNot Nothing Then
                    Return CurrentHttpContext.Request.QueryString.GetValueOrDefault(Of Guid)("DocumentId", Nothing)
                End If
                Return Guid.Empty
            End Get
        End Property


        Public ReadOnly Property CurrentHttpContext As HttpContext
            Get
                Return _currentHttpContext
            End Get
        End Property

        Overridable ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        Protected Property IdContainer As Integer?

        Protected Property PrivacyRoles As Guid()

        Protected Property PublicRoles As Guid()

        Protected Property ExpandDocumentUnitRoles As Boolean

        Protected Property FromFascicle As Boolean

        Protected ReadOnly Property FacadeFactory As FacadeFactory
            Get
                If _facadeFactory Is Nothing Then
                    _facadeFactory = New FacadeFactory()
                End If
                Return _facadeFactory
            End Get
        End Property

        Public ReadOnly Property CurrentODataFacade As ODataFacade
            Get
                If _currentODataFacade Is Nothing Then
                    _currentODataFacade = New ODataFacade()
                    Return _currentODataFacade
                End If
                Return _currentODataFacade
            End Get
        End Property

        Private ReadOnly Property ValidTypes As Integer()
            Get
                Return New Integer() {DSWEnvironment.Protocol, DSWEnvironment.Resolution, DSWEnvironment.DocumentSeries, DSWEnvironment.UDS}
            End Get
        End Property

        Protected Property CheckDocumentUnitExistence As Boolean

        Protected Property IsUserAuthorized As Boolean

#End Region

#Region " Methods "

        Overridable Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
            _currentHttpContext = context
        End Sub

        Protected MustOverride Function CheckRight() As Boolean
        Protected MustOverride Function CheckPrivacyRight() As Boolean

        Protected Overridable Sub LogView(documentName As String, guid As Guid)
        End Sub

        Protected Sub ElaborateDocument(context As HttpContext)

            Dim original As Boolean = context.Request.QueryString.GetValueOrDefault("Original", False)
            Dim biblosVersion As String = context.Request.QueryString.GetValueOrDefault("BiblosVersion", String.Empty)
            Dim idDocument As Guid = context.Request.QueryString.GetValueOrDefault("Guid", Guid.Empty)
            Dim download As Boolean = context.Request.QueryString.GetValueOrDefault("Download", False)
            Dim documentSignature As String = HttpUtility.HtmlDecode(context.Request.QueryString.GetValueOrDefault("Signature", String.Empty))
            Dim isInvoice As Boolean = context.Request.QueryString.GetValueOrDefault("IsInvoice", False)
            Dim invoiceKind As XMLModelKind? = context.Request.QueryString.GetValueOrDefault(Of XMLModelKind?)("InvoiceKind", Nothing)
            Dim invoiceStylePosition As Integer? = context.Request.QueryString.GetValueOrDefault(Of Integer?)("InvoiceStylePosition", Nothing)
            Dim stringToExposeInPage As String = String.Empty
            Dim doc As DocumentInfo = Nothing

            Try
                If CheckDocumentUnitExistence AndAlso Not IsPublic AndAlso CurrentDocumentUnit Is Nothing AndAlso UniqueId.HasValue AndAlso Environment.HasValue AndAlso ValidTypes.Contains(Environment.Value) Then
                    FileLogger.Warn(LogName.FileLog, String.Concat("Unità documentaria con id ", UniqueId.Value, " non trovata"))
                    ElaborateDocumentUnitException(context, download)
                    Exit Sub
                End If

                If Not IsPublic AndAlso Not CheckRight() Then
                    FileLogger.Error(LogName.FileLog, "Errore in fase di visualizzazione documento: non si possiedono i diritti di visualizzazione del documento richiesto")
                    ElaborateRightsException(context, download)
                    Exit Sub
                End If

                If (context.Session IsNot Nothing AndAlso context.Session("DocumentHandlerError") IsNot Nothing) Then
                    context.Session.Remove("DocumentHandlerError")
                End If

                If String.IsNullOrEmpty(biblosVersion) Then
                    doc = DocumentInfoFactory.BuildDocumentInfo(context.Request.QueryString)
                Else
                    Dim version As Decimal = CType(biblosVersion, Decimal)
                    doc = BiblosDocumentInfo.GetDocumentInfo(idDocument, version, False).FirstOrDefault()
                End If

                If doc.IsRemoved AndAlso doc Is GetType(BiblosDeletedDocumentInfo) Then
                    FileLogger.Warn(LogName.FileLog, "Il documento non è più presente negli archivi Biblos")
                    ElaborateDocumentDeletedException(context, download)
                    Exit Sub
                End If

                If Not CheckPrivacy(doc, download, context) Then
                    Exit Sub
                End If

                ' Aggiunta della signature di un documento
                doc.Signature = String.Concat(documentSignature, doc.Signature)
                If original Then
                    If Not Path.GetExtension(doc.Name).Eq(FileHelper.PDF) AndAlso Not download Then
                        Throw New InvalidOperationException("I documenti originali sono disponibili solo per il download.")
                    End If
                End If

                Dim name As String = String.Empty
                Dim stream() As Byte = Nothing
                If original Then
                    name = doc.Name
                    stream = doc.Stream
                Else
                    If DocSuiteContext.Current.ProtocolEnv.InvoiceSDIEnabled AndAlso isInvoice AndAlso invoiceKind.HasValue AndAlso invoiceStylePosition.HasValue Then
                        Dim kindStyleResources As InvoiceResources = DocSuiteContext.InvoiceResources.FirstOrDefault(Function(x) x.InvoiceKind = invoiceKind.Value)
                        If kindStyleResources IsNot Nothing Then
                            Dim xsl As String = kindStyleResources.Stylesheets.ElementAt(invoiceStylePosition.Value).Value
                            GetInvoicePdfStream(doc, xsl, stream, name)
                        Else
                            GetPdfStream(doc, stream, name)
                        End If
                    Else
                        GetPdfStream(doc, stream, name)
                    End If

                    ' Test coerenza PDF
                    If Not Encoding.Default.GetString(stream, 0, 4).Eq("%PDF") Then
                        Throw New Exception("Il pdf non può essere visualizzato a causa di un errore di conversione.", New Exception("Lo stream non inizia con '%PDF'"))
                    End If
                End If

                If download Then
                    DownloadStream(context, stream, doc.DownloadFileName)
                Else
                    ElaborateStream(context, stream, name)
                End If
                LogView(CStr(IIf(String.IsNullOrEmpty(DocumentName), name, DocumentName)), DocumentGuid.Value)
            Catch ex As Exception
                FileLogger.Error(LogName.FileLog, "Errore in fase di visualizzazione documento", ex)

                If doc IsNot Nothing AndAlso doc.Extension.Eq(FileHelper.PDF) Then
                    stringToExposeInPage = doc.PDFName
                End If
                If context.Session IsNot Nothing Then
                    context.Session("DocumentHandlerError") = stringToExposeInPage
                End If
                Try
                    ElaborateException(context, ex, download, doc)
                Catch ex_ex As Exception
                    FileLogger.Error(LogName.FileLog, "Errore in fase di visualizzazione del documento di errore", ex_ex)
                    Throw
                End Try
            End Try

        End Sub

        Private Function ToPdfStream(doc As DocumentInfo, ByRef name As String, toPdfAction As Func(Of DocumentInfo, Byte())) As Byte()
            Dim biblosDoc As BiblosDocumentInfo = TryCast(doc, BiblosDocumentInfo)

            If biblosDoc IsNot Nothing Then
                FileLogger.Debug(StampaConformeLog, String.Format("Inizio conversione in pdf del documento biblos: {0} (Id Biblos: {1})", biblosDoc.Name, biblosDoc.DocumentId))
            Else
                FileLogger.Debug(StampaConformeLog, String.Format("Inizio conversione in pdf del documento temporaneo: {0}", doc.Name))
            End If

            If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.StampaConformeExtensions) Then
                ' Verifico se il documento può essere convertito in PDF
                Dim extension As String = Path.GetExtension(doc.Name)
                If Not StringHelper.ExistsIn(DocSuiteContext.Current.ProtocolEnv.StampaConformeExtensions, extension, "|"c, "") Then
                    name = "Conversione non possibile.pdf"
                    Using stream As New MemoryStream()
                        FacadeFactory.Instance.BiblosFacade.GetConversionNotAllowedPdf(doc, stream)
                        Return stream.ToArray()
                    End Using
                End If
            End If

            ' Se la size dell'eml è inferiore ad 1Mb non è conveniente
            If Not DocSuiteContext.Current.ProtocolEnv.DisableRemoveAttachments _
                AndAlso FileHelper.MatchExtension(doc.Name, FileHelper.EML) _
                AndAlso doc.Size > (DocSuiteContext.Current.ProtocolEnv.SizeThresholdRemoveAttachments) Then
                doc.Stream = LimilabsMailHelper.RemoveAttachments(doc.Stream)
            End If

            name = doc.PDFName
            Return toPdfAction(doc)
        End Function

        Private Sub GetInvoicePdfStream(doc As DocumentInfo, xsl As String, ByRef data() As Byte, ByRef name As String)
            data = ToPdfStream(doc, name, Function(d As DocumentInfo)
                                              Return SC.Service.ToRasterXmlWithStylesheet(doc.Stream, xsl, doc.ExtensionOrName, doc.Signature)
                                          End Function)
        End Sub

        Private Sub GetPdfStream(doc As DocumentInfo, ByRef data() As Byte, ByRef name As String)
            Dim signerModels As String = String.Empty
            If TypeOf doc Is BiblosDocumentInfo AndAlso doc.Attributes.ContainsKey(BiblosFacade.SING_MODELS_ATTRIBUTE) Then
                signerModels = doc.Attributes(BiblosFacade.SING_MODELS_ATTRIBUTE).ToString()
            End If
            data = ToPdfStream(doc, name, Function(d As DocumentInfo) SC.Service.ConvertToPdf(doc.Stream, doc.ExtensionOrName, doc.Signature, String.Empty, signerModels))
        End Sub

        Protected Sub ElaborateException(context As HttpContext, exception As Exception, download As Boolean, doc As DocumentInfo)
            If download Then
                ElaborateException(context, exception, "application/octet-stream", "attachment")
            Else
                ElaborateException(context, exception, "application/pdf", "inline", doc)
            End If
        End Sub

        Protected Sub ElaborateException(context As HttpContext, exception As Exception, responseContentType As String, disposition As String, Optional doc As DocumentInfo = Nothing)
            If exception.GetType() Is GetType(FaultException(Of BiblosDsException)) _
                AndAlso DirectCast(exception, FaultException(Of BiblosDsException)).Detail.Code = DocumentsService.FaultCode.ParerRetriveDocuments_Exception Then
                ElaborateParerException(context, New String() {exception.Message}, disposition.Eq("attachment"))
                Return
            End If
            If doc IsNot Nothing AndAlso doc.Extension.Eq(FileHelper.PDF) Then
                If Not CheckPrivacy(doc, responseContentType.Eq("application/octet-stream"), context) Then
                    Exit Sub
                End If
                ElaborateStream(context, doc.Stream, doc.PDFName, responseContentType, disposition)
                LogView(CStr(IIf(String.IsNullOrEmpty(DocumentName), doc.Name, DocumentName)), DocumentGuid.Value)
            Else
                Dim ex As New Exception("Errore in fase di recupero/elaborazione documento.", exception)
                Try
                    Dim results As Byte() = File.ReadAllBytes(BiblosFacade.CustomPdfError.FullName)
                    Using stream As New MemoryStream()
                        FacadeFactory.Instance.BiblosFacade.GetExceptionReadingDocument(ex, stream)
                        Dim streamMs As MemoryStream = New MemoryStream(stream.ToArray())
                        Using MemoryStream As New MemoryStream,
                              Pdf1MemoryStream As New MemoryStream(File.ReadAllBytes(BiblosFacade.CustomPdfError.FullName)),
                              Pdf2MemoryStream As New MemoryStream(streamMs.ToArray())

                            Using PdfDocument As New PdfDocument(New PdfWriter(MemoryStream)),
                                  Pdf1 As New PdfDocument(New PdfReader(Pdf1MemoryStream)),
                                  Pdf2 As New PdfDocument(New PdfReader(Pdf2MemoryStream))
                                Dim Merger As New PdfMerger(PdfDocument)
                                Merger.Merge(Pdf1, 1, Pdf1.GetNumberOfPages)
                                Merger.Merge(Pdf2, 1, Pdf2.GetNumberOfPages)
                            End Using

                            ElaborateStream(context, MemoryStream.ToArray(), "Error.pdf", responseContentType, disposition)
                        End Using
                    End Using
                Catch e As FileNotFoundException
                    Using streamFile As New MemoryStream()
                        FacadeFactory.Instance.BiblosFacade.GetExceptionReadingDocument(e, streamFile)
                        ElaborateStream(context, streamFile.ToArray(), "Error.pdf", responseContentType, disposition)
                    End Using
                    FileLogger.Error(LogName.FileLog, "Il percorso del file non esiste", e)
                Catch exi As Exception
                    Using streamEx As New MemoryStream()
                        FacadeFactory.Instance.BiblosFacade.GetExceptionReadingDocument(exi, streamEx)
                        ElaborateStream(context, streamEx.ToArray(), "Error.pdf", responseContentType, disposition)
                    End Using
                    FileLogger.Error(LogName.FileLog, "Errore in fase generazione del pdf di errore. viene preso il default Error.pdf", exi)
                End Try
            End If

        End Sub

        Protected Sub ElaborateRightsException(context As HttpContext, download As Boolean)
            Dim path As String = HostingEnvironment.MapPath(RIGHTS_ERROR_TEMPLATE_PATH)
            Dim stream As Byte() = File.ReadAllBytes(path)
            ElaborateRightsException(context, stream, RIGHTS_ERROR_TEMPLATE_NAME, download)
        End Sub
        Private Sub ElaborateDocumentUnitException(context As HttpContext, download As Boolean)
            Dim par1 As String = "Si è verificata una anomalia temporanea."
            Dim par2 As String = "Contattare assistenza per riallineare le strutture documentali."
            Dim text As String() = New String() {par1, par2}
            ElaborateRightsException(context, text, download, ERROR_TEMPLATE_NAME)
        End Sub

        Private Sub ElaborateSimplifiedPrivacyException(context As HttpContext, message As String, download As Boolean)
            Dim par1 As String = "Attenzione: non è possibile visualizzare il documento."
            Dim par2 As String = message
            Dim text As String() = New String() {par1, par2}
            ElaborateRightsException(context, text, download, PRIVACY_ERROR_TEMPLATE_NAME)
        End Sub

        Private Sub ElaboratePrivacyException(context As HttpContext, privacyLevel As Integer, download As Boolean)
            Dim level As PrivacyLevel = New PrivacyLevelFacade().GetByLevel(privacyLevel)
            Dim par1 As String = "Attenzione: non è possibile visualizzare il documento."
            Dim privacyText As String = privacyLevel.ToString()
            If level IsNot Nothing Then
                privacyText = String.Concat("""", level.Description, " (", level.Level, ")"".")
            End If
            Dim par2 As String = String.Concat("L’utente non possiede un livello di ", CommonBasePage.PRIVACY_LABEL, " sufficiente alla visualizzazione, è necessario un livello minimo pari a ",
                                                privacyText)
            Dim text As String() = New String() {par1, par2}
            ElaborateRightsException(context, text, download, PRIVACY_ERROR_TEMPLATE_NAME)
        End Sub

        Private Sub ElaborateDocumentDeletedException(context As HttpContext, download As Boolean)
            Dim par1 As String = "Le informazioni su questo documento sono disponibili nel sommario."
            Dim par2 As String = " "
            Dim text As String() = New String() {par1, par2}
            ElaborateRightsException(context, text, download, ERROR_TEMPLATE_NAME)
        End Sub

        Private Sub ElaborateRightsException(context As HttpContext, text As String(), download As Boolean, templateName As String)
            Using outputStream As New MemoryStream()
                Helper.CreatePrivacyErrorDocument(outputStream, text)
                ElaborateRightsException(context, outputStream.ToArray(), templateName, download)
            End Using
        End Sub

        Private Sub ElaborateParerException(context As HttpContext, text As String(), download As Boolean)
            Dim path As String = HostingEnvironment.MapPath(PARER_ERROR_TEMPLATE_PATH)
            Dim stream As Byte() = File.ReadAllBytes(path)
            If download Then
                DownloadStream(context, stream, PARER_ERROR_TEMPLATE_NAME)
            Else
                ElaborateStream(context, stream, PARER_ERROR_TEMPLATE_NAME)
            End If
        End Sub

        Private Sub ElaborateRightsException(context As HttpContext, stream As Byte(), fileName As String, download As Boolean)
            If download Then
                ElaborateStream(context, stream, fileName, "application/octet-stream", "attachment")
            Else
                ElaborateStream(context, stream, fileName, "application/pdf", "inline")
            End If
        End Sub

        Protected Sub DownloadStream(context As HttpContext, stream() As Byte, name As String)
            ElaborateStream(context, stream, name, "application/octet-stream", "attachment")
        End Sub

        Protected Sub ElaborateStream(context As HttpContext, stream() As Byte, name As String)
            ElaborateStream(context, stream, name, "application/pdf", "inline")
        End Sub

        Protected Sub ElaborateStream(context As HttpContext, stream() As Byte, name As String, responseContentType As String, disposition As String)

            context.Response.Clear()
            context.Response.ContentType = responseContentType
            context.Response.AddHeader("Content-Disposition", String.Concat(disposition, "; filename=""", name, """"))
            context.Response.AddHeader("Content-Length", stream.Length.ToString())
            context.Response.BinaryWrite(stream)

            context.ApplicationInstance.CompleteRequest()

        End Sub

        Private Function CheckPrivacy(doc As DocumentInfo, download As Boolean, context As HttpContext) As Boolean
            If _checkPrivacy.HasValue Then
                Return _checkPrivacy.Value
            End If

            If DocSuiteContext.Current.PrivacyEnabled AndAlso
                (IdContainer.HasValue OrElse (CurrentDocumentUnit IsNot Nothing AndAlso CurrentDocumentUnit.Container IsNot Nothing) OrElse
                (PrivacyRoles IsNot Nothing AndAlso PrivacyRoles.Count > 0) OrElse (PublicRoles IsNot Nothing AndAlso PublicRoles.Count > 0)) Then

                Dim containerId As Integer
                If IdContainer.HasValue Then
                    containerId = IdContainer.Value
                End If
                If CurrentDocumentUnit IsNot Nothing AndAlso CurrentDocumentUnit.Container IsNot Nothing Then
                    containerId = CurrentDocumentUnit.Container.EntityShortId
                End If

                If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso Not FromFascicle Then
                    If Not CheckPrivacyRight() Then
                        FileLogger.Error(LogName.FileLog, "Errore in fase di visualizzazione documento: utente non autorizzato al trattamento privacy.")
                        ElaborateSimplifiedPrivacyException(context, "L’utente non risulta autorizzato al trattamento privacy.", download)
                        _checkPrivacy = False
                        Return _checkPrivacy.Value
                    End If
                    _checkPrivacy = True
                    Return _checkPrivacy.Value
                End If

                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    Dim attribute As String = doc.Attributes.Where(Function(x) x.Key.Eq(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)).FirstOrDefault().Value
                    Dim privacyLevel As Integer = Convert.ToInt32(attribute)
                    Dim maxPrivacyLevel As Integer = FacadeFactory.ContainerGroupFacade.GetMaxPrivacyLevel(containerId, DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName)
                    If maxPrivacyLevel < privacyLevel Then
                        FileLogger.Error(LogName.FileLog, String.Concat("Errore in fase di visualizzazione documento: il livello di ", CommonBasePage.PRIVACY_LABEL, " dell'utente non è coerente col livello di ", CommonBasePage.PRIVACY_LABEL, " del documento"))
                        ElaboratePrivacyException(context, privacyLevel, download)
                        _checkPrivacy = False
                        Return _checkPrivacy.Value
                    End If
                End If
            End If
            _checkPrivacy = True
            Return _checkPrivacy.Value
        End Function

#End Region

    End Class

End Namespace