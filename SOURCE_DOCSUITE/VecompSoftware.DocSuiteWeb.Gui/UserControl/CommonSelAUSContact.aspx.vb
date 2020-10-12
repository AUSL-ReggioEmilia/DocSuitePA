Imports System.Collections.Generic
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Services.AnagraficaAUS
Imports VecompSoftware.Services.AnagraficaAUS.Models.Contacts
Imports VecompSoftware.Services.AnagraficaAUS.Models.Enums
Imports VecompSoftware.Services.Logging

Public Class CommonSelAUSContact
    Inherits CommBasePage

#Region " Fields "
    Private Const AUS_CONTACTS_CALLBACK As String = "commonSelAUSContact.contactsCallback('{0}');"
    Private Const AUS_CONTACTS_ERROR_CALLBACK As String = "commonSelAUSContact.contactsErrorCallback('{0}');"
#End Region

#Region " Properties "
    Public ReadOnly Property CallerId As String
        Get
            Return GetKeyValue(Of String, CommonSelAUSContact)("ParentID")
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler AjaxManager.AjaxRequest, AddressOf CommonSelAUSContact_AjaxRequest
    End Sub

    Protected Sub CommonSelAUSContact_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            If ajaxModel Is Nothing Then
                Return
            End If
            Dim username As String = DocSuiteContext.Current.CurrentTenant.DomainUser
            Dim subjectType As SubjectType = [Enum].Parse(GetType(SubjectType), ajaxModel.Value(1))
            Dim infoLogger As Action(Of String) = Sub(infoMessage)
                                                      FileLogger.Info(LoggerName, infoMessage)
                                                  End Sub
            Dim errorLogger As Action(Of String) = Sub(errorMessage)
                                                       FileLogger.Error(LoggerName, errorMessage)
                                                   End Sub
            Dim successCallback As Action(Of List(Of ContactModel)) = Sub(contacts)
                                                                          SendAUSContactsToClient(contacts)
                                                                      End Sub
            Dim errorCallback As Action(Of String) = Sub(errorMessage)
                                                         SendErrorMessageToClient(errorMessage)
                                                     End Sub
            Dim anagraficaAUSService As AnagraficaAUSService = New AnagraficaAUSService(ProtocolEnv.AnagraficaAUSUrl, ProtocolEnv.AUSUser, ProtocolEnv.AUSPassword, username, subjectType, infoLogger, errorLogger)
            Select Case ajaxModel.ActionName
                Case "SearchByText"
                    Dim searchText As String = ajaxModel.Value(0)
                    anagraficaAUSService.GetAUSContactsByName(searchText, successCallback, errorCallback)
                Case "SearchByCode"
                    Dim searchCode As String = ajaxModel.Value(0)
                    anagraficaAUSService.GetAUSContactsByCode(searchCode, successCallback, errorCallback)
                    Exit Select
            End Select
        Catch ex As Exception
            SendErrorMessageToClient(ex.Message)
            Exit Sub
        End Try
    End Sub
#End Region

#Region " Methods "
    Private Sub SendAUSContactsToClient(contacts As List(Of ContactModel))
        Dim jsonResult As String = HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(contacts))
        AjaxManager.ResponseScripts.Add(String.Format(AUS_CONTACTS_CALLBACK, jsonResult))
    End Sub

    Private Sub SendErrorMessageToClient(ByVal errorMessage As String)
        If Not String.IsNullOrEmpty(errorMessage) Then
            AjaxManager.ResponseScripts.Add(String.Format(AUS_CONTACTS_ERROR_CALLBACK, errorMessage))
        End If
    End Sub
#End Region

End Class