Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models

Public Class DossierVisualizza
    Inherits DossierBasePage
    Implements ISendMail

#Region " Fields "

    Private _location As Location
#End Region

#Region " Properties "

    Public ReadOnly Property IsMiscellaneaLocationEnabled As Boolean
        Get
            Return HasMiscellaneaLocation IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property HasMiscellaneaLocation As Location
        Get
            If _location Is Nothing Then
                _location = Facade.LocationFacade.GetById(ProtocolEnv.DossierMiscellaneaLocation)
            End If
            Return _location
        End Get
    End Property

    Protected ReadOnly Property IdWorkflowActivity As String
        Get
            Dim _idWorkflowActivity As String = Request.QueryString.GetValueOrDefault("IdWorkflowActivity", String.Empty)
            Return _idWorkflowActivity
        End Get
    End Property

    Protected ReadOnly Property WorkflowEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.WorkflowManagerEnabled
        End Get
    End Property

    Protected ReadOnly Property DossierSendToSecretariesEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.DossierSendToSecretariesEnabled
        End Get
    End Property

    Public ReadOnly Property IsWindowPopupEnable As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault("IsWindowPopupEnable", False)
        End Get
    End Property

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
            Return RoleFacade.CopyGetValidContacts(CurrentDossier.DossierRoles.Select(Function(x) x.Role).ToList())
        End Get
    End Property

    Public ReadOnly Property Documents As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Subject As String Implements ISendMail.Subject
        Get
            Return MailFacade.GetDossierSubject(CurrentDossier)
        End Get
    End Property

    Public ReadOnly Property Body As String Implements ISendMail.Body
        Get
            Return MailFacade.GetDossierBody(CurrentDossier)
        End Get
    End Property


#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()

        If btnSendToRoles.Visible Then
            btnSendToRoles.PostBackUrl = $"~/MailSenders/DossierMailSender.aspx?SendToRoles=true&IdDossier={CurrentDossier.UniqueId}"
        End If

        If btnSendToSecretaries.Visible Then
            btnSendToSecretaries.PostBackUrl = $"~/MailSenders/DossierMailSender.aspx?SendToSecretaries=true&IdDossier={CurrentDossier.UniqueId}"
        End If

        If Not IsPostBack Then
            uscDossierFolders.IsWindowPopupEnable = IsWindowPopupEnable
            uscDossierFolders.FascicleModifyButtonEnable = False
        End If
    End Sub

    Protected Sub DossierVisualizzaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "Initialize"
                Initialize()
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AddHandler AjaxManager.AjaxRequest, AddressOf DossierVisualizzaAjaxRequest
    End Sub

    Private Sub Initialize()
    End Sub

#End Region



End Class