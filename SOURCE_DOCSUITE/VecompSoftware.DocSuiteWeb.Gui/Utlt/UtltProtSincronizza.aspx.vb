Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

Partial Public Class UtltProtSincronizza
    Inherits UtltBasePage


#Region " Constants "

    Private Const ProtocolImportedSessionName As String = "TopMediaProtocolImported"

#End Region

#Region " Fields "

    Private _finder As NHibernateProtocolHeaderFinderTD
    Private _protocols As IList(Of Protocol)

    Private _task As MultiStepLongRunningTask

#End Region

#Region " Properties "

    Private ReadOnly Property Finder As NHibernateProtocolHeaderFinderTD
        Get
            If _finder Is Nothing Then
                _finder = Facade.ProtocolHeaderFinderTD()
                _finder.YearFrom = 2008
                _finder.ProtocolStatus = 0
                _finder.IdContainerIn = DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.ContainerIdentifiers
                _finder.IdRoleIn = DocSuiteContext.Current.ProtocolEnv.TopMediaParameters.RoleIdentifiers
            End If
            Return _finder
        End Get
    End Property
    Private ReadOnly Property Protocols As IList(Of Protocol)
        Get
            If _protocols Is Nothing Then
                Dim headers As IList(Of ProtocolHeader) = Finder.DoSearchHeader()
                Dim identifiers As IList(Of Guid) = headers.Select(Function(h) h.UniqueId).ToList()
                _protocols = Facade.ProtocolFacade.GetProtocols(identifiers, NHibernateProtocolDao.FetchingStrategy.BasicDataAndLocation)
            End If
            Return _protocols
        End Get
    End Property
    Private Property ProtocolImported As Integer
        Get
            If Session(ProtocolImportedSessionName) Is Nothing Then
                Session(ProtocolImportedSessionName) = 0
            End If
            Return Session(ProtocolImportedSessionName)
        End Get
        Set(value As Integer)
            Session(ProtocolImportedSessionName) = value
        End Set
    End Property

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(gvProtocols, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest_TaskCompleted
        WindowBuilder.RegisterWindowManager(windowManager)
    End Sub

    Private Sub InitializeTask()
        _task = GlobalAsax.LongRunningTask
        If _task.Running Then
            SetupPageWithTaskRunning()
        End If
    End Sub

    Private Sub BindGrid()
        btnImport.Enabled = Not Protocols.IsNullOrEmpty()
        Dim paging As NHibernateProtocolHeaderFinderTD = Finder
        paging.EnablePaging = True
        gvProtocols.Finder = paging
        gvProtocols.PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
        gvProtocols.DataBindFinder()
    End Sub

    Private Sub VerifyImportComplete()
        If GlobalAsax.LongRunningTask.LastTaskSuccess Then
            AjaxAlert("Importazione completata, protocolli importati: " & ProtocolImported.ToString())
            ProtocolImported = 0
        Else
            AjaxAlert("Si sono verificati degli errori durante il processo d\'importazione.\nEccezione sollevata: " & StringHelper.ReplaceAlert(GlobalAsax.LongRunningTask.ExceptionOccured.Message))
        End If
        BindGrid()
    End Sub

    Private Sub SetupPageWithTaskRunning()
        Dim url As String = "..\Prot\ProtImportProgress.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&Title=Importazione TopMedia")
        WindowBuilder.LoadWindow("wndProgress", url, "onTaskCompleted", Unit.Pixel(500), Unit.Pixel(250))
    End Sub

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not IsPostBack Then
            BindGrid()
        End If
        InitializeAjaxSettings()
        InitializeTask()
    End Sub

    Private Sub AjaxRequest_TaskCompleted(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        VerifyImportComplete()
    End Sub

    Private Sub gvProtocols_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles gvProtocols.ItemCommand
        Select Case e.CommandName
            Case "ShowProt"
                Dim uniqueIdProtocol As Guid = Guid.Parse(e.CommandArgument.ToString())
                Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={uniqueIdProtocol}&Type=Prot")}")

        End Select
    End Sub

    Private Sub btnImport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnImport.Click
        If Not _task.Running Then
            _task = GlobalAsax.LongRunningTask
            _task.TaskUser = DocSuiteContext.Current.User.UserName
            _task.StepsCount = Finder.Count
            _task.TaskToExecute = New MultiStepLongRunningTask.TaskToExec(AddressOf ImportProtocol)
            _task.RunTask()

            SetupPageWithTaskRunning()
            ProtocolImported = 0
        Else
            AjaxAlert("Nessun file da importare.")
        End If
    End Sub

    Private Sub ImportProtocol(ByVal currentStep As Int32)
        Try
            FacadeFactory.Instance.ProtocolFacade.RaiseAfterInsert(Protocols(currentStep))
            Dim currentProtocol As Protocol = Protocols(currentStep)
            currentProtocol = FacadeFactory.Instance.ProtocolFacade.GetById(currentProtocol.Id)
            If currentProtocol.TDIdDocument.HasValue AndAlso String.IsNullOrEmpty(currentProtocol.TDError) Then
                ProtocolImported += 1
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
        End Try
    End Sub

#End Region

End Class