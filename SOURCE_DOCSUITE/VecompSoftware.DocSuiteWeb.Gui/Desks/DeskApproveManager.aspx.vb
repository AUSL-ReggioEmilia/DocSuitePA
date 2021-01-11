Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Desks
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks

Public Class DeskApproveManager
    Inherits DeskBasePage

#Region "Fields"
    Private Const PAGE_TITLE As String = "Tavoli - Controllo approvazioni"
    Private Const DESK_SUMMARY_PATH As String = "~/Desks/DeskSummary.aspx?Type=Desk&Action=View&DeskId={0}"
#End Region

#Region "Properties"

    Private Property CurrentDeskEndorments As List(Of DeskEndorsement)
        Get
            If ViewState("CurrentDeskEndorments") Is Nothing Then
                ViewState("CurrentDeskEndorments") = New List(Of DeskEndorsement)()
            End If
            Return DirectCast(ViewState("CurrentDeskEndorments"), List(Of DeskEndorsement))
        End Get
        Set(value As List(Of DeskEndorsement))
            ViewState("CurrentDeskEndorments") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
        dgvApproveManagerGrid.DataSource = CurrentDeskEndorments
    End Sub

    Protected Sub dgvApproveManagerGrid_Item(sender As Object, e As PivotGridCellDataBoundEventArgs) Handles dgvApproveManagerGrid.CellDataBound
        If TypeOf e.Cell Is PivotGridDataCell Then
            Dim cell As PivotGridDataCell = TryCast(e.Cell, PivotGridDataCell)
            If cell.CellType = PivotGridDataCellType.DataCell Then
                Dim image As Image = TryCast(cell.FindControl("imgApproval"), Image)
                If image IsNot Nothing Then
                    ' Inserisco l'icona di approvazione solamente se l'utente approva o rifiuta un documento. 
                    If cell.DataItem IsNot Nothing And cell.DataItem IsNot String.Empty Then
                        image.ImageUrl = GetImageFromValue(cell.DataItem)
                        cell.HorizontalAlign = HorizontalAlign.Center
                        cell.VerticalAlign = VerticalAlign.Middle
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub btnCloseDesk_Click() Handles btnCloseDesk.Click
        CurrentDesk.Status = DeskState.Closed
        CurrentDeskFacade.Update(CurrentDesk)
        Response.Redirect(String.Format(DESK_SUMMARY_PATH, CurrentDesk.Id))
    End Sub
#End Region

#Region "Methods"

    Private Function GetImageFromValue(approval As Integer) As String
        If approval = 1 Then
            Return "../App_Themes/DocSuite2008/imgset16/accept.png"
        ElseIf approval = 0 Then
            Return "../App_Themes/DocSuite2008/imgset16/remove.png"
        End If
    End Function

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCloseDesk, btnCloseDesk)
    End Sub

    Private Sub Initialize()
        Title = PAGE_TITLE
        BindApproveData()
    End Sub

    Private Sub BindApproveData()
        Dim baseFinder As DeskApprovalEndorsementFinder = New DeskApprovalEndorsementFinder(New MapperApprovalEndorsement())
        baseFinder.DeskId = CurrentDesk.Id

        Dim docEndorsements As List(Of DeskEndorsement) = New List(Of DeskEndorsement)
        Dim documentNameList As List(Of DeskEndorsement) = New List(Of DeskEndorsement)

        'Reperisco il nome del documento una sola volta 
        CurrentDeskEndorments = baseFinder.DoSearchHeader().ToList()
        For Each deskEndorsement As DeskEndorsement In CurrentDeskEndorments
            If documentNameList.Where(Function(x) x.IdChainBiblos = deskEndorsement.IdChainBiblos).Count() = 0 Then
                Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(deskEndorsement.IdChainBiblos.Value)
                If Not docInfos.Any() Then
                    Exit For
                End If
                Dim docInfo As BiblosDocumentInfo = docInfos.OrderByDescending(Function(f) f.Version).First()
                deskEndorsement.DeskDocumentName = docInfo.Name()
                documentNameList.Add(deskEndorsement)
            End If
        Next
        ' Per ogni Iddocument setto il nome reperito in precedenza da biblos
        For Each deskEndorsement As DeskEndorsement In CurrentDeskEndorments
            deskEndorsement.DeskDocumentName = documentNameList.Where(Function(x) x.IdChainBiblos = deskEndorsement.IdChainBiblos).Select(Function(z) z.DeskDocumentName).FirstOrDefault()
            docEndorsements.Add(deskEndorsement)
        Next
        CurrentDeskEndorments = docEndorsements
    End Sub

#End Region

End Class