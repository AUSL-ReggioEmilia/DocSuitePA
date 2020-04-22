Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class MessageViewLog
    Inherits CommBasePage

#Region " Fields "

    Private _messageEmail As MessageEmail

#End Region

#Region " Properties "

    Public ReadOnly Property MessageEmail As MessageEmail
        Get
            If _messageEmail Is Nothing Then
                _messageEmail = Facade.MessageEmailFacade.GetById(GetKeyValue(Of Integer)("MessageEmailId"))
            End If
            Return _messageEmail
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            lblMessage.Text = MessageEmail.Subject

            Dim finder As New MessageLogFinder
            finder.MessageIn = {MessageEmail.Message}
            finder.SortExpressions.Add("LogDate", "ASC")

            dgLog.Finder = finder
            dgLog.CurrentPageIndex = 0
            dgLog.CustomPageIndex = 0
            dgLog.PageSize = dgLog.Finder.PageSize
            dgLog.DataBindFinder()
        End If

    End Sub

#End Region

#Region " Methods "


    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, CommBasePage)(key)
    End Function

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgLog, dgLog, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

#End Region

End Class