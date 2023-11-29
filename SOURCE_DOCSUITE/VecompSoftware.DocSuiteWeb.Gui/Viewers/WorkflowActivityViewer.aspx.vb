Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports VecompSoftware.Services.Biblos.Models

Public Class WorkflowActivityViewer
    Inherits CommonBasePage
    Private Const _tollerance As Integer = 3000
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
        Dim requestFolder As FolderInfo = New FolderInfo With {
            .Name = "Documento originale",
            .Parent = mainFolder
        }
        Dim responseFolder As FolderInfo = New FolderInfo With {
            .Name = "Risposta",
            .Parent = mainFolder
        }

        Dim docs As IEnumerable(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(CurrentIdArchiveChain).OrderBy(Function(f) f.DateCreated.Value)
        Dim lastDate As Date = docs.First().DateCreated.Value
        Dim currentFolder As FolderInfo = requestFolder
        For Each docInfo As BiblosDocumentInfo In docs
            docInfo.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, True.ToString())
            docInfo.AddAttribute(DocumentUnitChainFacade.BIBLOS_ATTRIBUTE_Miscellanea, True.ToString())
            If (docInfo.DateCreated.Value - lastDate).TotalMilliseconds > _tollerance Then
                currentFolder = responseFolder
            End If
            currentFolder.AddChild(docInfo)
        Next
        uscDocument.DataSource = New List(Of DocumentInfo) From {mainFolder}
        uscDocument.DataBind()
    End Sub
End Class