Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class PackageFacade
    Inherits FacadeNHibernateBase(Of Package, PackageCompositeKey, NHibernatePackageDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overloads Sub Save(ByRef obj As Package)
        MyBase.Save(obj)
    End Sub

    Public Overloads Function GetById(ByVal origin As Char, ByVal package As Integer, Optional ByVal shoudLock As Boolean = False) As Package
        Dim id As PackageCompositeKey = New PackageCompositeKey(origin, package)
        Return GetById(id, shoudLock)
    End Function

    Public Overloads Function GetAll() As List(Of Package)
        Return MyBase.GetAll()
    End Function

    Public Function GetByAccount(ByVal account As String, ByVal filter As String) As IList(Of Package)
        Return _dao.GetByAccount(account, filter)
    End Function

    Public Function GetOrigin() As ArrayList
        Return _dao.GetOrigin()
    End Function

    Public Function GetMaxID(ByVal origin As Char) As Integer
        Return _dao.GetMaxId(origin)
    End Function

    Public Sub ChangePackage(ByVal actualPackage As Integer, ByVal nextPackage As Integer, ByVal origin As Char)
        _dao.ChangePackage(actualPackage, nextPackage, origin)
    End Sub

    Public Sub ChangeLot(ByVal actualPackage As Integer, ByVal actualOrigin As Char)
        _dao.ChangeLot(actualPackage, actualOrigin)
    End Sub

    Public Function VerifyPackage(ByVal actualPackage As Integer, ByVal connectedUser As String) As Integer
        Return _dao.VerifyPackage(actualPackage, connectedUser)
    End Function

    Function GetPackageDocumentByUser(ByVal user As String) As IList(Of Package)
        Return _dao.GetPackageDocumentByUser(user)
    End Function

    Function CheckPackageExistence(ByVal actualOrigin As Char, ByVal actualPackage As Integer, ByVal actualLot As Integer, ByVal actualIncremental As Integer) As Boolean
        Dim advancedDao As New NHibernateAdvancedProtocolDao(_dbName)
        Dim result As Object = advancedDao.CheckPackageExistence(actualOrigin, actualPackage, actualLot, actualIncremental)

        Return result IsNot Nothing
    End Function

    Sub UpdatePackage(ByVal origin As Char, ByVal package As Integer, ByVal incremental As Integer)
        Dim advancedDao As New NHibernateAdvancedProtocolDao(_dbName)
        Dim packageObj As Package = GetById(origin, package)
        If packageObj IsNot Nothing Then
            packageObj.Incremental = incremental
            packageObj.TotalIncremental = advancedDao.GetCountPackage(origin, package)
            _dao.UpdateWithoutTransaction(packageObj)
        End If
    End Sub

    Function GetNHibernateDomainObjectFinder(ByVal dbType As String) As NHibernateDomainObjectFinder(Of Package)
        Return New NHibernateDomainObjectFinder(Of Package)(Me.CurrentCriteria, dbType)
    End Function
End Class

