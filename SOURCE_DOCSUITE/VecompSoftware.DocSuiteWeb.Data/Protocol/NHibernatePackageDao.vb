Imports System
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Transformer
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePackageDao
    Inherits BaseNHibernateDao(Of Package)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetByAccount(ByVal account As String, ByVal filter As String) As IList(Of Package)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        If Not String.IsNullOrEmpty(account) Then
            criteria.Add(Restrictions.Eq("Account", account))
        End If
        If Not String.IsNullOrEmpty(filter) And filter IsNot Nothing Then
            criteria.Add(Restrictions.Eq("Origin", Convert.ToChar(filter)))
        End If

        Return criteria.List(Of Package)()
    End Function

    Public Function GetOrigin() As ArrayList
        Dim query As String = "select package.Origin from Package package group by package.Origin"
        Try
            Return CType(NHibernateSession.CreateQuery(query).List(), ArrayList)
        Catch ex As Exception
            Dim vList As ArrayList = New ArrayList
            vList.Add(String.Empty)
            Return vList
        End Try
    End Function

    Public Function GetMaxId(ByVal origin As Char) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Origin", origin))
        criteria.SetProjection(Projections.Max("Package"))
        Dim newPackage As Object = criteria.UniqueResult()
        If newPackage Is Nothing Then
            Return 0
        End If

        Return DirectCast(newPackage, Integer) + 1

        Dim pkg As Package = criteria.UniqueResult(Of Package)()

        Dim query As String = "SELECT Max(P.Package) FROM Package AS P WHERE P.Origin = '" & origin & "'"
        Try
            Return NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)() + 1
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Sub ChangePackage(ByVal actualPackage As Integer, ByVal nextPackage As Integer, ByVal origin As Char)
        Using transaction As ITransaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted)
            Try
                Dim actualItem As Package = GetById(New PackageCompositeKey(origin, actualPackage), True)
                actualItem.State = "C"c
                UpdateOnlyWithoutTransaction(actualItem)

                Dim nextItem As Package = GetById(New PackageCompositeKey(origin, nextPackage), True)
                nextItem.State = "A"c
                nextItem.Incremental = 0
                UpdateOnlyWithoutTransaction(nextItem)

                transaction.Commit()

            Catch ex As Exception
                transaction.Rollback()
                Throw New DocSuiteException("Errore gestione scatolone", String.Format("Impossibile aggiornare scatolone tipo [{0}]: attuale [{1}], prossimo [{2}].", origin, actualPackage, nextPackage), ex)
            End Try
        End Using
    End Sub

    Sub ChangeLot(ByVal actualPackage As Integer, ByVal actualOrigin As Char)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Package", actualPackage))
        criteria.Add(Restrictions.Eq("Origin", actualOrigin))
        criteria.Add(Expression.LtProperty("TotalIncremental", "MaxDocuments"))

        Dim pkg As Package = criteria.UniqueResult(Of Package)()
        pkg.Lot += 1
        pkg.Incremental = 0

        UpdateOnly(pkg)
    End Sub

    Function VerifyPackage(ByVal actualPackage As Integer, ByVal connectedUser As String) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Package", actualPackage))

        Dim pkgs As IList(Of Package) = criteria.List(Of Package)()
        If pkgs.IsNullOrEmpty() Then
            Return -1
        End If

        Dim criteria1 As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria1.Add(Restrictions.Eq("State", "D"c))
        criteria1.Add(Restrictions.Eq("Account", connectedUser))
        criteria1.SetProjection(Projections.Property("Package"))
        criteria1.AddOrder(Order.Asc("Package"))

        criteria1.SetResultTransformer(New TopRecordsResultTransformer(1))

        Return criteria1.UniqueResult(Of Integer)()
    End Function

    Function GetPackageDocumentByUser(ByVal user As String) As IList(Of Package)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("State", "A"c))
        criteria.Add(Restrictions.Eq("Account", user))

        Return criteria.List(Of Package)()

    End Function

#End Region

End Class

