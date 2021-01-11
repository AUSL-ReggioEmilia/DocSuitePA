
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class TableDocTypeFacade
    Inherits CommonFacade(Of DocumentType, Integer, NHibernateDocumentTypeDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

    Public Function GetMaxId() As Integer
        Return _dao.GetMaxId()
    End Function

    Public Overrides Function IsUsed(ByRef entity As DocumentType) As Boolean

        Dim protfacade As New ProtocolFacade

        If protfacade.IsUsedDocType(entity).Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Function DocTypeSearch(ByVal idDocType As Integer, ByVal onlyIsActive As Boolean, ByVal packageEnabled As Boolean, ByVal description As String) As IList(Of DocumentType)
        Return _dao.DocTypeSearch(idDocType, onlyIsActive, packageEnabled, Description)
    End Function

    Public Function GetByCode(code As String) As DocumentType
        Return Me._dao.GetByCode(code)
    End Function

End Class