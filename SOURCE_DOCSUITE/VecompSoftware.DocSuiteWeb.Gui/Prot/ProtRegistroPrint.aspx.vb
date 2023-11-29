Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class ProtRegistroPrint
    Inherits ProtBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeView1)
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub cmdStampa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdStampa.Click
        StampaRegistro()
    End Sub

    Protected Sub btnSelectAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll.Click
        SelectOrDeselectAll(True, RadTreeView1)
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDeselectAll.Click
        SelectOrDeselectAll(False, RadTreeView1)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, RadTreeView1, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, RadTreeView1, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdStampa, cmdStampa, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub Initialize()

        Dim root As RadTreeNode = New RadTreeNode
        root.Text = "Contenitori"
        root.Checkable = False
        root.Font.Bold = True
        root.Expanded = True

        If DocSuiteContext.Current.ProtocolEnv.IsSecurityEnabled Then
            Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True)
            If containers.IsNullOrEmpty() Then
                Throw New DocSuiteException("Registro Giornaliero", "Nessun contenitore abilitato.")
            End If

            For Each container As Container In containers
                Dim node As New RadTreeNode
                node.Text = container.Name
                node.Value = container.Id.ToString()
                node.ImageUrl = ImagePath.SmallBoxOpen
                node.Expanded = True
                root.Nodes.Add(node)
            Next
        Else
            Dim containers As IList(Of ContainerRightsDto) = Facade.ContainerFacade.GetAllRights("Prot", True)
            If containers Is Nothing OrElse containers.Count = 0 Then
                Throw New DocSuiteException("Registro Giornaliero", "Nessun contenitore abilitato.")
            End If

            For Each dto As ContainerRightsDto In containers
                Dim node As New RadTreeNode
                node.Text = dto.Name
                node.Value = CType(dto.ContainerId, String)
                node.ImageUrl = ImagePath.SmallBoxOpen
                node.Expanded = True
                root.Nodes.Add(node)
            Next
        End If


        RadTreeView1.Focus()
        RadTreeView1.Nodes.Clear()
        RadTreeView1.Nodes.Add(root)
        Page.Form.DefaultButton = cmdStampa.UniqueID
    End Sub

    Private Function CheckedNode(ByVal tvw As RadTreeView) As Boolean
        Dim b As Boolean = False
        For Each tn As RadTreeNode In tvw.Nodes(0).Nodes
            If tn.Checked = True Then
                b = True
                Exit For
            End If
        Next
        Return b
    End Function

    Private Sub SelectOrDeselectAll(ByVal selected As Boolean, ByRef treeview As RadTreeView)
        If treeview.Nodes.Count > 0 Then
            For Each tn As RadTreeNode In treeview.Nodes(0).Nodes
                tn.Checked = selected
            Next
        End If
    End Sub

    Private Sub StampaRegistro()
        If Not CheckedNode(RadTreeView1) Then
            AjaxAlert("Campo contenitore obbligatorio")
            Exit Sub
        End If

        Dim containerId As String = String.Empty
        Dim names As String = String.Empty
        For Each nodo As RadTreeNode In RadTreeView1.Nodes(0).Nodes
            If Not nodo.Checked Then
                Continue For
            End If

            If String.IsNullOrEmpty(containerId) Then
                containerId = nodo.Value
                names = nodo.Text
            Else
                containerId = containerId & "," & nodo.Value
                names = names & "," & nodo.Text
            End If
        Next

        Dim finder As New NHibernateProtocolFinder("ProtDB")
        finder.IdContainer = containerId
        finder.RegistrationDateFrom = RadDatePicker1.SelectedDate
        finder.RegistrationDateTo = RadDatePicker2.SelectedDate
        Select Case True
            Case rblTipologia.Items(0).Selected
                finder.NoStatus = True
            Case rblTipologia.Items(1).Selected
                finder.IdStatus = ProtocolStatusId.Attivo
            Case rblTipologia.Items(2).Selected
                finder.IdStatus = ProtocolStatusId.Annullato
            Case rblTipologia.Items(3).Selected
                finder.IdStatus = ProtocolStatusId.Errato
        End Select
        finder.EnablePaging = False
        finder.SortExpressions.Add("RegistrationDate", "ASC")

        If ProtocolEnv.PdfPrint() Then
            Dim journalPrintPdf As New ProtJournalPrintPdf()
            journalPrintPdf.ContainersName = names
            journalPrintPdf.Finder = finder
            journalPrintPdf.TitlePrint = "Stampa registro giornaliero - " & rblTipologia.SelectedItem.Text
            Session("Printer") = journalPrintPdf

        Else
            Dim journalPrint As New ProtJournalPrint()
            journalPrint.IdContainers = containerId
            journalPrint.ContainersName = names
            journalPrint.RegistrationDateFrom = RadDatePicker1.SelectedDate
            journalPrint.RegistrationDateTo = RadDatePicker2.SelectedDate

            If finder.IdStatus.HasValue Then
                journalPrint.IdStatus = finder.IdStatus.Value
            End If

            journalPrint.TitlePrint = "Stampa registro giornaliero - " & rblTipologia.SelectedItem.Text
            Session("Printer") = journalPrint

        End If

        Response.Redirect("..\Comm\CommPrint.aspx?Type=Prot&&PrintName=ProtJournalPrint")

    End Sub

#End Region

End Class
