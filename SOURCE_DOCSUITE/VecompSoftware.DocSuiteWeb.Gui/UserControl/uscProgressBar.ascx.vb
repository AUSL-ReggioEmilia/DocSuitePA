Imports Telerik.Web.UI

Partial Public Class uscProgressBar
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const AJAX_REQUEST_ARGUMENT_REFRESH As String = "RefreshArgument"
    Private Const SET_PERCENTAGE_FUNCTION As String = "setProgressBarPercentage({0})"
    Private Const START_FUNCTION As String = "startProgress()"
    Private Const STOP_FUNCTION As String = "stopProgress()"
#End Region

#Region " Properties "
    Public Property OperationTitle() As String
        Get
            Return GetPropertyValue(Of String)("TitleText", String.Empty)
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("TitleText", value)
        End Set
    End Property

    Public Property Operation() As String
        Get
            Return GetPropertyValue(Of String)("OperationText", String.Empty)
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("OperationText", value)
        End Set
    End Property

    Public Property OperationDescription() As String
        Get
            Return GetPropertyValue(Of String)("DescriptionText", String.Empty)
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("DescriptionText", value)
        End Set
    End Property

    Public Property OperationTime() As String
        Get
            Return GetPropertyValue(Of String)("TimeText", String.Empty)
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("TimeText", value)
        End Set
    End Property

    Public Property OperationNote() As String
        Get
            Return GetPropertyValue(Of String)("NoteText", String.Empty)
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("NoteText", value)
        End Set
    End Property

    Public Property EnableStopButton() As Boolean
        Get
            Return btnStop.Visible
        End Get
        Set(ByVal value As Boolean)
            btnStop.Visible = value
        End Set
    End Property

    Public Property Total() As Integer
        Get
            Return GetPropertyValue(Of Integer)("Total", 100)
        End Get
        Set(ByVal value As Integer)
            SetPropertyValue(Of Integer)("Total", value)
        End Set
    End Property

    Public Property CurrentValue() As Integer
        Get
            Return GetPropertyValue(Of Integer)("CurrentValue", 0)
        End Get
        Set(ByVal value As Integer)
            SetPropertyValue(Of Integer)("CurrentValue", value)
        End Set
    End Property
#End Region

#Region " Events "
    Public Event Refresh(ByVal sender As Object, ByVal e As EventArgs)
    Public Event StopClick(ByVal sender As Object, ByVal e As EventArgs)

    Private Sub btnStop_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStop.Click
        btnStop.Enabled = False
        btnStop.Text = "Completamento ultima operazione in corso..."
        RaiseEvent StopClick(Me, New EventArgs())
    End Sub

    Private Sub uscProgressBar_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
    End Sub
#End Region

#Region " Methods "
    Public Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, lblTitle)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, lblOperation)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, lblDescription)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, lblNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, lblTime)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnStop, btnStop)
    End Sub

    Protected Sub AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument.Equals(AJAX_REQUEST_ARGUMENT_REFRESH) Then
            RaiseEvent Refresh(Me, New EventArgs())
        End If
    End Sub

    Private Sub SendUpdateProgressBar()
        AjaxManager.ResponseScripts.Add(String.Format(SET_PERCENTAGE_FUNCTION, (CurrentValue / Total) * 100))
    End Sub

    Private Sub SendStartProgressBar()
        AjaxManager.ResponseScripts.Add(START_FUNCTION)
    End Sub

    Private Sub SendStopProgressBar()
        AjaxManager.ResponseScripts.Add(STOP_FUNCTION)
    End Sub

    Public Sub UpdateProgress()
        lblTitle.Text = OperationTitle
        lblOperation.Text = Operation
        lblDescription.Text = OperationDescription
        lblNote.Text = OperationNote
        lblTime.Text = OperationTime

        SendUpdateProgressBar()
    End Sub

    Public Sub StartProgress()
        SendStartProgressBar()
    End Sub

    Public Sub StopProgress()
        SendStopProgressBar()
    End Sub
#End Region

End Class
