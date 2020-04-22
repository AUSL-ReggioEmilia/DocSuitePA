Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class LocationFacade
    Inherits CommonFacade(Of Location, Integer, NHibernateLocationDao)
    Private _defaultDocumentServer As String = Nothing
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

    Public ReadOnly Property DefaultDocumentServer() As String
        Get
            If _defaultDocumentServer Is Nothing Then
                _defaultDocumentServer = _dao.GetDefaultDocumentServer()
            End If
            Return _defaultDocumentServer
        End Get
    End Property
End Class