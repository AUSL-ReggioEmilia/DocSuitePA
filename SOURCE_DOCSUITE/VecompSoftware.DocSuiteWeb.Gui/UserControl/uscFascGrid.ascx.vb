Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.OData

Partial Public Class uscFascGrid
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Public Const COLUMN_CLIENT_SELECT As String = "ClientSelectColumn"
    Public Const COLUMN_FASCICLE As String = "cFascicle"
    Public Const COLUMN_DOCUMENT As String = "cDocument"
    Public Const COLUMN_ID As String = "Id"
    Public Const COLUMN_REGISTRATION_DATE As String = "RegistrationDate"
    Public Const COLUMN_START_DATE As String = "StartDate"
    Public Const COLUMN_END_DATE As String = "EndDate"
    Public Const COLUMN_CATEGORY As String = "CategoryName"
    Public Const COLUMN_RIF As String = "FascicleContacts.Contact.Description"
    Public Const COLUMN_MANAGER As String = "Manager"
    Public Const COLUMN_OBJECT As String = "FascicleObject"

    Public Const COLUMN_UNREAD As String = "cUnread"
    Private _currentODataFacade As ODataFacade

#End Region

#Region " Properties "

    Public ReadOnly Property Grid() As BindGrid
        Get
            Return gvFascicles
        End Get
    End Property

    ''' <summary> Visualizza/Nasconde colonna di selezione </summary>
    Public Property ColumnClientSelectVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_CLIENT_SELECT).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_CLIENT_SELECT).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Fascicle </summary>
    Public Property ColumnFascicleVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_FASCICLE).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_FASCICLE).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Document </summary>
    Public Property ColumnDocumentVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_DOCUMENT).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_DOCUMENT).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Tipo </summary>
    Public Property ColumnIdVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_ID).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_ID).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Da leggere </summary>
    Public Property ColumnUnreadVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_UNREAD).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_UNREAD).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Data registrazione </summary>
    Public Property ColumnRegistrationDateVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_REGISTRATION_DATE).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_REGISTRATION_DATE).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Data registrazione Start </summary>
    Public Property ColumnStartDateVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_START_DATE).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_START_DATE).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Data End </summary>
    Public Property ColumnEndDateVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_END_DATE).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_END_DATE).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna c </summary>
    Public Property ColumnCategoryVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_CATEGORY).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_CATEGORY).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Numero Rif </summary>
    Public Property ColumnRifVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_RIF).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_RIF).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Descrizione manager </summary>
    Public Property ColumnManagerVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_MANAGER).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_MANAGER).Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde colonna Oggetto </summary>
    Public Property ColumnObjectVisible() As Boolean
        Get
            Return gvFascicles.Columns.FindByUniqueName(COLUMN_OBJECT).Visible
        End Get
        Set(ByVal value As Boolean)
            gvFascicles.Columns.FindByUniqueName(COLUMN_OBJECT).Visible = value
        End Set
    End Property

    Private ReadOnly Property CurrentODataFacade As ODataFacade
        Get
            If _currentODataFacade Is Nothing Then
                _currentODataFacade = New ODataFacade()
            End If
            Return _currentODataFacade
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then

        End If
    End Sub

    Private Sub gvFascicles_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles gvFascicles.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim entity As WebAPIDto(Of FascicleModel) = DirectCast(e.Item.DataItem, WebAPIDto(Of FascicleModel))
        Dim imgFascicle As Image = CType(e.Item.FindControl("imgFasc"), Image)
        If imgFascicle IsNot Nothing Then
            If entity.Entity.EndDate.HasValue Then
                imgFascicle.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/fascicle_close.png"
                imgFascicle.ToolTip = "Fascicolo chiuso"
            Else
                imgFascicle.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/fascicle_open.png"
                imgFascicle.ToolTip = "Fascicolo aperto"
            End If
        End If

        Dim imgFascicleType As Image = CType(e.Item.FindControl("imgFascicleType"), Image)
        If imgFascicleType IsNot Nothing Then
            Select Case entity.Entity.FascicleType
                Case Model.Entities.Fascicles.FascicleType.Legacy
                    imgFascicleType.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/fascicle_legacy.png"
                    imgFascicleType.ToolTip = "Fascicolo non a norma"
                Case Model.Entities.Fascicles.FascicleType.Period
                    imgFascicleType.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/history.png"
                    imgFascicleType.ToolTip = "Fascicolo periodico"
                Case Model.Entities.Fascicles.FascicleType.Procedure
                    imgFascicleType.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/fascicle_procedure.png"
                    imgFascicleType.ToolTip = "Fascicolo per procedimento"
                Case Else
                    imgFascicleType.Visible = False
            End Select
        End If

        Dim hasPrivacy As Boolean = False
        Dim lnkFascicle As LinkButton = CType(e.Item.FindControl("lnkFascicle"), LinkButton)
        If lnkFascicle IsNot Nothing Then
            lnkFascicle.Text = String.Concat(entity.Entity.Title, " ", GetFascicleObject(entity.Entity, hasPrivacy))
            lnkFascicle.CommandArgument = String.Concat(entity.Entity.UniqueId.ToString(), "|", entity.Entity.Year.ToString(), "|", entity.Entity.Category.IdCategory.ToString(), "|", entity.Entity.Number.ToString())
        End If

        If hasPrivacy Then
            Dim cbSelect As CheckBox = CType(e.Item.FindControl("cbSelect"), CheckBox)
            cbSelect.Enabled = False
        End If

        Dim lblManager As Label = DirectCast(e.Item.FindControl("lblManager"), Label)
        If lblManager IsNot Nothing AndAlso Not String.IsNullOrEmpty(entity.Entity.Manager) Then
            lblManager.Text = entity.Entity.Manager.Replace("|", " ")
        End If
    End Sub

    Private Sub gvFascicles_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles gvFascicles.ItemCommand
        Select Case e.CommandName
            Case "ShowFasc"
                Select Case BasePage.Action
                    Case "Docm"
                        Dim arguments As String() = Split(e.CommandArgument.ToString(), "|", 4)
                        Dim fascParam As FascicleParam = New FascicleParam() With {.Year = Integer.Parse(arguments(1)), .IdCategory = Integer.Parse(arguments(2)), .Incremental = Integer.Parse(arguments(3))}
                        Dim script As String = "CloseWindow('" & JsonConvert.SerializeObject(fascParam) & "');"
                        AjaxManager.ResponseScripts.Add(script)
                    Case "SearchFascicles"
                        Dim arguments As String() = Split(e.CommandArgument.ToString(), "|")
                        Dim params As String = String.Concat(arguments(0))
                        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", params))
                    Case Else
                        Dim arguments As String() = Split(e.CommandArgument.ToString(), "|")
                        Dim params As String = String.Concat("Type=Fasc&IdFascicle=", arguments(0))
                        Response.Redirect("../Fasc/FascVisualizza.aspx?" & CommonShared.AppendSecurityCheck(params))
                End Select
        End Select
    End Sub

#End Region

#Region " Methods "
    Private Function GetFascicleObject(fascicle As FascicleModel, ByRef hasPrivacy As Boolean) As String
        Dim fascicleObject As String = fascicle.FascicleObject
        hasPrivacy = False
        Dim privacyObject As String = DocSuiteContext.Current.ProtocolEnv.SecurityFascicleObject

        If String.IsNullOrEmpty(privacyObject) Then
            Return fascicleObject
        End If
        Dim isViewable As Boolean = CurrentODataFacade.HasFascicleViewableRight(fascicle.UniqueId)
        If isViewable Then
            Return fascicleObject
        End If
        hasPrivacy = True
        Return privacyObject
    End Function
#End Region

End Class