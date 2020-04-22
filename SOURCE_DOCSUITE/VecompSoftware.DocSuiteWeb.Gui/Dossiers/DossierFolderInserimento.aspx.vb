Imports System.Collections.Generic
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports DSW = VecompSoftware.DocSuiteWeb.Data


Public Class DossierFolderInserimento
    Inherits DossierBasePage

#Region " Fields "
    Private Const DOSSIERFOLDER_INSERT_CALLBACK As String = "dossierFolderInserimento.insertDossierFolder('{0}', '{1}');"
    Private Const CATEGORY_CHANGE_HANDLER As String = "dossierFolderInserimento.onCategoryChanged({0});"
    Private Const LOAD_EXTERNAL_DATA As String = "DossierFolderInserimentoLoadExternalData"
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
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DossierFolderInserimento_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscFolderAccounted, uscFolderAccounted)
    End Sub

    Protected Sub DossierFolderInserimento_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        Select Case ajaxModel.ActionName
            Case LOAD_EXTERNAL_DATA
                If ajaxModel.Value IsNot Nothing Then
                    Dim roles As List(Of APICommons.Role) = JsonConvert.DeserializeObject(Of List(Of APICommons.Role))(ajaxModel.Value(0))
                    LoadRoles(roles)
                End If
                Exit Select
        End Select
    End Sub

    Private Sub LoadRoles(Roles As List(Of APICommons.Role))

        Dim mapper As MapperRoleDSWEntity = New MapperRoleDSWEntity()
        Dim dswRoles As IList(Of DSW.Role) = New List(Of DSW.Role)
        For Each role As APICommons.Role In Roles
            dswRoles.Add(mapper.MappingDTO(role))
        Next

        uscFolderAccounted.SourceRoles.Clear()
        uscFolderAccounted.SourceRoles.AddRange(CType(dswRoles, IList(Of Data.Role)))
        uscFolderAccounted.DataBind(False, False)
    End Sub
#End Region

End Class