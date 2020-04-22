Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscProtocolObjectFinder
    Inherits DocSuite2008BaseControl

    Public Delegate Sub DoSearchEventHandler(ByVal sender As Object, ByVal e As ProtocolObjectFinderEventArgs)

    Public Event DoSearch As DoSearchEventHandler

#Region " Fields "

    Private _finder As NHibernateProtocolObjectFinder

#End Region

#Region " Properties "

    Public ReadOnly Property Finder() As NHibernateProtocolObjectFinder
        Get
            BindData()
            Return _finder
        End Get
    End Property

    Public ReadOnly Property SearchButtonControl() As Button
        Get
            Return btnSearch
        End Get
    End Property

    Public Property HideDatePanel() As Boolean
        Get
            Return GetPropertyValue(Of Boolean)("_hideDatePanel", False)
        End Get
        Set(ByVal value As Boolean)
            SetPropertyValue(Of Boolean)("_hideDatePanel", value)
        End Set
    End Property

    Public Property HideNumberPanel() As Boolean
        Get
            Return GetPropertyValue(Of Boolean)("_hideNumberPanel", False)
        End Get
        Set(ByVal value As Boolean)
            SetPropertyValue(Of Boolean)("_hideNumberPanel", value)
        End Set
    End Property

    Public Property HideContainerPanel() As Boolean
        Get
            Return GetPropertyValue(Of Boolean)("_hideContainerPanel", False)
        End Get
        Set(ByVal value As Boolean)
            SetPropertyValue(Of Boolean)("_hideContainerPanel", value)
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Initialize()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        RaiseEvent DoSearch(Me, New ProtocolObjectFinderEventArgs(Finder))
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        lblDateFrom.Visible = (Not HideDatePanel)
        rdpRegDate_From.Visible = (Not HideDatePanel)
        cvRegDate_From.Visible = (Not HideDatePanel)
        lblDateTo.Visible = (Not HideDatePanel)
        rdpRegDate_To.Visible = (Not HideDatePanel)
        cvRegDate_To.Visible = (Not HideDatePanel)

        lblNumberFrom.Visible = (Not HideNumberPanel)
        txtNumber_From.Visible = (Not HideNumberPanel)
        cvNumber_From.Visible = (Not HideNumberPanel)
        lblNumberTo.Visible = (Not HideNumberPanel)
        txtNumber_To.Visible = (Not HideNumberPanel)
        cvNumber_To.Visible = (Not HideNumberPanel)

        lblContainer.Visible = (Not HideContainerPanel)
        ddlContainer.Visible = (Not HideContainerPanel)
        If (Not HideContainerPanel) Then
            BindContainers()
        End If
    End Sub

    ''' <summary> bind contenitori con diritti </summary>
    Private Sub BindContainers()
        Dim containers As IList(Of ContainerRightsDto) = Facade.ContainerFacade.GetAllRights("Prot", Nothing)
        For Each container As ContainerRightsDto In containers
            ddlContainer.Items.Add(New ListItem(container.Name, container.ContainerId.ToString()))
        Next
    End Sub

    Private Sub BindData()
        _finder = New NHibernateProtocolObjectFinder()
        _finder.EnableFetchMode = False
        _finder.EnableTableJoin = False
        _finder.RegistrationDateFrom = rdpRegDate_From.SelectedDate
        _finder.RegistrationDateTo = rdpRegDate_To.SelectedDate
        _finder.NumberFrom = txtNumber_From.Text
        _finder.NumberTo = txtNumber_To.Text
        _finder.IdContainer = ddlContainer.SelectedValue

        Dim changeObjectParam As ChangeObjectParameter = DocSuiteContext.Current.ProtocolEnv.EnvChangeObject
        _finder.Year = changeObjectParam.Year
        _finder.ProtocolObject = changeObjectParam.Object
        _finder.PageSize = changeObjectParam.MaxRecords

        '_finder.SelectProjections.Add("DocumentCode", "DocumentCode")
        '_finder.SelectProjections.Add("Year", "Year")
        '_finder.SelectProjections.Add("Number", "Number")
        '_finder.SelectProjections.Add("RegistrationDate", "RegistrationDate")
    End Sub

#End Region

End Class
