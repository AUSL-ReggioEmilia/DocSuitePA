Imports System.Web
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers

Public Class DossierRisultati
    Inherits CommonBasePage
#Region " Properties "
    Public ReadOnly Property IsWindowPopupEnable As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault("IsWindowPopupEnable", False)
        End Get
    End Property
    Public ReadOnly Property DynamicMetadataEnabled As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault("DynamicMetadataEnabled", False)
        End Get
    End Property
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DossierRisultati_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDossierGrid.Grid)
    End Sub
#End Region

#Region " Events "
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            uscDossierGrid.IsWindowPopupEnable = IsWindowPopupEnable
        End If
    End Sub

    Protected Sub DossierRisultati_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            If ajaxModel Is Nothing Then
                Return
            End If
            Select Case ajaxModel.ActionName
                Case "GenerateColumns"
                    If ajaxModel.Value(0) IsNot Nothing Then
                        Dim dossierMoldels As List(Of DossierModel) = JsonConvert.DeserializeObject(Of List(Of DossierModel))(ajaxModel.Value(0), New JsonSerializerSettings() With {
                                                                                                       .NullValueHandling = NullValueHandling.Ignore,
                                                                                                       .TypeNameHandling = TypeNameHandling.Objects,
                                                                                                       .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                                                                       .PreserveReferencesHandling = PreserveReferencesHandling.All,
                                                                                                       .DateFormatString = "dd/MM/yyyy"
                                                                                                       })
                        Dim metadataColumns As List(Of BaseFieldModel) = New List(Of BaseFieldModel)

                        For Each dossierModel As DossierModel In dossierMoldels
                            If dossierModel.MetadataDesigner IsNot Nothing Then
                                Dim metadataModel As MetadataDesignerModel = JsonConvert.DeserializeObject(Of MetadataDesignerModel)(dossierModel.MetadataDesigner)

                                If metadataModel IsNot Nothing Then
                                    metadataColumns.AddRange(metadataModel.BoolFields.Where(Function(x) x.ShowInResults).ToList())
                                    metadataColumns.AddRange(metadataModel.ContactFields.Where(Function(x) x.ShowInResults).ToList())
                                    metadataColumns.AddRange(metadataModel.DateFields.Where(Function(x) x.ShowInResults).ToList())
                                    metadataColumns.AddRange(metadataModel.DiscussionFields.Where(Function(x) x.ShowInResults).ToList())
                                    metadataColumns.AddRange(metadataModel.EnumFields.Where(Function(x) x.ShowInResults).ToList())
                                    metadataColumns.AddRange(metadataModel.NumberFields.Where(Function(x) x.ShowInResults).ToList())
                                    metadataColumns.AddRange(metadataModel.TextFields.Where(Function(x) x.ShowInResults).ToList())
                                End If
                            End If
                        Next

                        For Each metadataColumn As BaseFieldModel In metadataColumns
                            If uscDossierGrid.Grid.Columns.FindByUniqueNameSafe(metadataColumn.KeyName) Is Nothing Then
                                Dim gridColumn As GridBoundColumn = New GridBoundColumn()
                                gridColumn.HeaderText = metadataColumn.KeyName
                                gridColumn.DataField = metadataColumn.KeyName
                                gridColumn.UniqueName = metadataColumn.KeyName
                                uscDossierGrid.Grid.MasterTableView.Columns.Add(gridColumn)
                            End If
                        Next
                        uscDossierGrid.Grid.DataSource = New List(Of Object)
                        uscDossierGrid.Grid.DataBind()
                        AjaxManager.ResponseScripts.Add(String.Format("generateColumnsCallback();"))
                    End If
                    Exit Select
            End Select
        Catch
            Exit Sub
        End Try
    End Sub
#End Region

End Class