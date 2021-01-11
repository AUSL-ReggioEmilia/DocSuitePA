Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging

Public Class uscContainerConstraintOptions
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Public Const LOAD_CONSTRAINTS As String = "loadConstraints({0})"
#End Region

#Region " Properties "

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

        End If
    End Sub
#End Region

#Region " Methods "
    Public Sub LoadConstraints(idContainer As Integer)
        Dim series As DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentByContainerId(idContainer)
        If series Is Nothing Then
            FileLogger.Error(LoggerName, String.Concat("Nessuna serie documentale trovata con idcontainer ", idContainer))
            BasePage.AjaxAlert("Errore in inizializzazione obblighi trasparenza per il contenitore corrente")
            Return
        End If
        AjaxManager.ResponseScripts.Add(String.Format(LOAD_CONSTRAINTS, series.Id))
    End Sub
#End Region

End Class