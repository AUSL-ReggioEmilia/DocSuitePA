Imports System.Linq
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models

Namespace Series
    Public Class SearchResult
        Inherits CommonBasePage

#Region " Fields "

        Private Const SelectIdentifierAttribute As String = "selectidentifier"
        Private Const YearNumberColumn As String = "YearNumber"
        Private Const MainDocumentColumn As String = "MainDocument"

        Private _finder As DocumentSeriesItemFinder
        Dim _dataSource As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))
        Private _rolesLabelDictionary As IDictionary(Of Integer, List(Of String))
        Private _draftFinder As DocumentSeriesItemFinder

#End Region

#Region " Properties "

        Public ReadOnly Property ShowDraft As Boolean
            Get
                Return Request.QueryString.GetValueOrDefault(Of Boolean)("Draft", False)
            End Get
        End Property

        Public ReadOnly Property LimitDraftToSeries As Integer?
            Get
                Return Request.QueryString.GetValueOrDefault(Of Integer?)("LimitDraftToSeries", Nothing)
            End Get
        End Property

        Public ReadOnly Property ViewDraftAssociatedResolution As Boolean
            Get
                Return Request.QueryString.GetValueOrDefault(Of Boolean)("ViewDraftAssociatedResolution", True)
            End Get
        End Property

        'Recupera dalla sessione l'ultima ricerca di serie documentali effettuata
        Private ReadOnly Property Finder As DocumentSeriesItemFinder
            Get
                If _finder Is Nothing Then
                    _finder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.DocumentSeriesFinderType), DocumentSeriesItemFinder)
                End If
                Return _finder
            End Get
        End Property

        'Recupera il finder per la gestione delle Bozze
        Private ReadOnly Property DraftFinder As DocumentSeriesItemFinder
            Get
                If _draftFinder Is Nothing Then
                    _draftFinder = New DocumentSeriesItemFinder()
                    _draftFinder.ItemStatusIn = New List(Of DocumentSeriesItemStatus)() From {DocumentSeriesItemStatus.Draft}
                    If LimitDraftToSeries.HasValue Then
                        _draftFinder.IdDocumentSeriesIn = New List(Of Integer)() From {LimitDraftToSeries.Value}
                    End If
                    _draftFinder.ViewDraftAssociatedResolution = ViewDraftAssociatedResolution
                End If
                Return _draftFinder
            End Get
        End Property

        Private ReadOnly Property DataSource As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))
            Get
                If _dataSource Is Nothing Then
                    _dataSource = CType(grdDocumentSeriesItem.DataSource, IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo)))
                End If
                Return _dataSource
            End Get
        End Property

        Private ReadOnly Property RolesLabelDictionary As IDictionary(Of Integer, List(Of String))
            Get
                If _rolesLabelDictionary Is Nothing Then
                    _rolesLabelDictionary = Facade.DocumentSeriesItemRoleFacade.GetRoleLabelsDictionary(DataSource.Select(Function(i) i.Id).ToList())
                End If
                Return _rolesLabelDictionary
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Action.Eq("CopyDocuments") Then
                MasterDocSuite.TitleVisible = False
                ButtonsWrapper.Visible = False
            End If

            InitializeAjax()

            If Not IsPostBack Then
                Initialize()
            End If
        End Sub

        Private Sub AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
            ' TODO: strano questo caricamento, provare a toglierlo
            If e.Argument.Eq("InitialPageLoad") Then
                InitializeGrid()
            End If
        End Sub

        Private Sub grdDocumentSeriesItem_DataBound(sender As Object, e As EventArgs) Handles grdDocumentSeriesItem.DataBound
            If grdDocumentSeriesItem IsNot Nothing Then
                Title = String.Format("{0} - Risultati ({1}/{2})", ProtocolEnv.DocumentSeriesName, grdDocumentSeriesItem.DataSource.Count, grdDocumentSeriesItem.VirtualItemCount)
            Else
                Title = ProtocolEnv.DocumentSeriesName & " - Risultati"
            End If

            MasterDocSuite.Title = Title
            MasterDocSuite.HistoryTitle = Title
            AjaxManager.ResponseScripts.Add("if(GetRadWindow()) {GetRadWindow().set_title(""" & Title & """);}")
        End Sub

        Private Sub grdDocumentSeriesItem_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles grdDocumentSeriesItem.ItemCommand
            Select Case e.CommandName
                Case "ViewDocumentSeriesItem"
                    Dim parameters As String = String.Format("IdDocumentSeriesItem={0}&Action={1}&Type=Series", e.CommandArgument, DocumentSeriesAction.View)
                    Response.Redirect("~/Series/Item.aspx?" & CommonShared.AppendSecurityCheck(parameters))
            End Select
        End Sub

        Private Sub GrdDocumentSeriesItemItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grdDocumentSeriesItem.ItemDataBound
            If Not (e.Item.ItemType = GridItemType.Item OrElse e.Item.ItemType = GridItemType.AlternatingItem) Then
                Return
            End If

            Dim dto As DocumentSeriesItemDTO(Of BiblosDocumentInfo) = CType(e.Item.DataItem, DocumentSeriesItemDTO(Of BiblosDocumentInfo))

            Dim chkSelect As CheckBox = CType(e.Item.FindControl("chkSelect"), CheckBox)
            chkSelect.Attributes.Add(SelectIdentifierAttribute, dto.Id.ToString())

            Dim lbtYearNumber As LinkButton = CType(e.Item.FindControl("lbtYearNumber"), LinkButton)
            lbtYearNumber.Text = dto.IdentifierString
            lbtYearNumber.CommandArgument = dto.Id.ToString()

            ' Se il risultato della ricerca serve in fase di copia cambio il comportamento del link
            If Action.Eq("CopyDocuments") Then
                lbtYearNumber.OnClientClick = String.Format("return CloseWindow({0});", dto.Id)
            End If

            Dim ibtMainDocument As ImageButton = CType(e.Item.FindControl("ibtMainDocument"), ImageButton)
            ibtMainDocument.PostBackUrl = GetPostBackUrl(dto, MainDocumentColumn)

            ' Registrazioni annullate
            Select Case dto.Status
                Case DocumentSeriesItemStatus.NotActive
                    ibtMainDocument.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/document_copies_error.png"
                Case DocumentSeriesItemStatus.Active
                    ibtMainDocument.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/document_copies.png"
                Case DocumentSeriesItemStatus.Draft
                    ibtMainDocument.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/document_copies_draft.png"
                Case DocumentSeriesItemStatus.Canceled
                    ibtMainDocument.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/document_copies_delete.png"
            End Select

            With DirectCast(e.Item.FindControl("lblRoles"), Label)
                If RolesLabelDictionary.ContainsKey(dto.Id) Then
                    .Text = String.Join("," & WebHelper.Br & " ", RolesLabelDictionary(dto.Id))
                Else
                    .Text = WebHelper.Space
                End If
            End With

            Dim lblRegistrationUser As Label = DirectCast(e.Item.FindControl("lblRegistrationUser"), Label)
            lblRegistrationUser.Text = CommonAD.GetDisplayName(dto.RegistrationUser)
        End Sub

        Private Sub CmdViewDocumentsClick(sender As Object, e As EventArgs) Handles cmdViewDocuments.Click
            Dim checked As IList(Of Integer) = GetCheckedRows()
            If checked IsNot Nothing AndAlso checked.Count > 0 Then
                Dim serialized As String = JsonConvert.SerializeObject(checked)
                Response.Redirect("~/Viewers/DocumentSeriesItemViewer.aspx?" & CommonShared.AppendSecurityCheck(String.Format("ids={0}", serialized)))
            End If
        End Sub

        Private Sub cmdPublish_Click(sender As Object, e As EventArgs) Handles cmdPublish.Click
            Dim selection As IList(Of Integer) = Me.GetCheckedRows()
            If Not selection.IsNullOrEmpty() Then
                Dim items As List(Of DocumentSeriesItem) = selection.Select(Function(i) FacadeFactory.Instance.DocumentSeriesItemFacade.GetById(i)).ToList()
                Dim available As List(Of DocumentSeriesItem) = items.Where(Function(i) New DocumentSeriesItemRights(i).IsPublishable).ToList()

                If available.IsNullOrEmpty() Then
                    Me.AjaxAlert("Nessuna delle {0} selezionate è risultata essere pubblicabile.", selection.Count)
                    Return
                End If

                available.ForEach(Sub(i) FacadeFactory.Instance.DocumentSeriesItemFacade.Publish(i))
                If available.Count.Equals(selection.Count) Then
                    Me.AjaxAlert("Tutte le {0} selezionate sono state pubblicate.", selection.Count)
                Else
                    Me.AjaxAlert("Sono state pubblicate {0} delle {1} selezionate.", available.Count, selection.Count)
                End If
                Me.InitializeGrid()
            End If
        End Sub

        Private Sub cmdRetire_Click(sender As Object, e As EventArgs) Handles cmdRetire.Click
            Dim selection As IList(Of Integer) = Me.GetCheckedRows()
            If Not selection.IsNullOrEmpty() Then
                Dim items As List(Of DocumentSeriesItem) = selection.Select(Function(i) FacadeFactory.Instance.DocumentSeriesItemFacade.GetById(i)).ToList()
                Dim available As List(Of DocumentSeriesItem) = items.Where(Function(i) New DocumentSeriesItemRights(i).IsRetirable).ToList()

                If available.IsNullOrEmpty() Then
                    Me.AjaxAlert("Nessuna delle {0} selezionate è risultata essere ritirabile.", selection.Count)
                    Return
                End If

                available.ForEach(Sub(i) FacadeFactory.Instance.DocumentSeriesItemFacade.Retire(i))
                If available.Count.Equals(selection.Count) Then
                    Me.AjaxAlert("Tutte le {0} selezionate sono state ritirate.", selection.Count)
                Else
                    Me.AjaxAlert("Sono state ritirate {0} delle {1} selezionate.", available.Count, selection.Count)
                End If
                Me.InitializeGrid()
            End If
        End Sub

#End Region

#Region " Methods "

        Private Sub Initialize()
            grdDocumentSeriesItem.MasterTableView.NoMasterRecordsText = String.Format("Nessuna {0} Trovata", ProtocolEnv.DocumentSeriesName)
        End Sub

        Private Sub InitializeAjax()
            AjaxManager.EnablePageHeadUpdate = True
            AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest

            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.TitleContainer)
            AjaxManager.AjaxSettings.AddAjaxSetting(grdDocumentSeriesItem, MasterDocSuite.TitleContainer)
            AjaxManager.AjaxSettings.AddAjaxSetting(grdDocumentSeriesItem, grdDocumentSeriesItem, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, grdDocumentSeriesItem, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdPublish, grdDocumentSeriesItem)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdRetire, grdDocumentSeriesItem)
        End Sub

        Private Sub InitializeGrid()
            'Se sto visualizzando la pagine delle Bozze gestisco il finder corretto
            If ShowDraft Then
                grdDocumentSeriesItem.Finder = DraftFinder
                If DraftFinder.EnablePaging Then
                    grdDocumentSeriesItem.PageSize = DraftFinder.PageSize
                End If
            Else
                grdDocumentSeriesItem.Finder = Finder
                If Finder.EnablePaging Then
                    grdDocumentSeriesItem.PageSize = Finder.PageSize
                End If
            End If

            grdDocumentSeriesItem.DataBindFinder()
        End Sub

        Private Function GetCheckedRows() As IList(Of Integer)
            Dim checked As New List(Of Integer)
            For Each item As GridDataItem In grdDocumentSeriesItem.Items
                With DirectCast(item.FindControl("chkSelect"), CheckBox)
                    If .Checked Then
                        Dim identifier As Integer = Integer.Parse(.Attributes(SelectIdentifierAttribute))
                        checked.Add(identifier)
                    End If
                End With
            Next
            If checked.Count = 0 Then
                Return Nothing
            End If
            Return checked
        End Function

        Private Function GetPostBackUrl(dto As DocumentSeriesItemDTO(Of BiblosDocumentInfo), uniqueName As String) As String
            Select Case uniqueName
                Case MainDocumentColumn
                    Return "~/Viewers/DocumentSeriesItemViewer.aspx?" & CommonShared.AppendSecurityCheck(String.Format("id={0}", dto.Id))
                Case YearNumberColumn
                    Return "~/Series/Item.aspx?" & CommonShared.AppendSecurityCheck(String.Format("IdDocumentSeriesItem={0}&Action={1}&Type=Series", dto.Id, DocumentSeriesAction.View))
            End Select
            Return Nothing
        End Function

#End Region

    End Class
End Namespace