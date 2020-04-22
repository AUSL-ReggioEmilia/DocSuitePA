Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.Services.Biblos.Models

Public Class DeskViewer
    Inherits DeskBasePage
    Implements ISendMail

#Region "Fields "

    Private _currentDesk As Desk
    Private _deskDocumentFacade As DeskDocumentFacade
    Private Const MAIL_SENDER_PATH_FORMAT As String = "~/MailSenders/DeskMailSender.aspx?DeskId={0}"

#End Region

#Region "Properties"

    Public ReadOnly Property ViewOriginal As Boolean
        Get
            Dim param As String = Request.QueryString("ViewOriginal")
            If String.IsNullOrEmpty(param) Then
                Return False
            End If
            Dim result As Boolean = False
            Boolean.TryParse(param, result)
            Return result
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
            Dim docs As IList(Of DocumentInfo) = ViewerLight.CheckedDocuments
            Return docs
        End Get
    End Property

    Public ReadOnly Property Subject() As String Implements ISendMail.Subject
        Get
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property Body() As String Implements ISendMail.Body
        Get
            Return String.Empty
        End Get
    End Property

#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            btnSend.PostBackUrl = String.Format(MAIL_SENDER_PATH_FORMAT, CurrentDeskId)
            BindViewerLight()
        End If
        ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("DeskViewer")
    End Sub
#End Region

#Region "Methods"
    Private Sub BindViewerLight()
        If ViewOriginal Then
            ViewerLight.ViewOriginal = True
        End If

        Dim documents As IList(Of DocumentInfo) = New List(Of DocumentInfo)
        For Each deskDocument As DeskDocument In CurrentDesk.DeskDocuments.Where(Function(x) x.IsActive = 0)
            Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(CurrentDesk.Container.DeskLocation.DocumentServer, deskDocument.IdDocument.Value)
            documents.Add(docInfos.OrderByDescending(Function(f) f.Version).FirstOrDefault())
        Next

        Dim folder As New FolderInfo() With {.Name = "Documenti"}
        folder.AddChildren(documents)
        ViewerLight.DataSource = New List(Of DocumentInfo) From {folder}
    End Sub

#End Region

End Class