Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Logging

Public Class TbltCategoryMetadata
    Inherits CommonBasePage

#Region "Fields"
    Private _categoryId As Integer?
    Private _metadataRepositoryId As Guid?
#End Region

#Region "Properties"
    Public ReadOnly Property CategoryId As Integer?
        Get
            If Not _categoryId.HasValue Then
                If Request.QueryString("CategoryID") IsNot Nothing Then
                    _categoryId = Integer.Parse(Request.QueryString("CategoryID"))
                End If
            End If
            Return _categoryId
        End Get
    End Property

    Public ReadOnly Property MetadataRepositoryId As Guid?
        Get
            If Not _metadataRepositoryId.HasValue AndAlso Request.QueryString("IdMetadata") IsNot Nothing Then
                _metadataRepositoryId = Guid.Parse(Request.QueryString("IdMetadata"))
            End If
            Return _metadataRepositoryId
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblCategoryRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        InitializeAjax()
        If Not Page.IsPostBack Then
            MasterDocSuite.TitleVisible = False
        End If
    End Sub

    Protected Sub TbltCategoryMetadata_AjaxRequest(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel IsNot Nothing Then
            Select Case ajaxModel.ActionName
                Case "AddMetadata"
                    If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 AndAlso ajaxModel.Value(0) IsNot Nothing AndAlso Guid.Parse(ajaxModel.Value(0)) <> Guid.Empty Then
                        Try
                            Dim category As Category = Facade.CategoryFacade.GetById(CategoryId.Value)
                            category.IdMetadataRepository = Guid.Parse(ajaxModel.Value(0))
                            Facade.CategoryFacade.Update(category)
                            FileLogger.Info(LoggerName, String.Concat("Associato MetadataRepository con id ", ajaxModel.Value(0), " al classificatore ", category.Name))
                            AjaxManager.ResponseScripts.Add("tbltCategoryMetadata.closeWindow();")
                        Catch ex As Exception
                            FileLogger.Error(LoggerName, String.Concat("Errore in associazione metadata a classificatore: ", ex.Message), ex)
                            AjaxAlert("Attenzione: è avvenuto un errore nell'associazione dei metadati al classificatore selezionato.")
                        End Try
                    End If
            End Select
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltCategoryMetadata_AjaxRequest
    End Sub
#End Region

End Class