﻿Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class DistributionListFacade
    Inherits CommonFacade(Of DistributionList, Integer, NHibernateDistributionListDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

    Public Function GetAllOrderedByName() As IList(Of DistributionList)
        Return _dao.GetAllOrderedByName()
    End Function
End Class