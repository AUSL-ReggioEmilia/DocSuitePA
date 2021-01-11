Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Services.AnagraficaANAS.Models
Imports VecompSoftware.Services.Logging

Partial Class FascModifica
    Inherits FascBasePage

#Region "Fields"
    Private Const FASCICLE_INITIALIZE_CALLBACK As String = "fascModifica.initializeCallback();"
    Private Const FASCICLE_UPDATE_CALLBACK As String = "fascModifica.updateCallback('{0}');"
#End Region

#Region "Properties"
    Public ReadOnly Property FasciclesPanelVisibilities As String
        Get
            Return JsonConvert.SerializeObject(ProtocolEnv.FasciclesPanelVisibilities)
        End Get
    End Property
#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        uscSetiContact.MetadataEditId = UscDynamicMetadataRest.PageContent.ClientID

        InitializeAjax()
        If Not IsPostBack Then
            uscFascicolo.UscDocumentReference.Visible = False
            uscFascicolo.CurrentFascicleId = IdFascicle
            uscContattiResp.Caption = DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel
            uscContattiResp.RequiredErrorMessage = $"Inserire un {DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel}"
        End If
        uscContact.FilterByParentId = ProtocolEnv.FascicleContactId
    End Sub

    Protected Sub FascModifica_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel IsNot Nothing Then
            Try
                Select Case ajaxModel.ActionName
                    Case "Initialize"
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
                            Dim fascicle As Entity.Fascicles.Fascicle = JsonConvert.DeserializeObject(Of Entity.Fascicles.Fascicle)(ajaxModel.Value(0))
                            Initialize(fascicle)
                        End If

                    Case "Update"
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
                            Dim contact As ContactDTO = uscContattiResp.GetContacts(False).FirstOrDefault()
                            AjaxManager.ResponseScripts.Add(String.Format(FASCICLE_UPDATE_CALLBACK, contact?.Contact?.Id))
                        End If

                End Select
            Catch ex As Exception
                FileLogger.Error(LoggerName, "Errore nel caricamento dei dati del fascicolo.", ex)
                AjaxManager.Alert("Errore nel caricamento dei dati del fascicolo.")
                Return
            End Try

        End If
    End Sub
#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascModifica_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscContattiResp)
    End Sub

    Private Sub Initialize(fascicle As Entity.Fascicles.Fascicle)
        LoadRiferimenti(fascicle)
        uscContattiResp.SearchInCategoryContacts = Nothing
        uscContattiResp.CategoryContactsProcedureType = String.Empty
        uscContattiResp.IsRequired = False
        If fascicle.FascicleType = FascicleType.Procedure Then
            uscContattiResp.IsRequired = True
            uscContattiResp.SearchInCategoryContacts = fascicle.Category.EntityShortId
            uscContattiResp.CategoryContactsProcedureType = RoleUserType.RP.ToString()
            uscContattiResp.Initialize()
        End If
        AjaxManager.ResponseScripts.Add(FASCICLE_INITIALIZE_CALLBACK)
    End Sub

    Private Sub LoadRiferimenti(fascicle As Entity.Fascicles.Fascicle)
        Dim riferimenti As IList(Of ContactDTO) = New List(Of ContactDTO)
        For Each fascicleContact As Entity.Commons.Contact In fascicle.Contacts
            Dim tmpDto As ContactDTO = New ContactDTO()
            Dim contact As Contact = Facade.ContactFacade.GetById(fascicleContact.EntityId)

            tmpDto.Contact = contact
            tmpDto.Type = ContactDTO.ContactType.Address
            tmpDto.Id = contact.Id
            riferimenti.Add(tmpDto)
        Next

        uscContattiResp.DataSource = riferimenti
        uscContattiResp.DataBind()
    End Sub

#End Region

End Class



