Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Gui
Imports VecompSoftware.Helpers.UDS

Public Class uscUDSStaticDataInsert
    Inherits DocSuite2008BaseControl
    Implements IUDSInsertStaticData

#Region "Fields"
    Public Const ACTION_TYPE_INSERT As String = "Insert"
    Public Const ACTION_TYPE_EDIT As String = "Edit"
    Public Const ACTION_TYPE_VIEW As String = "View"
    Public Const ACTION_TYPE_DUPLICATE As String = "Duplicate"
#End Region

#Region "Properties"

    Public ReadOnly Property Subject As String Implements IUDSInsertStaticData.Subject
        Get
            Return txtSubject.Text
        End Get
    End Property

    Public ReadOnly Property IdCategory As Integer? Implements IUDSInsertStaticData.IdCategory
        Get
            If uscSelCategory.SelectedCategories.Any() Then
                Return uscSelCategory.SelectedCategories.First().Id
            End If
            Return Nothing
        End Get
    End Property

    Public Property ActionType As String Implements IUDSInsertStaticData.ActionType
        Get
            If ViewState(String.Format("{0}_ActionType", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState(String.Format("{0}_ActionType", ID)), String)
        End Get
        Set(value As String)
            ViewState(String.Format("{0}_ActionType", ID)) = value
        End Set
    End Property

    Public Property RegistrationDate As DateTimeOffset? Implements IUDSInsertStaticData.RegistrationDate
        Get
            If ViewState(String.Format("{0}_RegistrationDate", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState(String.Format("{0}_RegistrationDate", ID)), DateTimeOffset)
        End Get
        Set(value As DateTimeOffset?)
            ViewState(String.Format("{0}_RegistrationDate", ID)) = value
        End Set
    End Property

#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub uscSelCategory_CategoryAdding(sender As Object, args As EventArgs) Handles uscSelCategory.CategoryAdding
        If RegistrationDate.HasValue Then
            uscSelCategory.FromDate = RegistrationDate.Value.Date
        End If
    End Sub
#End Region

#Region "Methods"
    Public Sub Initialize()

    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscSelCategory)
    End Sub

    Public Sub ResetControls() Implements IUDSInsertStaticData.ResetControls
        txtSubject.Visible = False
        trSubject.Visible = False
    End Sub

    Public Sub InitializeControls() Implements IUDSInsertStaticData.InitializeControls
        Select Case ActionType
            Case ACTION_TYPE_INSERT, ACTION_TYPE_EDIT, ACTION_TYPE_DUPLICATE
                txtSubject.Visible = True
                trSubject.Visible = True
                Exit Select
            Case ACTION_TYPE_VIEW
                rfvSubject.Enabled = False
                rowCategory.Visible = False
                uscSelCategory.ReadOnly = True
                Exit Select
        End Select
    End Sub

    Public Sub SetData(model As UDSModel) Implements IUDSInsertStaticData.SetData
        If txtSubject.Visible Then
            txtSubject.Text = If(String.IsNullOrEmpty(model.Model.Subject.Value), model.Model.Subject.DefaultValue, model.Model.Subject.Value)

            If (ActionType.Equals(ACTION_TYPE_INSERT) AndAlso (Not model.Model.Category.DefaultEnabled)) OrElse (ActionType.Equals(ACTION_TYPE_DUPLICATE) AndAlso model.Model.Category Is Nothing) Then
                Return
            End If

            If ActionType.Equals(ACTION_TYPE_EDIT) AndAlso Not String.IsNullOrEmpty(model.Model.Subject.Value) AndAlso Not model.Model.Subject.ModifyEnabled Then
                txtSubject.Enabled = False
            End If

            If ActionType.Equals(ACTION_TYPE_EDIT) AndAlso Not String.IsNullOrEmpty(model.Model.Category.IdCategory) AndAlso Not model.Model.Category.ModifyEnabled Then
                uscSelCategory.ReadOnly = True
            End If

            If Not String.IsNullOrEmpty(model.Model.Category.IdCategory) Then
                Dim category As Data.Category = Facade.CategoryFacade.GetById(Integer.Parse(model.Model.Category.IdCategory))
                uscSelCategory.DataSource = New List(Of Data.Category) From {category}
                uscSelCategory.DataBind()
                uscSelCategory.ReadOnly = model.Model.Category.ReadOnly
            End If
        End If
    End Sub
    Public Sub SetUDSBehaviour(model As UDSModel) Implements IUDSStaticData.SetUDSBehaviour
        'todo: per future implementazioni
    End Sub
#End Region

End Class