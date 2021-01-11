Imports VecompSoftware.NHibernateManager.Config
Imports NHibernate
Imports System.Configuration

Public Class DocSuiteInterceptor
    Inherits EmptyInterceptor

    Public Overrides Sub OnDelete(ByVal entity As Object, ByVal id As Object, ByVal state() As Object, ByVal propertyNames() As String, ByVal types() As NHibernate.Type.IType)
        MyBase.OnDelete(entity, id, state, propertyNames, types)
    End Sub

    Public Overrides Function OnSave(ByVal entity As Object, ByVal id As Object, ByVal state() As Object, ByVal propertyNames() As String, ByVal types() As NHibernate.Type.IType) As Boolean
        
        Return MyBase.OnSave(entity, id, state, propertyNames, types)

    End Function


    Public Overrides Sub PostFlush(ByVal entities As ICollection)

        MyBase.PostFlush(entities)
    End Sub

    Protected ReadOnly Property DataBaseConfig() As OpenSessionInViewSection
        Get
            Dim openSessionInViewSection As OpenSessionInViewSection = TryCast(ConfigurationManager.GetSection("nhibernateSettings"), OpenSessionInViewSection)
            If openSessionInViewSection Is Nothing Then
                Throw New DocSuiteException("Configurazione NHibernate") With {.Descrizione = "Impossibile trovare la sezione nhibernateSettings nel ConfigurationManager."}
            End If
            Return openSessionInViewSection
        End Get
    End Property


End Class
