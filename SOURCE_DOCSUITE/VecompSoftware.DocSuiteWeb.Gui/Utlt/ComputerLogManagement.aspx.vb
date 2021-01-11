Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ComputerLogManagement
    Inherits CommonBasePage

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdFilterSystemComputer, rgComputerLog, MasterDocSuite.AjaxLoadingPanelSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(rgComputerLog, rgComputerLog, MasterDocSuite.AjaxLoadingPanelSearch)
    End Sub

    Private Sub BindGrid()
        Dim finder As New NHibernateComputerLogFinder
        If Not String.IsNullOrEmpty(txtFilterSystemComputer.Text) Then
            finder.SystemComputer = txtFilterSystemComputer.Text
        End If
        If Not String.IsNullOrEmpty(txtFilterSystemUser.Text) Then
            finder.SystemUser = txtFilterSystemUser.Text
        End If
        rgComputerLog.DataSource = finder.DoSearch
        rgComputerLog.DataBind()
    End Sub

    Private _nullZebraPrinter As ZebraPrinter
    Private ReadOnly Property NullZebraPrinter As ZebraPrinter
        Get
            If _nullZebraPrinter Is Nothing Then
                _nullZebraPrinter = New ZebraPrinter
                _nullZebraPrinter.Id = -1
                _nullZebraPrinter.Description = "Nessuna"
            End If
            Return _nullZebraPrinter
        End Get
    End Property

    Private _availableZebraPrinters As IList(Of ZebraPrinter)
    Private ReadOnly Property AvailableZebraPrinters As IList(Of ZebraPrinter)
        Get
            If _availableZebraPrinters Is Nothing Then
                _availableZebraPrinters = New List(Of ZebraPrinter)
                _availableZebraPrinters = Facade.ZebraPrinterFacade.GetAllOrdered("Description DESC")
                _availableZebraPrinters.Insert(0, NullZebraPrinter)
            End If
            Return _availableZebraPrinters
        End Get
    End Property

    Private _nullScannerConfiguration As ScannerConfiguration
    Private ReadOnly Property NullScannerConfiguration As ScannerConfiguration
        Get
            If _nullScannerConfiguration Is Nothing Then
                _nullScannerConfiguration = New ScannerConfiguration
                _nullScannerConfiguration.Id = -1
                _nullScannerConfiguration.Description = "Nessuna"
            End If
            Return _nullScannerConfiguration
        End Get
    End Property


    Private _availableScannerConfiguration As IList(Of ScannerConfiguration)
    Private ReadOnly Property AvailableScannerConfiguration As IList(Of ScannerConfiguration)
        Get
            If _availableScannerConfiguration Is Nothing Then
                _availableScannerConfiguration = New List(Of ScannerConfiguration)
                _availableScannerConfiguration = Facade.ScannerConfigurationFacade.GetAllOrdered("Description DESC")
                _availableScannerConfiguration.Insert(0, NullScannerConfiguration)
            End If
            Return _availableScannerConfiguration
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            rgComputerLog.MasterTableView.GetColumn("ZebraPrinter").Visible = DocSuiteContext.Current.ProtocolEnv.ZebraPrinterEnabled
            rgComputerLog.MasterTableView.GetColumn("ScannerConfiguration").Visible = DocSuiteContext.Current.ProtocolEnv.ScannerConfigurationEnabled
            BindGrid()
        End If
    End Sub

    Private Sub rgComputerLog_ItemCreated(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rgComputerLog.ItemCreated
        If TypeOf e.Item Is GridDataItem Then
            Dim ddlAdvancedScanner As DropDownList = e.Item.FindControl("ddlAdvancedScanner")
            Dim ddlAdvancedViewer As DropDownList = e.Item.FindControl("ddlAdvancedViewer")

            ddlAdvancedScanner.AutoPostBack = True
            AddHandler ddlAdvancedScanner.SelectedIndexChanged, AddressOf ddlAdvancedScanner_SelectedIndexChanged

            ddlAdvancedViewer.AutoPostBack = True
            AddHandler ddlAdvancedViewer.SelectedIndexChanged, AddressOf ddlAdvancedViewer_SelectedIndexChanged

            If DocSuiteContext.Current.ProtocolEnv.ZebraPrinterEnabled Then
                Dim ddlIdZebraPrinter As DropDownList = e.Item.FindControl("ddlIdZebraPrinter")

                ddlIdZebraPrinter.DataSource = AvailableZebraPrinters
                ddlIdZebraPrinter.DataTextField = "Description"
                ddlIdZebraPrinter.DataValueField = "Id"
                ddlIdZebraPrinter.AutoPostBack = True
                AddHandler ddlIdZebraPrinter.SelectedIndexChanged, AddressOf ddlIdZebraPrinter_SelectedIndexChanged
            End If

            If DocSuiteContext.Current.ProtocolEnv.ScannerConfigurationEnabled Then
                Dim ddlIdScannerConfiguration As DropDownList = e.Item.FindControl("ddlIdScannerConfiguration")

                ddlIdScannerConfiguration.DataSource = AvailableScannerConfiguration
                ddlIdScannerConfiguration.DataTextField = "Description"
                ddlIdScannerConfiguration.DataValueField = "Id"
                ddlIdScannerConfiguration.AutoPostBack = True
                AddHandler ddlIdScannerConfiguration.SelectedIndexChanged, AddressOf ddlIdScannerConfiguration_SelectedIndexChanged
            End If
        End If
    End Sub

    Private Sub rgComputerLog_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles rgComputerLog.ItemDataBound
        If TypeOf e.Item Is GridDataItem Then
            Dim ddlAdvancedScanner As DropDownList = e.Item.FindControl("ddlAdvancedScanner")
            Dim ddlAdvancedViewer As DropDownList = e.Item.FindControl("ddlAdvancedViewer")

            Dim currentItem As ComputerLog = e.Item.DataItem
            ddlAdvancedScanner.SelectedValue = currentItem.AdvancedScanner
            ddlAdvancedViewer.SelectedValue = currentItem.AdvancedViewer

            If DocSuiteContext.Current.ProtocolEnv.ZebraPrinterEnabled Then
                Dim ddlIdZebraPrinter As DropDownList = e.Item.FindControl("ddlIdZebraPrinter")

                If Not currentItem.ZebraPrinter Is Nothing AndAlso Not String.IsNullOrEmpty(currentItem.ZebraPrinter.Id) Then
                    ddlIdZebraPrinter.SelectedValue = currentItem.ZebraPrinter.Id
                Else : ddlIdZebraPrinter.SelectedValue = -1
                End If
            End If

            If DocSuiteContext.Current.ProtocolEnv.ScannerConfigurationEnabled Then
                Dim ddlIdScannerConfiguration As DropDownList = e.Item.FindControl("ddlIdScannerConfiguration")

                If Not currentItem.ScannerConfiguration Is Nothing AndAlso Not String.IsNullOrEmpty(currentItem.ScannerConfiguration.Id) Then
                    ddlIdScannerConfiguration.SelectedValue = currentItem.ScannerConfiguration.Id
                Else : ddlIdScannerConfiguration.SelectedValue = -1
                End If
            End If
        End If
    End Sub

    Private Sub ddlAdvancedScanner_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddlSender As DropDownList = sender
        Dim currentId As String = ddlSender.Attributes("CommandArgument")
        Dim cl As ComputerLog = Facade.ComputerLogFacade.GetById(currentId)
        cl.AdvancedScanner = ddlSender.SelectedValue
        Facade.ComputerLogFacade.Update(cl)
    End Sub

    Private Sub ddlAdvancedViewer_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddlSender As DropDownList = sender
        Dim currentId As String = ddlSender.Attributes("CommandArgument")
        Dim cl As ComputerLog = Facade.ComputerLogFacade.GetById(currentId)
        If currentId.Equals(CommonUtil.DSUserComputer) Then
            CommonShared.AdvancedViewer = ddlSender.SelectedValue
        End If
        cl.AdvancedViewer = ddlSender.SelectedValue
        Facade.ComputerLogFacade.Update(cl)
    End Sub

    Private Sub ddlIdZebraPrinter_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddlSender As DropDownList = sender
        Dim currentId As String = ddlSender.Attributes("CommandArgument")
        Dim cl As ComputerLog = Facade.ComputerLogFacade.GetById(currentId)
        cl.ZebraPrinter = Facade.ZebraPrinterFacade.GetById(ddlSender.SelectedValue)
        Facade.ComputerLogFacade.Update(cl)
    End Sub

    Private Sub ddlIdScannerConfiguration_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddlSender As DropDownList = sender
        Dim currentId As String = ddlSender.Attributes("CommandArgument")
        Dim cl As ComputerLog = Facade.ComputerLogFacade.GetById(currentId)
        cl.ScannerConfiguration = Facade.ScannerConfigurationFacade.GetById(ddlSender.SelectedValue)
        Facade.ComputerLogFacade.Update(cl)
    End Sub

    Private Sub cmdFilterSystemComputer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdFilterSystemComputer.Click
        BindGrid()
    End Sub

    Private Sub rgComputerLog_PageIndexChanged(ByVal source As Object, ByVal e As Telerik.Web.UI.GridPageChangedEventArgs) Handles rgComputerLog.PageIndexChanged
        BindGrid()
        rgComputerLog.CurrentPageIndex = e.NewPageIndex
    End Sub

End Class