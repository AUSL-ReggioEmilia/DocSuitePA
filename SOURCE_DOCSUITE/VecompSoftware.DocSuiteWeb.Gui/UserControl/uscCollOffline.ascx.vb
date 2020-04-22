Imports System.Collections.Generic
Imports System.IO
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class uscCollOffline
    Inherits DocSuite2008BaseControl

#Region "Properties"
    Public Property IdCollaboration() As Integer
        Get
            Return ViewState("IdCollaboration")
        End Get
        Set(ByVal value As Integer)
            ViewState("IdCollaboration") = value
            Initialize()
        End Set
    End Property
#End Region

#Region "Initialize"

    Private Sub Initialize()
        If IdCollaboration = 0 Then
            Exit Sub
        End If

        Dim coll As Collaboration = Facade.CollaborationFacade.GetById(IdCollaboration)
        If coll Is Nothing Then
            BasePage.AjaxAlert("La Registrazione non \è più valida.\n\nAggiornare l\'elenco", False)
            Exit Sub
        End If

        lblTitle.Text = "Collaborazione n. " & IdCollaboration
        lblDocTitle.Text = String.Format("Documento ( Versione n. {0} Ultimo Utente: {1} {2} )", coll.GetFirstCollaborationSignActive.Incremental, coll.RegistrationName, coll.RegistrationDate.DefaultString)

        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            lblMemorandumDateTitle.Text = "Data scadenza:"
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            tAlertData.Visible = False
        End If

        lblProposer.Text = coll.RegistrationUser
        lblObject.Text = coll.CollaborationObject
        lblDestinations.Text = String.Empty
        If coll.MemorandumDate.HasValue Then
            lblMemurandumDate.Text = coll.MemorandumDate.DefaultString
        End If
        lblAlertDate.Text = coll.AlertDate.DefaultString
        lblNote.Text = coll.Note

        pnlDocument.Controls.Add(CreaImageUrl(coll.GetFirstDocumentVersioningName(), coll.Location.Id, coll.GetFirstDocumentVersioning()))

        Dim attachments As IList(Of DocumentFDQDTO) = Facade.CollaborationFacade.GetFdqAttachments(New List(Of Integer)({coll.Id}))
        For Each attachment As DocumentFDQDTO In attachments
            pnlAttachments.Controls.Add(CreaImageUrl(attachment.DocumentName, attachment.IdLocation, attachment.IdDocument))
        Next

        Dim collSigns As IList(Of CollaborationSign) = Facade.CollaborationSignsFacade.SearchFull(coll.Id)
        For Each collSign As CollaborationSign In collSigns
            lblDestinations.Text &= collSign.SignName & WebHelper.Br
        Next
    End Sub

#End Region

#Region "Image URL"
    Private Function CreaImageUrl(ByVal fileName As String, ByVal idLocation As Integer, ByVal idBiblos As Long) As LiteralControl
        Dim location As Location = Facade.LocationFacade.GetById(idLocation)
        Dim fileNameTemp As String = "Offline/Files/" & CommonUtil.UserDocumentName & String.Format("{0:HHmmss}", Now()) & "-Offline"
        If idBiblos <> 0 Then
            Try
                Dim doc As New BiblosDocumentInfo(location.DocumentServer, location.ProtBiblosDSDB, idBiblos, 0)
                BiblosFacade.SaveUniqueToTemp(doc, Path.Combine(CommonUtil.GetInstance().AppTempPath, "/Offline/Files/", doc.Name))
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
                BasePage.AjaxAlert("Errore estrazione documento: " & StringHelper.ReplaceAlert(ex.Message), False)
            End Try
        Else
            BasePage.AjaxAlert("Errore in Estrazione del Documento", False)
        End If

        ' TODO: Rimuovere CAPRE!
        Dim str As String
        str = "<a href='" & CommonInstance().AppTempHttp & fileNameTemp & "' target='_blank'>"
        str += "<img border='0' src='" & ImagePath.FromFile(fileName) & "'>&nbsp;" & fileName & "<br>"
        str += "</a>"

        Dim lit As LiteralControl = New LiteralControl
        lit.Text = str
        Return lit
    End Function
#End Region

End Class