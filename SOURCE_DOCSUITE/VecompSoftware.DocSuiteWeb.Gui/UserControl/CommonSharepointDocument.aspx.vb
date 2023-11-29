Imports VecompSoftware.Helpers.ExtensionMethods
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Sharepoint
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports System.Web
Imports VecompSoftware.Helpers
Imports Microsoft.SharePoint.Client
Imports VecompSoftware.DocSuiteWeb.DTO.Protocols
Imports VecompSoftware.Services.Logging
Imports System.Net
Imports VecompSoftware.Helpers.Compress

Public Class CommonSharepointDocument
    Inherits CommBasePage

#Region " Fields "

    Public Const AllowedExtensionsQueryItem As String = "allowedextensions"
    Private Const MultipleSelectionQueryItem As String = "MultiDoc"
    Private _allowedExtensions As List(Of String)
    Private _fileExtensionWhiteList As List(Of String)
    Private _fileExtensionBlackList As List(Of String)
    Private _clientSideFileExtensionBlackList As String
    Private _nameDocumentLibrarySharepoint As String
    Private _sharepointServerUrl As String
    Private _gridRows As List(Of SharepointResult)
    Private _fileNameToDownload As String

#End Region

#Region " Properties "
    Public Property SharepointServerUrl() As String
        Get
            Return Me.ViewState("SharepointServerUrl").ToString
        End Get
        Set(ByVal value As String)
            Me.ViewState.Add("SharepointServerUrl", value)
        End Set
    End Property


    Public Property NameDocumentLibrerySharepoint() As String
        Get
            Return Me.ViewState("NameDocumentLibrerySharepoint").ToString
        End Get
        Set(ByVal value As String)
            Me.ViewState.Add("NameDocumentLibrerySharepoint", value)
        End Set
    End Property

    Private ReadOnly Property MultipleSelection As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)(MultipleSelectionQueryItem, False)
        End Get
    End Property

    Private ReadOnly Property AllowedExtensions As List(Of String)
        Get
            If _allowedExtensions Is Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString.GetValueOrDefault(Of String)(AllowedExtensionsQueryItem, Nothing)) Then
                _allowedExtensions = New List(Of String)(Request.Item(AllowedExtensionsQueryItem).ToLowerInvariant().Split(","c))
            End If
            Return _allowedExtensions
        End Get
    End Property

    Private ReadOnly Property FileExtensionWhiteList As List(Of String)
        Get
            If _fileExtensionWhiteList Is Nothing Then
                If AllowedExtensions IsNot Nothing Then
                    _fileExtensionWhiteList = New List(Of String)(AllowedExtensions)
                    If _fileExtensionWhiteList.Contains(FileHelper.ZIP) AndAlso DocSuiteContext.Current.ProtocolEnv.UploadZipManaged AndAlso Not MultipleSelection Then
                        _fileExtensionWhiteList.Remove(FileHelper.ZIP)
                    End If

                ElseIf Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.FileExtensionWhiteList) Then
                    Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.FileExtensionWhiteList.ToLowerInvariant().Split("|"c)
                    _fileExtensionWhiteList = New List(Of String)(splitted)

                    ' Considero le sole estensioni non menzionate in BlackList.
                    _fileExtensionWhiteList = _fileExtensionWhiteList.Where(Function(white) Not FileExtensionBlackList.Contains(white)).ToList()

                    If DocSuiteContext.Current.ProtocolEnv.UploadZipManaged AndAlso MultipleSelection Then
                        _fileExtensionWhiteList.Add(FileHelper.ZIP)
                    End If
                End If
            End If
            Return _fileExtensionWhiteList
        End Get
    End Property

    Private ReadOnly Property FileExtensionBlackList As List(Of String)
        Get
            If _fileExtensionBlackList Is Nothing AndAlso AllowedExtensions Is Nothing AndAlso Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.FileExtensionBlackList) Then

                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.FileExtensionBlackList.ToLowerInvariant().Split("|"c)
                _fileExtensionBlackList = New List(Of String)(splitted)
            End If
            Return _fileExtensionBlackList
        End Get
    End Property

    Public ReadOnly Property ClientSideFileExtensionBlackList As String
        Get
            If String.IsNullOrEmpty(_clientSideFileExtensionBlackList) Then
                _clientSideFileExtensionBlackList = String.Empty
                If FileExtensionBlackList IsNot Nothing Then
                    Dim bl As New List(Of String)(FileExtensionBlackList)
                    If DocSuiteContext.Current.ProtocolEnv.UploadZipManaged AndAlso MultipleSelection Then
                        bl.Remove(FileHelper.ZIP)
                    End If
                    _clientSideFileExtensionBlackList = String.Join("|", bl)
                End If
            End If
            Return _clientSideFileExtensionBlackList
        End Get
    End Property

    Public ReadOnly Property WarningUploadThreshold As Integer
        Get
            Return ProtocolEnv.WarningUploadThreshold
        End Get
    End Property

    Public ReadOnly Property WarningUploadThresholdType As String
        Get
            Return ProtocolEnv.WarningUploadThresholdType
        End Get
    End Property

#End Region

#Region " Events "

    ''' <summary>
    ''' ItemDataBound rddDocumentLibrary controllo che carica l'elenco degli uri abilitati per l'utente corrente
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rddDocumentLibrary_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.DropDownListItemEventArgs) Handles rddDocumentLibrary.ItemDataBound
        InitializeSharepointUri()
    End Sub

    ''' <summary>
    ''' ItemDataBound dgSharepointFiles controllo che carica la griglia dei file per la selezione della treeview
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub dgSharepointFiles_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles dgSharepointFiles.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If
        If RadTreeViewFolder.SelectedNode Is Nothing Then
            Exit Sub
        End If
        Dim row As SharepointResult = DirectCast(e.Item.DataItem, SharepointResult)

        With DirectCast(e.Item.FindControl("lblFileName"), Label)
            .Text = row.FileName
        End With

        With DirectCast(e.Item.FindControl("imgFile"), Image)
            .ImageUrl = ImagePath.FromFile(row.FileName, True)
        End With

        With DirectCast(e.Item.FindControl("lblFileModify"), Label)
            .Text = row.Modified.ToString
        End With

    End Sub

    ''' <summary>
    ''' Al momento della selezione imposta il valore della proprietà NameDocumentLibrerySharepoint Cioè il nome della DocumentLibrary
    ''' </summary>
    Private Sub EnableViewFolders()
        Dim selectedItem As DropDownListItem = rddDocumentLibrary.SelectedItem
        NameDocumentLibrerySharepoint = selectedItem.Text
        SharepointServerUrl = GetSharepointServerUrl(NameDocumentLibrerySharepoint)
    End Sub

    ''' <summary>
    ''' Action del bottone conferma dove si visualizza la document library di sharepoint
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        If String.IsNullOrEmpty(rddDocumentLibrary.SelectedText) OrElse rddDocumentLibrary.Items.Count.Equals(0) Then
            AjaxAlert("Nessuna document library presente.")
            Exit Sub
        End If

        EnableViewFolders()
        BindNavigationTree(NameDocumentLibrerySharepoint)
    End Sub

    ''' <summary>
    ''' Page Load
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            ' Inizializzo la raddropdown con l'elenco degli sharepoint disponibili per l'utente corrente
            GridBinding()
            InitializeSharepointUri()

        End If
        MasterDocSuite.TitleVisible = False
    End Sub



    ''' <summary>
    ''' Sub che esegue il download dello stream del file da Sharepoint e la gestione di serializzazione del json contenente il dati del file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub cmdConfirm_Click(sender As Object, e As EventArgs) Handles cmdConfirm.Click
        Dim selectedFileName As String = String.Empty
        Dim selectedItem As GridDataItem = Nothing
        If dgSharepointFiles.MasterTableView.GetSelectedItems() IsNot Nothing Then
            selectedItem = dgSharepointFiles.MasterTableView.GetSelectedItems().FirstOrDefault()
        End If

        If (selectedItem IsNot Nothing AndAlso selectedItem.FindControl("lblFileName") IsNot Nothing) Then
            selectedFileName = DirectCast(selectedItem.FindControl("lblFileName"), Label).Text
        End If

        If selectedItem Is Nothing OrElse String.IsNullOrEmpty(selectedFileName) Then
            AjaxAlert("Nessun documento selezionato.")
            Exit Sub
        End If

        ' scarico lo stream da sharepoint
        Using clientcontext As ClientContext = New ClientContext(SharepointServerUrl)

            Dim fileName As String = Path.GetFileName(FileHelper.ReplaceUnicode(selectedFileName))
            Dim targetFileName As String = FileHelper.UniqueFileNameFormat(fileName, DocSuiteContext.Current.User.UserName)
            Dim targetPath As String = Path.Combine(CommonUtil.GetInstance().AppTempPath, targetFileName)

            Try
                Dim sharepointHelper As SharepointHelper = New SharepointHelper()
                Dim url As String = String.Format("{0}/{1}/{2}", SharepointServerUrl, RadTreeViewFolder.SelectedNode.FullPath, selectedFileName)
                sharepointHelper.DownloadStreamFile(url, targetPath)
            Catch ex As PathTooLongException
                Dim theoricalMaxLenght As Integer = FileHelper.MaxFullyQualifiedNameLength - CommonUtil.GetInstance().AppTempPath.Length - FileHelper.UniqueFileNameFormat(String.Empty, DocSuiteContext.Current.User.UserName).Length
                Throw New DocSuiteException("Errore inserimento", String.Format("Il nome del file è troppo lungo:  [{0}] caratteri di [{1}] possibili.", fileName.Length, theoricalMaxLenght), ex)
            End Try
            Dim serializeMe As New Dictionary(Of String, String)
            Dim invalidEntries As New List(Of String)
            Dim warningSizeEntries As New List(Of String)

            If (FileHelper.MatchExtension(selectedFileName, FileHelper.ZIP) OrElse FileHelper.MatchExtension(selectedFileName, FileHelper.RAR)) AndAlso DocSuiteContext.Current.ProtocolEnv.UploadZipManaged AndAlso MultipleSelection Then
                Dim prefix As String = FileHelper.UniquePrefixFormat(DocSuiteContext.Current.User.UserName)
                Try
                    Dim compressManager As ICompress = New ZipCompress()
                    If FileHelper.MatchExtension(selectedFileName, FileHelper.RAR) Then
                        compressManager = New RarCompress()
                    End If

                    For Each contentFileName As String In compressManager.GetContentFileNames(targetPath)
                        Dim extension As String = Path.GetExtension(contentFileName).ToLowerInvariant()
                        If FileExtensionWhiteList IsNot Nothing AndAlso Not chkDisableFileExtensionWhiteList.Checked AndAlso Not FileExtensionWhiteList.Contains(extension) Then
                            invalidEntries.Add(contentFileName)
                            Continue For
                        End If
                        If FileExtensionBlackList IsNot Nothing AndAlso FileExtensionBlackList.Contains(extension) Then
                            invalidEntries.Add(contentFileName)
                            Continue For
                        End If
                        serializeMe.Add(prefix & contentFileName, contentFileName)
                    Next

                    compressManager.Extract(targetPath, CommonUtil.GetInstance().AppTempPath, prefix)
                Catch ex As ExtractException
                    AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
                    Exit Sub
                End Try
            ElseIf FileHelper.MatchExtension(fileName, FileHelper.ZIP) Then
                invalidEntries.Add(fileName)
            Else
                serializeMe.Add(targetFileName, fileName)
            End If

            If invalidEntries.Count > 0 Then
                AjaxAlert(String.Format("I seguenti file non presentano un'estensione valida al caricamento: {0}", String.Join(", ", invalidEntries)))
            End If

            If WarningUploadThresholdType = "Alert" AndAlso warningSizeEntries.Count > 0 Then
                AjaxAlert(String.Format("I seguenti file risultano avere una grandezza elevata e potrebbero rallentare i servizi:{0}{1}{0}{0}Si consiglia, se possibile, di utilizzare versioni con dimensione minore.", Environment.NewLine, String.Join(Environment.NewLine, warningSizeEntries)))
            End If

            Dim serialized As String = HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(serializeMe))
            AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", serialized))
        End Using
    End Sub

    ''' <summary>
    ''' Metodo che esegue il bind al clieck della treevie delle folder
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub RadTreeViewFolder_NodeClick(sender As Object, e As RadTreeNodeEventArgs) Handles RadTreeViewFolder.NodeClick
        GridBinding()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(rddDocumentLibrary, rddDocumentLibrary, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewFolder, dgSharepointFiles, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgSharepointFiles, dgSharepointFiles, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    ''' <summary>
    ''' Function che al click di un node della Radtreeview estrae da Sharepoint i file da visualizzare in griglia
    ''' </summary>
    ''' <param name="SelectedNode"></param>
    ''' <returns></returns>
    Private Function GetFilesFromSelectedFolder(ByVal SelectedNode As RadTreeNode) As List(Of SharepointResult)

        Using clientcontext As New ClientContext(SharepointServerUrl)
            Dim filegrid As New List(Of SharepointResult)
            'Load Libraries from SharePoint
            clientcontext.Load(clientcontext.Web.Lists)
            clientcontext.ExecuteQuery()
            Dim List As List = clientcontext.Web.GetList(NameDocumentLibrerySharepoint)
            Dim camlQuery As New CamlQuery()
            camlQuery.ViewXml = "<View Scope='FilesOnly' />"
            camlQuery.FolderServerRelativeUrl = String.Format("/{0}", SelectedNode.FullPath)
            Dim listItems As ListItemCollection = List.GetItems(camlQuery)
            clientcontext.Load(listItems)
            clientcontext.ExecuteQuery()
            FileLogger.Debug(LoggerName, "Execute query Sharepoint document library. ")

            For Each file As ListItem In listItems
                Try

                    FileLogger.Debug(LoggerName, "Execute query path della Sharepoint document library. ")
                    Dim filetoAdd As New SharepointResult
                    filetoAdd.FileName = file("FileLeafRef").ToString
                    filetoAdd.Modified = Convert.ToDateTime(file("Last_x0020_Modified"))
                    filegrid.Add(filetoAdd)
                    FileLogger.Debug(LoggerName, String.Format("Execute query Sharepoint document library. {0}", filetoAdd.FileName))

                Catch ex As Exception
                    FileLogger.Error(LoggerName, String.Format("Durante la lettura dei listitem in path di sharepoint. {0}", file.ToString), ex)
                End Try
            Next

            Return filegrid

        End Using
    End Function

    Private Function GetSharepointServerUrl(ByVal NameDocumentLibrarySharepoint As String) As String
        Dim baseUri As New Uri(NameDocumentLibrarySharepoint)
        Return baseUri.AbsoluteUri.Replace(baseUri.AbsolutePath, "")
    End Function


    ''' <summary>
    ''' Metodo per il bind delle Document library di sharepoint sul controllo RadTreeViewFolder
    ''' </summary>
    ''' <param name="NameDocumentLibrarySharepoint"></param>
    Private Sub BindNavigationTree(ByVal NameDocumentLibrarySharepoint As String)
        RadTreeViewFolder.Nodes.Clear()

        Try
            Using clientcontext As New ClientContext(SharepointServerUrl)
                clientcontext.Credentials = New NetworkCredential()
                'Load Libraries from SharePoint
                clientcontext.Load(clientcontext.Web.Lists)
                clientcontext.ExecuteQuery()
                Dim List As List = clientcontext.Web.GetList(NameDocumentLibrarySharepoint)
                clientcontext.Load(List)
                clientcontext.ExecuteQuery()
                clientcontext.Load(List.RootFolder.Folders)
                clientcontext.ExecuteQuery()

                Dim LibraryNode As New RadTreeNode(List.Title)
                LibraryNode.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/folder.png"
                RadTreeViewFolder.Nodes.Add(LibraryNode)
                For Each subFolder As Folder In List.RootFolder.Folders.Where(Function(f) Not f.Name.Eq("Forms"))
                    Dim MainNode As New RadTreeNode(subFolder.Name)
                    LibraryNode.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/folder.png"
                    MainNode.PostBack = True
                    LibraryNode.Nodes.Add(MainNode)
                    FillTreeViewNodes(subFolder, MainNode, clientcontext)
                Next
            End Using
        Catch ex As Exception
            AjaxAlert("Nessun documento selezionato.")
            FileLogger.Error(LoggerName, String.Format("Errore nel caricamento delle folder. {0}", ex))
            Exit Sub
        End Try

    End Sub

    ''' <summary>
    ''' Sub per il fill dei node della radtreeview
    ''' </summary>
    ''' <param name="SubFolder"></param>
    ''' <param name="MainNode"></param>
    ''' <param name="clientcontext"></param>
    Private Sub FillTreeViewNodes(SubFolder As Folder, MainNode As RadTreeNode, clientcontext As ClientContext)
        clientcontext.Load(SubFolder.Folders)
        clientcontext.ExecuteQuery()
        For Each Fol As Folder In SubFolder.Folders
            Dim SubNode As New RadTreeNode(Fol.Name)
            SubNode.PostBack = True
            MainNode.Nodes.Add(SubNode)
            FillTreeViewNodes(Fol, SubNode, clientcontext)
        Next
    End Sub

    ''' <summary>
    '''  Sub che esegue il databind degli uri di sharepoint in base al Settore cui appartiene l'utente corrente
    ''' </summary>
    ''' <returns></returns>
    Private Sub InitializeSharepointUri()
        rddDocumentLibrary.Items.Clear()
        Dim targetRoles As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Any, Nothing, True, CurrentTenant.TenantAOO.UniqueId)
        Dim dropDownListItem As DropDownListItem = Nothing
        Dim coll As SortedDictionary(Of String, DropDownListItem) = New SortedDictionary(Of String, DropDownListItem)
        If targetRoles IsNot Nothing Then
            For Each role As Role In targetRoles.Where(Function(x) Not String.IsNullOrEmpty(x.UriSharepoint))
                If Not coll.ContainsKey(role.UriSharepoint) Then
                    dropDownListItem = New DropDownListItem(role.UriSharepoint, role.UriSharepoint)
                    coll.Add(role.UriSharepoint, dropDownListItem)
                    rddDocumentLibrary.Items.Add(dropDownListItem)
                End If
            Next
        End If

        rddDocumentLibrary.DataBind()

    End Sub


    ''' <summary>
    ''' Sub che Esegue  il binding della grid in seguito all'evento di selezione sulla RadTreeViewFolder
    ''' </summary>
    Private Sub GridBinding()
        If RadTreeViewFolder.SelectedNode Is Nothing Then
            Exit Sub
        End If
        dgSharepointFiles.DataSource = GetFilesFromSelectedFolder(RadTreeViewFolder.SelectedNode)
        dgSharepointFiles.DataBind()
    End Sub

#End Region

End Class