Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ServiceCategoryFacade
    Inherits FacadeNHibernateBase(Of ServiceCategory, Integer, NHibernateServiceCategoryDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
        _dbName = DbName
    End Sub

    Public Function GetObjectByDescritpion(ByVal Description As String, ByVal type As NHibernateServiceCategoryDao.DescriptionSearchType, ByVal Code As String) As IList(Of ServiceCategory)
        Return _dao.GetObjectByDescription(Description, type, Code)
    End Function

End Class