Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DossierFascicleFolderInserimento
    Inherits DossierBasePage

#Region " Fields "
    Private Const DOSSIERFOLDER_INSERT_CALLBACK As String = "dossierFascicleFolderInserimento.insertDossierFolder('{0}', '{1}');"
    Private Const CATEGORY_CHANGE_HANDLER As String = "dossierFascicleFolderInserimento.onCategoryChanged({0});"
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

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        uscFascicleInsert.ValidationDisabled = PersistanceDisabled
        If Not IsPostBack Then
            uscFascicleInsert.PageContentDiv = PageContentDiv
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
                Dim contactId As Integer
                If uscFascicleInsert.RespContactDTO IsNot Nothing AndAlso uscFascicleInsert.RespContactDTO.Contact IsNot Nothing Then
                    contactId = uscFascicleInsert.RespContactDTO.Contact.Id
                End If
                Dim metadataModel As MetadataModel = Nothing
                If ProtocolEnv.MetadataRepositoryEnabled Then
                    metadataModel = uscFascicleInsert.GetDynamicValues()
                End If
                AjaxManager.ResponseScripts.Add(String.Format(DOSSIERFOLDER_INSERT_CALLBACK, JsonConvert.SerializeObject(contactId),
                                                               If(metadataModel IsNot Nothing, JsonConvert.SerializeObject(metadataModel), Nothing)))
                Exit Select
        End Select
    End Sub

#End Region

End Class