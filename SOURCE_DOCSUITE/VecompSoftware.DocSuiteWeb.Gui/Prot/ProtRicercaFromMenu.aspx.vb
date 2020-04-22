Imports VecompSoftware.DocSuiteWeb.Facade

Public Class ProtRicercaFromMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _finder As ProtocolTaskHeaderFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ProtFinderType), ProtocolTaskHeaderFinder)
        If _finder IsNot Nothing Then
            SessionSearchController.SaveSessionFinder(Nothing, SessionSearchController.SessionFinderType.ProtFinderType)
        End If
        Response.Redirect("./ProtRicerca.aspx?Type=Prot")
    End Sub

End Class