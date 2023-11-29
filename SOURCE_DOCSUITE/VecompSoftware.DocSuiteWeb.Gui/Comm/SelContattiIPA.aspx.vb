Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.IPA
Imports VecompSoftware.Services.IPA.Models
Imports VecompSoftware.Services.Logging

Partial Class SelContattiIPA
    Inherits CommonBasePage

#Region " Fields "
    Private Const SEARCH_ADMINISTRATIONS_CALLBACK As String = "selContattiIPA.searchAdministrationsCallback('{0}')"
    Private Const SEARCH_AOO_CALLBACK As String = "selContattiIPA.searchAOOCallback('{0}', '{1}')"
    Private Const SEARCH_OU_CALLBACK As String = "selContattiIPA.searchOUCallback('{0}', '{1}')"
    Private Const CONFIRM_CALLBACK As String = "selContattiIPA.confirmCallback('{0}', {1})"
    Private Const SHOW_ADMINISTRATION_DETAILS_CALLBACK As String = "selContattiIPA.showAdministrationDetailsCallback('{0}')"
    Private _ipaService As Lazy(Of IPAService) = New Lazy(Of IPAService)(InitializeIPAService)
#End Region

#Region " Properties "
    Public ReadOnly Property CallerId As String
        Get
            Return GetKeyValue(Of String, CommonSelContactRest)("ParentID")
        End Get
    End Property

    Private ReadOnly Property IPAService As IPAService
        Get
            Return _ipaService.Value
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        MasterDocSuite.TitleVisible = False

        If Not IsPostBack Then
            If Not ProtocolEnv.IPAEnabled Then
                Throw New DocSuiteException($"Ricerca IPA non abilitata, controllare che sia abilitato il parametro {NameOf(ProtocolEnv.IPAEnabled)}")
            End If
        End If
    End Sub

    Protected Sub SelContattiIPA_AjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            If ajaxModel Is Nothing Then
                Return
            End If
            Select Case ajaxModel.ActionName
                Case "SearchAdministrations"
                    Dim description As String = ajaxModel.Value.First().ToString()
                    Dim ipaAdministrations As ICollection(Of IPAAdministration) = IPAService.FindAdministrations(description).Result

                    AjaxManager.ResponseScripts.Add(String.Format(SEARCH_ADMINISTRATIONS_CALLBACK, HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(ipaAdministrations))))
                Case "SearchAOOs"
                    Dim codAmm As String = ajaxModel.Value.First().ToString()
                    Dim ipaAOOs As ICollection(Of IPAAOO) = IPAService.FindAOO(codAmm).Result

                    AjaxManager.ResponseScripts.Add(String.Format(SEARCH_AOO_CALLBACK, HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(ipaAOOs)), codAmm))
                Case "SearchOUs"
                    Dim codAmm As String = ajaxModel.Value.First().ToString()
                    Dim ipaOUs As ICollection(Of IPAOU) = IPAService.FindOU(codAmm).Result

                    AjaxManager.ResponseScripts.Add(String.Format(SEARCH_OU_CALLBACK, HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(ipaOUs)), codAmm))
                Case "Confirm"
                    Dim ipaContactType As String = ajaxModel.Value.First().ToString()
                    Dim ipaContact As Contact = Nothing
                    Select Case ipaContactType
                        Case IPAHelper.IPA_ADMINISTRATION
                            Dim ipaAdministration As IPAAdministration = JsonConvert.DeserializeObject(Of IPAAdministration)(ajaxModel.Value(1).ToString())
                            ipaAdministration = IPAService.GetAdministrationDetails(ipaAdministration.CodAmm).Result
                            ipaContact = CreateAdministrationContact(ipaAdministration)
                        Case IPAHelper.IPA_AOO
                            Dim ipaAOO As IPAAOO = JsonConvert.DeserializeObject(Of IPAAOO)(ajaxModel.Value(1).ToString())
                            ipaContact = CreateAOOContact(ipaAOO)
                        Case IPAHelper.IPA_OU
                            Dim ipaOU As IPAOU = JsonConvert.DeserializeObject(Of IPAOU)(ajaxModel.Value(1).ToString())
                            ipaContact = CreateOUContact(ipaOU)
                    End Select

                    AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(ipaContact)), ajaxModel.Value(2)))
                Case "ShowAdministrationDetails"
                    Dim ipaAdministration As IPAAdministration = JsonConvert.DeserializeObject(Of IPAAdministration)(ajaxModel.Value.First().ToString())
                    ipaAdministration = IPAService.GetAdministrationDetails(ipaAdministration.CodAmm).Result

                    AjaxManager.ResponseScripts.Add(String.Format(SHOW_ADMINISTRATION_DETAILS_CALLBACK, HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(ipaAdministration))))
            End Select
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("E' avvenuto un errore nella ricerca delle pubbliche amministrazioni")
            Exit Sub
        End Try
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf SelContattiIPA_AjaxRequest
    End Sub

    Private Function CreateAdministrationContact(ipaAdministration As IPAAdministration) As Contact
        Dim contact As New Contact With {
            .EmailAddress = ipaAdministration.Mail1,
            .CertifiedMail = ipaAdministration.Mail1,
            .Description = ipaAdministration.DesAmm,
            .Address = New Address
        }
        With contact.Address
            .ZipCode = StringHelper.Truncate(ipaAdministration.CAP, 20)
            .CityCode = StringHelper.Truncate(ipaAdministration.Provincia, 2)
            .Address = StringHelper.Truncate(ipaAdministration.Indirizzo, 60)
        End With
        contact.ContactType = New ContactType(ContactType.Ipa)
        Return contact
    End Function

    Private Function CreateAOOContact(ipaAOO As IPAAOO) As Contact
        Dim contact As New Contact With {
            .EmailAddress = ipaAOO.Mail1,
            .CertifiedMail = ipaAOO.Mail1,
            .Description = ipaAOO.DesAOO,
            .Address = New Address
        }
        With contact.Address
            .ZipCode = StringHelper.Truncate(ipaAOO.CAP, 20)
            .CityCode = StringHelper.Truncate(ipaAOO.Provincia, 2)
            .Address = StringHelper.Truncate(ipaAOO.Indirizzo, 60)
        End With
        contact.TelephoneNumber = StringHelper.Truncate(ipaAOO.Telefono, 50)
        contact.ContactType = New ContactType(ContactType.Ipa)
        Return contact
    End Function

    Private Function CreateOUContact(ipaOU As IPAOU) As Contact
        Dim contact As New Contact With {
            .EmailAddress = ipaOU.Mail1,
            .CertifiedMail = ipaOU.Mail1,
            .Description = ipaOU.DesOU,
            .Address = New Address
        }
        With contact.Address
            .ZipCode = StringHelper.Truncate(ipaOU.CAP, 20)
            .CityCode = StringHelper.Truncate(ipaOU.Provincia, 2)
            .Address = StringHelper.Truncate(ipaOU.Indirizzo, 60)
        End With
        contact.TelephoneNumber = StringHelper.Truncate(ipaOU.Telefono, 50)
        contact.ContactType = New ContactType(ContactType.Ipa)
        Return contact
    End Function

    Private Function InitializeIPAService() As Func(Of IPAService)
        Return Function()
                   Return New IPAService(ProtocolEnv.IPAWebServiceUrl, ProtocolEnv.IPAAuthId,
                              Sub(message)
                                  FileLogger.Info(LoggerName, message)
                              End Sub,
                              Sub(errorMessage)
                                  FileLogger.Error(LoggerName, errorMessage)
                              End Sub)
               End Function
    End Function
#End Region

End Class