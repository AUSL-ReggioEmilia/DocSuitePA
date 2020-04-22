Imports System.Collections.Generic
Imports System.IO
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Public Class ProtImport
    Inherits ProtBasePage

#Region " Fields "

    Private _task As MultiStepLongRunningTask
    Private _pathImport As String
    Private _prot As Protocol

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        'verifica InvoicePathImport
        _pathImport = ProtocolEnv.InvoicePathImport

        '#If DEBUG Then
        '        _pathImport = "C:\temp\FastInput"
        '#End If

        InitializeButtons()
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
        InitializeTask()
    End Sub

    Private Sub AjaxRequest_TaskCompleted(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        VerifyImportComplete()
        btnRisultati.Enabled = True
    End Sub

    Protected Sub GrResults_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles GrResults.NeedDataSource
        GrResults.DataSource = CommImport.ErrorsTable ' CommImport.ResultTable
    End Sub

    Private Sub cmbIdContainer_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbIdContainer.SelectedIndexChanged

        If cmbIdContainer.SelectedItem.Text = "" Then
            btnDocumenti.Enabled = False
        Else
            _pathImport = _pathImport & "\" & cmbIdContainer.SelectedItem.Text & "\In"
            If Not (Directory.Exists(_pathImport)) Then
                btnDocumenti.Enabled = False
                btnInserimento.Enabled = False
                btnInserimentoParziale.Enabled = False
                AjaxAlert("La directory non Esiste\n\n" & Replace(_pathImport, "\", "\\"))
            Else
                btnDocumenti.Enabled = True
                btnInserimento.Enabled = True
                btnInserimentoParziale.Enabled = True
            End If
        End If
    End Sub

    Private Sub btnDocumenti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocumenti.Click
        Dim parameters As String = "Titolo=Elenco File&ImportType=Fattura&Action=ALL&Container=" & cmbIdContainer.SelectedItem.Text
        Dim url As String = "../Prot/ProtImportList.aspx?" & CommonShared.AppendSecurityCheck(parameters)
        WindowBuilder.LoadWindow("wndResult", url, "", Unit.Pixel(700), Unit.Pixel(500))
    End Sub

    Private Sub btnInserimento_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimento.Click
        ImportClick(True)
    End Sub

    Protected Sub btnInserimentoParziale_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimentoParziale.Click
        ImportClick(False)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, GrResults)
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest_TaskCompleted

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmbIdContainer)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmbIdContainer, btnInserimento)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmbIdContainer, btnInserimentoParziale)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmbIdContainer, btnDocumenti)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnInserimento)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnInserimentoParziale)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnRisultati)
    End Sub

    Private Sub InitializeButtons()
        WindowBuilder.RegisterWindowManager(RadWindowManagerImport)
        WindowBuilder.RegisterOpenerElement(btnDocumenti)
        WindowBuilder.RegisterOpenerElement(btnInserimentoParziale)
        WindowBuilder.RegisterOpenerElement(btnInserimento)
        WindowBuilder.RegisterOpenerElement(btnRisultati)
    End Sub

    Private Sub InitializeTask()
        _task = GlobalAsax.LongRunningTask

        If (_task.Running) Then
            SetupPageWithTaskRunning()
        End If
    End Sub

    Private Sub Initialize()

        cmbIdDocType.DataValueField = "Id"
        cmbIdDocType.DataTextField = "Description"

        Dim importer As IProtocolImporter
        If Not String.IsNullOrEmpty(ProtocolEnv.ProtocolImportClass) Then
            importer = ProtocolImportFactory.GetProtocolImporter(ProtocolEnv.ProtocolImportClass)
            UscClassificatore1.Visible = Not importer.ClassificationManaged
        End If

        tblDocType.Visible = ProtocolEnv.IsTableDocTypeEnabled

        btnInserimentoParziale.Visible = ProtocolEnv.ProtocolImportLimit > 0

        WebUtils.ObjAttDisplayNone(btnRisultati)

        Title = String.Format("Protocollo - Importazione ({0})", _pathImport)

        If _pathImport = "" Then
            Throw New DocSuiteException("Importazione Fatture", "In ParamerEnv Manca la defizione della chiave InvoicePathImport")
        End If

        If Dir(_pathImport, FileAttribute.Directory) = "" Then
            Throw New DocSuiteException("Importazione Fatture", String.Format("La Directory [{0}] non è Valida. Verificare in ParamerEnv la defizione della chiave InvoicePathImport", _pathImport))
        End If

        ' Verifica diritti utente
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True)
        If containers.Count > 0 Then
            cmbIdContainer.DataSource = containers
            cmbIdContainer.DataValueField = "Id"
            cmbIdContainer.DataTextField = "Name"
            cmbIdContainer.DataBind()
        Else
            Throw New DocSuiteException("Protocollo Inserimento", "Utente senza diritti di Inserimento")
        End If

        Dim idContainer As String = CType(Session("IDContainer"), String)
        If idContainer <> "" Then
            If Not (cmbIdContainer.Items.FindByValue(idContainer) Is Nothing) Then
                cmbIdContainer.Items.FindByValue(idContainer).Selected = True
            End If
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

    Private Sub SetupPageWithTaskRunning()
        Dim url As String = "..\Prot\ProtImportProgress.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&Title=Importazione Fatture&UserStopEnable=1")
        WindowBuilder.LoadWindow("wndProgress", url, "onTaskCompleted", Unit.Pixel(500), Unit.Pixel(250))
    End Sub

    Protected Sub EnablePageControls(ByVal isTaskRunning As Boolean)
        btnDocumenti.Enabled = Not isTaskRunning
        cmbIdContainer.Enabled = Not isTaskRunning
        cmbIdDocType.Enabled = Not isTaskRunning
        txtNote.Enabled = Not isTaskRunning
        UscClassificatore1.ReadOnly = isTaskRunning
    End Sub

    Private Sub VerifyImportComplete()
        cmbIdContainer.Enabled = True
        cmbIdDocType.Enabled = True

        If cmbIdContainer.SelectedItem.Text <> "" Then
            btnInserimento.Enabled = True
            btnDocumenti.Enabled = True
            btnInserimentoParziale.Enabled = True
        End If

        If GlobalAsax.LongRunningTask.LastTaskSuccess Then
            If Not CommImport.ErrorsTable Is Nothing AndAlso CommImport.ErrorsTable.Rows.Count > 0 Then
                GrResults.DataSource = CommImport.ErrorsTable
                GrResults.DataBind()
                AjaxAlert("Si sono verificati degli errori durante il processo d''importazione.{0}Consultare il log d''importazione per verifiche.{0}Documenti Validi: {1}.{0}Documenti Scartati: {2}.", Environment.NewLine, CommImport.Imported, CommImport.Errors)
                GrResults.Visible = True
            Else
                AjaxAlert("Importazione eseguita con successo.{0}Importati Documenti {1}.", Environment.NewLine, CommImport.Imported)
                GrResults.Visible = False
            End If

            If CommImport.Errors + CommImport.Imported = 0 Then
                AjaxAlert("Directory di importazione senza documenti da importare.")
                GrResults.Visible = False
                Exit Sub
            End If
        Else
            FileLogger.Warn(LoggerName, GlobalAsax.LongRunningTask.ExceptionOccured.Message, GlobalAsax.LongRunningTask.ExceptionOccured)
            AjaxAlert("Si sono verificati degli errori durante il processo d''importazione.\nEccezione sollevata: " & StringHelper.ReplaceAlert(GlobalAsax.LongRunningTask.ExceptionOccured.Message))
        End If
    End Sub

    Private Sub ImportClick(ByVal all As Boolean)

        Dim tIdContainer As Integer
        Dim tIdLocation As Integer
        Dim tIdAttachLocation As Integer = 0
        Dim tIdDocType As Integer = 0
        Dim tIdType As Integer = 1
        Dim tIdstatus As Integer = -5

        'Contenitore
        tIdContainer = cmbIdContainer.SelectedValue
        tIdLocation = Facade.ContainerFacade.GetById(tIdContainer, False, "ProtDB").ProtLocation.Id
        If ProtocolEnv.IsProtocolAttachLocationEnabled Then
            tIdAttachLocation = Facade.ContainerFacade.GetById(tIdContainer, False, "ProtDB").ProtAttachLocation.Id
        End If

        'Classificatore
        'tIdCategory = UscClassificatore1.CategoryID

        'Tipo Documento
        If ProtocolEnv.IsTableDocTypeEnabled Then
            tIdDocType = cmbIdDocType.SelectedValue
        End If

        If Not _task.Running Then
            Dim import As New CommImport(cmbIdContainer.SelectedItem.Text, "Fattura")

            _prot = New Protocol()

            With _prot
                .Number = -1                'fake
                .Year = -1                  'fake
                If ProtocolEnv.IsStatusEnabled Then
                    .Status = Facade.ProtocolStatusFacade.GetById(tIdstatus)
                End If
                .Type = Facade.ProtocolTypeFacade.GetById(tIdType)
                .DocumentType = Facade.TableDocTypeFacade.GetById(tIdDocType)
                .Location = Facade.LocationFacade.GetById(tIdLocation)
                If ProtocolEnv.IsProtocolAttachLocationEnabled Then
                    .AttachLocation = Facade.LocationFacade.GetById(tIdAttachLocation)
                End If
                .Container = Facade.ContainerFacade.GetById(tIdContainer, False, "ProtDB")
                If UscClassificatore1.Visible Then
                    .Category = UscClassificatore1.SelectedCategories(UscClassificatore1.CategoryID)
                End If
                .Note = txtNote.Text
            End With

            If import.InserimentoProtocollo(_prot, all) Then
                SetupPageWithTaskRunning()
            Else
                AjaxAlert("Nessun file da importare.")
            End If
        End If
    End Sub

#End Region

End Class
