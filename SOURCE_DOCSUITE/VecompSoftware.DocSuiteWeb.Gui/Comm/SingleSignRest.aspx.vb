Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.Documents
Imports VecompSoftware.DocSuiteWeb.Model.Documents.Signs

Public Class SingleSignRest
    Inherits CommonBasePage

#Region " Fields "
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

    Public ReadOnly Property CurrentUserDomain As String
        Get
            Return DocSuiteContext.Current.User.FullUserName.Replace("\", "\\")
        End Get
    End Property

    Public ReadOnly Property SignalRAddress As String
        Get
            Return DocSuiteContext.SignalRServerAddress
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

    Public ReadOnly Property TypeOfSign As String
        Get
            Dim defaultSignOption As String = CurrentUserProfile.DefaultProvider.ToString()
            Return defaultSignOption.ToString()
        End Get
    End Property

    Public ReadOnly Property StorageInformationType As String
        Get
            Dim defaultSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = Nothing
            If CurrentUserProfile.Value.Any(Function(x) x.Value.IsDefault = True) Then
                defaultSignOption = CurrentUserProfile.Value.FirstOrDefault(Function(x) x.Value.IsDefault = True)
            Else
                defaultSignOption = CurrentUserProfile.Value.FirstOrDefault(Function(x) x.Key = CurrentUserProfile.DefaultProvider)
            End If

            Return defaultSignOption.Value.Value.StorageInformationType.ToString()
        End Get
    End Property

#End Region
#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub
#End Region

End Class