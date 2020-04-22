Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class ProtImportProgress
    Inherits ProtBasePage

#Region " Fields "
    Private task As MultiStepLongRunningTask = Nothing
    Private Const EXPORT_TASK_TYPE As String = "E"
    Private ReadOnly _operationNote As String = String.Format("Processo avviato il {0:dd/MM/yyyy HH:mm:ss} da {1}", DateTime.Now, DocSuiteContext.Current.User.FullUserName)
    Private Const CLOSE_WINDOW_FUNCTION As String = "CloseWindow('{0}');"
#End Region

#Region " Properties "
    Private ReadOnly Property TaskTitle As String
        Get
            Return GetKeyValue(Of String)("Title")
        End Get
    End Property

    Private ReadOnly Property IsExportTask As Boolean
        Get
            Return GetValueOrDefault(Of String)("TaskType", String.Empty).Eq(EXPORT_TASK_TYPE)
        End Get
    End Property

    Private ReadOnly Property UserStopEnable As Boolean
        Get
            Return GetValueOrDefault(Of Boolean)("UserStopEnable", False)
        End Get
    End Property
#End Region

#Region " Events "
    Private Sub uscProgressBarImpot_Refresh(ByVal sender As Object, ByVal e As EventArgs) Handles uscProgressBarImpot.Refresh
        SetupPageWithTaskRunning()
    End Sub

    Private Sub uscProgressBarImpot_StopClick(ByVal sender As Object, ByVal e As EventArgs) Handles uscProgressBarImpot.StopClick
        task = GlobalAsax.LongRunningTask
        If task.Running Then
            task.RequestStop()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Public Sub Initialize()
        Me.Title = TaskTitle
        MasterDocSuite.TitleVisible = False
        uscProgressBarImpot.OperationNote = _operationNote
        uscProgressBarImpot.OperationTitle = String.Concat("Il processo di ", If(IsExportTask, "esportazione", "importazione"), " è in esecuzione")
        uscProgressBarImpot.EnableStopButton = UserStopEnable
        SetupPageWithTaskRunning()
        uscProgressBarImpot.StartProgress()
    End Sub

    Private Sub SetupPageWithTaskRunning()
        task = GlobalAsax.LongRunningTask
        If Not task.Running Then
            CloseWindow()
        End If

        If (GlobalAsax.LongRunningTask.StepsCount <> 0) Then
            Dim tSpan2 As TimeSpan = DateTime.Now.Subtract(GlobalAsax.LongRunningTask.LastStartTime)

            With uscProgressBarImpot
                .Total = task.StepsCount
                .CurrentValue = GlobalAsax.LongRunningTask.CurrentStep
                .Operation = If(String.IsNullOrEmpty(GlobalAsax.LongRunningTask.CurrentFileName), String.Empty, String.Concat("Documento da importare: ", GlobalAsax.LongRunningTask.CurrentFileName))
                .OperationTime = String.Format("Tempo trascorso {0:mm\:ss} secondi", tSpan2)
                .OperationDescription = String.Format("{0}: {1} di {2}", If(IsExportTask, "Esportazione", "Importazione"), .CurrentValue, .Total)
                .UpdateProgress()
            End With

            VerifyImportComplete()
        End If
    End Sub

    Private Sub VerifyImportComplete()
        If (GlobalAsax.LongRunningTask.StepsCount <> 0 AndAlso GlobalAsax.LongRunningTask.CurrentStep.Equals(GlobalAsax.LongRunningTask.StepsCount)) Then
            CloseWindow()
        End If
    End Sub

    Private Sub CloseWindow()
        Dim builder As StringBuilder = New StringBuilder()
        builder.Append(GlobalAsax.LongRunningTask.LastTaskSuccess)
        builder.Append(String.Concat("|", CommExport.Errors))
        builder.Append(String.Concat("|", CommExport.Exported))
        builder.Append(String.Concat("|", GlobalAsax.LongRunningTask.LastStartTime))
        AjaxManager.ResponseScripts.Add(String.Format(CLOSE_WINDOW_FUNCTION, builder.ToString()))
        uscProgressBarImpot.StopProgress()
    End Sub
#End Region
End Class