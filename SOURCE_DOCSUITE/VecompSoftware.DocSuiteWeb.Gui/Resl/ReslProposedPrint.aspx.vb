Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Partial Public Class ReslProposedPrint
    Inherits ReslBasePage

#Region " Properties "

    Private ReadOnly Property CurrentTypeSelected As Short
        Get
            Return Convert.ToInt16(rblTipologia.SelectedValue)
        End Get
    End Property

    Public Property CurrentPrinterSession As Object
        Get
            Return Session("Printer")
        End Get
        Set(value As Object)
            Session("Printer") = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        CommonInstance.UserDeleteTemp(TempType.P)

        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub rblTipologia_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles rblTipologia.SelectedIndexChanged
        UpdateTipologia()
    End Sub
    Private Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSelectAll.Click
        SelectOrDeselectAll(True)
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnDeselectAll.Click
        SelectOrDeselectAll(False)
    End Sub

    Private Sub cmdStampa_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdStampa.Click
        CommonInstance.UserDeleteTemp(TempType.P)
        If Not CheckedNode(tvwContenitore) Then
            AjaxAlert("Campo contenitore obbligatorio.")
            Exit Sub
        End If
        StampaElenco()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        With AjaxManager.AjaxSettings
            .AddAjaxSetting(rblTipologia, pnlHeaders)
            .AddAjaxSetting(cmdStampa, pnlHeaders)
            .AddAjaxSetting(btnSelectAll, pnlHeaders)
            .AddAjaxSetting(btnDeselectAll, pnlHeaders)

            .AddAjaxSetting(rblTipologia, pnlControls)
            .AddAjaxSetting(cmdStampa, pnlControls)
            .AddAjaxSetting(btnSelectAll, pnlControls)
            .AddAjaxSetting(btnDeselectAll, pnlControls)
        End With
    End Sub

    Private Sub Initialize()
        Title = String.Format("{0} - {1}", Facade.TabMasterFacade.TreeViewCaption, "Stampa Elenco Provvedimenti Proposti Al Collegio Sindacale")
        Dim reslContainers As New List(Of ListItem)
        Dim activeContainers As IList(Of ContainerRightsDto) = Facade.ContainerFacade.GetAllRights("Resl", True)

        For Each container As ContainerRightsDto In activeContainers
            Dim containerName As String = container.Name
            Dim containerId As String = container.ContainerId.ToString()
            reslContainers.Add(New ListItem(containerName, containerId))
        Next

        rblTipologia.Items.Add(New ListItem(Facade.ResolutionTypeFacade.DeliberaCaption, ResolutionType.IdentifierDelibera.ToString()))
        rblTipologia.Items.Add(New ListItem(Facade.ResolutionTypeFacade.DeterminaCaption, ResolutionType.IdentifierDetermina.ToString()))
        rblTipologia.SelectedValue = ResolutionType.IdentifierDelibera.ToString()

        UpdateTipologia()

        If ResolutionEnv.SecurityPrint Then
            ' Solo contenitori con almeno diritto di sommario
            Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Adoption, True)

            reslContainers.Clear()
            For Each container As Container In containers
                reslContainers.Add(New ListItem(container.Name, container.Id.ToString()))
            Next
        End If

        If reslContainers IsNot Nothing AndAlso reslContainers.Count > 0 Then
            tvwContenitore.Nodes.Clear()
            Dim parentNode As New RadTreeNode
            parentNode.Text = "Contenitori"
            parentNode.Expanded = True
            parentNode.Checkable = False
            tvwContenitore.Nodes.Add(parentNode)
            For Each container As ListItem In reslContainers
                Dim childNode As New RadTreeNode
                childNode.Text = container.Text
                childNode.Value = container.Value
                childNode.ImageUrl = ImagePath.SmallBoxOpen
                childNode.Expanded = True
                tvwContenitore.Nodes(0).Nodes.Add(childNode)
            Next
        Else
            cmdStampa.Enabled = False
        End If

    End Sub

    Private Sub VisualizzaCampiRicercaStampaElenco(ByVal selectedType As Short)
        pnlGestione.Visible = True
        pnlSession.Visible = True
        pnlAdottataIntervallo.Visible = True
        pnlContenitoreTvw.Visible = True
    End Sub

    Private Sub SelectOrDeselectAll(ByVal selected As Boolean)
        Dim tn As RadTreeNode
        For Each tn In tvwContenitore.Nodes(0).Nodes
            tn.Checked = selected
        Next
    End Sub

    Private Function CheckedNode(ByVal tvw As RadTreeView) As Boolean
        Dim tn As RadTreeNode
        For Each tn In tvw.Nodes(0).Nodes
            If tn.Checked = True Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub GeneraAdozioneIntervallo(ByRef s As String)
        If AdoptionDate_From.SelectedDate.HasValue And AdoptionDate_To.SelectedDate.HasValue Then
            s = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) >= '" & String.Format("{0:yyyyMMdd}", AdoptionDate_From.SelectedDate) & "' AND " &
             "CONVERT(varchar(8),{alias}.AdoptionDate, 112) <= '" & String.Format("{0:yyyyMMdd}", AdoptionDate_To.SelectedDate) & "'"
        Else
            If AdoptionDate_From.SelectedDate.HasValue Then
                s = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) >= '" & String.Format("{0:yyyyMMdd}", AdoptionDate_From.SelectedDate) & "'"
            End If
            If AdoptionDate_To.SelectedDate.HasValue Then
                s = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) <= '" & String.Format("{0:yyyyMMdd}", AdoptionDate_To.SelectedDate) & "'"
            End If
        End If
    End Sub

    Private Sub StampaElenco(Optional ByVal filterByContainer As Boolean = True)

        Dim finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB")
        'Verifico se visualizzare solo le resolution attive
        If DocSuiteContext.Current.ResolutionEnv.ResolutionJournalShowOnlyActive Then
            finder.IdStatus = 0
        End If

        'Tipologia
        Dim sql As String = String.Empty
        Select Case CurrentTypeSelected
            Case ResolutionType.IdentifierDelibera
                finder.Delibera = True
                finder.Determina = False
            Case ResolutionType.IdentifierDetermina
                finder.Delibera = False
                finder.Determina = True
        End Select
        GeneraAdozioneIntervallo(sql)

        finder.SQLExpressions.Add("AdoptionDate", New SQLExpression(sql))

        ' Contenitori
        Dim sContName As String = String.Empty
        Dim tn As RadTreeNode
        If filterByContainer = True Then
            Dim sID As String = String.Empty
            For Each tn In tvwContenitore.Nodes(0).Nodes
                If tn.Checked Then
                    If sID <> "" Then sID &= ","
                    sID &= tn.Value

                    If sContName <> "" Then sContName &= ", "
                    sContName &= tn.Text

                End If
            Next tn
            finder.ContainerIds = sID
        End If

        'Gestione
        If chkCollegio.Checked = True Then
            finder.OCSupervisoryBoard = True
        End If

        finder.EnablePaging = False
        finder.IsPrint = True
        finder.EnableStatus = False
        finder.HasPublishingDate = False
        finder.Adottata = True
        finder.StepAttivo = True

        If finder.Delibera Then
            Dim elencoPrint As New ReslElencoProposedDelPrint()
            elencoPrint.SessionDate = SessionDate.SelectedDate
            elencoPrint.Finder = finder
            elencoPrint.ContainersName = sContName
            CurrentPrinterSession = elencoPrint
            AjaxManager.ResponseScripts.Add("window.open('../Comm/CommPrint.aspx?Type=Resl&PrintName=ReslElencoProposedDelPrint');")
        Else
            Dim elencoPrint As New ReslElencoProposedDetPrint()
            elencoPrint.SessionDate = SessionDate.SelectedDate
            elencoPrint.Finder = finder
            CurrentPrinterSession = elencoPrint
            AjaxManager.ResponseScripts.Add("window.open('../Comm/CommPrint.aspx?Type=Resl&PrintName=ReslElencoProposedDetPrint');")
        End If

    End Sub

    Private Sub UpdateTipologia()
        VisualizzaCampiRicercaStampaElenco(CurrentTypeSelected)
    End Sub
#End Region

End Class