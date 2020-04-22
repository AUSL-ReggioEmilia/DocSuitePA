Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Public Class TbltTemplateProtocolManager
    Inherits ProtBasePage

#Region "Fields"
    Private Const TemplateInsertPage As String = "../Prot/TemplateProtocolInsert.aspx"
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub TemplateProtocol_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case "add"
                BtnAddTemplate_Click(sender, e)
            Case "loadData"
                LoadFinder()
            Case "delete"
                BtnDeleteTemplate_Click(sender, e)
            Case "setDefault"
                BtnSetDefault_Click(sender, e)
            Case Else
                Throw New ArgumentException()
        End Select
    End Sub

    Protected Sub grdTemplateProtocol_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles grdTemplateProtocol.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundTemplate As TemplateProtocol = DirectCast(e.Item.DataItem, TemplateProtocol)

        Dim templateNameColumn As LinkButton = TryCast(e.Item.FindControl("lblTemplateName"), LinkButton)
        If templateNameColumn IsNot Nothing Then
            templateNameColumn.CommandArgument = boundTemplate.Id.ToString()
            templateNameColumn.Text = boundTemplate.TemplateName
        End If

        Dim imgDefaultColumn As Image = TryCast(e.Item.FindControl("imgDefault"), Image)
        If imgDefaultColumn IsNot Nothing Then
            imgDefaultColumn.Visible = True
            If boundTemplate.IsDefault Then
                imgDefaultColumn.ImageUrl = "../App_Themes/DocSuite2008/imgset16/star.png"
            Else
                imgDefaultColumn.Visible = False
            End If
        End If

        Dim imgRemoveDefaultColumn As ImageButton = TryCast(e.Item.FindControl("imgRemoveDefault"), ImageButton)
        If imgRemoveDefaultColumn IsNot Nothing Then
            imgRemoveDefaultColumn.Visible = True
            If boundTemplate.IsDefault Then
                imgRemoveDefaultColumn.CommandArgument = boundTemplate.Id.ToString()
                imgRemoveDefaultColumn.ImageUrl = "../App_Themes/DocSuite2008/imgset16/remove_star.png"
                imgRemoveDefaultColumn.Attributes.Add("onmouseover", "this.style.cursor='hand';")
            Else
                imgRemoveDefaultColumn.Visible = False
            End If
        End If

    End Sub

    Private Sub grdTemplateProtocol_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles grdTemplateProtocol.ItemCommand
        Select Case e.CommandName
            Case "editTemplate"
                EditTemplate(e.CommandArgument)
            Case "removeDefault"
                RemoveDefault(e.CommandArgument)
        End Select
    End Sub

    Private Sub EditTemplate(ByVal argument As Object)
        Dim idTemplate As Integer = Integer.Parse(argument.ToString())
        Dim params As String = String.Format("Type=Prot&Action=modify&IdTemplate={0}", idTemplate)
        Dim url As String = String.Format("{0}?{1}", TemplateInsertPage, CommonShared.AppendSecurityCheck(params))
        Response.Redirect(url)
    End Sub

    Private Sub BtnAddTemplate_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim url As String = String.Format("{0}?{1}", TemplateInsertPage, CommonShared.AppendSecurityCheck("Type=Prot&Action=add"))
        Response.Redirect(url)
    End Sub

    Private Sub RemoveDefault(ByVal argument As Object)
        Dim idTemplate As Integer = Integer.Parse(argument.ToString())
        Facade.TemplateProtocolFacade.RemoveDefaultTemplate(idTemplate)
        AjaxManager.ResponseScripts.Add("return LoadPageData();")
    End Sub

    Private Sub BtnDeleteTemplate_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim idTemplate As Integer = GetCheckedRow()
        If idTemplate.Equals(0) Then
            AjaxAlert("Nessun template selezionato per l' annullamento")
            Exit Sub
        End If

        Dim template As TemplateProtocol = Facade.TemplateProtocolFacade.GetById(idTemplate)
        template.IdTemplateStatus = TemplateStatus.Disabled
        Facade.TemplateProtocolFacade.UpdateOnly(template)
        AjaxManager.ResponseScripts.Add("return LoadPageData();")
    End Sub

    Private Sub BtnSetDefault_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim idTemplate As Integer = GetCheckedRow()
        If idTemplate.Equals(0) Then
            AjaxAlert("Nessun template selezionato per la modifica")
            Exit Sub
        End If

        Dim template As TemplateProtocol = Facade.TemplateProtocolFacade.GetById(idTemplate)
        Facade.TemplateProtocolFacade.SetDefaultTemplate(template)
        AjaxManager.ResponseScripts.Add("return LoadPageData();")
    End Sub

    Protected Sub grdTemplateProtocol_ItemCreated(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdTemplateProtocol.ItemCreated
        If (TypeOf e.Item Is GridDataItem) Then
            Dim rdb As RadioButton = CType(e.Item.FindControl("rdbSelect"), RadioButton)
            rdb.Attributes.Add("OnClick", "SelectMeOnly(" & rdb.ClientID & ", " & "'grdTemplateProtocol'" & ")")
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, grdTemplateProtocol, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(grdTemplateProtocol, grdTemplateProtocol, MasterDocSuite.AjaxDefaultLoadingPanel)

        AddHandler AjaxManager.AjaxRequest, AddressOf TemplateProtocol_AjaxRequest
    End Sub

    Private Sub Initialize()
        If Not CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateGroups) Then
            Throw New DocSuiteException("Utente non abilitato alla gestione del template di Protocollo")
        End If

        Page.Title = "Gestione Template di Protocollo"
    End Sub

    Private Sub LoadFinder()
        Dim finder As NHibernateTemplateProtocolFinder = New NHibernateTemplateProtocolFinder("ProtDB")
        finder.Status = TemplateStatus.Active

        grdTemplateProtocol.PageSize = finder.PageSize
        grdTemplateProtocol.MasterTableView.SortExpressions.Clear()
        AddExpressionOrder()
        grdTemplateProtocol.Finder = finder
        grdTemplateProtocol.DataBindFinder()
        grdTemplateProtocol.Visible = True
        grdTemplateProtocol.MasterTableView.AllowMultiColumnSorting = False
    End Sub

    Private Function GetCheckedRow() As Integer
        For Each item As GridDataItem In grdTemplateProtocol.Items
            With DirectCast(item.FindControl("rdbSelect"), RadioButton)
                If .Checked Then
                    Dim key As String = item.GetDataKeyValue("Id").ToString()
                    Return Integer.Parse(key)
                End If
            End With
        Next
        Return 0
    End Function

    Private Sub AddExpressionOrder()
        Dim expression As New GridSortExpression()
        expression.FieldName = "TemplateName"
        expression.SetSortOrder("Ascending")
        grdTemplateProtocol.MasterTableView.SortExpressions.AddSortExpression(expression)
    End Sub
#End Region

End Class