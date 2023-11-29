Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Logging

Public Class OChartItemFacade
    Inherits FacadeNHibernateBase(Of OChartItem, Guid, NHibernateOChartItemDao)

#Region " Costructor "
    Private _userName As String
    Public Sub New(userName As String)
        MyBase.New()
        _userName = userName
    End Sub
#End Region

#Region " Methods "

    Public Function GetByCode(code As String) As IList(Of OChartItem)
        Return _dao.GetByCode(code)
    End Function

    Public Function GetByFullCode(fullCode As String) As IList(Of OChartItem)
        Return _dao.GetByFullCode(fullCode)
    End Function

    Public Function GetVariations(source As OChart, destination As OChart) As IList(Of OChartItem)
        Return _dao.GetVariations(source, destination)
    End Function

    Protected Function CrudManager(Of TEntity)(ByRef crud_action As Action(Of OChartItem, TEntity),
                                                  ByRef predicate_condition As Func(Of OChartItem, TEntity, Boolean),
                                                  entity As TEntity, oChartItem As OChartItem,
                                               oChartItemCode As String,
                                               Optional externalTransaction As Boolean = False) As OChart
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Dim step_i As String = "init"
        Dim oChartCurrent As OChart = oChartItem.OrganizationChart
        Dim oChartItemToRefresh As OChartItem = Nothing
        Try
            If Not externalTransaction Then
                step_i = "BeginTransaction"
                unitOfWork.BeginTransaction()
            End If

            step_i = "CrudManager in OChartItem"
            crud_action(oChartItem, entity)

            step_i = "Update oChartItem"
            UpdateWithoutTransaction(oChartItem)

            step_i = "GetFollowings"
            Dim followingOCharts As IList(Of OChart) = FacadeFactory.Instance.OChartFacade.GetFollowings(oChartCurrent)
            If (followingOCharts IsNot Nothing AndAlso followingOCharts.Count > 0) Then
                step_i = "Replicate changes in following ochart"
                For Each oChart As OChart In followingOCharts.Where(Function(f) f.HasItems)
                    step_i = "Replicate changes in ochartitem"
                    oChartItemToRefresh = oChart.Items.SingleOrDefault(Function(f) f.Code.Equals(oChartItemCode))
                    If (oChartItemToRefresh IsNot Nothing AndAlso predicate_condition(oChartItemToRefresh, entity)) Then
                        step_i = String.Format("Update changes in oChartItemToRefresh {0}", oChartItemToRefresh.Id)
                        crud_action(oChartItemToRefresh, entity)
                        UpdateWithoutTransaction(oChartItemToRefresh)
                    End If
                Next
            End If

            If Not externalTransaction Then
                step_i = "Commit"
                unitOfWork.Commit()
            End If

        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, String.Format("OChartItem CrudManager - Errore Inserimento step '{0}'", step_i), ex)

            If Not externalTransaction Then
                unitOfWork.Rollback()
            End If

            Throw New Exception(String.Format("OChartItem CrudManager - Errore Inserimento step '{0}'. {1}", step_i, ex.Message))
        End Try

        Return oChartCurrent
    End Function

    Public Function AddContact(contact As Contact, oChartItem As OChartItem, Optional externalTransaction As Boolean = False) As OChart
        Return CrudManager(Sub(f, c) f.AddContact(c),
                           Function(item, c) item.Contacts.FirstOrDefault(Function(f) f.Contact.Id = contact.Id) Is Nothing,
                           contact, oChartItem, oChartItem.Code, externalTransaction)
    End Function

    Public Function RemoveContact(contact As Contact, oChartItem As OChartItem) As OChart
        Return CrudManager(Sub(f, c) f.RemoveContact(c),
                           Function(item, c) item.Contacts.FirstOrDefault(Function(f) f.Contact.Id = contact.Id) IsNot Nothing,
                           contact, oChartItem, oChartItem.Code)
    End Function

    Public Function AddOChartItem(oChartItem As OChartItem, Optional externalTransaction As Boolean = False) As OChart
        Dim step_i As String = "BeginTransaction"
        Dim oChartCurrent As OChart = oChartItem.OrganizationChart
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Dim oChartItemToRefresh As OChartItem = Nothing
        Dim oChartItemToRefreshParent As OChartItem = Nothing

        Try
            step_i = "BeginTransaction"
            unitOfWork.BeginTransaction()

            step_i = "CrudManager in OChartItem"
            If oChartItem.Parent IsNot Nothing Then
                oChartItem.Parent.AddChild(oChartItem)
            Else
                oChartItem.SetCodes()
            End If

            step_i = "Update oChartItem"
            UpdateWithoutTransaction(oChartItem)

            step_i = "GetFollowings"
            Dim followingOCharts As IList(Of OChart) = FacadeFactory.Instance.OChartFacade.GetFollowings(oChartCurrent)
            If (followingOCharts IsNot Nothing AndAlso followingOCharts.Count > 0) Then
                step_i = "Replicate changes in following ochart"
                For Each oChart As OChart In followingOCharts.Where(Function(f) Not f.Items.Any(Function(i) i.Code = oChartItem.Code))
                    oChartItemToRefreshParent = Nothing
                    step_i = "Replicate changes in ochartitem"
                    If oChartItem.Parent IsNot Nothing AndAlso oChart.HasItems Then
                        oChartItemToRefreshParent = oChart.Items.SingleOrDefault(Function(f) f.Code.Equals(oChartItem.Parent.Code))
                    End If
                    oChartItemToRefresh = New OChartItem(oChartItem, oChartItemToRefreshParent, oChart, _userName)
                    SaveWithoutTransaction(oChartItemToRefresh)
                Next
            End If
            unitOfWork.Commit()
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, String.Format("AddOChartItem CrudManager - Errore Inserimento step '{0}'", step_i), ex)
            unitOfWork.Rollback()
        End Try
        Return oChartCurrent
    End Function

    Public Function UpdateOChartItem(oChartItem As OChartItem, originaloChartItemCode As String, Optional externalTransaction As Boolean = False) As OChart
        Return CrudManager(Sub(f, c) f.Swap(c),
                           Function(item, c) True, oChartItem, oChartItem, originaloChartItemCode, externalTransaction)
    End Function

    Public Function RemoveOChartItem(oChartItem As OChartItem) As OChart
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Dim step_i As String = "init"
        Dim oChartCurrent As OChart = oChartItem.OrganizationChart
        Dim oChartItemToRefresh As OChartItem = Nothing
        Try
            unitOfWork.BeginTransaction()
            step_i = "CrudManager in oChartItem"
            If (oChartItem.HasItems OrElse oChartItem.HasResources) Then
                Throw New Exception(String.Format("Impossibile eliminare il nodo '{0}' ({1}) se contiente dei sottonodi o se contiente 'Risorse' associate", oChartItem.Title, oChartItem.FullCode))
            End If

            step_i = "Update oChartItem"
            oChartCurrent.Items.Remove(oChartItem)
            DeleteWithoutTransaction(GetById(oChartItem.Id))

            step_i = "GetFollowings"
            Dim followingOCharts As IList(Of OChart) = FacadeFactory.Instance.OChartFacade.GetFollowings(oChartCurrent)
            If (followingOCharts IsNot Nothing AndAlso followingOCharts.Count > 0) Then
                step_i = "Replicate changes in following ochart"
                For Each oChartToRefresh As OChart In followingOCharts.Where(Function(f) f.HasItems)
                    step_i = "Replicate changes in ochart"
                    oChartItemToRefresh = oChartToRefresh.Items.SingleOrDefault(Function(f) f.Code.Equals(oChartItem.Code))
                    If (oChartItemToRefresh IsNot Nothing) Then
                        If (Not oChartItemToRefresh.HasItems AndAlso Not oChartItemToRefresh.HasResources) Then
                            oChartToRefresh.Items.Remove(oChartItemToRefresh)
                            DeleteWithoutTransaction(oChartItemToRefresh)
                        End If
                    End If
                Next

            End If
            unitOfWork.Commit()
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, String.Format("OChart RemoveOChartItem - Errore Inserimento step '{0}'", step_i), ex)
            unitOfWork.Rollback()
            Throw New Exception(String.Format("OChart RemoveOChartItem - Errore Inserimento step '{0}'. {1}", step_i, ex.Message))
        End Try
        Return oChartCurrent
    End Function

    Public Function AddRole(role As Role, oChartItem As OChartItem, contacts As IList(Of Contact)) As OChart
        Dim step_i As String = "BeginTransaction"
        Dim ochart As OChart = Nothing
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)

        Try
            step_i = "BeginTransaction"
            unitOfWork.BeginTransaction()

            step_i = "CrudManager"
            ochart = CrudManager(Sub(f, c) f.AddRole(c),
                                 Function(item, c) item.Roles.FirstOrDefault(Function(f) f.Role.Id = role.Id) Is Nothing,
                                 role, oChartItem, oChartItem.Code, externalTransaction:=True)

            If (contacts IsNot Nothing AndAlso contacts.Count > 0) Then
                step_i = "contacts"
                For Each contact As Contact In contacts
                    AddContact(contact, oChartItem, True)
                Next
            End If
            unitOfWork.Commit()
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, String.Format("AddRole CrudManager - Errore Inserimento step '{0}'", step_i), ex)
            unitOfWork.Rollback()
        End Try
        Return ochart

    End Function

    Public Function RemoveRole(role As Role, oChartItem As OChartItem) As OChart
        Return CrudManager(Sub(f, c) f.RemoveRole(c),
                           Function(item, c) item.Roles.FirstOrDefault(Function(f) f.Role.Id = role.Id) IsNot Nothing,
                           role, oChartItem, oChartItem.Code)
    End Function

    Public Function AddContainer(container As Container, oChartItem As OChartItem) As OChart
        Return CrudManager(Sub(f, c)
                               Dim oChart As OChart = f.OrganizationChart
                               Dim isMaster As Boolean = oChart.Items.FindResourceMaster(c) Is Nothing
                               Dim oChartItemContainer As New OChartItemContainer With {.Container = c, .Master = isMaster}

                               If DocSuiteContext.Current.ProtocolEnv.ProtocolRejectionEnabled Then
                                   oChartItemContainer.Rejection = oChart.Items.GetContainerRejection(c)
                               End If

                               f.AddContainer(c, oChartItemContainer)
                           End Sub,
                           Function(item, c) item.Containers.FirstOrDefault(Function(f) f.Container.Id = container.Id) Is Nothing,
                           container, oChartItem, oChartItem.Code)
    End Function

    Public Function RemoveContainer(container As Container, oChartItem As OChartItem) As OChart
        Return CrudManager(Sub(f, c) f.RemoveContainer(c),
                           Function(item, c) item.Containers.FirstOrDefault(Function(f) f.Container.Id = container.Id) IsNot Nothing,
                           container, oChartItem, oChartItem.Code)
    End Function

    Public Function GetByRole(role As Role) As ICollection(Of OChartItem)
        Return _dao.GetByRole(role)
    End Function

    Public Function GetByContact(contact As Contact) As ICollection(Of OChartItem)
        Return _dao.GetByContact(contact)
    End Function
    Public Sub CalculateFullCode(ByRef item As OChartItem)
        If item.Parent Is Nothing Then
            item.FullCode = item.Code
        Else
            item.FullCode = String.Format("{0}|{1}", item.Parent.FullCode, item.Code)
        End If
    End Sub
    Public Sub CalculateFullCodeRecursive(ByRef childs As IList(Of OChartItem))
        If (childs Is Nothing) Then
            Return
        End If
        For Each item As OChartItem In childs
            Dim obj As OChartItem = GetById(item.Id)
            CalculateFullCode(obj)
            UpdateOnly(obj)
            obj = GetById(item.Id)
            CalculateFullCodeRecursive(obj.Items)
        Next
    End Sub

#End Region

End Class
