Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateAdvancedProtocolDao
    Inherits BaseNHibernateDao(Of AdvancedProtocol)

    Public Sub New(ByVal sessionFactoryName As String)
		MyBase.New(sessionFactoryName)
	End Sub

	Public Sub New()
		MyBase.New()
	End Sub

    Public Function GetCountByCategory(ByVal Category As Category) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Category", Category))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)()
    End Function

    Public Function GetCountPackage(ByVal origin As Char, ByVal package As Integer) As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("PackageOrigin", origin))
        criteria.Add(Restrictions.Eq("Package", package))
        criteria.SetProjection(Projections.Count("Package"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Function CheckPackageExistence(ByVal actualorigin As Char, ByVal actualPackage As Integer, ByVal actualLot As Integer, ByVal actualIncremental As Integer) As Object
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("PackageOrigin", actualorigin))
        criteria.Add(Restrictions.Eq("Package", actualPackage))
        criteria.Add(Restrictions.Eq("PackageLot", actualLot))
        criteria.Add(Restrictions.Eq("PackageIncremental", actualIncremental))

        criteria.SetProjection(Projections.Property("Year"))

        Return criteria.UniqueResult()
    End Function

    Function SearchInvoiceAccountingDouble(ByVal idContainer As Integer, ByVal accountingSectional As String, ByVal accountinYear As Short, ByVal accountingNumber As Integer) As IList(Of AdvancedProtocol)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Protocol", "Protocol", SqlCommand.JoinType.LeftOuterJoin)
        If String.IsNullOrEmpty(accountingSectional) Then
            criteria.Add(Restrictions.IsNull("AccountingSectional"))
        Else
            criteria.Add(Expression.Like("AccountingSectional", accountingSectional))
        End If
        criteria.Add(Restrictions.Eq("AccountingYear", accountinYear))
        criteria.Add(Restrictions.Eq("AccountingNumber", accountingNumber))
        criteria.Add(Restrictions.Eq("Protocol.Container.Id", idContainer))
        criteria.Add(Restrictions.Eq("Protocol.IdStatus", 0))

        Return criteria.List(Of AdvancedProtocol)()
    End Function
End Class
