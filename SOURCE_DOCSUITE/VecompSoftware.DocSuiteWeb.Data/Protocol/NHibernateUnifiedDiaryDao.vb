Imports NHibernate
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateUnifiedDiaryDao
    Inherits BaseNHibernateDao(Of UnifiedDiary)

#Region "Constructor"
    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub
#End Region

#Region "Methods"
    Public Function GetLastLogsEntities(ByVal type As Integer?, ByVal fromDate As Date, ByVal toDate As Date, ByVal userName As String, Optional subject As String = "") As IList(Of UnifiedDiary)
        Dim result As IList(Of UnifiedDiary)
        Dim query As IQuery = NHibernateSession.GetNamedQuery("GetUnifiedDiary")

        'set parameters
        query.SetParameter(Of Integer?)("IdTipologia", Nothing)
        If type.HasValue Then
            query.SetInt32("IdTipologia", type.Value)
        End If

        query.SetDateTime("DataDal", fromDate)
        query.SetDateTime("DataAl", toDate)
        query.SetAnsiString("User", userName)
        query.SetParameter(Of String)("Subject", Nothing)

        If Not String.IsNullOrEmpty(subject) Then
            query.SetString("Subject", subject)
        End If

        result = query.List(Of UnifiedDiary)()
        Return result
    End Function

    Public Function GetLogDetailsByEntity(ByVal type As Integer, ByVal fromDate As Date, ByVal toDate As Date, ByVal userName As String, ByVal year As Integer, ByVal number As Integer?, udsId As Guid?) As IList(Of UnifiedDiary)
        Dim result As IList(Of UnifiedDiary)
        Dim query As IQuery = NHibernateSession.GetNamedQuery("GetUnifiedDiaryDetails")

        'set parameters
        query.SetInt32("IdTipologia", type)
        query.SetDateTime("DataDal", fromDate)
        query.SetDateTime("DataAl", toDate)
        query.SetAnsiString("User", userName)

        query.SetInt32("Riferimento1", year)
        query.SetParameter(Of Integer?)("Riferimento2", Nothing)
        query.SetParameter(Of Guid?)("Riferimento3", Nothing)
        If type > 100 AndAlso udsId.HasValue Then
            query.SetGuid("Riferimento3", udsId.Value)
        End If
        If number.HasValue Then
                query.SetInt32("Riferimento2", number.Value)
            End If


            result = query.List(Of UnifiedDiary)()
        Return result
    End Function
#End Region
End Class
