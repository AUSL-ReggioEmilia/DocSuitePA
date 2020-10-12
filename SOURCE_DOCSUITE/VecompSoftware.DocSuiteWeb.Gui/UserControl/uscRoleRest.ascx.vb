Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data

Public Class uscRoleRest
    Inherits DocSuite2008BaseControl

#Region " Properties "
    Public Property ReadOnlyMode As Boolean
    Public Property Expanded As Boolean
    Public Property MultipleRoles As Boolean
    Public Property OnlyMyRoles As Boolean
    Public Property Required() As Boolean
        Get
            Return AnyNodeCheck.Enabled
        End Get
        Set(ByVal value As Boolean)
            AnyNodeCheck.Enabled = value
        End Set
    End Property
    Public WriteOnly Property Collapsable() As Boolean
        Set(value As Boolean)
            btnExpandRoles.Visible = value
        End Set
    End Property

    Public Property RequiredMessage() As String
        Get
            Return AnyNodeCheck.ErrorMessage
        End Get
        Set(ByVal value As String)
            AnyNodeCheck.ErrorMessage = value
        End Set
    End Property
    Public ReadOnly Property TableContentControl() As HtmlTable
        Get
            Return tblRoles
        End Get
    End Property
    Public Property Caption() As String
        Get
            Return lblCaption.Text
        End Get
        Set(ByVal value As String)
            lblCaption.Text = value
        End Set
    End Property
    Protected ReadOnly Property ControlConfiguration As String
        Get
            Return JsonConvert.SerializeObject(New With {.isReadOnlyMode = ReadOnlyMode})
        End Get
    End Property

    Public Property AllDataButtonEnabled As Boolean

    Public Property RemoveAllDataButtonEnabled As Boolean

    Public Property LoadAllRoles As Boolean

    Public Property RACIButtonEnabled As Boolean = False

    Public Property FascicleVisibilityTypeButtonEnabled As Boolean = False
    Public Property DSWEnvironmentType As DSWEnvironment

#End Region

#Region " Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblCurrentTenant.Text = DocSuiteContext.Current.CurrentTenant.TenantName
            If Not ReadOnlyMode Then
                actionToolbar.Visible = True
            End If
        End If
    End Sub
#End Region

#Region " Methods "

#End Region

End Class