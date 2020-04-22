Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class ReslDuplica
    Inherits ReslBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim s As String = "0000000000"
        SetCheck(s, CShort(cbTipologia.Attributes("Value")), cbTipologia.Checked)
        SetCheck(s, CShort(cbContenitore.Attributes("Value")), cbContenitore.Checked)
        SetCheck(s, CShort(cbDestinatari.Attributes("Value")), cbDestinatari.Checked)
        SetCheck(s, CShort(cbProponente.Attributes("Value")), cbProponente.Checked)
        SetCheck(s, CShort(cbOggetto.Attributes("Value")), cbOggetto.Checked)
        SetCheck(s, CShort(cbNote.Attributes("Value")), cbNote.Checked)
        If pnlCategory.Visible Then SetCheck(s, CShort(cbClassificazione.Attributes("Value")), cbClassificazione.Checked)
        If pnlAssegnatario.Visible Then SetCheck(s, CShort(cbAssegnatario.Attributes("Value")), cbAssegnatario.Checked)
        If pnlResponsabile.Visible Then SetCheck(s, CShort(cbResponsabile.Attributes("Value")), cbResponsabile.Checked)

        Me.MasterDocSuite.AjaxManager.ResponseScripts.Add("CloseWindow('" & s & "');")
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Title = Facade.TabMasterFacade.TreeViewCaption & " Selezione dati duplicazione"
        pnlCategory.Visible = Facade.ResolutionFacade.IsManagedProperty("Category", ResolutionType.IdentifierDelibera)
        pnlAssegnatario.Visible = Facade.ResolutionFacade.IsManagedProperty("Assignee", ResolutionType.IdentifierDelibera, "IN")
        pnlResponsabile.Visible = Facade.ResolutionFacade.IsManagedProperty("Manager", ResolutionType.IdentifierDetermina, "IN")
        cbTipologia.Attributes("Value") = ResolutionDuplicaOption.Type
        cbContenitore.Attributes("Value") = ResolutionDuplicaOption.Container
        cbDestinatari.Attributes("Value") = ResolutionDuplicaOption.Recipients
        cbProponente.Attributes("Value") = ResolutionDuplicaOption.Proposer
        cbAssegnatario.Attributes("Value") = ResolutionDuplicaOption.Assignee
        cbResponsabile.Attributes("Value") = ResolutionDuplicaOption.Manager
        cbOggetto.Attributes("Value") = ResolutionDuplicaOption.Object
        cbNote.Attributes("Value") = ResolutionDuplicaOption.Note
        cbClassificazione.Attributes("Value") = ResolutionDuplicaOption.Category
    End Sub

    Private Sub SetCheck(ByRef field As String, ByVal right As Short, ByVal value As Boolean)
        If value Then
            Mid$(field, right, 1) = "1"
        Else
            Mid$(field, right, 1) = "0"
        End If
    End Sub

#End Region

End Class