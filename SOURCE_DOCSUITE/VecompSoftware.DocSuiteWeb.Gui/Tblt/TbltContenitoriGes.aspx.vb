Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
Imports APIEntity = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI

Partial Class TbltContenitoriGes
    Inherits CommonBasePage

#Region " Fields "

    Private _currentContainer As Container
    Private _currentPrivacyLevelFinder As Data.WebAPI.Finder.Commons.PrivacyLevelFinder = Nothing

#End Region

#Region " Properties "

    Public ReadOnly Property CurrentContainer() As Container
        Get
            If _currentContainer Is Nothing Then
                Dim temp As Integer = Request.QueryString.GetValueOrDefault("IdContainer", 0)
                If Not temp.Equals(0) Then
                    _currentContainer = Facade.ContainerFacade.GetById(temp, False)
                Else
                    _currentContainer = New Container()
                End If
            End If
            Return _currentContainer
        End Get
    End Property

    Public ReadOnly Property CurrentPrivacyLevelFinder() As Data.WebAPI.Finder.Commons.PrivacyLevelFinder
        Get
            If _currentPrivacyLevelFinder Is Nothing Then
                _currentPrivacyLevelFinder = New Data.WebAPI.Finder.Commons.PrivacyLevelFinder(DocSuiteContext.Current.CurrentTenant)
                Return _currentPrivacyLevelFinder
            Else
                Return _currentPrivacyLevelFinder
            End If
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False

        If Not CommonShared.HasGroupAdministratorRight AndAlso Not CommonShared.HasGroupTblContainerAdminRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            AjaxManager.ResponseScripts.Add("CloseWindow();")
            Exit Sub
        End If

        InitializaAjax()

        If Not IsPostBack Then
            InitializePage()
        End If
    End Sub

    Protected Sub chkIsPrivacy_CheckedChanged(sender As Object, e As EventArgs)
        lblPrivacyLevel.Disabled = Not chkIsPrivacy.Checked
        ddlPrivacyLevel.SelectedValue = "0"
    End Sub
    Private Sub BtnConfermaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        If String.IsNullOrEmpty(txtName.Text) Then
            AjaxAlert("Campo nome obbligatorio")
            Exit Sub
        End If

        Select Case Action
            Case "Add"
                If CheckContainerNameExist() Then
                    AjaxAlert("Esiste già un contenitore con lo stesso nome")
                    Exit Sub
                End If
                FillContainer(CurrentContainer)
                Facade.ContainerFacade.Save(CurrentContainer)

                Dim currentTenant As Tenant = CType(Session("CurrentTenant"), Tenant)
                currentTenant.Containers.Add(New APIEntity.Container With {.EntityShortId = CType(CurrentContainer.Id, Short)})
                Dim currentTenantFacade As WebAPI.Tenants.TenantFacade = New WebAPI.Tenants.TenantFacade(DocSuiteContext.Current.Tenants, currentTenant)
                currentTenantFacade.Update(currentTenant, UpdateActionType.TenantContainerAdd.ToString())
                currentTenant.Containers.Clear()

                FillDocumentSeries(CurrentContainer)
                FillFrontalino(CurrentContainer)
                Facade.ContainerFacade.Update(CurrentContainer)
                FillContainerBehaviours(CurrentContainer)
            Case "Rename"
                If CheckContainerNameExist() AndAlso Not txtName.Text.Eq(CurrentContainer.Name) Then
                    AjaxAlert("Esiste già un contenitore con lo stesso nome")
                    Exit Sub
                End If
                FillContainer(CurrentContainer)
                FillDocumentSeries(CurrentContainer)
                FillFrontalino(CurrentContainer)
                FillContainerBehaviours(CurrentContainer)
                Facade.ContainerFacade.Update(CurrentContainer)
            Case "Delete"
                Facade.ContainerFacade.Delete(CurrentContainer)
                Facade.DocumentSeriesFacade.DeactivateDocumentSeries(CurrentContainer)
                Facade.ContainerGroupFacade.DeactivateUD(CurrentContainer)
            Case "Recovery"
                CurrentContainer.IsActive = 1
                Facade.ContainerFacade.Update(CurrentContainer)
        End Select

        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}', '{1}', {2});", Action, CurrentContainer.Name, CurrentContainer.Id.ToString()))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializaAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPrivacy)
    End Sub


    Private Function IsConservationEnable() As Boolean
        If DocSuiteContext.Current.IsProtocolEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.IsConservationEnabled Then
            Return True
        End If
        If DocSuiteContext.Current.IsResolutionEnabled AndAlso DocSuiteContext.Current.ResolutionEnv.IsConservationEnabled Then
            Return True
        End If
        If DocSuiteContext.Current.IsDocumentEnabled AndAlso DocSuiteContext.Current.DocumentEnv.IsConservationEnabled Then
            Return True
        End If
        Return False
    End Function

    Private Sub InitializePage()

        lblDocumentSeries.Text = ProtocolEnv.DocumentSeriesName

        Select Case Action
            Case "Add"
                Page.Title = "Contenitori - Aggiungi"
                pnlLocation.Visible = True

            Case "Rename"
                Page.Title = String.Format("Contenitore [{0}] - Rinomina ", CurrentContainer.Name)
                pnlLocation.Visible = True
                txtName.Text = CurrentContainer.Name
                txtNote.Text = CurrentContainer.Note
                txtName.Focus()
            Case "Delete", "Recovery"
                Page.Title = String.Format("Contenitore [{0}] - {1} ", CurrentContainer.Name, If(Action.Eq("Delete"), "Elimina", "Recupera"))
                pnlLocation.Visible = False
                pnlConservation.Visible = False
                pnlDocumentSeries.Visible = False
                pnlPrivacy.Disabled = True
                chkIsPrivacy.ForeColor = Drawing.Color.Gray
                txtName.Text = CurrentContainer.Name
                txtName.ReadOnly = True
                txtNote.Text = CurrentContainer.Note
                txtNote.ReadOnly = True
        End Select

        txtHeadingFrontalino.DisableFilter(Telerik.Web.UI.EditorFilters.ConvertTags)
        txtHeadingFrontalino.DisableFilter(Telerik.Web.UI.EditorFilters.MozEmStrong)
        txtHeadingLetter.DisableFilter(Telerik.Web.UI.EditorFilters.ConvertTags)
        txtHeadingLetter.DisableFilter(Telerik.Web.UI.EditorFilters.MozEmStrong)

        pnlPrivacy.Visible = False
        If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso CommonShared.HasGroupAdministratorRight Then
            pnlPrivacy.Visible = True
            chkIsPrivacy.Checked = CurrentContainer.PrivacyEnabled
            LoadLevels()
        End If

        lblPrivacyLevel.Disabled = True
        If chkIsPrivacy.Checked Then
            lblPrivacyLevel.Disabled = False
        End If

        ' Conservazione Sostitutiva
        If IsConservationEnable() Then
            pnlConservation.Visible = True
            chkConservation.Checked = CType(CurrentContainer.Conservation, Boolean)
        Else
            pnlConservation.Visible = False
        End If

        uscProtLocation.Visible = DocSuiteContext.Current.IsProtocolEnabled
        If DocSuiteContext.Current.IsProtocolEnabled Then
            If CurrentContainer.ProtLocation IsNot Nothing Then
                uscProtLocation.Location = New LocationFacade("ProtDB").GetById(CurrentContainer.ProtLocation.Id)
            End If

            uscProtAttachLocation.Visible = DocSuiteContext.Current.ProtocolEnv.IsProtocolAttachLocationEnabled
            If CurrentContainer.ProtAttachLocation IsNot Nothing Then
                uscProtAttachLocation.Location = New LocationFacade("ProtDB").GetById(CurrentContainer.ProtAttachLocation.Id)
            End If
        End If

        uscReslLocation.Visible = DocSuiteContext.Current.IsResolutionEnabled
        If DocSuiteContext.Current.IsResolutionEnabled Then
            uscReslLocation.Caption = Facade.TabMasterFacade.TreeViewCaption & ":"
            If CurrentContainer.ReslLocation IsNot Nothing Then
                uscReslLocation.Location = New LocationFacade("ReslDB").GetById(CurrentContainer.ReslLocation.Id)
            End If
        End If

        uscDocmLocation.Visible = DocSuiteContext.Current.IsDocumentEnabled
        If DocSuiteContext.Current.IsDocumentEnabled Then
            If CurrentContainer.DocmLocation IsNot Nothing Then
                uscDocmLocation.Location = New LocationFacade("DocmDB").GetById(CurrentContainer.DocmLocation.Id)
            End If
        End If

        ' Behaviours di Contenitore
        If ProtocolEnv.ContainerBehaviourEnabled Then
            uscClassificatori.Visible = True
            Dim behaviour As ContainerBehaviour = Facade.ContainerBehaviourFacade.GetBehaviours(CurrentContainer, ContainerBehaviourAction.Insert, "#uscClassificatori").FirstOrDefault()
            If behaviour IsNot Nothing Then
                Dim category As Category = Facade.CategoryFacade.GetById(Integer.Parse(behaviour.AttributeValue))
                If category IsNot Nothing Then
                    uscClassificatori.DataSource.Add(category)
                    uscClassificatori.DataBind()
                End If
            End If
        End If

        ' Serie Documentali
        uscSeriesLocation.Visible = False
        uscSeriesAnnexedLocation.Visible = False
        uscSeriesUnpublishedAnnexedLocation.Visible = False
        pnlDocumentSeries.Visible = False
        If Not ProtocolEnv.DocumentSeriesEnabled Then
            ' Se disattive rimane tutto nascosto
            Exit Sub
        End If
        ' Locations
        uscSeriesLocation.Visible = True
        uscSeriesAnnexedLocation.Visible = True
        uscSeriesUnpublishedAnnexedLocation.Visible = True
        If CurrentContainer.DocumentSeriesLocation IsNot Nothing Then
            uscSeriesLocation.Location = New LocationFacade("ProtDB").GetById(CurrentContainer.DocumentSeriesLocation.Id)
        End If
        If CurrentContainer.DocumentSeriesAnnexedLocation IsNot Nothing Then
            uscSeriesAnnexedLocation.Location = New LocationFacade("ProtDB").GetById(CurrentContainer.DocumentSeriesAnnexedLocation.Id)
        End If
        If CurrentContainer.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing Then
            uscSeriesUnpublishedAnnexedLocation.Location = New LocationFacade("ProtDB").GetById(CurrentContainer.DocumentSeriesUnpublishedAnnexedLocation.Id)
        End If
        ' Dati specifici
        Dim series As DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(CurrentContainer)
        If series IsNot Nothing Then
            pnlDocumentSeries.Visible = True

            lblDocumentSeries.ToolTip = "Identificativo"
            lblDocumentSeries.Text = String.Format("{0} [{1}]", ProtocolEnv.DocumentSeriesName, series.Id)

            chkSeriesPub.Checked = series.PublicationEnabled.GetValueOrDefault(False)
            chkSubsection.Checked = series.SubsectionEnabled.GetValueOrDefault(False)
            chkZeroDoc.Checked = series.AllowNoDocument.GetValueOrDefault(False)
            chkAddDocs.Checked = series.AllowAddDocument.GetValueOrDefault(False)
            chkRoles.Checked = series.RoleEnabled.GetValueOrDefault(False)

            ' ContainerArchive: in futuro non sarà legato alle serie documentali, per ora è visibile solo se la SD è attiva
            ddlArchive.DataSource = Facade.ContainerArchiveFacade.GetAll()
            ddlArchive.DataBind()
            ddlArchive.Items.Insert(0, "")
            If CurrentContainer.Archive IsNot Nothing Then
                ddlArchive.SelectedValue = CurrentContainer.Archive.Id.ToString()
            End If

            ' Families
            ddlDocumentSeriesFamily.DataSource = Facade.DocumentSeriesFamilyFacade.GetAll()
            ddlDocumentSeriesFamily.DataBind()
            If series.Family IsNot Nothing Then
                ddlDocumentSeriesFamily.SelectedValue = series.Family.Id.ToString()
            End If
        End If

        uscDeskLocation.Visible = False
        If ProtocolEnv.DeskEnable Then
            uscDeskLocation.Visible = True
            If CurrentContainer.DeskLocation IsNot Nothing Then
                uscDeskLocation.Location = New LocationFacade("ProtDB").GetById(CurrentContainer.DeskLocation.Id)
            End If
        End If

        If CurrentContainer.UDSLocation IsNot Nothing Then
            uscUDSLocation.Location = New LocationFacade("ProtDB").GetById(CurrentContainer.UDSLocation.Id)
        End If

        If ProtocolEnv.EnableEditingHeadingFrontalini Then
            FrontaliniPrivacy.Visible = True
            lblFrontaliniHeader.Text = "Frontalini e privacy"
            lblHeadingFrontalino.Text = "Intestazione frontalino"
            lblHeadingLetter.Text = "Intestazione lettera"
            lblPrivacy.Text = "Privacy"
            txtHeadingFrontalino.Content = CurrentContainer.HeadingFrontalino
            txtHeadingLetter.Content = CurrentContainer.HeadingLetter
            chkPrivacy.Checked = Convert.ToBoolean(CurrentContainer.Privacy)
        End If
    End Sub

    Private Sub LoadLevels()
        Dim PrivacyLevelsDTO As ICollection(Of WebAPIDto(Of APIEntity.PrivacyLevel)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentPrivacyLevelFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.EnablePaging = False
                        If CurrentContainer IsNot Nothing AndAlso CurrentContainer.ContainerGroups IsNot Nothing AndAlso CurrentContainer.ContainerGroups.Count() > 0 AndAlso CurrentContainer.ContainerGroups.Max(Function(p) p.PrivacyLevel) > 0 Then
                            finder.MaximumLevel = CurrentContainer.ContainerGroups.Max(Function(p) p.PrivacyLevel)
                        End If
                        Return finder.DoSearch()
                    End Function)

        Dim privacyLevels As IList(Of APIEntity.PrivacyLevel) = New List(Of APIEntity.PrivacyLevel)
        For Each PrivacyLevelDTO As WebAPIDto(Of APIEntity.PrivacyLevel) In PrivacyLevelsDTO
            privacyLevels.Add(PrivacyLevelDTO.Entity)
        Next
        chkIsPrivacy.Text = String.Concat("Abilita come contenitore ", PRIVACY_LABEL)
        ddlPrivacyLevel.DataSource = privacyLevels.OrderBy(Function(s) s.Level)
        ddlPrivacyLevel.DataBind()
        ddlPrivacyLevel.SelectedValue = CurrentContainer.PrivacyLevel.ToString()
    End Sub
    Private Sub FillContainer(ByVal container As Container)
        container.Name = txtName.Text
        container.Note = txtNote.Text
        container.DocmLocation = uscDocmLocation.Location
        container.ProtLocation = uscProtLocation.Location
        container.ReslLocation = uscReslLocation.Location
        container.ProtAttachLocation = uscProtAttachLocation.Location
        container.DeskLocation = uscDeskLocation.Location
        container.UDSLocation = uscUDSLocation.Location
        container.Conservation = Convert.ToByte(chkConservation.Checked)
        If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso CommonShared.HasGroupAdministratorRight Then
            If Not container.PrivacyEnabled = chkIsPrivacy.Checked OrElse Not container.PrivacyLevel = Int16.Parse(ddlPrivacyLevel.SelectedValue) Then
                Dim message As String = String.Concat("Modificato il contenitore ", container.Id, ": ")

                If Not container.PrivacyEnabled = chkIsPrivacy.Checked Then
                    If chkIsPrivacy.Checked Then
                        message = String.Concat(message, PRIVACY_LABEL, " abilitata ")
                    Else
                        message = String.Concat(message, PRIVACY_LABEL, " disabilitata ")
                    End If
                End If

                If Not container.PrivacyLevel = Int16.Parse(ddlPrivacyLevel.SelectedValue) Then
                    message = String.Concat(message, "livello ", PRIVACY_LABEL, " cambiato da ", container.PrivacyLevel, " a ", ddlPrivacyLevel.SelectedValue)
                End If
                FacadeFactory.Instance.TableLogFacade.Insert("Container", LogEvent.PR, message, container.UniqueId)
            End If
            container.PrivacyEnabled = chkIsPrivacy.Checked
            container.PrivacyLevel = Short.Parse(ddlPrivacyLevel.SelectedValue)
        End If
    End Sub
    Private Sub FillFrontalino(ByVal container As Container)
        Try
            container.HeadingFrontalino = txtHeadingFrontalino.Content
            container.HeadingLetter = txtHeadingLetter.Content
            container.Privacy = Convert.ToInt16(chkPrivacy.Checked)
        Catch ex As Exception
            AjaxAlert("Errore in fase di recupero dati 'frontalino e privacy' da Form.")
            FileLogger.Error(LoggerName, "Errore in 'frontalino e privacy'.", ex)
        End Try
    End Sub

    Private Sub FillDocumentSeries(container As Container)
        If Not ProtocolEnv.DocumentSeriesEnabled Then
            Exit Sub
        End If

        container.DocumentSeriesLocation = uscSeriesLocation.Location
        container.DocumentSeriesAnnexedLocation = uscSeriesAnnexedLocation.Location
        container.DocumentSeriesUnpublishedAnnexedLocation = uscSeriesUnpublishedAnnexedLocation.Location
        If Not String.IsNullOrEmpty(ddlArchive.SelectedValue) Then
            container.Archive = Facade.ContainerArchiveFacade.GetById(Integer.Parse(ddlArchive.SelectedValue))
        Else
            container.Archive = Nothing
        End If

        Try
            If container.DocumentSeriesLocation IsNot Nothing Then
                ' Crea o aggiorna la Serie
                Dim series As DocumentSeries = Facade.DocumentSeriesFacade.ActivateDocumentSeries(container)
                If Not String.IsNullOrEmpty(ddlDocumentSeriesFamily.SelectedValue) Then
                    series.Family = Facade.DocumentSeriesFamilyFacade.GetById(Integer.Parse(ddlDocumentSeriesFamily.SelectedValue))
                End If
                series.PublicationEnabled = chkSeriesPub.Checked
                series.SubsectionEnabled = chkSubsection.Checked
                series.AllowNoDocument = chkZeroDoc.Checked
                series.AllowAddDocument = chkAddDocs.Checked
                series.RoleEnabled = chkRoles.Checked
                Facade.DocumentSeriesFacade.UpdateNoLastChange(series)
            Else
                ' Disabilita la serie
                Facade.DocumentSeriesFacade.DeactivateDocumentSeries(container)
            End If
        Catch ex As Exception
            AjaxAlert("Errore in fase di recupero dati Serie da Form.")
            FileLogger.Error(LoggerName, "Errore in FillDocumentSeries.", ex)
        End Try
    End Sub

    Private Sub FillContainerBehaviours(container As Container)
        Try
            If Not ProtocolEnv.ContainerBehaviourEnabled Then
                Exit Sub
            End If

            ''Verifico se già esista un behaviour
            Dim behaviour As ContainerBehaviour = Facade.ContainerBehaviourFacade.GetBehaviours(container, ContainerBehaviourAction.Insert, "#uscClassificatori").FirstOrDefault()
            ''Lo elimino a prescindere
            If behaviour IsNot Nothing Then Facade.ContainerBehaviourFacade.Delete(behaviour)

            ''Se non sono presenti classificatori sulla maschera
            If uscClassificatori.HasSelectedCategories Then
                ''Creo un nuovo behaviour da sostituire all'eventuale già esistente
                Dim selectedCategory As Category = uscClassificatori.SelectedCategories.First()
                behaviour = New ContainerBehaviour() With
                {
                    .Container = CurrentContainer,
                    .KeepValue = True,
                    .AttributeName = "#uscClassificatori",
                    .Action = ContainerBehaviourAction.Insert,
                    .AttributeValue = selectedCategory.Id.ToString
                }
                Facade.ContainerBehaviourFacade.Save(behaviour)
            End If
        Catch ex As Exception
            AjaxAlert("Errore in fase di recupero dati Behaviours da Form.")
            FileLogger.Error(LoggerName, "Errore in FillBehaviours.", ex)
        End Try
    End Sub

    Private Function CheckContainerNameExist() As Boolean
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainerByName(txtName.Text)
        Return containers.Any()
    End Function

#End Region

End Class