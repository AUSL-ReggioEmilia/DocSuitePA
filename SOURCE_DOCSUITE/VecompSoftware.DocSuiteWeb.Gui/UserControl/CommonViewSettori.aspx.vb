Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class CommonViewSettori
    Inherits CommBasePage

#Region " Fields "

    Private _currentProtocol As Protocol
#End Region

#Region " Properties "

    Private ReadOnly Property CurrentProtocol As Protocol
        Get
            If _currentProtocol Is Nothing AndAlso UniqueIdProtocol.HasValue Then
                _currentProtocol = Facade.ProtocolFacade.GetByUniqueId(UniqueIdProtocol.Value)
            End If
            Return _currentProtocol
        End Get
    End Property

    Private ReadOnly Property UniqueIdProtocol As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("UniqueId", Nothing)
        End Get
    End Property


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        If Not Page.IsPostBack Then
            Initialize()
        End If

    End Sub


#End Region

#Region " Methods "

    Private Sub BindRoles()

    End Sub


    Private Sub Initialize()
        If CurrentProtocol.Roles.Any(Function(r) r.Status = ProtocolRoleStatus.ToEvaluate) Then
            uscSettori.Visible = True
            uscSettori.Caption = "Settori con Autorizzazione non accettata"
            uscSettori.CurrentProtocol = CurrentProtocol
            uscSettori.DataBindProtocolRoles(CurrentProtocol.Roles.Where(Function(r) r.Status = ProtocolRoleStatus.ToEvaluate).ToList(), True)
        Else
            uscSettori.Visible = False
        End If
    End Sub



#End Region

End Class