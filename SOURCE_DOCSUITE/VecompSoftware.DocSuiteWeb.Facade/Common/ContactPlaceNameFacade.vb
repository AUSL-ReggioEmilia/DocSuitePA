Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ContactPlaceNameFacade
    Inherits CommonFacade(Of ContactPlaceName, Integer, NHibernateContactPlaceNameDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

    Public Function GetByDescription(ByVal Description As String) As IList(Of ContactPlaceName)
        Return _dao.GetByDescription(Description)
    End Function

End Class