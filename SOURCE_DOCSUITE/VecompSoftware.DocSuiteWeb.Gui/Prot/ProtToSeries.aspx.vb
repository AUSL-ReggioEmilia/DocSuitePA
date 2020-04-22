Imports System.Collections.Generic
Imports System.ComponentModel
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports System.Linq
Imports VecompSoftware.Services.Biblos.Models

Namespace Prot

    Public Class ProtToSeries
        Inherits ProtBasePage

#Region " Fields "

        Private _allDocuments As List(Of DocumentInfo)
        Private _currentDocumentSeriesId As String

#End Region

#Region " Properties "
        Private ReadOnly Property AllDocuments() As List(Of DocumentInfo)
            Get
                If _allDocuments Is Nothing Then
                    _allDocuments = New List(Of DocumentInfo)
                    Dim mainDoc As IList(Of DocumentInfo) = New List(Of DocumentInfo)
                    mainDoc.Add(Facade.ProtocolFacade.GetDocument(CurrentProtocol))
                    _allDocuments.AddRange(uscToSeries.GetWithDummyParent("Documenti", mainDoc))
                    _allDocuments.AddRange(uscToSeries.GetWithDummyParent("Allegati", Facade.ProtocolFacade.GetAttachments(CurrentProtocol, False)))
                    _allDocuments.AddRange(uscToSeries.GetWithDummyParent("Annessi", Facade.ProtocolFacade.GetAnnexes(CurrentProtocol)))
                End If

                Return _allDocuments
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            SetResponseNoCache()
            InitializeAjax()
            If Not Page.IsPostBack Then
                Dim sessionParamDocs As Object = Session("ProtSelectedDocuments")
                If sessionParamDocs IsNot Nothing Then
                    Session.Remove("ProtSelectedDocuments")
                End If

                lblNumber.Text = Facade.ProtocolFacade.GetProtocolNumber(CurrentProtocolYear, CurrentProtocolNumber)
                uscToSeries.BindContainers(Nothing)
                BindDocuments()
            End If
        End Sub

        Private Sub uscToSeries_NeedDocumentsSource(ByVal sender As Object, ByVal e As EventArgs) Handles uscToSeries.NeedDocumentsSource
            BindDocuments()
        End Sub

        Protected Sub btnConfirmClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
            Session("ProtSelectedDocuments") = uscToSeries.SelectedDocuments
            Response.Redirect(String.Concat("~/Series/Item.aspx?", CommonShared.AppendSecurityCheck(String.Format("Type=Series&Action={0}&ProtYear={1}&ProtNumber={2}&DocumentSeriesId={3}", DocumentSeriesAction.FromProtocol.ToString(), CurrentProtocol.Year, CurrentProtocol.Number, uscToSeries.SelectedDocumentSeriesId))))
        End Sub
#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, uscToSeries, MasterDocSuite.AjaxDefaultLoadingPanel)
        End Sub

        ''' <summary> caricamento documenti </summary>
        Private Sub BindDocuments()
            uscToSeries.DocumentSource = AllDocuments
            uscToSeries.BindDocuments()
        End Sub

#End Region

    End Class

End Namespace