''' <summary> Utility di protocollo vecchie. </summary>
''' <remarks>
''' TODO: Spostare tutto nelle facade appropriate
''' </remarks>
Public Class ProtocolUtil

#Region "Fields"
    Private Shared _instance As ProtocolUtil
    Private _envGroupSelected As String = Nothing
    Private _envGroupManagerSelected As String = Nothing
#End Region

#Region "Singleton"
    Public Shared Function GetInstance() As ProtocolUtil
        If _instance Is Nothing Then
            _instance = New ProtocolUtil
        End If
        Return (_instance)
    End Function

#End Region

#Region "Properties"

    Public Property EnvGroupSelected() As String
        Get
            Return _envGroupSelected
        End Get
        Set(ByVal value As String)
            _envGroupSelected = value
            CommonUtil.GroupProtocolNotManagerSelected = value
        End Set
    End Property

    Public Property EnvGroupManagerSelected() As String
        Get
            Return _envGroupManagerSelected
        End Get
        Set(ByVal value As String)
            _envGroupManagerSelected = value
            CommonUtil.GroupProtocolManagerSelected = value
        End Set
    End Property

#End Region
End Class
