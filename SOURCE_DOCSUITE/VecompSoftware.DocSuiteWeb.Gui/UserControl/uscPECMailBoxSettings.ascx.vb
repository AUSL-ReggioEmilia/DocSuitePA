Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

Public Class uscPECMailBoxSettings
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Private Const PEC_MAIL_BOX_SETTINGS_MODEL As String = "{0}_uscPECMailBoxSettings.pecMAilWithEncryptedPassword(`{1}`, {2});"
#End Region

#Region " Properties "

    Public ReadOnly Property PageContent As Panel
        Get
            Return pnlDetails
        End Get
    End Property

    Public ReadOnly Property IsValidEncryptionKey As Boolean
        Get
            Return String.IsNullOrEmpty(DocSuiteContext.PasswordEncryptionKey) AndAlso DocSuiteContext.PasswordEncryptionKey.Length <> 32
        End Get
    End Property

    Public ReadOnly Property InvoiceTypes As String
        Get
            Dim InvoiceTypesDictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Dim itemValues As Array = System.Enum.GetValues(GetType(InvoiceType))
            Dim itemNames As Array = System.Enum.GetNames(GetType(InvoiceType))
            For i As Integer = 0 To itemNames.Length - 1
                Dim item As New ListItem(DirectCast(i, InvoiceType).GetDescription(), itemValues(i))
                InvoiceTypesDictionary.Add(itemNames(i), item.Text)
            Next
            Return JsonConvert.SerializeObject(InvoiceTypesDictionary)
        End Get
    End Property
    Public Property IsInsertAction As Boolean
    Public Property ValidationGroupName As String
#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            txtMailboxNameRequireValidator.ValidationGroup = ValidationGroupName
            txtUsernameRequireValidator.ValidationGroup = ValidationGroupName
            txtPasswordRequireValidator.ValidationGroup = ValidationGroupName
            ddlLocationRequireValidator.ValidationGroup = ValidationGroupName
            txtINPortRequireValidator.ValidationGroup = ValidationGroupName
            txtOUTPortRequireValidator.ValidationGroup = ValidationGroupName
        End If
    End Sub

    Protected Sub UscPECMailBoxSettingsAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel IsNot Nothing Then
            Try
                Select Case ajaxModel.ActionName
                    Case "EncryptPassword"
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
                            Dim pecMailBoxSettingsModel As Entity.PECMails.PECMailBox = JsonConvert.DeserializeObject(Of Entity.PECMails.PECMailBox)(ajaxModel.Value(1))

                            Dim actionType As String = ajaxModel.Value(0)
                            Dim clientId As String = ajaxModel.Value(2)
                            If clientId <> Me.ClientID Then
                                Exit Sub
                            End If

                            Dim password As String = String.Empty
                            Select Case actionType
                                Case "Insert"
                                    password = Helpers.Security.EncryptionHelper.EncryptString(pecMailBoxSettingsModel.Password, DocSuiteContext.PasswordEncryptionKey)
                                    IsInsertAction = True
                                Case "Update"
                                    If String.IsNullOrEmpty(txtPassword.Text) Then
                                        password = pecMailBoxSettingsModel.Password
                                    Else
                                        password = Helpers.Security.EncryptionHelper.EncryptString(pecMailBoxSettingsModel.Password, DocSuiteContext.PasswordEncryptionKey)
                                    End If
                                    IsInsertAction = False
                            End Select

                            pecMailBoxSettingsModel.Password = password
                            If (pecMailBoxSettingsModel.RulesetDefinition IsNot Nothing) Then
                                pecMailBoxSettingsModel.RulesetDefinition = pecMailBoxSettingsModel.RulesetDefinition.Replace("""", "\""")
                            End If
                            AjaxManager.ResponseScripts.Add(String.Format(PEC_MAIL_BOX_SETTINGS_MODEL, Me.ClientID, JsonConvert.SerializeObject(pecMailBoxSettingsModel), IsInsertAction.ToString().ToLower()))
                            End If
                End Select
            Catch ex As Exception
                FileLogger.Error(LoggerName, "Errore nel caricamento dei dati del pec.", ex)
                AjaxManager.Alert("Errore nel caricamento dei dati del pec.")
                Return
            End Try

        End If

    End Sub

#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscPECMailBoxSettingsAjaxRequest
    End Sub
#End Region

End Class