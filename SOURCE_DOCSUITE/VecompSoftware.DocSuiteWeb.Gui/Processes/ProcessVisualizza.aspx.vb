Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class ProcessVisualizza
    Inherits CommonBasePage
#Region "Properties"
    Protected ReadOnly Property ProcessId As String
        Get
            Dim _processId As String = Request.QueryString.GetValueOrDefault("IdProcess", String.Empty)
            Return _processId
        End Get
    End Property
    Protected ReadOnly Property ReadOnlyRoles As Boolean
        Get
            Dim _readonlyRoles As Boolean = Request.QueryString.GetValueOrDefault("ReadOnlyRoles", False)
            Return _readonlyRoles
        End Get
    End Property
#End Region

#Region "Methods"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscProcessDetails.ReadOnlyRoles = ReadOnlyRoles
    End Sub
#End Region
End Class