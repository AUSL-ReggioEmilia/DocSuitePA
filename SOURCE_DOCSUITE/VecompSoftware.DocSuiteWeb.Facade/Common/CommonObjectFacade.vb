Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class CommonObjectFacade
    Inherits FacadeNHibernateBase(Of CommonObject, Integer, NHibernateObjectDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
        _dbName = DbName
    End Sub

    Public Overrides Sub Save(ByRef obj As CommonObject)
        Dim _id As Integer = _dao.GetMaxId()

        If _id <> 0 Then
            obj.Id = _id + 1
            MyBase.Save(obj)
        End If

    End Sub

    Public Overrides Sub Save(ByRef obj As CommonObject, ByVal dbName As String, Optional needTransaction As Boolean = True)
        _dao.ConnectionName = dbName
        Dim _id As Integer = _dao.GetMaxId()

        obj.Id = _id + 1
        MyBase.Save(obj, dbName, needTransaction:=needTransaction)
    End Sub
    Public Function GetObjectByDescritpion(ByVal description As String, ByVal type As NHibernateObjectDao.DescriptionSearchType, ByVal code As String) As IList(Of CommonObject)
        Return _dao.GetObjectByDescription(description, type, code)
    End Function

End Class