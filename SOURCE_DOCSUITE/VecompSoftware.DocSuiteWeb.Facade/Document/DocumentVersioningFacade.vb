Imports System
Imports System.Data.SqlClient
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class DocumentVersioningFacade
    Inherits BaseDocumentFacade(Of DocumentVersioning, YearNumberIncrCompositeKey, NHibernateDocumentVersioningDao)

    Public Sub New()
        MyBase.New()
    End Sub


    Function GetDocumentVersion(ByVal year As Short, ByVal number As Integer, ByVal incrementalObject As Short, ByVal checkStatus As String) As DocumentVersioning
        Return _dao.GetDocumentVersion(year, number, incrementalObject, checkStatus)
    End Function

    Function GetDocumentVersionCount(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Integer
        Return _dao.GetDocumentVersionCount(YearNumberInc)
    End Function

    ''' <summary>
    ''' Controlla quante righe di Versioning sono associate ad uno specifico documentObject 
    ''' </summary>
    ''' <param name="YearNumberInc">Chaive per identificare univocamente il documentObject</param>
    ''' <returns>Il numero dei versioning relativia al documento</returns>
    Function GetVersioningForDocumentObject(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Integer
        Return _dao.GetVersioningForDocumentObject(YearNumberInc)
    End Function

    Public Function GetDocumentVersionAll(ByVal year As Short, ByVal number As Integer, ByVal incremental As Short, ByVal checkStatus As String) As IList(Of DocumentVersioning)
        Return _dao.GetVersioningAll(year, number, incremental, checkStatus)
    End Function

    Public Function DocumentVersioningSearch(ByVal YearNumberIncr As YearNumberIncrCompositeKey, ByVal CheckStatus As String) As IList(Of DocumentVersioning)
        Return _dao.DocumentVersioningSearch(YearNumberIncr, CheckStatus)
    End Function

    Public Function GetMaxId(ByVal year As Short, ByVal number As Integer) As Short
        Return _dao.GetMaxId(year, number)
    End Function

    Function DocumentVersionings(ByVal year As Short, ByVal number As Integer, ByVal incrementalObject As Short) As IList(Of DocumentVersioning)
        Return _dao.DocumentVersionings(year, number, incrementalObject)
    End Function

#Region "Update CheckIn"
    Function UpdateCheckIn(ByVal Year As Short, ByVal Number As Integer, ByVal IncObjectCheckOut As Integer, ByVal IncrObjectCheckIn As Integer) As Boolean
        Dim session As NHibernate.ISession = NHibernateSessionManager.Instance.GetSessionFrom("DocmDB")
        Dim transaction As NHibernate.ITransaction = session.BeginTransaction(IsolationLevel.ReadCommitted)
        Try
            Dim sqlCommand As String = "UPDATE DocumentVersioning SET CheckinUser=@CheckinUser, CheckInDate=@CheckInDate, CheckStatus=@CheckStatus " & _
                                        "WHERE Year=@Year AND Number=@Number " & _
                                        "AND IncrementalObject=@IncrementalObject " & _
                                        "AND CheckInDate IS NULL "
            Dim cmd As SqlCommand = New SqlCommand(sqlCommand, session.Connection)
            cmd.Parameters.Add(New SqlParameter("@CheckinUser", DocSuiteContext.Current.User.FullUserName))
            cmd.Parameters.Add(New SqlParameter("@CheckInDate", _dao.GetServerDate()))
            cmd.Parameters.Add(New SqlParameter("@CheckStatus", "I"))
            cmd.Parameters.Add(New SqlParameter("@Year", Year))
            cmd.Parameters.Add(New SqlParameter("@Number", Number))
            cmd.Parameters.Add(New SqlParameter("@IncrementalObject", IncObjectCheckOut))
            transaction.Enlist(cmd)
            cmd.ExecuteNonQuery()

            sqlCommand = "UPDATE DocumentObject SET ValidIncremental=@ValidIncremental " & _
                         "WHERE Year=@Year AND Number=@Number " & _
                         "AND ((ValidIncremental=@IncrementalObject) OR (ValidIncremental IS NULL AND Incremental=@IncrementalObject))"
            cmd = New SqlCommand(sqlCommand, session.Connection)
            cmd.Parameters.Add(New SqlParameter("@ValidIncremental", IncrObjectCheckIn))
            cmd.Parameters.Add(New SqlParameter("@Year", Year))
            cmd.Parameters.Add(New SqlParameter("@Number", Number))
            cmd.Parameters.Add(New SqlParameter("@IncrementalObject", IncObjectCheckOut))
            transaction.Enlist(cmd)
            cmd.ExecuteNonQuery()

            transaction.Commit()
        Catch ex As Exception
            transaction.Rollback()
            Return False
        End Try

        Return True
    End Function
#End Region
End Class