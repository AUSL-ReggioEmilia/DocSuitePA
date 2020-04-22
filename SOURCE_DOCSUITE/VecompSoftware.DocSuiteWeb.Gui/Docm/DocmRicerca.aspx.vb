Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class DocmRicerca
    Inherits DocmBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()

        'Protocollo
        WebUtils.ObjAttDisplayNone(txtProtYear)
        WebUtils.ObjAttDisplayNone(txtProtNumber)
        'Resolution
        WebUtils.ObjAttDisplayNone(txtReslId)


        'Collegato con Protocollo
        If Not IsNothing(Request.QueryString("ProtYear")) And Not IsNothing(Request.QueryString("ProtNumber")) Then
            txtProtYear.Text = Request.QueryString("ProtYear")
            txtProtNumber.Text = Request.QueryString("ProtNumber")
            btnNuovo.Visible = True
        End If

        'Collegato con Atti e Delibere
        If Not IsNothing(Request.QueryString("ReslId")) Then
            txtReslId.Text = Request.QueryString("ReslId")
            btnNuovo.Visible = True
        End If

        If Not Page.IsPostBack Then
            Page.Form.DefaultButton = btnSearch.UniqueID
        End If

        btnSearch.Focus()
    End Sub

    Protected Sub Search_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSearch.Click
        If Not CommonInstance.ApplyDocumentFinderSecurity(uscDocumentFinder.Finder) Then
            Throw New DocSuiteException("Ricerca Pratiche", "Diritti insufficienti per la ricerca nel modulo Pratiche")
        End If

        If DocumentEnv.SearchMaxRecords <> 0 Then
            uscDocumentFinder.PageSize = DocumentEnv.SearchMaxRecords
        End If

        SessionSearchController.SaveSessionFinder(uscDocumentFinder.Finder, SessionSearchController.SessionFinderType.DocmFinderType)
        ClearSessions(Of DocmRisultati)()

        Dim s As String = "Type=Docm&"
        If txtProtYear.Text <> "" And txtProtNumber.Text <> "" Then
            s &= "ProtYear=" & txtProtYear.Text & "&ProtNumber=" & txtProtNumber.Text
        Else
            s &= "ReslId=" & txtReslId.Text
        End If
        Response.Redirect("../Docm/DocmRisultati.aspx?" & CommonShared.AppendSecurityCheck(s))

    End Sub

    Protected Sub btnNuovo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNuovo.Click
        Dim s As String = "Type=Docm&Action=Insert&"
        If txtProtYear.Text <> "" And txtProtNumber.Text <> "" Then
            s &= "ProtYear=" & txtProtYear.Text & "&ProtNumber=" & txtProtNumber.Text
        Else
            s &= "ReslId=" & txtReslId.Text
        End If
        Response.Redirect("DocmInserimento.aspx?" & CommonShared.AppendSecurityCheck(s))
    End Sub

#End Region

End Class
