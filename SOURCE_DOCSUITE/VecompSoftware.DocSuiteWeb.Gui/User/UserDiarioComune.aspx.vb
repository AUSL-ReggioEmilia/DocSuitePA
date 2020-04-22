Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class UserDiarioComune
    Inherits UserBasePage

#Region " Events "

    Private Sub Page_DataBinding(sender As Object, e As EventArgs) Handles Me.DataBinding

    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.SetResponseNoCache()
        MasterDocSuite.TitleVisible = False
        InitializeAjax()

        If Not Page.IsPostBack Then
            If Not CommonInstance.ProtDiaryLogDateFrom.HasValue Then
                CommonInstance.ProtDiaryLogDateFrom = DateTime.Now
            End If
            If Not CommonInstance.ProtDiaryLogDateTo.HasValue Then
                CommonInstance.ProtDiaryLogDateTo = DateTime.Now
            End If
            rdpDateFrom.SelectedDate = CommonInstance.ProtDiaryLogDateFrom
            rdpDateTo.SelectedDate = CommonInstance.ProtDiaryLogDateTo
            Initialize()
        End If
        gvDiario.DataSource = New JournalFacade().GetUserCommonDiary(rdpDateFrom.SelectedDate.Value, rdpDateTo.SelectedDate.Value)
    End Sub

    Private Sub dg_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles gvDiario.ItemCommand
        If e.CommandName.Eq("Export") Then
            gvDiario.DoExport(e, gvDiario.Columns, gvDiario.Items)
            Exit Sub
        ElseIf e.CommandName.Eq("Sort") Then
            Exit Sub
        End If

        Dim selPage As String = String.Empty
        Dim sommPage As String = String.Empty
        Dim url As String = String.Empty

        Select Case e.CommandName
            Case "Docm"
                selPage = "../Docm/DocmLog.aspx?"
                sommPage = "../Docm/DocmVisualizza.aspx?"

                Dim year As String = e.CommandArgument.Substring(0, 4).Trim()
                Dim number As String = e.CommandArgument.Substring(5).Trim()
                url = String.Format("Type={0}&Year={1}&Number={2}", e.CommandName, year, number)
            Case "Prot"
                selPage = "../Prot/ProtLog.aspx?"
                sommPage = "../Prot/ProtVisualizza.aspx?"

                Dim year As String = e.CommandArgument.Substring(0, 4).Trim()
                Dim number As String = e.CommandArgument.Substring(5).Trim()
                url = String.Format("Type={0}&Year={1}&Number={2}&User={3}", e.CommandName, year, number, DocSuiteContext.Current.User.UserName)
            Case "Resl"
                selPage = "../Resl/ReslLog.aspx?"
                sommPage = "../Resl/ReslVisualizza.aspx?"
                url = String.Format("Type={0}&IdResolution={1}&User={2}", e.CommandName, e.CommandArgument, DocSuiteContext.Current.User.UserName)
        End Select

        Select Case DirectCast(e.CommandSource, Control).ID
            Case "lnkLog"
                Response.Redirect(selPage & CommonShared.AppendSecurityCheck(url))
            Case "lnkReference"
                If DocSuiteContext.Current.ProtocolEnv.MoveScrivaniaMenu AndAlso DocSuiteContext.Current.ProtocolEnv.IsDiaryEnabled Then
                    AjaxManager.ResponseScripts.Add(String.Format("location.href='{0}{1}';", sommPage, CommonShared.AppendSecurityCheck(url)))
                Else
                    AjaxManager.ResponseScripts.Add(String.Format("parent.location.href='{0}{1}';", sommPage, CommonShared.AppendSecurityCheck(url)))
                End If
        End Select

    End Sub

    Private Sub btnCerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCerca.Click
        If DateDiff(DateInterval.Day, rdpDateFrom.SelectedDate.Value, rdpDateTo.SelectedDate.Value) > 31 Then
            AjaxAlert("L'Intervallo non deve superare i 31 giorni")
            Exit Sub
        End If

        CommonInstance.ProtDiaryLogDateFrom = rdpDateFrom.SelectedDate.Value
        CommonInstance.ProtDiaryLogDateTo = rdpDateTo.SelectedDate.Value
        Initialize()
        gvDiario.MasterTableView.SortExpressions.AddSortExpression("LogDate DESC")
        gvDiario.DataBind()
    End Sub

    
    Protected Sub odsDiario_Selected(ByVal sender As System.Object, ByVal e As ObjectDataSourceStatusEventArgs)
        If e.ReturnValue IsNot Nothing Then
            'lblHeader.Text = String.Format("Diario generale ({0})", DirectCast(e.ReturnValue, DataSet).Tables(0).Rows.Count)
        End If
    End Sub

    Protected Sub gvDiario_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles gvDiario.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As UserDiary = DirectCast(e.Item.DataItem, UserDiary)


        With DirectCast(e.Item.FindControl("imgType"), Image)
            Select Case item.type
                Case "Prot"
                    .ImageUrl = "../Comm/Images/DocSuite/Protocollo16.gif"
                    .ToolTip = "Protocollo"
                Case "Docm"
                    .ImageUrl = "../Comm/Images/DocSuite/Pratica16.gif"
                    .ToolTip = "Pratiche"
                Case "Resl"
                    .ImageUrl = "../Comm/Images/DocSuite/resolution16.gif"
                    .ToolTip = Facade.TabMasterFacade.TreeViewCaption
            End Select
        End With

        With DirectCast(e.Item.FindControl("lnkLog"), RadButton)
            .CommandName = item.Type
            .CommandArgument = item.Codice
        End With

        With DirectCast(e.Item.FindControl("lnkReference"), LinkButton)
            Select Case item.Type
                Case "Docm", "Prot"
                    .Text = item.Codice
                Case "Resl"
                    Dim idRes As Object = item.Codice
                    If idRes IsNot DBNull.Value Then
                        .Text = Facade.ResolutionFacade.GetResolutionNumber(Facade.ResolutionFacade.GetById(idRes), False)
                    End If
            End Select
            .CommandName = item.Type
            .CommandArgument = item.Codice
        End With

        With DirectCast(e.Item.FindControl("lblObject"), Label)
            .Text = item.Object
        End With

        With DirectCast(e.Item.FindControl("lblLogDate"), Label)
            .Text = item.LogDate
        End With

        With DirectCast(e.Item.FindControl("imgInsert"), Image)
            .ImageUrl = If(item.PI = 0, ImagePath.SmallEmpty, "../Comm/Images/Flag16.gif")
        End With

        With DirectCast(e.Item.FindControl("imgSummary"), Image)
            .ImageUrl = If(item.PS = 0, ImagePath.SmallEmpty, "../Comm/Images/Flag16.gif")
        End With

        With DirectCast(e.Item.FindControl("imgDocument"), Image)
            .ImageUrl = If(item.PD = 0, ImagePath.SmallEmpty, "../Comm/Images/Flag16.gif")
        End With

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(gvDiario, gvDiario, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, gvDiario, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, rdpDateFrom)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, rdpDateTo)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, divTitolo)
    End Sub

    Private Sub Initialize()
        If Not rdpDateFrom.SelectedDate.HasValue Then
            AjaxAlert("Manca data Inizio")
            Exit Sub
        End If
        If Not rdpDateTo.SelectedDate.HasValue Then
            AjaxAlert("Manca data Fine")
            Exit Sub
        End If
    End Sub

#End Region

End Class
