Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports DSW = VecompSoftware.DocSuiteWeb.Data

Public Class DossierFolderModifica
    Inherits DossierBasePage

#Region " Fields "

    Private _idDossierFolder As Guid?
    Private Const CATEGORY_CHANGE_HANDLER As String = "dossierFolderModifica.onCategoryChanged({0});"
    Private Const EXTERNAL_DATA_INITIALIZE_CALLBACK As String = "dossierFolderModifica.loadExternalDataCallback();"

#End Region

#Region " Properties "
    Protected ReadOnly Property PersistanceDisabled As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("PersistanceDisabled", False)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        uscRole.PropagateAuthorizationsEnabled = True
    End Sub

    Private Sub uscClassificatore_CategoryChange(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryAdded, uscCategory.CategoryRemoved
        If Not uscCategory.HasSelectedCategories Then
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, 0))
            Exit Sub
        End If

        Dim category As Category = uscCategory.SelectedCategories.First()

        If category Is Nothing Then
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, 0))
            Exit Sub
        End If

        AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, category.Id))
    End Sub

    Protected Sub DossierModificaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If "LoadExternalData".Equals(ajaxModel.ActionName) AndAlso ajaxModel IsNot Nothing AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then

            Try
                If ajaxModel.Value(0) IsNot Nothing Then
                    Dim roles As List(Of APICommons.Role) = JsonConvert.DeserializeObject(Of List(Of APICommons.Role))(ajaxModel.Value(0))
                    LoadRoles(roles)
                End If

                If ajaxModel.Value(1) IsNot Nothing Then
                    Dim idCategory As Integer = JsonConvert.DeserializeObject(Of Integer)(ajaxModel.Value(1))
                    uscCategory.DataSource.Add(Facade.CategoryFacade.GetById(idCategory))
                    uscCategory.DataBind()
                End If
            Catch
                AjaxManager.Alert("Anomalia critica nell'esecuzione della richiesta. Contattare assistenza.")
            End Try
        End If

        AjaxManager.ResponseScripts.Add(EXTERNAL_DATA_INITIALIZE_CALLBACK)
    End Sub



#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DossierModificaAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscRole, uscRole)
    End Sub

    Private Sub LoadRoles(roles As List(Of APICommons.Role))

        Dim mapper As MapperRoleDSWEntity = New MapperRoleDSWEntity()
        Dim dswRoles As IList(Of DSW.Role) = New List(Of DSW.Role)
        For Each role As APICommons.Role In roles
            dswRoles.Add(mapper.MappingDTO(role))
        Next

        uscRole.SourceRoles.Clear()
        uscRole.SourceRoles.AddRange(CType(dswRoles, IList(Of Data.Role)))
        uscRole.DataBind(False, False)
    End Sub
#End Region

End Class