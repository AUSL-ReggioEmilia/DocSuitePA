Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ContactTitleFacade
    Inherits CommonFacade(Of ContactTitle, Integer, NHibernateContactTitleDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

    Public Function GetMaxId() As Integer
        Return _dao.GetMaxId()
    End Function

    Public Overrides Function IsUsed(ByRef obj As ContactTitle) As Boolean

        If _dao.ContactTitleUsedProtocol(obj) Then
            Return True
        End If

        If DocSuiteContext.Current.IsDocumentEnabled Then
            If _dao.ContactTitleUsedDocument(obj) Then
                Return True
            End If
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            If _dao.ContactTitleUsedResolution(obj) Then
                Return True
            End If
        End If

        Return False
    End Function

    Public Overrides Function Delete(ByRef obj As ContactTitle) As Boolean

        If obj.GetType().GetInterface("ISupportLogicDelete") IsNot Nothing And IsUsed(obj) Then
            obj.IsActive = 0
            MyBase.UpdateOnly(obj)
        Else
            MyBase.Delete(obj)
        End If

    End Function

    Public Function GetByDescription(ByVal Description As String) As IList(Of ContactTitle)
        Return _dao.GetByDescription(Description)
    End Function
End Class
