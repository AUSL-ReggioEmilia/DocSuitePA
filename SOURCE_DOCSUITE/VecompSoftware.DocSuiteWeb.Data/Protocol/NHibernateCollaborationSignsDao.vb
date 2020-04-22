Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports System.Data.SqlClient
Imports System.Linq
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class NHibernateCollaborationSignsDao
    Inherits BaseNHibernateDao(Of CollaborationSign)

#Region " Constructor "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    ''' <summary> Controlla che una collaborazione negli incrementali passati sia firmata dall'utente </summary>
    ''' <remarks>
    ''' select 1 from CollaborationSigns
    ''' where IdCollaboration=55
    ''' and Incremental in (1, 2, 3)
    ''' and SignUser='Caccin'
    ''' and not SignDate is null
    ''' </remarks>
    Public Function IsSigned(ByVal idCollaboration As Integer, ByVal incrementalList As List(Of Short), ByVal signUser As String) As Boolean
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetProjection(Projections.Constant("1"))
        criteria.Add(Restrictions.Eq("IdCollaboration", idCollaboration))

        If incrementalList IsNot Nothing Then
            criteria.Add(Restrictions.In("Incremental", incrementalList))
        End If

        criteria.Add(Restrictions.Eq("SignUser", signUser))
        criteria.Add(Restrictions.IsNotNull("SignDate"))
        Dim result As Object = criteria.UniqueResult()

        If result IsNot Nothing Then
            Return True
        End If

        Return False

    End Function

    Public Function GetByAccount(username As String) As IList(Of CollaborationSign)

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Like("SignUser", String.Format("\{0}", username), MatchMode.End))
        criteria.Add(Restrictions.Eq("IsActive", 1S))
        criteria.Add(Restrictions.IsNull("SignDate"))

        Return criteria.List(Of CollaborationSign)()
    End Function

    Public Function GetSigns(idCollaborations As Integer()) As IList(Of CollaborationSign)
        criteria = NHibernateSession.CreateCriteria(Of CollaborationSign)()
        criteria.Add(Restrictions.In("IdCollaboration", idCollaborations))
        criteria.AddOrder(Order.Asc("IdCollaboration"))
        criteria.AddOrder(Order.Asc("Incremental"))

        Return criteria.List(Of CollaborationSign)()
    End Function

    Public Function SearchFull(ByVal idCollaboration As Integer, ByVal isActive As Boolean, ByVal incremental As Short) As IList(Of CollaborationSign)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("IdCollaboration", idCollaboration))

        If isActive Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If

        If incremental <> 0 Then
            criteria.Add(Restrictions.Eq("Incremental", CShort(incremental)))
        End If
        criteria.AddOrder(Order.Asc("IdCollaboration"))
        criteria.AddOrder(Order.Asc("Incremental"))

        Return criteria.List(Of CollaborationSign)()

    End Function

    Public Function GetNextIncremental(ByVal idCollaboration As Integer) As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdCollaboration", idCollaboration))
        criteria.SetProjection(Projections.Max("Incremental"))
        Dim retValue As Object = criteria.UniqueResult()
        If retValue Is Nothing Then
            Return 0
        End If
        Return DirectCast(retValue, Short) + 1S
    End Function

    Public Function GetNext(idCollaboration As Integer) As IList(Of CollaborationSign)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of CollaborationSign)("CSS")
        dc.Add(Restrictions.Eq("CSS.IsActive", 1S))
        dc.Add(Restrictions.EqProperty("CSS.IdCollaboration", "CSM.IdCollaboration"))
        dc.SetProjection(Projections.Property("CSS.Incremental"))

        criteria = NHibernateSession.CreateCriteria(Of CollaborationSign)("CSM")
        criteria.Add(Restrictions.Eq("CSM.IdCollaboration", idCollaboration))
        criteria.Add(Restrictions.Disjunction().Add(Restrictions.Or(Restrictions.IsNull("CSM.IsAbsent"), Restrictions.Eq("CSM.IsAbsent", False))))
        criteria.Add(Subqueries.PropertyGt("CSM.Incremental", dc))

        Return criteria.List(Of CollaborationSign)()
    End Function

    Public Function GetPrev(idCollaboration As Integer) As IList(Of CollaborationSign)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of CollaborationSign)("CSS")
        dc.Add(Restrictions.Eq("CSS.IsActive", 1S))
        dc.Add(Restrictions.EqProperty("CSS.IdCollaboration", "CSM.IdCollaboration"))
        dc.SetProjection(Projections.Property("CSS.Incremental"))

        criteria = NHibernateSession.CreateCriteria(Of CollaborationSign)("CSM")
        criteria.Add(Restrictions.Eq("CSM.IdCollaboration", idCollaboration))
        criteria.Add(Subqueries.PropertyLt("CSM.Incremental", dc))

        Return criteria.List(Of CollaborationSign)()
    End Function

    ''' <summary> Lista di CollaborationSigns per una collaborazione. </summary>
    Function GetByIdCollaboration(ByVal idCollaboration As Integer) As IList(Of CollaborationSign)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of CollaborationSign)()
        criteria.Add(Restrictions.Eq("IdCollaboration", idCollaboration))
        criteria.AddOrder(Order.Asc("Incremental"))
        criteria.AddOrder(Order.Asc("RegistrationDate"))
        criteria.AddOrder(Order.Asc("IsActive"))

        Return criteria.List(Of CollaborationSign)()
    End Function

    ''' <summary> Ritorna la lista delle firme di una Collaborazione dati idCollaboration, signUser e isActive </summary>
    Function GetCollaborationSignsBy(ByVal idCollaboration As Integer, ByVal signUser As String, ByVal isActive As Short) As IList(Of CollaborationSign)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdCollaboration", idCollaboration))
        criteria.Add(Restrictions.Eq("SignUser", signUser))
        criteria.Add(Restrictions.Eq("IsActive", isActive))

        Return criteria.List(Of CollaborationSign)()
    End Function

    ''' <summary> Ritorna la lista delle firme di una Collaborazione dati idCollaboration e isActive </summary>
    Function GetCollaborationSignsBy(ByVal idCollaboration As Integer, ByVal isActive As Short) As IList(Of CollaborationSign)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdCollaboration", idCollaboration))
        criteria.Add(Restrictions.Eq("IsActive", isActive))

        Return criteria.List(Of CollaborationSign)()
    End Function

    ''' <summary>
    ''' Ritorna la lista dei firmatari di una determinata collaborazione con Incremental maggior o uguale a quello del firmatario attivo.
    ''' </summary>
    Function GetCollaborationSignsByGeActiveIncremental(ByVal idCollaboration As Integer) As IList(Of CollaborationSign)
        Dim detachedActiveIncremental As DetachedCriteria = DetachedCriteria.For(Of CollaborationSign)("SCS")
        With detachedActiveIncremental
            .SetMaxResults(1)
            .Add(Restrictions.EqProperty("SCS.IdCollaboration", "CS.IdCollaboration"))
            .Add(Restrictions.Eq("SCS.IsActive", 1S))
            .SetProjection(Projections.Property("SCS.Incremental"))
        End With

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of CollaborationSign)("CS")
        With criteria
            .Add(Restrictions.Eq("CS.IdCollaboration", idCollaboration))
            .Add(Subqueries.PropertyGe("CS.Incremental", detachedActiveIncremental))
            .AddOrder(Order.Asc("CS.Incremental"))
        End With
        Return criteria.List(Of CollaborationSign)()
    End Function

    ''' <summary> Verifica se il firmatario attivo ha firmato la collaborazione </summary>
    ''' <param name="idCollaboration"> identificativo della collaborazione </param>
    ''' <returns> True se la collaborazione è stata firmata dal firmatario attivo, False altrimenti </returns>
    Function IsCollaborationSignedByActiveSigner(ByVal idCollaboration As Integer) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdCollaboration", idCollaboration))
        criteria.Add(Restrictions.Eq("IsActive", Convert.ToInt16(1)))
        criteria.SetProjection(Projections.Property("SignDate"))
        Dim result As Date? = criteria.UniqueResult(Of Date?)
        Return result.HasValue
    End Function

    Public Function DisableActiveRequiredSigns(idCollaboration As Integer) As List(Of CollaborationSign)
        Dim signs As IList(Of CollaborationSign) = GetByIdCollaboration(idCollaboration)
        Dim requiresEditing As List(Of CollaborationSign) = signs.Where(Function(s) s.IsActive = 1S AndAlso s.IsRequired.GetValueOrDefault()).ToList()
        If requiresEditing.IsNullOrEmpty() Then
            Return New List(Of CollaborationSign)
        End If

        For Each item As CollaborationSign In requiresEditing
            item.IsRequired = False
            Update(item)
        Next
        Return requiresEditing
    End Function

    Public Function GetEffectiveSigners(idCollaboration As Integer) As ICollection(Of CollaborationSign)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdCollaboration", idCollaboration))
        criteria.Add(Restrictions.IsNotNull("SignDate"))
        criteria.AddOrder(Order.Asc("IdCollaboration"))
        criteria.AddOrder(Order.Asc("Incremental"))

        Return criteria.List(Of CollaborationSign)()
    End Function

End Class
