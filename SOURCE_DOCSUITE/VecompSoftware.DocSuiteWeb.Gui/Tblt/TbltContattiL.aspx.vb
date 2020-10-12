Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class TbltContattiL
    Inherits CommonBasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub Initialize()
        ' TODO: sostituire con tutti i tipi configurati, basta usare Facade.ContactTypeFacade.GetAll()
        For Each contactTypeId As Char In {ContactType.Administration, ContactType.Group, ContactType.Aoo, ContactType.OrganizationUnit, ContactType.Role, ContactType.Person}
            Dim image As New Image()
            image.ImageUrl = ImagePath.ContactTypeIcon(contactTypeId)
            tblLegenda.Rows.AddRaw(Nothing, Nothing, Nothing, {10, 90}, {image, New LiteralControl(Facade.ContactTypeFacade.LegacyDescription(contactTypeId))}, Nothing)
        Next
    End Sub

End Class


