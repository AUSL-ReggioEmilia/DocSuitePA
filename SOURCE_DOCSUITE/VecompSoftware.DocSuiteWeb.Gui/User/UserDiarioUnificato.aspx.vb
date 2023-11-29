Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.Text
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data.MessageLog
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UserDiary

Public Class UserDiarioUnificato
    Inherits CommBasePage


#Region "Fields"
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private _UDSRepositoryFacade As UDSRepositoryFacade
    Private _UDSFacade As UDSFacade
#End Region

#Region "Properties"
    Public ReadOnly Property CurrentUDSRepositoryFacade As UDSRepositoryFacade
        Get
            If _UDSRepositoryFacade Is Nothing Then
                _UDSRepositoryFacade = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _UDSRepositoryFacade
        End Get
    End Property

    Public ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _UDSFacade Is Nothing Then
                _UDSFacade = New UDSFacade()
            End If
            Return _UDSFacade
        End Get
    End Property

#End Region

#Region "Methods"

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(gvDiarioUnificato, gvDiarioUnificato, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, gvDiarioUnificato, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, pnlFilters, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        drpType.Items.Clear()
        drpType.Items.Add(New DropDownListItem("Visualizza Tutti", 0))

        Dim coll As SortedDictionary(Of String, DropDownListItem) = New SortedDictionary(Of String, DropDownListItem)
        For Each kind As LogKindEnum In [Enum].GetValues(GetType(LogKindEnum))
            coll.Add(kind.GetDescription(), New DropDownListItem(kind.GetDescription(), Convert.ToInt16(kind)))
        Next
        For Each dropDownListItem As DropDownListItem In coll.Values
            drpType.Items.Add(dropDownListItem)
        Next

        Dim activeUDSRepositories As IList(Of UDSRepository) = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName).GetActiveRepositories()
        For Each item As UDSRepository In activeUDSRepositories
            drpType.Items.Add(New DropDownListItem(item.Name, item.DSWEnvironment.ToString()))
        Next

        drpType.DataBind()

        If String.IsNullOrEmpty(drpType.SelectedValue) Then
            drpType.SelectedValue = 0
            Exit Sub
        End If
    End Sub

    Private Function Validations() As Boolean
        If Not rdpDateFrom.SelectedDate.HasValue Then
            AjaxAlert("Manca data Inizio")
            Return False
        End If
        If Not rdpDateTo.SelectedDate.HasValue Then
            AjaxAlert("Manca data Fine")
            Return False
        End If
        If DateDiff(DateInterval.Day, rdpDateFrom.SelectedDate.Value, rdpDateTo.SelectedDate.Value) >= ProtocolEnv.MaxDaySearchUnifiedDiary Then
            AjaxAlert(String.Format("L'Intervallo non deve superare i {0} giorni", ProtocolEnv.MaxDaySearchUnifiedDiary))
            Return False
        End If
        Return True
    End Function

    Private Sub BindDataFinder()
        Dim finder As DiaryParameterFinder = New DiaryParameterFinder With {
            .DateFrom = rdpDateFrom.SelectedDate.Value,
            .DateTo = rdpDateTo.SelectedDate.Value,
            .TypeLog = Nothing,
            .Subject = txtFinderSubject.Text
        }

        Dim typeLog As LogKindEnum
        If (Not drpType.SelectedValue.Eq("0")) AndAlso [Enum].TryParse(drpType.SelectedValue, typeLog) Then
            finder.TypeLog = typeLog
        End If
        Session.Add("DSW_UnifiedDiaryFinderObject", finder)

        gvDiarioUnificato.DataSource = FacadeFactory.Instance.UnifiedDiaryFacade.GetEntitiesLastLogs(finder.DateFrom.Value, finder.DateTo.Value, DocSuiteContext.Current.User.FullUserName, CurrentTenant.TenantAOO.UniqueId, finder.TypeLog, finder.Subject)
        gvDiarioUnificato.Finder = FacadeFactory.Instance.UnifiedDiaryFacade
        gvDiarioUnificato.AllowPaging = True
        gvDiarioUnificato.CurrentPageIndex = 0
        gvDiarioUnificato.CustomPageIndex = 0
        gvDiarioUnificato.PageSize = FacadeFactory.Instance.UnifiedDiaryFacade.PageSize
        gvDiarioUnificato.DataBindFinder()
    End Sub


    Protected Function GetLogDetails(typeLog As Integer, dateFrom As DateTime, dateTo As DateTime, id As Integer, udsId As Guid?) As IList(Of UnifiedDiary)
        Dim lista As IList(Of UnifiedDiary)
        lista = Facade.UnifiedDiaryFacade.GetEntityLogDetails(typeLog, dateFrom, dateTo, DocSuiteContext.Current.User.FullUserName, id, udsId, CurrentTenant.TenantAOO.UniqueId)
        Return lista
    End Function

    Protected Function GetLogDetailsYearNumber(typeLog As Integer, dateFrom As DateTime, dateTo As DateTime, year As String, number As String) As IList(Of UnifiedDiary)
        Dim lista As IList(Of UnifiedDiary)
        lista = Facade.UnifiedDiaryFacade.GetEntityLogDetails(typeLog, dateFrom, dateTo, DocSuiteContext.Current.User.FullUserName, Convert.ToInt16(year), Convert.ToInt32(number), Nothing, CurrentTenant.TenantAOO.UniqueId)
        Return lista
    End Function

    Protected Function GetSource(UDSRepositoryId As Guid, UDSId As Guid) As UDSDto
        Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(UDSRepositoryId)
        Return CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, UDSId))
    End Function

#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Title = "DIARIO"
        If Not ProtocolEnv.EnableUnifiedDiary Then
            Response.Redirect("~/Comm/CommIntro.aspx")
            Return
        End If

        SetResponseNoCache()
        gvDiarioUnificato.MasterTableView.NoMasterRecordsText = "Nessun log disponibile"
        InitializeAjaxSettings()

        If Not Page.IsPostBack Then

            If Not Session("DSW_UnifiedDiaryFinderObject") Is Nothing Then

                Dim finder As DiaryParameterFinder = CType(Session.Item("DSW_UnifiedDiaryFinderObject"), DiaryParameterFinder)
                rdpDateFrom.SelectedDate = finder.DateFrom
                rdpDateTo.SelectedDate = finder.DateTo
                txtFinderSubject.Text = finder.Subject
                drpType.SelectedIndex = 0
                If finder.TypeLog.HasValue Then
                    drpType.SelectedValue = finder.TypeLog.Value
                End If


            End If

            If Not rdpDateFrom.SelectedDate.HasValue Then
                rdpDateFrom.SelectedDate = DateTime.Today.AddDays(-1)
            End If
            If Not rdpDateTo.SelectedDate.HasValue Then
                rdpDateTo.SelectedDate = DateTime.Now
            End If

            Initialize()
            BindDataFinder()
        End If
    End Sub

    Private Sub cmdRefreshGrid_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRefreshGrid.Click
        If (Validations()) Then
            BindDataFinder()
        Else
            Exit Sub
        End If
    End Sub

    Protected Sub gvDiarioUnificato_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles gvDiarioUnificato.PreRender
        gvDiarioUnificato.HierarchySettings.ExpandTooltip = "Visualizza dettaglio"
    End Sub

    Private Function GetEntityObject(dataSourceRow As UnifiedDiary, Optional ByVal maxChars As Int32 = 0) As String
        Dim objectStr As String = String.Empty
        If (dataSourceRow.Collaboration IsNot Nothing) Then
            objectStr = dataSourceRow.Collaboration.CollaborationObject
        End If

        If (dataSourceRow.Document IsNot Nothing) Then
            objectStr = dataSourceRow.Document.DocumentObject
        End If

        If (dataSourceRow.Message IsNot Nothing) Then
            Dim messageEmail As MessageEmail = dataSourceRow.Message.Emails.FirstOrDefault()
            If (messageEmail IsNot Nothing) Then
                objectStr = messageEmail.Subject
            End If
        End If

        If (dataSourceRow.DocumentSeriesItem IsNot Nothing) Then
            objectStr = dataSourceRow.DocumentSeriesItem.Subject
        End If

        If (dataSourceRow.PecMail IsNot Nothing) Then
            objectStr = dataSourceRow.PecMail.MailSubject
        End If

        If (dataSourceRow.Message IsNot Nothing) Then
            objectStr = dataSourceRow.Message.Emails.First().Subject
        End If

        If (dataSourceRow.Protocol IsNot Nothing) Then
            objectStr = dataSourceRow.Protocol.ProtocolObject
        End If

        If (dataSourceRow.Resolution IsNot Nothing) Then
            objectStr = dataSourceRow.Resolution.ResolutionObject
        End If
        If maxChars > 0 AndAlso Not String.IsNullOrEmpty(objectStr) AndAlso objectStr.Length > maxChars Then
            objectStr = String.Concat(objectStr.Remove(maxChars), "  [...]")
        End If
        Return objectStr
    End Function

    Private Function GetEntityLogDescription(dataSourceRow As UnifiedDiary) As String
        Dim objectStr As String = dataSourceRow.LogDescription
        Dim enumStr As String = String.Empty
        Select Case dataSourceRow.Type
            Case LogKindEnum.Collaboration
                'Dim t As CollaborationLogType = DirectCast([Enum].Parse(GetType(CollaborationLogType), dataSourceRow.LogType), CollaborationLogType)
                'enumStr = t.GetDescription()
            Case LogKindEnum.Document
                Dim t As DocumentLogType = DirectCast([Enum].Parse(GetType(DocumentLogType), dataSourceRow.LogType), DocumentLogType)
                enumStr = t.GetDescription()
            Case LogKindEnum.DocumentSeries
                Dim t As DocumentSeriesItemLogType = DirectCast([Enum].Parse(GetType(DocumentSeriesItemLogType), dataSourceRow.LogType), DocumentSeriesItemLogType)
                enumStr = t.GetDescription()
            Case LogKindEnum.Message
                Dim t As MessageLogType = DirectCast([Enum].Parse(GetType(MessageLogType), dataSourceRow.LogType), MessageLogType)
                enumStr = t.GetDescription()
            Case LogKindEnum.PEC
                Dim t As PECMailLogType = DirectCast([Enum].Parse(GetType(PECMailLogType), dataSourceRow.LogType), PECMailLogType)
                enumStr = t.GetDescription()
            Case LogKindEnum.Protocol
                Dim t As ProtocolLogEvent = DirectCast([Enum].Parse(GetType(ProtocolLogEvent), dataSourceRow.LogType), ProtocolLogEvent)
                enumStr = t.GetDescription()
            Case LogKindEnum.Resolution
                Dim t As ResolutionLogType = DirectCast([Enum].Parse(GetType(ResolutionLogType), dataSourceRow.LogType), ResolutionLogType)
                enumStr = t.GetDescription()
            Case LogKindEnum.Sign
        End Select

        Return If(enumStr.Eq(objectStr), objectStr, String.Concat(enumStr, " ", objectStr))
    End Function

    Private Function GetEntityNavigateUrl(dataSourceRow As UnifiedDiary) As String
        Dim navigateUrl As String = String.Empty
        If (dataSourceRow.Collaboration IsNot Nothing) Then
            navigateUrl = String.Format("~/User/UserCollGestione.aspx?idCollaboration={0}&Type=Prot&Action=Prt&Titolo=Inserimento&Title2=Visualizzazione&Action2=CP", dataSourceRow.Collaboration.Id)
        End If

        If (dataSourceRow.Document IsNot Nothing) Then
            navigateUrl = String.Concat("~/Docm/DocmVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}&Type=Docm", dataSourceRow.Document.Year, dataSourceRow.Document.Number)))
        End If

        If (dataSourceRow.DocumentSeriesItem IsNot Nothing) Then
            navigateUrl = String.Concat("~/Series/Item.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdDocumentSeriesItem={0}&Action=View&Type=Series", dataSourceRow.DocumentSeriesItem.Id)))
        End If

        If (dataSourceRow.PecMail IsNot Nothing) Then
            navigateUrl = String.Concat("~/PEC/PECSummary.aspx?", CommonShared.AppendSecurityCheck(String.Format("Type=Pec&PECId={0}", dataSourceRow.PecMail.Id)))
        End If

        If (dataSourceRow.Protocol IsNot Nothing) Then
            navigateUrl = String.Concat("~/Prot/ProtVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("UniqueId={0}&Type=Prot", dataSourceRow.Protocol.Id)))
        End If

        If (dataSourceRow.Resolution IsNot Nothing) Then
            navigateUrl = String.Concat("~/Resl/ReslVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&Type=Resl", dataSourceRow.Resolution.Id)))
        End If

        If (dataSourceRow.UDSId.HasValue AndAlso dataSourceRow.IdUDSRepository.HasValue AndAlso dataSourceRow.UDSId <> Guid.Empty AndAlso dataSourceRow.IdUDSRepository <> Guid.Empty) Then
            navigateUrl = String.Concat("~/UDS/UDSView.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdUDSRepository={0}&IdUDS={1}&Action=View&Type=UDS", dataSourceRow.IdUDSRepository.Value, dataSourceRow.UDSId.Value)))
        End If

        Return navigateUrl
    End Function

    Protected Sub gvDiarioUnificato_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles gvDiarioUnificato.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If
        gvDiarioUnificato.HierarchySettings.ExpandTooltip = "Visualizza log"

        Dim dataSourceRow As UnifiedDiary = DirectCast(e.Item.DataItem, UnifiedDiary)

        Dim dataItem As GridDataItem = DirectCast(e.Item, GridDataItem)

        Dim udsSource As UDSDto = New UDSDto
        If dataSourceRow.Type > 100 AndAlso dataSourceRow.UDSId.HasValue AndAlso dataSourceRow.UDSId <> Guid.Empty Then
            udsSource = GetSource(dataSourceRow.IdUDSRepository.Value, dataSourceRow.UDSId.Value)
        End If

        If TypeOf e.Item Is GridDataItem And e.Item.OwnerTableView.Name.Equals("MasterGrid") Then
            Dim imgPriority As Image = DirectCast(e.Item.FindControl("imgPriority"), Image)
            Dim btnLog As RadButton = DirectCast(e.Item.FindControl("btnLog"), RadButton)
            Dim lblLogDate As Label = DirectCast(e.Item.FindControl("lblLogDate"), Label)
            Dim lblType As Label = DirectCast(e.Item.FindControl("lblType"), Label)
            Dim lblLogType As HyperLink = DirectCast(e.Item.FindControl("lblLogType"), HyperLink)
            Dim lblLogDescription As Label = DirectCast(e.Item.FindControl("lblLogDescription"), Label)
            Dim hfId As HiddenField = DirectCast(e.Item.FindControl("hfId"), HiddenField)
            Dim hfUniqueId As HiddenField = DirectCast(e.Item.FindControl("hfUniqueId"), HiddenField)
            Dim hfType As HiddenField = DirectCast(e.Item.FindControl("hfType"), HiddenField)

            imgPriority.Visible = False
            If (dataSourceRow.Severity.HasValue) Then
                imgPriority.Visible = True
            End If
            btnLog.Enabled = False
            If dataSourceRow.Type < 100 Then
                lblType.Text = CType(dataSourceRow.Type, LogKindEnum).GetDescription()
            Else
                lblType.Text = udsSource.UDSModel.Model.Title
            End If

            hfId.Value = dataSourceRow.Id.ToString()
            hfUniqueId.Value = Guid.Empty.ToString()
            hfType.Value = dataSourceRow.Type.ToString()
            lblLogDate.Text = dataSourceRow.LogDate.ToString()

            If dataSourceRow.Type < 100 Then
                lblLogDescription.Text = GetEntityObject(dataSourceRow, DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec)
            Else
                lblLogDescription.Text = udsSource.Subject
            End If

            lblLogType.NavigateUrl = GetEntityNavigateUrl(dataSourceRow)

            Select Case dataSourceRow.Type

                Case LogKindEnum.Sign
                    lblLogType.Text = GetEntityObject(dataSourceRow)
                    dataItem("ExpandColumn").Controls(0).Visible = False

                Case LogKindEnum.Collaboration
                    If (dataSourceRow.Collaboration IsNot Nothing) Then
                        lblLogType.Text = dataSourceRow.Collaboration.CollaborationObject
                        hfId.Value = dataSourceRow.Collaboration.Id
                    End If
                Case LogKindEnum.Document
                    If (dataSourceRow.Document IsNot Nothing) Then
                        lblLogType.Text = String.Format("{0}/{1}/{2}", dataSourceRow.Document.Year, dataSourceRow.Document.Number, dataSourceRow.Document.Category.Name)
                        With DirectCast(e.Item.FindControl("hfYear"), HiddenField)
                            .Value = dataSourceRow.Document.Year
                        End With
                        With DirectCast(e.Item.FindControl("hfNumber"), HiddenField)
                            .Value = dataSourceRow.Document.Number
                        End With

                    End If
                Case LogKindEnum.DocumentSeries
                    lblLogType.Text = String.Format("{0} - {1}", DocumentSeriesItemDTO(Of Object).GetIdentifierString(dataSourceRow.DocumentSeriesItem.Status, dataSourceRow.DocumentSeriesItem.Year, dataSourceRow.DocumentSeriesItem.Number, dataSourceRow.DocumentSeriesItem.Id), dataSourceRow.DocumentSeriesItem.DocumentSeries.Name)
                    hfId.Value = dataSourceRow.DocumentSeriesItem.Id

                Case LogKindEnum.Message
                    btnLog.Enabled = True
                    btnLog.ToolTip = "Visualizza log Email"
                    Dim rec As New StringBuilder()
                    For Each recipient As MessageContact In dataSourceRow.Message.MessageContacts.Where(Function(f) f.ContactPosition.Equals(MessageContact.ContactPositionEnum.Recipient))
                        rec.Append(String.Format(" {0} ", recipient.ContactEmails.FirstOrDefault()))
                    Next
                    Dim messageEmail As MessageEmail = dataSourceRow.Message.Emails.FirstOrDefault()
                    hfId.Value = dataSourceRow.Message.Id
                    If (messageEmail IsNot Nothing) Then
                        lblLogType.NavigateUrl = String.Format("~/Prot/MessageEmailView.aspx?Type=Prot&MessageEmailId={0}", messageEmail.Id)
                        hfId.Value = messageEmail.Id
                    End If

                    lblLogType.Text = String.Format("{0} {1}", dataSourceRow.Message.RegistrationDate, rec)


                Case LogKindEnum.PEC
                    btnLog.Enabled = CommonShared.HasGroupPecMailLogViewRight
                    btnLog.ToolTip = "Visualizza log PEC"
                    lblLogType.Text = String.Format("{0} {1} {2}", If(dataSourceRow.PecMail.Direction = PECMailDirection.Ingoing, "Ricevuta il ", If(dataSourceRow.PecMail.MailDate.HasValue, "Inviata il ", "NON ANCORA INVIATA, ")), If(dataSourceRow.PecMail.MailDate.HasValue, dataSourceRow.PecMail.MailDate.Value.ToString(), String.Empty), HttpUtility.HtmlEncode(dataSourceRow.PecMail.MailRecipients))
                    hfId.Value = dataSourceRow.PecMail.Id

                Case LogKindEnum.Protocol
                    If (dataSourceRow.Protocol IsNot Nothing) Then
                        btnLog.Enabled = New ProtocolRights(dataSourceRow.Protocol).EnableViewLog
                        btnLog.ToolTip = "Visualizza log protocollo"
                        lblLogType.Text = String.Format("{0}", dataSourceRow.Protocol.FullNumber)
                        hfId.Value = dataSourceRow.Protocol.Id.ToString()

                        With DirectCast(e.Item.FindControl("hfYear"), HiddenField)
                            .Value = dataSourceRow.Protocol.Year
                        End With
                        With DirectCast(e.Item.FindControl("hfNumber"), HiddenField)
                            .Value = dataSourceRow.Protocol.Number
                        End With
                    End If


                Case LogKindEnum.Resolution
                    btnLog.Enabled = ResolutionEnv.IsLogEnabled
                    btnLog.ToolTip = "Visualizza log atto"
                    lblLogType.Text = If(String.IsNullOrEmpty(dataSourceRow.Resolution.InclusiveNumber), "<numero non assegnato>", dataSourceRow.Resolution.InclusiveNumber)
                    hfId.Value = dataSourceRow.Resolution.Id

                Case > 100
                    'TODO: da decommentare quando verrà implementata la pagina di visualizzazione dei log delle UDS
                    'btnLog.Enabled = udsSource.UDSModel.Model.PecButtonEnabled
                    'btnLog.ToolTip = String.Format("Visualizza log {0}", udsSource.UDSModel.Model.Title)
                    btnLog.Enabled = False
                    lblLogType.Text = udsSource.FullNumber
                    hfUniqueId.Value = udsSource.Id.ToString()
                Case Else

            End Select
            If (Not btnLog.Enabled) Then
                btnLog.Style.Add("opacity", "0.4")
                btnLog.Style.Add("filter", "alpha(opacity=40)")
            End If
        End If

        If TypeOf e.Item Is GridDataItem And e.Item.OwnerTableView.Name.Equals("ChildGrid") Then
            Dim lblDetailDate As Label = DirectCast(e.Item.FindControl("lblDetailDate"), Label)
            Dim lblDetailType As Label = DirectCast(e.Item.FindControl("lblDetailType"), Label)
            Dim lblDetailDescription As Label = DirectCast(e.Item.FindControl("lblDetailDescription"), Label)

            lblDetailDate.Text = dataSourceRow.LogDate

            If dataSourceRow.Type < 100 Then
                lblDetailType.Text = CType(dataSourceRow.Type, LogKindEnum).GetDescription()
            Else
                lblDetailType.Text = udsSource.UDSModel.Model.Title
            End If

            lblDetailDescription.Text = GetEntityLogDescription(dataSourceRow)
        End If

    End Sub

    Private Function SetDetailDescription(logDescription As String) As String
        Dim labelText As String = logDescription
        If DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec > 0 AndAlso logDescription.Length > DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec Then
            labelText = String.Concat(logDescription.Remove(DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec), " [...]")
        End If
        Return labelText
    End Function

    Private Sub gvDiarioUnificato_DetailTableDataBind(ByVal source As Object, ByVal e As GridDetailTableDataBindEventArgs) Handles gvDiarioUnificato.DetailTableDataBind
        Dim dataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)
        If dataItem.Edit Then
            Return
        End If

        Dim gridDataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)

        Dim dateFrom As DateTime = rdpDateFrom.SelectedDate
        Dim dateTo As DateTime = rdpDateTo.SelectedDate

        Dim logType As Integer? = Nothing
        Dim year As String = String.Empty
        Dim number As String = String.Empty
        Dim id As Integer? = Nothing
        Dim uniqueId As Guid? = Nothing

        If gridDataItem.FindControl("hfType") IsNot Nothing Then
            Dim enumId As Int32
            If Int32.TryParse(DirectCast(gridDataItem.FindControl("hfType"), HiddenField).Value, enumId) Then
                logType = enumId
            End If
        End If

        If Not dataItem Is Nothing And logType.HasValue Then
            If gridDataItem.FindControl("hfId") IsNot Nothing Then
                Dim ret As Int32
                If Int32.TryParse(DirectCast(gridDataItem.FindControl("hfId"), HiddenField).Value, ret) Then
                    id = ret
                End If
            End If

            If gridDataItem.FindControl("hfYear") IsNot Nothing Then
                year = DirectCast(gridDataItem.FindControl("hfYear"), HiddenField).Value
            End If

            If gridDataItem.FindControl("hfNumber") IsNot Nothing Then
                number = DirectCast(gridDataItem.FindControl("hfNumber"), HiddenField).Value
            End If

            If gridDataItem.FindControl("hfUniqueId") IsNot Nothing Then
                uniqueId = Guid.Parse(DirectCast(gridDataItem.FindControl("hfUniqueId"), HiddenField).Value)
            End If

            If (logType = LogKindEnum.Collaboration OrElse logType = LogKindEnum.DocumentSeries OrElse
                      logType = LogKindEnum.Message OrElse logType = LogKindEnum.PEC OrElse logType = LogKindEnum.Resolution) _
                    AndAlso id.HasValue Then

                e.DetailTableView.DataSource = GetLogDetails(logType.Value, dateFrom, dateTo, id.Value, Nothing)
                Return
            End If

            If logType > 100 AndAlso uniqueId.HasValue Then
                e.DetailTableView.DataSource = GetLogDetails(logType.Value, dateFrom, dateTo, id.Value, uniqueId)
            End If

            If (logType = LogKindEnum.Document OrElse logType = LogKindEnum.Protocol) AndAlso
                    Not String.IsNullOrEmpty(year) AndAlso Not String.IsNullOrEmpty(number) Then

                e.DetailTableView.DataSource = GetLogDetailsYearNumber(logType.Value, dateFrom, dateTo, year, number)
                Return
            End If
        End If

    End Sub
    Protected Sub gvDiarioUnificato_ItemCommand(source As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles gvDiarioUnificato.ItemCommand
        If e.CommandName = "Log" Then
            Dim dataItem As GridDataItem = DirectCast(e.Item, GridDataItem)
            Dim logType As String = DirectCast(dataItem.FindControl("lblType"), Label).Text
            Select Case logType

                Case LogKindEnum.Protocol.GetDescription()
                    Dim uniqueIdProtocol As Guid = Guid.Parse(DirectCast(dataItem.FindControl("hfId"), HiddenField).Value)
                    Dim redirectUrl As String = String.Format("Type=Prot&UniqueId={0}", uniqueIdProtocol)
                    redirectUrl = String.Concat("~/Prot/ProtLog.aspx?", CommonShared.AppendSecurityCheck(redirectUrl))
                    Response.Redirect(redirectUrl)

                Case LogKindEnum.Resolution.GetDescription()

                    Dim Id As String = DirectCast(dataItem.FindControl("hfId"), HiddenField).Value
                    Dim redirectUrl As String = String.Format("Type=Resl&IdResolution={0}")
                    redirectUrl = String.Concat("~/Resl/ReslLog.aspx?", CommonShared.AppendSecurityCheck(redirectUrl))
                    Response.Redirect(redirectUrl)

                Case LogKindEnum.PEC.GetDescription()

                    Dim Id As String = DirectCast(dataItem.FindControl("hfId"), HiddenField).Value
                    Dim redirectUrl As String = String.Format("Type=PEC&PECId={0}&ProtocolBox=False", Id)
                    redirectUrl = String.Concat("~/PEC/PecViewLog.aspx?", redirectUrl)
                    Response.Redirect(redirectUrl)

                Case LogKindEnum.Message.GetDescription()
                    Dim Id As String = DirectCast(dataItem.FindControl("hfId"), HiddenField).Value
                    Dim redirectUrl As String = String.Format("Type=Comm&MessageEmailId={0}", Id)
                    redirectUrl = String.Concat("~/MailSenders/MessageViewLog.aspx?", redirectUrl)
                    Response.Redirect(redirectUrl)


            End Select


        End If
    End Sub

#End Region

End Class

