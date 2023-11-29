Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class ReslRicercaFlusso
    Inherits ReslBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Title = String.Format("{0} - Ricerca Flusso", Facade.TabMasterFacade.TreeViewCaption)
        btnSearch.Focus()
    End Sub

    Private Sub SearchClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        Search()
    End Sub

#End Region

#Region " Methods "

    Private Sub Search()
        Dim finder As NHibernateResolutionFinder = CType(uscResWorkflowFinder.Finder, NHibernateResolutionFinder)
        If (CommonInstance.ApplyResolutionFinderSecurity(Me, finder, CurrentTenant.TenantAOO.UniqueId, fromRicercaFlusso:=ResolutionEnv.Configuration.Eq(ConfTo))) Then
            'Setto le proprietà del controllo in sessione
            Session.Add("CurrentStep", uscResWorkflowFinder.MyStep)
            Session.Add("CurrentTipo", uscResWorkflowFinder.Tipologia)
            Session.Add("PublishingDate", uscResWorkflowFinder.PubDate)
            Session.Add("CollegioWarningDate", uscResWorkflowFinder.CollWarningDate)
            Session.Add("TextProtocollo", uscResWorkflowFinder.TextProtocollo)
            Session.Add("CheckOmissis", uscResWorkflowFinder.CheckOmissis)
        End If

        If ResolutionEnv.SearchMaxRecords > 0 Then
            uscResWorkflowFinder.PageSize = ResolutionEnv.SearchMaxRecords
        End If

        SessionSearchController.SaveSessionFinder(finder, SessionSearchController.SessionFinderType.ReslFlussoFinderType)
        ClearSessions(Of ReslRisultatiFlusso)()
        Response.Redirect("../Resl/ReslRisultatiFlusso.aspx?Type=Resl&Action=" & Action)
    End Sub

#End Region

End Class