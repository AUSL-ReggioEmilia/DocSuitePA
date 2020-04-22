Imports System.Collections.Generic
Imports NHibernate.Mapping
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.IO
Imports System.Globalization
Imports VecompSoftware.Services.Logging
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class ReslJournalSummary
    Inherits ReslBasePage

#Region "Fields"
    Private Const OpenWindowSign As String = "return {0}_OpenWindowSign('{1}', '{2}', '{3}');"
    Private _resolutionJournal As ResolutionJournal
#End Region

#Region " Properties "

    Private ReadOnly Property CurrentResolutionJournal As ResolutionJournal
        Get
            If (_resolutionJournal Is Nothing) Then
                Dim idResolutionJournal As Integer? = GetKeyValue(Of Integer?)("ResolutionJournal")
                If (idResolutionJournal.HasValue) Then
                    _resolutionJournal = Facade.ResolutionJournalFacade.GetById(idResolutionJournal.Value)
                    Return _resolutionJournal
                End If
                Throw New DocSuiteException("Errore in recupero Registro. ID mancante.")
            End If
            Return _resolutionJournal
        End Get
    End Property

    Private ReadOnly Property TitleString() As String
        Get
            Return Request.QueryString("TitleString")
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        InitializeButtons()
        If Not Page.IsPostBack Then
            Initialize()
            InitializeSignature()
            If (Not ResolutionEnv.UseWindowPreview) Then
                MostraRegistro()
            End If
        End If
    End Sub

    Private Sub Sign_Click(ByVal sender As Object, ByVal e As EventArgs)
        SignResolutionJournal()
    End Sub

    Private Sub Delete_Click(ByVal sender As Object, ByVal e As EventArgs)
        AjaxManager.ResponseScripts.Add("DeleteRegConfirm();")
    End Sub

    Private Sub Summary_Click(ByVal sender As Object, ByVal e As EventArgs)
        AjaxManager.ResponseScripts.Add(String.Format("location.href = '{0}/Resl/ReslJournal.aspx?Type=Resl&Group={1}&TitleString={2}'", DocSuiteContext.Current.CurrentTenant.DSWUrl, CurrentResolutionJournal.Template.TemplateGroup, Server.HtmlEncode(TitleString)))
    End Sub

    Protected Sub ShowClick(sender As Object, e As EventArgs) Handles Show.Click
        MostraRegistro()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf ReslJournalSummaryAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, Sign)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, SignatureDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, SignatureUser)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscViewerLight)
    End Sub

    Private Sub Initialize()
        Desc.Text = ResolutionJournalFacade.GetDescription(CurrentResolutionJournal)
        Template.Text = CurrentResolutionJournal.Template.Description
        Year.Text = CurrentResolutionJournal.Year.ToString()

        Dim monthName As String = CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames(CurrentResolutionJournal.Month - 1)
        Month.Text = StringHelper.UppercaseFirst(monthName)

        FirstPageNumberRow.Visible = CurrentResolutionJournal.Template.Pagination.GetValueOrDefault(False)
        LastPageNumberRow.Visible = CurrentResolutionJournal.Template.Pagination.GetValueOrDefault(False)
        FirstPageNumber.Text = CurrentResolutionJournal.FirstPage.ToString()
        LastPageNumber.Text = CurrentResolutionJournal.LastPage.ToString()

        CountResolutions.Text = CurrentResolutionJournal.Resolutions.Count.ToString()
    End Sub

    Private Sub InitializeSignature()
        If CurrentResolutionJournal.Signdate.HasValue Then
            SignatureDate.Text = CurrentResolutionJournal.Signdate.Value.ToString("dd/MM/yyyy HH:mm:ss")
            SignatureUser.Text = CurrentResolutionJournal.SignUser
        End If
    End Sub

    Private Sub InitializeButtons()
        WindowBuilder.RegisterWindowManager(RadWindowManager)
        AddHandler Summary.Click, AddressOf Summary_Click

        ' Firma del documento
        If CommonShared.UserConnectedBelongsTo(ResolutionEnv.ResolutionJournalSigners) Then
            If CurrentResolutionJournal.IDSignedDocument.HasValue AndAlso Not CurrentResolutionJournal.Template.MultipleSign.GetValueOrDefault(False) Then
                Sign.Enabled = False
            Else
                AddHandler Sign.Click, AddressOf Sign_Click
                WindowBuilder.RegisterOpenerElement(Sign)
            End If
        Else
            Sign.Visible = False
        End If

        If Facade.ResolutionJournalFacade.IsDeletable(CurrentResolutionJournal) Then
            AddHandler Delete.Click, AddressOf Delete_Click
            WindowBuilder.RegisterOpenerElement(Delete)
        Else
            Delete.Visible = False
        End If
        Show.Visible = ResolutionEnv.UseWindowPreview
    End Sub

    ''' <summary> Cancellazione </summary>
    Public Sub DeleteResolutionJournal()
        Try
            CurrentResolutionJournal.IsActive = 0
            Facade.ResolutionJournalFacade.DetachDocuments(CurrentResolutionJournal)
            Facade.ResolutionJournalFacade.Update(CurrentResolutionJournal)
            AjaxManager.ResponseScripts.Add(String.Format("location.href = '{0}/Resl/ReslJournal.aspx?Type=Resl&Group={1}&TitleString={2}'", DocSuiteContext.Current.CurrentTenant.DSWUrl, CurrentResolutionJournal.Template.TemplateGroup, Server.HtmlEncode(TitleString)))
        Catch ex As Exception
            AjaxAlert("Errore: '" & ResolutionJournalFacade.GetDescription(CurrentResolutionJournal) & "' non cancellato correttamente.")
        End Try
    End Sub

    ''' <summary> Firma del documento </summary>
    Private Sub SignResolutionJournal()
        Dim documentToSign As DocumentInfo
        Try
            documentToSign = Facade.ResolutionJournalFacade.ExtractToSign(CurrentResolutionJournal.Id)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert("Errore estrazione Documento: " & StringHelper.ReplaceAlert(ex.Message))
            Exit Sub
        End Try

        signDocument.OnClientClose = ID & "_CloseSignWindow"
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowSign, ID, "../Comm/SingleSign.aspx", signDocument.ClientID, documentToSign.ToQueryString().AsEncodedQueryString()))
    End Sub

    ''' <summary> Evento chiamato quando il documento è firmato </summary>
    Protected Sub ReslJournalSummaryAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        'argument(0) = ID controllo che ha effettuato chiamata ajax
        'argument(1) = comando chiamato
        Dim arguments As String() = Split(e.Argument, "|", 3)
        If (arguments(0) = Me.ClientID) Then
            Select Case arguments(1)
                Case "SIGN"
                    Dim signed As TempFileDocumentInfo = CType(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(arguments(2))), TempFileDocumentInfo)

                    FileLogger.Info(LoggerName, String.Format("[{0}.SIGN] fileName: {1}", ClientID, signed.Name))

                    If Not signed.FileInfo.Exists Then
                        AjaxAlert(String.Format("Documento firmato non è valido, reinserire il documento. {0}", ProtocolEnv.DefaultErrorMessage), False)
                        Exit Sub
                    End If

                    Dim docInfo As FileInfo = signed.FileInfo
                    Sign.Enabled = False

                    Try
                        ' Salvo su biblos                        
                        CurrentResolutionJournal.IDSignedDocument = Facade.ResolutionJournalFacade.SaveSignedDocument(CurrentResolutionJournal, docInfo)
                        ' Imposto la firma
                        Facade.ResolutionJournalFacade.SetSign(CurrentResolutionJournal, DocSuiteContext.Current.User.FullUserName, DateTime.Now)
                    Catch exception As Exception
                        FileLogger.Warn(LoggerName, String.Format("Impossibile inserire in Biblos il file '{0}': {1}", docInfo.Name, exception.Message), exception)
                        AjaxAlert(String.Format("Impossibile inserire in Biblos il file '{0}': {1}", docInfo.Name, exception.Message))
                    End Try

                    InitializeSignature()
                    If Not ResolutionEnv.UseWindowPreview Then
                        MostraRegistro(needRefresh:=True)
                    End If
                    AjaxManager.ResponseScripts.Add("ToggleToolBar(getToolBar(), true);")
                    AjaxAlert("Documento Firmato correttamente")
                Case "delete"
                    DeleteResolutionJournal()
            End Select
        End If
    End Sub

    Protected Sub MostraRegistro(Optional needRefresh As Boolean = False)
        Dim path As String = Me.ResolveUrl(Facade.ResolutionJournalFacade.ResolutionJournalViewerUrl(CurrentResolutionJournal))
        If (ResolutionEnv.UseWindowPreview) Then
            AjaxManager.ResponseScripts.Add(String.Format("{0}_OpenPreview('{1}','{2}','{3}');", Me.ID, path, DocSuiteContext.Current.ResolutionEnv.WindowWidthRegistri, DocSuiteContext.Current.ResolutionEnv.WindowHeightRegistri))
        Else
            Dim document As DocumentInfo = Facade.ResolutionJournalFacade.ExtractToSign(CurrentResolutionJournal.Id)
            uscViewerLight.DataSource = New List(Of DocumentInfo) From {document}
            If (needRefresh) Then
                uscViewerLight.Refresh()
            End If
        End If
    End Sub

#End Region

End Class