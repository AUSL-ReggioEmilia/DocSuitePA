Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateResolutionLogFinder
    Inherits NHibernateBaseLogFinder(Of ResolutionLog)

#Region " New "

    Public Sub New()
        SessionFactoryName = "ReslDB"
    End Sub

#End Region

#Region " Private members "
    Private _id As Integer
#End Region

#Region " Properties "

    ''' <summary>
    ''' Id della risoluzione da ricercare
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns></returns>
    ''' <remarks>Integer</remarks>
    Public Overloads Property Id() As Integer
        Get
            Return MyBase.Id
        End Get
        Set(ByVal value As Integer)
            MyBase.Id = value
        End Set
    End Property

#End Region

#Region " Virtual method overrides"

    ''' <summary>
    ''' Aggiunta del filtro su Id ai criteri di ricerca comuni
    ''' </summary>
    ''' <returns>La lista dei criteria necessari per la ricerca in ResolutionLog</returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria
        Dim criteria As ICriteria = MyBase.CreateCriteria()

        If MyBase.Id <> 0 Then
            criteria.Add(Restrictions.Eq("D.IdResolution", MyBase.Id))
        End If

        If String.IsNullOrEmpty(LogType) Then
            criteria.Add(Restrictions.Not(Restrictions.Like("LogType", "SB")))
        End If

        Return criteria
    End Function

    ''' <summary>
    ''' Ricerca senza limiti di paginazione 
    ''' </summary>
    ''' <returns>La lista dei ResolutionLog che soddisfano i criteri di ricerca</returns>
    ''' <remarks></remarks>
    Public Overrides Function DoSearch() As System.Collections.Generic.IList(Of ResolutionLog)
        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "LogDate", SortOrder.Descending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of ResolutionLog)()
    End Function

#End Region

#Region " Virtual methods overload"
    ''' <summary>
    ''' Ricerca con ordinamento
    ''' </summary>
    ''' <param name="SortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <returns>Una lista ordinata di ResolutionLog </returns>
    Public Overloads Function DoSearch(ByVal SortExpr As String) As IList(Of ResolutionLog)
        MyBase.DoSearch(SortExpr)
        Return DoSearch()
    End Function

    ''' <summary>
    '''  Ricerca con ordinamento, riga di partenza e dimensione della paginazione 
    ''' </summary>
    ''' <param name="SortExpr">Una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <param name="StartRow">La riga a partire dalla quale tornare il risultato</param>
    ''' <param name="PageSize">La dimensione della pagina dei risultati</param>
    ''' <returns>Una lista ordinata di risultati limitati a PageSize</returns>
    Public Overloads Function DoSearch(ByVal sortExpr As String, ByVal startRow As Integer, ByVal pageSize As Integer) As IList(Of TableLog)
        MyBase.DoSearch(sortExpr, startRow, pageSize)
        Return DoSearch()
    End Function
#End Region

#Region " NHibernate Properties"
    ''' <summary>
    ''' Ritorna la sessione hibernate corrente
    ''' </summary>
    ''' <returns>ISession</returns>
    ''' <remarks>ReadOnly</remarks>
    Protected Overloads ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

End Class
