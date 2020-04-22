Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager

Public Class NHMessageLogFinder
    Inherits NHibernateBaseFinder(Of MessageLog, MessageLog)

#Region " Properties "

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property EnablePaging As Boolean

    Public Property MessageIn As IList(Of DSWMessage)

#End Region

#Region " Constructors "

    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
    End Sub

    Public Sub New()
        Me.New(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
    End Sub

#End Region

#Region " Methods "

    Public Overloads Overrides Function DoSearch() As IList(Of MessageLog)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        AttachFilterExpressions(criteria)

        Return criteria.List(Of MessageLog)()
    End Function

    Protected Overrides Function CreateCriteria() As ICriteria
        Return NHibernateSession.CreateCriteria(Of MessageLog)("ML")
    End Function

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)
        If Not MessageIn.IsNullOrEmpty() Then
            criteria.Add(Restrictions.In("ML.Message", MessageIn.ToArray()))
        End If
    End Sub

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachFilterExpressions(criteria)
        criteria.SetProjection(Projections.CountDistinct("ML.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

#End Region

End Class
