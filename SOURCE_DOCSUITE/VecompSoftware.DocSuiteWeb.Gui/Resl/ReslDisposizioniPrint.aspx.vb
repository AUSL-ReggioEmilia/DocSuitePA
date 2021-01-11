Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ReslDisposizioniPrint
    Inherits ReslBasePage

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not CommonUtil.VerifyChkQueryString(Request.QueryString, True) Then
            Exit Sub
        End If

        If Not Page.IsPostBack Then
            Inizializza()
        End If
    End Sub

    Protected Sub cmdStampa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdStampa.Click
        StampaRegistro()
    End Sub

#End Region

#Region " Methods "

    Private Sub Inizializza()
        Me.Title = "Elenco delle Disposizioni Dirigenziali"
    End Sub

    Private Sub StampaRegistro()
        Dim finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB")
        Dim journalPrint As ReslDisposPrint = New ReslDisposPrint()

        finder.Determina = True
        finder.Adottata = True
        finder.DateFrom = RadDatePicker1.SelectedDate
        finder.DateTo = RadDatePicker2.SelectedDate

        finder.EnablePaging = False
        finder.IsPrint = True

        journalPrint.Finder = finder
        journalPrint.TitlePrint = "Elenco delle Disposizioni Dirigenziali"
        Session("Printer") = journalPrint

        'redirect
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Resl&PrintName=ReslDisposPrint")

    End Sub

#End Region

End Class