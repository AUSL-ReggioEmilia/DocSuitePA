Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports System.Linq

<DataObject()>
Public Class TabWorkflowFacade
    Inherits BaseResolutionFacade(Of TabWorkflow, Integer, NHibernateTabWorkflowDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetActive(resolution As Resolution) As TabWorkflow
        Dim incremental As Short = Factory.ResolutionWorkflowFacade.GetActiveIncremental(resolution.Id, 1S)
        Return GetTabWorkflow(resolution, incremental)
    End Function

    Public Function GetTabWorkflow(resolution As Resolution, incremental As Short) As TabWorkflow
        Dim key As New ResolutionWorkflowCompositeKey() With {.IdResolution = resolution.Id, .Incremental = incremental}
        Dim wf As ResolutionWorkflow = Factory.ResolutionWorkflowFacade.GetById(key)
        Return GetTabWorkFlow(resolution, wf)
    End Function

    Public Function GetTabWorkFlow(resolution As Resolution, workflow As ResolutionWorkflow) As TabWorkflow
        Dim tab As New TabWorkflow
        If (workflow Is Nothing) Then
            Return Nothing
        End If
        GetByStep(resolution.WorkflowType, workflow.ResStep.Value, tab)
        Return tab
    End Function

    Public Function GetByStep(ByVal workflowType As String, ByVal searchStep As Short, ByRef workflow As TabWorkflow) As Boolean
        Return _dao.GetByStep(workflowType, searchStep, workflow)
    End Function

    Public Function GetAllNextStep(ByVal workflowType As String, ByVal activeStep As Short, ByRef tabWorkflows As IList(Of TabWorkflow)) As Boolean
        Return _dao.GetAllNextStep(workflowType, activeStep, tabWorkflows)
    End Function

    Public Function GetChangeableData(ByVal idResolution As Integer, ByVal workflowType As String, ByVal searchStep As Short, ByRef changeableData As String) As Boolean
        ' TODO: usando due diverse dao questa cosa dovrebbe stare a livello facade
        Dim daoWorkflow As New NHibernateResolutionWorkflowDao(_dbName)
        Dim reslWorkflow As ResolutionWorkflow = daoWorkflow.SqlResolutionWorkflowSearch(idResolution, searchStep)

        Dim workflow As TabWorkflow = Nothing
        If GetByStep(workflowType, reslWorkflow.ResStep.Value, workflow) Then
            changeableData = workflow.ChangeableData
            Return True
        End If

        Return False
    End Function

    Function GetByResolutionType(ByVal resolutionType As Short) As IList(Of TabWorkflow)
        Return _dao.GetByResolutionType(resolutionType)
    End Function

    Function GetByResolution(ByVal idResolution As Integer) As TabWorkflow
        Return _dao.GetByResolution(idResolution)
    End Function

    Function GetItemsByDescription(ByVal description As String) As IList(Of TabWorkflow)
        Return _dao.GetItemsByDescription(description)
    End Function

    Function GetByDescription(ByVal description As String, ByVal workflowType As String) As TabWorkflow
        Return _dao.GetByDescription(description, workflowType)
    End Function

    Function GetDescriptionByResolution(ByVal idResolution As Integer) As String
        Dim workflow As TabWorkflow = _dao.GetByResolution(idResolution)
        If workflow Is Nothing Then
            Return String.Empty
        End If
        Return workflow.Description
    End Function

    Public Function SqlTabWorkflowManagedWData(ByVal idResolution As Integer, ByVal workflowType As String, ByVal searchStep As Short, ByRef managedWorkflowData As String) As Boolean
        'Se passo uno step mi ritorna la riga di quel step, altrimenti mi passa la riga dello step attivo
        Dim myStep As Short
        If (searchStep = 0) Then
            Dim resWorkflow As ResolutionWorkflow = Factory.ResolutionWorkflowFacade.SqlResolutionWorkflowSearch(idResolution, searchStep)
            myStep = resWorkflow.ResStep.Value
        Else
            myStep = searchStep
        End If

        Dim workflow As TabWorkflow = Nothing
        If Factory.TabWorkflowFacade.GetByStep(workflowType, myStep, workflow) Then
            managedWorkflowData = workflow.ManagedWorkflowData
        End If

        Return workflow IsNot Nothing
    End Function

    Public Shared Function TestManagedWorkflowDataProperty(ByVal managedWData As String, ByVal fld As String, ByVal fldProperty As String, ByVal fldPropertyOpt As String) As Boolean
        If Not StringHelper.InStrTest(managedWData, fld) Then
            Exit Function
        End If
        '--
        Dim s() As String = Split(managedWData, "|")
        Dim b As Boolean
        Dim sAllProp As String = fldProperty & If(Not String.IsNullOrEmpty(fldPropertyOpt), "-" & fldPropertyOpt, "")
        For i As Integer = 0 To UBound(s)
            If StringHelper.InStrTest(s(i), fld) AndAlso StringHelper.InStrTest(s(i), sAllProp) Then
                b = True
                Exit For
            End If
        Next
        Return b
    End Function

    Public Shared Function GetPipedParametersFromWorkflowManagedData(managedWorkflowData As String, mainKey As String) As String()
        If Not StringHelper.InStrTest(managedWorkflowData, mainKey) Then
            Throw New DocSuiteException("Errore ricerca parametri", String.Format("Chiave {0} non trovata.", mainKey))
        End If
        Dim retvalStringArray As String() = Mid(managedWorkflowData, InStr(UCase(managedWorkflowData), UCase(mainKey))).Split("["c)(1).Split("]"c)(0).Split("|"c)
        Return retvalStringArray
    End Function

    Public Shared Function GetAccountingBidType(managedWData As String) As String
        Dim results As String = String.Empty

        Dim operationSteps As ICollection(Of String) = managedWData.Split("|"c)
        If operationSteps.Any(Function(x) x.StartsWith("Accounting")) Then
            results = operationSteps.First(Function(x) x.StartsWith("Accounting")) _
                .Replace("Accounting", String.Empty) _
                .Replace("[.", String.Empty) _
                .Replace(".]", String.Empty)
        End If

        Return results
    End Function

    Public Function GetOperationStepVisibilityRoles(resolution As Resolution) As ICollection(Of Integer)
        Dim currentStep As TabWorkflow = GetActive(resolution)
        If (currentStep Is Nothing) Then
            Return New List(Of Integer)
        End If
        Return GetOperationStepVisibilityRoles(currentStep)
    End Function

    Public Function GetOperationStepVisibilityRoles(currentStep As TabWorkflow) As ICollection(Of Integer)
        Dim results As ICollection(Of Integer) = New List(Of Integer)

        If String.IsNullOrEmpty(currentStep.OperationStep) Then
            Return results
        End If

        Dim operationSteps As ICollection(Of String) = currentStep.OperationStep.Split("|"c)
        If operationSteps.Any(Function(x) x.StartsWith("VIEWROLES")) Then
            results = operationSteps.First(Function(x) x.StartsWith("VIEWROLES")) _
                .Replace("VIEWROLES", String.Empty) _
                .Replace("[", String.Empty) _
                .Replace("]", String.Empty).Split("&"c) _
                .Select(Function(s) Integer.Parse(s)).ToList()
        End If
        Return results
    End Function

    Public Function GetOperationStepVisibilityContainers(resolution As Resolution) As ICollection(Of Integer)
        Dim currentStep As TabWorkflow = GetActive(resolution)
        If (currentStep Is Nothing) Then
            Return New List(Of Integer)
        End If
        Return GetOperationStepVisibilityContainers(currentStep)
    End Function

    Public Function GetOperationStepVisibilityContainers(currentStep As TabWorkflow) As ICollection(Of Integer)
        Dim results As ICollection(Of Integer) = New List(Of Integer)

        If String.IsNullOrEmpty(currentStep.OperationStep) Then
            Return results
        End If

        Dim operationSteps As ICollection(Of String) = currentStep.OperationStep.Split("|"c)
        If operationSteps.Any(Function(x) x.StartsWith("VIEWCONTAINERS")) Then
            results = operationSteps.First(Function(x) x.StartsWith("VIEWCONTAINERS")) _
                .Replace("VIEWCONTAINERS", String.Empty) _
                .Replace("[", String.Empty) _
                .Replace("]", String.Empty).Split("&"c) _
                .Select(Function(s) Integer.Parse(s)).ToList()
        End If
        Return results
    End Function

    Public Function GetOperationStepFlowResponsability(resolution As Resolution) As ICollection(Of AuthorizationRoleType)
        Dim currentStep As TabWorkflow = GetActive(resolution)
        If (currentStep Is Nothing) Then
            Return New List(Of AuthorizationRoleType)
        End If
        Return GetOperationStepFlowResponsability(currentStep)
    End Function

    Public Function GetOperationStepFlowResponsability(currentStep As TabWorkflow) As ICollection(Of AuthorizationRoleType)
        Dim results As ICollection(Of AuthorizationRoleType) = New List(Of AuthorizationRoleType)

        If String.IsNullOrEmpty(currentStep.OperationStep) Then
            Return results
        End If

        Dim operationSteps As ICollection(Of String) = currentStep.OperationStep.Split("|"c)
        If operationSteps.Any(Function(x) x.StartsWith("FLOWRESPONSABILITY[")) Then
            results = operationSteps.First(Function(x) x.StartsWith("FLOWRESPONSABILITY[")) _
                .Replace("FLOWRESPONSABILITY[", String.Empty) _
                .Replace("]", String.Empty).Split("&"c) _
                .Select(Function(s) DirectCast([Enum].Parse(GetType(AuthorizationRoleType), s), AuthorizationRoleType)).ToList()
        End If
        Return results
    End Function

    Public Function GetOperationStepFlowResponsabilityRoles(resolution As Resolution) As ICollection(Of Integer)
        Dim currentStep As TabWorkflow = GetActive(resolution)
        If (currentStep Is Nothing) Then
            Return New List(Of Integer)
        End If
        Return GetOperationStepFlowResponsabilityRoles(currentStep)
    End Function

    Public Function GetOperationStepFlowResponsabilityRoles(currentStep As TabWorkflow) As ICollection(Of Integer)
        Dim results As ICollection(Of Integer) = New List(Of Integer)

        If String.IsNullOrEmpty(currentStep.OperationStep) Then
            Return results
        End If

        Dim operationSteps As ICollection(Of String) = currentStep.OperationStep.Split("|"c)
        If operationSteps.Any(Function(x) x.StartsWith("FLOWRESPONSABILITYROLES")) Then
            results = operationSteps.First(Function(x) x.StartsWith("FLOWRESPONSABILITYROLES")) _
                .Replace("FLOWRESPONSABILITYROLES", String.Empty) _
                .Replace("[", String.Empty) _
                .Replace("]", String.Empty).Split("&"c) _
                .Select(Function(s) Integer.Parse(s)).ToList()
        End If
        Return results
    End Function

    Public Function HasUserStepVisibility(account As AccountModel, stepDescription As String, tabMaster As TabMaster) As Boolean
        Dim stepVisible As Boolean = False
        If tabMaster IsNot Nothing Then
            Dim currentStep As TabWorkflow = FacadeFactory.Instance.TabWorkflowFacade.GetByDescription(stepDescription, tabMaster.WorkflowType)
            If currentStep IsNot Nothing Then
                stepVisible = HasUserStepVisibility(account, currentStep)
            End If
        End If
        Return stepVisible
    End Function

    Public Function HasUserStepVisibility(account As AccountModel, currentStep As TabWorkflow) As Boolean
        Dim stepVisible As Boolean = False
        If currentStep IsNot Nothing Then
            Dim stepRolesVisibility As ICollection(Of Integer) = GetOperationStepVisibilityRoles(currentStep)
            Dim stepContainersVisibility As ICollection(Of Integer) = GetOperationStepVisibilityContainers(currentStep)
            If stepRolesVisibility IsNot Nothing AndAlso stepRolesVisibility.Count > 0 Then
                Dim stepRoles As IList(Of Role) = FacadeFactory.Instance.RoleFacade.GetByIds(stepRolesVisibility)
                stepVisible = FacadeFactory.Instance.RoleFacade.UserBelongsToRoles(account.Domain, account.Account, DSWEnvironment.Resolution, stepRoles)
            End If
            If Not stepVisible AndAlso stepContainersVisibility IsNot Nothing AndAlso stepContainersVisibility.Count > 0 Then
                Dim userContainers As ICollection(Of Data.Container) = FacadeFactory.Instance.ContainerFacade.GetContainers(account.Domain, account.Account, DSWEnvironment.Resolution, Nothing, True)
                stepVisible = userContainers.Any(Function(x) stepContainersVisibility.Any(Function(xx) xx.Equals(x.Id)))
            End If
        End If
        Return stepVisible
    End Function

    Public Function GetOperationStepConfirmViewResponsabilityGroups(resolution As Resolution) As ICollection(Of Integer)
        Dim currentStep As TabWorkflow = GetActive(resolution)
        If (currentStep Is Nothing) Then
            Return New List(Of Integer)
        End If
        Return GetOperationStepConfirmViewResponsabilityGroups(currentStep)
    End Function

    Public Function GetOperationStepConfirmViewResponsabilityGroups(currentStep As TabWorkflow) As ICollection(Of Integer)
        Dim results As ICollection(Of Integer) = New List(Of Integer)

        If String.IsNullOrEmpty(currentStep.OperationStep) Then
            Return results
        End If

        Dim operationSteps As ICollection(Of String) = currentStep.OperationStep.Split("|"c)
        If operationSteps.Any(Function(x) x.StartsWith("CONFIRMVIEWRESPONSABILITYGROUPS")) Then
            results = operationSteps.First(Function(x) x.StartsWith("CONFIRMVIEWRESPONSABILITYGROUPS")) _
                .Replace("CONFIRMVIEWRESPONSABILITYGROUPS", String.Empty) _
                .Replace("[", String.Empty) _
                .Replace("]", String.Empty).Split("&"c) _
                .Select(Function(s) Integer.Parse(s)).ToList()
        End If
        Return results
    End Function

    Public Function GetOperationStepAccountingBidType(resolution As Resolution) As String
        Dim currentStep As TabWorkflow = GetActive(resolution)
        If (currentStep Is Nothing) Then
            Return String.Empty
        End If
        Return GetOperationStepAccountingBidType(currentStep)
    End Function

    Public Function GetOperationStepAccountingBidType(currentStep As TabWorkflow) As String
        If String.IsNullOrEmpty(currentStep.OperationStep) Then
            Return String.Empty
        End If

        Dim operationSteps As ICollection(Of String) = currentStep.OperationStep.Split("|"c)
        If operationSteps.Any(Function(x) x.StartsWith("ACCOUNTING")) Then
            Return operationSteps.First(Function(x) x.StartsWith("ACCOUNTING")) _
                .Replace("ACCOUNTING", String.Empty) _
                .Replace("[", String.Empty) _
                .Replace("]", String.Empty)
        End If
        Return String.Empty
    End Function

    Public Function GetOperationStepEconomicDataGroups(resolution As Resolution) As ICollection(Of Integer)
        Dim currentStep As TabWorkflow = GetActive(resolution)
        If (currentStep Is Nothing) Then
            Return New List(Of Integer)
        End If
        Return GetOperationStepEconomicDataGroups(currentStep)
    End Function

    Public Function GetOperationStepEconomicDataGroups(currentStep As TabWorkflow) As ICollection(Of Integer)
        Dim results As ICollection(Of Integer) = New List(Of Integer)

        If String.IsNullOrEmpty(currentStep.OperationStep) Then
            Return results
        End If

        Dim operationSteps As ICollection(Of String) = currentStep.OperationStep.Split("|"c)
        Dim economicDataGroupsStep As String = operationSteps.FirstOrDefault(Function(x) x.StartsWith("ECONOMICDATAGROUPS"))

        If Not String.IsNullOrEmpty(economicDataGroupsStep) Then
            ' Check if there are elements inside the brackets []
            Dim startIndex As Integer = economicDataGroupsStep.IndexOf("["c)
            Dim endIndex As Integer = economicDataGroupsStep.IndexOf("]"c)

            If startIndex >= 0 AndAlso endIndex >= 0 AndAlso startIndex < endIndex - 1 Then
                ' Extract the content inside the brackets and split it by '&'
                Dim content As String = economicDataGroupsStep.Substring(startIndex + 1, endIndex - startIndex - 1)

                ' Parse each element and add to the results if parsing is successful
                Dim elements As String() = content.Split("&"c)
                Dim value As Integer
                For Each element As String In elements
                    If Integer.TryParse(element, value) Then
                        results.Add(value)
                    End If
                Next
            End If
        End If

        Return results
    End Function

#End Region

End Class