Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging


Partial Public Class DocmFile
    Inherits DocmBasePage

#Region " Fields "

    Private _multiple As Boolean?
    Private _refreshDocument As String

#End Region

#Region " Properties "

    Public ReadOnly Property ManagerID() As String
        Get
            Return Request.QueryString("ManagerID")
        End Get
    End Property

    ''' <summary>Indica se la pagina è stata chiamata per l'inserimento multiplo dei files</summary>
    Protected ReadOnly Property Multiple As Boolean
        Get
            If Not _multiple.HasValue Then
                _multiple = (Not String.IsNullOrEmpty(Request.QueryString("Multiple"))) AndAlso Boolean.Parse(Request.QueryString("Multiple"))
            End If
            Return _multiple.Value
        End Get
    End Property

    Private ReadOnly Property refreshDocument As String
        Get
            If _refreshDocument Is Nothing Then
                _refreshDocument = Request.QueryString.GetValueOrDefault("Refresh", String.Empty)
            End If
            Return _refreshDocument
        End Get
    End Property

    Private ReadOnly Property IncrementalObject As Short
        Get
            Return Request.QueryString.GetValue(Of Short)("IncrementalObject")
        End Get
    End Property

    Private ReadOnly Property Incremental As Short
        Get
            Return Request.QueryString.GetValue(Of Short)("Incremental")
        End Get
    End Property

    Private ReadOnly Property Add As Boolean
        Get
            Return Request.QueryString("Add").Eq("ON")
        End Get
    End Property

    ''' <summary> Ruolo del folder </summary>
    ''' <remarks> Cagata pazzesca, viene inizializzato da un metodo stupido, centralizzare sensatamente </remarks>
    Private Property IdRole As Integer?
        Get
            Return CType(ViewState("idRole"), Integer?)
        End Get
        Set(value As Integer?)
            ViewState("idRole") = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        btnCancella.Attributes.Add("onclick", "if (confirm('Conferma Cancellazione del Documento')) return true;else return false;")
        InitializeWindow()
        InitializeAjax()

        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub BtnInserimentoClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimento.Click
        If CurrentDocument Is Nothing Then
            Throw New DocSuiteException("Inserimento documento", "Pratica non disponibile non trovato.")
        ElseIf CurrentDocument.Location Is Nothing Then
            Throw New DocSuiteException("Inserimento documento", "Location per la Pratica corrente mancante o non valida.")
        End If

        Dim signature As String = Facade.DocumentFacade.GenerateSignature(CurrentDocument)
        Dim inserted As DocumentObject = Nothing
        For Each document As DocumentInfo In UscDocumentUpload1.DocumentInfos
            Try
                Dim tokens As IList(Of DocumentToken) = Facade.DocumentTokenFacade.DocumentTokenStep(CurrentDocumentYear, CurrentDocumentNumber, IdRole.Value)
                Dim item As New DocumentObject With
                {
                    .Year = CurrentDocumentYear,
                    .Number = CurrentDocumentNumber,
                    .IncrementalFolder = Incremental,
                    .DocStep = tokens.First().DocStep,
                    .SubStep = tokens.First().SubStep,
                    .idObjectType = "FL",
                    .Description = document.Name,
                    .DocumentDate = UscDocumentDati1.DateText,
                    .DocObject = UscDocumentDati1.ObjectText,
                    .Reason = UscDocumentDati1.ReasonText,
                    .Note = UscDocumentDati1.NoteText,
                    .Link = String.Empty
                }

                document.Signature = signature
                inserted = Facade.DocumentObjectFacade.InsertDocumentObject(item, CurrentDocument.Location, document, tokens)
            Catch ex As DocSuiteException
                FileLogger.Warn(LoggerName, "Errore inserimento documento in pratica", ex)
                AjaxAlert(ex.Message)
            Catch ex As Exception
                FileLogger.Error(LoggerName, "Errore imprevisto inserimento documento pratica", ex)
                AjaxAlert("Errore imprevisto, contattare l'assistenza.")
            End Try
        Next

        If CurrentDocument.Container.IsMailCCEnable AndAlso inserted IsNot Nothing AndAlso Not Multiple Then
            Dim tokensCC As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, True, , 1)
            If Not tokensCC.IsNullOrEmpty() Then
                Dim url As String = String.Format("Type=Docm&Year={0}&Number={1}&Folder={2}&File={3}", CurrentDocumentYear, CurrentDocumentNumber, Server.UrlEncode(Session("FOLDER").ToString()), Server.UrlEncode(inserted.Description))

                Dim jsOpenWindowMailCC As String = String.Format("return {0}_OpenWindowMailCC('wndMailCC', '{1}');", Me.ID, url)
                AjaxManager.ResponseScripts.Add(jsOpenWindowMailCC)
                jsOpenWindowMailCC = String.Format("CloseWindow('{0}');", refreshDocument)
                AjaxManager.ResponseScripts.Add(jsOpenWindowMailCC)
                Return
            End If
        End If

        Dim jsCloseWindow As String = String.Format("CloseWindow('{0}');", refreshDocument)
        AjaxManager.ResponseScripts.Add(jsCloseWindow)
        AjaxManager.ResponseScripts.Add("parent.parent.RefreshFolderAjax();")
    End Sub

    Protected Sub btnModifica_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnModifica.Click
        Dim tn As RadTreeNode = UscDocumentFolder1.Destination.SelectedNode()

        Try
            Dim yearnumberincr As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject)
            Dim documentobject As DocumentObject = Facade.DocumentObjectFacade.GetById(yearnumberincr)

            Dim newIncrementalFolder As Integer = 0
            If tn IsNot Nothing AndAlso Not String.IsNullOrEmpty(tn.Value) Then
                newIncrementalFolder = tn.Value
                If (documentobject IsNot Nothing) AndAlso documentobject.IncrementalFolder <> newIncrementalFolder Then
                    documentobject.IncrementalFolder = newIncrementalFolder
                End If
            End If

            documentobject.DocumentDate = UscDocumentDati1.DateText
            documentobject.Reason = UscDocumentDati1.ReasonText
            documentobject.Note = UscDocumentDati1.NoteText
            documentobject.DocObject = UscDocumentDati1.ObjectText


            Facade.DocumentObjectFacade.Update(documentobject)
            RegisterFolderRefreshScript(newIncrementalFolder)
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in Modifica Documento", ex)
            AjaxAlert("Errore in Modifica Documento, contattare l'assistenza.")
        End Try
    End Sub

    Protected Sub btnCancella_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancella.Click
        Dim documentobject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))
        If documentobject Is Nothing Then
            Exit Sub
        End If

        documentobject.idObjectStatus = "A"
        documentobject.LastChangedDate = DateTimeOffset.UtcNow
        documentobject.LastChangedUser = DocSuiteContext.Current.User.FullUserName

        Facade.DocumentObjectFacade.Update(documentobject)
        RegisterFolderRefreshScript()
    End Sub

    Protected Sub btnCheckOut_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCheckOut.Click
        Dim ynick As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject)
        Dim documentobject As DocumentObject = Facade.DocumentObjectFacade.GetById(ynick)
        If documentobject Is Nothing Then
            AjaxAlert("Errore in ricerca del Documento.{0}Check Out annullato", Environment.NewLine)
            Exit Sub
        End If

        Dim idBiblos As Integer = documentobject.idBiblos

        Dim impersonator As Impersonator = Nothing
        Try
            impersonator = CommonAD.ImpersonateSuperUser()
            Dim Prefisso As String = DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber, "-") & "-" & IncrementalObject
            Dim NomeFile As String = String.Empty
            CheckOutDocument(CurrentDocument.Location.Id, idBiblos, Prefisso, NomeFile, True)
            Dim versioning As New DocumentVersioning
            With versioning
                .Id.Year = CurrentDocumentYear
                .Id.Number = CurrentDocumentNumber
                .Id.Incremental = Facade.DocumentVersioningFacade.GetMaxId(CurrentDocumentYear, CurrentDocumentNumber)
                .IncrementalObject = IncrementalObject
                .CheckOutUser = DocSuiteContext.Current.User.FullUserName
                .CheckOutDate = Date.Now
                .CheckDir = NomeFile
                .CheckSystemComputer = DocSuiteContext.Current.UserComputer
                .CheckStatus = "O"
            End With

            Facade.DocumentVersioningFacade.Save(versioning)
            RegisterFolderRefreshScript()
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert(ex.Message)
        Finally
            ' Rilascio i permessi per manipolare cartelle sul server
            If impersonator IsNot Nothing Then
                impersonator.ImpersonationUndo()
            End If
        End Try
        impersonator.ImpersonationUndo()
    End Sub

    Protected Sub BtnCancelCheckOutClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelCheckOut.Click
        If Not Facade.DocumentObjectFacade.CheckDocObjectCheckedOut(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject) Then
            AjaxAlert("Errore in ricerca dati del Versioning.")
        End If

        Dim impersonator As Impersonator = Nothing
        Try
            impersonator = CommonAD.ImpersonateSuperUser()

            Dim documentversioning As DocumentVersioning = Facade.DocumentVersioningFacade.GetDocumentVersion(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject, "O")
            If documentversioning Is Nothing Then
                Throw New DocSuiteException("Annullamento versionamento", String.Format("Impossibile trovare versione [{0}/{1}/{2}]", CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))
            End If

            With documentversioning
                .CheckOutUser = DocSuiteContext.Current.User.FullUserName
                .CheckOutDate = Date.Now
                .CheckStatus = "A"
            End With

            Kill(DocSuiteContext.Current.ProtocolEnv.VersioningShare & "\Docm\" & documentversioning.CheckDir)

            RegisterFolderRefreshScript()
            Facade.DocumentVersioningFacade.Update(documentversioning)
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore annullamento versionamento pratiche", ex)
            AjaxAlert("Errore in operazione di Annullamento.")
        Finally
            If impersonator IsNot Nothing Then
                impersonator.ImpersonationUndo()
            End If

        End Try
    End Sub

    Protected Sub btnCheckIn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCheckIn.Click
        Dim fileName As String

        Dim versionings As IList(Of DocumentVersioning) = Facade.DocumentVersioningFacade.GetDocumentVersionAll(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject, "O")
        If Not versionings.IsNullOrEmpty() Then
            fileName = GetVersioningFullPath(versionings.First())
            UscDocumentUpload1.TreeViewControl.Nodes(0).Nodes(0).Value = fileName
        Else
            AjaxAlert("Errore in ricerca precedente estrazione, operazione di Check In interrotta.")
            Return
        End If

        Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()

        For Each x As DocumentInfo In UscDocumentUpload1.DocumentInfos
            If Not x.Exists Then
                AjaxAlert("Il Documento sul Server non è valido.{0}Operazione di Check In Interrotta.", Environment.NewLine)
                Exit Sub
            End If
        Next

        If FileHelper.IsInUse(fileName) Then
            AjaxAlert("Il Documento è in Uso.{0}Per Procedere con il Check In Chiudere il Documento.", Environment.NewLine)
            Exit Sub
        End If
        'biblos
        Dim doc As DocumentInfo = UscDocumentUpload1.DocumentInfos.LastOrDefault()
        doc.Signature = Facade.DocumentFacade.GenerateSignature(CurrentDocument)

        Dim idCatena As Integer
        Try
            idCatena = doc.ArchiveInBiblos(CurrentDocument.Location.DocmBiblosDSDB).BiblosChainId
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in checkin", ex)
            AjaxAlert("Errore in fase di archiviazione del documento, contattare l'assistenza.")
            Exit Sub
        End Try

        impersonator.ImpersonationUndo()

        Dim documentobject As New DocumentObject()

        With documentobject
            .Id.Year = CurrentDocumentYear
            .Id.Number = CurrentDocumentNumber
            .IncrementalFolder = versionings.First().DocumentObject.IncrementalFolder
            .Id.Incremental = Facade.DocumentObjectFacade.GetMaxId(CurrentDocumentYear, CurrentDocumentNumber)
            .DocStep = versionings.First().DocumentObject.DocStep
            .SubStep = versionings.First().DocumentObject.SubStep
            .Description = versionings.First().DocumentObject.Description
            .DocumentDate = UscDocumentDati1.DateText
            .DocObject = UscDocumentDati1.ObjectText
            .Reason = UscDocumentDati1.ReasonText
            .Note = UscDocumentDati1.NoteText
            .idBiblos = idCatena
            .idObjectType = "FL"
            .Link = ""
            .RegistrationDate = DateTime.Now
            .OrdinalPosition = versionings(0).DocumentObject.OrdinalPosition
        End With

        Facade.DocumentObjectFacade.Save(documentobject)

        If Facade.DocumentVersioningFacade.UpdateCheckIn(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject, documentobject.Incremental) Then
            Kill(fileName)
            RegisterFolderRefreshScript()
        Else
            AjaxAlert("Si è verificato un errore in aggiornamento archivio.{0}Operazione di Check In Interrotta", Environment.NewLine)
            Exit Sub
        End If

    End Sub

    Private Sub btnMailCC_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMailCC.Click
        WindowBuilder.LoadWindow("wndMailCC", "../Docm/DocmMailCC.aspx?Type=Docm&Year=" & CurrentDocumentYear & "&Number=" & CurrentDocumentNumber & "&Folder=" & Session("FOLDER") & "&File=" & Session("DocObjectDescription"))
    End Sub

#End Region

#Region " Methods "

    Private Function GetVersioningFullPath(versioning As DocumentVersioning) As String
        Return Path.Combine(DocSuiteContext.Current.ProtocolEnv.VersioningShare, "Docm", versioning.CheckDir)
    End Function

    Private Sub Initialize()
        'Put user code to initialize the page here
        Dim titolo As String = String.Empty
        Dim roleIncremental As Integer
        Dim fullIncremental As String = String.Empty

        WebUtils.ObjAttDisplayNone(Documento)
        WebUtils.ObjAttDisplayNone(DocumentoDes)

        btnInserimento.Visible = False
        btnModifica.Visible = False
        btnCancella.Visible = False
        pnlCartella.Visible = False

        FncCalcolaPath(roleIncremental, fullIncremental, titolo, Incremental)
        Session.Add("FOLDER", titolo)

        UscDocumentUpload1.MultipleDocuments = Multiple

        Select Case Action
            Case "Insert"
                Title = "Inserimento Documento"
                btnInserimento.Visible = True
                pnlCartella.Visible = False
                ' se inserimento multiplo cambio le etichette
                If Multiple Then
                    Title = "Inserimento Multiplo"
                    UscDocumentDati1.HeaderText = "Informazioni (verranno applicate a tutti i documenti)"
                    UscDocumentDati1.DateLabel = "Data Documenti:"
                    UscDocumentUpload1.Caption = "Documenti"
                End If
            Case "Modify"
                If Add Then
                    Title = "Modifica Documento"
                Else
                    Title = "Visualizza Documento"
                End If

                Dim obj As DocumentObject
                Dim ynick As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject)

                obj = Facade.DocumentObjectFacade.GetById(ynick)

                If Not obj Is Nothing Then

                    Dim nodo As New RadTreeNode
                    nodo.Text = obj.Description
                    Session.Add("DocObjectDescription", obj.Description)

                    nodo.Font.Bold = True

                    UscDocumentUpload1.TreeViewControl.Nodes(0).Nodes.Add(nodo)

                    With UscDocumentDati1
                        .DateText = obj.DocumentDate
                        .ObjectText = obj.DocObject
                        .ReasonText = obj.Reason
                        .NoteText = obj.Note
                    End With

                Else
                    AjaxAlert("Errore in Ricerca Documento")
                    Exit Sub
                End If

                If Add Then
                    If Not obj.idObjectStatus.Eq("A") Then
                        btnModifica.Visible = True
                        btnCancella.Visible = True
                        btnCheckOut.Visible = True

                        If DocSuiteContext.Current.DocumentEnv.IsVersioningEnabled Then
                            Dim versioning As DocumentVersioning = Facade.DocumentVersioningFacade.GetDocumentVersion(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject, "O")
                            If versioning IsNot Nothing Then
                                If String.IsNullOrEmpty(versioning.CheckStatus) OrElse versioning.CheckStatus.Eq("A") Then
                                    btnCheckOut.Visible = True
                                ElseIf versioning.CheckStatus.Eq("O") AndAlso DocSuiteContext.Current.User.FullUserName.Eq(versioning.CheckOutUser) Then
                                    btnCancelCheckOut.Visible = True
                                    btnCheckIn.Visible = True
                                    btnCheckOut.Visible = False
                                End If

                                Dim fi As New FileInfo(GetVersioningFullPath(versioning))
                                Dim document As New FileDocumentInfo(fi) With {.Caption = obj.Description}
                                UscDocumentUpload1.LoadDocumentInfo(document)
                                Dim added As RadTreeNode = UscDocumentUpload1.GetNodeByDocumentInfo(document)
                                With added
                                    .Style.Add("font-weight", "bold")
                                    .Text = document.Caption
                                End With
                            End If
                        End If
                    Else
                        btnModifica.Visible = True
                    End If

                    UscDocumentUpload1.ReadOnly = True

                    With UscDocumentFolder1
                        .Year = CurrentDocumentYear
                        .Number = CurrentDocumentNumber
                        .Incremental = Incremental
                        .IncrementalFolder = obj.IncrementalFolder
                        .Document = CurrentDocument
                    End With
                    pnlCartella.Visible = True
                Else
                    With UscDocumentDati1
                        .ObjectReadOnly = True
                        .NoteReadOnly = True
                        .ReasonReadOnly = True
                        .DateReadOnly = True
                    End With
                    btnModifica.Visible = False
                End If

                If CurrentDocument.Container.IsMailCCEnable Then
                    Dim cc As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, True, , 1)
                    If cc.Count > 0 Then
                        btnMailCC.Visible = True
                    End If
                End If
        End Select

        If Not String.IsNullOrEmpty(titolo) Then
            Title &= " - " & titolo
        End If
    End Sub

    Private Sub InitializeWindow()
        WindowBuilder.RegisterWindowManager(alertManager)
        WindowBuilder.RegisterOpenerElement(btnMailCC)
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModifica, UscDocumentFolder1)
    End Sub

    Private Sub FncCalcolaPath(ByRef idRoleIncremental As Integer, ByRef fullIncremental As String, ByRef titolo As String, ByRef newIncremental As Short)
        Dim yearnumberincr As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, newIncremental)
        Dim documentfolder As DocumentFolder = Facade.DocumentFolderFacade.GetById(yearnumberincr)
        If documentfolder Is Nothing Then
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(titolo) Then
            titolo = "/" & titolo
        End If
        If Not String.IsNullOrEmpty(fullIncremental) Then
            fullIncremental = "|" & fullIncremental
        End If
        fullIncremental = documentfolder.Incremental & fullIncremental
        If documentfolder.Role Is Nothing Then
            titolo = documentfolder.FolderName & titolo
        Else
            titolo = documentfolder.Role.Name & titolo
            If Not IdRole.HasValue Then
                IdRole = documentfolder.Role.Id
                idRoleIncremental = documentfolder.Incremental
            End If
        End If
        If documentfolder.IncrementalFather.HasValue Then
            FncCalcolaPath(idRoleIncremental, fullIncremental, titolo, documentfolder.IncrementalFather.Value)
        End If
    End Sub

    Public Sub CheckOutDocument(ByVal idLocation As Integer, ByVal biblosDsChain As Integer, ByVal prefisso As String, ByRef nomeFile As String, Optional ByVal security As Boolean = False)
        If String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.VersioningShare) Then
            Throw New DocSuiteException("Estrazione documento", "In ParameterENV (Protocollo) non è definita la chiave VersioningShare.")
        End If

        Dim versioningPath As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.VersioningShare, Type)
        Dim dir As New DirectoryInfo(versioningPath)
        If Not dir.Exists Then
            Throw New DocSuiteException("Estrazione documento", String.Format("Percorso [{0}] mancante o non valido.", versioningPath))
        End If

        Dim location As Location = Facade.LocationFacade.GetById(idLocation)
        If location Is Nothing Then
            Throw New DocSuiteException("Estrazione documento", String.Format("Location [{0}] mancante o non valida.", idLocation))
        End If

        Dim doc As New BiblosDocumentInfo(location.DocmBiblosDSDB, biblosDsChain)
        nomeFile = String.Format("{0}-{1}{2}", prefisso, CommonUtil.UserDocumentName, Path.GetExtension(doc.Name))
        Dim file As FileInfo = doc.SaveToDisk(dir, nomeFile)

        If security Then
            Dim fACL As New FileACL.Security
            fACL.SetFileACL(file.FullName, CommonUtil.UserFullName & ";" & DocSuiteContext.Current.CurrentTenant.DomainUser)
        End If
    End Sub

#End Region

End Class