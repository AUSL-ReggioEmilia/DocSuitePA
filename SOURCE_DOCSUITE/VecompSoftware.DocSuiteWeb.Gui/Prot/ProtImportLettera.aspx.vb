Imports System.IO
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers

Public Class ProtImportLettera
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

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeButtons()
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
        InitializeTask()
    End Sub

#End Region

#Region "AjaxRequest"
    Private Sub AjaxRequest_TaskCompleted(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        VerifyImportComplete()
        Me.btnRisultati.Enabled = True
    End Sub
#End Region

#Region "Initialize"
    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, GrResults)
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest_TaskCompleted

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmbIdContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnRisultati)
    End Sub

    Private Sub InitializeButtons()
        WindowBuilder.RegisterWindowManager(RadWindowManagerImport)
        WindowBuilder.RegisterOpenerElement(btnDocumenti)
        WindowBuilder.RegisterOpenerElement(btnInserimento)
        WindowBuilder.RegisterOpenerElement(btnRisultati)
    End Sub

    Private Sub InitializeTask()
        task = GlobalAsax.LongRunningTask

        If (task.Running) Then
            SetupPageWithTaskRunning()
        End If
    End Sub

    Private Sub Initialize()
        cmbIdDocType.DataValueField = "Id"
        cmbIdDocType.DataTextField = "Description"

        'verifica PathImport
        Dim chiave As String = "WordPathImport"
        Dim PathImport As String = ProtocolEnv.WordPathImport
        If ImportFile.ToUpper = "EXCEL" Then
            PathImport = ProtocolEnv.ExcelPathImport
            chiave = "ExcelImport"
            btnDocumenti.Text = "Importazione Documenti"
            Title &= " Excel"
        End If

        ' Verifica che sia definita la cartella di importazione
        If String.IsNullOrEmpty(PathImport) Or PathImport = "" Then
            Throw New DocSuiteException("Importazione", String.Format("In ParamerEnv Manca la definizione della chiave {0}", chiave), Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        ' Verifica i permessi sulla cartella di importazione
        If Dir(PathImport, FileAttribute.Directory) = "" Then
            Throw New DocSuiteException("Importazione", String.Format("La Directory {0} non è Valida. Verificare in ParamerEnv la defizione della chiave {1}", PathImport, chiave), Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        lblTitle.Text = "Importazione" '"Protocollo - Importazione (" & PathImport & ")"
        lblDirectory.Text = PathImport

        ' Verifica diritti utente
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True)
        If containers.Count > 0 Then
            cmbIdContainer.DataSource = containers
            cmbIdContainer.DataValueField = "Id"
            cmbIdContainer.DataTextField = "name"
            cmbIdContainer.DataBind()
        Else
            Throw New DocSuiteException("Protocollo Inserimento", "Utente senza diritti di Inserimento", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        Dim files As String() = Directory.GetFiles(PathImport, "*.xls")

        If files.Length = 0 Then
            If ImportFile.ToUpper = "EXCEL" Then
                btnInserimento.Enabled = False
                lblExcel.Text = "Nessun file Excel presente nella directory"
            Else
                ' Non ci sono file excel nella cartella di importazione
                Throw New DocSuiteException("Importazione", String.Format("La Directory {0} non contiene nessun file di Excel.", PathImport), Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If
            ' Esistono dei file excel nella cartella di importazione
        ElseIf files.Length > 1 Then
            Throw New DocSuiteException("Importazione", String.Format("La Directory {0} contiene più file di Excel.", PathImport), Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        Else
            'Setto il nome del file excel
            lblExcel.Text = files(0)
        End If

        ' Tipo documento
        If ProtocolEnv.IsTableDocTypeEnabled Then
            tblDocType.Visible = True
            cmbIdDocType.Visible = True
            rfvDocType.Enabled = True
        Else
            tblDocType.Visible = False
            cmbIdDocType.Visible = False
            rfvDocType.Enabled = False
        End If

        'stato protocollo
        If ProtocolEnv.IsStatusEnabled Then
            tblIdStatus.Visible = True
            cmbProtocolStatus.Visible = True
            cmbProtocolStatus.DataSource = Facade.ProtocolStatusFacade.GetByProtocolStatusExclusion("P")
            cmbProtocolStatus.DataValueField = "Incremental"
            cmbProtocolStatus.DataTextField = "Description"
            cmbProtocolStatus.DataBind()
        Else
            tblIdStatus.Visible = False
            cmbProtocolStatus.Visible = False
        End If

        ' Importazione dati pregresso
        If ProtocolEnv.HasImportPregresso Then
            lblImportPregresso.Text = "IMPORTAZIONE DATI PREGRESSO"
            lblImportPregresso.Visible = True
        Else
            lblImportPregresso.Text = ""
            lblImportPregresso.Visible = False
        End If

        'pulizia temp
        CommonInstance.UserDeleteTemp(TempType.I)
    End Sub
#End Region

#Region "Setup Task"
    Private Sub SetupPageWithTaskRunning()
        Dim url As String = "..\Prot\ProtImportProgress.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&Title=Importazione Lettere&UserStopEnable=1")
        WindowBuilder.LoadWindow("wndProgress", url, "onTaskCompleted", Unit.Pixel(500), Unit.Pixel(250))
    End Sub

    Private Sub SetupPageWithTaskNotRunning()
        EnablePageControls(False)
    End Sub
#End Region

#Region "Enable/Disable Controls"
    Protected Sub EnablePageControls(ByVal IsTaskRunning As Boolean)
        btnDocumenti.Enabled = Not IsTaskRunning
        btnInserimento.Enabled = Not IsTaskRunning
        cmbIdContainer.Enabled = Not IsTaskRunning
        cmbIdDocType.Enabled = Not IsTaskRunning
        cmbProtocolStatus.Enabled = Not IsTaskRunning
        txtNote.Enabled = Not IsTaskRunning
        UscOggetto1.Enabled = Not IsTaskRunning
    End Sub
#End Region

#Region "Verify Import Complete"
    Private Sub VerifyImportComplete()
        If GlobalAsax.LongRunningTask.LastTaskSuccess Then ' (GlobalAsax.LongRunningTask.CurrentStep = GlobalAsax.LongRunningTask.StepsCount - 1) Then
            If Not CommImportLettera.ErrorsTable Is Nothing AndAlso CommImportLettera.ErrorsTable.Rows.Count > 0 Then
                GrResults.DataSource = CommImportLettera.ErrorsTable
                GrResults.DataBind()
                AjaxAlert("Si sono verificati " & CommImportLettera.Errors & " errori durante il processo d\'importazione. Controllare il file di Log.\n\nImportate: " & _
                    CommImportLettera.Imported & " lettere su " & CommImportLettera.Total)
                GrResults.Visible = True
            Else
                AjaxAlert("Importazione eseguita con successo. Importate: " & CommImportLettera.Imported & _
                   " lettere su " & CommImportLettera.Total)
                GrResults.Visible = False
            End If
            If CommImportLettera.Errors + CommImportLettera.Imported = 0 Then
                AjaxAlert("Directory di importazione vuota")
                GrResults.Visible = False
                Exit Sub
            End If
        Else
            FileLogger.Warn(LoggerName, "Si è verificato un errore.", GlobalAsax.LongRunningTask.ExceptionOccured)
            AjaxAlert("Si sono verificati degli errori durante il processo d\'importazione.\nEccezzione sollevata: " & StringHelper.ReplaceAlert(GlobalAsax.LongRunningTask.ExceptionOccured.Message))
        End If
    End Sub
#End Region

#Region "DataGrid Events"
    Protected Sub RadGrid1_NeedDataSource(ByVal source As System.Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles GrResults.NeedDataSource
        GrResults.DataSource = CommImportLettera.ResultTable
    End Sub
#End Region

#Region "Windows Opener Events"
    Private Sub btnDocumenti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocumenti.Click
        If Not ImportFile.ToUpper = "EXCEL" Then
            Dim URL As String = "../Prot/ProtImportList.aspx?" & CommonShared.AppendSecurityCheck("Titolo=Elenco File&ImportType=Lettera&Action=ALL")
            WindowBuilder.LoadWindow("wndResult", URL, "", Unit.Pixel(700), Unit.Pixel(500))
        Else
            Response.Redirect("../Prot/ProtImportM.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&ImportFile=Excel"))
        End If
    End Sub

    Private Sub btnRisultati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRisultati.Click
        Dim URL As String = "../Prot/ProtImportResult.aspx?" & CommonShared.AppendSecurityCheck("Titolo=Risultato Importazione&Action=ALL&ImportFile=" & ImportFile)
        WindowBuilder.LoadWindow("wndResult", URL, "", Unit.Pixel(700), Unit.Pixel(500))
    End Sub
#End Region

#Region "Events"
    Private Sub cmbIdContainer_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbIdContainer.SelectedIndexChanged
        Dim PathImport As String = ProtocolEnv.WordPathImport
        If ImportFile.ToUpper = "EXCEL" Then
            PathImport = ProtocolEnv.ExcelPathImport
        End If
        If Dir(PathImport, FileAttribute.Directory) = "" Then
            AjaxAlert("La directory non Esiste\n\n" & Replace(PathImport, "\", "\\"))
        End If
    End Sub
#End Region


    Private Sub btnInserimento_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInserimento.Click
        Dim tIdContainer As Integer = 0
        Dim tIdLocation As Integer = 0
        Dim tIdDocType As Integer = 0
        Dim tIdType As Integer = 1
        Dim tIdstatus As Integer = -5

        'contenitore
        tIdContainer = cmbIdContainer.SelectedValue
        tIdLocation = Facade.ContainerFacade.GetById(tIdContainer, False, "ProtDB").ProtLocation.Id

        If ProtocolEnv.IsStatusEnabled Then
            tIdstatus = cmbProtocolStatus.SelectedValue
        End If

        If ProtocolEnv.IsTableDocTypeEnabled Then
            tIdDocType = cmbIdDocType.SelectedValue
        End If

        If Not task.Running Then

            Dim oImport As CommImportLettera
            If ImportFile.ToUpper = "EXCEL" Then
                oImport = New CommImportLettera(CommonInstance, "Excel")
            Else
                oImport = New CommImportLettera(CommonInstance)
            End If

            Dim prot As New Protocol()


            With prot
                .Number = -1                'fake
                .Year = -1                  'fake
                If ProtocolEnv.IsStatusEnabled Then
                    .Status = Facade.ProtocolStatusFacade.GetByIncremental(tIdstatus)(0)
                End If
                .IdStatus = tIdstatus
                .Type = Facade.ProtocolTypeFacade.GetById(tIdType)
                .DocumentType = Facade.TableDocTypeFacade.GetById(tIdDocType)
                .Location = Facade.LocationFacade.GetById(tIdLocation)
                .Container = Facade.ContainerFacade.GetById(tIdContainer, False, "ProtDB")

                If uscClassificatore.HasSelectedCategories Then
                    .Category = uscClassificatore.SelectedCategories.First()
                End If

                .Note = txtNote.Text
                .ProtocolObject = UscOggetto1.Text
            End With

            Dim b As Boolean
            If ImportFile.Eq("EXCEL") Then
                b = oImport.InserimentoProtocolloExcel(prot)
            Else
                b = oImport.InserimentoProtocollo(prot, True)
            End If

            If b Then
                Me.SetupPageWithTaskRunning()
            Else
                AjaxAlert("Nessun file da importare.")
            End If

        End If

    End Sub


End Class