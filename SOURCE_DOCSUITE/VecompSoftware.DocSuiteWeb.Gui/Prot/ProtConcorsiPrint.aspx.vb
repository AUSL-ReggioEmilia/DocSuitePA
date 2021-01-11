Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ProtConcorsiPrint
    Inherits ProtBasePage

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        SetResponseNoCache()
        If ProtocolEnv.UseConcourseWithGrid Then
            trOrdinamento.Visible = False
            btnSearch.Visible = False
            btnSearchGrid.Visible = True
            If Not UscClassificatore1.SelectedCategories.IsNullOrEmpty Then
                btnSearchGrid.PostBackUrl = String.Format("{0}&ConcourseId={1}", btnSearchGrid.PostBackUrl, UscClassificatore1.SelectedCategories.First().Id)
            End If
        Else
            trOrdinamento.Visible = True
            btnSearch.Visible = True
            btnSearchGrid.Visible = False
        End If
    End Sub

    Protected Sub BtnSearchClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSearch.Click
        'If ProtocolEnv.UseConcourseWithGrid Then
        '    RicercaConcorsiFinder()
        'Else
        RicercaElenco()
        'End If
    End Sub

    Private Sub RicercaElenco()
        Dim print As New ProtConcoursePrint()
        print.DateFrom = RadDatePicker1.SelectedDate
        print.DateTo = RadDatePicker2.SelectedDate

        If UscClassificatore1.HasSelectedCategories Then
            print.CategoryPath = UscClassificatore1.SelectedCategories.First().FullIncrementalPath
        End If

        Select Case rblTipologia.SelectedValue
            Case "P"
                print.OrderClause = NHibernateProtocolDao.ConcourseOrder.ProtocolNumber
                print.OrderText = "Ordinato per numero protocollo"
            Case "A"
                print.OrderClause = NHibernateProtocolDao.ConcourseOrder.Alphabetic
                print.OrderText = "Ordinamento alfabetico"
        End Select

        Session("Printer") = print
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Comm&PrintName=ProtConcoursePrint")
    End Sub

    Public Function RicercaConcorsiFinder() As NHibernateProtocolFinder
        Dim finder As New NHibernateProtocolFinder("ProtDB")
        finder.RegistrationDateFrom = RadDatePicker1.SelectedDate
        finder.RegistrationDateTo = RadDatePicker2.SelectedDate
        finder.Classifications = UscClassificatore1.SelectedCategories.First().FullIncrementalPath
        ''Ordinamento statico per protocollo
        finder.SortExpressions.Add("Id", "ASC")
        Return finder
    End Function

    Public Function GetConcourseName() As String
        Return UscClassificatore1.SelectedCategories.First().Name
    End Function
End Class