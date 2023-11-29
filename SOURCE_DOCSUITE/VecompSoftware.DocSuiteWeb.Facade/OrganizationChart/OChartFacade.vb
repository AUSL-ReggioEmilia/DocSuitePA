Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Logging

Public Class OChartFacade
    Inherits FacadeNHibernateBase(Of OChart, Guid, NHibernateOChartDao)

#Region " Costructor "
    Private _userName As String
    Public Sub New(userName As String)
        MyBase.New()
        _userName = userName
    End Sub
#End Region

#Region " Methods "

    Public Function GetEnabled() As IList(Of OChart)
        Dim finder As New OChartFinder()
        finder.IsEnabled = True
        Return finder.DoSearch()
    End Function

    Public Function EffectiveOrDefault() As OChart
        Dim finder As New OChartFinder()
        finder.IsEnabled = True
        finder.EffectivenessLowerBound = True
        Return finder.DoSearch().SingleOrDefault()
    End Function

    Public Function GetEffective() As OChart
        Dim effective As OChart = EffectiveOrDefault()
        If effective IsNot Nothing Then
            Return GetHierarchy(effective.Id)
        End If
        Return Nothing
    End Function

    Public Function GetPreviousOrDefault(oChart As OChart, Optional needTransaction As Boolean = True) As OChart
        Dim previous As OChart = _dao.GetPrevious(oChart)
        If previous Is Nothing Then
            previous = New OChart() With {.Items = New List(Of OChartItem)}
            If needTransaction Then
                Save(previous)
            Else
                SaveWithoutTransaction(previous)
            End If
        End If
        Return previous
    End Function

    Public Function GetFollowingOrDefault(oChart As OChart) As OChart
        Return _dao.GetFollowing(oChart)
    End Function

    Public Function GetFollowings(oChart As OChart) As IList(Of OChart)
        Return _dao.GetFollowings(oChart)
    End Function

    Public Function GetHierarchy(id As Guid) As OChart
        Return _dao.GetHierarchy(id)
    End Function

    Public Function Transform(dtos As IEnumerable(Of OChartTransformerDTO)) As List(Of OChart)
        Return _dao.Transform(dtos)
    End Function

    Public Function GetRejectionContainers(header As OChart) As IEnumerable(Of Integer)
        Dim result As IEnumerable(Of Integer) = header.Items.SelectMany(Function(i) i.Containers) _
                                                .Where(Function(c) c.IsRejection) _
                                                .Select(Function(c) c.Container.Id).Distinct()
        Return result
    End Function

    Public Function IsRejectionContainer(idContainer As Integer, header As OChart) As Boolean
        Dim available As IEnumerable(Of Integer) = Me.GetRejectionContainers(header)
        If available.IsNullOrEmpty() Then
            Return False
        End If
        Return available.Any(Function(c) c.Equals(idContainer))
    End Function

    Public Function IsRejectionContainer(idContainer As Integer) As Boolean
        Dim effective As OChart = Me.GetEffective()
        Return IsRejectionContainer(idContainer, effective)
    End Function
    Public Function IsRejectionContainer(container As Container) As Boolean
        Return Me.IsRejectionContainer(container.Id)
    End Function

    Public Function AddOChart(oChart As OChart) As OChart
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Dim step_i As String = "init"
        Try
            step_i = "BeginTransaction"
            unitOfWork.BeginTransaction()

            step_i = "GetPreviousOrDefault"
            Dim oChartPrevious As OChart = GetPreviousOrDefault(oChart, False)

            step_i = "Update oChartEffective"
            oChartPrevious.EndDate = oChart.StartDate.Value.BeginOfTheDay()
            UpdateWithoutTransaction(oChartPrevious)

            step_i = "cloning previous ochart"
            oChart.CloneItems(oChartPrevious, _userName)

            step_i = "Save ochart"
            SaveWithoutTransaction(oChart)

            step_i = "GetFollowingOrDefault"
            Dim oChartFollowing As OChart = GetFollowingOrDefault(oChart)
            If (oChartFollowing IsNot Nothing) Then
                oChart.EndDate = oChartFollowing.StartDate
                UpdateOnlyWithoutTransaction(oChart)
            End If
            unitOfWork.Commit()
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, String.Concat("OChart AddOChart - Errore Inserimento step '{0}'", step_i), ex)
            unitOfWork.Rollback()
            Throw New Exception(String.Concat("OChart AddOChart - Errore Inserimento step '{0}'. {1}", step_i, ex.Message))
        End Try

        Return oChart

    End Function


#End Region


End Class
