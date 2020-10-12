Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports VecompSoftware.Services.Biblos.Models

Public Class PosteWebActivityViewer
    Inherits CommonBasePage
    Private Const _tollerance As Integer = 10000
    Protected ReadOnly Property CurrentIdArchiveChain As Guid
        Get
            Return Guid.Parse(Request.QueryString("IdArchiveChain"))
        End Get
    End Property

    Protected ReadOnly Property CurrentTitle As String
        Get
            Return Request.QueryString("Title")
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles uscDocument.Load
        Dim mainFolder As FolderInfo = New FolderInfo With {
            .Name = CurrentTitle
        }
        Dim folder As FolderInfo = New FolderInfo With {
            .Name = "Ricevute TNotice",
            .Parent = mainFolder
        }

        Dim docs As IEnumerable(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(CurrentIdArchiveChain).OrderBy(Function(f) f.DateCreated.Value)

        Dim lastDate As Date = docs.First().DateCreated.Value
        Dim currentFolder As FolderInfo = folder
        For Each docInfo As BiblosDocumentInfo In docs
            docInfo.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, True.ToString())
            docInfo.AddAttribute(DocumentUnitChainFacade.BIBLOS_ATTRIBUTE_Miscellanea, True.ToString())
            folder.AddChild(docInfo)
        Next
        uscDocument.DataSource = New List(Of DocumentInfo) From {mainFolder}
        uscDocument.DataBind()
    End Sub

End Class