Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Logging

Public Class OChartItemContainerFacade
    Inherits FacadeNHibernateBase(Of OChartItemContainer, Guid, NHibernateOChartItemContainerDao)

#Region " Costructor "
    Private _userName As String
    Public Sub New(userName As String)
        MyBase.New()
        _userName = userName
    End Sub
#End Region

    Private Sub ReplicateMasterTrasformer(oChartItem As OChartItem, container As Container)
        Dim itemContainer As OChartItemContainer = oChartItem.Containers.Single(Function(c) c.Container.Id = container.Id)
        Dim reversalContainers As IList(Of OChartItemContainer) = GetMastersByContainer(itemContainer)

        itemContainer.Master = True
        For Each item As OChartItemContainer In reversalContainers
            item.Master = False
            UpdateWithoutTransaction(item)
        Next
        UpdateWithoutTransaction(itemContainer)
    End Sub

    Private Sub ReplicateRejectionTrasformer(oChartItem As OChartItem, container As Container, rejectionValue As Boolean)
        Dim itemContainer As OChartItemContainer = oChartItem.Containers.Single(Function(c) c.Container.Id = container.Id)
        Dim reversalContainers As IList(Of OChartItemContainer) = GetRejectionsByContainer(itemContainer)

        itemContainer.Rejection = rejectionValue
        For Each item As OChartItemContainer In reversalContainers
            item.Rejection = rejectionValue
            UpdateWithoutTransaction(item)
        Next
        UpdateWithoutTransaction(itemContainer)
    End Sub

    Protected Function CrudManager(crud_action As Action(Of OChartItem, Container),
                                   predicate_condition As Func(Of IList(Of OChartItemContainer), Container, Boolean),
                                   oChartItem As OChartItem, container As Container) As OChart
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Dim step_i As String = "init"
        Dim oChartCurrent As OChart = oChartItem.OrganizationChart
        Dim oChartItemToRefresh As OChartItem = Nothing
        Try
            step_i = "BeginTransaction"
            unitOfWork.BeginTransaction()

            step_i = "CrudManager in OChartItemContainer"

            crud_action(oChartItem, container)

            step_i = "GetFollowings"
            Dim followingOCharts As IList(Of OChart) = FacadeFactory.Instance.OChartFacade.GetFollowings(oChartCurrent)
            If (followingOCharts IsNot Nothing AndAlso followingOCharts.Count > 0) Then
                step_i = "Replicate changes in following ochart"
                For Each oChart As OChart In followingOCharts.Where(Function(f) f.HasItems)
                    step_i = "Replicate changes in OChartItemContainer"
                    oChartItemToRefresh = oChart.Items.SingleOrDefault(Function(f) f.Code.Equals(oChartItem.Code) AndAlso predicate_condition(f.Containers, container))
                    If (oChartItemToRefresh IsNot Nothing) Then
                        step_i = String.Format("Update changes in oChartItemToRefresh {0}", oChartItemToRefresh.Id)
                        crud_action(oChartItemToRefresh, container)
                    End If
                Next
            End If

            step_i = "Commit"
            unitOfWork.Commit()
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, String.Format("CrudManager - Errore Inserimento step '{0}'", step_i), ex)
            unitOfWork.Rollback()

            Throw New Exception(String.Format("CrudManager - Errore Inserimento step '{0}'. {1}", step_i, ex.Message))
        End Try
        Return oChartCurrent

    End Function

    Public Function MasterTrasformer(oChartItem As OChartItem, container As Container) As OChart
        Return CrudManager(Sub(i, c) ReplicateMasterTrasformer(i, c),
                           Function(f, con) f.Any(Function(c) c.Container.Id = con.Id AndAlso c.Master.HasValue AndAlso Not c.Master.Value),
                           oChartItem, container)
    End Function

    Public Function RejectionTrasformer(oChartItem As OChartItem, container As Container, rejectionValue As Boolean) As OChart
        Return CrudManager(Sub(i, c) ReplicateRejectionTrasformer(i, c, rejectionValue),
                           Function(f, con) f.Any(Function(c) c.Container.Id = con.Id AndAlso c.Rejection.HasValue AndAlso c.Rejection.Value = Not rejectionValue),
                           oChartItem, container)
    End Function
    Public Function GetMastersByContainer(itemContainer As OChartItemContainer) As IList(Of OChartItemContainer)
        Return _dao.GetMastersByContainer(itemContainer)
    End Function

    Public Function GetRejectionsByContainer(itemContainer As OChartItemContainer) As IList(Of OChartItemContainer)
        If itemContainer.IsRejection AndAlso itemContainer.Container.Id.Equals(DocSuiteContext.Current.ProtocolEnv.ProtocolRejectionContainerId) Then
            Throw New DocSuiteException("Errore di configurazione contenitore", "Il contenitore dei protocolli rigettati non può essere a sua volta soggetto a rigetto.")
        End If
        Return _dao.GetRejectionsByContainer(itemContainer)
    End Function

    Public Function GetVariations(source As OChart, destination As OChart) As IList(Of OChartItemContainer)
        Return _dao.GetVariations(source, destination)
    End Function

End Class
