Imports System.ComponentModel
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class Duplicate
    Inherits CommonBasePage

#Region " Constants "
    Private Const DuplicateDefaultInSessionName As String = "Series.DuplicateDefaults"
#End Region

#Region " Fields "

    Private _documentSeriesItem As DocumentSeriesItem

#End Region

#Region " Properties "

    Public ReadOnly Property CurrentDocumentSeriesItem() As DocumentSeriesItem
        Get
            If _documentSeriesItem Is Nothing Then
                _documentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(IdDocumentSeriesItem)
            End If

            Return _documentSeriesItem
        End Get
    End Property

    Public ReadOnly Property IdDocumentSeriesItem() As Integer
        Get
            Dim idDoc As Integer
            If ViewState("idDocumentSeriesItem") Is Nothing Then
                idDoc = Request.QueryString.GetValue(Of Integer)("idDocumentSeriesItem")
            Else
                idDoc = Convert.ToInt32(ViewState("idDocumentSeriesItem"))
            End If
            ViewState("idDocumentSeriesItem") = idDoc
            Return idDoc
        End Get
    End Property

    Public ReadOnly Property ItemsToDuplicate() As IList(Of ItemToDuplicate)
        Get
            Dim tor As New List(Of ItemToDuplicate)
            For Each item As ListItem In DuplicationList.Items
                If item.Selected Then
                    tor.Add(CType(item.Value, ItemToDuplicate))
                End If
            Next
            Return tor
        End Get
    End Property

    Private _duplicateDefault As IList(Of ItemToDuplicate)

    Private ReadOnly Property DuplicateDefault() As IList(Of ItemToDuplicate)
        Get
            If _duplicateDefault Is Nothing Then
                If ProtocolEnv.DuplicateDefaultsInSession AndAlso Session(DuplicateDefaultInSessionName) IsNot Nothing Then
                    _duplicateDefault = CType(Session(DuplicateDefaultInSessionName), IList(Of ItemToDuplicate))
                Else
                    _duplicateDefault = New List(Of ItemToDuplicate)()
                    For Each x As String In ProtocolEnv.DuplicateDefaults
                        _duplicateDefault.Add(CType([Enum].Parse(GetType(ItemToDuplicate), x), ItemToDuplicate))
                    Next
                End If
            End If
            Return _duplicateDefault
        End Get
    End Property

    Private Sub SaveDuplicateDefaultsInSession()
        If ProtocolEnv.DuplicateDefaultsInSession Then
            Session(DuplicateDefaultInSessionName) = ItemsToDuplicate
        End If
    End Sub

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ItemPreview.Item = CurrentDocumentSeriesItem
            ItemPreview.Show()

            DataBindItems()
        End If
    End Sub

    Private Sub DataBindItems()
        DuplicationList.Items.Clear()
        For Each val As ItemToDuplicate In [Enum].GetValues(GetType(ItemToDuplicate))
            Dim selected As Boolean = DuplicateDefault.Contains(val)
            AddItem(DuplicationList, val.GetDescription(), val, selected)
        Next
    End Sub

#End Region

#Region " Methods "

    Private Shared Sub AddItem(ByVal cbl As CheckBoxList, ByVal text As String, value As Integer, ByVal selected As Boolean)
        Dim li As New ListItem
        li.Value = value.ToString()
        li.Text = text
        li.Selected = selected
        cbl.Items.Add(li)
    End Sub
#End Region

    Private Sub BtnDuplicateClick(sender As Object, e As EventArgs) Handles btnDuplicate.Click
        SaveDuplicateDefaultsInSession()
    End Sub

    Private Sub CmdRestoreDefaultClick(sender As Object, e As EventArgs) Handles cmdRestoreDefault.Click
        Session(DuplicateDefaultInSessionName) = Nothing
        DataBindItems()
    End Sub
End Class

Public Enum ItemToDuplicate
    <Description("Settori di appartenenza")> _
    OwnerRoles
    <Description("Autorizzazioni di conoscenza")> _
    KnowledgeRoles
    <Description("Oggetto")> _
    Subject
    <Description("Data pubblicazione e Data ritiro")> _
    Publication
    <Description("Dati dinamici")> _
    DynamicData
    <Description("Classificazione")> _
    Category
    <Description("Documenti")> _
    Documents
End Enum