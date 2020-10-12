Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Services.AnagraficaANAS.Models
Imports VecompSoftware.Services.AnagraficaANAS.Services
Imports VecompSoftware.Services.Logging

Public Class CommonSetiContactSel
    Inherits CommBasePage


#Region " Fields "
    Private Const SETI_CONTACTS_CALLBACK As String = "commonSetiContactSel.contactsCallback('{0}');"
    Private Const SETI_CONTACTS_ERROR_CALLBACK As String = "commonSetiContactSel.contactsErrorCallback('{0}');"
    Private _anagraficaANASService As AnagraficaANASService
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler AjaxManager.AjaxRequest, AddressOf CommonSetiContactSel_AjaxRequest
    End Sub
    Protected Sub CommonSetiContactSel_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            If ajaxModel Is Nothing Then
                Return
            End If

            Dim infoLogger As Action(Of String) = Sub(infoMessage)
                                                      FileLogger.Info(LoggerName, infoMessage)
                                                  End Sub
            Dim errorLogger As Action(Of String) = Sub(errorMessage)
                                                       FileLogger.Error(LoggerName, errorMessage)
                                                   End Sub
            _anagraficaANASService = New AnagraficaANASService(ProtocolEnv.AnagraficaANASUrl, infoLogger, errorLogger)

            Select Case ajaxModel.ActionName
                Case "SearchByText"
                    Dim searchText As String = ajaxModel.Value(0)
                    _anagraficaANASService.GetSetiContactsAsync(searchText, AnAsContactsSuccessCallback, AnAsContactsErrorCallback)
            End Select

        Catch ex As Exception
            AjaxManager.ResponseScripts.Add(String.Format(SETI_CONTACTS_ERROR_CALLBACK, ex.Message))
            Exit Sub
        End Try
    End Sub

    Private ReadOnly AnAsContactsSuccessCallback As Action(Of List(Of AnagraficaAnAsModel)) =
        Sub(contacts)
            Dim response As List(Of AnagraficaAnAsModel) = contacts
            AjaxManager.ResponseScripts.Add(String.Format(SETI_CONTACTS_CALLBACK, JsonConvert.SerializeObject(response)))
        End Sub

    Private ReadOnly AnAsContactsErrorCallback As Action(Of String) =
        Sub(errorMessage)
            AjaxManager.ResponseScripts.Add(String.Format(SETI_CONTACTS_ERROR_CALLBACK, errorMessage))
        End Sub

#End Region



End Class