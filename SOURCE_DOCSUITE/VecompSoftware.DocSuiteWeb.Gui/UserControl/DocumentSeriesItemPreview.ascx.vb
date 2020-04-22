
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocumentSeriesItemPreview
    Inherits DocSuite2008BaseControl

    Public Property Item As DocumentSeriesItem

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        DocumentSeries.Text = "Registrazione in " & ProtocolEnv.DocumentSeriesName
    End Sub

    Public Sub Show()
        If Item.Year.HasValue AndAlso Item.Number.HasValue Then
            lblStatus.Text = "Registrazione:"
            lblId.Text = String.Format("{0}/{1:000000} del {2:dd/MM/yyyy}", Item.Year, Item.Number, Item.RegistrationDate)
        Else
            lblStatus.Text = "Bozza:"
            lblId.Text = String.Format("{0} del {1:dd/MM/yyyy}", Item.Id, Item.RegistrationDate)
        End If

        ' Numero e data
        lblDocumentSeries.Text = Item.DocumentSeries.Container.Name
        lblObject.Text = Item.Subject

        ' Descrizione categoria
        lblCategoryDescription.Text = Facade.CategoryFacade.GetFullIncrementalName(Item.Category)

        tblPreview.Style.Remove("Display")
    End Sub
End Class