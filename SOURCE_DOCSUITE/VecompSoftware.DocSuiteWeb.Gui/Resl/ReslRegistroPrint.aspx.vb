Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Logging

Partial Public Class ReslRegistroPrint
    Inherits ReslBasePage

#Region "Field"
    Dim contenitori As IList(Of Container)
    Dim contenitoriDto As IList(Of ContainerRightsDto)
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeView1)

        If Not Me.IsPostBack Then
            RadTreeView1.Focus()
            Initialize()
        End If
    End Sub

    Protected Sub cmdStampaPDF_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStampaPDF.Click
        printRegistroGiornaliero()
    End Sub

    Protected Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectAll.Click
        SelectOrDeselectAll(True, RadTreeView1)
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeselectAll.Click
        SelectOrDeselectAll(False, RadTreeView1)
    End Sub
#End Region

#Region "Private Methods"

    Private Sub Initialize()
        Title = Facade.TabMasterFacade.TreeViewCaption & " - Stampa Registro Giornaliero"
        rblTipologia.Items.Add(Facade.ResolutionTypeFacade.DeliberaCaption)
        rblTipologia.Items(0).Value = ResolutionType.IdentifierDelibera
        rblTipologia.Items(0).Selected = True
        rblTipologia.Items.Add(Facade.ResolutionTypeFacade.DeterminaCaption)
        rblTipologia.Items(1).Value = ResolutionType.IdentifierDetermina

        If DocSuiteContext.Current.ResolutionEnv.SecurityPrint Then
            contenitori = Facade.ContainerFacade.GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Adoption, True)
        Else
            contenitoriDto = Facade.ContainerFacade.GetAllRights("Resl", True)
        End If

        If contenitori.Count > 0 Then
            PopolaTreeView(RadTreeView1)
        Else
            cmdStampaPDF.Enabled = False
        End If

    End Sub

    Private Sub PopolaTreeView(ByRef treeview As RadTreeView)
        Dim nodo As RadTreeNode

        treeview.Nodes.Clear()
        nodo = New RadTreeNode
        nodo.Text = "Contenitori"
        nodo.Checkable = False
        nodo.Font.Bold = True
        nodo.Expanded = True
        treeview.Nodes.Add(nodo)

        If DocSuiteContext.Current.ResolutionEnv.SecurityPrint Then
            For Each contenitore As Container In contenitori
                nodo = New RadTreeNode
                nodo.Text = contenitore.Name
                nodo.Value = contenitore.Id.ToString()
                nodo.ImageUrl = ImagePath.SmallBoxOpen
                nodo.Expanded = True
                treeview.Nodes(0).Nodes.Add(nodo)
            Next
        Else
            For Each contenitore As ContainerRightsDto In contenitoriDto
                nodo = New RadTreeNode
                nodo.Text = contenitore.Name
                nodo.Value = contenitore.ContainerId.ToString()
                nodo.ImageUrl = ImagePath.SmallBoxOpen
                nodo.Expanded = True
                treeview.Nodes(0).Nodes.Add(nodo)
            Next

        End If
    End Sub

    Private Function CheckedNode(ByVal tvw As RadTreeView) As Boolean
        Dim tn As RadTreeNode = Nothing
        Dim b As Boolean = False
        For Each tn In tvw.Nodes(0).Nodes
            If tn.Checked = True Then
                b = True
                Exit For
            End If
        Next
        Return b
    End Function

    Private Sub SelectOrDeselectAll(ByVal Selected As Boolean, ByRef treeview As RadTreeView)
        Dim tn As RadTreeNode = Nothing
        For Each tn In treeview.Nodes(0).Nodes
            tn.Checked = Selected
        Next
    End Sub
#End Region

    Private Sub printRegistroGiornaliero()

        Dim finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB")
        Dim id As String = String.Empty
        Dim names As String = String.Empty

        If CheckedNode(RadTreeView1) Then
            For Each nodo As RadTreeNode In RadTreeView1.Nodes(0).Nodes
                If nodo.Checked Then
                    If id = "" Then
                        id = nodo.Value
                        names = nodo.Text
                    Else
                        id = id & "," & nodo.Value
                        names = names & "," & nodo.Text
                    End If
                End If
            Next
            finder.ContainerIds = id
            finder.Adottata = True
            finder.DateFrom = RadDatePicker1.SelectedDate
            finder.DateTo = RadDatePicker2.SelectedDate
            Select Case True
                Case rblTipologia.Items(0).Selected
                    finder.Delibera = True
                Case rblTipologia.Items(1).Selected
                    finder.Determina = True
            End Select
            finder.EnablePaging = False
            finder.IsPrint = True
            finder.EagerLog = False

            Dim file As FileInfo
            Dim tempFolder As New DirectoryInfo(CommonUtil.GetInstance.AppTempPath)
            Try
                file = ReslRegistroGiornalieroPrinter.CreatePrint(finder, tempFolder, names, rblTipologia.SelectedItem.Text)

                ' Demando la visualizzazione del report generato al nuovo visualizzatore light
                Dim querystring As String = String.Format("DataSourceType=Resl&DownloadFile={0}", file.Name)
                Dim temp As String = String.Format("{0}/viewers/TempFileViewer.aspx?{1}", DocSuiteContext.Current.CurrentTenant.DSWUrl, CommonShared.AppendSecurityCheck(querystring))
                Response.Redirect(temp, False)
                ' Per evitare ThreadAbortException
                Return
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
                AjaxAlert(ex.Message)
            End Try

        Else
            AjaxAlert("Campo contenitore obbligatorio")
        End If

    End Sub

End Class