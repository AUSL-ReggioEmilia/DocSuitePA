Imports System.Linq
Imports System.Collections.Generic
Imports System.Diagnostics
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Public Class PecMailViewer
    Inherits PECBasePage
    Implements ISendMail

#Region " Fields "


#End Region

#Region " Properties "

    Public ReadOnly Property SenderDescription As String Implements ISendMail.SenderDescription
        Get
            Return CommonInstance.UserDescription
        End Get
    End Property

    Public ReadOnly Property SenderEmail As String Implements ISendMail.SenderEmail
        Get
            Return CommonInstance.UserMail
        End Get
    End Property

    Public ReadOnly Property Recipients As IList(Of ContactDTO) Implements ISendMail.Recipients
        Get
            Return New List(Of ContactDTO)()
        End Get
    End Property

    Public ReadOnly Property Documents As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            Dim docs As IList(Of DocumentInfo) = ViewerLight.CheckedDocuments
            If docs.Count = 0 Then
                docs = If(CurrentPecMailWrapper IsNot Nothing, Facade.PECMailFacade.GetDocumentList(CurrentPecMailWrapper), ViewerLight.AllDocuments)
            End If
            Return docs
        End Get
    End Property

    Public ReadOnly Property Subject As String Implements ISendMail.Subject
        Get
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property Body As String Implements ISendMail.Body
        Get
            Return String.Empty
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()
        ViewerLight.EnabledCheckIsSignedButton = Not ProtocolEnv.CheckSignedEvaluateStream
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            'send.PostBackUrl = String.Format("{0}?recipients=false&Year={1}&Number={2}&overridepreviouspageurl=true", send.PostBackUrl, CurrentProtocolYear, CurrentProtocolNumber)
            BindViewerLight()
        End If
        ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("PecMailViewer")
    End Sub

#End Region

#Region " Methods "

    Private Sub BindViewerLight()
        Dim elapsed As Int32 = 0
#If DEBUG Then
        Dim watch As Stopwatch = Stopwatch.StartNew()
#End If
        Dim datasource As New List(Of DocumentInfo)
        Dim viewList As IList(Of PECMailView) = Facade.PECMailViewFacade.GetByRightAndPageType()
        Dim defaultPecMailView As PECMailView = Facade.PECMailViewFacade.GetDefault(viewList)

        If defaultPecMailView IsNot Nothing Then
            ViewerLight.MultiViewDefaultId = defaultPecMailView.Id
        End If
        For Each pecMailView As PECMailView In viewList
            Dim pecMailViewFolderInfo As New FolderInfo(pecMailView.Id.ToString(), pecMailView.Title)

            For Each pecMail As PECMail In CurrentPecMailList
                Dim temp As DocumentInfo = Facade.PECMailFacade.GetDocumentInfo(New BiblosPecMailWrapper(pecMail, ProtocolEnv.CheckSignedEvaluateStream), pecMailView)
                temp.Caption = String.Format("{0} - {1}", temp.Name, pecMail.MailSubject)
                pecMailViewFolderInfo.AddChild(temp)
            Next
            datasource.Add(pecMailViewFolderInfo)
        Next

        ViewerLight.DataSource = datasource
        ViewerLight.MultiViewEnable = True

#If DEBUG Then
        watch.Stop()
        elapsed = watch.Elapsed.Milliseconds
#End If
        FileLogger.Debug(LoggerName, String.Format("{0} - BindViewerLight - {1}", Request.RawUrl, elapsed))
    End Sub

#End Region
End Class