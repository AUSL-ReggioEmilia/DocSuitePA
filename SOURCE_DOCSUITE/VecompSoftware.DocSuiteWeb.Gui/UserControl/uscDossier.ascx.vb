Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports DSW = VecompSoftware.DocSuiteWeb.Data
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports System.Linq

Public Class uscDossier
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const EXTERNAL_DATA_INITIALIZE_CALLBACK As String = "uscDossier.loadExternalDataCallback();"
#End Region

#Region " Properties "

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public CurrentWorkflowActivityId As String

#End Region


#Region " Events "

    Protected Sub uscDossier_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then

        End If
    End Sub


    Protected Sub UscDossierAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, "LoadExternalData") AndAlso ajaxModel IsNot Nothing AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then

            Dim roles As List(Of APICommons.Role) = JsonConvert.DeserializeObject(Of List(Of APICommons.Role))(ajaxModel.Value(0))
            LoadRoles(roles)

            If ajaxModel.Value(1) IsNot Nothing Then
                Dim contactsModel As List(Of ReferenceModel) = JsonConvert.DeserializeObject(Of List(Of ReferenceModel))(ajaxModel.Value(1))
                If contactsModel.Count < 1 Then
                    rowContact.Visible = False
                Else
                    LoadContacts(contactsModel)
                End If
            End If
        End If

        AjaxManager.ResponseScripts.Add(EXTERNAL_DATA_INITIALIZE_CALLBACK)
    End Sub

#End Region


#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscDossierAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscContatto)
    End Sub

    Private Sub LoadContacts(contacts As List(Of ReferenceModel))
        Dim references As IList(Of ContactDTO) = New List(Of ContactDTO)
        Dim mapper As MapperContactDTODSWEntity = New MapperContactDTODSWEntity()
        For Each contact As ReferenceModel In contacts
            Dim tempDTO As ContactDTO = mapper.MappingDTO(contact)
            tempDTO.Contact = Facade.ContactFacade.GetById(contact.EntityShortId)
            references.Add(tempDTO)
        Next
        uscContatto.DataSource = references
        uscContatto.DataBind()

    End Sub


    Private Sub LoadRoles(Roles As List(Of APICommons.Role))

        Dim mapper As MapperRoleDSWEntity = New MapperRoleDSWEntity()
        Dim dswRoles As IList(Of DSW.Role) = New List(Of DSW.Role)
        For Each role As APICommons.Role In Roles
            dswRoles.Add(mapper.MappingDTO(role))
        Next

        uscSettori.AddRoles(CType(dswRoles, IList(Of Data.Role)), True, False, False)
        uscSettori.SourceRoles.Clear()
    End Sub

#End Region

End Class