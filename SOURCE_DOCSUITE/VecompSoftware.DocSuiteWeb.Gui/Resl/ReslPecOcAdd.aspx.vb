Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslPecOcAdd
    Inherits ReslBasePage

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjaxSetting()
        If Not Page.IsPostBack Then
            initialize()
        End If

    End Sub


    Protected Sub ReslType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ReslType.SelectedIndexChanged
        SetCurrentResolutionType()
    End Sub

    Protected Sub Conferma_Click(sender As Object, e As EventArgs) Handles Conferma.Click
        Save()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSetting()
        AjaxManager.AjaxSettings.AddAjaxSetting(ReslType, dataPanel)
    End Sub

    Private Sub initialize()
        Me.Title = "Inserimento richiesta PEC"

        ' carico tutti i tipi di atti
        ReslType.DataValueField = "ID"
        ReslType.DataTextField = "Description"
        ReslType.DataSource = Facade.ResolutionTypeFacade.GetAll()
        ReslType.DataBind()

        SetCurrentResolutionType()
    End Sub

    Private Sub SetCurrentResolutionType()
        If ReslType.SelectedValue = ResolutionType.IdentifierDelibera Then
            ' il from è l'unica data richiesta
            From.Text = "Data:"
            DateToRow.Visible = False
            rowExtractOneDay.Visible = True
        Else
            ' from -> to
            From.Text = "Dalla data:"
            [To].Text = "Alla data:"
            DateToRow.Visible = True
            rowExtractOneDay.Visible = False
        End If
    End Sub

    Private Sub Save()
        If DateFrom.SelectedDate Is Nothing OrElse (DateToRow.Visible AndAlso DateTo.SelectedDate Is Nothing) Then
            AjaxAlert("Errore inserimento.")
            Exit Sub
        End If

        Dim newPecOc As PECOC = New PECOC()
        newPecOc.FromDate = DateFrom.SelectedDate.Value

        If rowExtractOneDay.Visible AndAlso chkExtractOneDay.Checked Then
            newPecOc.ToDate = DateFrom.SelectedDate.Value.AddDays(1).AddSeconds(-1)
        End If

        If ReslType.SelectedValue = ResolutionType.IdentifierDetermina AndAlso DateTo.SelectedDate.HasValue Then
            newPecOc.ToDate = DateTo.SelectedDate.Value.AddDays(1).AddSeconds(-1)
        End If

        newPecOc.ResolutionType = Facade.ResolutionTypeFacade.GetById(ReslType.SelectedValue)
        newPecOc.Status = PECOCStatus.Aggiunto
        newPecOc.ExtractAttachments = ExtractAttachment.Checked
        newPecOc.IsActive = True

        Facade.PECOCFacade.Save(newPecOc)
        Facade.PECOCLogFacade.InsertLog(newPecOc)

        AjaxManager.ResponseScripts.Add(String.Format("location.href = '{0}/Resl/ReslPecOc.aspx?Type=Resl'", DocSuiteContext.Current.CurrentTenant.DSWUrl))
    End Sub

#End Region

End Class