Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Linq
Imports iTextSharp.tool.xml.html
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Xml
Imports VecompSoftware.Helpers.Web
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Web

Public Class PECFix
    Inherits PECBasePage

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            InitializeFromPec(CurrentPecMail)
        End If
    End Sub

    Private Sub CmdFixClick(txtMailSubject As String, txtMailTo As String, txtMailCC As String)
        Dim pecToUpdate As PECMail = CurrentPecMail
        FacadeFactory.Instance.PECMailLogFacade.InsertLog(pecToUpdate, _
            String.Format("Correzione manuale di pec in errore. MailRecipients: {0} -> {1}\nMailRecipientsCc: {2} -> {3}\nMailSubject: {4} -> {5}\nBody: {6} -> {7}", _
                          CurrentPecMail.MailRecipients, HttpUtility.HtmlDecode(txtMailTo), CurrentPecMail.MailRecipientsCc, HttpUtility.HtmlDecode(txtMailCC), CurrentPecMail.MailSubject, HttpUtility.HtmlDecode(txtMailSubject), CurrentPecMail.MailBody, HttpUtility.HtmlDecode(txtMailBody.Text)), _
                      PECMailLogType.Fixed)
        pecToUpdate.MailSubject = HttpUtility.HtmlDecode(txtMailSubject)
        pecToUpdate.MailRecipients = HttpUtility.HtmlDecode(txtMailTo)
        pecToUpdate.MailRecipientsCc = HttpUtility.HtmlDecode(txtMailCC)
        pecToUpdate.MailBody = txtMailBody.GetHtml(EditorStripHtmlOptions.None)
        FacadeFactory.Instance.PECMailFacade.UpdateOnly(pecToUpdate)
    End Sub

    Protected Sub Request_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If

        Select Case arguments(1)
            Case "FixPEC"
                Dim txtMailTo As String = arguments(2)
                Dim txtMailCC As String = arguments(3)
                Dim txtMailSubject As String = arguments(4)
                CmdFixClick(txtMailSubject, txtMailTo, txtMailCC)
        End Select
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf Request_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdFix, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdFix, pnlContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdFix, cmdFix, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.ClientEvents.OnResponseEnd = "responseEnd"
    End Sub

    Private Sub InitializeFromPec(pec As PECMail)
        If pec Is Nothing Then
            Exit Sub
        End If

        txtMailSubject.Text = pec.MailSubject
        txtMailTo.Text = pec.MailRecipients
        txtMailCC.Text = pec.MailRecipientsCc
        txtMailBody.Content = pec.MailBody
    End Sub

#End Region

End Class
