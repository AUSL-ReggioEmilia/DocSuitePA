Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models

Namespace Viewers

    Public Class DocumentSeriesItemViewer
        Inherits BaseViewer

#Region " Fields "

        Private _documentSeriesItemId As Integer?
        Private _documentSeriesItemIdentifiers As List(Of Integer)
        Private _documentSeriesItem As IList(Of DocumentSeriesItem)

#End Region

#Region " Properties "

        Protected Overrides ReadOnly Property CurrentViewer As ViewerLight
            Get
                Return ViewerLight
            End Get
        End Property

        Private ReadOnly Property DocumentSeriesItemId As Integer?
            Get
                If Not _documentSeriesItemId.HasValue Then
                    If Not String.IsNullOrEmpty(Request.QueryString.Item("id")) Then
                        ' Altrimenti dallo specifico parametro.
                        _documentSeriesItemId = Request.QueryString.GetValue(Of Integer)("id")
                    End If
                End If
                Return _documentSeriesItemId
            End Get
        End Property

        Private ReadOnly Property DocumentSeriesItemIdentifiers As List(Of Integer)
            Get
                If _documentSeriesItemIdentifiers Is Nothing Then
                    Dim ids As List(Of Integer) = Nothing
                    If Not String.IsNullOrEmpty(Request.QueryString.Item("ids")) Then
                        ' Altrimenti dallo specifico parametro.
                        Dim queryStringValue As String = Server.UrlDecode(Request.QueryString.GetValue(Of String)("ids"))
                        ids = JsonConvert.DeserializeObject(Of List(Of Integer))(queryStringValue)
                    End If
                    If ids IsNot Nothing AndAlso ids.Count > 0 Then
                        ' Inizializzo solo se mi è stato fornito un elenco valido.
                        _documentSeriesItemIdentifiers = ids
                    End If
                End If
                Return _documentSeriesItemIdentifiers
            End Get
        End Property

        Private ReadOnly Property DocumentSeriesItemList As IList(Of DocumentSeriesItem)
            Get
                If _documentSeriesItem Is Nothing Then
                    If DocumentSeriesItemId.HasValue Then
                        ' Valorizzo a partire dal particolare.
                        _documentSeriesItem = New List(Of DocumentSeriesItem) From {Facade.DocumentSeriesItemFacade.GetById(DocumentSeriesItemId.Value)}
                    ElseIf DocumentSeriesItemIdentifiers IsNot Nothing Then
                        ' Altrimenti dallo specifico parametro.
                        _documentSeriesItem = Facade.DocumentSeriesItemFacade.GetListByIds(DocumentSeriesItemIdentifiers)
                    Else
                        ' Genero errore qualora non abbia a disposizione nessuna collaborazione valida.
                        Throw New DocSuiteException("Errore inizializzazione", String.Format("Nessuna {0} specificata.", ProtocolEnv.DocumentSeriesName))
                    End If
                End If
                Return _documentSeriesItem
            End Get
        End Property

        Protected Overrides ReadOnly Property Allowed() As Boolean
            Get
                For Each item As DocumentSeriesItem In DocumentSeriesItemList
                    If Not (New DocumentSeriesItemRights(item)).IsReadable Then
                        Return False
                    End If
                Next
                Return True
            End Get
        End Property
#End Region

#Region " Methods "
        Protected Overrides Function GetDataSource() As IList(Of DocumentInfo)
            If DocumentSeriesItemList IsNot Nothing AndAlso DocumentSeriesItemList.Count > 0 Then
                Dim dataSource As New List(Of DocumentInfo)
                For Each item As DocumentSeriesItem In DocumentSeriesItemList
                    Dim source As DocumentInfo = Facade.DocumentSeriesItemFacade.GetDocumentSeriesItemViewerSource(item, ProtocolEnv.DocumentSeriesDocumentsLabel, ProtocolEnv.DocumentSeriesReorderDocumentEnabled)
                    dataSource.Add(source)
                Next
                Return dataSource
            End If
            Return Nothing
        End Function

#End Region

#Region " Events "

        Protected Overridable Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            MasterDocSuite.TitleVisible = False
            If (DocumentSeriesItemList.Count = 1) Then
                Dim doc As DocumentSeriesItem = DocumentSeriesItemList.First()
                InitializeAjax()

                Dim currentDocumentSeriesRights As DocumentSeriesItemRights = New DocumentSeriesItemRights(doc)
                If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso doc.DocumentSeries.Container.PrivacyEnabled AndAlso currentDocumentSeriesRights.IsEditable Then
                    ViewerLight.ModifyPrivacyEnabled = True
                    ViewerLight.CurrentDocumentUnitID = doc.UniqueId
                    ViewerLight.CurrentLocationId = doc.Location.Id
                End If

                If doc.Number Is Nothing OrElse doc.Year Is Nothing Then
                    ViewerLight.PrefixFileName = String.Empty
                Else
                    ViewerLight.PrefixFileName = String.Concat("DOC", doc.Year, "_", doc.Number.ToString("0000000"))
                End If
            End If
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("DocumentSeriesItemViewer")
        End Sub

#End Region

#Region " Methods "
        Protected Sub ManagerAjaxRequests(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
            Dim arguments As String() = Split(e.Argument, "|")

            Dim source As List(Of DocumentInfo) = GetDataSource()
            ViewerLight.ReloadViewer(arguments, source)
        End Sub

        Private Sub InitializeAjax()
            AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf ManagerAjaxRequests
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        End Sub
#End Region
    End Class

End Namespace