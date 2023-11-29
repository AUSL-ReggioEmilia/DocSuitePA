Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.Fascicles
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class uscCategoryRest
    Inherits DocSuite2008BaseControl

#Region " Fields "
    ''' <summary>
    ''' TODO: Rimuovere questi eventi in 8.80. Utilizzato per compatibilità con lo user control Fascicoli
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Delegate Sub EntityAddedEventHandler(ByVal sender As Object, ByVal e As List(Of String))
    Public Delegate Sub EntityRemovedEventHandler(ByVal sender As Object, ByVal e As List(Of String))
    Public Event EntityAdded As EntityAddedEventHandler
    Public Event EntityRemoved As EntityRemovedEventHandler
#End Region
#Region " Properties "
    Public ReadOnly Property MainContent As Control
        Get
            Return pnlMainContent
        End Get
    End Property

    Public Property ShowManagerFascicolable As String

    Public Property ShowSecretaryFascicolable As String

    Public Property ShowRoleFascicolable As Short?

    Public Property FascicleType As FascicleType?

    Public Property ShowAuthorizedFascicolable As Boolean
    Public Property ShowProcessFascicleTemplate As Boolean = False
    Public Property ProcessNodeSelectable As Boolean = False
    Public Property AjaxRequestEnabled As Boolean = False

    Public Property HideTitle As Boolean?

    Protected ReadOnly Property IdCategoryToPage As String
        Get
            If (IdCategory.HasValue) Then
                Return IdCategory.ToString()
            End If
            Return "null"
        End Get
    End Property

    Public Property IdCategory As Integer?

    Protected ReadOnly Property ControlConfiguration As String
        Get
            Return JsonConvert.SerializeObject(New With {.showAuthorizedFascicolable = ShowAuthorizedFascicolable, .showManagerFascicolable = ShowManagerFascicolable, .showSecretaryFascicolable = ShowSecretaryFascicolable,
                                        .showRoleFascicolable = ShowRoleFascicolable, .fascicleType = FascicleType})
        End Get
    End Property

    Public ReadOnly Property SelectedCategories As ICollection(Of Integer)
        Get
            Return treeCategory.GetAllNodes().Where(Function(x) x.Attributes.Item("IsSelected").Eq(True.ToString())).Select(Function(s) Integer.Parse(s.Value)).ToList()
        End Get
    End Property

    Public Property ShowProcesses As Boolean = False

    Public Property IsRequired As Boolean
        Get
            Return AnyNodeCheck.Enabled
        End Get
        Set(value As Boolean)
            AnyNodeCheck.Enabled = value
        End Set
    End Property
    Public Property DefaultCategoryId As Integer?
    Public Property IsProcessActive As Boolean = False
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        '''TODO: rimuovere appena la gestione di inserimento del fascicolo verrà completamente resa client side
        AddHandler AjaxManager.AjaxRequest, AddressOf uscCategoryRest_AjaxRequest
        If Not IsPostBack Then
            rowTitle.Visible = Not HideTitle.HasValue OrElse (HideTitle.HasValue AndAlso Not HideTitle.Value)
        End If
    End Sub

    ''' <summary>
    ''' TODO: rimuovere solo quando lo usercontrol dei contatti sarà REST
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub uscCategoryRest_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            Select Case ajaxModel.ActionName
                Case "CategoryAdded"
                    RaiseEvent EntityAdded(Me, ajaxModel.Value)
                Case "CategoryRemoved"
                    RaiseEvent EntityRemoved(Me, ajaxModel.Value)
            End Select

        Catch
            Exit Sub
        End Try
    End Sub
#End Region

#Region " Methods "

#End Region

End Class