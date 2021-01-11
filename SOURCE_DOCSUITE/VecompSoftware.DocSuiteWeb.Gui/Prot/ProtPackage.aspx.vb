Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtPackage
    Inherits ProtBasePage

#Region " Fields "

    Dim _packages As IList(Of Package)

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not DocSuiteContext.Current.ProtocolEnv.IsPackageEnabled Then
            Throw New DocSuiteException("Impossibile aprire pagina", "Funzionalità non abilitata")
        End If

        _packages = New List(Of Package)

        InitializeAjax()
        If Not IsPostBack Then
            FillDatagrid()
        End If

    End Sub

    Protected Sub gvPackages_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument.Eq("Refresh") Then
            FillDatagrid()
        End If
    End Sub

    Private Sub gvPackages_Init(ByVal sender As Object, ByVal e As EventArgs) Handles gvPackages.Init
        For Each column As GridColumn In gvPackages.Columns
            Select Case column.UniqueName
                Case "Origin"
                    DirectCast(column, SuggestFilteringColumn).DataSourceCombo = Facade.PackageFacade.GetOrigin()
                Case "State"
                    Dim stateList As New ArrayList()
                    stateList.Add("A")
                    stateList.Add("C")
                    stateList.Add("D")
                    DirectCast(column, SuggestFilteringColumn).DataSourceCombo = stateList
            End Select
        Next
    End Sub

    Private Sub gvPackages_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles gvPackages.ItemDataBound
        If e.Item.ItemType <> GridItemType.AlternatingItem AndAlso e.Item.ItemType <> GridItemType.Item Then
            Exit Sub
        End If

        Dim package As Package = DirectCast(e.Item.DataItem, Package)

        With DirectCast(e.Item.FindControl("btnDelete"), RadButton)
            If package.State <> "D"c Then
                .Visible = False
            Else
                .Image.ImageUrl = ImagePath.SmallCancel
            End If
        End With

        With DirectCast(e.Item.FindControl("btnEdit"), RadButton)
            If package.State = "C"c Then
                .Visible = False
            Else
                .Image.ImageUrl = ImagePath.SmallEdit
                .CommandArgument = String.Format("&OR={0}&PK={1}", package.Origin, package.Package)
            End If

        End With
    End Sub

    ''' <summary> Elimina un package </summary>
    Private Sub gvPackagesItemCommand(ByVal [source] As Object, ByVal e As GridCommandEventArgs) Handles gvPackages.ItemCommand
        If e.CommandName.Eq("DeleteItem") Then
            Dim gridItem As GridDataItem = gvPackages.Items(e.Item.ItemIndex)
            ' Numero package
            Dim idOrigin As Char = DirectCast(gridItem.GetDataKeyValue("Origin"), Char)
            Dim idPackage As Integer = DirectCast(gridItem.GetDataKeyValue("Package"), Integer)

            Facade.PackageFacade.Delete(Facade.PackageFacade.GetById(idOrigin, idPackage, False))

            FillDatagrid()
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf gvPackages_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, gvPackages, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    ''' <summary> Popola la datagrid dei packages </summary>
    Private Sub FillDatagrid()
        ' Package abilitati
        Dim contList As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True)
        If CommonShared.HasGroupAdministratorRight Then
            ' utente amministratore
            _packages = Facade.PackageFacade.GetByAccount("", "")
        ElseIf Not contList Is Nothing And contList.Count > 0 Then
            ' Utente connesso
            _packages = Facade.PackageFacade.GetByAccount(DocSuiteContext.Current.User.UserName, String.Empty)
            gvPackages.Columns.FindByUniqueName("Origin").Visible = False
        End If
        ' Popola datagrid
        Dim finder As NHibernateDomainObjectFinder(Of Package) = Facade.PackageFacade.GetNHibernateDomainObjectFinder("ProtDB")
        finder.PageSize = 2000

        gvPackages.Finder = finder

        gvPackages.DataSource = _packages
        gvPackages.DataBind()
    End Sub

#End Region

End Class
