Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.Services.Biblos.Models

Public Class MultipleSingleSign
    Inherits MultipleSignBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub DocumentListGrid_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim bound As MultiSignDocumentInfo = DirectCast(e.Item.DataItem, MultiSignDocumentInfo)

        Dim documentImage As Image = DirectCast(e.Item.FindControl("documentType"), Image)
        documentImage.ImageUrl = ImagePath.FromDocumentInfo(bound.DocumentInfo, True)

        Dim documentName As Label = DirectCast(e.Item.FindControl("lblFileName"), Label)
        documentName.Text = bound.DocumentInfo.Name

        Dim lblDocType As Label = DirectCast(e.Item.FindControl("lblDocType"), Label)
        lblDocType.Text = bound.DocType

        Dim btnSign As RadButton = DirectCast(e.Item.FindControl("btnSign"), RadButton)
        btnSign.Icon.PrimaryIconUrl = ImagePath.SmallSigned
        btnSign.CommandArgument = bound.ToQueryString().AsEncodedQueryString()

    End Sub

    Protected Sub Page_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim signed As TempFileDocumentInfo = CType(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(e.Argument)), TempFileDocumentInfo)

        If Not signed.FileInfo.Exists Then
            AjaxAlert(String.Format("Documento firmato non è valido, reinserire il documento. {0}", ProtocolEnv.DefaultErrorMessage), False)
            Exit Sub
        End If

        Dim newDocument As New MultiSignDocumentInfo(HttpUtility.ParseQueryString(hiddenPezza.Value))
        newDocument.DocumentInfo = signed

        Dim documents As List(Of MultiSignDocumentInfo) = SignedDocuments
        If documents.IsNullOrEmpty() Then
            documents = New List(Of MultiSignDocumentInfo)
        End If

        documents.Add(newDocument)

        SignedDocuments = documents
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        MyBase.SignedComplete = False
        If MyBase.SaveResponseToSession Then
            MyBase.SignedComplete = True
        End If
        ' Trasferisco la response sulla pagina che dovrà occuparsi di processare i documenti firmati
        Server.Transfer(PostBackUrl, False)
    End Sub

    Private Sub btnUndo_Click(sender As Object, e As EventArgs) Handles btnUndo.Click
        SignedDocuments = New List(Of MultiSignDocumentInfo)
        Response.Redirect(String.Format("{0}&{1}=true", PostBackUrl, MultipleSign.MultiSignUndoQuerystring))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf Page_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, hiddenPezza)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, hiddenPezza)
    End Sub

    Private Sub Initialize()
        If Not ProtocolEnv.EnableMultiSign Then
            Throw New DocSuiteException("Impossibile aprire pagina", String.Format("Firma multipla non abilitata. {0}", ProtocolEnv.DefaultErrorMessage))
        End If

        Dim sourcePage As ISignMultipleDocuments = TryCast(PreviousPage, ISignMultipleDocuments)
        If sourcePage Is Nothing Then
            Throw New DocSuiteException("Firma Multipla", "Pagina di provenienza errata.")
        End If

        OriginalDocuments = sourcePage.DocumentsToSign.ToList()
        If OriginalDocuments.IsNullOrEmpty() Then
            Throw New DocSuiteException("Firma Multipla", "Nessun documento da firmare.")
        End If

        DocumentListGrid.DataSource = OriginalDocuments
        DocumentListGrid.DataBind()

        PostBackUrl = sourcePage.ReturnUrl
    End Sub

#End Region

End Class