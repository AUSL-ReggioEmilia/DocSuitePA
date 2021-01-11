Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DossierFascicleFolderInserimento
    Inherits DossierBasePage

#Region " Fields "
    Private Const DOSSIERFOLDER_INSERT_CALLBACK As String = "dossierFascicleFolderInserimento.insertDossierFolder({0}, '{1}', '{2}');"
    Private _fascicleInsertControlClientId As String
#End Region

#Region " Properties "
    Protected ReadOnly Property PersistanceDisabled As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("PersistanceDisabled", False)
        End Get
    End Property

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public ReadOnly Property FascicleInsertControlClientId As String
        Get
            If String.IsNullOrEmpty(_fascicleInsertControlClientId) Then
                If ProtocolEnv.ProcessEnabled Then
                    _fascicleInsertControlClientId = UscFascicleProcessInsert.PageContentDiv.ClientID
                Else
                    _fascicleInsertControlClientId = UscFascicleInsert.PageContentDiv.ClientID
                End If
            End If
            Return _fascicleInsertControlClientId
        End Get
    End Property

    Private ReadOnly Property UscFascicleInsert As uscFascicleInsert
        Get
            Return DirectCast(dynamicUscFascicleInsertControls.FindControl("uscFascicleInsert"), uscFascicleInsert)
        End Get
    End Property

    Private ReadOnly Property UscFascicleProcessInsert As uscFascicleProcessInsert
        Get
            Return DirectCast(dynamicUscFascicleInsertControls.FindControl("uscFascicleProcessInsert"), uscFascicleProcessInsert)
        End Get
    End Property

#End Region

#Region " Events "
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not ProtocolEnv.ProcessEnabled Then
            Dim uscFascicleInsert As uscFascicleInsert = DirectCast(Page.LoadControl("~/UserControl/uscFascicleInsert.ascx"), uscFascicleInsert)
            uscFascicleInsert.ID = "uscFascicleInsert"
            uscFascicleInsert.ValidationDisabled = PersistanceDisabled
            dynamicUscFascicleInsertControls.Controls.Add(uscFascicleInsert)
        Else
            Dim uscFascicleProcessInsert As uscFascicleProcessInsert = DirectCast(Page.LoadControl("~/UserControl/uscFascicleProcessInsert.ascx"), uscFascicleProcessInsert)
            uscFascicleProcessInsert.ID = "uscFascicleProcessInsert"
            uscFascicleProcessInsert.ValidationDisabled = PersistanceDisabled
            dynamicUscFascicleInsertControls.Controls.Add(uscFascicleProcessInsert)
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            If Not ProtocolEnv.ProcessEnabled Then
                UscFascicleInsert.PageContentDiv = PageContentDiv
            End If
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DossierFascicleFolderInserimento_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub

    Protected Sub DossierFascicleFolderInserimento_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

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
            Case "Insert"
                Dim contactId As Integer = 0
                If UscFascicleInsert.RespContactDTO IsNot Nothing AndAlso UscFascicleInsert.RespContactDTO.Contact IsNot Nothing Then
                    contactId = UscFascicleInsert.RespContactDTO.Contact.Id
                End If
                Dim metadataModel As Tuple(Of MetadataDesignerModel, ICollection(Of MetadataValueModel)) = Nothing
                If ProtocolEnv.MetadataRepositoryEnabled Then
                    metadataModel = UscFascicleInsert.GetDynamicValues()
                End If
                AjaxManager.ResponseScripts.Add(String.Format(DOSSIERFOLDER_INSERT_CALLBACK, contactId,
                                                              If(metadataModel IsNot Nothing, JsonConvert.SerializeObject(metadataModel.Item1).Replace("'", "\'"), Nothing),
                                                              If(metadataModel IsNot Nothing, JsonConvert.SerializeObject(metadataModel.Item2).Replace("'", "\'"), Nothing)))
                Exit Select
        End Select
    End Sub

#End Region

End Class