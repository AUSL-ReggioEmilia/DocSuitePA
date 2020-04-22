Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers

Partial Class TbltLog
    Inherits CommonBasePage

#Region " Properties "

    Private ReadOnly Property EntityId() As Integer?
        Get
            Dim id As Integer
            If Integer.TryParse(Request.QueryString("entityId"), id) Then
                Return id
            End If
            Return Nothing
        End Get
    End Property
    Private ReadOnly Property EntityUniqueId() As Guid?
        Get
            Dim uid As Guid
            If Guid.TryParse(Request.QueryString("entityUniqueId"), uid) Then
                Return uid
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property TableName() As String
        Get
            Return Request.QueryString("TableName")
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        MasterDocSuite.TitleVisible = False
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(RadGridLog, RadGridLog, MasterDocSuite.AjaxLoadingPanelSearch)

        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub RadGridLog_ItemDataBound(ByVal sender As System.Object, ByVal e As GridItemEventArgs) Handles RadGridLog.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim item As TableLog = DirectCast(e.Item.DataItem, TableLog)
        Dim row As GridDataItem = DirectCast(e.Item, GridDataItem)

        With DirectCast(e.Item.FindControl("LogTypeDescription"), Label)
            If Not String.IsNullOrEmpty(item.LogType) Then
                row.Item("LogTypeDescription").Text = EnumHelper.GetDescription(item.LogType)
            End If
        End With

    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Dim finder As New NHibernateTableLogFinder()

        If EntityUniqueId IsNot Nothing Then
            finder.EntityUniqueId = EntityUniqueId
        End If

        If EntityId IsNot Nothing Then
            finder.EntityId = EntityId
        End If

        finder.TableName = TableName
        finder.PageSize = 10

        RadGridLog.Finder = finder
        RadGridLog.MasterTableView.PageSize = 10
        RadGridLog.MasterTableView.SortExpressions.AddSortExpression("LogDate DESC")
        RadGridLog.DataBindFinder()
    End Sub

#End Region

End Class