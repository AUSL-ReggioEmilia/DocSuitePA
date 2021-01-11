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
    Public Const COLUMN_TENANT As String = "TenantModel.TenantName"
    Public Const COLUMN_ENTITY_DOCUMENT_TYPE As String = "Entity.DocumentType"

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
                SetCollSelected(Integer.Parse(arguments(0)), arguments(1), arguments(2))
            Case "Prot"
                Dim url As String = String.Concat("~/Prot/ProtVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("UniqueId={0}&Type=Prot", arguments(0))))
                If ProtocolEnv.MultiDomainEnabled AndAlso Not DocSuiteContext.Current.CurrentTenant.TenantName.Eq(arguments(3)) Then
                    Dim tenant As TenantModel = DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) x.TenantName.Eq(arguments(3)))
                    url = String.Format("{0}/?Tipo=Prot&Azione=Apri&Anno={1}&Numero={2}", tenant.DSWUrl, arguments(1), arguments(2))
                    Response.RedirectToNewWindow(url)
                End If
                Response.Redirect(ResolveUrl(url))
            Case "Resl"
                Dim url As String = String.Concat("~/Resl/ReslVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&Type=Resl", arguments(0))))
                If ProtocolEnv.MultiDomainEnabled AndAlso Not DocSuiteContext.Current.CurrentTenant.TenantName.Eq(arguments(1)) Then
                    Dim tenant As TenantModel = DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) x.TenantName.Eq(arguments(1)))
                    url = String.Format("{0}/?Tipo=Resl&Azione=Apri&Identificativo={1}", tenant.DSWUrl, arguments(0))
                    Response.RedirectToNewWindow(url)
                End If
                Response.Redirect(ResolveUrl(url))
            Case "Series"
                Dim url As String = String.Concat("~/Series/Item.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdDocumentSeriesItem={0}&Type=Series&Action=View", arguments(0))))
                If ProtocolEnv.MultiDomainEnabled AndAlso Not DocSuiteContext.Current.CurrentTenant.TenantName.Eq(arguments(1)) Then
                    Dim tenant As TenantModel = DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) x.TenantName.Eq(arguments(1)))
                    url = String.Format("{0}/?Tipo=DocumentSeries&Azione=Apri&IdDocumentSeriesItem={1}", tenant.DSWUrl, arguments(0))
                    Response.RedirectToNewWindow(url)
                End If
                Response.Redirect(ResolveUrl(url))
            Case "UDS"
                Dim url As String = String.Concat("~/UDS/UDSView.aspx?Type=UDS&IdUDS=", arguments(0), "&IdUDSRepository=", arguments(1))
                If ProtocolEnv.MultiDomainEnabled AndAlso Not DocSuiteContext.Current.CurrentTenant.TenantName.Eq(arguments(2)) Then
                    Dim tenant As TenantModel = DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) x.TenantName.Eq(arguments(2)))
                    url = String.Format("{0}/?Tipo=UDS&Azione=Apri&IdUDS={1}&IdUDSRepository={2}", tenant.DSWUrl, arguments(0), arguments(1))
                    Response.RedirectToNewWindow(url)
                End If
                Response.Redirect(ResolveUrl(url))

        End Select
    End Sub

    Protected Sub cmbTenants_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim comboBox As RadComboBox = DirectCast(sender, RadComboBox)

        Dim filterItem As GridFilteringItem = DirectCast(comboBox.NamingContainer, GridFilteringItem)
        Dim filters As IList(Of IFilterExpression) = New List(Of IFilterExpression)()
        Dim filterType As Data.FilterExpression.FilterType = Data.FilterExpression.FilterType.NoFilter
        If (Not String.IsNullOrEmpty(comboBox.SelectedValue)) Then
            filterType = Data.FilterExpression.FilterType.EqualTo
        End If
        filters.Add(New Data.FilterExpression(COLUMN_TENANT, GetType(Short), comboBox.SelectedValue, filterType))
        filterItem.FireCommandEvent("CustomFilter", filters)
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

        If (TypeOf e.Item Is GridFilteringItem) Then
            Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
            Dim combo As RadComboBox = DirectCast(filterItem.FindControl("cmbTenants"), RadComboBox)
            combo.Items.Insert(0, New RadComboBoxItem("", ""))
            For Each tenant As TenantModel In DocSuiteContext.Current.Tenants
                combo.Items.Add(New RadComboBoxItem(tenant.TenantName, tenant.TenantName))
            Next

            If gvCollaboration.Finder.FilterExpressions.Any(Function(x) x.Key.Eq(COLUMN_TENANT)) Then
                Dim control As Control = filterItem.FindControl("cmbTenants")
                Dim value As String = gvCollaboration.Finder.FilterExpressions(COLUMN_TENANT).FilterValue.ToString()
                FilterHelper.SetFilterValue(control, value)
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
                        imgDocTypeItem.Image.ImageUrl = "../Comm/images/Docsuite/Protocollo16.gif"
                    Case CollaborationDocumentType.D.ToString()
                        imgDocTypeItem.Image.ImageUrl = "../Comm/images/Docsuite/Delibera16.gif"
                    Case CollaborationDocumentType.A.ToString()
                        imgDocTypeItem.Image.ImageUrl = "../Comm/images/Docsuite/Atto16.gif"
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
                imgDocTypeItem.CommandArgument = String.Format("{0}|{1}|{2}", item.Entity.IdCollaboration, item.Entity.DocumentType, item.TenantModel.TenantName)
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
                    If ProtocolEnv.MultiDomainEnabled AndAlso Not item.TenantModel.CurrentTenant Then
                        url = String.Format("{0}/?Tipo=Coll&Azione=Documenti&Identificativo={1}", item.TenantModel.DSWUrl, item.Entity.IdCollaboration)
                        imgTypeItem.NavigateUrl = url
                        imgTypeItem.Target = "_blank"
                    Else
                        imgTypeItem.NavigateUrl = ResolveUrl(url)
                    End If
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
                        Dim protocolId As String = ""
                        If item.Entity.Year.HasValue AndAlso item.Entity.Number.HasValue Then
                            protocolId = String.Format("{0}/{1:0000000}", item.Entity.Year.Value, item.Entity.Number.Value)
                        End If
                        lnkNumberItem.Text = String.Format("{0}{1}{2}", protocolId, WebHelper.Br, item.Entity.PublicationDate.DefaultString())
                        Dim currentCollaboration As Collaboration = Facade.CollaborationFacade.GetById(item.Entity.IdCollaboration.Value)
                        lnkNumberItem.CommandArgument = String.Format("{0}|{1}|{2:0000000}|{3}", currentCollaboration.IdDocumentUnit, item.Entity.Year, item.Entity.Number, item.TenantModel.TenantName)

                    Case CollaborationDocumentType.D.ToString(), CollaborationDocumentType.A.ToString()
                        Dim reslType As ResolutionType
                        If item.Entity.HasResolution() Then
                            If item.Entity.DocumentType.Eq(CollaborationDocumentType.D.ToString()) Then
                                reslType = Facade.ResolutionTypeFacade.GetById(ResolutionType.IdentifierDelibera)
                            Else
                                reslType = Facade.ResolutionTypeFacade.GetById(ResolutionType.IdentifierDetermina)
                            End If
                            Dim formattedNumber As String = Facade.ResolutionFacade.ComposeReslNumber(item.Entity.Resolution.Year, item.Entity.Resolution.Number,
                                                                                                          item.Entity.Resolution.ServiceNumber, item.Entity.Resolution.IdResolution.Value, reslType, item.Entity.Resolution.AdoptionDate,
                                                                                                          item.Entity.Resolution.PublishingDate, False, item.TenantModel.TenantName)
                            lnkNumberItem.Text = If(item.Entity.HasResolution(), formattedNumber, "")
                            lnkNumberItem.CommandArgument = String.Format("{0}|{1}", item.Entity.Resolution.IdResolution, item.TenantModel.TenantName)
                        End If
                    Case CollaborationDocumentType.S.ToString()
                        If item.Entity.HasSeries() Then
                            If item.Entity.DocumentSeriesItem.Year.HasValue Then
                                lnkNumberItem.Text = String.Format("{0}/{1:0000000}", item.Entity.DocumentSeriesItem.Year, item.Entity.DocumentSeriesItem.Number)
                            Else
                                lnkNumberItem.Text = String.Format("Bozza N. {0}", item.Entity.DocumentSeriesItem.IdDocumentSeriesItem)
                            End If
                            lnkNumberItem.CommandArgument = String.Format("{0}|{1}", item.Entity.DocumentSeriesItem.IdDocumentSeriesItem, item.TenantModel.TenantName)
                        End If
                End Select

                If item.Entity.DocumentType.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(item.Entity.DocumentType, 0) Then
                    Dim results As ICollection(Of WebAPIDto(Of Entity.UDS.UDSCollaboration)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentUDSCollaborationFinder,
                        Function(impersonationType, finder)
                            finder.ResetDecoration()
                            finder.EnablePaging = False
                            finder.IdCollaboration = item.Entity.IdCollaboration
                            finder.ExpandRepository = True
                            Return finder.DoSearch()
                        End Function)

                    If results IsNot Nothing Then
                        Dim udsCollaboration As Entity.UDS.UDSCollaboration = results.Select(Function(s) s.Entity).SingleOrDefault()
                        If udsCollaboration IsNot Nothing Then
                            Dim udsId As String = String.Empty
                            Dim tmpRepository As UDSRepository = CurrentUDSRepositoryFacade.GetById(udsCollaboration.Repository.UniqueId)
                            Dim tmpUDS As UDSDto = CurrentUDSFacade.GetUDSSource(tmpRepository, String.Concat("$filter=UDSId eq ", udsCollaboration.IdUDS))
                            If tmpUDS IsNot Nothing Then
                                udsId = String.Format("{0}/{1:0000000}", tmpUDS.Year, tmpUDS.Number)
                            End If
                            lnkNumberItem.Text = String.Format("{0}{1}{2}", udsId, WebHelper.Br, item.Entity.PublicationDate.DefaultString())
                            lnkNumberItem.CommandArgument = String.Format("{0}|{1}|{2}", udsCollaboration.IdUDS, udsCollaboration.Repository.UniqueId, item.TenantModel.TenantName)
                        End If
                    End If
                End If
            End If

            Dim lblSignsItem As Label = CType(e.Item.FindControl("lblSigns"), Label)
            If lblSignsItem IsNot Nothing Then
                lblSignsItem.Text = SetSignsLabel(item.Entity)
            End If

            Dim lblTenantName As Label = CType(e.Item.FindControl("lblTenantName"), Label)
            If lblTenantName IsNot Nothing Then
                lblTenantName.Text = item.TenantModel.TenantName
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

#End Region

#Region " Methods "

    Private Sub SetCollSelected(idCollaboration As Integer, documentType As String, tenant As String)
        If Not ProtocolEnv.MultiDomainEnabled Then
            Dim coll As Collaboration = Facade.CollaborationFacade.GetById(idCollaboration)
            If coll Is Nothing Then
                BasePage.AjaxAlert(Server.HtmlEncode("La Registrazione è stata modificata\n\nAggiornamento automatico dell'elenco"), False)
                RaiseEvent OnRefresh(Me, New CollaborationEventArgs(idCollaboration))
                Exit Sub
            Else
                RaiseEvent OnSelectedCollaboration(Me, New CollaborationEventArgs(idCollaboration) With {.DocumentType = documentType})
            End If
        Else
            RaiseEvent OnSelectedCollaboration(Me, New CollaborationEventArgs(idCollaboration) With {.DocumentType = documentType, .TenantName = tenant})
        End If
    End Sub

    Public Sub SelectCollaboration(ByVal idCollaboration As Integer)
        SetCollSelected(idCollaboration, "", DocSuiteContext.Current.CurrentTenant.TenantName)
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

#End Region

End Class