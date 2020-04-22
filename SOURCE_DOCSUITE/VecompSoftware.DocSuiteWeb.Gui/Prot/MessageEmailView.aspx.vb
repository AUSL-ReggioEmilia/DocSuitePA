Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports VecompSoftware.Services.Biblos.Models

Public Class MessageEmailView
    Inherits ProtBasePage
    Implements IHaveViewerLight

#Region " Fields "

    Public Const SessionSeed As String = "MailMessagevSeed"
    Private _currentMessageEmailId As Integer? = Nothing
    Private _currentMessageEmail As MessageEmail = Nothing

#End Region

#Region " Properties "

    Public Overridable Property CurrentMessageEmailId As Integer?
        Get
            If Not _currentMessageEmailId.HasValue Then
                _currentMessageEmailId = GetKeyValue(Of Integer?)("MessageEmailId")
            End If
            Return _currentMessageEmailId
        End Get
        Protected Set(value As Integer?)
            ViewState("MessageEmailId") = value
        End Set
    End Property

    Public ReadOnly Property CurrentMessageEmail As MessageEmail
        Get
            If _currentMessageEmail Is Nothing AndAlso CurrentMessageEmailId.HasValue Then
                _currentMessageEmail = Facade.MessageEmailFacade.GetById(CurrentMessageEmailId.Value, False)
            End If
            Return _currentMessageEmail
        End Get
    End Property

    Public ReadOnly Property CheckedDocuments() As List(Of DocumentInfo) Implements IHaveViewerLight.CheckedDocuments
        Get
            Return viewerLight.CheckedDocuments
        End Get
    End Property

    Public ReadOnly Property SelectedDocument() As DocumentInfo Implements IHaveViewerLight.SelectedDocument
        Get
            Return viewerLight.SelectedDocument
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        ' Aggiunge le signature ai documenti se abilitate da configurazione
        CreateSignature()

        If Not IsPostBack AndAlso Not Page.IsCallback Then
            SetResponseNoCache()

            Title = String.Format("{0:dddd dd/MM/yyyy} - {1}", If(CurrentMessageEmail.SentDate.HasValue, CurrentMessageEmail.SentDate.Value, CurrentMessageEmail.Message.RegistrationDate), CurrentMessageEmail.Subject)
            viewerLight.CheckBoxes = True
            BindViewerLight()
        End If
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        If Not viewerLight.Initialized Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, viewerLight)
        End If
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, WarningPanel)
    End Sub

    Private Sub BindViewerLight()
        viewerLight.DataSource = Facade.MessageEmailFacade.GetDocuments(CurrentMessageEmail)
    End Sub

    Private Sub CreateSignature()
        ' Decorazione della signature da mettere nella testata dei documenti della mail in caso di Spedizione delle mail inviate
        If ProtocolEnv.EnableSignatureMailSend = True Then
            ' Stampo l'intestazione solamente se posso inserire un orario di invio
            If Not CurrentMessageEmail.SentDate Is Nothing Then
                viewerLight.AddSignature = String.Format(ProtocolEnv.SignatureMailSend, "inviata", CurrentMessageEmail.SentDate)
            End If
        End If
    End Sub
#End Region

End Class