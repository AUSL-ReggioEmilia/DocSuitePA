Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltContenitoriGesProprieta
    Inherits SuperAdminPage

#Region " Fields "

    Private _currentContainer As Container

#End Region

#Region " Properties "

    Public ReadOnly Property CurrentContainer() As Container
        Get
            If _currentContainer Is Nothing Then
                Dim temp As Integer = Request.QueryString.GetValueOrDefault("IdContainer", 0)
                If Not temp.Equals(0) Then
                    _currentContainer = Facade.ContainerFacade.GetById(temp, False)
                Else
                    _currentContainer = New Container
                End If
            End If
            Return _currentContainer
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        MasterDocSuite.TitleVisible = False
        uscParam.CurrentContainer = CurrentContainer

        If Not IsPostBack Then
            Page.Title = String.Format("Contenitore [{0}] - Rinomina ", CurrentContainer.Name)
        End If
    End Sub
#End Region


End Class
