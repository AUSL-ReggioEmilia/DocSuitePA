Public Class ProtImportReload
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

	Private _labelMessage As String = ""

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim task As MultiStepLongRunningTask = GlobalAsax.LongRunningTask

		If (task.Running) Then
			Dim completionPerc As Int32 = 0
			Dim color As String = "white"

            ClientScript.RegisterClientScriptBlock(Me.GetType(), "refreshScript", "<script type=""text/javascript"">window.setTimeout('document.location.reload()', 1000);</script>")

			If task.StepsCount > 0 Then
				completionPerc = (task.CurrentStep * 100) \ task.StepsCount
			End If

			If task.CurrentStep > 0 Then
				color = "blue"
			End If

            _labelMessage = "Il processo di importazione è in esecuzione.\n" & "E\' stato avviato il " & _
             task.LastStartTime.ToString() & " da " & task.TaskUser & ".\n" & "Tempo di esecuzione: " & _
             DateTime.Now.Subtract(task.LastStartTime).Hours.ToString("00") & ":" & _
             DateTime.Now.Subtract(task.LastStartTime).Minutes.ToString("00") & ":" & _
             DateTime.Now.Subtract(task.LastStartTime).Seconds.ToString("00") & ".\n" & _
             "Documento " & task.CurrentStep & " di " & task.StepsCount & "."

			If completionPerc > 0 Then
                ClientScript.RegisterStartupScript(Me.GetType(), "changeBar", "<script type=""text/javascript"">parent.document.getElementById('tdBar').style.width = '" _
                 & completionPerc.ToString() & "%'; parent.document.getElementById('tdBar').style.backgroundColor = '" _
                 & color & "';</script>")
            End If
        Else
            If Not task.LastTaskSuccess Then
                _labelMessage = "Task fallito."
                If Not task.ExceptionOccured Is Nothing Then
                    _labelMessage &= "\nLanciata eccezione: " & task.ExceptionOccured.ToString()
                End If
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "changeLabel", "<script type=""text/javascript"">parent.document.getElementById('btnInserimento').disabled = false; parent.document.getElementById('btnGetResults').click();</script>")
        End If

        ClientScript.RegisterStartupScript(Me.GetType(), "changeLabel", "<script type=""text/javascript"">parent.document.getElementById('lblDescription').innerText = '" + _labelMessage + "';</script>")
	End Sub
End Class
