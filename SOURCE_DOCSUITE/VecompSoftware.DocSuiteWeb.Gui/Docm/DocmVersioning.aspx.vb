Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Services.Biblos.Models

Public Class DocmVersioning
    Inherits DocmBasePage

#Region " Property "

    Public ReadOnly Property IncrementalObject As Short
        Get
            Return Short.Parse(Request.QueryString("IncrementalObject"))
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub RadGrid11_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles RadGrid11.ItemCommand
        If Not e.CommandName.Eq("show") Then
            Exit Sub
        End If

        Dim year As Short = DirectCast(e.Item.OwnerTableView.DataKeyValues(e.Item.ItemIndex)("Year"), Short)
        Dim number As Integer = DirectCast(e.Item.OwnerTableView.DataKeyValues(e.Item.ItemIndex)("Number"), Integer)
        Dim incremental As Short = DirectCast(e.Item.OwnerTableView.DataKeyValues(e.Item.ItemIndex)("Incremental"), Short)

        Dim document As Document = Facade.DocumentFacade.GetById(year, number)
        Dim documentObject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(year, number, incremental))

        Dim documentInfo As New BiblosDocumentInfo(document.Location.DocumentServer, document.Location.DocmBiblosDSDB, documentObject.idBiblos)

        Dim parameters As String = String.Format("servername={0}&guid={1}&label={2}", documentInfo.Server, documentInfo.ChainId, "Documenti")
        Response.Redirect("~/Viewers/BiblosViewer.aspx?" & CommonShared.AppendSecurityCheck(parameters))
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Dim docObj As IList(Of DocumentObject) = Facade.DocumentObjectFacade.GetVersionedDocumentObjects(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject)
        If docObj.IsNullOrEmpty() Then
            AjaxAlert("Nessun documento versionato trovato")
            Exit Sub
        End If

        Dim finder As NHibernateDocumentObjectFinder = Facade.DocumentObjectFinder()
        finder.DocumentYear = CurrentDocumentYear
        finder.DocumentNumber = CurrentDocumentNumber
        finder.DocumentIncr = IncrementalObject
        finder.TypeSearch = 2

        RadGrid11.MasterTableView.DataKeyNames = {"Year", "Number", "Incremental"}
        RadGrid11.Finder = finder
        RadGrid11.VirtualItemCount = docObj.Count
        RadGrid11.DataBindFinder()
    End Sub

#End Region

End Class
