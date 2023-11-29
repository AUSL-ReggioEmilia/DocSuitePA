Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging

Partial Public Class uscCollGrid
    Inherits DocSuite2008BaseControl

    Public Event OnRefresh(ByVal sender As Object, ByVal e As CollaborationEventArgs)
    Public Event OnSelectedCollaboration(ByVal sender As Object, ByVal e As CollaborationEventArgs)

#Region " Fields "

    Private _gridDataSource As IList(Of WebAPIDto(Of CollaborationModel))
    Private _idCollaborations As IList(Of Integer)
    Private _hasMainDocumentSignedDictionary As IDictionary(Of Integer, Boolean)
    Private _signsDictionary As IDictionary(Of Integer, List(Of CollaborationSign))

    Public Const COLUMN_CLIENT_SELECT As String = "ClientSelectColumn"
    Public Const COLUMN_DOCUMENT_TYPE As String = "cDocType"
    Public Const COLUMN_STATUS As String = "cDelete"
    Public Const COLUMN_PRIORITY As String = "cPriority"
    Public Const COLUMN_PERSON As String = "cPerson"
    Public Const COLUMN_TYPE As String = "cType"
    Public Const COLUMN_DOCUMENT_SIGN As String = "cDocSign"
    Public Const COLUMN_USER_DOCUMENT_SIGN As String = "cUserDocSign"
    Public Const COLUMN_VERSIONING_COUNT As String = "VersioningCount"
    Public Const COLUMN_NUMBER As String = "Number"
    Public Const COLUMN_MEMORANDUM_DATE As String = "Entity.MemorandumDate"
    Public Const COLUMN_OBJECT As String = "Entity.Subject"
    Public Const COLUMN_NOTE As String = "Entity.Note"
    Public Const COLUMN_PROPOSER As String = "Proposer"
    Public Const COLUMN_TO_SIGN As String = "cToSign"
    Public Const COLUMN_TO_PROTOCOL As String = "cToProtocol"
    Public Const COLUMN_DOWNLOAD As String = "cDownload"
    Public Const COLUMN_SIGN_REQUIRED As String = "signDocRequired"
    Public Const COLUMN_ENTITY_DOCUMENT_TYPE As String = "Entity.DocumentType"
    Private Const CUSTOM_EXPORT_TEXT As String = "Esporta per firmatario"
    Private Const CUSTOM_EXPORT_TOOLTIP As String = "Esporta tutti i risultati della ricerca raggruppati per firmatari in Excel"
    Private Const EXCEL_COLUMN_ID As String = "Identificativo"
    Private Const EXCEL_COLUMN_NUMBER As String = "Numero"
    Private Const EXCEL_COLUMN_MEMORANDUM_DATE As String = "Data Prom."
    Private Const EXCEL_COLUMN_SUBJECT As String = "Oggetto"
    Private Const EXCEL_COLUMN_NOTE As String = "Note"
    Private Const EXCEL_COLUMN_PROPOSER As String = "Proponente"
    Private Const EXCEL_COLUMN_TO_SIGN As String = "Da visionare/firmare"
    Private Const EXCEL_COLUMN_SIGN_DATE As String = "Data di firma"
    Private Const EXCEL_COLUMN_IS_ACTIVE As String = "Attivo?"
    Private Const EXCEL_COLUMN_REGISTRATION_DATE As String = "Data collaborazione"

    Const ContentName As String = "~\UserControl\uscCollOffline.ascx"
    Private _currentUDSFacade As UDSFacade
    Private _currentUDSCollaborationFinder As UDSCollaborationFinder

#End Region

#Region " Properties "

    Public ReadOnly Property Grid() As BindGrid
        Get
            Return gvCollaboration
        End Get
    End Property

    Private ReadOnly Property GridDataSource As IList(Of WebAPIDto(Of CollaborationModel))
        Get
            If _gridDataSource Is Nothing Then
                _gridDataSource = CType(gvCollaboration.DataSource, IList(Of WebAPIDto(Of CollaborationModel)))
            End If
            Return _gridDataSource
        End Get
    End Property

    Private ReadOnly Property IdCollaborations As IList(Of Integer)
        Get
            If _idCollaborations Is Nothing Then
                _idCollaborations = GridDataSource.Select(Function(ch) ch.Entity.IdCollaboration.Value).ToList()
            End If
            Return _idCollaborations
        End Get
    End Property

    Private ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _currentUDSFacade Is Nothing Then
                _currentUDSFacade = New UDSFacade()
            End If
            Return _currentUDSFacade
        End Get
    End Property

    Private ReadOnly Property CurrentUDSCollaborationFinder As UDSCollaborationFinder
        Get
            If _currentUDSCollaborationFinder Is Nothing Then
                _currentUDSCollaborationFinder = New UDSCollaborationFinder(DocSuiteContext.Current.Tenants)
            End If
            Return _currentUDSCollaborationFinder
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub gvProtocols_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles gvCollaboration.ItemCommand
        Dim arguments As String() = Split(CType(e.CommandArgument, String), "|")
        Select Case e.CommandName
            Case "Down"
                Dim ctrl As Control = Page.LoadControl(ContentName)
                If ctrl IsNot Nothing Then
                    DirectCast(ctrl, uscCollOffline).IdCollaboration = CType(e.CommandArgument, Integer)
                    Using sw As StringWriter = New StringWriter
                        Using nhw As HtmlTextWriter = New HtmlTextWriter(sw)
                            ctrl.RenderControl(nhw)

                            Dim fileHtml As StreamWriter
                            Try
                                Dim temp As String = String.Format("{0}\Offline\Collaboration n.{1}.htm", CommonUtil.GetInstance().AppTempPath, e.CommandArgument)
                                fileHtml = New StreamWriter(File.Create(temp))
                                fileHtml.Write(sw.ToString())
                                fileHtml.Close()
                            Catch ex As Exception
                                FileLogger.Warn(LoggerName, ex.Message, ex)
                                AjaxManager.Alert(ex.Message)
                            End Try
                            sw.Close()
                            nhw.Close()
                        End Using
                    End Using
                End If
            Case "Selz"
                SetCollSelected(Integer.Parse(arguments(0)), arguments(1))
            Case "Prot"
                Dim url As String = String.Concat("~/Prot/ProtVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("UniqueId={0}&Type=Prot", arguments(0))))
                Response.Redirect(ResolveUrl(url))
            Case "Resl"
                Dim url As String = String.Concat("~/Resl/ReslVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&Type=Resl", arguments(0))))
                Response.Redirect(ResolveUrl(url))
            Case "Series"
                Dim url As String = String.Concat("~/Series/Item.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdDocumentSeriesItem={0}&Type=Series&Action=View", arguments(0))))
                Response.Redirect(ResolveUrl(url))
            Case "UDS"
                Dim url As String = String.Concat("~/UDS/UDSView.aspx?Type=UDS&IdUDS=", arguments(0), "&IdUDSRepository=", arguments(1))
                Response.Redirect(ResolveUrl(url))

        End Select
    End Sub

    Private Sub gvCollaboration_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles gvCollaboration.ItemDataBound
        If TypeOf e.Item Is GridGroupHeaderItem Then
            Dim item As GridGroupHeaderItem = CType(e.Item, GridGroupHeaderItem)
            If item.DataCell.Text.StartsWith("Firma obbligatoria") Then
                Dim groupDataRow As DataRowView = CType(e.Item.DataItem, DataRowView)
                Dim isRequired As Boolean = False
                Boolean.TryParse(groupDataRow("IsSignedRequired").ToString(), isRequired)
                item.DataCell.Text = If(isRequired, "Firma obbligatoria", "Firma non obbligatoria")
            End If
        End If

        If e.Item.ItemType = GridItemType.Item OrElse e.Item.ItemType = GridItemType.AlternatingItem Then
            Dim item As WebAPIDto(Of CollaborationModel) = DirectCast(e.Item.DataItem, WebAPIDto(Of CollaborationModel))

            Dim cbSelectItem As CheckBox = CType(e.Item.FindControl("cbSelect"), CheckBox)
            If cbSelectItem IsNot Nothing Then
                cbSelectItem.Visible = Not item.Entity.DocumentType.Eq(CollaborationDocumentType.O.ToString()) AndAlso item.TenantModel.TenantName.Eq(DocSuiteContext.Current.CurrentTenant.TenantName)
            End If

            Dim imgDocTypeItem As RadButton = CType(e.Item.FindControl("imgDocType"), RadButton)
            If imgDocTypeItem IsNot Nothing Then
                Select Case item.Entity.DocumentType
                    Case CollaborationDocumentType.P.ToString(),
                         CollaborationDocumentType.W.ToString()
                        imgDocTypeItem.Image.ImageUrl = "../Comm/Images/DocSuite/Protocollo16.png"
                    Case CollaborationDocumentType.D.ToString()
                        imgDocTypeItem.Image.ImageUrl = "../Comm/Images/DocSuite/Delibera16.png"
                    Case CollaborationDocumentType.A.ToString()
                        imgDocTypeItem.Image.ImageUrl = "../Comm/images/Docsuite/Atto16.png"
                    Case CollaborationDocumentType.S.ToString(),
                         CollaborationDocumentType.UDS.ToString()
                        imgDocTypeItem.Image.ImageUrl = ImagePath.SmallDocumentSeries
                    Case Else
                        imgDocTypeItem.Image.ImageUrl = ImagePath.SmallEmailDocuments
                End Select
                If Integer.TryParse(item.Entity.DocumentType, 0) Then
                    imgDocTypeItem.Image.ImageUrl = ImagePath.SmallDocumentSeries
                End If
                imgDocTypeItem.ToolTip = item.Entity.TemplateName
                imgDocTypeItem.CommandArgument = String.Format("{0}|{1}", item.Entity.IdCollaboration, item.Entity.DocumentType)
            End If

            Dim lblHasDocumentExtracted As Label = CType(e.Item.FindControl("lblHasDocumentExtracted"), Label)
            lblHasDocumentExtracted.Text = "1"
            If item.Entity.HasDocumentExtracted() Then
                lblHasDocumentExtracted.Text = item.Entity.HasDocumentExtracted().ToString()
            End If

            Dim lblProposer As Label = CType(e.Item.FindControl("lblProposer"), Label)
            If lblProposer IsNot Nothing AndAlso Not String.IsNullOrEmpty(item.Entity.RegistrationUser) Then
                lblProposer.Text = CommonAD.GetDisplayName(item.Entity.RegistrationUser)
            End If

            Dim lblVersioningCount As Label = CType(e.Item.FindControl("lblVersioningCount"), Label)
            If lblVersioningCount IsNot Nothing Then
                lblVersioningCount.Text = item.Entity.VersioningCount().ToString()
            End If

            Dim imgDeleteItem As Image = CType(e.Item.FindControl("imgDelete"), Image)
            If imgDeleteItem IsNot Nothing Then
                imgDeleteItem.ImageUrl = ImagePath.SmallEmpty
                imgDeleteItem.AlternateText = String.Empty
                If item.Entity.IdStatus.Eq(CollaborationStatusType.DL.ToString()) Then
                    imgDeleteItem.ImageUrl = "../Comm/images/Loop16.gif"
                    imgDeleteItem.AlternateText = "Documento restituito"
                End If
            End If

            Dim imgPriorityItem As Image = CType(e.Item.FindControl("imgPriority"), Image)
            If imgPriorityItem IsNot Nothing Then
                Select Case item.Entity.IdPriority
                    Case "B"
                        imgPriorityItem.ImageUrl = "../Comm/images/PriorityLow16.gif"
                        imgPriorityItem.AlternateText = "Priorità bassa"
                    Case "A"
                        imgPriorityItem.ImageUrl = "../Comm/images/PriorityHigh16.gif"
                        imgPriorityItem.AlternateText = "Priorità alta"
                    Case Else
                        imgPriorityItem.ImageUrl = ImagePath.SmallEmpty
                        imgPriorityItem.AlternateText = ""
                End Select
            End If

            Dim imgPersonItem As Image = CType(e.Item.FindControl("imgPerson"), Image)
            If imgPersonItem IsNot Nothing Then
                imgPersonItem.AlternateText = ""
                imgPersonItem.ImageUrl = ImagePath.SmallEmpty
                If item.Entity.DestinationFirst.GetValueOrDefault(False) Then
                    imgPersonItem.AlternateText = "Utente principale per la protocollazione"
                    If item.Entity.Account.Eq(DocSuiteContext.Current.User.FullUserName) Then
                        imgPersonItem.ImageUrl = "../App_Themes/DocSuite2008/imgset16/user.png"
                    End If
                End If
            End If

            Dim imgTypeItem As RadButton = CType(e.Item.FindControl("imgType"), RadButton)
            If imgTypeItem IsNot Nothing Then
                If Not item.Entity.DocumentType.Eq(CollaborationDocumentType.O.ToString()) Then
                    imgTypeItem.Image.ImageUrl = ImagePath.FromFileNoSignature(MainDocumentName(item.Entity.CollaborationVersionings), True)
                    Dim url As String = String.Concat("~/viewers/CollaborationViewer.aspx?", CommonShared.AppendSecurityCheck(String.Concat("DataSourceType=coll&id=", item.Entity.IdCollaboration.ToString())))
                    imgTypeItem.NavigateUrl = ResolveUrl(url)
                Else
                    imgTypeItem.Visible = False
                End If
            End If

            Dim imgDocSignItem As Image = CType(e.Item.FindControl("imgDocSign"), Image)
            If imgDocSignItem IsNot Nothing Then
                If HasMainDocumentSigned(item.Entity.CollaborationVersionings) Then
                    CheckDocumentSign(True, imgDocSignItem)
                Else
                    CheckDocumentSign(False, imgDocSignItem)
                End If
            End If

            If gvCollaboration.Columns.FindByUniqueName(COLUMN_SIGN_REQUIRED).Visible Then
                Dim imgSignDocRequired As Image = CType(e.Item.FindControl("imgSignDocRequired"), Image)
                If imgSignDocRequired IsNot Nothing Then
                    imgSignDocRequired.Visible = False
                    If Not item.Entity.DocumentType.Eq(CollaborationDocumentType.O.ToString()) Then
                        Dim requiredSigns As ICollection(Of CollaborationSignModel) = item.Entity.CollaborationSigns
                        Dim currentSign As CollaborationSignModel = requiredSigns.SingleOrDefault(Function(x) x.SignUser.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso x.IsActive)
                        If currentSign IsNot Nothing AndAlso currentSign.IsRequired Then
                            imgSignDocRequired.Visible = True
                            If currentSign.SignDate.HasValue Then
                                imgSignDocRequired.ImageUrl = "../Comm/images/Flag16.gif"
                                imgSignDocRequired.ToolTip = "Collaborazione correttamente firmata"
                            Else
                                imgSignDocRequired.ImageUrl = ImagePath.SmallError
                                imgSignDocRequired.ToolTip = "Collaborazione con obbligo di firma"
                            End If
                        End If
                    End If
                End If
            End If

            Dim imgSignItem As Image = CType(e.Item.FindControl("imgSign"), Image)
            If imgSignItem IsNot Nothing Then
                'verifica se è un ODG altrimenti lavora su una collaborazione
                If Not item.Entity.DocumentType.Eq(CollaborationDocumentType.O.ToString()) Then
                    If (item.Entity.HasDocumentExtracted()) OrElse Not HasMainDocumentSigned(item.Entity.CollaborationVersionings, DocSuiteContext.Current.User.FullUserName) Then
                        imgSignItem.ImageUrl = "../App_Themes/DocSuite2008/imgset16/delete.png"
                        imgSignItem.ToolTip = "Almeno un documento in modifica o documento non firmato"
                    Else
                        imgSignItem.ImageUrl = "../Comm/images/Flag16.gif"
                        imgSignItem.ToolTip = "Nessun documento in modifica"
                    End If
                Else
                    imgSignItem.Visible = False
                End If


            End If

            Dim lnkNumberItem As LinkButton = CType(e.Item.FindControl("lnkNumber"), LinkButton)
            If lnkNumberItem IsNot Nothing Then
                ' comando
                lnkNumberItem.CommandName = CollaborationFacade.GetPageTypeFromDocumentType(item.Entity.DocumentType)
                ' testo e argomento
                Select Case item.Entity.DocumentType
                    Case CollaborationDocumentType.P.ToString(), CollaborationDocumentType.U.ToString(), CollaborationDocumentType.W.ToString()
                        If item.Entity.HasProtocol() Then
                            Dim protocolId As String = String.Empty
                            If item.Entity.Year.HasValue AndAlso item.Entity.Number.HasValue Then
                                protocolId = $"{item.Entity.DocumentUnit.Title}"
                            End If
                            lnkNumberItem.Text = $"{protocolId}{WebHelper.Br}{item.Entity.PublicationDate.DefaultString()}"
                            lnkNumberItem.CommandArgument = $"{item.Entity.DocumentUnit.UniqueId}|{item.Entity.Year}|{item.Entity.Number:0000000}"
                        End If
                    Case CollaborationDocumentType.D.ToString(), CollaborationDocumentType.A.ToString()
                        If item.Entity.HasResolution() Then
                            lnkNumberItem.Text = $"{item.Entity.DocumentUnit.DocumentUnitName} {item.Entity.DocumentUnit.Title} del {item.Entity.DocumentUnit.RegistrationDate.DefaultString()}"
                            lnkNumberItem.CommandArgument = $"{item.Entity.DocumentUnit.EntityId}"
                        End If
                    Case CollaborationDocumentType.S.ToString()
                        If item.Entity.HasSeries() Then
                            If item.Entity.DocumentUnit.Year > 0 Then
                                lnkNumberItem.Text = $"{item.Entity.DocumentUnit.Title}"
                            Else
                                lnkNumberItem.Text = $"Bozza N. {item.Entity.DocumentUnit.EntityId}"
                            End If
                            lnkNumberItem.CommandArgument = $"{item.Entity.DocumentUnit.EntityId}"
                        End If
                End Select

                Dim udsTypeParsed As Integer = 0
                If item.Entity.DocumentType.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(item.Entity.DocumentType, udsTypeParsed) AndAlso udsTypeParsed >= 100 Then
                    If item.Entity.DocumentUnit IsNot Nothing AndAlso item.Entity.DocumentUnit.IdUDSRepository.HasValue Then
                        Dim udsId As String = $"{item.Entity.DocumentUnit.Title}"
                        lnkNumberItem.Text = $"{udsId}{WebHelper.Br}{item.Entity.PublicationDate.DefaultString()}"
                        lnkNumberItem.CommandArgument = $"{item.Entity.DocumentUnit.UniqueId}|{item.Entity.DocumentUnit.IdUDSRepository.Value}"
                    End If
                End If
            End If

            Dim lblSignsItem As Label = CType(e.Item.FindControl("lblSigns"), Label)
            If lblSignsItem IsNot Nothing Then
                lblSignsItem.Text = SetSignsLabel(item.Entity)
            End If

            Dim lblToProtocolItem As Label = CType(e.Item.FindControl("lblToProtocol"), Label)
            If lblToProtocolItem IsNot Nothing AndAlso Not item.Entity.CollaborationUsers.IsNullOrEmpty() Then
                lblToProtocolItem.Text = SetDestinations(item.Entity.CollaborationUsers.ToList())
            End If

            Dim imgDownloadItem As ImageButton = CType(e.Item.FindControl("imgDownload"), ImageButton)
            If imgDownloadItem IsNot Nothing Then
                imgDownloadItem.CommandArgument = item.Entity.IdCollaboration.ToString()
            End If
        End If
    End Sub

    Private Sub gvCollaboration_Init(sender As Object, e As EventArgs) Handles gvCollaboration.Init
        gvCollaboration.EnableCustomExportButtons = True
        gvCollaboration.CustomExportButtonText = CUSTOM_EXPORT_TEXT
        gvCollaboration.CustomExportButtonTooltip = CUSTOM_EXPORT_TOOLTIP
    End Sub

    Private Sub gvCollaboration_OnCustomExportDataRequest(sender As Object, e As EventArgs) Handles gvCollaboration.OnCustomExportDataRequest
        gvCollaboration.DataBindFinder()
        Dim customTable As Table = CreateCustomTable()
        gvCollaboration.DoCustomExport(customTable)
    End Sub
#End Region

#Region " Methods "

    Private Sub SetCollSelected(idCollaboration As Integer, documentType As String)
        Dim coll As Collaboration = Facade.CollaborationFacade.GetById(idCollaboration)
        If coll Is Nothing Then
            BasePage.AjaxAlert(Server.HtmlEncode("La Registrazione è stata modificata\n\nAggiornamento automatico dell'elenco"), False)
            RaiseEvent OnRefresh(Me, New CollaborationEventArgs(idCollaboration))
            Exit Sub
        Else
            RaiseEvent OnSelectedCollaboration(Me, New CollaborationEventArgs(idCollaboration) With {.DocumentType = documentType})
        End If
    End Sub

    Public Sub SelectCollaboration(ByVal idCollaboration As Integer)
        SetCollSelected(idCollaboration, String.Empty)
    End Sub

    Private Function MainDocumentName(versionings As ICollection(Of CollaborationVersioningModel)) As String
        If versionings.IsNullOrEmpty() Then
            Return String.Empty
        End If

        Dim collaborationVersioning As CollaborationVersioningModel = versionings.Where(Function(x) x.CollaborationIncremental = 0).OrderByDescending(Function(x) x.Incremental).FirstOrDefault()
        If collaborationVersioning Is Nothing Then
            Return String.Empty
        End If

        Return collaborationVersioning.DocumentName
    End Function

    Private Function HasMainDocumentSigned(versionings As ICollection(Of CollaborationVersioningModel)) As Boolean
        Return HasMainDocumentSigned(versionings, String.Empty)
    End Function

    Private Function HasMainDocumentSigned(versionings As ICollection(Of CollaborationVersioningModel), user As String) As Boolean
        If versionings.IsNullOrEmpty() Then
            Return False
        End If

        Dim query As IEnumerable(Of CollaborationVersioningModel) = versionings.Where(Function(x) x.CollaborationIncremental = 0)
        If Not String.IsNullOrEmpty(user) Then
            query = query.Where(Function(x) x.RegistrationUser.Eq(user))
        End If

        Dim collaborationVersioning As CollaborationVersioningModel = query.OrderByDescending(Function(x) x.Incremental).FirstOrDefault()
        If collaborationVersioning Is Nothing Then
            Return False
        End If

        Return FileHelper.MatchExtension(collaborationVersioning.DocumentName, FileHelper.P7M)
    End Function

    Private Function SetSignsLabel(collaboration As CollaborationModel) As String
        If collaboration.DocumentType.Eq("O") Then
            Return String.Format("<b>{0}</b>", CommonInstance.UserDescription)
        End If

        Dim labels As New List(Of String)
        For Each item As CollaborationSignModel In collaboration.CollaborationSigns.OrderBy(Function(o) o.Incremental)
            Dim signName As String = Facade.CollaborationSignsFacade.GetCollaborationSignDescription(item.SignName, item.IsAbsent)

            Dim excludedStatuses As New List(Of String) From {CollaborationStatusType.DP.ToString(), CollaborationStatusType.PT.ToString()}

            If item.IsActive AndAlso Not excludedStatuses.Any(Function(s) collaboration.IdStatus.ContainsIgnoreCase(s)) Then
                signName = String.Format("<b>{0}</b>", signName)
            End If
            If ProtocolEnv.ShowCollaborationSignDate AndAlso item.SignDate.HasValue Then
                signName = $"{signName} {item.SignDate.Value.ToShortDateString()}"
            End If
            labels.Add(signName)
        Next
        If labels.Count = 0 Then
            Return String.Empty
        End If
        Return String.Join(WebHelper.Br, labels.ToArray())
    End Function

    Public Function SetDestinations(collaborationUsers As IList(Of CollaborationUserModel)) As String
        Dim destinations As New StringBuilder()
        For Each collUsers As CollaborationUserModel In collaborationUsers
            Dim temp As String
            If collUsers.DestinationFirst.GetValueOrDefault(False) Then
                temp = "<B>{0}</B>"
            Else
                temp = "{0}"
            End If
            destinations.AppendFormat(temp, collUsers.DestinationName)
            destinations.Append(WebHelper.Br)
        Next
        Return destinations.ToString()
    End Function

    Public Sub DisableColumn(ByVal columnName As String)
        Dim col As GridColumn = gvCollaboration.Columns.FindByUniqueNameSafe(columnName)
        If col IsNot Nothing Then
            Try
                'nasconde la colonna
                col.Visible = False
                'elimina il bind dei dati della colonna
                If TypeOf (col) Is GridTemplateColumn Then
                    DirectCast(col, GridTemplateColumn).ItemTemplate = Nothing
                End If

                If TypeOf (col) Is GridBoundColumn Then
                    DirectCast(col, GridBoundColumn).DataField = ""
                End If

            Catch ex As Exception
                Throw New Exception("Unable to disable column " & col.UniqueName)
            End Try
        End If
    End Sub

    Public Sub RenameColumn(ByVal columnName As String, ByVal value As String)
        Dim col As GridColumn = gvCollaboration.Columns.FindByUniqueNameSafe(columnName)
        If col IsNot Nothing Then
            Try
                col.HeaderText = value
            Catch ex As Exception
                Throw New Exception("Unable to rename column " & col.UniqueName)
            End Try
        End If
    End Sub

    Public Sub CheckDocumentSign(ByVal signed As Boolean, ByVal imageItem As Image)
        If signed Then
            imageItem.ImageUrl = ImagePath.SmallSigned
            imageItem.ToolTip = "Documento principale firmato"
            imageItem.AlternateText = "Documento con Firma Digitale Qualificata"
        Else
            imageItem.ImageUrl = ImagePath.SmallEmpty
            imageItem.AlternateText = ""
        End If
    End Sub


    Private Function CreateCustomTable() As Table
        Dim customTable As DSTable = New DSTable()
        customTable.Table.Style.Add("border-collapse", "collapse")
        CreateCustomTableHeader(customTable)

        Dim signUsers As ICollection(Of CollaborationSignModel) = GridDataSource _
            .SelectMany(Function(x) x.Entity.CollaborationSigns) _
            .ToList()
        Dim collaborations As ICollection(Of CollaborationModel) = GridDataSource.Select(Function(x) x.Entity).ToList()

        Dim collaborations_collaborationSigns As List(Of KeyValuePair(Of CollaborationSignModel, CollaborationModel)) = signUsers _
            .Join(collaborations, Function(CS) CS.IdCollaboration, Function(C) C.IdCollaboration.Value,
             Function(signUser, collection) New KeyValuePair(Of CollaborationSignModel, CollaborationModel)(signUser, collection)) _
             .OrderBy(Function(x) x.Key.SignName).ToList()

        For Each collaboration_collaboratonSign As KeyValuePair(Of CollaborationSignModel, CollaborationModel) In collaborations_collaborationSigns
            customTable.CreateEmptyRow()
            For Each customTableColumn As DSTableCell In customTable.Rows(0).Cells
                customTable.CurrentRow.CreateEmpytCell()
                customTable.CurrentRow.CurrentCell.ApplyStyle(GetCellItemStyle())
                customTable.CurrentRow.CurrentCell.Text = GetCellItemText(customTableColumn.Text, collaboration_collaboratonSign.Value, collaboration_collaboratonSign.Key)
            Next
        Next
        Return customTable.Table
    End Function

    Private Function GetTableWidth() As Double
        Dim index As Integer = -1
        Dim manageableColumns As IDictionary(Of Integer, GridColumn) = gvCollaboration.Columns.Cast(Of GridColumn) _
            .ToDictionary(Function(k)
                              index += 1
                              Return index
                          End Function, Function(k) k).Where(Function(c) c.Value.Visible AndAlso c.Value.Display AndAlso Not c.Value.UniqueName.Eq("ClientSelectColumn")).ToDictionary(Function(i) i.Key, Function(c) c.Value)
        Return manageableColumns.Sum(Function(c) c.Value.HeaderStyle.Width.Value)
    End Function

    Private Sub CreateCustomTableHeader(customTable As DSTable)
        Dim cellPercentageWidth As Double = 0
        Dim dateCellWidth As Double = 0
        customTable.CreateEmptyRow()

        customTable.CurrentRow.CreateEmpytCell()
        customTable.CurrentRow.CurrentCell.Text = EXCEL_COLUMN_ID
        customTable.CurrentRow.CurrentCell.ApplyStyle(GetCellHeaderStyle(dateCellWidth))

        For Each gridColumn As GridColumn In gvCollaboration.Columns
            If gridColumn.HeaderText = String.Empty OrElse Not gridColumn.Visible OrElse gridColumn.UniqueName = COLUMN_TO_PROTOCOL OrElse gridColumn.UniqueName = COLUMN_VERSIONING_COUNT Then
                Continue For
            End If

            cellPercentageWidth = (gridColumn.HeaderStyle.Width.Value / GetTableWidth()) * 100

            If gridColumn.UniqueName = COLUMN_MEMORANDUM_DATE Then
                dateCellWidth = cellPercentageWidth
            End If

            customTable.CurrentRow.CreateEmpytCell()
            customTable.CurrentRow.CurrentCell.Text = gridColumn.HeaderText
            customTable.CurrentRow.CurrentCell.ApplyStyle(GetCellHeaderStyle(cellPercentageWidth))
        Next

        customTable.CurrentRow.CreateEmpytCell()
        customTable.CurrentRow.CurrentCell.Text = EXCEL_COLUMN_SIGN_DATE
        customTable.CurrentRow.CurrentCell.ApplyStyle(GetCellHeaderStyle(dateCellWidth))

        customTable.CurrentRow.CreateEmpytCell()
        customTable.CurrentRow.CurrentCell.Text = EXCEL_COLUMN_IS_ACTIVE
        customTable.CurrentRow.CurrentCell.ApplyStyle(GetCellHeaderStyle(cellPercentageWidth))

        customTable.CurrentRow.CreateEmpytCell()
        customTable.CurrentRow.CurrentCell.ApplyStyle(GetCellHeaderStyle(dateCellWidth))
        customTable.CurrentRow.CurrentCell.Text = EXCEL_COLUMN_REGISTRATION_DATE
    End Sub

    Private Function GetCellHeaderStyle(width As Double) As DSTableCellStyle
        Dim cellHeaderStyle As New DSTableCellStyle()
        cellHeaderStyle.Width = Unit.Percentage(width)
        cellHeaderStyle.Font.Bold = True
        cellHeaderStyle.LineBox = True
        Return cellHeaderStyle
    End Function

    Private Function GetCellItemStyle() As DSTableCellStyle
        Return New DSTableCellStyle() With {
            .LineBox = True
        }
    End Function

    Private Function GetCollaborationNumber(collaboration As CollaborationModel) As String
        Dim collaborationNumber As String = String.Empty
        Select Case collaboration.DocumentType
            Case CollaborationDocumentType.P.ToString(), CollaborationDocumentType.U.ToString(), CollaborationDocumentType.W.ToString()
                If collaboration.HasProtocol() Then
                    Dim protocolId As String = String.Empty
                    If collaboration.Year.HasValue AndAlso collaboration.Number.HasValue Then
                        protocolId = $"{collaboration.DocumentUnit.Title}"
                    End If
                    collaborationNumber = $"{protocolId}{WebHelper.Br}{collaboration.PublicationDate.DefaultString()}"
                End If
            Case CollaborationDocumentType.D.ToString(), CollaborationDocumentType.A.ToString()
                If collaboration.HasResolution() Then
                    collaborationNumber = $"{collaboration.DocumentUnit.DocumentUnitName} {collaboration.DocumentUnit.Title} del {collaboration.DocumentUnit.RegistrationDate.DefaultString()}"
                End If
            Case CollaborationDocumentType.S.ToString()
                If collaboration.HasSeries() Then
                    If collaboration.DocumentUnit.Year > 0 Then
                        collaborationNumber = $"{collaboration.DocumentUnit.Title}"
                    Else
                        collaborationNumber = $"Bozza N. {collaboration.DocumentUnit.EntityId}"
                    End If
                End If
        End Select
        Return collaborationNumber
    End Function

    Private Function GetCellItemText(columnDescription As String, collaboration As CollaborationModel, signUser As CollaborationSignModel) As String
        Dim cellItemText As String = String.Empty
        Select Case columnDescription
            Case EXCEL_COLUMN_ID
                cellItemText = collaboration.IdCollaboration.ToString()
            Case EXCEL_COLUMN_NUMBER
                cellItemText = GetCollaborationNumber(collaboration)
            Case EXCEL_COLUMN_MEMORANDUM_DATE
                cellItemText = $"{collaboration.MemorandumDate:dd/MM/yyyy}"
            Case EXCEL_COLUMN_SUBJECT
                cellItemText = collaboration.Subject
            Case EXCEL_COLUMN_NOTE
                cellItemText = collaboration.Note
            Case EXCEL_COLUMN_PROPOSER
                cellItemText = collaboration.RegistrationName
            Case EXCEL_COLUMN_TO_SIGN
                cellItemText = Facade.CollaborationSignsFacade.GetCollaborationSignDescription(signUser.SignName, signUser.IsAbsent)
            Case EXCEL_COLUMN_SIGN_DATE
                cellItemText = $"{signUser.SignDate:dd/MM/yyyy}"
            Case EXCEL_COLUMN_IS_ACTIVE
                cellItemText = If(signUser.IsActive, "Si", "No")
            Case EXCEL_COLUMN_REGISTRATION_DATE
                cellItemText = $"{collaboration.RegistrationDate:dd/MM/yyyy}"
        End Select
        Return cellItemText
    End Function
#End Region

End Class