Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()>
Public Class NHibernateProtocolLogFinder
    Inherits NHibernateBaseLogFinder(Of ProtocolLog)

#Region " Fields "

    Private _protYear As Short
    Private _protNumber As Integer

#End Region

#Region " Properties "

    ''' <summary> Sessione nhibernate corrente. </summary>
    Protected Overloads ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    ''' <summary> Anno del protocollo da ricercare. </summary>
    Public Property ProtocolYear As Short

    ''' <summary> Numero del protocollo da ricercare. </summary>
    Public Property ProtocolNumber As Integer

#End Region

#Region " Constructor "

    Public Sub New()
        SessionFactoryName = "ProtDB"
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Aggiunta dei filtri anno e numero ai criteri di ricerca comuni. </summary>
    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = MyBase.CreateCriteria()

        If ProtocolYear <> 0 Then
            criteria.Add(Restrictions.Eq("D.Year", ProtocolYear))
        End If

        If ProtocolNumber <> 0 Then
            criteria.Add(Restrictions.Eq("D.Number", ProtocolNumber))
        End If

        If Not String.IsNullOrEmpty(LogType) Then
            criteria.Add(Restrictions.Like("LogType", LogType))
        Else
            criteria.Add(Restrictions.Not(Restrictions.Eq("LogType", "SB")))
        End If

        Return criteria

    End Function

    ''' <summary> Ricerca senza limiti di paginazione. </summary>
    Public Overrides Function DoSearch() As IList(Of ProtocolLog)
        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "LogDate", SortOrder.Descending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Dim projList As ProjectionList = Projections.ProjectionList()
        projList.Add(Projections.Property("D.Id"), "Id")
        projList.Add(Projections.Property("D.LogDate"), "LogDate")
        projList.Add(Projections.Property("D.SystemComputer"), "SystemComputer")
        projList.Add(Projections.Property("D.SystemUser"), "SystemUser")
        projList.Add(Projections.Property("D.Program"), "Program")
        projList.Add(Projections.Property("D.LogType"), "LogType")
        projList.Add(Projections.Property("D.LogDescription"), "LogDescription")
        projList.Add(Projections.Property("D.Year"), "Year")
        projList.Add(Projections.Property("D.Number"), "Number")

        criteria.SetProjection(projList)
        criteria.SetResultTransformer(New Transform.AliasToBeanResultTransformer(GetType(ProtocolLog)))
        Return criteria.List(Of ProtocolLog)()
    End Function

    ''' <summary> Ricerca con ordinamento. </summary>
    ''' <param name="sortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    Public Overloads Function DoSearch(ByVal sortExpr As String) As IList(Of ProtocolLog)
        MyBase.DoSearch(sortExpr)
        Return DoSearch()
    End Function

    ''' <summary>  Ricerca con ordinamento, riga di partenza e dimensione della paginazione. </summary>
    ''' <param name="sortExpr">Una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <param name="startRow">La riga a partire dalla quale tornare il risultato</param>
    ''' <param name="PageSize">La dimensione della pagina dei risultati</param>
    ''' <returns>Una lista ordinata di risultati limitati a PageSize</returns>
    Public Overloads Function DoSearch(ByVal sortExpr As String, ByVal startRow As Integer, ByVal pageSize As Integer) As IList(Of ProtocolLog)
        MyBase.DoSearch(sortExpr, startRow, pageSize)
        Return DoSearch()
    End Function

#End Region

End Class
