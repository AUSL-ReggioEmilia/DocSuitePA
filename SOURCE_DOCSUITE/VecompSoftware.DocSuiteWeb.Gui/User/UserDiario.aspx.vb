Imports System.Collections.Specialized
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class UserDiario
    Inherits UserBasePage

#Region " Fields "

    Dim selPage As String = String.Empty
    Dim sommPage As String = String.Empty

#End Region

#Region " Properties "

    Private ReadOnly Property Titolo() As String
        Get
            Return Request.QueryString("Title")
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False

        InitializeAjax()
        If Not IsPostBack Then
            ' Inizializzo da commoninstance o metto ad oggi
            Select Case Type
                Case "Prot"
                    If CommonInstance.ProtDiaryLogDateFrom.HasValue Then
                        rdpDateFrom.SelectedDate = CommonInstance.ProtDiaryLogDateFrom.Value
                    End If
                    If CommonInstance.ProtDiaryLogDateTo.HasValue Then
                        rdpDateTo.SelectedDate = CommonInstance.ProtDiaryLogDateTo.Value
                    End If
                Case "Docm"
                    If CommonInstance.DocmDiaryLogDateFrom.HasValue Then
                        rdpDateFrom.SelectedDate = CommonInstance.DocmDiaryLogDateFrom.Value
                    End If
                    If CommonInstance.DocmDiaryLogDateTo.HasValue Then
                        rdpDateTo.SelectedDate = CommonInstance.DocmDiaryLogDateTo.Value
                    End If
                Case "Resl"
                    If CommonInstance.ReslDiaryLogDateFrom.HasValue Then
                        rdpDateFrom.SelectedDate = CommonInstance.ReslDiaryLogDateFrom.Value
                    End If
                    If CommonInstance.ReslDiaryLogDateTo.HasValue Then
                        rdpDateTo.SelectedDate = CommonInstance.ReslDiaryLogDateTo.Value
                    End If
            End Select
            If Not rdpDateFrom.SelectedDate.HasValue Then
                rdpDateFrom.SelectedDate = DateTime.Now
            End If
            If Not rdpDateTo.SelectedDate.HasValue Then
                rdpDateTo.SelectedDate = DateTime.Now
            End If

            Initialize()
        End If
    End Sub

    Private Sub gvProtocols_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles gvProtocols.ItemDataBound
        If e.Item.ItemType.Equals(GridItemType.Item) OrElse e.Item.ItemType.Equals(GridItemType.AlternatingItem) Then
            Dim item As UserDiary = e.Item.DataItem

            With DirectCast(e.Item.FindControl("imgSelection"), RadButton)
                .CommandName = "ProtLogs:" & item.Codice
            End With

            With DirectCast(e.Item.FindControl("lbProtocollo"), LinkButton)
                .Text = item.Codice
                .CommandName = "ProtRefs:" & item.Codice
            End With

            With DirectCast(e.Item.FindControl("lblObject"), Label)
                .Text = item.Object
            End With

            With DirectCast(e.Item.FindControl("lblLogDate"), Label)
                .Text = item.LogDate
            End With

            With DirectCast(e.Item.FindControl("imgInserimento"), Image)
                .ImageUrl = SetIcona(item.PI)
            End With

            With DirectCast(e.Item.FindControl("imgSommario"), Image)
                .ImageUrl = SetIcona(item.PS)
            End With

            With DirectCast(e.Item.FindControl("imgDocumento"), Image)
                .ImageUrl = SetIcona(item.PD)
            End With

            With DirectCast(e.Item.FindControl("imgAutoriz"), Image)
                .ImageUrl = SetIcona(item.PZ)
            End With

            With DirectCast(e.Item.FindControl("imgModifica"), Image)
                .ImageUrl = SetIcona(item.PM)
            End With

            With DirectCast(e.Item.FindControl("imgHandled"), Image)
                .ImageUrl = SetIcona(item.IsHandled)
            End With

        End If
    End Sub

    Protected Sub gvDocuments_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles gvDocuments.ItemDataBound
        If e.Item.ItemType.Equals(GridItemType.Item) OrElse e.Item.ItemType.Equals(GridItemType.AlternatingItem) Then
            Dim item As UserDiary = e.Item.DataItem

            With DirectCast(e.Item.FindControl("imgSelection"), RadButton)
                .CommandName = "DocmLogs:" & item.Codice
            End With

            With DirectCast(e.Item.FindControl("lblPratica"), LinkButton)
                .Text = item.Codice
                .CommandName = "DocmRefs:" & item.Codice
            End With

            With DirectCast(e.Item.FindControl("lblObject"), Label)
                .Text = item.Object
            End With

            With DirectCast(e.Item.FindControl("lblLogDate"), Label)
                .Text = item.LogDate.ToShortDateString()
            End With

            With DirectCast(e.Item.FindControl("lblLogDate"), Label)
                .Text = item.LogDate.ToShortDateString()
            End With

            With DirectCast(e.Item.FindControl("imgInserimento"), Image)
                .ImageUrl = SetIcona(item.PI)
            End With

            With DirectCast(e.Item.FindControl("imgSummary"), Image)
                .ImageUrl = SetIcona(item.PS)
            End With

            With DirectCast(e.Item.FindControl("imgDocument"), Image)
                .ImageUrl = SetIcona(item.PD)
            End With

            With DirectCast(e.Item.FindControl("imgAuth"), Image)
                .ImageUrl = SetIcona(item.PZ)
            End With
        End If
    End Sub

    Protected Sub gvResolutions_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles gvResolutions.ItemDataBound
        If e.Item.ItemType.Equals(GridItemType.Item) OrElse e.Item.ItemType.Equals(GridItemType.AlternatingItem) Then
            Dim item As UserDiary = e.Item.DataItem

            With DirectCast(e.Item.FindControl("imgSelection"), RadButton)
                .CommandName = "ReslLogs:" & item.Codice
            End With

            With DirectCast(e.Item.FindControl("cmdResolution"), LinkButton)
                .Text = If(item.Codice IsNot DBNull.Value, Facade.ResolutionFacade.GetResolutionNumber(Facade.ResolutionFacade.GetById(item.Codice), False), "")
                .CommandName = "ReslRefs:" & item.Codice
            End With

            With DirectCast(e.Item.FindControl("lblObject"), Label)
                .Text = item.Object
            End With

            With DirectCast(e.Item.FindControl("lblLogDate"), Label)
                .Text = item.LogDate
            End With

            With DirectCast(e.Item.FindControl("imgInserimento"), Image)
                .ImageUrl = SetIcona(item.PI)
            End With

            With DirectCast(e.Item.FindControl("imgSummary"), Image)
                .ImageUrl = SetIcona(item.PS)
            End With

            With DirectCast(e.Item.FindControl("imgDocument"), Image)
                .ImageUrl = SetIcona(item.PD)
            End With

            With DirectCast(e.Item.FindControl("imgAuth"), Image)
                .ImageUrl = SetIcona(item.PZ)
            End With

        End If
    End Sub

    Private Sub gvProtocols_ItemCommand(ByVal source As System.Object, ByVal e As GridCommandEventArgs) Handles gvProtocols.ItemCommand
        If e.CommandName.Eq("Export") OrElse e.CommandName.Eq("ExportFull") Then
            gvProtocols.DoExport(e, gvProtocols.Columns, gvProtocols.Items)
            Exit Sub
        ElseIf e.CommandName.Eq("Sort") Then
            Exit Sub
        End If

        SetDocumento(e.CommandName)
    End Sub

    Private Sub gvDocuments_ItemCommand(ByVal source As System.Object, ByVal e As GridCommandEventArgs) Handles gvDocuments.ItemCommand
        If e.CommandName.Eq("Export") OrElse e.CommandName.Eq("ExportFull") Then
            gvDocuments.DoExport(e, gvDocuments.Columns, gvDocuments.Items)
            Exit Sub
        ElseIf e.CommandName.Eq("Sort") Then
            Exit Sub
        End If

        SetDocumento(e.CommandName)
    End Sub

    Private Sub gvResolutions_ItemCommand(ByVal source As System.Object, ByVal e As GridCommandEventArgs) Handles gvResolutions.ItemCommand
        If e.CommandName.Eq("Export") OrElse e.CommandName.Eq("ExportFull") Then
            gvResolutions.DoExport(e, gvResolutions.Columns, gvResolutions.Items)
            Exit Sub
        ElseIf e.CommandName.Eq("Sort") Then
            Exit Sub
        End If

        SetDocumento(e.CommandName)
    End Sub

    Private Sub btnCerca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCerca.Click
        CercaLog()
    End Sub

    Protected Sub odsProtocol_Selecting(ByVal sender As System.Object, ByVal e As ObjectDataSourceSelectingEventArgs) Handles odsProtocol.Selecting
        e.Cancel = Not pnlProtGrid.Visible
        SetDataSourceParameter(e.InputParameters)
    End Sub

    Protected Sub odsDocument_Selecting(ByVal sender As System.Object, ByVal e As ObjectDataSourceSelectingEventArgs) Handles odsDocument.Selecting
        e.Cancel = Not pnlDocmGrid.Visible
        SetDataSourceParameter(e.InputParameters)
    End Sub

    Private Sub odsResolution_Selecting(ByVal sender As Object, ByVal e As ObjectDataSourceSelectingEventArgs) Handles odsResolution.Selecting
        e.Cancel = Not pnlReslGrid.Visible
        SetDataSourceParameter(e.InputParameters)
    End Sub

    Protected Sub odsDocuments_Selected(ByVal sender As System.Object, ByVal e As ObjectDataSourceStatusEventArgs) Handles odsDocument.Selected
        If Not e.ReturnValue Is Nothing Then
            lblHeader.Text &= String.Format(" ({0})", DirectCast(e.ReturnValue, ICollection).Count)
        End If
    End Sub

    Protected Sub odsProtocols_Selected(ByVal sender As System.Object, ByVal e As ObjectDataSourceStatusEventArgs) Handles odsProtocol.Selected
        If Not e.ReturnValue Is Nothing Then
            lblHeader.Text &= String.Format(" ({0})", DirectCast(e.ReturnValue, ICollection).Count)
        End If
    End Sub

    Private Sub odsResolution_Selected(ByVal sender As Object, ByVal e As ObjectDataSourceStatusEventArgs) Handles odsResolution.Selected
        If Not e.ReturnValue Is Nothing Then
            lblHeader.Text &= String.Format(" ({0})", DirectCast(e.ReturnValue, ICollection).Count)
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, divTitolo)

        If gvProtocols.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(gvProtocols, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
        End If

        If gvDocuments.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, gvDocuments, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(gvDocuments, gvDocuments, MasterDocSuite.AjaxDefaultLoadingPanel)
        End If

        If gvResolutions.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, gvResolutions, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(gvResolutions, gvResolutions, MasterDocSuite.AjaxDefaultLoadingPanel)
        End If
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
        If (rdpDateTo.SelectedDate.Value - rdpDateFrom.SelectedDate.Value).TotalDays > 31 Then
            AjaxAlert("L'intervallo non deve superare i 31 giorni")
            Exit Sub
        End If

        ' salvo nella commonInstance
        Select Case Type
            Case "Prot"
                CommonInstance.ProtDiaryLogDateFrom = rdpDateFrom.SelectedDate.Value
                CommonInstance.ProtDiaryLogDateTo = rdpDateTo.SelectedDate.Value
            Case "Docm"
                CommonInstance.DocmDiaryLogDateFrom = rdpDateFrom.SelectedDate.Value
                CommonInstance.DocmDiaryLogDateTo = rdpDateTo.SelectedDate.Value
            Case "Resl"
                CommonInstance.ReslDiaryLogDateFrom = rdpDateFrom.SelectedDate.Value
                CommonInstance.ReslDiaryLogDateTo = rdpDateFrom.SelectedDate.Value
        End Select

        'in base alla tipologia di sezione visualizza la griglia corretta
        pnlProtGrid.Visible = False
        pnlDocmGrid.Visible = False
        pnlReslGrid.Visible = False
        Select Case Type
            Case "Prot"
                gvProtocols.MasterTableView.GetColumn("cHandled").Visible = ProtocolEnv.ProtHandlerEnabled
                pnlProtGrid.Visible = True
                lblHeader.Text = "Protocollo - " & Titolo
                gvProtocols.DataBind()
            Case "Docm"
                pnlDocmGrid.Visible = True
                lblHeader.Text = "Pratiche - " & Titolo
                gvDocuments.DataBind()
            Case "Resl"
                pnlReslGrid.Visible = True
                lblHeader.Text = Facade.TabMasterFacade.TreeViewCaption & " - " & Titolo
                gvResolutions.DataBind()
        End Select

    End Sub

    Public Function SetIcona(ByVal count As Integer) As String
        Return If(count = 0, ImagePath.SmallEmpty, "../Comm/Images/Flag16.gif")
    End Function

    Public Sub SetDocumento(ByVal commandName As String)
        Dim parameters As String = String.Empty
        Dim paramType As String = Left(commandName, 4)
        Select Case paramType
            Case "Prot"
                selPage = "../Prot/ProtLog.aspx?"
                sommPage = "../Prot/ProtVisualizza.aspx?"
                Dim year As String = Mid$(commandName, 10, 4)
                Dim number As String = Mid$(commandName, 15, 7)
                parameters = String.Format("Type={0}&Year={1}&Number={2}", paramType, year, number)
            Case "Docm"
                selPage = "../Docm/DocmLog.aspx?"
                sommPage = "../Docm/DocmVisualizza.aspx?"
                Dim year As String = Mid$(commandName, 10, 4)
                Dim number As String = Mid$(commandName, 15, 7)
                parameters = String.Format("Type={0}&Year={1}&Number={2}&User={3}", paramType, year, number, DocSuiteContext.Current.User.UserName)
            Case "Resl"
                selPage = "../Resl/ReslLog.aspx?"
                sommPage = "../Resl/ReslVisualizza.aspx?"
                Dim idResolution As String = Mid$(commandName, 10)
                parameters = String.Format("Type={0}&IdResolution={1}&User={2}", paramType, idResolution, DocSuiteContext.Current.User.UserName)
        End Select

        Dim command As String = Mid$(commandName, 5, 5)
        Select Case command
            Case "Logs:"
                Response.Redirect(selPage & CommonShared.AppendSecurityCheck(parameters))
            Case "Refs:"
                Response.Redirect(sommPage & CommonShared.AppendSecurityCheck(parameters))
                'Dim script As String = "parent.location='" & sommPage & CommonShared.AppendSecurityCheck(s) & "';"
                'AjaxManager.ResponseScripts.Add(script)
        End Select
    End Sub

    Private Sub CercaLog()
        Initialize()
    End Sub

    Private Sub SetDataSourceParameter(ByRef oParams As IOrderedDictionary)
        oParams.Item(0) = rdpDateFrom.SelectedDate
        oParams.Item(1) = rdpDateTo.SelectedDate
    End Sub

#End Region

End Class
