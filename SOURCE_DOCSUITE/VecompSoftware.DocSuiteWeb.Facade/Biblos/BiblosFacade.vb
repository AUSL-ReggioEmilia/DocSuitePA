Imports System.IO
Imports VecompSoftware.Helpers.PDF
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class BiblosFacade
    Public Const SECURE_DOCUMENT_ATTRIBUTE As String = "SecureDocumentId"
    Public Const DOCUMENT_POSITION_ATTRIBUTE As String = "Position"
    Public Const FILENAME_ATTRIBUTE As String = "Filename"
    Public Const SIGNATURE_ATTRIBUTE As String = "Signature"
    Public Const PRIVACYLEVEL_ATTRIBUTE As String = "PrivacyLevel"
    Public Const NOTE_ATTRIBUTE As String = "Note"
    Public Const REGISTRATION_USER_ATTRIBUTE As String = "RegistrationUser"

    Overridable ReadOnly Property CustomPdfError As FileInfo
        Get
            Dim tor As FileInfo = Nothing
            Dim path As String = DocSuiteContext.Current.ProtocolEnv.CustomErrorFrontPageDocument
            If Not String.IsNullOrEmpty(path) Then
                tor = New FileInfo(HttpContext.Current.Server.MapPath(path))
            End If
            Return tor
        End Get
    End Property

    Public Sub GetConversionNotAllowedPdf(doc As DocumentInfo, outputStream As Stream)

        Dim title As String = String.Concat(Environment.NewLine, Environment.NewLine, "Conversione PDF non disponibile.",
                                                          Environment.NewLine, Environment.NewLine)
        Dim filename As String = String.Concat(Environment.NewLine, Environment.NewLine,
                                                                   "Documento selezionato: ", doc.Name)
        Dim desc As String = String.Concat(Environment.NewLine, Environment.NewLine,
                                                                    "Il documento selezionato non è convertibile in PDF per la visualizzazione.",
                                                                    Environment.NewLine,
                                                                    "Selezionare il documento e premere Originali per scaricare il documento.")

        Helper.CreateNewDocument(outputStream, title, CustomPdfError, filename, desc)
    End Sub

    Public Sub GetExceptionReadingDocument(ex As Exception, outputStream As Stream)
        Helper.CreateFromException(ex, outputStream, CustomPdfError)
    End Sub

    Public Shared Function SaveUniqueToTemp(doc As DocumentInfo, filename As String) As FileInfo
        Return doc.SaveUniqueToTemp(filename)
    End Function

    Public Shared Function SaveUniqueToTemp(doc As DocumentInfo) As FileInfo
        Return doc.SaveUniqueToTemp()
    End Function

    Public Shared Function SaveUniquePdfToTemp(doc As DocumentInfo) As FileInfo
        Return SaveUniquePdfToTemp(doc, doc.PDFName)
    End Function

    Public Shared Function SaveUniquePdfToTemp(file As FileInfo) As FileInfo
        Dim doc As New FileDocumentInfo(file)
        Return SaveUniquePdfToTemp(doc)
    End Function

    Public Shared Function SaveUniquePdfToTemp(file As FileInfo, filename As String) As FileInfo
        Dim doc As New FileDocumentInfo(file)
        Return SaveUniquePdfToTemp(doc, filename)
    End Function

    Public Shared Function SaveUniquePdfToTemp(doc As DocumentInfo, filename As String) As FileInfo
        Dim uniquename As String = FileHelper.UniqueFileNameFormat(filename, DocSuiteContext.Current.User.UserName)
        Return doc.SavePdf(CommonUtil.GetInstance().TempDirectory, uniquename, String.Empty)
    End Function

    Public Shared Function SaveUniquePdfToTempNoSignature(doc As DocumentInfo) As FileInfo
        Return SaveUniquePdfToTempNoSignature(doc, doc.PDFName)
    End Function

    Public Shared Function SaveUniquePdfToTempNoSignature(file As FileInfo, filename As String) As FileInfo
        Dim doc As New FileDocumentInfo(file)
        Return SaveUniquePdfToTempNoSignature(doc, filename)
    End Function

    Public Shared Function SaveUniquePdfToTempNoSignature(doc As DocumentInfo, filename As String) As FileInfo
        Return SaveUniquePdfToTempNoSignature(doc, filename, CommonUtil.GetInstance().AppTempPath)
    End Function

    'metodo usato dal modulo ResolutionWorkflowManager del JeepService per salvare i documenti nella cartella Temp
    Public Shared Function SaveUniquePdfToTempNoSignature(doc As DocumentInfo, filename As String, path As String) As FileInfo
        Dim uniquename As String = FileHelper.UniqueFileNameFormat(filename, DocSuiteContext.Current.User.UserName)
        Dim directory As DirectoryInfo = New DirectoryInfo(path)
        Return doc.SavePdfNoSignature(directory, uniquename)
    End Function

    Public Shared Function SortDocuments(documents As IList(Of BiblosDocumentInfo)) As List(Of BiblosDocumentInfo)
        Dim positionDocumentsDictionary As IDictionary(Of BiblosDocumentInfo, Integer) = New Dictionary(Of BiblosDocumentInfo, Integer)()
        Dim positionVal As String
        For Each document As BiblosDocumentInfo In documents
            positionVal = document.Attributes.Where(Function(x) x.Key.Eq(DOCUMENT_POSITION_ATTRIBUTE)).Select(Function(s) s.Value).DefaultIfEmpty("0").First()
            positionDocumentsDictionary.Add(document, Integer.Parse(positionVal))
        Next
        Return positionDocumentsDictionary.OrderBy(Function(o) o.Value).Select(Function(s) s.Key).ToList()
    End Function
End Class