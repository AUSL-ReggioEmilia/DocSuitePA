Imports System.Collections.Generic
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Validators
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Validators.Categories
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging


Partial Public Class TbltClassificatoreGesNew
    Inherits CommonBasePage


#Region " Fields "

    Private _idCategory As Short?
    Private _category As Category
    Private _currentCategoryFascicleFacade As CategoryFascicleFacade
    Private Const ACTION_ADD As String = "Add"
    Private Const ACTION_EDIT As String = "Rename"
    Private Const ACTION_DELETE As String = "Delete"
    Private Const ACTION_RECOVERY As String = "Recovery"
    Private Const TITLE_ACTION_ADD As String = "Classificatori - Aggiungi"
    Private Const TITLE_ACTION_EDIT As String = "Classificatori - Modifica"
    Private Const TITLE_ACTION_DELETE As String = "Classificatori - Elimina"
    Private Const TITLE_ACTION_RECOVERY As String = "Classificatori - Recupera"
    Private Const CLOSE_WINDOW_SCRIPT As String = "CloseWindow('{0}');"

#End Region

#Region " Properties "

    Private ReadOnly Property IdCategory As Short
        Get
            If Not _idCategory.HasValue Then
                Dim queryString As String = Request.QueryString.GetValueOrDefault(Of String)("CategoryID", String.Empty)
                Dim parsed As Short = 0
                Short.TryParse(queryString, parsed)
                _idCategory = parsed
            End If
            Return _idCategory.Value
        End Get
    End Property

    Private Property CurrentCategory As Category
        Get
            If _category Is Nothing Then
                _category = New Category()
                If Not IdCategory = 0 Then
                    _category = Facade.CategoryFacade.GetById(IdCategory)
                End If
            End If
            Return _category
        End Get
        Set(value As Category)
            _category = value
        End Set
    End Property

    Private ReadOnly Property CurrentCategoryFascicleFacade As CategoryFascicleFacade
        Get
            If _currentCategoryFascicleFacade Is Nothing Then
                _currentCategoryFascicleFacade = New CategoryFascicleFacade()
            End If
            Return _currentCategoryFascicleFacade
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

    Protected Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim deleted As Boolean = False

        Try
            Select Case Action
                Case "Add"
                    If Not AddCategory() Then
                        Exit Sub
                    End If

                Case "Rename"
                    If Not EditCategory() Then
                        Exit Sub
                    End If

                Case "Delete"
                    If Not DeleteCategory() Then
                        Exit Sub
                    End If

                Case "Recovery"
                    If Not RecoveryCategory() Then
                        Exit Sub
                    End If
            End Select

            AjaxManager.ResponseScripts.Add(String.Format(CLOSE_WINDOW_SCRIPT, Action))
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            Dim jsScript As String = "alert('{0}');"
            jsScript = String.Format(jsScript, ex.Message)

            AjaxManager.ResponseScripts.Add(jsScript)
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub InitializePage()
        MasterDocSuite.TitleVisible = False
        InitializeTreeView()

        Select Case Action
            Case ACTION_ADD
                InitializeAddAction()

            Case ACTION_EDIT
                InitializeEditAction()

            Case ACTION_DELETE
                InitializeDeleteAction()

            Case ACTION_RECOVERY
                InitializeRecoveryAction()
        End Select
    End Sub

    Private Sub InitializeAddAction()
        Title = TITLE_ACTION_ADD
        rdpStartDate.MinDate = CurrentCategory.StartDate.Date
        pnlRinomina.Visible = False
        pnlInserimento.Visible = True
        pnlStartDate.Visible = True
        txtCode.Focus()
    End Sub

    Private Sub InitializeEditAction()
        Title = TITLE_ACTION_EDIT
        pnlRinomina.Visible = True
        pnlInserimento.Visible = False
        txtOldName.Text = CurrentCategory.Name
        txtOldName.ReadOnly = True
        txtOldCode.Text = CurrentCategory.Code.ToString()
        txtOldCode.ReadOnly = True
        txtNewName.Text = txtOldName.Text
        txtNewCode.Text = txtOldCode.Text
        pnlNewStartDate.Visible = True
        rdpOldStartDate.Enabled = False
        rdpOldStartDate.SelectedDate = CurrentCategory.StartDate.Date
        rdpNewStartDate.SelectedDate = CurrentCategory.StartDate.Date
        txtNewCode.Focus()
    End Sub

    Private Sub InitializeDeleteAction()
        Title = TITLE_ACTION_DELETE
        pnlRinomina.Visible = False
        pnlInserimento.Visible = True
        pnlStartDate.Visible = True
        lblStartDate.Text = "Data di Disattivazione:"
        rdpStartDate.MinDate = CurrentCategory.StartDate.Date
        If CurrentCategory.EndDate.HasValue Then
            rdpStartDate.SelectedDate = CurrentCategory.EndDate.Value.Date
        End If
        txtName.Text = CurrentCategory.Name
        txtName.ReadOnly = True
        txtCode.Text = CurrentCategory.Code.ToString()
        txtCode.ReadOnly = True
        rfvStartDate1.ErrorMessage = "Campo Data di Disattivazione Obbligatorio"
    End Sub

    Private Sub InitializeRecoveryAction()
        Title = TITLE_ACTION_RECOVERY
        pnlRinomina.Visible = False
        pnlInserimento.Visible = True
        pnlStartDate.Visible = False
        txtName.Text = CurrentCategory.Name
        txtName.ReadOnly = True
        txtCode.Text = CurrentCategory.Code.ToString()
        txtCode.ReadOnly = True
    End Sub

    Private Sub InitializeTreeView()
        If Not String.IsNullOrEmpty(CurrentCategory.Name) Then
            Dim node As New RadTreeNode()
            If CurrentCategory.Parent Is Nothing Then
                node.ImageUrl = "../Comm/images/FolderOpen16.gif"
            Else
                node.ImageUrl = "../Comm/images/Classificatore.gif"
            End If
            RadTreeViewSelectedCategory.Nodes.Add(New RadTreeNode(CurrentCategory.GetFullName()))
        Else
            RadTreeViewSelectedCategory.Nodes.Add(New RadTreeNode(".Classificatore"))
        End If
    End Sub

    Private Function GetCategoryCode(source As String) As Short
        Dim parsed As Short = 0
        If Short.TryParse(source, parsed) Then
            Return parsed
        End If
        Throw New InvalidCastException("Codice mancante o non valido.")
    End Function

    Private Function GetCategoryConservationYear(source As String) As Short
        Dim parsed As Short = 0
        If Short.TryParse(source, parsed) Then
            Return parsed
        End If
        Throw New InvalidCastException("Conservazione Anni mancante o non valido.")
    End Function

    Private Function AddCategory() As Boolean
        Dim currentCategorySchema As CategorySchema = Facade.CategorySchemaFacade.GetActiveCategorySchema(New DateTimeOffset(rdpStartDate.SelectedDate.Value, DateTimeOffset.UtcNow.Offset))
        Dim category As New Category()
        With category
            .Name = txtName.Text
            .IsActive = 1
            .Code = GetCategoryCode(txtCode.Text)
            .StartDate = (New DateTimeOffset(rdpStartDate.SelectedDate.Value, DateTimeOffset.UtcNow.Offset))
            .CategorySchema = currentCategorySchema
            If CurrentCategory.Id <> 0 Then
                .Parent = CurrentCategory
            End If
        End With

        Dim validator As CategoryValidator = New CategoryValidator(category, CategoryRuleset.Insert)
        Dim results As ValidatorResult = validator.Validate()
        If results.HasErrors Then
            AjaxAlert(String.Join(Environment.NewLine, results.Errors))
            Return False
        End If

        Facade.CategoryFacade.Save(category)
        CurrentCategory = category
        Return True
    End Function

    Private Function EditCategory() As Boolean
        Dim currentCategorySchema As CategorySchema = Facade.CategorySchemaFacade.GetActiveCategorySchema(New DateTimeOffset(rdpNewStartDate.SelectedDate.Value, DateTimeOffset.UtcNow.Offset))
        Dim category As New Category()
        With category
            .Id = CurrentCategory.Id
            .Name = txtNewName.Text
            .Code = GetCategoryCode(txtNewCode.Text)
            .StartDate = (New DateTimeOffset(rdpNewStartDate.SelectedDate.Value, DateTimeOffset.UtcNow.Offset))
            .CategorySchema = currentCategorySchema
            If CurrentCategory.Parent IsNot Nothing Then
                .Parent = CurrentCategory.Parent
            End If
        End With

        Dim validator As CategoryValidator = New CategoryValidator(category, CategoryRuleset.Update)
        Dim results As ValidatorResult = validator.Validate()
        If results.HasErrors Then
            AjaxAlert(String.Join(Environment.NewLine, results.Errors))
            Return False
        End If

        With CurrentCategory
            .Name = category.Name
            .Code = category.Code
            .StartDate = category.StartDate
        End With

        Facade.CategoryFacade.Update(CurrentCategory)
        Return True
    End Function

    Private Function DeleteCategory() As Boolean
        With CurrentCategory
            .EndDate = rdpStartDate.SelectedDate
            .IsActive = 0
        End With

        Dim validator As CategoryValidator = New CategoryValidator(CurrentCategory, CategoryRuleset.Delete)
        Dim results As ValidatorResult = validator.Validate()
        If results.HasErrors Then
            AjaxAlert(String.Join(Environment.NewLine, results.Errors))
            Return False
        End If

        Dim documentUnitFinder As DocumentUnitModelFinder = New DocumentUnitModelFinder(DocSuiteContext.Current.CurrentTenant)
        documentUnitFinder.DocumentUnitFinderAction = DocumentUnitFinderActionType.CategorizedUD
        documentUnitFinder.CategoryId = CurrentCategory.UniqueId

        If Not CurrentCategoryFascicleFacade.ExistFascicleDefinition(CurrentCategory.Id) _
            AndAlso (Not Facade.CategoryFacade.IsUsed(CurrentCategory) AndAlso documentUnitFinder.DoSearch().Count = 0) Then
            Dim categoryFascicles As ICollection(Of CategoryFascicle) = CurrentCategoryFascicleFacade.GetByIdCategory(CurrentCategory.Id)
            For Each item As CategoryFascicle In categoryFascicles
                CurrentCategoryFascicleFacade.Delete(item)
            Next
            Facade.CategoryFacade.Delete(CurrentCategory)
            Return True
        End If

        Facade.CategoryFacade.UpdateOnly(CurrentCategory)
        Return True
    End Function

    Private Function RecoveryCategory() As Boolean
        Dim validator As CategoryValidator = New CategoryValidator(CurrentCategory, CategoryRuleset.Recover)
        Dim results As ValidatorResult = validator.Validate()
        If results.HasErrors Then
            AjaxAlert(String.Join(Environment.NewLine, results.Errors))
            Return False
        End If

        With CurrentCategory
            .EndDate = Nothing
            .IsActive = 1
        End With

        Facade.CategoryFacade.UpdateOnly(CurrentCategory)
        Return True
    End Function
#End Region

End Class