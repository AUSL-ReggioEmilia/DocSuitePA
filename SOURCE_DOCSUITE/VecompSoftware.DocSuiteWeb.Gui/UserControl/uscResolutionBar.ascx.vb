Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade.Common.OData

Partial Public Class uscResolutionBar
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Dim _reslId As Integer
    Dim _currentResolutionRight As ResolutionRights
    Dim _currentTabWorkflow As TabWorkflow
    Private _currentODataFacade As ODataFacade = Nothing
#End Region

#Region " Properties "

    Public Property CurrentResolution() As Resolution

    Public ReadOnly Property CurrentResolutionRight As ResolutionRights
        Get
            If _currentResolutionRight Is Nothing AndAlso CurrentResolution IsNot Nothing Then
                _currentResolutionRight = New ResolutionRights(CurrentResolution)
            End If
            Return _currentResolutionRight
        End Get
    End Property

    Private Property CurrentResolutionModel As ResolutionInsertModel
        Get
            If Session("CurrentResolutionModel") IsNot Nothing Then
                Return DirectCast(Session("CurrentResolutionModel"), ResolutionInsertModel)
            End If
            Return Nothing
        End Get
        Set(value As ResolutionInsertModel)
            If value Is Nothing Then
                Session.Remove("CurrentResolutionModel")
            Else
                Session("CurrentResolutionModel") = value
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentActiveTabWorkflow() As TabWorkflow
        Get
            If _currentTabWorkflow Is Nothing AndAlso CurrentResolution IsNot Nothing Then
                _currentTabWorkflow = Facade.TabWorkflowFacade.GetActive(CurrentResolution)
            End If
            Return _currentTabWorkflow
        End Get
    End Property

    Public Property ReslId() As Integer
        Get
            Return _reslId
        End Get
        Set(ByVal value As Integer)
            _reslId = value
        End Set
    End Property

    Public ReadOnly Property TableDocuments As Table
        Get
            Return tblButtons
        End Get
    End Property

    Public ReadOnly Property PanelDocumentButtons() As Panel
        Get
            Return pnlButtonsDocument
        End Get
    End Property

    Public ReadOnly Property ButtonProposal() As Button
        Get
            Return btnProposta
        End Get
    End Property

    Public ReadOnly Property ButtonDoc4() As Button
        Get
            Return btnDoc4
        End Get
    End Property

    Public ReadOnly Property ButtonDoc5() As Button
        Get
            Return btnDoc5
        End Get
    End Property

    Public ReadOnly Property ButtonFrontespizio() As Button
        Get
            Return btnFrontespizio
        End Get
    End Property

    Public ReadOnly Property ButtonDeleteFrontespizio() As Button
        Get
            Return btnDeleteFrontespizio
        End Get
    End Property

    Public ReadOnly Property ButtonDeleteUltimaPagina() As Button
        Get
            Return btnDeleteUltimaPagina
        End Get
    End Property

    Public ReadOnly Property ButtonUltimaPagina() As Button
        Get
            Return btnUltimaPagina
        End Get
    End Property

    Public ReadOnly Property ButtonLastPageUpload As Button
        Get
            Return btnLastPageUpload
        End Get
    End Property

    Public ReadOnly Property ButtonPrivacyAttachments() As Button
        Get
            Return btnPrivacyAttachments
        End Get
    End Property

    Public ReadOnly Property PanelWebPublishButtons() As Panel
        Get
            Return pnlWebPublication
        End Get
    End Property

    Public ReadOnly Property TempTable() As TableRow
        Get
            Return rowBtn
        End Get
    End Property

    Public ReadOnly Property ButtonPublishWeb() As Button
        Get
            Return bntPubblicaInternet
        End Get
    End Property

    Public ReadOnly Property ButtonRevokeWeb() As Button
        Get
            Return bntRitiraInternet
        End Get
    End Property

    Public ReadOnly Property ButtonMail() As Button
        Get
            Return btnMail
        End Get
    End Property

    Public ReadOnly Property ButtonMailRoles() As Button
        Get
            Return btnMailSettori
        End Get
    End Property

    Public ReadOnly Property ButtonPrint() As Button
        Get
            Return btnStampa
        End Get
    End Property

    Public ReadOnly Property ButtonDuplicate() As Button
        Get
            Return btnDuplica
        End Get
    End Property

    Public ReadOnly Property PanelExtraButtons() As Panel
        Get
            Return pnlButtonsExtra
        End Get
    End Property

    Public ReadOnly Property ButtonDocument() As Button
        Get
            Return btnPratica
        End Get
    End Property

    Public ReadOnly Property ButtonAddToPratica() As Button
        Get
            Return btnAddToPratica
        End Get
    End Property

    Public ReadOnly Property ButtonbtnToSeries() As Button
        Get
            Return btnToSeries
        End Get
    End Property

    Public ReadOnly Property ButtonChange() As Button
        Get
            Return btnModifica
        End Get
    End Property

    Public ReadOnly Property ButtonDelete() As HtmlInputButton
        Get
            Return inputElimina
        End Get
    End Property

    Public ReadOnly Property ButtonCancel() As Button
        Get
            Return btnAnnulla
        End Get
    End Property

    Public ReadOnly Property ButtonLog() As Button
        Get
            Return btnLog
        End Get
    End Property

    Public ReadOnly Property ButtonPubblicaRevoca() As Button
        Get
            Return btnPubblicaRevoca
        End Get
    End Property

    Public ReadOnly Property ButtonPublishedDocument() As Button
        Get
            Return btnPublished
        End Get
    End Property

    Public ReadOnly Property ButtonFascicle As Button
        Get
            Return btnFascicle
        End Get
    End Property

    Public ReadOnly Property PanelPreviewButtons() As Panel
        Get
            Return pnlToolbarPreView
        End Get
    End Property

    Public ReadOnly Property ButtonRegistration() As Button
        Get
            Return btnRegistrazione
        End Get
    End Property

    Public ReadOnly Property ButtonFlushAnnexed() As Button
        Get
            Return btnFlushAnnexed
        End Get
    End Property

    Public ReadOnly Property RepeaterbtnRoles() As Repeater
        Get
            Return btnRolesRepeater
        End Get
    End Property

    Public ReadOnly Property CurrentODataFacade As ODataFacade
        Get
            If _currentODataFacade Is Nothing Then
                _currentODataFacade = New ODataFacade()
                Return _currentODataFacade
            End If
            Return _currentODataFacade
        End Get
    End Property

    Public ReadOnly Property ButtonConfirmView() As Button
        Get
            Return btnConfirmView
        End Get
    End Property
#End Region

#Region " Events "
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeButtons()
        InitializeAjax()
    End Sub

    Protected Sub BtnAddToPraticaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddToPratica.Click
        Response.Redirect("../Docm/DocmRicerca.aspx?" & CommonShared.AppendSecurityCheck("Type=Docm&ReslId=" & ReslId))
    End Sub

    Protected Sub btnToSeries_OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnToSeries.Click
        Dim params As String = String.Format("IdResolution={0}&Type=Resl", ReslId)
        BindModelFromPage()
        Response.Redirect("../Resl/ToSeries.aspx?" & CommonShared.AppendSecurityCheck(params))
    End Sub

    Protected Sub BtnConfirmSeries_OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirmSeries.Click
        Dim params As String = String.Format("IdResolution={0}&Type=Resl", ReslId)
        Response.Redirect("../Resl/ReslSeriesToConfirm.aspx?" & CommonShared.AppendSecurityCheck(params))
    End Sub

    Protected Sub BtnFascicle_OnClick(sender As Object, e As EventArgs) Handles btnFascicle.Click
        Dim params As String = String.Format("Type=Fasc&UDType={0}&UniqueId={1}&CategoryId={2}", Convert.ToInt32(DSWEnvironment.Resolution), CurrentResolution.UniqueId, If(IsNothing(CurrentResolution.SubCategory), CurrentResolution.Category, CurrentResolution.SubCategory).Id)
        Response.Redirect(String.Format("../Fasc/FascUDManager.aspx?{0}", params))
    End Sub

    Private Sub BtnRolesRepeaterItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles btnRolesRepeater.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As ResolutionRoleType = DirectCast(e.Item.DataItem, ResolutionRoleType)
        Dim btn As Button = DirectCast(e.Item.FindControl("btnRole"), Button)

        btn.Text = If(String.IsNullOrEmpty(item.CommandName), item.Name, item.CommandName)
        btn.PostBackUrl = ResolveUrl(String.Format("~/Resl/ReslAutorizza.aspx?Type=Resl&IdResolution={0}&Action={1}&ResolutionRoleTypeId={2}", CurrentResolution.Id, BasePage.Action, item.Id))

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeButtons()
        btnAddToPratica.Visible = ProtocolEnv.PraticheEnabled AndAlso CurrentResolutionRight.IsPreviewable
        'Visualizzo il bottone delle serie documentali (e quindi di conseguenza lo personalizzo se e solo se l'utente corrente ha diritto su almeno 1 contenitore per le serie documentali)
        If CommonShared.UserDocumentSeriesCheckRight(DocumentSeriesContainerRightPositions.Insert) Then
            btnToSeries.Text = ResolutionEnv.ButtonSeriesTitle
            btnToSeries.Visible = Facade.ResolutionFacade.TestOperationStepProperty(CurrentActiveTabWorkflow.OperationStep, "SERIES", CurrentResolution) AndAlso CurrentResolutionRight.IsPreviewable
        End If

        If ResolutionEnv.ResolutionKindEnabled Then
            btnConfirmSeries.Visible = False
            If Facade.ResolutionFacade.HasSeriesToComplete(CurrentResolution) Then
                btnConfirmSeries.Visible = True
            End If

            'Il pulsante 'Aggiungi Amministrazione Trasparente' è visibile in tutti gli step eccetto lo step di Proposta
            If Not String.Equals(FacadeFactory.Instance.TabWorkflowFacade.GetByResolution(CurrentResolution.Id).Description, WorkflowStep.PROPOSTA) Then
                btnToSeries.Visible = CurrentResolutionRight.IsPreviewable
            End If
        End If


        WebUtils.ObjAttDisplayNone(btnProposta)
        btnProposta.Style.Add("position", "absolute !important")

        If (CurrentResolutionRight.IsDocumentViewable(CurrentActiveTabWorkflow) OrElse (CurrentResolution.EffectivenessDate.HasValue AndAlso ResolutionEnv.ShowExecutiveDocumentEnabled)) _
            AndAlso Facade.ResolutionFacade.HasDocuments(CurrentResolution, ResolutionEnv.QuickDocumentsCheckAllChains) Then

            btnQuickDocument.Enabled = True
            Dim viewableDocument As Boolean = True
            If CurrentResolution.Container.Privacy.HasValue AndAlso Convert.ToBoolean(CurrentResolution.Container.Privacy.Value) Then
                viewableDocument = CurrentResolutionRight.IsPrivacyViewable
            End If
            Dim querystring As String = String.Format("IdResolution={0}&documents={1}&attachments=true&annexes=true&documentsomissis=true&attachmentsomissis=true&previous=conditional", CurrentResolution.Id, viewableDocument.ToString().ToLower())
            btnQuickDocument.PostBackUrl = String.Concat(ResolveUrl("~/viewers/ResolutionViewer.aspx?"), CommonShared.AppendSecurityCheck(querystring))
        Else
            btnQuickDocument.Enabled = False
        End If

        Dim rrts As New List(Of ResolutionRoleType)
        For Each resolutionRoleType As ResolutionRoleType In Facade.ResolutionRoleTypeFacade.GetEnabled()

            If ResolutionEnv.CheckRoleButtonRightsEnabled AndAlso Not CurrentResolutionRight.CanInsertInContainer() AndAlso
                Not CurrentResolutionRight.IsAdministrable() AndAlso Not CurrentResolutionRight.IsExecutive() Then
                Continue For
            End If

            ' Ho i pulsanti solo se sono disponibili per tutti o se ho i diritti richiesti
            If Not ResolutionEnv.CheckRoleButtonRightsEnabled AndAlso resolutionRoleType.RightPosition.HasValue AndAlso Not CurrentResolutionRight.Check(resolutionRoleType.RightPosition.Value) Then
                Continue For
            End If

            ' Ho diritto di vedere il pulsante
            ' Verifico se ho bisogno di vederlo (check distribuzione)
            If Not resolutionRoleType.RightDistributed.HasValue OrElse Not CurrentResolutionRight.Check(resolutionRoleType.RightDistributed.Value, True) Then
                ' Pulsante di distribuzione, lo visualizzo solo se il diritto NON ce l'ho da contenitore
                rrts.Add(resolutionRoleType)
            End If

        Next
        Dim cat As Category
        If CurrentResolution.SubCategory Is Nothing Then
            cat = CurrentResolution.Category
        Else
            cat = CurrentResolution.SubCategory
        End If

        Dim firstCategoryWithCategoryFascicle As String() = New CategoryFascicleFacade().GetFirstIdCategoryWithProcedureCategoryFascicle(cat.Id).ToArray
        btnFascicle.Visible = DocSuiteContext.Current.ProtocolEnv.FascicleEnabled AndAlso CurrentODataFacade.HasViewableRight(CurrentResolution.UniqueId, DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain) AndAlso CurrentResolution.AdoptionDate.HasValue
        btnRolesRepeater.DataSource = rrts
        btnRolesRepeater.DataBind()

        If (CurrentResolution.Status.Id = ResolutionStatusId.Annullato) Then
            btnAddToPratica.Visible = False
            btnRolesRepeater.Visible = False
            btnToSeries.Visible = False
            btnFascicle.Visible = False
            btnModifica.Visible = False
        End If

    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, tblButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeleteFrontespizio, Me)
    End Sub

    Private Sub BindModelFromPage()
        Dim model As ResolutionInsertModel = New ResolutionInsertModel()
        'Tipologia di atto
        model.ResolutionKind = CurrentResolution.ResolutionKind.Id
        'Categoria
        model.Category = New List(Of Integer)()
        If CurrentResolution.Category IsNot Nothing Then
            model.Category.Add(CurrentResolution.Category.Id)
        End If
        'Contenitore
        model.Container = CurrentResolution.Container.Id
        'Dati Adozione
        model.AdoptionDate = CurrentResolution.AdoptionDate
        'Salvo lo stato degli oggetti in sessione
        CurrentResolutionModel = model
    End Sub

#End Region

End Class