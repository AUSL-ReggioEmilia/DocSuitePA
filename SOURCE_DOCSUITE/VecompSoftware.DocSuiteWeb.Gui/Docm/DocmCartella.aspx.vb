Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic

Partial Public Class DocmCartella
    Inherits DocmBasePage

#Region " Fields "

#End Region

#Region " Property "

    Private ReadOnly Property Incremental As Short?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Short?)("Incremental", Nothing)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()
        MasterDocSuite.TitleVisible = False
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Select Case Action
            Case "add"
                Dim folder As New DocumentFolder()
                With folder
                    .Year = CurrentDocumentYear
                    .Number = CurrentDocumentNumber
                    .Incremental = Facade.DocumentFolderFacade.GetMaxId(CurrentDocumentYear, CurrentDocumentNumber)
                    .IncrementalFather = Incremental
                    .FolderName = txtFolderName.Text
                    .RegistrationUser = DocSuiteContext.Current.User.FullUserName
                    .RegistrationDate = DateTimeOffset.UtcNow
                    .DocumentsRequired = Integer.Parse(txtDocNumber.Text)
                    .ExpiryDate = rdpExpiryDate.SelectedDate
                    .Description = txtDescription.Text
                    .IsActive = True
                End With

                Facade.DocumentFolderFacade.Save(folder)

            Case "rename"
                Dim idKey As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, Incremental)
                Dim folder As DocumentFolder = Facade.DocumentFolderFacade.GetById(idKey)
                If folder Is Nothing Then
                    Throw New DocSuiteException("Modifica Cartella", "Impossibile trovare la cartella.")
                End If
                With folder
                    .FolderName = txtNewFolderName.Text
                    .DocumentsRequired = Integer.Parse(txtNewDocNumber.Text)
                    .ExpiryDate = rdpNewExpiryDate.SelectedDate
                    .Description = txtNewDescription.Text
                End With

                Facade.DocumentFolderFacade.Update(folder)

            Case "delete"
                Dim idKey As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, Incremental)
                Dim folder As DocumentFolder = Facade.DocumentFolderFacade.GetById(idKey)
                Facade.DocumentFolderFacade.Delete(folder)

        End Select

        AjaxManager.ResponseScripts.Add("CloseWindow();")
    End Sub

    Private Sub btnClearExpiryDate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClearExpiryDate.Click
        Dim idKey As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, Incremental)
        Dim folder As DocumentFolder = Facade.DocumentFolderFacade.GetById(idKey)
        With folder
            .ExpiryDate = Nothing
            .Description = Nothing
        End With
        Facade.DocumentFolderFacade.Update(folder)

        AjaxManager.ResponseScripts.Add("CloseWindow();")
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()

        btnClearExpiryDate.Enabled = False
        Select Case Action
            Case "add"
                Title = "Inserimento Cartella"
                FolderAdd.Visible = True
                txtFolderName.Focus()

            Case "rename"
                Title = "Rinomina Cartella"
                Dim idKey As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, Incremental)
                Dim folder As DocumentFolder = Facade.DocumentFolderFacade.GetById(idKey)
                If Not folder Is Nothing Then
                    'Corrente
                    txtOldFolderName.Text = folder.FolderName
                    If folder.DocumentsRequired.HasValue Then
                        txtOldDocNumber.Text = folder.DocumentsRequired.Value.ToString()
                    Else
                        txtOldDocNumber.Text = "0"
                    End If
                    If folder.ExpiryDate.HasValue Then
                        txtOldExpiryDate.Text = folder.ExpiryDate.DefaultString()
                    End If
                    txtOldDescription.Text = folder.Description
                    'Nuovo
                    txtNewFolderName.Text = folder.FolderName
                    If folder.DocumentsRequired.HasValue Then
                        txtNewDocNumber.Text = folder.DocumentsRequired.Value.ToString()
                    Else
                        txtNewDocNumber.Text = "0"
                    End If
                    rdpNewExpiryDate.SelectedDate = folder.ExpiryDate
                    txtNewDescription.Text = folder.Description
                    btnClearExpiryDate.Enabled = folder.ExpiryDate.HasValue
                End If

                FolderRename.Visible = True
                txtNewFolderName.Focus()

            Case "delete"
                Title = "Cancellazione Cartella"

                Dim folders As IList(Of DocumentFolder) = Facade.DocumentFolderFacade.GetByYearAndNumber(CurrentDocumentYear, CurrentDocumentNumber, Incremental)
                If folders.Count <= 0 Then
                    Dim yearNumberInc As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, Incremental)
                    If Facade.DocumentObjectFacade.FolderHasDocumentObjectActive(yearNumberInc) Then
                        AjaxAlert("La cartella contiene dei documenti.{0}Non è possibile rimuoverla", Environment.NewLine)
                        btnConferma.Enabled = False
                    Else
                        Facade.DocumentFolderFacade.Delete(yearNumberInc)
                    End If
                Else
                    AjaxAlert("La cartella contiene [{0}] cartelle.{1}Non è possibile rimuoverla", folders.Count, Environment.NewLine)
                    btnConferma.Enabled = False
                End If

        End Select

        Dim titolo As String = String.Empty
        FncCalcolaCartella(CurrentDocumentYear, CurrentDocumentNumber, titolo, Incremental.Value)
        If Not String.IsNullOrEmpty(titolo) Then
            Title &= " - " & titolo
        End If
    End Sub

    Public Sub FncCalcolaCartella(ByVal incYear As Short, ByVal incNumber As Integer, ByRef titolo As String, ByVal incNewIncremental As Short)
        Dim idKey As New YearNumberIncrCompositeKey(incYear, incNumber, incNewIncremental)
        Dim folder As DocumentFolder = Facade.DocumentFolderFacade.GetById(idKey)
        If folder Is Nothing Then
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(titolo) Then
            titolo = "/" & titolo
        End If

        If folder.Role Is Nothing Then
            titolo = folder.FolderName.ToString() & titolo
        Else
            titolo = folder.Role.Name.ToString() & titolo
        End If

        If folder.IncrementalFather.HasValue Then
            FncCalcolaCartella(incYear, incNumber, titolo, folder.IncrementalFather.Value)
        End If
    End Sub

#End Region

End Class
