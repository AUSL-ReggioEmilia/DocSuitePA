Imports System
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateDocumentObjectDao
    Inherits BaseNHibernateDao(Of DocumentObject)


    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Ritorna il max Id del documentObject dati year e number </summary>
    Public Function GetMaxId(ByVal year As Short, ByVal number As Integer) As Short
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        criteria.SetProjection(Projections.ProjectionList.Add(Projections.Max("Id.Incremental")))

        Return criteria.UniqueResult(Of Short)() + 1S
    End Function

    ''' <summary>
    ''' Seleziono tutti i documenti Attivi. Diversi da idObjectStatus -->"A"
    ''' </summary>
    ''' <param name="YearNumberInc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FolderHasDocumentObjectActive(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "D")

        criteria.CreateAlias("D.DocumentVersionings", "DocumentVersioning", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("D.id.Year", YearNumberInc.Year))
        criteria.Add(Restrictions.Eq("D.id.Number", YearNumberInc.Number))
        criteria.Add(Restrictions.Eq("D.IncrementalFolder", YearNumberInc.Incremental))

        Dim condiction As Disjunction = Expression.Disjunction()
        condiction.Add(Expression.Not(Restrictions.Eq("D.idObjectStatus", "A")))
        condiction.Add(Restrictions.IsNull("D.idObjectStatus"))
        criteria.Add(condiction)
        Return criteria.List().Count > 0

    End Function

    ''' <summary>
    ''' Ritorna il numero di documenti associati ad un folder
    ''' </summary>
    ''' <param name="YearNumberInc">Parametro di tipo YearNumberIncrCompositeKey</param>
    ''' <returns>il numero dei documenti</returns>
    Public Function FolderDocumentObjectCount(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.Year", YearNumberInc.Year))
        criteria.Add(Restrictions.Eq("Id.Number", YearNumberInc.Number))
        criteria.Add(Restrictions.Eq("ValidIncremental", YearNumberInc.Incremental))
        criteria.SetProjection(Projections.RowCount())

        Return criteria.UniqueResult(Of Integer)()
    End Function

    ''' <summary> Lista di documenti con i versioning </summary>
    ''' <param name="id"> Chiave </param>
    Public Function GetObjectsWithVersioning(ByVal id As YearNumberIncrCompositeKey, ByVal listAllDocs As Boolean) As IList(Of DocumentObject)
        NHibernateSession.EnableFilter("Status").SetParameter("CheckStatus", "A")

        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("DocumentVersionings", "DV", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("id.Year", id.Year))
        criteria.Add(Restrictions.Eq("id.Number", id.Number))

        criteria.Add(Restrictions.IsNull("ValidIncremental"))

        If id.Incremental.HasValue Then
            criteria.Add(Restrictions.Eq("IncrementalFolder", id.Incremental))
            If listAllDocs Then
                criteria.AddOrder(Order.Asc("id.Incremental"))
            Else
                criteria.AddOrder(Order.Asc("OrdinalPosition"))
            End If
        Else
            criteria.AddOrder(Order.Asc("DocStep"))
            criteria.AddOrder(Order.Asc("SubStep"))
        End If

        Return criteria.List(Of DocumentObject)()
    End Function


    ''' <summary>
    ''' Dato un documentObject, ritorna quello immediatamente precedente o successivo 
    ''' </summary>
    ''' <param name="SwapPosition">Indica se ritornare il nodo precedente (MOVEUP) o il successivo (MOVEDOWN) </param>
    ''' <param name="YearNumberInc">L'id del nodo da cercare</param>
    ''' <param name="OrdinalPosition">L'ordinal position del nodo di partenza</param>
    Function GetNextDocObjectPosition(ByVal swapPosition As String, ByVal yearNumberInc As YearNumberIncrCompositeKey, ByVal ordinalPosition As Short) As DocumentObject
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("id.Year", yearNumberInc.Year))
        criteria.Add(Restrictions.Eq("id.Number", yearNumberInc.Number))
        criteria.Add(Restrictions.Eq("IncrementalFolder", yearNumberInc.Incremental))
        criteria.Add(Restrictions.IsNull("ValidIncremental"))

        Select Case swapPosition.ToUpper()
            Case "MOVEUP" 'up
                criteria.Add(Expression.Lt("OrdinalPosition", ordinalPosition))
                criteria.AddOrder(Order.Desc("OrdinalPosition"))
            Case "MOVEDOWN" 'down
                criteria.Add(Expression.Gt("OrdinalPosition", ordinalPosition))
                criteria.AddOrder(Order.Asc("OrdinalPosition"))
        End Select

        Return criteria.List(Of DocumentObject)()(0)
    End Function

    Function GetDocumentCountOfDocumentFolder(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Integer
        Dim count As Integer = 0
        Const sql As String = "SELECT  DO.IncrementalFolder, COUNT(*) AS Cont " & _
                              " FROM ( " & _
                              " SELECT Year,Number,IncrementalFolder " & _
                              " FROM DocumentObject " & _
                              " WHERE idObjectStatus <> @ObjStatus  OR idObjectStatus is NULL " & _
                              " GROUP BY [Year], Number, IncrementalFolder, OrdinalPosition) as DO " & _
                              " WHERE DO.Year = @Year AND DO.Number = @Number AND DO.IncrementalFolder = @IncrementalFolder" & _
                              " GROUP BY  DO.IncrementalFolder"
        Dim command As SqlClient.SqlCommand = New SqlClient.SqlCommand(sql, NHibernateSession.Connection)
        command.Parameters.AddWithValue("@ObjStatus", "A")
        command.Parameters.AddWithValue("@Year", YearNumberInc.Year)
        command.Parameters.AddWithValue("@Number", YearNumberInc.Number)
        command.Parameters.AddWithValue("@IncrementalFolder", YearNumberInc.Incremental)
        Dim reader As SqlClient.SqlDataReader = Nothing
        Try
            reader = command.ExecuteReader()
            If reader.HasRows Then
                reader.Read()
                count = reader.GetInt32(reader.GetOrdinal("Cont"))
            End If
        Catch ex As Exception
            Return count
        Finally
            reader.Close()
        End Try

        Return count
    End Function

    Function GetDocumentObjectLink(ByVal idObjectType As String, ByVal link As String) As IList(Of DocumentObject)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Document", "Do", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("idObjectType", idObjectType))
        criteria.Add(Expression.Like("Link", link & "%"))

        Return criteria.List(Of DocumentObject)()
    End Function

    Function GetVersionedDocumentObjects(ByVal year As Short, ByVal number As Integer, ByVal incremental As Short) As IList(Of DocumentObject)

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        criteria.Add(Expression.Or(Restrictions.Eq("Id.Incremental", incremental), Restrictions.Eq("ValidIncremental", incremental)))

        Return criteria.List(Of DocumentObject)()

    End Function

    Function DocumentObjectVerifyLink(ByVal Year As Short, ByVal Number As Integer, ByVal Link As String, Optional ByVal IncrementalFolder As Short = 0) As Boolean

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Year", Year))
        criteria.Add(Restrictions.Eq("Id.Number", Number))
        criteria.Add(Restrictions.Eq("Link", Link))
        If IncrementalFolder > 0 Then
            criteria.Add(Restrictions.Eq("IncrementalFolder", IncrementalFolder))
        End If

        Return (criteria.List(Of DocumentObject)().Count > 0)
    End Function

    Function CheckDocObjectCheckedOut(ByVal year As Short, ByVal number As Integer, ByVal incremental As Integer) As Boolean

        Dim command As IDbCommand = New SqlClient.SqlCommand()

        Const sqlQuery As String = "SELECT D.Year,D.Number,D.Incremental,V.CheckStatus,V.CheckOutUser,V.CheckDir " & _
                                   "FROM DocumentObject AS D " & _
                                   "LEFT JOIN DocumentVersioning AS V ON (D.Year = V.Year AND D.Number = V.Number AND D.Incremental = V.IncrementalObject) " & _
                                   "WHERE D.Year = @year AND D.Number = @number AND D.Incremental = @incremental"


        command.Connection = NHibernateSession.Connection
        command.CommandText = sqlQuery

        command.Parameters.Add(New SqlClient.SqlParameter("@year", year))
        command.Parameters.Add(New SqlClient.SqlParameter("@number", number))
        command.Parameters.Add(New SqlClient.SqlParameter("@incremental", incremental))

        Dim dt As DataTable = New DataTable("Roles")
        Dim cReader As IDataReader = command.ExecuteReader()
        dt.Load(cReader)

        Return dt.Rows.Count > 0

    End Function

End Class
