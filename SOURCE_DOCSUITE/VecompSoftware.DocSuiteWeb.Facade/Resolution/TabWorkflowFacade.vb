Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class TabWorkflowFacade
    Inherits BaseResolutionFacade(Of TabWorkflow, Integer, NHibernateTabWorkflowDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetActive(resolution As Resolution) As TabWorkflow
        Dim incremental As Short = Factory.ResolutionWorkflowFacade.GetActiveIncremental(resolution.Id, 1)
        Return GetTabWorkflow(resolution, incremental)
    End Function

    Public Function GetTabWorkflow(resolution As Resolution, incremental As Short) As TabWorkflow
        Dim key As New ResolutionWorkflowCompositeKey() With {.IdResolution = resolution.Id, .Incremental = incremental}
        Dim wf As ResolutionWorkflow = Factory.ResolutionWorkflowFacade.GetById(key)
        Return GetTabWorkflow(resolution, wf)
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

#End Region

End Class