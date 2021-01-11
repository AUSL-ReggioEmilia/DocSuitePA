Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class UserFDQMultiple
    Inherits UserBasePage

#Region " Fields "

    Private gErrori As String = String.Empty

#End Region

#Region " Properties "

    Private ReadOnly Property IdC() As String
        Get
            Return Request.QueryString("IdC")
        End Get
    End Property

    Protected Property TempDir() As String
        Get
            Return CType(ViewState("TempDir"), String)
        End Get
        Set(ByVal value As String)
            ViewState("TempDir") = value
        End Set
    End Property

    Public ReadOnly Property FVicario As String
        Get
            Return DocSuiteContext.Current.ProtocolEnv.DefaultFVicario
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        WebUtils.ObjAttDisplayNone(btnCheckIn)
        InitializeAjaxSettings()
        MasterDocSuite.TitleVisible = False
        If Not IsPostBack Then
            Initialize(chkAllegati.Checked)

            If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.DefaultFVicario) Then
                chkFVicario.Style.Remove("display")
            Else
                chkFVicario.Style.Add("display", "none")
            End If
        End If
    End Sub

    Private Sub btnFDQ_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnFDQ.Click
        FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - btnFDQ_Click")

        Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()

        Dim di As DirectoryInfo
        If Not String.IsNullOrEmpty(TempDir) Then
            Try
                di = New DirectoryInfo(TempDir)
                di.Delete(True)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
            End Try
        End If

        'Creazione Directory esportazione
        TempDir = String.Format("{0}{1}{2:hhmmss}", DocSuiteContext.Current.ProtocolEnv.FDQMultipleShare.Replace("%ServerName%", Server.MachineName), DocSuiteContext.Current.User.UserName, DateTime.Now)
        di = Directory.CreateDirectory(TempDir)
        di.CreateSubdirectory("In")

        'Estrazione documenti per la firma
        If di.Exists Then
            For Each dataGridItem As GridDataItem In gvFDQMultiple.Items
                Dim coll As Collaboration = Facade.CollaborationFacade.GetById(Integer.Parse(dataGridItem("Collaboration").Text))
                If coll Is Nothing Then
                    AjaxAlert("La Collaboration n. " & IdC & " non esiste.")
                    di.Delete(True)
                    Exit Sub
                End If
                If DirectCast(dataGridItem.FindControl("chkSignDoc"), CheckBox).Checked Then
                    Dim doc As New BiblosDocumentInfo(coll.Location.ProtBiblosDSDB, Integer.Parse(dataGridItem("IdDocument").Text), 0)
                    Dim dir As New DirectoryInfo(Path.Combine(TempDir, "In"))
                    doc.SaveToDisk(dir, dataGridItem("FileName").Text)
                End If
            Next
        End If

        StartFDQ()
        impersonator.ImpersonationUndo()
    End Sub

    Private Sub btnCheckIn_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnCheckIn.Click
        FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - CheckIN")

        If Not CheckInDocumenti(TempDir & "\Out") Then
            Exit Sub
        End If

        btnFDQ.Enabled = False

        If String.IsNullOrEmpty(gErrori) AndAlso String.IsNullOrEmpty(hdnHasError.Value) Then
            AjaxManager.ResponseScripts.Add("Close('');")
            Exit Sub
        End If

        Dim jsScript As New StringBuilder
        With jsScript
            .Append("<script language='javascript'>")
            .Append(CreateIconsJS())
            .Append(hdnHasError.Value)
            .Append(gErrori)
            .Append(hdnComplete.Value)
            .Append(" alert('Si sono verificati errori nella firma dei documenti.');")
            .Append(" GetRadWindow().BrowserWindow.UpdateGroups();")
            .Append("</script>")
        End With

        ClientScript.RegisterStartupScript(Me.GetType(), "btnCheckIn_Click_" & Guid.NewGuid.ToString("N"), jsScript.ToString())
    End Sub

    Private Sub gvFDQMultiple_InsertCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles gvFDQMultiple.InsertCommand
        Select Case e.CommandName
            Case "Docu"
                Dim coll As Collaboration = Facade.CollaborationFacade.GetById(Integer.Parse(e.CommandArgument))
                If coll IsNot Nothing Then
                    Throw New NotImplementedException("Passaggio a ViewerLight non implementato")
                End If
        End Select
    End Sub

    Private Sub gvFDQMultiple_ItemDataBound(ByVal source As Object, ByVal e As GridItemEventArgs) Handles gvFDQMultiple.ItemDataBound
        If e.Item.ItemType.Equals(GridItemType.Item) OrElse e.Item.ItemType.Equals(GridItemType.AlternatingItem) Then
            Dim item As DocumentFDQDTO = DirectCast(e.Item.DataItem, DocumentFDQDTO)
            With DirectCast(e.Item.FindControl("chkSignDoc"), CheckBox)
                .Checked = String.Compare(item.DocumentType, "Doc. Principale", StringComparison.OrdinalIgnoreCase) = 0
                .Enabled = String.Compare(item.DocumentType, "Doc. Principale", StringComparison.OrdinalIgnoreCase) <> 0
            End With
            With DirectCast(e.Item.FindControl("imgType"), ImageButton)
                .ImageUrl = ImagePath.FromFile(item.DocumentName)
                .CommandArgument = item.Collaboration.ToString()
            End With
        End If
    End Sub

    Protected Sub chkAllegati_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkAllegati.CheckedChanged
        Initialize(chkAllegati.Checked)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(gvFDQMultiple, gvFDQMultiple, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkAllegati, gvFDQMultiple, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCheckIn, gvFDQMultiple, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkFVicario, gvFDQMultiple, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnFDQ, chkFVicario, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub Initialize(ByVal flgAllegati As Boolean)

        Dim idCollList As List(Of Integer) = StringHelper.ConvertStringToList(Of Integer)(IdC)
        If idCollList Is Nothing Then
            Exit Sub
        End If

        Dim documents As IList(Of DocumentFDQDTO) = Facade.CollaborationFacade.GetSortedDocumentsToSign(idCollList, flgAllegati)

        gvFDQMultiple.DataSource = documents
        gvFDQMultiple.VirtualItemCount = documents.Count
        gvFDQMultiple.AllowPaging = ProtocolEnv.PagingOnMultipleFDQ
        gvFDQMultiple.DataBind()
    End Sub

    Private Function CheckInDocumenti(ByVal outTempDir As String) As Boolean

        FileLogger.Info(LoggerName, String.Concat(DocSuiteContext.Current.User.FullUserName, " - CheckInDocumenti - ", outTempDir))

        gErrori = String.Empty

        Dim impersonator As Impersonator = Nothing
        Try
            impersonator = CommonAD.ImpersonateSuperUser()
            Dim di As DirectoryInfo = New DirectoryInfo(outTempDir)
            If Not di.Exists Then
                Throw New DocSuiteException("Impossibile accedere alla directory di destinazione dei files firmati.")
            End If

            Dim files As FileInfo() = di.GetFiles()
            If files.Length = 0 Then
                Throw New DocSuiteException("Nessun documento firmato. Possibile errore in inserimento del PIN.")
            End If

            For Each file As FileInfo In di.GetFiles()
                FileLogger.Info(Facade.CollaborationFacade.LoggerName, String.Concat(DocSuiteContext.Current.User.FullUserName, " - CheckInDocumento - ", file.Name))

                Dim values As String() = file.Name.Split("§"c)
                If values.Length < 3 Then
                    AddError(file.Name, "Il nome del file '" + file.Name + "' non è nel formato corretto")
                    Continue For
                End If

                Try
                    Dim col As Collaboration = Facade.CollaborationFacade.GetById(Integer.Parse(values(0)))
                    Dim incremental As Short = 0 ' Valore impostato per riferimento nel metodo BiblosInsert
                    Dim collaborationIncremental As Short = Short.Parse(values(1))
                    Dim chainId As Integer = BiblosInsert(col, collaborationIncremental, incremental, values(2), file, outTempDir)
                    Facade.CollaborationFacade.SetSignedByUser(col, DocSuiteContext.Current.User.FullUserName, DateTime.Now)

                    FileLogger.Info(Facade.CollaborationFacade.LoggerName, String.Format("{0} - CheckInDocumento IDCatena - {1}", DocSuiteContext.Current.User.FullUserName, chainId))
                    Facade.CollaborationLogFacade.Insert(col, collaborationIncremental, incremental, chainId, CollaborationLogType.MF, String.Format("Firma Documento [{0}].", values(2)))
                Catch ex As Exception
                    AddError(file.Name, ex.Message)
                End Try
            Next

            Dim temporaryDirectory As DirectoryInfo = New DirectoryInfo(TempDir)
            temporaryDirectory.Delete(True)

        Catch ex As DocSuiteException
            AjaxAlert(ex.Message)
            FileLogger.Error(LoggerName, ex.Message)
            Return False

        Finally
            impersonator.ImpersonationUndo()
        End Try

        Return True

    End Function

    Private Function BiblosInsert(ByRef coll As Collaboration, ByVal collaborationIncremental As Short, ByRef incremental As Short, ByVal documentName As String, ByVal file As FileInfo, ByVal OutTempDir As String) As Integer

        FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - BiblosInsert - " & documentName)
        Try
            Dim idChain As Int32 = 0

            Dim document As New FileDocumentInfo(file) With {.Name = documentName}

            Try
                document.Signature = Facade.CollaborationFacade.GenerateSignature(coll, DateTime.Now, "")
                Dim location As Location = Facade.LocationFacade.GetById(ProtocolEnv.CollaborationLocation)

                idChain = document.ArchiveInBiblos(location.ProtBiblosDSDB, idChain).BiblosChainId

                FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - BiblosInsert DONE - " & idChain)

                Dim versioningDoc As CollaborationVersioning = Facade.CollaborationVersioningFacade.InsertDocument(coll.Id, idChain, documentName, collaborationIncremental)

                If Not versioningDoc Is Nothing Then
                    FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - Documento inserito in collaborazione")
                    incremental = versioningDoc.Incremental
                    Return idChain
                Else
                    FileLogger.Info(LoggerName, "Impossibile inserire il documento '" + file.Name + "'")
                    AddError(file.Name, "Impossibile inserire il documento '" + file.Name + "'")
                    Return 0
                End If
            Catch ex As Exception
                AddError(file.Name, "Impossibile inserire in Biblos il file '" + file.Name + "'")
                Return 0
            End Try
        Catch Ex As Exception
            AddError(file.Name, Ex.Message)
            Return 0
        Finally
            Dim di As New FileInfo(OutTempDir & "\" & file.Name)
            di.Delete()
        End Try

    End Function

    Private Sub AddError(ByVal NomeFile As String, ByVal Errore As String)
        gErrori += "icons[" & NomeFile & "].src = '../Comm/Images/Remove16.gif';"
        gErrori += "icons[" & NomeFile & "].title = '" + Errore + "';"
    End Sub

#End Region

#Region "Sign Functions"

    Private Sub StartFDQ()
        FileLogger.Info(LoggerName, DocSuiteContext.Current.User.FullUserName & " - StartFDQ")

        Dim outputDirInfo As DirectoryInfo = Directory.CreateDirectory(TempDir & "\Out")
        If outputDirInfo.Exists Then
            Dim inputDir, outputDir, jsScript As String
            inputDir = TempDir & "\In"
            outputDir = TempDir & "\Out"

            inputDir = inputDir.Replace("\", "\\")
            outputDir = outputDir.Replace("\", "\\")

            jsScript = String.Format("raise('{0}', '{1}');", inputDir, outputDir)
            AjaxManager.ResponseScripts.Add(jsScript)
        Else
            AjaxAlert("Impossibile trovare la directory: " & outputDirInfo.FullName)
        End If
    End Sub

    Private Function CreateIconsJS() As String
        Dim icons As String = "var icons = {"
        Dim fileName As String

        For Each dgi As GridDataItem In gvFDQMultiple.Items
            fileName = String.Concat(dgi("Collaboration").Text, ";", dgi("Incremental").Text).Replace("'", "\'")
            icons &= String.Format("'{0}' : document.getElementById('{1}')", fileName, dgi("cType").FindControl("imgType").ClientID)

            If dgi.ItemIndex < gvFDQMultiple.Items.Count - 1 Then
                icons &= ","
            End If
        Next
        icons &= " };"

        Return icons
    End Function

#End Region

End Class