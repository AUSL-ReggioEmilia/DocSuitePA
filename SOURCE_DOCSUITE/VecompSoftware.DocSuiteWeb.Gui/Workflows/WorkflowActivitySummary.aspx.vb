Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data

Public Class WorkflowActivitySummary
    Inherits CommonBasePage

#Region "Fields"
    Private _currentUserProfile As Model.Documents.Signs.UserProfile = Nothing
    Private _currentUserLog As UserLog
#End Region

#Region " Properties "
    Private ReadOnly Property CurrentUserLog As UserLog
        Get
            If _currentUserLog Is Nothing Then
                _currentUserLog = Facade.UserLogFacade.GetByUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
            End If
            Return _currentUserLog
        End Get
    End Property
    Protected ReadOnly Property CurrentUser As String
        Get
            Return JsonConvert.SerializeObject(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property
    Public ReadOnly Property CurrentUserProfile As Model.Documents.Signs.UserProfile
        Get
            If _currentUserProfile Is Nothing Then
                If Not String.IsNullOrEmpty(CurrentUserLog.UserProfile) Then
                    _currentUserProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(CurrentUserLog.UserProfile)
                End If
            End If
            Return _currentUserProfile
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub
#End Region
End Class