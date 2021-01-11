Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UscFascInsertUD
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const CATEGORY_CHANGE_HANDLER As String = "uscFascInsertUD.onCategoryChanged({0});"
    Private _location As Boolean?
#End Region

#Region " Properties "

    Protected ReadOnly Property FascMiscellaneaLocationEnabled As Boolean
        Get
            If Not _location.HasValue Then
                _location = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation) IsNot Nothing
            End If
            Return _location.Value
        End Get
    End Property

    Protected ReadOnly Property IdFascicleFolder As Guid?
        Get
            Return GetKeyValueOrDefault(Of Guid?)("IdFascicleFolder", Nothing)
        End Get
    End Property

    Protected ReadOnly Property IdFascicle As Guid?
        Get
            Return GetKeyValueOrDefault(Of Guid?)("IdFascicle", Nothing)
        End Get
    End Property

    Protected ReadOnly Property IdCategory As Integer?
        Get
            Return GetKeyValueOrDefault(Of Integer?)("IdCategory", Nothing)
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            grdUdDocSelected.DataSource = New List(Of String)
            rcbUDDoc.Items.AddRange(BasePage.FillComboBoxDocumentUnitNames(True))
            If IdCategory.HasValue Then
                Dim currentCategory As Category = Facade.CategoryFacade.GetById(IdCategory.Value)
                uscCategoryFasc.DataSource = New List(Of Category) From {currentCategory}
                uscCategoryFasc.DataBind()
            End If
        End If

    End Sub

    Private Sub uscClassificatore_CategoryChange(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategoryFasc.CategoryAdded, uscCategoryFasc.CategoryRemoved
        If Not uscCategoryFasc.HasSelectedCategories Then
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, 0))
            Exit Sub
        End If

        Dim category As Category = uscCategoryFasc.SelectedCategories.First()

        If category Is Nothing Then
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, 0))
            Exit Sub
        End If

        AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, category.Id))
    End Sub
#End Region

#Region " Methods "

    Public Overloads Function GetKeyValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function

    Private Sub InitializeAjaxSettings()

    End Sub

#End Region

End Class

