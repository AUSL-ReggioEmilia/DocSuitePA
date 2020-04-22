Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Logging

Public Class DocmModifica
    Inherits DocmBasePage

#Region " Properties "

    Public ReadOnly Property Publication() As Boolean
        Get
            Return Request.QueryString("Publication") = "1"
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        uscDocumentData.CurrentDocument = CurrentDocument
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        If Publication Then
            CurrentDocument.CheckPublication = If(uscDocumentData.GetCheckPubblication, "1", Nothing)
        Else
            CurrentDocument.StartDate = uscDocumentData.GetStartDate
            CurrentDocument.ExpiryDate = uscDocumentData.GetExpiryDate

            Facade.DocumentFacade.RemoveDocumentContact(CurrentDocument)

            For Each cntDTO As ContactDTO In uscDocumentData.GetContacts
                Facade.DocumentFacade.AddDocumentContact(CurrentDocument, cntDTO.Contact)
            Next

            CurrentDocument.Name = uscDocumentData.GetName
            CurrentDocument.ServiceNumber = uscDocumentData.GetServiceNumber
            CurrentDocument.DocumentObject = uscDocumentData.GetDocumentObject
            CurrentDocument.Manager = uscDocumentData.GetManager
            CurrentDocument.Note = uscDocumentData.GetNote
            CurrentDocument.SubCategory = uscDocumentData.GetSottoClassificazione
            CurrentDocument.CheckPublication = If(uscDocumentData.GetCheckPubblication, "1", Nothing)
        End If
        Try
            Facade.DocumentFacade.Update(CurrentDocument)

            Response.Redirect("../Docm/DocmInfo.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}&Type=Docm", CurrentDocument.Year, CurrentDocument.Number)))
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in modifica della pratica.", ex)
            AjaxAlert("Errore durante la modifica della pratica: " & StringHelper.ReplaceAlert(ex.Message))
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        uscDocumentData.Show()
        If Publication Then
            uscDocumentData.VisiblePubblicazioneModifica = True
            uscDocumentData.VisibleGenerale = True
            uscDocumentData.VisiblePratica = True
            uscDocumentData.VisibleDateSovrapposte = True
            uscDocumentData.VisibleDati = False
            uscDocumentData.VisibleDatiModifica = False
            uscDocumentData.VisibleClassificatoreModifica = False
            uscDocumentData.VisibleContatti = False
            uscDocumentData.VisibleDateModifica = False
            uscDocumentData.VisibleDate = False
            uscDocumentData.VisibleAltri = False
            uscDocumentData.VisiblePubblicazione = False
            uscDocumentData.LoadStatus()
        Else
            uscDocumentData.SetLayoutPerModifica()
        End If
    End Sub

#End Region

End Class

