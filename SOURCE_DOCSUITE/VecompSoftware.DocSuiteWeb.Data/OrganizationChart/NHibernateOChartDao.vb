Imports System.Linq
Imports NHibernate
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateOChartDao
    Inherits BaseNHibernateDao(Of OChart)

#Region " Constants "

#End Region

#Region " Methods "

    Public Function GetPrevious(oChart As OChart) As OChart
        Dim finder As New NHibernateOChartFinder()
        finder.IsEnabled = True
        finder.PreviousOf = oChart.StartDate
        Return finder.DoSearch().SingleOrDefault()
    End Function

    Public Function GetFollowing(oChart As OChart) As OChart
        Dim finder As New NHibernateOChartFinder()
        finder.IsEnabled = True
        finder.FollowingOf = oChart.StartDate
        Return finder.DoSearch().FirstOrDefault(Function(f) f.Id <> oChart.Id)
    End Function

    Public Function GetFollowings(oChart As OChart) As IList(Of OChart)
        Dim finder As New NHibernateOChartFinder()
        finder.IsEnabled = True
        finder.FollowingOf = oChart.StartDate
        Return finder.DoSearch().Where(Function(f) f.Id <> oChart.Id).ToList()
    End Function

    Public Function GetHeader(id As Guid) As OChart
        Dim finder As New NHibernateOChartFinder()
        finder.IdIn = New List(Of Guid) From {id}
        Return finder.DoSearch().FirstOrDefault()
    End Function

    Public Function GetHierarchy(id As Guid) As OChart
        Dim finder As New NHibernateOChartFinder()
        finder.IdIn = New List(Of Guid) From {id}
        Return finder.ListHierarchies().SingleOrDefault()
    End Function

    Public Function Transform(dtos As IEnumerable(Of OChartTransformerDTO)) As List(Of OChart)
        Dim session As ISession = NHibernateSession()

        Dim valid As IEnumerable(Of OChartTransformerDTO) = dtos.Where(Function(t) t.Validate())
        Dim expected As IEnumerable(Of OChart) = valid.Where(Function(c) c.RequiresReplication).GroupBy(Function(g) g.ReferenceDateOrDefault, Function(k, g) g.First()).Select(Function(f) New OChart(f))
        Dim finder As New NHibernateOChartFinder() With {.IsEnabled = True, .EffectivenessLowerBound = True}
        Dim hierarchies As IEnumerable(Of OChart) = finder.ListHierarchies(session)
        Dim missing As IEnumerable(Of OChart) = expected.Where(Function(e) Not hierarchies.Any(Function(a) a.IsSame(e)))
        Dim replicated As IEnumerable(Of OChart) = missing.Select(Function(m) m.CloneItems(hierarchies.PreviousOrDefault(m), DocSuiteContext.Current.User.FullUserName))

        Using transaction As ITransaction = session.BeginTransaction(IsolationLevel.ReadCommitted)
            Dim list As List(Of OChart) = hierarchies.ToList()
            If Not replicated.IsNullOrEmpty() Then
                list.AddRange(replicated)
            End If

            For Each header As OChart In list.OrderChronologically()
                Dim iterations As Integer = 5
                If dtos.HasSingle Then
                    iterations = 0
                End If

                For iteration As Integer = 0 To iterations
                    For Each dto As OChartTransformerDTO In valid.Where(Function(i) i.ComplyHeader(header)).OrderByPriority()
                        header.Transform(dto)
                    Next
                Next
                If valid.Any(Function(d) d.ComplyHeader(header)) Then
                    session.SaveOrUpdate(header)
                End If
            Next
            transaction.Commit()
            list.ForEach(Sub(t) t.RebuildFlattened())
            Return list
        End Using

    End Function

#End Region

End Class
