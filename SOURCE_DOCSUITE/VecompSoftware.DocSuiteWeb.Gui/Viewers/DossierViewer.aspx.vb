Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Dossiers
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Namespace Viewers
    Public Class DossierViewer
        Inherits DossierBasePage
        Implements ISendMail

#Region " Fields "
        Private _currentDossierFoldersFinder As DossierFoldersFinder
#End Region

#Region " Properties "
        Public ReadOnly Property CurrentDossierFoldersFinder As DossierFoldersFinder
            Get
                If _currentDossierFoldersFinder Is Nothing Then
                    _currentDossierFoldersFinder = New DossierFoldersFinder(DocSuiteContext.Current.CurrentTenant)
                End If
                Return _currentDossierFoldersFinder
            End Get
        End Property
        Protected ReadOnly Property CurrentDossierTitle As String
            Get
                Return String.Format("{0}/{1}", CurrentDossier.Year, CurrentDossier.Number)
            End Get
        End Property

        Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
            Get
                Return CommonInstance.UserDescription
            End Get
        End Property

        Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
            Get
                Return CommonInstance.UserMail
            End Get
        End Property

        Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
            Get
                Return New List(Of ContactDTO)()
            End Get
        End Property

        Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property Subject() As String Implements ISendMail.Subject
            Get
                Return MailFacade.GetDossierSubject(CurrentDossier)
            End Get
        End Property

        Public ReadOnly Property Body() As String Implements ISendMail.Body
            Get
                Return MailFacade.GetDossierBody(CurrentDossier)
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
                MasterDocSuite.TitleVisible = False
                If (IdDossier.IsEmpty) Then
                    btnSend.Enabled = False
                Else
                    btnSend.PostBackUrl = String.Format("{0}?Type=Dossier&IdDossier={1}", btnSend.PostBackUrl, IdDossier)
                End If
                BindViewerLight()
            End If
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("DossierViewer")
        End Sub

#End Region

#Region " Methods "

        Private Sub BindViewerLight()
            Dim fascicleViewer As FascicleViewer = New FascicleViewer()

            Dim mainFolder As New FolderInfo()
            mainFolder.Name = String.Format("Dossier {0} del {1:dd/MM/yyyy}", CurrentDossierTitle, CurrentDossier.RegistrationDate)
            mainFolder.ID = CurrentDossier.UniqueId.ToString()

            Dim documentInfoSource As DocumentInfo = GetDossierInserts(CurrentDossier)
            Dim datasource As List(Of DocumentInfo) = New List(Of DocumentInfo)
            If documentInfoSource IsNot Nothing Then
                datasource.Add(documentInfoSource)
            End If

            Dim results As ICollection(Of WebAPIDto(Of Entity.Dossiers.DossierFolder)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentDossierFoldersFinder,
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.EnablePaging = False
                                finder.DossierId = CurrentDossier.UniqueId
                                finder.ExpandProperties = True
                                finder.HasFascicles = True
                                Return finder.DoSearch()
                            End Function)

            For Each fascicle As Entity.Fascicles.Fascicle In results.Select(Function(f) f.Entity.Fascicle)
                datasource.Add(fascicleViewer.GetFascicleUDDocuments(fascicle))
            Next
            ViewerLight.DataSource = datasource
            FileLogger.Debug(LoggerName, String.Format("{0} - BindViewerLight", Request.RawUrl))
        End Sub


        Private Function GetDossierInserts(dossier As Entity.Dossiers.Dossier) As FolderInfo
            Dim insertsFolder As FolderInfo = Nothing
            Dim insertsDocument As Entity.Dossiers.DossierDocument = CurrentDossier.DossierDocuments.FirstOrDefault()
            Dim insertsLocation As Location = Facade.LocationFacade.GetById(ProtocolEnv.DossierMiscellaneaLocation)
            If insertsLocation IsNot Nothing Then
                insertsFolder = New FolderInfo()
                insertsFolder.Name = "Inserti"
                insertsFolder.ID = dossier.UniqueId.ToString()
                If insertsDocument IsNot Nothing Then
                    Dim insertsDocs As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(insertsDocument.IdArchiveChain).ToArray()
                    If (insertsDocs IsNot Nothing) Then
                        insertsFolder.AddChildren(insertsDocs.OfType(Of DocumentInfo).ToList())
                    End If
                End If
            End If
            Return insertsFolder
        End Function

#End Region

    End Class
End Namespace