Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateContactFinder
    Inherits NHibernateBaseFinder(Of Contact, Contact)

#Region " Fields "

    Private _description As String = String.Empty
    Private _descriptionContain As Boolean = False

#End Region
    
#Region " Properties "

    Property Description() As String
        Get
            Return _description.Replace("_", " ")
        End Get
        Set(ByVal value As String)
            _description = value.Replace(" ", "_")
        End Set
    End Property

    Property DescriptionContain() As Boolean
        Get
            Return _descriptionContain
        End Get
        Set(value As Boolean)
            _descriptionContain = value
        End Set
    End Property

    ' Mettere a true se si vuole abilitare la ricerca anche all'interno dei figli.
    Property SearchDescendantOf() As Boolean

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

#Region " Constructors "

    Public Sub New(ByVal dbName As String)
        SessionFactoryName = dbName
    End Sub

#End Region

#Region " Methods "

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "C")
        ' Se il criterio di ricerca dei contatti prevedere la ricerca anche nei contatti figli il filtro corretto viene eseguito nella DoSearch()
        If Not SearchDescendantOf Then
            Dim recipientMatchMode As MatchMode = If(DescriptionContain, MatchMode.Anywhere, MatchMode.Start)

            If Not String.IsNullOrEmpty(Description) Then
                criteria.Add(Restrictions.Like("Description", Description, recipientMatchMode))
            End If

        End If
        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of Contact)
        If Not SearchDescendantOf Then
            Dim criteria As ICriteria = CreateCriteria()
            Return criteria.List(Of Contact)()
        Else
            ' Se il criterio di ricerca dei contatti prevedere la ricerca anche nei contatti figli il filtro corretto viene eseguito nella DoSearch()
            ' Vado a individuare l'attuale FullIncrementalPath e in seguito ricerco all'interno dei figli (oggetti che hanno la stesso FullIncrementalPath iniziale)
            Dim sql As String = "select {c.*} " _
            & "FROM Contact {c} WITH (NOLOCK) " _
            & "inner join ( " _
                            & "SELECT CAST(FullIncrementalPath + '%' AS VARCHAR(512)) As FullIncrementalPath, Incremental " _
                            & "FROM Contact WITH (NOLOCK) " _
                            & "WHERE Description like '%" + Description + "%') " _
            & "Search ON c.FullIncrementalPath like Search.FullIncrementalPath"

            Dim query As ISQLQuery = NHibernateSession.CreateSQLQuery(sql).AddEntity("c", New Contact().GetType())
            Return query.List(Of Contact)()
        End If
        
    End Function

#End Region

End Class
