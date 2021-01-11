Imports System.Runtime.CompilerServices
Imports System.Collections.Specialized
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Data

Namespace Viewers.Extensions

    Module DocumentInfoEx

        ''' <summary>
        ''' Extension method per ottenere il link per scaricare un documentInfo
        ''' </summary>
        ''' <param name="doc">Il documento da scaricare</param>
        ''' <param name="asPdf">Parametro opzionale che definisce se il download deve prima passare da stampa conforme [default: false]</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function DownloadLink(ByVal doc As DocumentInfo, Optional ByVal asPdf As Boolean = False) As String
            Dim items As NameValueCollection = doc.ToQueryString()
            items.Add("Download", "true")
            If Not asPdf Then
                items.Add("Original", "true")
            End If
            Return String.Format("{0}/Viewers/Handlers/{1}.ashx/{2}?{3}",
                                                 DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                                 "DocumentInfoHandler",
                                                 FileHelper.FileNameToUrl(If(asPdf, doc.PDFName, doc.Name)),
                                                 CommonShared.AppendSecurityCheck(items.AsEncodedQueryString()))
        End Function

    End Module
End Namespace