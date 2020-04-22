Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.MassimariScarto
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.MassimariScarto
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class uscFascicleInsert
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Dim _currentCategory As Category
    Private Const CATEGORY_CHANGE_HANDLER As String = "uscFascicleInsert.onCategoryChanged('{0}', '{1}');"
    Private Const INITIALIZE_CALLBACK As String = "uscFascicleInsert.initializeCallback();"
    Private Const CATEGORY_NOT_FASCICOLABLE_WARNING As String = "uscFascicleInsert.printCategoryNotFascicolable();"
    Private Const FASCICLE_TYPE_SELECTED_CALLBACK As String = "uscFascicleInsert.fascicleTypeSelectedCallback();"
    Dim _categoryFascicleFacade As CategoryFascicleFacade
    Private _categoryFascicleRightFacade As CategoryFascicleRightFacade
    Private _currentMassimarioScartoFacade As MassimarioScartoFacade
    Private _currentUDId As Guid?
    Private _environment As Integer?
    Private _currentIdUDSRepository As Guid?
    Private _contactFacade As ContactFacade
#End Region

#Region " Properties "

    Public Property IsPeriodic As Boolean

    Public Property PageContentDiv As RadPageLayout

    Public Property IdCategory As Integer?

    Public Property ValidationDisabled As Boolean
    Private ReadOnly Property CurrentCategoryFascicleRightFacade As CategoryFascicleRightFacade
        Get
            If _categoryFascicleRightFacade Is Nothing Then
                _categoryFascicleRightFacade = New CategoryFascicleRightFacade()
            End If
            Return _categoryFascicleRightFacade
        End Get
    End Property

    Private ReadOnly Property CurrentCategory As Category
        Get
            If _currentCategory Is Nothing Then
                _currentCategory = Facade.CategoryFacade.GetById(IdCategory.Value)
            End If
            Return _currentCategory
        End Get
    End Property

    Private ReadOnly Property CurrentCategoryFascicleFacade As CategoryFascicleFacade
        Get
            If _categoryFascicleFacade Is Nothing Then
                _categoryFascicleFacade = New CategoryFascicleFacade()
            End If
            Return _categoryFascicleFacade
        End Get
    End Property

    Private ReadOnly Property ContactFacade As ContactFacade
        Get
            If _contactFacade Is Nothing Then
                _contactFacade = New ContactFacade()
            End If
            Return _contactFacade
        End Get
    End Property

    Private ReadOnly Property CurrentMassimarioScartoFacade As MassimarioScartoFacade
        Get
            If _currentMassimarioScartoFacade Is Nothing Then
                _currentMassimarioScartoFacade = New MassimarioScartoFacade(DocSuiteContext.Current.Tenants)
            End If
            Return _currentMassimarioScartoFacade
        End Get
    End Property

    'TO DO: da togliere e mettere client-side quando lo user control dei contatti sarà client side 
    Public ReadOnly Property RespContactDTO As ContactDTO
        Get
            Return uscContattiResp.GetContacts(False).FirstOrDefault()
        End Get
    End Property

    Public ReadOnly Property FasciclesPanelVisibilities As String
        Get
            Return JsonConvert.SerializeObject(ProtocolEnv.FasciclesPanelVisibilities)
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        uscDynamicMetadata.ValidationDisabled = ValidationDisabled
        uscContattiResp.Caption = DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel
        uscContattiResp.RequiredErrorMessage = $"Inserire un {DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel}"
        If Not IsPostBack() Then
            If IsPeriodic Then
                rdlFascicleType.Items.Add(item:=New DropDownListItem With {.Text = "Fascicolo periodico", .Value = "2"})
                rdlFascicleType.Items(3).Selected = True
            End If
            Initialize()
        End If
        RefreshControls(False)
    End Sub

    Private Sub uscSettoreMaster_RoleChange(ByVal sender As Object, ByVal e As EventArgs) Handles uscSettoreMaster.RoleAdded, uscSettoreMaster.RoleRemoved
        If Not uscSettoreMaster.HasSelectedRole Then
            uscContattiResp.SearchInRoleContacts = Nothing
            uscContattiResp.RoleContactsProcedureType = Nothing
            uscContattiResp.UpdateButtons()
            Exit Sub
        End If

        If uscSettoreMaster.GetRoles().Any() Then
            uscContattiResp.SearchInRoleContacts = uscSettoreMaster.GetRoles().FirstOrDefault().Id
            uscContattiResp.CategoryContactsProcedureType = RoleUserType.RP.ToString()
            uscContattiResp.RoleContactsProcedureType = RoleUserType.RP.ToString()
        End If

        uscContattiResp.UpdateButtons()

        Dim idRole As Integer = Integer.Parse(uscSettoreMaster.TreeViewControl.GetAllNodes().First().Value)
        AjaxManager.ResponseScripts.Add($"uscFascicleInsert.setCategoryRole({idRole})")
    End Sub

    Private Sub uscSettoreMaster_RoleAdding(sender As Object, e As RoleEventArgs) Handles uscSettoreMaster.RoleAdding
        If uscSettori.HasSelectedRole Then
            e.Cancel = uscSettori.GetRoles().Any(Function(x) x.Id = e.Role.Id)
            If e.Cancel Then
                BasePage.AjaxAlert($"Non è possibile selezionare il settore {e.Role.Name} in quanto già presente come settore autorizzato del fascicolo")
            End If
        End If
    End Sub

    Private Sub uscSettori_RoleAdding(sender As Object, e As RoleEventArgs) Handles uscSettori.RoleAdding
        If uscSettoreMaster.HasSelectedRole Then
            e.Cancel = uscSettoreMaster.GetRoles().Any(Function(x) x.Id = e.Role.Id)
            If e.Cancel Then
                BasePage.AjaxAlert($"Non è possibile selezionare il settore {e.Role.Name} in quanto già presente come settore responsabile del fascicolo")
            End If
        End If
    End Sub

    Private Sub uscClassificatore_CategoryChange(ByVal sender As Object, ByVal e As EventArgs) Handles uscClassificatore.CategoryAdded, uscClassificatore.CategoryRemoved
        If uscClassificatore.SelectedCategories.Count = 0 Then
            uscSettoreMaster.IdCategorySelected = Nothing
            uscContattiResp.SearchInCategoryContacts = Nothing
            uscContattiResp.CategoryContactsProcedureType = String.Empty
            uscContattiResp.UpdateButtons()
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, String.Empty, String.Empty))
            Exit Sub
        End If
        Dim category As Category = Facade.CategoryFacade.GetById(uscClassificatore.SelectedCategories.First())
        Dim categoryHasSpecialRole As Boolean = False
        If Not String.IsNullOrEmpty(rdlFascicleType.SelectedValue) AndAlso rdlFascicleType.SelectedValue.Equals(DirectCast(FascicleType.Procedure, Integer).ToString()) Then
            categoryHasSpecialRole = CurrentCategoryFascicleRightFacade.HasSpecialRole(category.Id)
            If Not categoryHasSpecialRole Then
                uscSettoreMaster.IdCategorySelected = category.Id
            End If
        End If
        Dim idMetadataRepository As String = String.Empty
        If category.IdMetadataRepository.HasValue Then
            idMetadataRepository = category.IdMetadataRepository.Value.ToString()
        End If
        Dim categoryFascicle As CategoryFascicle = CurrentCategoryFascicleFacade.GetProcedureFascicles(category.Id).FirstOrDefault()
        If categoryFascicle IsNot Nothing Then
            If categoryFascicle.Manager IsNot Nothing Then
                uscContattiResp.DataSource = New List(Of ContactDTO) From {New ContactDTO(categoryFascicle.Manager, ContactDTO.ContactType.Address)}
                uscContattiResp.DataBind()
            End If
            If Not categoryHasSpecialRole AndAlso CurrentCategoryFascicleRightFacade.HasRoles(category.Id) Then
                uscContattiResp.SearchInCategoryContacts = category.Id
            End If
            uscContattiResp.CategoryContactsProcedureType = RoleUserType.RP.ToString()
            uscContattiResp.UpdateButtons()
        End If
        Dim massimarioScarto As MassimarioScarto = GetMassimario(category.Id)
        If massimarioScarto IsNot Nothing Then
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, massimarioScarto.ConservationPeriod, idMetadataRepository))
            Exit Sub
        End If
        AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, String.Empty, idMetadataRepository))
    End Sub

    Protected Sub FascInserimento_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "Initialize"
                Initialize()
                If Not String.IsNullOrEmpty(rdlFascicleType.SelectedItem.Value) Then
                    RefreshControls()
                End If
                AjaxManager.ResponseScripts.Add(INITIALIZE_CALLBACK)

            Case "FascicleTypeSelected"
                RefreshControls()
                uscSettori.RemoveAllRoles()
                uscSettoreMaster.DataBind()

                Dim roleRestriction As Integer? = Nothing
                If uscSettoreMaster.HasSelectedRole Then
                    roleRestriction = uscSettoreMaster.TreeViewControl.GetAllNodes.First().Value
                End If
                If RespContactDTO Is Nothing Then
                    Dim contactFascicleId As Integer? = Nothing
                    If ProtocolEnv.FascicleContactId > 0 Then
                        contactFascicleId = ProtocolEnv.FascicleContactId
                    End If
                    Dim respUser As IList(Of Contact) = Facade.ContactFacade.GetContactByRole(DocSuiteContext.Current.User.FullUserName, 1, parentId:=contactFascicleId, idRole:=roleRestriction)
                    If respUser IsNot Nothing AndAlso respUser.Count > 0 Then
                        uscContattiResp.DataSource = New List(Of ContactDTO) From {New ContactDTO(respUser.First(), ContactDTO.ContactType.Address)}
                        uscContattiResp.DataBind()
                    End If
                End If
                AjaxManager.ResponseScripts.Add(FASCICLE_TYPE_SELECTED_CALLBACK)

        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascInserimento_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscClassificatore, uscContattiResp)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscSettori)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscSettoreMaster)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettoreMaster, uscSettoreMaster)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscContattiResp)
        AjaxManager.AjaxSettings.AddAjaxSetting(rdlFascicleType, uscSettori)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscClassificatore, uscSettoreMaster)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscClassificatore, uscMetadataRepositorySel)
    End Sub

    Private Sub Initialize()
        If IdCategory.HasValue Then
            If Not CurrentCategoryFascicleFacade.ExistFascicleProcedure(IdCategory.Value) Then
                AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_NOT_FASCICOLABLE_WARNING, JsonConvert.SerializeObject(True)))
            End If
            uscClassificatore.IdCategory = IdCategory
        End If
    End Sub

    Private Function GetMassimario(idCategory As Integer) As MassimarioScarto
        Dim category As Category = Facade.CategoryFacade.GetById(idCategory)
        If Not category.IdMassimarioScarto.HasValue Then
            Return Nothing
        End If

        Dim massimario As DTO.WebAPI.WebAPIDto(Of MassimarioScarto) = CurrentMassimarioScartoFacade.GetById(category.IdMassimarioScarto.Value)
        Return massimario.Entity
    End Function


    Private Sub RefreshControls(Optional clearUscSession As Boolean = True)
        If (Not ProtocolEnv.FascicleAuthorizedRoleCaption.IsNullOrEmpty()) Then
            uscSettori.Caption = ProtocolEnv.FascicleAuthorizedRoleCaption.ToString()
        End If
        If (String.IsNullOrEmpty(rdlFascicleType.SelectedValue)) Then
            uscContattiResp.IsRequired = False
            Exit Sub
        End If
        uscContattiResp.RoleContactsProcedureType = Nothing
        uscSettori.Required = Convert.ToInt32(rdlFascicleType.SelectedValue).Equals(FascicleType.Activity)
        uscContattiResp.IsRequired = Convert.ToInt32(rdlFascicleType.SelectedValue).Equals(FascicleType.Procedure)
        If Convert.ToInt32(rdlFascicleType.SelectedValue).Equals(FascicleType.Activity) Then
            uscSettori.MultiSelect = False
            uscSettori.MultipleRoles = False
            uscSettori.RoleRestictions = RoleRestrictions.OnlyMine
            uscSettori.Environment = DSWEnvironment.Any
            uscSettori.AnyRightInAnyEnvironment = True
            uscSettori.FascicleVisibilityTypeEnabled = False
        End If

        If Convert.ToInt32(rdlFascicleType.SelectedValue).Equals(FascicleType.Procedure) Then
            uscSettori.MultiSelect = True
            uscSettori.MultipleRoles = True
            uscSettori.RoleRestictions = RoleRestrictions.None
            uscSettori.AnyRightInAnyEnvironment = False
            uscSettori.FascicleVisibilityTypeEnabled = True
            uscSettoreMaster.MultiSelect = False
            uscSettoreMaster.MultipleRoles = False
            uscSettoreMaster.RoleRestictions = RoleRestrictions.OnlyMine
            If uscSettoreMaster.HasSelectedRole Then
                Dim idRole As Integer = Integer.Parse(uscSettoreMaster.TreeViewControl.GetAllNodes().First().Value)
                AjaxManager.ResponseScripts.Add($"uscFascicleInsert.setCategoryRole({idRole})")
            End If
            If uscClassificatore.SelectedCategories.Count > 0 Then
                Dim categoryId As Integer = uscClassificatore.SelectedCategories.First()
                If Not CurrentCategoryFascicleRightFacade.HasSpecialRole(categoryId) Then
                    uscSettoreMaster.IdCategorySelected = categoryId
                End If
            End If
            uscContattiResp.RoleContactsProcedureType = RoleUserType.RP.ToString()
        End If


        If Convert.ToInt32(rdlFascicleType.SelectedValue).Equals(FascicleType.Period) Then
            uscSettoreMaster.Required = Not ProtocolEnv.FascicleContainerEnabled
            uscSettori.MultiSelect = True
            uscSettori.MultipleRoles = True
            uscSettori.RoleRestictions = RoleRestrictions.None
            uscSettori.AnyRightInAnyEnvironment = False
            uscSettoreMaster.MultiSelect = False
            uscSettoreMaster.MultipleRoles = False
            uscSettoreMaster.RoleRestictions = RoleRestrictions.OnlyMine
            radStartDate.ValidateRequestMode = ValidateRequestMode.Enabled
        End If
        uscSettori.Initialize(clearUscSession)
        uscSettoreMaster.Initialize(clearUscSession)
        'in questo modo non si perdono i settori selezionati nel settore master
        uscSettoreMaster.GetRoles()

    End Sub

    Public Function GetDynamicValues() As MetadataModel
        Return uscDynamicMetadata.GetControlValues()
    End Function
#End Region

End Class