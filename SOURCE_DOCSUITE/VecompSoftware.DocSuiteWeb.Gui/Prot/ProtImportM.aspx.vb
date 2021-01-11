Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers


Partial Public Class ProtImportM
    Inherits ProtBasePage

#Region "Fields"
    Private task As MultiStepLongRunningTask
#End Region

#Region "QueryString"
    Private ReadOnly Property ImportFile() As String
        Get
            Dim s As String = Request.QueryString("ImportFile")
            Return If(String.IsNullOrEmpty(s), String.Empty, s)
        End Get
    End Property
#End Region

#Region "Load Page"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeButtons()
        InitializeAjaxSettings()
        If Not Me.IsPostBack Then
            Initialize()
        End If
        InitializeTask()
    End Sub
#End Region

#Region "AjaxRequest"
    Private Sub AjaxRequest_TaskCompleted(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        VerifyImportComplete()
    End Sub
#End Region

#Region "Initialize"
    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, GrResults)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, btnInserimento)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, btnDocumenti)
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest_TaskCompleted
    End Sub

    Private Sub InitializeButtons()
        WindowBuilder.RegisterWindowManager(RadWindowManagerImport)
        WindowBuilder.RegisterOpenerElement(btnDocumenti)
        WindowBuilder.RegisterOpenerElement(btnInserimento)
    End Sub

    Private Sub InitializeTask()
        task = GlobalAsax.LongRunningTask

        If (task.Running) Then
            SetupPageWithTaskRunning()
        Else
            SetupPageWithTaskNotRunning()
        End If
    End Sub

    Private Sub Initialize()

        Dim chiave As String = "FastInputImportPath"
        Dim PathImport As String = ProtocolEnv.FastInputImportPath
        If ImportFile.ToUpper = "EXCEL" Then
            PathImport = ProtocolEnv.ExcelPathImport
            chiave = "ExcelImport"
            Me.Title = "Protocollo - Importazione Massiva Excel (" & PathImport & ")"
        Else
            Me.Title = "Protocollo - Importazione Massiva (" & PathImport & ")"
        End If

        If PathImport = "" Then
            Throw New DocSuiteException(
                "Importazione Massiva",
                String.Format("In ParameterEnv Manca la definizione della chiave  {0}", chiave),
                Request.Url.ToString(),
                DocSuiteContext.Current.User.FullUserName)
        End If
        If Dir(PathImport, FileAttribute.Directory) = "" Then
            Throw New DocSuiteException(
                "Importazione Fatture",
                String.Format("La Directory {0} non è Valida. Verificare in ParamerEnv la defizione della chiave {1}", PathImport, chiave),
                Request.Url.ToString(),
                DocSuiteContext.Current.User.FullUserName)
        End If
        '---
        'pulizia temp
        CommonInstance.UserDeleteTemp(TempType.I)
    End Sub
#End Region

#Region "Setup Task"
    Private Sub SetupPageWithTaskRunning()
        EnablePageControls(True)
        Dim URL As String = "..\Prot\ProtImportProgress.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&Title=Importazione Massiva")
        WindowBuilder.LoadWindow("wndProgress", URL, "onTaskCompleted", Unit.Pixel(500), Unit.Pixel(250))
    End Sub

    Private Sub SetupPageWithTaskNotRunning()
        EnablePageControls(False)
        'GlobalAsax.LongRunningTask.UpdateControl = Nothing
        'uscProgressBarImpot.TimerEnabled = False
    End Sub
#End Region

#Region "Enable/Disable Controls"
    Protected Sub EnablePageControls(ByVal IsTaskRunning As Boolean)
        btnDocumenti.Enabled = Not IsTaskRunning
        btnInserimento.Enabled = Not IsTaskRunning
    End Sub
#End Region

#Region "Verify Import Complete"
    Private Sub VerifyImportComplete()
        If GlobalAsax.LongRunningTask.LastTaskSuccess Then
            If Not CommImport.ErrorsTable Is Nothing AndAlso CommImport.ErrorsTable.Rows.Count > 0 Then
                GrResults.DataSource = CommImport.ErrorsTable
                GrResults.DataBind()
                AjaxAlert("Si sono verificati degli errori durante il processo d\'importazione. Consultare il log d\'importazione per verifiche.\n\nDocumenti Validi: " & CommImport.Imported & "\nDocumenti Scartati: " & CommImport.Errors)
                GrResults.Visible = True
            Else
                AjaxAlert("Importazione eseguita con successo\n\nImportati Documenti " & CommImport.Imported)
                GrResults.Visible = False
            End If
            If CommImport.Errors + CommImport.Imported = 0 Then
                AjaxAlert("Directory di importazione vuota")
                GrResults.Visible = False
                Exit Sub
            End If
        Else
            AjaxAlert("Si sono verificati degli errori durante il processo d\'importazione.\nEccezzione sollevata: " & StringHelper.ReplaceAlert(GlobalAsax.LongRunningTask.ExceptionOccured.Message))
            FileLogger.Warn(LoggerName, "Si è verificato un errore.", GlobalAsax.LongRunningTask.ExceptionOccured)
        End If
    End Sub
#End Region

#Region "DataGrid Events"
    Protected Sub RadGrid1_NeedDataSource(ByVal source As System.Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles GrResults.NeedDataSource
        GrResults.DataSource = CommImport.ResultTable
    End Sub
#End Region

#Region "Windows Opener Events"
    Private Sub btnDocumenti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocumenti.Click
        Dim URL As String = "../Prot/ProtImportList.aspx?" & CommonShared.AppendSecurityCheck("Titolo=Elenco File&ImportType=Massiva&Action=ALL")
        If ImportFile.ToUpper = "EXCEL" Then
            URL = "../Prot/ProtImportList.aspx?" & CommonShared.AppendSecurityCheck("Titolo=Elenco File&ImportType=Excel&Action=ALL")
        End If
        WindowBuilder.LoadWindow("wndResult", URL, "", Unit.Pixel(700), Unit.Pixel(500))
    End Sub
#End Region

#Region "Do Import"
    Private Sub btnInserimento_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserimento.Click
        If Not task.Running Then
            Dim oImport As CommImport

            If ImportFile.ToUpper = "EXCEL" Then
                oImport = New CommImport("", "Excel")
            Else
                oImport = New CommImport("", "Massiva")
            End If


            If oImport.InserimentoMassiva(DocSuiteContext.Current.User.UserName) Then
                Me.SetupPageWithTaskRunning()
            Else
                AjaxAlert("Nessun file da importare.")
            End If
        End If
    End Sub
#End Region

End Class