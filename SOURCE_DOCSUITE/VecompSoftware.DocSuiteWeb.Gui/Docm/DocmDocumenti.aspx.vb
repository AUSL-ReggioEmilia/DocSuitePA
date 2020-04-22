Imports Telerik.Web.UI
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class DocmDocumenti
    Inherits DocmBasePage

#Region " Fields "

    Const Pipe As String = "|"
    Const Slash As String = "/"

    Private _document As Document
    Private _add As String
    Private _documentWithVersion As IList(Of DocumentObject)
    Private _docFolder As DocumentFolder
    Private _incremental As Short? = Nothing

#End Region

#Region " Properties "

    Public Property Document() As Document
        Get
            Return _document
        End Get
        Set(ByVal value As Document)
            _document = value
        End Set
    End Property

    Public ReadOnly Property DocumentFolder() As DocumentFolder
        Get
            If _docFolder Is Nothing AndAlso Incremental.HasValue Then
                _docFolder = Facade.DocumentFolderFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, Incremental.Value)
            End If
            Return _docFolder
        End Get
    End Property

    Public ReadOnly Property Incremental() As Short?
        Get
            If Not _incremental.HasValue Then
                _incremental = Request.QueryString.GetValueOrDefault(Of Short?)("Incremental", Nothing)
            End If
            Return _incremental
        End Get
    End Property

    Public ReadOnly Property ListAllDocuments() As Boolean
        Get
            Return Not Incremental.HasValue
        End Get
    End Property

    Public Property Add() As String
        Get
            Return _add
        End Get
        Set(ByVal value As String)
            _add = value
        End Set
    End Property

    Public Property DocVersioned() As IList(Of DocumentObject)
        Get
            Return _documentWithVersion
        End Get
        Set(ByVal value As IList(Of DocumentObject))
            _documentWithVersion = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Add = Request.QueryString("Add")

        btnRefresh.CssClass = "hiddenField"

        Document = Facade.DocumentFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber)
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Dim expiry As String = DocumentFolderFacade.GetExpiryDescription(DocumentFolder)
            Title = String.Format("Lista Documenti {0}", If(String.IsNullOrEmpty(expiry), "", " - " & expiry))
            InitializeColumns()
            Initialize()
        End If

    End Sub

    Private Sub DG_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles DG.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim documentObject As DocumentObject = DirectCast(e.Item.DataItem, DocumentObject)

        Dim btnEdit As RadButton = CType(e.Item.FindControl("btnEdit"), RadButton)
        If btnEdit IsNot Nothing Then
            btnEdit.Image.ImageUrl = ImagePath.SmallEdit
            btnEdit.CommandName = String.Format("Selz:{0}{1:0000000}{2}", documentObject.Year, documentObject.Number, documentObject.Incremental)
            btnEdit.ToolTip = "Modifica"
        End If

        Dim imgType As Image = CType(e.Item.FindControl("imgType"), Image)
        If imgType IsNot Nothing Then
            If documentObject.idObjectStatus.Eq("a") Then
                imgType.ImageUrl = ImagePath.SmallDelete
                imgType.ToolTip = "Cancellato"
            ElseIf documentObject.idObjectType.Eq("fl") Then
                imgType.ImageUrl = ImagePath.FromFile(documentObject.Description)
            Else
                imgType.ImageUrl = String.Format("../Comm/Images/File/{0}16.gif", CommonUtil.GetIconName(documentObject.idObjectType, documentObject.Description))
            End If
        End If

        Dim btnDown As RadButton = CType(e.Item.FindControl("btnDown"), RadButton)
        If btnDown IsNot Nothing Then
            If documentObject.OrdinalPosition < ViewState("opMax") Then
                btnDown.Image.ImageUrl = "../Comm/Images/ArrowDown.gif"
                btnDown.CommandName = String.Format("opDn:{0}{1:0000000}{2}", documentObject.Year, documentObject.Number, documentObject.Incremental)
            Else
                btnDown.Image.ImageUrl = ImagePath.SmallEmpty
            End If
        End If

        Dim btnUp As RadButton = CType(e.Item.FindControl("btnUp"), RadButton)
        If btnUp IsNot Nothing Then
            If documentObject.OrdinalPosition > ViewState("opMin") Then
                btnUp.Image.ImageUrl = "../Comm/Images/ArrowUp.gif"
                btnUp.CommandName = String.Format("opUp:{0}{1:0000000}{2}", documentObject.Year, documentObject.Number, documentObject.Incremental)
            Else
                btnUp.Image.ImageUrl = ImagePath.SmallEmpty
            End If
        End If

        Dim btnVersion As RadButton = CType(e.Item.FindControl("btnVersion"), RadButton)
        If btnVersion IsNot Nothing Then
            Dim checkout As String = If(Not documentObject.DocumentVersionings.IsNullOrEmpty(), documentObject.DocumentVersionings(0).CheckStatus, "")
            If checkout.Eq("O") Then
                btnVersion.Image.ImageUrl = "../Comm/images/file/CheckOut16.gif"
            ElseIf Facade.DocumentObjectFacade.FolderDocumentObjectCount(documentObject.Id) > 0 AndAlso checkout = "" Then
                btnVersion.Image.ImageUrl = "../App_Themes/DocSuite2008/imgset16/delete.png"
            ElseIf Facade.DocumentVersioningFacade.GetDocumentVersionCount(documentObject.Id) > 0 Then
                btnVersion.Image.ImageUrl = "../Comm/images/file/version16.gif"
            Else
                btnVersion.Image.ImageUrl = ImagePath.SmallEmpty
            End If

            btnVersion.CommandName = String.Format("Vers:{0}{1:0000000}{2}", documentObject.Year, documentObject.Number, documentObject.Incremental)
        End If
    End Sub

    Private Sub DG_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles DG.ItemCommand
        Dim commandType As String = Left(e.CommandName, 5)
        If (String.IsNullOrEmpty(commandType)) Then
            Exit Sub
        End If

        Dim commandYear As Short
        Short.TryParse(Mid$(e.CommandName, 6, 4), commandYear)
        Dim commandNumber As Integer
        Integer.TryParse(Mid$(e.CommandName, 10, 7), commandNumber)
        Dim incrementalObject As Short
        Short.TryParse(Mid$(e.CommandName, 17), incrementalObject)

        Select Case commandType
            Case "Docm:"
                Dim docObject As New DocumentObject
                For Each docObj As DocumentObject In Document.Objects
                    If docObj.Year = commandYear AndAlso docObj.Number = commandNumber AndAlso docObj.Incremental = incrementalObject Then
                        docObject = docObj
                    End If
                Next

                Select Case docObject.idObjectType
                    Case "FL"
                        If docObject.DocumentVersionings.Count > 0 Then
                            Dim lastDocVersioning As DocumentVersioning = docObject.DocumentVersionings(docObject.DocumentVersionings.Count - 1)
                            ' TODO: Verificare se esiste ancora questo visualizzatore
                            If lastDocVersioning.CheckStatus.Eq("O") AndAlso DocSuiteContext.Current.User.FullUserName.Eq(lastDocVersioning.CheckOutUser) Then
                                Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()
                                Dim s As String = ProtocolEnv.VersioningShare & "\Docm\" & lastDocVersioning.CheckDir
                                If String.IsNullOrEmpty(Dir(s)) Then
                                    AjaxAlert("Il Documento sul Server non è valido.{0}Non è possibile Aprire il Documento.", Environment.NewLine)
                                    Exit Select
                                End If
                                If FileHelper.IsInUse(s) Then
                                    AjaxAlert("Il Documento è già Aperto.")
                                    Exit Select
                                End If
                                s = Replace(s, "\", "\\")
                                AjaxManager.ResponseScripts.Add("OpenFile('" & s & "')")
                                impersonator.ImpersonationUndo()
                                Exit Select
                            End If

                        End If

                        ' Loggo l'apertura del documento nella tabella di log delle pratiche
                        Dim incrementalFolder As Short
                        Short.TryParse(e.Item.Cells(0).Text, incrementalFolder)
                        Dim folderPath As String = ""
                        FncCalcolaCartella(DocumentFolder, folderPath, incrementalFolder)
                        Dim documentDescription As String = folderPath & "/" & DirectCast(e.CommandSource, LinkButton).Text

                        Facade.DocumentLogFacade.Insert(Document.Year, Document.Number, "DD", documentDescription)

                        Dim location As Location = Document.Location
                        Dim doc As New BiblosDocumentInfo(location.DocumentServer, location.DocmBiblosDSDB, docObject.idBiblos)

                        Dim parameters As String = String.Format("servername={0}&guid={1}&label={2}&prefixFileName={3}", doc.Server, doc.ChainId, "Documenti", _
                                                                  String.Concat("PR_", Document.Year, "_", Document.Number.ToString("0000000")))

                        Dim viewerUrl As String = "~/Viewers/BiblosViewer.aspx?" & CommonShared.AppendSecurityCheck(parameters)
                        Response.Redirect(viewerUrl)
                    Case "LP"
                        Dim a As String() = Split(docObject.Link, "|")
                        Dim s As String = "Year=" & a(0) & "&Number=" & a(1)
                        Response.Redirect("../Prot/ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck(s))
                    Case "LF"
                        Dim a As String() = Split(docObject.Link, "|")
                        Dim s As String = "Year=" & a(0) & "&idSubCategory=" & a(1) & "&Incremental=" & a(2)
                        Response.Redirect("../Fasc/FascVisualizza.aspx?" & CommonShared.AppendSecurityCheck(s))
                    Case "LR"
                        Dim a As String() = Split(docObject.Link, "|")
                        Dim s As String = "Type=Resl&idResolution=" & a(0)
                        Response.Redirect(String.Format("../Resl/{0}?{1}", ReslBasePage.GetViewPageName(a(0)), CommonShared.AppendSecurityCheck(s)))

                End Select

            Case "Vers:"
                If DocSuiteContext.Current.DocumentEnv.IsVersioningEnabled Then
                    Dim yniKey As New YearNumberIncrCompositeKey(commandYear, commandNumber, incrementalObject)
                    If Facade.DocumentVersioningFacade.GetVersioningForDocumentObject(yniKey) Then
                        Response.Redirect("DocmVersioning.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Docm&Year={0}&Number={1}&IncrementalObject={2}", commandYear, commandNumber, incrementalObject)))
                    End If
                End If

            Case "Selz:"
                Dim yniKey As New YearNumberIncrCompositeKey(commandYear, commandNumber, incrementalObject)
                Dim docIncrementalObject As DocumentObject = Facade.DocumentObjectFacade.GetById(yniKey)
                If Not docIncrementalObject Is Nothing Then
                    Dim pageName As String = String.Empty
                    Select Case docIncrementalObject.idObjectType
                        Case "FL" : pageName = "DocmFile.aspx"
                        Case "LP" : pageName = "DocmProtocollo.aspx"
                        Case "LF" : pageName = "DocmFascicolo.aspx"
                        Case "LR" : pageName = "DocmAtti.aspx"
                    End Select
                    Dim s As String = String.Format("Year={0}&Number={1}&Incremental={2}&Type=Docm&IncrementalObject={3}&Action=Modify&Add={4}&Refresh={5}", commandYear, commandNumber, Incremental, incrementalObject, Add, btnRefresh.ClientID)
                    Response.Redirect(pageName & "?" & CommonShared.AppendSecurityCheck(s))
                End If

            Case "opUp:"
                Dim yniKey As New YearNumberIncrCompositeKey(commandYear, commandNumber, incrementalObject)
                If Facade.DocumentObjectFacade.SwapDocObjectPosition("MOVEUP", yniKey) Then
                    Initialize()
                Else
                    AjaxAlert("Si è verificato un errore nell\'aggiornamento della Posizione")
                End If

            Case "opDn:"
                Dim yniKey As New YearNumberIncrCompositeKey(commandYear, commandNumber, incrementalObject)
                If Facade.DocumentObjectFacade.SwapDocObjectPosition("MOVEDOWN", yniKey) Then
                    Initialize()
                Else
                    AjaxAlert("Si è verificato un errore nell\'aggiornamento della Posizione")
                End If

            Case "Mail:"
                Dim s As String = Right(e.CommandName, Len(e.CommandName) - 5)

                AjaxManager.ResponseScripts.Add(String.Format("return OpenWindowMailCC('wndMailCC', 'Type=Docm&Year={0}&Number={1}&Folder={2}&File={3}');", Document.Id.Year.Value, Document.Id.Number.Value, Session("FOLDER"), s))

        End Select
    End Sub

    Private Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRefresh.Click
        Dim yeNuInc As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, Incremental)
        DocVersioned = Facade.DocumentObjectFacade.GetObjectsWithVersioning(yeNuInc, ListAllDocuments)

        If Not ListAllDocuments Then
            ViewState.Clear()
            If Document.Objects.Count > 0 Then
                If DocVersioned.Count > 0 Then
                    Dim dobStart, dobEnd As DocumentObject
                    dobStart = DocVersioned.Item(0)
                    dobEnd = DocVersioned.Item(DocVersioned.Count - 1)
                    ViewState.Add("opMin", dobStart.OrdinalPosition)
                    ViewState.Add("opMax", dobEnd.OrdinalPosition)
                Else
                    ViewState.Add("opMin", 0)
                    ViewState.Add("opMax", 0)
                End If
            End If
        End If

        Dim finder As NHibernateDocumentObjectFinder = Facade.DocumentObjectFinder
        finder.DocumentYear = CurrentDocumentYear
        finder.DocumentNumber = CurrentDocumentNumber
        finder.DocumentIncr = Incremental
        finder.TypeSearch = 1
        DG.Finder = finder
        DG.DataBindFinder()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(DG, DG)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnRefresh, DG)
    End Sub

    Private Sub InitializeColumns()
        ' Recupero da ParameterEnv l'elenco delle colonne da nascondere
        If String.IsNullOrEmpty(DocumentEnv.PraticheHiddenColumns) Then
            Exit Sub
        End If

        ' Cerco la colonna e la nascondo
        For Each item As String In DocumentEnv.PraticheHiddenColumns.Split("|"c)
            Dim col As GridColumn = DG.Columns.FindByUniqueNameSafe(item)
            If col IsNot Nothing Then
                col.Visible = False
            Else
                FileLogger.Warn(LoggerName, String.Format("Impossibile cancellare colonna documenti pratiche [{0}].", item))
            End If
        Next
    End Sub

    Private Sub Initialize()
        Dim titolo As String = String.Empty

        If ListAllDocuments Then
            lblTitolo.Text = "Pratica " & DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber)
            DG.Columns.FindByUniqueNameSafe("cSelezione").Visible = False
            DG.Columns.FindByUniqueNameSafe("cArrowDown").Visible = False
            DG.Columns.FindByUniqueNameSafe("cArrowUp").Visible = False
        Else
            FncCalcolaCartella(DocumentFolder, titolo, Incremental.Value)
            Session.Add("FOLDER", titolo)
            lblTitolo.Text = titolo
            DG.Columns.FindByUniqueNameSafe("cCartella").Visible = False
            If Not Add.Eq("ON") Then
                DG.Columns.FindByUniqueNameSafe("cArrowDown").Visible = False
                DG.Columns.FindByUniqueNameSafe("cArrowUp").Visible = False
            End If
        End If

        If Not String.IsNullOrEmpty(CurrentYear) And Not String.IsNullOrEmpty("txtSelNumber") Then
            Dim s As String = String.Format("&txtSelYear={0}&txtSelNumber={1}", CurrentYear, CurrentNumber)
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "OpenDocmProtocollo", String.Format("OpenWindow('../Docm/DocmProtocollo.aspx','windowDocmProt','Titolo=Aggiungi Collegamento Protocollo&{0}');", s), True)
        End If

        If DocSuiteContext.IsFullApplication AndAlso Add.Eq("ON") Then

            Dim s As String = CommonShared.AppendSecurityCheck(String.Format("Type=Docm&Year={0}&Number={1}&Incremental={2}&Action=Insert&Refresh={3}", CurrentDocumentYear(), CurrentDocumentNumber, Incremental, btnRefresh.ClientID))
            AddFile.OnClientClick = String.Format("return OpenWindowFullScreen('../Docm/DocmFile.aspx','windowDocmFile','Titolo=Aggiungi File&{0}&ManagerID={1}')", s, alertManager.ClientID)

            s = CommonShared.AppendSecurityCheck(String.Format("Type=Docm&Multiple=true&Year={0}&Number={1}&Incremental={2}&Action=Insert&Refresh={3}", CurrentDocumentYear(), CurrentDocumentNumber, Incremental, btnRefresh.ClientID))
            AddMultipleFile.OnClientClick = String.Format("return OpenWindow('../Docm/DocmFile.aspx','windowDocmFile','Titolo=Aggiungi File&{0}&ManagerID={1}')", s, alertManager.ClientID)

            AddProtocol.Visible = False
            AddFascicle.Visible = False
            AddResolution.Visible = False

            s = CommonShared.AppendSecurityCheck(String.Format("Type=Docm&Year={0}&Number={1}&Incremental={2}&Action=Insert&Refresh={3}", CurrentDocumentYear(), CurrentDocumentNumber, Incremental, btnRefresh.ClientID))

            If DocSuiteContext.Current.IsProtocolEnabled Then
                AddProtocol.OnClientClick = String.Format("return OpenWindow('../Docm/DocmProtocollo.aspx','windowDocmProt','Titolo=Aggiungi Collegamento Protocollo&{0}')", s)
                AddProtocol.Visible = True
                If ProtocolEnv.FascicleEnabled Then
                    'AddFascicle.OnClientClick = String.Format("return OpenWindow('../Docm/DocmFascicolo.aspx','windowDocmFile','Titolo=Aggiungi Collegamento Fascicolo&{0}')", s)
                    'AddFascicle.Visible = True
                End If
            End If

            If DocSuiteContext.Current.IsResolutionEnabled Then
                AddResolution.OnClientClick = String.Format("return OpenWindow('../Docm/DocmAtti.aspx','windowDocmFile','Titolo=Aggiungi Collegamento Atti&{0}')", s)
                AddResolution.Visible = True
            End If
        Else
            AddToolbar.Visible = False
        End If

        Dim yeNuInc As New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, Incremental)
        DocVersioned = Facade.DocumentObjectFacade.GetObjectsWithVersioning(yeNuInc, ListAllDocuments)

        If Not ListAllDocuments Then
            ViewState.Clear()
            If Document.Objects.Count > 0 Then
                Dim dobStart, dobEnd As DocumentObject

                If DocVersioned.Count > 0 Then
                    dobStart = DocVersioned.Item(0)
                    dobEnd = DocVersioned.Item(DocVersioned.Count - 1)
                    ViewState.Add("opMin", dobStart.OrdinalPosition)
                    ViewState.Add("opMax", dobEnd.OrdinalPosition)
                Else
                    ViewState.Add("opMin", 0)
                    ViewState.Add("opMax", 0)
                End If
            End If
        End If

        AddMultipleFile.Visible = DocumentEnv.MultipleUploadEnabled

        Dim finder As NHibernateDocumentObjectFinder = Facade.DocumentObjectFinder
        finder.TypeSearch = 1
        finder.DocumentYear = CurrentDocumentYear
        finder.DocumentNumber = CurrentDocumentNumber
        finder.DocumentIncr = Incremental
        DG.Finder = finder
        DG.DataBindFinder()
    End Sub

    Protected Function IsSignedIcon(ByVal description As String) As String
        Return If(FileHelper.MatchExtension(description, FileHelper.P7M), ImagePath.SmallSigned, ImagePath.SmallEmpty)
    End Function

    Public Function SetupFolder(ByVal incrementalFolder As Short) As String
        If Not ListAllDocuments Then
            Return String.Empty
        End If

        Dim incrementalDocumentFolder As DocumentFolder = Facade.DocumentFolderFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, incrementalFolder)
        Dim returnTitle As String = String.Empty
        FncCalcolaCartella(incrementalDocumentFolder, returnTitle, incrementalFolder)
        Return returnTitle
    End Function

    ''' <summary> Crea il contenuto della cella description della datagrid </summary>
    ''' <param name="ObjectType">Il campo idObjectType di DocumentObject </param>
    ''' <param name="Description">Il campo Description di DocumentObject</param>
    ''' <param name="Link">Il campo Link di DocumentObject</param>
    Public Function SetupDescription(ByVal objectType As String, ByVal description As String, ByVal link As String) As String
        Dim a As String() = Split(link, Pipe)

        Dim s As String = String.Empty
        Select Case objectType
            Case "FL"
                s = description
            Case "LP"
                s = String.Format("{0}{1}{2:0000000}{3}{4}", a(0), Slash, a(1), WebHelper.Br, a(3))
            Case "LF"
                s = ComposeCode(a(1))
            Case "LR"
                s = a(0)

                Dim res As Resolution = Facade.ResolutionFacade.GetById(Int32.Parse(s))

                Dim nr As String = ""
                If res IsNot Nothing Then
                    If res.Year.HasValue Then
                        nr &= res.Year.Value.ToString() & " "
                    End If
                    If Not res.ServiceNumber Is Nothing Then
                        nr &= res.ServiceNumber
                    ElseIf res.Number.HasValue Then
                        nr &= res.Number.Value.ToString()
                    End If
                End If
                s = If(nr = "", "*", nr)
        End Select
        Return s
    End Function

    Private Function ComposeCode(ByVal idSubcategory As Integer) As String
        Dim category As Category = Facade.CategoryFacade.GetById(idSubcategory)

        Dim codeString As String = ""
        If category.Parent IsNot Nothing Then
            codeString &= ComposeCode(category.Parent.Id) & "."
        End If

        codeString &= category.Code

        Return codeString
    End Function

    Public Function SetupObject(ByVal objectType As String, ByVal sObject As String, ByVal Link As String) As String
        Dim s As String = String.Empty

        Select Case objectType
            Case "FL"
                s = sObject

            Case "LP"
                If DocSuiteContext.Current.IsProtocolEnabled Then
                    Dim a As String() = Split(Link, "|")
                    Dim protocolYear As Short = Short.Parse(a(0))
                    Dim protocolNumber As Integer = Int32.Parse(a(1))

                    Dim prot As Protocol = Facade.ProtocolFacade.GetById(protocolYear, protocolNumber)
                    If prot IsNot Nothing Then
                        s = "" & prot.ProtocolObject
                    End If
                End If

            Case "LF"
                If ProtocolEnv.FascicleEnabled Then
                    Dim a As String() = Split(Link, "|")
                    Dim fascicleYear As Short = Short.Parse(a(0))
                    Dim idCategory As Integer = Integer.Parse(a(1))
                    Dim fascicleNumber As Integer = Int32.Parse(a(2))

                    Dim fasc As Fascicle = New FascicleFacade().GetByYearNumberCategory(fascicleYear, idCategory, fascicleNumber)
                    If Not fasc Is Nothing Then
                        s = "" & fasc.FascicleObject
                    End If
                End If

            Case "LR"
                If DocSuiteContext.Current.IsResolutionEnabled Then
                    Dim a As String() = Split(Link, "|")

                    Dim idResolution As Integer = Integer.Parse(a(0))
                    Dim resl As Resolution = New ResolutionFacade().GetById(idResolution)

                    If Not resl Is Nothing Then
                        s = "" & resl.ResolutionObject 'resl.Object
                    End If
                End If

        End Select
        Return s
    End Function

    Public Sub FncCalcolaCartella(ByVal docFold As DocumentFolder, ByRef titolo As String, ByVal newIncremental As Short)
        If docFold Is Nothing Then
            Exit Sub
        End If

        Dim yearNumberInc As New YearNumberIncrCompositeKey(docFold.Year, docFold.Number, newIncremental)
        Dim folder As DocumentFolder = Facade.DocumentFolderFacade.GetById(yearNumberInc)
        If folder Is Nothing Then
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(titolo) Then
            titolo = Slash & titolo
        End If
        If folder.Role Is Nothing Then
            titolo = folder.FolderName & titolo
        Else
            titolo = folder.Role.Name & titolo
        End If
        If folder.IncrementalFather.HasValue Then
            FncCalcolaCartella(folder, titolo, folder.IncrementalFather.Value)
        End If

    End Sub

#End Region

End Class
