Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging


Partial Public Class TbltWorkflowRepositoryGes
    Inherits CommonBasePage


#Region " Fields "
    Private _idWorkflowRoleMapping As Guid?
    Private Const CLOSE_WINDOW_SCRIPT As String = "tbltWorkflowRepositoryGes.closeWindow('{0}');"
    Private _currentWorkflowRoleMappingFinder As WorkflowRoleMappingFinder
    Private _currentWorkflowRoleMapping As WorkflowRoleMapping
    Private _currentWorkflowRoleMappingFacade As WorkflowRoleMappingFacade
#End Region

#Region " Properties "
    Private ReadOnly Property IdWorkflowRoleMapping As Guid?
        Get
            If Not _idWorkflowRoleMapping.HasValue Then
                _idWorkflowRoleMapping = Request.QueryString.GetValueOrDefault(Of Guid?)("IdWorkflowRoleMapping", Nothing)
            End If
            Return _idWorkflowRoleMapping
        End Get
    End Property

    Private ReadOnly Property IdWorkflowRepository As Guid
        Get
            Return Request.QueryString.GetValue(Of Guid)("IdWorkflowRepository")
        End Get
    End Property

    Private ReadOnly Property MappingTag As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("MappingTag", String.Empty)
        End Get
    End Property

    Private ReadOnly Property FromXamlActivity As Boolean?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean?)("FromXamlActivity", Nothing)
        End Get
    End Property

    Private ReadOnly Property XamlInternalActivity As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("XamlInternalActivity", String.Empty)
        End Get
    End Property

    Private ReadOnly Property CurrentWorkflowRoleMappingFinder As WorkflowRoleMappingFinder
        Get
            If _currentWorkflowRoleMappingFinder Is Nothing Then
                _currentWorkflowRoleMappingFinder = New WorkflowRoleMappingFinder(DocSuiteContext.Current.Tenants)
            End If
            Return _currentWorkflowRoleMappingFinder
        End Get
    End Property

    Private ReadOnly Property CurrentWorkflowRoleMappingFacade As WorkflowRoleMappingFacade
        Get
            If _currentWorkflowRoleMappingFacade Is Nothing Then
                _currentWorkflowRoleMappingFacade = New WorkflowRoleMappingFacade(DocSuiteContext.Current.Tenants)
            End If
            Return _currentWorkflowRoleMappingFacade
        End Get
    End Property

    Private ReadOnly Property CurrentWorkflowRoleMapping As WorkflowRoleMapping
        Get
            If _currentWorkflowRoleMapping Is Nothing AndAlso IdWorkflowRoleMapping.HasValue Then
                CurrentWorkflowRoleMappingFinder.ResetDecoration()
                CurrentWorkflowRoleMappingFinder.EnablePaging = False
                CurrentWorkflowRoleMappingFinder.UniqueId = IdWorkflowRoleMapping.Value
                CurrentWorkflowRoleMappingFinder.ExpandRole = True
                _currentWorkflowRoleMapping = CurrentWorkflowRoleMappingFinder.DoSearch().FirstOrDefault().Entity
            End If
            Return _currentWorkflowRoleMapping
        End Get
    End Property

    Private ReadOnly Property IsRoleVisibile As Boolean
        Get
            Return Not pnlSettoriNew.Style("Display").Eq("none")
        End Get
    End Property

    Private ReadOnly Property IsContactVisibile As Boolean
        Get
            Return Not pnlNewContact.Style("Display").Eq("none")
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            InitializePage()
        End If
    End Sub

    Protected Sub TbltWorkflowRepositoryGes_AjaxRequest(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        Dim arr As String() = e.Argument.Split({"|"c}, StringSplitOptions.None)

        If arr(0).Eq("SaveMapping") Then
            Try
                Select Case Action
                    Case "Add"
                        Dim newWorkflowRoleMapping As WorkflowRoleMapping = New WorkflowRoleMapping()
                        newWorkflowRoleMapping.WorkflowRepository = New WorkflowRepository(IdWorkflowRepository)
                        newWorkflowRoleMapping.MappingTag = txtNewMappingTag.Text
                        If FromXamlActivity.HasValue AndAlso FromXamlActivity.Value Then
                            newWorkflowRoleMapping.IdInternalActivity = XamlInternalActivity
                        End If
                        newWorkflowRoleMapping.AuthorizationType = DirectCast(Short.Parse(ddlNewAuthorizationType.SelectedValue), WorkflowAuthorizationType)
                        If (IsRoleVisibile AndAlso Not uscSettoriNew.RoleListAdded.IsNullOrEmpty()) Then
                            newWorkflowRoleMapping.Role = New Entity.Commons.Role() With {.EntityShortId = Convert.ToInt16(uscSettoriNew.RoleListAdded.First())}
                        End If

                        If IsContactVisibile AndAlso uscContattiNew.GetManualContacts().Count > 0 Then
                            newWorkflowRoleMapping.AccountName = uscContattiNew.GetManualContacts().First().Contact.Code
                        End If
                        CurrentWorkflowRoleMappingFacade.Save(newWorkflowRoleMapping)

                    Case "Edit"
                        CurrentWorkflowRoleMapping.MappingTag = txtNewMappingTag.Text
                        CurrentWorkflowRoleMapping.AuthorizationType = DirectCast(Short.Parse(ddlNewAuthorizationType.SelectedValue), WorkflowAuthorizationType)
                        If (IsRoleVisibile AndAlso Not uscSettoriNew.RoleListAdded.IsNullOrEmpty()) Then
                            CurrentWorkflowRoleMapping.Role = New Entity.Commons.Role() With {.EntityShortId = Convert.ToInt16(uscSettoriNew.RoleListAdded.First())}
                        Else
                            CurrentWorkflowRoleMapping.Role = Nothing
                        End If
                        If IsContactVisibile AndAlso uscContattiNew.GetManualContacts().Count > 0 Then
                            CurrentWorkflowRoleMapping.AccountName = uscContattiNew.GetManualContacts().First().Contact.Code
                        Else
                            CurrentWorkflowRoleMapping.AccountName = String.Empty
                        End If
                        CurrentWorkflowRoleMappingFacade.Update(CurrentWorkflowRoleMapping)
                End Select

                AjaxManager.ResponseScripts.Add(String.Format(CLOSE_WINDOW_SCRIPT, Action))
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
                AjaxAlert(ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DdlNewAuthorizationType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim selectedAuth As WorkflowAuthorizationType = DirectCast(Short.Parse(ddlNewAuthorizationType.SelectedValue), WorkflowAuthorizationType)
        Select Case selectedAuth
            Case WorkflowAuthorizationType.ADGroup,
                 WorkflowAuthorizationType.MappingTags,
                 WorkflowAuthorizationType.UserName
                pnlSettoriNew.SetDisplay(False)
                uscSettoriNew.Required = False
                lblNewRole.Visible = False
            Case Else
                pnlSettoriNew.SetDisplay(True)
                uscSettoriNew.Required = True
                lblNewRole.Visible = True
        End Select

        pnlNewContact.SetDisplay(selectedAuth.Equals(WorkflowAuthorizationType.UserName))
        uscContattiNew.IsRequired = selectedAuth.Equals(WorkflowAuthorizationType.UserName)
        lblNewContact.Visible = selectedAuth.Equals(WorkflowAuthorizationType.UserName)
    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltWorkflowRepositoryGes_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlNewAuthorizationType, pnlSettoriNew)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlNewAuthorizationType, lblNewRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlNewAuthorizationType, pnlNewContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlNewAuthorizationType, lblNewContact)
    End Sub

    Private Sub InitializePage()
        MasterDocSuite.TitleVisible = False
        Dim authorizationTypes As ListItem() = [Enum].GetValues(GetType(WorkflowAuthorizationType)) _
                                                     .OfType(Of WorkflowAuthorizationType)() _
                                                     .Select(Function(s) New ListItem(EnumHelper.GetDescription(s), DirectCast(s, Short).ToString())) _
                                                     .OrderBy(Function(o) o.Text) _
                                                     .ToArray()

        Select Case Action
            Case "Add"
                If FromXamlActivity.HasValue AndAlso FromXamlActivity.Value Then
                    txtNewMappingTag.Text = MappingTag
                    txtNewMappingTag.ReadOnly = True
                End If
                ddlNewAuthorizationType.Items.AddRange(authorizationTypes)
                ddlNewAuthorizationType.SelectedValue = WorkflowAuthorizationType.AllManager.ToString()

            Case "Edit"
                txtOldMappingTag.Text = CurrentWorkflowRoleMapping.MappingTag
                txtOldMappingTag.ReadOnly = True

                If Not String.IsNullOrEmpty(CurrentWorkflowRoleMapping.AccountName) Then
                    Dim dto As ContactDTO = New ContactDTO(New Contact() With {.Code = CurrentWorkflowRoleMapping.AccountName, .ContactType = New ContactType(ContactType.Aoo)}, ContactDTO.ContactType.Manual)
                    Dim user As AccountModel = CommonAD.GetAccount(dto.Contact.Code)
                    dto.Contact.Description = user.DisplayName
                    uscOldContatti.DataSource = New List(Of ContactDTO) From {dto}
                    uscOldContatti.DataBind()

                    uscContattiNew.DataSource = New List(Of ContactDTO) From {dto}
                    uscContattiNew.DataBind()
                End If

                txtNewMappingTag.Text = txtOldMappingTag.Text
                If FromXamlActivity.HasValue AndAlso FromXamlActivity.Value Then
                    txtNewMappingTag.ReadOnly = True
                End If

                ddlNewAuthorizationType.Items.AddRange(authorizationTypes)
                ddlNewAuthorizationType.SelectedValue = Convert.ToInt32(CurrentWorkflowRoleMapping.AuthorizationType).ToString()

                ddlOldAuthorizationType.Items.Add(New ListItem(EnumHelper.GetDescription(CurrentWorkflowRoleMapping.AuthorizationType), Convert.ToInt32(CurrentWorkflowRoleMapping.AuthorizationType).ToString()))
                ddlOldAuthorizationType.SelectedValue = Convert.ToInt32(CurrentWorkflowRoleMapping.AuthorizationType).ToString()
                ddlOldAuthorizationType.Enabled = False

                If CurrentWorkflowRoleMapping.Role IsNot Nothing Then
                    txtOldRole.Text = CurrentWorkflowRoleMapping.Role.Name.ToString()
                    Dim oldRole As Role = Facade.RoleFacade.GetById(CurrentWorkflowRoleMapping.Role.EntityShortId)
                    uscSettoriNew.SourceRoles = New List(Of Role) From {oldRole}
                    uscSettoriNew.DataBind()
                End If
                txtOldRole.ReadOnly = True
        End Select
        DdlNewAuthorizationType_SelectedIndexChanged(ddlNewAuthorizationType, New EventArgs())
    End Sub

#End Region

End Class