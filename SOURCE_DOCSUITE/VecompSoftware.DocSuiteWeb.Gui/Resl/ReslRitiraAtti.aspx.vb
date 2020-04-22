Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos.Models

Public Class ReslRitiraAtti
    Inherits ReslBasePage

#Region "Fields"

#End Region

#Region "Properties"
    ''' <summary>
    ''' Directory temporanea dove si trovano i files da firmare
    ''' </summary>
    Protected Property TempDir() As String
        Get
            Return ViewState("TempDir")
        End Get
        Set(ByVal Value As String)
            ViewState("TempDir") = Value
        End Set
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        initializeAjaxSettings()
        initializeButtons()

        If Not Page.IsPostBack Then
            initialize()
            Search()
        End If
    End Sub

    Private Sub FirmaSelezionate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FirmaSelezionate.Click
        SignSelected()
    End Sub

    Private Sub Journals_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles Resolutions.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim res As ResolutionHeader = DirectCast(e.Item.DataItem, ResolutionHeader)

        Dim imgType As ImageButton = DirectCast(e.Item.FindControl("imgType"), ImageButton)
        imgType.PostBackUrl = ResolutionViewerUrl(res)
        imgType.ImageUrl = ImagePath.SmallPdf

        Dim typeLabel As Label = DirectCast(e.Item.FindControl("Type"), Label)
        typeLabel.Text = res.Type.Description
    End Sub

#End Region

#Region " Methods "

    Private Sub initialize()
        Me.Title = "Firma atti ritirati"
        Resolutions.Visible = False
    End Sub

    Public Sub initializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(Resolutions, Resolutions, MasterDocSuite.AjaxLoadingPanelGrid)

    End Sub

    Private Sub initializeButtons()
        If CommonShared.UserConnectedBelongsTo(ResolutionEnv.RitiraAttiGroups) Then
            FirmaSelezionate.Visible = True
            AjaxManager.AjaxSettings.AddAjaxSetting(FirmaSelezionate, Resolutions, MasterDocSuite.AjaxLoadingPanelGrid)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, Resolutions, MasterDocSuite.AjaxLoadingPanelGrid)
            AddHandler AjaxManager.AjaxRequest, AddressOf ReslJournal_AjaxRequest
        Else
            FirmaSelezionate.Visible = False
        End If
    End Sub

    Private Sub Search()
        If ResolutionEnv.SearchMaxRecords <> 0 Then
            Resolutions.PageSize = ResolutionEnv.SearchMaxRecords
        End If

        Dim finder As New NHibernateResolutionFinder(ResolutionFacade.ReslDB)
        finder.WebState = Resolution.WebStateEnum.Revoked
        finder.HasFrontalinoRitiro = True
        finder.EnablePaging = False

        Resolutions.Finder = finder
        Resolutions.DataBindFinder()
        Resolutions.Visible = True

        ' Imposto titolo a seconda dei risultati della ricerca
        If Resolutions.DataSource IsNot Nothing Then
            lblHeader.Text = String.Format(" Atti Ritirati - Risultati ({0}/{1})", Resolutions.DataSource.Count, Resolutions.VirtualItemCount)
        Else
            lblHeader.Text = " Atti Ritirati - Nessun Risultato"
        End If
    End Sub

    ''' <summary> Visualizzazione del frontalino di ritiro. </summary>
    Public Function ResolutionViewerUrl(item As ResolutionHeader) As String

        If Not item.IdFrontalinoRitiro.HasValue Then
            Return String.Empty
        End If

        ' Ritiro il file resolution
        Dim resolution As Resolution = Facade.ResolutionFacade.GetById(item.Id)

        ' Compongo il link del documento  da firmare
        Dim doc As New BiblosDocumentInfo(resolution.Location.DocumentServer, resolution.Location.ReslBiblosDSDB, item.IdFrontalinoRitiro.Value)

        Dim parameters As String = String.Format("servername={0}&guid={1}&label={2}", doc.Server, doc.ChainId, "Ritira Atti")
        Return "~/Viewers/BiblosViewer.aspx?" & CommonShared.AppendSecurityCheck(parameters)
    End Function

#End Region

#Region " Signature "

    ''' <summary> Comincia il processo di firma multipla sui registri selezionati. </summary>
    Private Sub SignSelected()
        FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - signSelected BEGIN")

        Dim fieldName As String = Nothing

        ' Estraggo l'elenco di registri selezionati
        Dim resolutionList As New List(Of Resolution)
        For Each item As GridDataItem In Resolutions.Items
            ' Controllo se la riga è selezionata
            If Not DirectCast(item.FindControl("chkSelect"), CheckBox).Checked Then
                Continue For
            End If

            Dim itemId As Integer = DirectCast(item.GetDataKeyValue("ID"), Integer)
            Dim res As Resolution = Facade.ResolutionFacade.GetById(itemId)
            If res Is Nothing Then
                AjaxAlert("Impossibile estrarre l'atto n. " & itemId)
                Exit Sub
            Else
                resolutionList.Add(res)

                If String.IsNullOrEmpty(fieldName) Then
                    ' Estraggo il nome del campo contenente l'id catena corretto
                    Dim twFacade As New TabWorkflowFacade()
                    fieldName = twFacade.GetByResolution(res.Id).FieldFrontalinoRitiro
                End If

            End If

        Next

        If resolutionList.Count = 0 Then
            AjaxAlert("Nessun atto selezionato.")
            Exit Sub
        End If

        Dim tempDirectory As DirectoryInfo = Nothing
        Dim inDirectory As DirectoryInfo = Nothing
        Dim outDirectory As DirectoryInfo = Nothing
        Dim errorMessage As String = Facade.FileResolutionFacade.Extract(resolutionList, tempDirectory, inDirectory, outDirectory, fieldName)
        TempDir = tempDirectory.FullName

        If String.IsNullOrEmpty(errorMessage) Then
            ' Richiamo il componente per la firma
            Dim inputDir As String = inDirectory.FullName.Replace("\", "\\")
            Dim outputDir As String = outDirectory.FullName.Replace("\", "\\")
            AjaxManager.ResponseScripts.Add(String.Format("OpenFile('{0}', '{1}');", inputDir, outputDir))
        Else
            AjaxAlert(errorMessage)
        End If

        FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - signSelected END")
    End Sub

    Private Sub ReslJournal_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If String.Compare("SIGN", e.Argument) = 0 Then
            completeSigning()
            Search()
        End If
    End Sub

    ''' <summary> Chiamato dopo che il componente ha restituito i files. </summary>
    Private Sub completeSigning()
        FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - completeSigning")

        ' Se la directory temporanea non esiste è per un qualche genere di errore
        If Not Directory.Exists(TempDir) Then
            Exit Sub
        End If

        checkInDocuments(New DirectoryInfo(Path.Combine(TempDir, "Out")))

        ' Cancello la directory temporanea
        Dim tempDirectory As New DirectoryInfo(TempDir)
        If tempDirectory.Exists Then
            tempDirectory.Delete(True)
        End If
    End Sub

    ''' <summary>
    ''' Completa il processo di firma multipla sui registri firmati
    ''' </summary>
    ''' <remarks>Spostare nella facade</remarks>
    Private Function checkInDocuments(ByVal outDirectory As DirectoryInfo) As Boolean

        If Not outDirectory.Exists Then
            AjaxAlert("Nessun documento firmato. Errore tecnico.")
            Return False
        End If

        Dim files As FileInfo() = outDirectory.GetFiles()
        If files.Length = 0 Then
            AjaxAlert("Nessun documento firmato. Possibile dispositivo mancante o errore in inserimento PIN.")
            Return False
        End If

        Dim errorMessage As New StringBuilder

        For Each file As FileInfo In files
            FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - checkInDocumenti - " & file.Name)
            ' Estraggo dal nome l'id del resolution journal
            Dim temp As String = file.Name.Substring(0, file.Name.IndexOf(FileHelper.P7M, StringComparison.InvariantCultureIgnoreCase))
            Dim id As String = temp.Split(ResolutionJournalFacade.Separator)(0)

            Try
                Dim resolution As Resolution = Facade.ResolutionFacade.GetById(id)

                ' Genero la signature del documento da accodare
                Dim signature As String = String.Format(
                    "{0}: Ritiro Albo {1:dd/MM/yyyy}",
                    Facade.ResolutionFacade.SqlResolutionGetNumber(resolution.Id, complete:=True),
                    DateTime.Today)

                ' Estraggo il nome del file su biblos

                Dim doc As New FileDocumentInfo(file)
                doc.Name = Service.GetDocumentName(resolution.Location.DocumentServer, resolution.Location.ReslBiblosDSDB, resolution.File.IdFrontalinoRitiro, 0)
                doc.Signature = signature
                doc.ArchiveInBiblos(resolution.Location.DocumentServer, resolution.Location.ReslBiblosDSDB, resolution.File.IdResolutionFile.GetValueOrDefault(0))

                resolution.File.IdFrontalinoRitiro = Nothing

                ' aggiorno l'idFrontalinoRitiro
                Facade.ResolutionFacade.Update(resolution)

            Catch exception As Exception
                Dim message As String = String.Format("Firma non riuscita per il file '{0}': {1}", file.Name, exception.Message)

                errorMessage.AppendLine(message)

                FileLogger.Warn(LoggerName, message, exception)
            End Try

            FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - checkInDocumenti DONE")
        Next

        If errorMessage.Length <> 0 Then
            AjaxAlert(StringHelper.ReplaceAlert(errorMessage.ToString()))
        Else
            AjaxAlert("Tutti i documenti sono stati firmati con successo.")
        End If

        Return (errorMessage.Length = 0)

    End Function

#End Region

End Class