
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslPecOc
    Inherits ReslBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not Page.IsPostBack Then
            Title = "PEC a Collegio Sindacale"

            If Not CommonShared.ResolutionPECOCEnabled Then
                Throw New DocSuiteException("Errore abilitazioni", "La pagina non è abilitata, controllare funzionamento PEC e diritti.")
            End If
            Search()
        End If
    End Sub

    Protected Sub Nuovo_Click(sender As Object, e As EventArgs) Handles Nuovo.Click
        Response.Redirect("ReslPecOcAdd.aspx?Type=Resl")
    End Sub

    Protected Sub PecOcGrid_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles PecOcGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If
        
        Dim pecOc As PECOC = DirectCast(e.Item.DataItem, PECOC)

        ' Se esiste la mail apro il mailViewer
        With DirectCast(e.Item.FindControl("allegato"), RadButton)
            .Visible = pecOc.IdMail.HasValue

            If pecOc.IdMail.HasValue Then
                .OnClientClicked = "OpenViewer"
                .CommandArgument = pecOc.IdMail.ToString()
                If pecOc.Status <> PECOCStatus.Spedito Then
                    .Image.ImageUrl = "../App_Themes/DocSuite2008/imgset16/mail.png"
                    .ToolTip = "Visualizza la mail da spedire"
                Else
                    .Image.ImageUrl = "../App_Themes/DocSuite2008/imgset16/sendEmail.png"
                    .ToolTip = "Visualizza la mail spedita"
                End If
            End If
        End With

        With DirectCast(e.Item.FindControl("date"), LinkButton)
            If pecOc.ToDate.HasValue Then
                .Text = String.Format("Dal {0} al {1}", pecOc.FromDate.ToString("dd/MM/yyyy"), pecOc.ToDate.Value.ToString("dd/MM/yyyy"))
            Else
                .Text = pecOc.FromDate.ToString("dd/MM/yyyy")
            End If
            If pecOc.Status = PECOCStatus.Completo Then
                .Text &= " (verifica spedizione)"
            End If

            .PostBackUrl = String.Format("../Resl/ReslPecOcSummary.aspx?PECOC={0}&Type=Resl", pecOc.Id)
        End With

        With DirectCast(e.Item.FindControl("extractAttachments"), Label)
            .Text = If(pecOc.ExtractAttachments, "Si", "No")
        End With

        With DirectCast(e.Item.FindControl("stato"), Label)
            .Text = [Enum].GetName(GetType(PECOCStatus), pecOc.Status)
        End With
    End Sub

    Private Sub PecOcGrid_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Search()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(PecOcGrid, PecOcGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, PecOcGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf PecOcGrid_AjaxRequest
    End Sub

    Private Sub Search()
        If ResolutionEnv.SearchMaxRecords <> 0 Then
            PecOcGrid.PageSize = ResolutionEnv.SearchMaxRecords
        End If

        Dim finder As NHibernatePECOCFinder = Facade.PECOCFinder

        PecOcGrid.Finder = finder
        PecOcGrid.DataBindFinder()
        ' Imposto titolo a seconda dei risultati della ricerca
        If PecOcGrid.DataSource IsNot Nothing Then
            lblHeader.Text = String.Format(" Registri - Risultati ({0}/{1})", PecOcGrid.DataSource.Count, PecOcGrid.VirtualItemCount)
        Else
            lblHeader.Text = " Registri - Nessun Risultato"
        End If
    End Sub

    Protected Function WindowWidth() As Integer
        Return DocSuiteContext.Current.ProtocolEnv.PECWindowWidth
    End Function

    Protected Function WindowHeight() As Integer
        Return DocSuiteContext.Current.ProtocolEnv.PECWindowHeight
    End Function

    Protected Function WindowBehaviors() As String
        Return DocSuiteContext.Current.ProtocolEnv.PECWindowBehaviors
    End Function

    Protected Function WindowPosition() As String
        Return DocSuiteContext.Current.ProtocolEnv.PECWindowPosition
    End Function

#End Region

End Class