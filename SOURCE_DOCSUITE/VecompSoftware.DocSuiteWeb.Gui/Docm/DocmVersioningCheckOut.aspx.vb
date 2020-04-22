Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocmVersioningCheckOut
    Inherits DocmBasePage

    Private Const IncrementalObject As Short = 0

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(gvDocuments, gvDocuments, MasterDocSuite.AjaxLoadingPanelSearch)
    End Sub

    Private Sub Initialize()
        Dim datasrc As IList(Of DocumentVersioning) = Facade.DocumentVersioningFacade.GetDocumentVersionAll(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject, "O")

        If (datasrc Is Nothing) Then
            Response.End()
            Exit Sub
        End If

        Dim finder As NHibernateDocumentVersioningFinder = Facade.DocumentVersioningFinder()
        finder.DocumentYear = CurrentDocumentYear
        finder.DocumentNumber = CurrentDocumentNumber
        finder.DocumentIncr = IncrementalObject
        finder.DocumentCheckStatus = "O"
        finder.TypeSearch = 1

        gvDocuments.Finder = finder
        gvDocuments.VirtualItemCount = datasrc.Count
        gvDocuments.DataBindFinder()
    End Sub

    Public Function SetupFolder(ByVal IncrementalFolder As Integer) As String
        Dim Titolo As String = String.Empty
        DocUtil.FncCalcolaCartella(CurrentDocumentYear, CurrentDocumentNumber, Titolo, IncrementalFolder)
        Return titolo

    End Function

End Class
