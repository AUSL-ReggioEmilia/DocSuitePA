Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons

Public Class DossierModifica
    Inherits DossierBasePage

#Region " Fields "
    Private Const DOSSIER_MODIFY_CALLBACK As String = "dossierModifica.updateCallback('{0}');"
    Private Const DOSSIER_END_LOAD_DATA_CALLBACK As String = "dossierModifica.endLoadingDataCallback();"
    Private _currentDossierDocumentLocation As Location
    Private _argument As String = String.Empty
#End Region

#Region " Properties "
    Protected ReadOnly Property CurrentDossierDocumentLocation As Location
        Get
            If _currentDossierDocumentLocation Is Nothing Then
                _currentDossierDocumentLocation = Facade.LocationFacade.GetById(ProtocolEnv.DossierMiscellaneaLocation)
            End If
            Return _currentDossierDocumentLocation
        End Get
    End Property

#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack() Then

        End If
    End Sub

    Protected Sub btnConfirm_OnClick(sender As Object, e As EventArgs) Handles btnConferma.Click

        'RIFERIMENTI
        Dim contacts As IList(Of ContactDTO) = uscContattiSel.GetContacts(False)
        Dim contactJson As String = String.Empty
        If contacts IsNot Nothing AndAlso contacts.Any() Then
            Dim contactIds As Integer() = contacts.Select(Function(x) x.Contact.Id).ToArray()
            contactJson = JsonConvert.SerializeObject(contactIds)
        End If

        AjaxManager.ResponseScripts.Add(String.Format(DOSSIER_MODIFY_CALLBACK, contactJson))
    End Sub


    Protected Sub DossierModifica_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, "LoadExternalData") AndAlso ajaxModel IsNot Nothing AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
            'CARICAMENTO RIFERIMENTI
            Dim contactsModel As List(Of ReferenceModel) = JsonConvert.DeserializeObject(Of List(Of ReferenceModel))(ajaxModel.Value(0))
            LoadContacts(contactsModel)
        End If

        AjaxManager.ResponseScripts.Add(String.Format(DOSSIER_END_LOAD_DATA_CALLBACK))
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DossierModifica_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscContattiSel, uscContattiSel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub

    Private Sub LoadContacts(contacts As List(Of ReferenceModel))
        Dim references As IList(Of ContactDTO) = New List(Of ContactDTO)
        Dim mapper As MapperContactDTODSWEntity = New MapperContactDTODSWEntity()
        For Each contact As ReferenceModel In contacts
            references.Add(mapper.MappingDTO(contact))
        Next
        uscContattiSel.DataSource = references
        uscContattiSel.DataBind()
    End Sub

#End Region

End Class