Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
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
    Public Delegate Sub CategoryAddedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Delegate Sub CategoryRemovedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Event CategoryAdded As CategoryAddedEventHandler
    Public Event CategoryRemoved As CategoryRemovedEventHandler
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
    ''' TODO: rimuover in 8.80 con refactor usc fascicolo
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub uscCategoryRest_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "Add"
                RaiseEvent CategoryAdded(Me, New EventArgs())

            Case "Remove"
                RaiseEvent CategoryRemoved(Me, New EventArgs())
        End Select
    End Sub
#End Region

#Region " Methods "

#End Region

End Class