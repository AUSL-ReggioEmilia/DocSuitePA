Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.MassimariScarto
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.MassimariScarto
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class uscFascicleInsert
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Dim _currentCategory As Category
    Private Const CATEGORY_CHANGE_HANDLER As String = "uscFascicleInsert.onCategoryChanged('{0}', '{1}', '{2}');"
    Private Const INITIALIZE_CALLBACK As String = "uscFascicleInsert.initializeCallback();"
    Private Const BIND_LOADED As String = "uscFascicleInsert.bindLoaded();"
    Private Const CATEGORY_NOT_FASCICOLABLE_WARNING As String = "uscFascicleInsert.printCategoryNotFascicolable();"
    Private Const FASCICLE_TYPE_SELECTED_CALLBACK As String = "uscFascicleInsert.fascicleTypeSelectedCallback();"
    Dim _categoryFascicleFacade As CategoryFascicleFacade
    Private _categoryFascicleRightFacade As CategoryFascicleRightFacade
    Private _currentMassimarioScartoFacade As MassimarioScartoFacade
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
                _currentMassimarioScartoFacade = New MassimarioScartoFacade(DocSuiteContext.Current.Tenants, BasePage.CurrentTenant)
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
        uscDynamicMetadataRest.ValidationEnabled = Not ValidationDisabled
        uscContattiResp.Caption = DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel
        uscContattiResp.RequiredErrorMessage = $"Inserire un {DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel}"
        If Not IsPostBack() Then
            If IsPeriodic Then
                rdlFascicleType.Items.Add(item:=New DropDownListItem With {.Text = "Fascicolo periodico", .Value = "2"})
            End If
            Initialize()
        End If
        RefreshControls(False)
    End Sub

    Private Sub uscClassificatore_CategoryChange(ByVal sender As Object, ByVal e As List(Of String)) Handles uscClassificatore.EntityAdded, uscClassificatore.EntityRemoved
        If uscClassificatore.SelectedCategories.Count = 0 Then
            uscContattiResp.SearchInCategoryContacts = Nothing
            uscContattiResp.CategoryContactsProcedureType = String.Empty
            uscContattiResp.UpdateButtons()
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, String.Empty, String.Empty, String.Empty))
            Exit Sub
        End If
        If uscContattiResp.DataSource.Any() Then
            Exit Sub
        End If
        Dim category As Category = Facade.CategoryFacade.GetById(uscClassificatore.SelectedCategories.First())
        Dim categoryHasSpecialRole As Boolean = False
        If Not String.IsNullOrEmpty(rdlFascicleType.SelectedValue) AndAlso rdlFascicleType.SelectedValue.Equals(DirectCast(FascicleType.Procedure, Integer).ToString()) Then
            categoryHasSpecialRole = CurrentCategoryFascicleRightFacade.HasSpecialRole(category.Id)
        End If
        Dim idMetadataRepository As String = String.Empty
        If category.IdMetadataRepository.HasValue Then
            idMetadataRepository = category.IdMetadataRepository.Value.ToString()
        End If
        Dim categoryFascicle As CategoryFascicle = CurrentCategoryFascicleFacade.GetProcedureFascicles(category.Id).FirstOrDefault()
        Dim customActions As String = String.Empty
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
            customActions = categoryFascicle.CustomActions
        End If
        Dim massimarioScarto As MassimarioScarto = GetMassimario(category.Id)
        If massimarioScarto IsNot Nothing Then
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, massimarioScarto.ConservationPeriod, idMetadataRepository, customActions))
            Exit Sub
        End If
        AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, String.Empty, idMetadataRepository, customActions))
    End Sub

    Protected Sub FascInserimento_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = TryParseJsonModel(Of AjaxModel)(e.Argument)
        If ajaxModel Is Nothing Then
            Exit Sub
        End If
        Select Case ajaxModel.ActionName
            Case "Initialize"
                Initialize()
                If Not String.IsNullOrEmpty(rdlFascicleType.SelectedItem.Value) Then
                    RefreshControls()
                End If
                AjaxManager.ResponseScripts.Add(INITIALIZE_CALLBACK)

            Case "FascicleTypeSelected"
                RefreshControls(True, If(ajaxModel.Value.Count > 0, ajaxModel.Value(0), Nothing))
                Dim roleRestriction As Integer
                If ajaxModel.Value.Count > 0 Then
                    Integer.TryParse(ajaxModel.Value(0), roleRestriction)
                End If
                If RespContactDTO Is Nothing Then
                    Dim contactFascicleId As Integer? = Nothing
                    If ProtocolEnv.FascicleContactId > 0 Then
                        contactFascicleId = ProtocolEnv.FascicleContactId
                    End If
                    Dim respUser As IList(Of Contact) = Facade.ContactFacade.GetContactByRole(DocSuiteContext.Current.User.FullUserName, True, parentId:=contactFascicleId, idRole:=roleRestriction)
                    If respUser IsNot Nothing AndAlso respUser.Count > 0 Then
                        uscContattiResp.DataSource = New List(Of ContactDTO) From {New ContactDTO(respUser.First(), ContactDTO.ContactType.Address)}
                        uscContattiResp.DataBind()
                    End If
                End If
                AjaxManager.ResponseScripts.Add(FASCICLE_TYPE_SELECTED_CALLBACK)
            Case "SetDefaultContact"
                Dim contactId As Integer
                If ajaxModel.Value.Count > 0 AndAlso Integer.TryParse(ajaxModel.Value(0), contactId) Then
                    Dim contact As Contact = Facade.ContactFacade.GetById(contactId)
                    If Not contact Is Nothing Then
                        Dim contacts As New List(Of ContactDTO)
                        contacts.Add(New ContactDTO With {.Contact = contact, .Type = ContactDTO.ContactType.Address})
                        uscContattiResp.DataSource = contacts
                        uscContattiResp.DataBind(contacts.Count)
                    End If
                End If
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascInserimento_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscClassificatore, uscContattiResp)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscContattiResp.TreeViewControl)
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


    Private Sub RefreshControls(Optional clearUscSession As Boolean = True, Optional roleMasterId As String = Nothing)
        If (Not ProtocolEnv.FascicleAuthorizedRoleCaption.IsNullOrEmpty()) Then
            uscRole.Caption = ProtocolEnv.FascicleAuthorizedRoleCaption.ToString()
        End If
        If (String.IsNullOrEmpty(rdlFascicleType.SelectedValue)) Then
            uscContattiResp.IsRequired = False
            Exit Sub
        End If
        uscContattiResp.RoleContactsProcedureType = Nothing
        uscContattiResp.IsRequired = Convert.ToInt32(rdlFascicleType.SelectedValue).Equals(FascicleType.Procedure)

        If Convert.ToInt32(rdlFascicleType.SelectedValue).Equals(FascicleType.Procedure) Then
            If roleMasterId IsNot Nothing Then
                Dim idRole As Integer
                Integer.TryParse(roleMasterId, idRole)
                AjaxManager.ResponseScripts.Add($"uscFascicleInsert.setCategoryRole({idRole})")
            End If
            uscContattiResp.RoleContactsProcedureType = RoleUserType.RP.ToString()
        End If


        If Convert.ToInt32(rdlFascicleType.SelectedValue).Equals(FascicleType.Period) Then
            radStartDate.ValidateRequestMode = ValidateRequestMode.Enabled
        End If

        AjaxManager.ResponseScripts.Add(FASCICLE_TYPE_SELECTED_CALLBACK)
    End Sub


    Private Function TryParseJsonModel(Of T)(json As String) As T
        Try
            Return JsonConvert.DeserializeObject(Of T)(json)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region

End Class