Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports NHibernate.Criterion
Imports NHibernate
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Helpers.NHibernate

Public Class NHResolutionFinder2

#Region " Properties "

    Public Property SessionFactoryName As String

    Public Property InSteps As IList(Of TabWorkflow)
    Public Property InStepActive As Boolean?
    Public Property InStepDateRange As Range(Of DateTime)
    
#End Region

#Region " Costructor "
    Public Sub New(ByVal dbName As String)
        SessionFactoryName = DbName
    End Sub
#End Region

#Region " Mothods "

    Public Sub ClearFields()
        InSteps = Nothing
        InStepActive = Nothing
        InStepDateRange = Nothing
    End Sub

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Function DoSearch() As IList(Of Resolution)
        Dim criteria As ICriteria = CreateCriteria()

        Return criteria.List(Of Resolution)()
    End Function

    Private Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Resolution)("RESL")

        DecorateCriteria(criteria)

        Return criteria
    End Function

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)

        DecorateInWorkflowStep(criteria)

    End Sub

    ''' <summary>
    ''' Aggiunge filtro per Step attivo.
    ''' </summary>
    Private Sub DecorateInWorkflowStep(ByRef criteria As ICriteria)

        If InSteps.IsNullOrEmpty Then
            Return
        End If

        Dim tor As Conjunction = Expression.Conjunction()

        ' Filtro i risultati per tipologi di WorkFlow
        criteria.CreateAlias("RESL.ResolutionWorkflows", "RWF")

        Dim disj As New Disjunction
        For Each item As TabWorkflow In InSteps

            Dim conj As New Conjunction
            conj.Add(Restrictions.Eq("RESL.WorkflowType", item.Id.WorkflowType))
            conj.Add(Restrictions.Eq("RWF.ResStep", item.Id.ResStep))
            If InStepDateRange.HasValue Then
                criteria.AddRestrictionsBetween(String.Format("RESL.{0}", item.FieldDate), InStepDateRange)
            End If

            disj.Add(conj)
        Next
        criteria.Add(disj)

        ' Filtro per cercare solo quelli che al momento hanno lo step attivo
        If InStepActive.GetValueOrDefault(False) Then
            tor.Add(Restrictions.Eq("RWF.IsActive", 1S))
        Else
            tor.Add(Restrictions.Not(Restrictions.Eq("RWF.IsActive", 2S)))
        End If
        
        criteria.Add(tor)
    End Sub


#End Region

End Class