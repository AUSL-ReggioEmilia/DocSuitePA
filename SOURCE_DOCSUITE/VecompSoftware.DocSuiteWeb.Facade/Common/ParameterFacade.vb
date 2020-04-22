Imports System
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging

<ComponentModel.DataObject()> _
Public Class ParameterFacade
    Inherits CommonFacade(Of Parameter, Integer, NHibernateParameterDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

#End Region

    ''' <summary> Restuisce la riga della tabella Parameter corrente </summary>
    ''' <returns>Oggetto Parameter che identifica la riga corrente memorizzata su database</returns>
    Public Function GetCurrent() As Parameter
        Dim temp As Parameter = _dao.GetCurrent()
        'UpdateOnly(temp)
        Return temp
    End Function

    ''' <summary> Restuisce la riga della tabella Parameter corrente forzando l'aggiornamento da DB </summary>
    ''' <returns>Oggetto Parameter che identifica la riga corrente memorizzata su database</returns>
    Public Function GetCurrentAndRefresh() As Parameter
        Return _dao.GetCurrentAndRefresh()
    End Function
    
    ''' <summary> Incrementa il valore del cambo LastUsedNumber su database e restituisce la riga aggiornata </summary>
    ''' <returns>Oggetto Parameter aggiornato</returns>
    Public Function GetYearNumber() As YearNumberCompositeKey
        _dao.ConnectionName = "DocmDB"

        Dim _parameter As Parameter = GetCurrent()
        Dim yearNumberKey As New YearNumberCompositeKey(_parameter.LastUsedYear, _parameter.LastUsedNumber)

        _parameter.LastUsedNumber = _parameter.LastUsedNumber + 1
        _parameter.Locked = False
        Try
            Update(_parameter)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore aggiornamento parametro", ex)
            Return Nothing
        End Try

        Return yearNumberKey
    End Function

    ''' <summary> Incrementa il valore del cambo LastUsedidResolution su database e restituisce la riga aggiornata </summary>
    ''' <returns> Oggetto aggiornato, -1 in caso d'errore </returns>
    Public Function GetIdresolution() As Integer
        Dim idResolution As Integer
        _dao.ConnectionName = "ReslDB"
        Dim _parameter As Parameter = GetCurrent()
        If Not _parameter Is Nothing Then
            idResolution = _parameter.LastUsedIdResolution
            _parameter.LastUsedIdResolution = _parameter.LastUsedIdResolution + 1
            _parameter.Locked = False
        Else
            _parameter = New Parameter()
            idResolution = _parameter.LastUsedIdResolution
            _parameter.LastUsedIdResolution = _parameter.LastUsedIdResolution + 1
        End If
        Try
            Update(_parameter)
        Catch ex As Exception
            idResolution = -1
        End Try

        Return idResolution
    End Function

    ''' <summary> Incrementa il valore del cambo LastUsedResolutionYear su database e restituisce la riga aggiornata </summary>
    ''' <returns>Oggetto Parameter aggiornato</returns>
    Public Function GetLastUsedResolutionYear() As Parameter
        _dao.ConnectionName = "ReslDB"
        Dim _parameter As Parameter = GetCurrent()
        If Not _parameter Is Nothing Then
            _parameter.LastUsedResolutionYear = _parameter.LastUsedResolutionYear + 1S
            _parameter.Locked = False
        Else
            _parameter = New Parameter()
            _parameter.LastUsedResolutionYear = _parameter.LastUsedResolutionYear + 1S

        End If
        
        Try
            Update(_parameter)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore aggiornamento parametro", ex)
            Return Nothing
        End Try

        Return _parameter
    End Function

    ''' <summary> Incrementa il valore del cambo LastUsedResolutionNumber su database e restituisce la riga aggiornata </summary>
    ''' <returns>Oggetto Parameter aggiornato</returns>
    Public Function GetLastUsedResolutionNumber() As Parameter
        _dao.ConnectionName = "ReslDB"
        Dim _parameter As Parameter = GetCurrent()
        If Not _parameter Is Nothing Then
            _parameter.LastUsedResolutionNumber = _parameter.LastUsedResolutionNumber + 1S
            _parameter.Locked = False
        Else
            _parameter = New Parameter()
            _parameter.LastUsedResolutionNumber = _parameter.LastUsedResolutionNumber + 1S

        End If

        Try
            Update(_parameter)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore aggiornamento parametro", ex)
            Return Nothing
        End Try

        Return _parameter
    End Function

    ''' <summary> Incrementa il valore del cambo LastUsedBillNumber su database e restituisce la riga aggiornata </summary>
    ''' <returns>Oggetto Parameter aggiornato</returns>
    Public Function GetLastUsedBillNumber() As Parameter
        _dao.ConnectionName = "ReslDB"
        Dim _parameter As Parameter = GetCurrent()
        If Not _parameter Is Nothing Then
            _parameter.LastUsedBillNumber = _parameter.LastUsedBillNumber + 1S
            _parameter.Locked = False
        Else
            _parameter = New Parameter()
            _parameter.LastUsedBillNumber = _parameter.LastUsedBillNumber + 1S

        End If

        Try
            Update(_parameter)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            Return Nothing
        End Try

        Return _parameter
    End Function

    Public Sub UpdateSingleInstance(ByRef parameter As Parameter, ByVal DBName As String)
        _dao.ConnectionName = DBName
        _dao.UpdateOnly(parameter)
    End Sub
    Public Function UpdateReplicateLastIdRoleUser(ByVal newId As Integer, ByVal oldId As Integer) As Boolean
        Dim query As String = String.Format("UPDATE Parameter SET LastUsedIdRoleUser={0} WHERE LastUsedIdRoleUser= {1}", newId, oldId)
        Return ReplicateQuery(query)
    End Function

    Public Function UpdateReplicateLastIdRole(ByVal newId As Integer, ByVal oldId As Integer) As Boolean
        Dim query As String = String.Format("UPDATE Parameter SET LastUsedIdRole={0} WHERE LastUsedIdRole= {1}", newId, oldId)
        Return ReplicateQuery(query)
    End Function

    Public Function UpdateReplicateLastIdCategory(ByVal newId As Integer, ByVal oldId As Integer) As Boolean
        Dim query As String = String.Format("UPDATE Parameter SET LastUsedIdCategory={0} WHERE LastUsedIdCategory= {1}", newId, oldId)
        Return ReplicateQuery(query)
    End Function

    Public Function UpdateReplicateLastIdContainer(ByVal newId As Integer, ByVal oldId As Integer) As Boolean
        Dim query As String = String.Format("UPDATE Parameter SET LastUsedIdContainer={0} WHERE LastUsedIdContainer= {1}", newId, oldId)
        Return ReplicateQuery(query)
    End Function

    Public Sub UpdateLastUsedNumber(ByVal year As Int32)
        _dao.UpdateLastUsedNumber(year)
    End Sub

    Public Function ReplicateQuery(ByVal query As String) As Boolean
        Try
            Dim command As IDbCommand = New SqlClient.SqlCommand()
            If DocSuiteContext.Current.IsProtocolEnabled Then
                command.Connection = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").Connection
                command.CommandText = query
                command.ExecuteNonQuery()
            End If

            If DocSuiteContext.Current.IsDocumentEnabled Then
                command.Connection = NHibernateSessionManager.Instance.GetSessionFrom("DocmDB").Connection
                command.CommandText = query
                command.ExecuteNonQuery()
            End If

            If DocSuiteContext.Current.IsResolutionEnabled Then
                command.Connection = NHibernateSessionManager.Instance.GetSessionFrom("ReslDB").Connection
                command.CommandText = query
                command.ExecuteNonQuery()
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub UpdateLastIdRoleWithoutTransaction(idRole As Integer, oldIdRole As Integer)
        _dao.ConnectionName = ProtDB
        _dao.UpdateLastIdRole(idRole, oldIdRole)
        If DocSuiteContext.Current.IsResolutionEnabled Then
            _dao.ConnectionName = ReslDB
            _dao.UpdateLastIdRole(idRole, oldIdRole)
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            _dao.ConnectionName = DocmDB
            _dao.UpdateLastIdRole(idRole, oldIdRole)
        End If
        _dao.ConnectionName = ProtDB
    End Sub

    Public Sub UpdateLastIdRoleUserWithoutTransaction(idRoleUser As Integer, oldIdRoleUser As Integer)
        _dao.ConnectionName = ProtDB
        _dao.UpdateLastIdRoleUser(idRoleUser, oldIdRoleUser)
        If DocSuiteContext.Current.IsResolutionEnabled Then
            _dao.ConnectionName = ReslDB
            _dao.UpdateLastIdRoleUser(idRoleUser, oldIdRoleUser)
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            _dao.ConnectionName = DocmDB
            _dao.UpdateLastIdRoleUser(idRoleUser, oldIdRoleUser)
        End If
        _dao.ConnectionName = ProtDB
    End Sub

    Public Function GetLastUsedIdRole() As Short
        _dao.ConnectionName = ProtDB
        Return _dao.GetLastUsedIdRole()
    End Function

    Public Function GetLastUsedIdRoleUser() As Short
        _dao.ConnectionName = ProtDB
        Return _dao.GetLastUsedIdRoleUser()
    End Function
End Class
